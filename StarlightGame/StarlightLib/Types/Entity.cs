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
        public float X, Y;
        public float VX, VY;
        public float Mass;
        public float Rotation;
        public float Scale;
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
        Projectile,
        PowerUp,
        Environment,
    }
}
