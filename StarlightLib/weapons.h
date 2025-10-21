#pragma once

#include "pch.h"
#include "simulation.h"

// Reusable weapon/targeting utilities for both host and AMP device code.
// All functions marked restrict(amp) so they can be called inside AMP kernels.

// Normalize an angle difference to [-180, 180]
inline float normalize_angle_deg(float a) restrict(amp)
{
    while (a > 180.0f) a -= 360.0f;
    while (a < -180.0f) a += 360.0f;
    return a;
}

// Turn current angle towards target by at most maxDelta degrees
inline float turn_towards_deg(float current, float target, float maxDelta) restrict(amp)
{
    float diff = normalize_angle_deg(target - current);
    if (fabs(diff) <= maxDelta)
        return target;
    return current + (diff > 0.0f ? 1.0f : -1.0f) * maxDelta;
}

// Compute angle (degrees) to aim directly at target's current position.
inline float compute_direct_aim_angle(const Entity& shooter, const Entity& target) restrict(amp)
{
    float dx = target.x - shooter.x;
    float dy = target.y - shooter.y;
    return atan2(dx, dy) * 180.0f / 3.141592653589793f;
}

// Compute predictive lead angle (degrees) to intercept target moving at constant velocity
// given the projectile speed (in world units/sec). Falls back to direct aim when no
// positive real intercept time exists.
// Note: This variant assumes projectile absolute speed is `projectile_speed` (world frame)
// and does NOT add shooter velocity (appropriate for guided missiles already in flight).
inline float compute_lead_aim_angle(const Entity& shooter, const Entity& target, float projectile_speed) restrict(amp)
{
    // shooter position (current missile position)
    float sx = shooter.x;
    float sy = shooter.y;

    // target position/velocity
    float tx = target.x;
    float ty = target.y;
    float tvx = target.vx;
    float tvy = target.vy;

    // relative vectors
    float rx = tx - sx;
    float ry = ty - sy;
    float vxRel = tvx; // do not subtract shooter velocity for guided missile intercept
    float vyRel = tvy;

    // Quadratic: (v·v - s^2)t^2 + 2(r·v)t + r·r = 0
    float s2 = projectile_speed * projectile_speed;
    float a = (vxRel * vxRel + vyRel * vyRel) - s2;
    float b = 2.0f * (rx * vxRel + ry * vyRel);
    float c = (rx * rx + ry * ry);

    float tIntercept = -1.0f;
    const float EPS = 1e-5f;

    if (fabs(a) < EPS)
    {
        if (fabs(b) > EPS)
        {
            float t = -c / b;
            if (t > 0.0f) tIntercept = t;
        }
    }
    else
    {
        float disc = b * b - 4.0f * a * c;
        if (disc >= 0.0f)
        {
            float sqrtDisc = sqrt(disc);
            float t1 = (-b - sqrtDisc) / (2.0f * a);
            float t2 = (-b + sqrtDisc) / (2.0f * a);

            if (t1 > EPS && t2 > EPS)
                tIntercept = t1 < t2 ? t1 : t2;
            else if (t1 > EPS)
                tIntercept = t1;
            else if (t2 > EPS)
                tIntercept = t2;
        }
    }

    if (tIntercept > 0.0f)
    {
        float ix = tx + tvx * tIntercept;
        float iy = ty + tvy * tIntercept;
        float dx = ix - sx;
        float dy = iy - sy;
        return atan2(dx, dy) * 180.0f / 3.141592653589793f;
    }

    // Fallback to current position
    return compute_direct_aim_angle(shooter, target);
}

// Predictive lead angle for a missile with acceleration (non-constant speed).
// Uses fixed-point iterations matching missile travel distance s(t) to target distance d(t).
// v0: current missile speed, accel: forward acceleration magnitude, vmax: speed clamp.
inline float compute_lead_aim_angle_accel(const Entity& shooter, const Entity& target, float v0, float accel, float vmax) restrict(amp)
{
    // Shooter and target state
    const float sx = shooter.x;
    const float sy = shooter.y;
    const float tx0 = target.x;
    const float ty0 = target.y;
    const float tvx = target.vx;
    const float tvy = target.vy;

    // Initial guess for time-to-go
    const float minSpeed = 50.0f;
    float vInit = v0 > minSpeed ? v0 : minSpeed;
    float dx0 = tx0 - sx;
    float dy0 = ty0 - sy;
    float dist0 = sqrt(dx0 * dx0 + dy0 * dy0);
    float t = (vInit > 1e-4f) ? (dist0 / vInit) : 0.0f;

    // Piecewise distance for accelerating missile up to vmax
    auto missile_distance = [=](float tt) restrict(amp) -> float {
        if (accel <= 0.0f) {
            return vInit * tt;
        }
        float t_to_vmax = (vmax > vInit) ? ((vmax - vInit) / accel) : 0.0f;
        if (tt <= t_to_vmax) {
            return vInit * tt + 0.5f * accel * tt * tt;
        }
        float s_acc = vInit * t_to_vmax + 0.5f * accel * t_to_vmax * t_to_vmax;
        return s_acc + vmax * (tt - t_to_vmax);
    };

    const int ITER = 6;
    for (int i = 0; i < ITER; ++i)
    {
        float tx = tx0 + tvx * t;
        float ty = ty0 + tvy * t;
        float dx = tx - sx;
        float dy = ty - sy;
        float d = sqrt(dx * dx + dy * dy);

        float s = missile_distance(t);
        float vAvg = (t > 1e-4f) ? (s / t) : vInit;
        if (vAvg < minSpeed) vAvg = minSpeed;

        float tNew = (vAvg > 1e-4f) ? (d / vAvg) : t;
        t = 0.5f * t + 0.5f * tNew;

        if (fabs(tNew - t) < 1e-3f)
            break;
    }

    float ix = tx0 + tvx * t;
    float iy = ty0 + tvy * t;
    float dx = ix - sx;
    float dy = iy - sy;
    return atan2(dx, dy) * 180.0f / 3.141592653589793f;
}

// Compute a steering heading that accounts for current momentum by aiming along
// the velocity error vector (v_desired - v_current). The nose points in the direction
// where acceleration should act to reduce velocity error.
inline float compute_momentum_adjusted_heading(float desiredHeadingDeg, float desiredSpeed, float currVx, float currVy) restrict(amp)
{
    const float PI = 3.141592653589793f;
    float rad = desiredHeadingDeg * PI / 180.0f;
    float hx = sin(rad);
    float hy = cos(rad);
    float vdx = hx * desiredSpeed;
    float vdy = hy * desiredSpeed;
    float ex = vdx - currVx;
    float ey = vdy - currVy;
    // If the error is tiny, just use desired heading
    if (ex * ex + ey * ey < 1e-6f)
        return desiredHeadingDeg;
    return atan2(ex, ey) * 180.0f / 3.141592653589793f;
}
