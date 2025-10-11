using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StarlightGame.StarlightLib.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct Entity
    {
        public EntityLayer Layer;
        public EntityType Type;
        public EntityEvent LastEvent;
        public EntityEvent QueuedEvent;
        public float EventTime;
        public float X, Y;
        public float VX, VY;
        public float Mass;
        public float RotationDeg;
        public float Scale;
        public float TimeToLive;
        public float TimeAlive;

        public void ApplyForwardForce(float force)
        {
            float rad = RotationDeg * (float)Math.PI / 180.0f;
            VX += force * (float)Math.Sin(rad);
            VY += force * (float)Math.Cos(rad);
        }

        public void ApplyBackwardForce(float force)
        {
            ApplyForwardForce(-force);
        }

        public void ApplyLeftForce(float force)
        {
            float rad = (RotationDeg - 90) * (float)Math.PI / 180.0f;
            VX += force * (float)Math.Sin(rad);
            VY += force * (float)Math.Cos(rad);
        }

        public void ApplyRightForce(float force)
        {
            float rad = (RotationDeg + 90) * (float)Math.PI / 180.0f;
            VX += force * (float)Math.Sin(rad);
            VY += force * (float)Math.Cos(rad);
        }
    }

    internal enum EntityLayer
    {
        Background,
        Midground,
        Foreground,
        UI,
    }

    internal enum EntityType
    {
        Player,
        Enemy,
        Cannon,
        Missile,
        PowerUp,
        Environment,
    }

    internal enum EntityEvent
    {
        None,
        FireCannon,
        FireMissile,
        Shields,
        Die
    }
}
