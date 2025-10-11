using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StarlightGame.StarlightLib.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct Camera
    {
        public float X, Y ;
        public float VX, VY;
        public float Zoom;

        public void UpdateAttraction(Entity player, float deltaTime)
        {
            float playerSpeed = (float)Math.Sqrt(player.VX * player.VX + player.VY * player.VY);
            float cameraSpeed = (float)Math.Sqrt(VX * VX + VY * VY);
            float dx = player.X - X;
            float dy = player.Y - Y;
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            if (dist > 0.01f)
            {
                float dirX = dx / dist;
                float dirY = dy / dist;
                float relVelX = player.VX - VX;
                float relVelY = player.VY - VY;
                float relSpeedAway = Math.Max(0, relVelX * dirX + relVelY * dirY);
                float forceMag = MathF.Pow(dist,1.5f) + relSpeedAway * 1.8f;
                VX += dirX * forceMag * deltaTime;
                VY += dirY * forceMag * deltaTime;
            }
            // Damping
            VX *= 0.98f;
            VY *= 0.98f;
            // Update position
            X += VX * deltaTime;
            Y += VY * deltaTime;
        }
    }
}
