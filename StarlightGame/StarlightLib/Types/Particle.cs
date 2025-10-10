using System.Runtime.InteropServices;

namespace StarlightGame.StarlightLib.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct Particle
    {
        public float X, Y, Z;
        public float Vx, Vy, Vz;
        public float Mass;
    }
}
