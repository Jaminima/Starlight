#pragma once
#include "pch.h"
#include "Entity.h"

using namespace concurrency;

extern "C" __declspec(dllexport) void _stdcall square_array(float* arr, int size);
extern "C" __declspec(dllexport) void _stdcall update_entities(Entity* particles, int count, float dt);