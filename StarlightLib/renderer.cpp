#include "pch.h"
#include "renderer.h"

void render_entities(Camera* camera, Entity* entities, unsigned int entity_count, unsigned int* canvas, unsigned int canvas_w, unsigned int canvas_h) {
	array_view<Entity, 1> entityView(entity_count, entities);
	array_view<unsigned int, 1> canvasView(canvas_w * canvas_h, canvas);
	Camera cam = *camera;

	parallel_for_each(extent<1>(entity_count), [=](index<1> idx) restrict(amp) {
		int i = idx[0];
		Entity e = entityView[i];
		
		switch (e.type)
		{
			case Type_Cannon:
				render_circle(cam, e, canvasView, canvas_w, canvas_h, 0xFF0000FF * (1.0f - (e.timeAlive / e.timeToLive)));
				break;

			case Type_Missile:
				render_circle(cam, e, canvasView, canvas_w, canvas_h, 0xFFFFFF00);
				break;

			case Type_Player:
				render_triangle(cam, e, canvasView, canvas_w, canvas_h, 0xFFFFFFFF);
				if (e.lastEvent == EntityEvent::Event_Shields && e.eventTime + 5.0f > e.timeAlive) {
					render_circle_outline(cam, e, canvasView, canvas_w, canvas_h, 0xFF00FFFF);
				}
				break;

			case Type_Enemy:
				render_triangle(cam, e, canvasView, canvas_w, canvas_h, 0xFFFF00FF);
				break;
		}
	});
}

void render_circle_outline(Camera camera, Entity e, array_view<unsigned int, 1> canvasView, unsigned int canvas_w, unsigned int canvas_h, unsigned int color) restrict(amp) {
	float scaled_radius = e.scale * camera.zoom * 3;
	int radius = static_cast<int>(scaled_radius + 0.5f);
	float center_x = (e.x - camera.x) * camera.zoom + canvas_w / 2.0f;
	float center_y = (e.y - camera.y) * camera.zoom + canvas_h / 2.0f;
	for (int y = -radius; y <= radius; y++) {
		for (int x = -radius; x <= radius; x++) {
			if (x * x + y * y <= scaled_radius * scaled_radius && x * x + y * y >= (scaled_radius - 1) * (scaled_radius - 1)) {
				int px = static_cast<int>(center_x + x);
				int py = static_cast<int>(center_y + y);
				if (px >= 0 && px < canvas_w && py >= 0 && py < canvas_h) {
					canvasView[py * canvas_w + px] = color;
				}
			}
		}
	}
}

void render_circle(Camera camera, Entity e, array_view<unsigned int, 1> canvasView, unsigned int canvas_w, unsigned int canvas_h, unsigned int color) restrict(amp) {
	float scaled_radius = e.scale * camera.zoom;
	int radius = static_cast<int>(scaled_radius + 0.5f);
	float center_x = (e.x - camera.x) * camera.zoom + canvas_w / 2.0f;
	float center_y = (e.y - camera.y) * camera.zoom + canvas_h / 2.0f;
	for (int y = -radius; y <= radius; y++) {
		for (int x = -radius; x <= radius; x++) {
			if (x * x + y * y <= scaled_radius * scaled_radius) {
				int px = static_cast<int>(center_x + x);
				int py = static_cast<int>(center_y + y);
				if (px >= 0 && px < canvas_w && py >= 0 && py < canvas_h) {
					canvasView[py * canvas_w + px] = color;
				}
			}
		}
	}
}

void render_triangle(Camera camera, Entity e, array_view<unsigned int, 1> canvasView, unsigned int canvas_w, unsigned int canvas_h, unsigned int color) restrict(amp) {
	float scaled_scale = e.scale * camera.zoom;
	int half_base = static_cast<int>(scaled_scale);
	int height = static_cast<int>(scaled_scale * 2);
	float angle_rad = -e.rotation * 3.14159265f / 180.0f;
	float cos_a = cos(angle_rad);
	float sin_a = sin(angle_rad);
	for (int y = 0; y < height; y++) {
		int row_width = (half_base * (height - y)) / height;
		for (int x = -row_width; x <= row_width; x++) {
			float world_x = e.x + x * cos_a - y * sin_a;
			float world_y = e.y + x * sin_a + y * cos_a;
			int px = static_cast<int>((world_x - camera.x) * camera.zoom + canvas_w / 2.0f);
			int py = static_cast<int>((world_y - camera.y) * camera.zoom + canvas_h / 2.0f);
			if (px >= 0 && px < canvas_w && py >= 0 && py < canvas_h) {
				canvasView[py * canvas_w + px] = color;
			}
		}
	}
}