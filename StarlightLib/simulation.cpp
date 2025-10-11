#include "pch.h"
#include "simulation.h"

void _stdcall update_entities(Entity* entities, int count, float dt) {
	Entity player = entities[0];
	array_view<Entity, 1> view(count, entities);
	parallel_for_each(view.extent, [=](index<1> idx) restrict(amp) {
		Entity e = view[idx];

		e.x += e.vx * dt;
		e.y += e.vy * dt;

		switch (e.type) {
		case EntityType::Type_Enemy:
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
			} else {
				e.rotation += (angleDiff > 0.0f ? 1.0f : -1.0f) * turnAmount;
			}

			// Accelerate forward in the direction facing
			float rad = e.rotation * 3.141592653589793f / 180.0f;
			float accel = 100.0f; // acceleration forward
			e.vx += sin(rad) * accel * dt;
			e.vy += cos(rad) * accel * dt;

			// Slow down on final approach
			if (dist < 200.0f) {
				float speed = sqrt(e.vx * e.vx + e.vy * e.vy);
				if (speed > 0.0f) {
					float decel = 50.0f; // deceleration rate
					e.vx -= (e.vx / speed) * decel * dt;
					e.vy -= (e.vy / speed) * decel * dt;
				}
			}

			// Cap max speed
			float max_speed = 300.0f; // maximum speed
			float current_speed = sqrt(e.vx * e.vx + e.vy * e.vy);
			if (current_speed > max_speed) {
				e.vx = (e.vx / current_speed) * max_speed;
				e.vy = (e.vy / current_speed) * max_speed;
			}

			break;
		}

		view[idx] = e;
	});
	view.synchronize();
}
