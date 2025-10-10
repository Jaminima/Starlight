#include "pch.h"
#include "Entity.h"

using namespace concurrency;

extern "C" __declspec(dllexport) void _stdcall render_entities(Entity* particles, int particle_count, unsigned int* canvas, int canvas_w, int canvas_h);