using StarlightGame.StarlightLib;
using StarlightGame.StarlightLib.Types;
using System;

namespace StarlightGame.GL
{
    internal class Renderer
    {
        public void RenderEntities(Scene scene, uint[] canvas, uint width, uint height)
        {
            AmpSharp.RenderEntities(scene.Camera, scene.Entities, scene.entityHead, canvas, width, height, scene.CurrentTargetIndex);
        }
    }
}