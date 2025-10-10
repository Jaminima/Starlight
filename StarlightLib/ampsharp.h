#pragma once
#define _SILENCE_AMP_DEPRECATION_WARNINGS
#include <amp.h>

using namespace concurrency;

extern "C" __declspec(dllexport) void square_array(float* arr, int size);