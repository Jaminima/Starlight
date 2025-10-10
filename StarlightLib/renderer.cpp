#include "pch.h"
#include "renderer.h"

void render_entities(Entity* particles, int particle_count, unsigned int* canvas, int canvas_w, int canvas_h) {
	array_view<Entity, 1> entityView(particle_count, particles);
	array_view<unsigned int, 1> canvasView(canvas_w * canvas_h, canvas);

	parallel_for_each(extent<2>(canvas_h, canvas_w), [=](index<2> idx) restrict(amp) {
		int y = idx[0];
		int x = idx[1];
		int pixelIndex = y * canvas_w + x;
		unsigned int rgba = 0xFF000000; // Default to black pixel
		for (int i = 0; i < particle_count; i++) {
			Entity e = entityView[i];
			int px = static_cast<int>(e.x);
			int py = static_cast<int>(e.y);
			if (px == x && py == y) {
				rgba = 0xFFFFFFFF; // White pixel for particle
				break;
			}
		}
		canvasView[pixelIndex] = rgba;
		});
}