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
			// Steadily turn towards the player
			float dx = player.x - e.x;
			float dy = player.y - e.y;

			float targetAngle = atan2(dx, dy) * 180.0f / 3.141592653589793f;

			float angleDiff = targetAngle - e.rotation;
			while (angleDiff > 180.0f) angleDiff -= 360.0f;
			while (angleDiff < -180.0f) angleDiff += 360.0f;

			float turnSpeed = 90.0f; // degrees per second
			float turnAmount = turnSpeed * dt;
			if (fabsf(angleDiff) < turnAmount) {
				e.rotation = targetAngle;
			} else {
				e.rotation += (angleDiff > 0.0f ? 1.0f : -1.0f) * turnAmount;
			}

			break;
		}

		view[idx] = e;
	});
	view.synchronize();
}
