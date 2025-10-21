#include "pch.h"
#include "simulation.h"
#include "weapons.h"

void update_enemy(Entity& e, const Entity& player, float dt) restrict(amp) {
    // Gradually turn towards the player and accelerate forward, slowing down on final approach
    float dx = player.x - e.x;
    float dy = player.y - e.y;
    float dist = sqrt(dx * dx + dy * dy);

    float targetAngle = compute_direct_aim_angle(e, player);

    // Turn towards target with a max turn speed
    const float turnSpeed = 90.0f; // degrees per second
    const float turnAmount = turnSpeed * dt;
    // turn_towards_deg implemented in weapons.h
    e.rotation = turn_towards_deg(e.rotation, targetAngle, turnAmount);

    // Accelerate forward in the direction facing
    float rad = e.rotation * 3.141592653589793f / 180.0f;
    float accel = 70.0f; // acceleration forward
    e.vx += sin(rad) * accel * dt;
    e.vy += cos(rad) * accel * dt;

    // Slow down on final approach
    if (dist < 900.0f) {
        float speed = sqrt(e.vx * e.vx + e.vy * e.vy);
        if (speed > 0.0f) {
            float decel = 50.0f; // deceleration rate
            e.vx -= (e.vx / speed) * decel * dt;
            e.vy -= (e.vy / speed) * decel * dt;
        }
    }

    // Events
   /* if (dist < 400.0f) {
        e.queuedEvent = EntityEvent::Event_FireCannon;
    }*/
    if (dist < 700.0f) {
        e.queuedEvent = EntityEvent::Event_FireMissile;
    }
    /*else {
        e.queuedEvent = EntityEvent::Event_Shields;
    }*/
}

inline Entity get_target_or_default(const array_view<Entity,1>& view, const Entity& self, const Entity& fallback) restrict(amp)
{
    if (self.targetIndex >= 0 && self.targetIndex < (int)view.extent[0])
    {
        return view[self.targetIndex];
    }
    return fallback;
}

void update_missile(Entity& e, const Entity& target, float dt) restrict(amp) {
    // Current missile speed and kinematics parameters
    float currSpeed = sqrt(e.vx * e.vx + e.vy * e.vy);
    const float accel = 200.0f;  // forward acceleration (units/s^2)
    const float maxSpeed = 700.0f; // design maximum speed

    // Predict an intercept heading that accounts for missile acceleration and speed clamp
    float leadHeading = compute_lead_aim_angle_accel(e, target, currSpeed, accel, maxSpeed);

    // Choose a desired speed for the guidance law (accelerating towards max)
    float desiredSpeed = currSpeed + accel * dt;
    if (desiredSpeed > maxSpeed) desiredSpeed = maxSpeed;

    // Compute a momentum-aware heading that points the nose to reduce velocity error
    float steerHeading = compute_momentum_adjusted_heading(leadHeading, desiredSpeed, e.vx, e.vy);

    // Turn towards the momentum-adjusted heading with missile turn speed
    const float turnSpeed = 240.0f; // degrees per second
    const float turnAmount = turnSpeed * dt;
    e.rotation = turn_towards_deg(e.rotation, steerHeading, turnAmount);

    // Accelerate forward in the direction facing
    float rad = e.rotation * 3.141592653589793f / 180.0f;
    e.vx += sin(rad) * accel * dt;
    e.vy += cos(rad) * accel * dt;

    // Optional: clamp to max speed to avoid uncontrollable dynamics
    float newSpeed = sqrt(e.vx * e.vx + e.vy * e.vy);
    if (newSpeed > maxSpeed)
    {
        float scale = maxSpeed / newSpeed;
        e.vx *= scale;
        e.vy *= scale;
    }
}

void _stdcall update_entities(Entity* entities, int count, float dt) {
	Entity player = entities[0];
	array_view<Entity, 1> view(count, entities);
	parallel_for_each(view.extent, [=](index<1> idx) restrict(amp) {
		Entity e = view[idx];

		// Integrate position
		e.x += e.vx * dt;
		e.y += e.vy * dt;

		// Per-type update
		switch (e.type) {
		case EntityType::Type_Enemy:
			update_enemy(e, player, dt);
			break;
			
		case EntityType::Type_Missile:
        {
            // If missile has a target index, use that entity otherwise fallback to player
            const Entity tgt = get_target_or_default(view, e, player);
 			update_missile(e, tgt, dt);
             break;
         }

		case EntityType::Type_Cannon:
			// Cannons fly straight, no update needed
			break;
		}

        // Collision tests
        if (e.type == EntityType::Type_Missile || e.type == EntityType::Type_Cannon)
        {
            // Collision against player (existing)
            const Entity p = player;
            const bool shieldActive = p.lastEvent == EntityEvent::Event_Shields && p.eventTime + 5.0f > p.timeAlive;
            const float playerRadius = p.scale * 1.5f;
            const float shieldRadius = p.scale * 3.0f;

            float d2p = dist2(e.x, e.y, p.x, p.y);
            float rProj = e.scale;

            bool hitPlayer = d2p <= (playerRadius + rProj) * (playerRadius + rProj);
            bool hitShield = false;
            if (!hitPlayer && shieldActive)
            {
                hitShield = d2p <= (shieldRadius + rProj) * (shieldRadius + rProj);
            }

            // Collision against enemies for player-fired missiles: check targetIndex when set
            bool hitEnemy = false;
            if (!hitPlayer /*&& e.type == EntityType::Type_Missile*/ && e.targetIndex > 0 && e.targetIndex < (int)view.extent[0])
            {
                const Entity tgt = view[e.targetIndex];
                if (tgt.type == EntityType::Type_Enemy)
                {
                    float d2e = dist2(e.x, e.y, tgt.x, tgt.y);
                    float rEnemy = tgt.scale;
                    hitEnemy = d2e <= (rEnemy + rProj) * (rEnemy + rProj);
                }
            }

            if (hitPlayer || hitShield || hitEnemy)
            {
                // Flag explosion and expire missile quickly via TTL
                e.queuedEvent = EntityEvent::Event_Explosion;
                e.timeToLive = 0.05f; // let host sweep it
            }
        }

		view[idx] = e;
	});
	view.synchronize();
}
