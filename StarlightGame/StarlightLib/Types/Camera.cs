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
    }
}
