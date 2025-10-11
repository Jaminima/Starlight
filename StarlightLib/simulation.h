#pragma once
#include "pch.h"
#include "Entity.h"

using namespace concurrency;
using namespace concurrency::fast_math;

extern "C" __declspec(dllexport) void _stdcall update_entities(Entity* particles, int count, float dt);

// Helper used in simulation to compute squared distance
inline float dist2(float ax, float ay, float bx, float by) restrict(amp)
{
    float dx = ax - bx;
    float dy = ay - by;
    return dx * dx + dy * dy;
}