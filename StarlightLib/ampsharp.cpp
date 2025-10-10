#include "pch.h"
#include "ampsharp.h"

void _stdcall square_array(float* arr, int size) {
	array_view<float, 1> arrView(size, arr);
	parallel_for_each(arrView.extent, [=](index<1> idx) restrict(amp) {
		arrView[idx] = arrView[idx] * arrView[idx];
		});
	arrView.synchronize();
}

void _stdcall update_particles(Particle* particles, int count, float dt) {
	array_view<Particle, 1> view(count, particles);
	parallel_for_each(view.extent, [=](index<1> idx) restrict(amp) {
		Particle p = view[idx];
		p.x += p.vx * dt;
		p.y += p.vy * dt;
		p.z += p.vz * dt;
		view[idx] = p;
	});
	view.synchronize();
}
