#include "pch.h"
#include "Entity.h"
#include "Camera.h"

using namespace concurrency;
using namespace concurrency::fast_math;

extern "C" __declspec(dllexport) void _stdcall render_entities(Camera* camera, Entity* entities, unsigned int entitiy_count, unsigned int* canvas, unsigned int canvas_w, unsigned int canvas_h, int targetIndex);

void render_circle(Camera camera, Entity e, array_view<unsigned int, 1> canvasView, unsigned int canvas_w, unsigned int canvas_h, unsigned int color) restrict(amp);

void render_triangle(Camera camera, Entity e, array_view<unsigned int, 1> canvasView, unsigned int canvas_w, unsigned int canvas_h, unsigned int color) restrict(amp);

void render_circle_outline(Camera camera, Entity e, array_view<unsigned int, 1> canvasView, unsigned int canvas_w, unsigned int canvas_h, unsigned int color) restrict(amp);

void render_explosion(Camera camera, Entity e, array_view<unsigned int, 1> canvasView, unsigned int canvas_w, unsigned int canvas_h) restrict(amp);

unsigned int scale_color(unsigned int color, float brightness) restrict(amp);