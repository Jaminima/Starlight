#pragma once
#include "pch.h"
#include "Entity.h"

using namespace concurrency;
using namespace concurrency::fast_math;

extern "C" __declspec(dllexport) void _stdcall update_entities(Entity* particles, int count, float dt);