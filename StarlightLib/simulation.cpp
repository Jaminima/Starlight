#include "pch.h"
#include "simulation.h"

void update_enemy(Entity& e, const Entity& player, float dt) restrict(amp) {
    // Gradually turn towards the player and accelerate forward, slowing down on final approach
    float dx = player.x - e.x;
    float dy = player.y - e.y;
    float dist = sqrt(dx * dx + dy * dy);

    float targetAngle = atan2(dx, dy) * 180.0f / 3.141592653589793f;

    float angleDiff = targetAngle - e.rotation;
    while (angleDiff > 180.0f) angleDiff -= 360.0f;
    while (angleDiff < -180.0f) angleDiff += 360.0f;

    float turnSpeed = 90.0f; // degrees per second
    float turnAmount = turnSpeed * dt;
    if (fabs(angleDiff) < turnAmount) {
        e.rotation = targetAngle;
    }
    else {
        e.rotation += (angleDiff > 0.0f ? 1.0f : -1.0f) * turnAmount;
    }

    // Accelerate forward in the direction facing
    float rad = e.rotation * 3.141592653589793f / 180.0f;
    float accel = 10.0f; // acceleration forward
    e.vx += sin(rad) * accel * dt;
    e.vy += cos(rad) * accel * dt;

    // Slow down on final approach
    if (dist < 500.0f) {
        float speed = sqrt(e.vx * e.vx + e.vy * e.vy);
        if (speed > 0.0f) {
            float decel = 50.0f; // deceleration rate
            e.vx -= (e.vx / speed) * decel * dt;
            e.vy -= (e.vy / speed) * decel * dt;
        }

        if (dist < 400.0f) {
            e.queuedEvent = EntityEvent::Event_FireMissile;
        }
    }
}

void update_missile(Entity& e, const Entity& player, float dt) restrict(amp) {
    // Calculate predicted position of the player
    float dx = player.x - e.x;
    float dy = player.y - e.y;
    float rvx = player.vx - e.vx;
    float rvy = player.vy - e.vy;
    float missile_speed = 500.0f; // assumed constant speed for prediction
    float a = rvx * rvx + rvy * rvy - missile_speed * missile_speed;
    float b = 2.0f * (dx * rvx + dy * rvy);
    float c = dx * dx + dy * dy;
    float disc = b * b - 4.0f * a * c;
    if (disc > 0.0f) {
        float sqrt_disc = sqrt(disc);
        float t1 = (-b - sqrt_disc) / (2.0f * a);
        float t2 = (-b + sqrt_disc) / (2.0f * a);
        float t = (t1 > 0.0f) ? t1 : t2;
        if (t > 0.0f) {
            float pred_x = player.x + player.vx * t;
            float pred_y = player.y + player.vy * t;
            dx = pred_x - e.x;
            dy = pred_y - e.y;
        }
    }

    float dist = sqrt(dx * dx + dy * dy);

    float targetAngle = atan2(dx, dy) * 180.0f / 3.141592653589793f;

    float angleDiff = targetAngle - e.rotation;
    while (angleDiff > 180.0f) angleDiff -= 360.0f;
    while (angleDiff < -180.0f) angleDiff += 360.0f;

    float turnSpeed = 90.0f; // degrees per second
    float turnAmount = turnSpeed * dt;
    if (fabs(angleDiff) < turnAmount) {
        e.rotation = targetAngle;
    }
    else {
        e.rotation += (angleDiff > 0.0f ? 1.0f : -1.0f) * turnAmount;
    }

    // Accelerate forward in the direction facing
    float rad = e.rotation * 3.141592653589793f / 180.0f;
    float accel = 500.0f; // acceleration forward
    e.vx += sin(rad) * accel * dt;
    e.vy += cos(rad) * accel * dt;
}

void _stdcall update_entities(Entity* entities, int count, float dt) {
	Entity player = entities[0];
	array_view<Entity, 1> view(count, entities);
	parallel_for_each(view.extent, [=](index<1> idx) restrict(amp) {
		Entity e = view[idx];

		e.x += e.vx * dt;
		e.y += e.vy * dt;

		switch (e.type) {
		case EntityType::Type_Enemy:
			update_enemy(e, player, dt);
			break;
			
		case EntityType::Type_Missile:
			update_missile(e, player, dt);
			break;
		}

		// Cap max speed
		//float max_speed = 300.0f; // maximum speed

		//switch (e.type)
		//{
		//	case EntityType::Type_Cannon:
		//	case EntityType::Type_Missile:
		//		max_speed = 500.0f;
		//}

		//float current_speed = sqrt(e.vx * e.vx + e.vy * e.vy);
		//if (current_speed > max_speed) {
		//	e.vx = (e.vx / current_speed) * max_speed;
		//	e.vy = (e.vy / current_speed) * max_speed;
		//}

		view[idx] = e;
	});
	view.synchronize();
}
