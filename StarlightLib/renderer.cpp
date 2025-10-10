#include "pch.h"
#include "renderer.h"

void render_entities(Entity* particles, unsigned int particle_count, unsigned int* canvas, unsigned int canvas_w, unsigned int canvas_h) {
	array_view<Entity, 1> entityView(particle_count, particles);
	array_view<unsigned int, 1> canvasView(canvas_w * canvas_h, canvas);

	parallel_for_each(extent<1>(particle_count), [=](index<1> idx) restrict(amp) {
		int i = idx[0];
		Entity e = entityView[i];
		
		if (e.type == Type_Projectile) {
			render_circle(e, canvasView, canvas_w, canvas_h);
		}
	});
}

void render_circle(Entity e, array_view<unsigned int, 1> canvasView, unsigned int canvas_w, unsigned int canvas_h) restrict(amp) {
	int radius = e.scale;
	for (int y = -radius; y <= radius; y++) {
		for (int x = -radius; x <= radius; x++) {
			if (x * x + y * y <= radius * radius) {
				int px = e.x + x;
				int py = e.y + y;
				if (px >= 0 && px < canvas_w && py >= 0 && py < canvas_h) {
					canvasView[py * canvas_w + px] = 0xFFFFFFFF;
				}
			}
		}
	}
}