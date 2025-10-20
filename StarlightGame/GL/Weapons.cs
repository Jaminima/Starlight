using StarlightGame.StarlightLib;
using StarlightGame.StarlightLib.Types;
using System;

namespace StarlightGame.GL
{
    internal static class Weapons
    {
        // Select a target index for the shooter
        internal static int ResolveTargetIndex(Scene scene, Entity shooter)
        {
            return shooter.Type == EntityType.Player ? scene.CurrentTargetIndex : 0;
        }

        // Angle toward the target's current position
        internal static float ComputeDirectAimAngle(in Entity shooter, in Entity target)
        {
            float dx = target.X - shooter.X;
            float dy = target.Y - shooter.Y;
            return (float)Math.Atan2(dx, dy);
        }

        // Predictive lead angle for constant-velocity interception
        internal static float ComputeLeadAimAngle(in Entity shooter, in Entity target, float projectileSpeed)
        {
            // shooter position/velocity
            float sx = shooter.X;
            float sy = shooter.Y;
            float svx = shooter.VX;
            float svy = shooter.VY;

            // target position/velocity
            float tx = target.X;
            float ty = target.Y;
            float tvx = target.VX;
            float tvy = target.VY;

            // relative vectors
            float rx = tx - sx;
            float ry = ty - sy;
            float vxRel = tvx - svx;
            float vyRel = tvy - svy;

            // Quadratic: (v·v - s^2)t^2 + 2(r·v)t + r·r = 0
            float a = (vxRel * vxRel + vyRel * vyRel) - (projectileSpeed * projectileSpeed);
            float b = 2.0f * (rx * vxRel + ry * vyRel);
            float c = (rx * rx + ry * ry);

            float tIntercept = -1.0f;
            const float EPS = 1e-5f;
            if (Math.Abs(a) < EPS)
            {
                if (Math.Abs(b) > EPS)
                {
                    float t = -c / b;
                    if (t > 0) tIntercept = t;
                }
            }
            else
            {
                float disc = b * b - 4.0f * a * c;
                if (disc >= 0.0f)
                {
                    float sqrtDisc = (float)Math.Sqrt(disc);
                    float t1 = (-b - sqrtDisc) / (2.0f * a);
                    float t2 = (-b + sqrtDisc) / (2.0f * a);

                    if (t1 > EPS && t2 > EPS)
                        tIntercept = Math.Min(t1, t2);
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
                return (float)Math.Atan2(dx, dy);
            }

            // Fallback to current position
            return ComputeDirectAimAngle(shooter, target);
        }
    }
}
