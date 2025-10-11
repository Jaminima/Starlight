using StarlightGame.StarlightLib;
using StarlightGame.StarlightLib.Types;
using System;

namespace StarlightGame.GL
{
    internal class Renderer
    {
        public void RenderEntities(Camera camera,Entity[] entities, uint[] canvas, uint width, uint height)
        {
            AmpSharp.RenderEntities(camera, entities, canvas, width, height);
        }
    }
}