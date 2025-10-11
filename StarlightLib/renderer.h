#include "pch.h"
#include "Entity.h"
#include "Camera.h"

using namespace concurrency;
using namespace concurrency::fast_math;

extern "C" __declspec(dllexport) void _stdcall render_entities(Camera* camera, Entity* particles, unsigned int particle_count, unsigned int* canvas, unsigned int canvas_w, unsigned int canvas_h);

void render_circle(Camera camera, Entity e, array_view<unsigned int, 1> canvasView, unsigned int canvas_w, unsigned int canvas_h, unsigned int color) restrict(amp);

void render_triangle(Camera camera, Entity e, array_view<unsigned int, 1> canvasView, unsigned int canvas_w, unsigned int canvas_h, unsigned int color) restrict(amp);