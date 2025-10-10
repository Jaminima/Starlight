using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarlightGame.GL
{
    internal class Window : GameWindow
    {
        public Window() : base(new GameWindowSettings(),
            new NativeWindowSettings()
            {
                ClientSize = new OpenTK.Mathematics.Vector2i(800, 600),
                Title = "Starlight"
            })
        {
        }

        protected override void OnLoad()
        {
        }

        protected override void OnUnload()
        {
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
        }
    }
}
