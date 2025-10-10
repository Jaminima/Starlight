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
        extern unsafe static void update_entities(Entity* entities, int count, float dt);

        public unsafe static void UpdateEntities(Span<Entity> entities, float dt)
        {
            fixed (Entity* p = entities)
            {
                update_entities(p, entities.Length, dt);
            }
        }

        [DllImport("StarlightLib", CallingConvention = CallingConvention.StdCall)]
        extern unsafe static void render_entities(Entity* particles, uint particle_count, uint* canvas, uint canvas_w, uint canvas_h);

        public unsafe static void RenderEntities(Entity[] particles, uint[] canvas, uint canvas_w, uint canvas_h)
        {
            fixed (Entity* pParticles = particles)
            fixed (uint* pCanvas = canvas)
            {
                render_entities(pParticles, (uint)particles.Length, pCanvas, canvas_w, canvas_h);
            }
        }
    }
}
