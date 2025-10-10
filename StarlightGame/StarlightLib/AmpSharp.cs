using StarlightGame.StarlightLib.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StarlightGame.StarlightLib
{
    internal class AmpSharp
    {
        [DllImport("StarlightLib", CallingConvention = CallingConvention.StdCall)]
        extern unsafe static void square_array(float* input, int length);

        public unsafe static float[] SquareArray(float[] input)
        {
            fixed (float* pInput = input) {
                square_array(pInput, input.Length);
            }

            return input;
        }

        [DllImport("StarlightLib", CallingConvention = CallingConvention.StdCall)]
        extern unsafe static void update_particles(Particle* particles, int count, float dt);

        public unsafe static void UpdateParticles(Span<Particle> particles, float dt)
        {
            fixed (Particle* p = particles)
            {
                update_particles(p, particles.Length, dt);
            }
        }
    }
}
