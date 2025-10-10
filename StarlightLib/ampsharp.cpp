#include "pch.h"
#include "ampsharp.h"

void _stdcall square_array(float* arr, int size) {
	array_view<float, 1> arrView(size, arr);
	parallel_for_each(arrView.extent, [=](index<1> idx) restrict(amp) {
		arrView[idx] = arrView[idx] * arrView[idx];
		});
	arrView.synchronize();
}
