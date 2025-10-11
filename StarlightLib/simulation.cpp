#include "pch.h"
#include "simulation.h"

void _stdcall update_entities(Entity* entities, int count, float dt) {
	array_view<Entity, 1> view(count, entities);
	parallel_for_each(view.extent, [=](index<1> idx) restrict(amp) {
		Entity e = view[idx];
		e.x += e.vx * dt;
		e.y += e.vy * dt;
		view[idx] = e;
	});
	view.synchronize();
}
