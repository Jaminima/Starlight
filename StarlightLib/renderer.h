#include "pch.h"
#include "Entity.h"

using namespace concurrency;

extern "C" __declspec(dllexport) void _stdcall render_entities(Entity* particles, unsigned int particle_count, unsigned int* canvas, unsigned int canvas_w, unsigned int canvas_h);

void render_circle(Entity e, array_view<unsigned int, 1> canvasView, unsigned int canvas_w, unsigned int canvas_h) restrict(amp);