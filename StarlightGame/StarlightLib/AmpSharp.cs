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
        extern unsafe static void update_entities(Entity* entities, int count, float dt);

        public unsafe static void UpdateEntities(Span<Entity> entities, float dt)
        {
            fixed (Entity* p = entities)
            {
                update_entities(p, entities.Length, dt);
            }
        }

        [DllImport("StarlightLib", CallingConvention = CallingConvention.StdCall)]
        extern unsafe static void render_entities(Camera* camera, Entity* entities, uint entity_count, uint* canvas, uint canvas_w, uint canvas_h);

        public unsafe static void RenderEntities(Camera camera, Entity[] entities, uint entity_count, uint[] canvas, uint canvas_w, uint canvas_h)
        {
            fixed (Entity* pEntities = entities)
            fixed (uint* pCanvas = canvas)
            {
                render_entities(&camera, pEntities, entity_count, pCanvas, canvas_w, canvas_h);
            }
        }
    }
}
