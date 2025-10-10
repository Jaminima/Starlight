using StarlightGame.StarlightLib;
using StarlightGame.StarlightLib.Types;
using System;

namespace StarlightGame.GL
{
    internal class Renderer
    {
        public void RenderEntities(Entity[] entities, uint[] canvas, uint width, uint height)
        {
            AmpSharp.RenderEntities(entities, canvas, width, height);
        }
    }
}