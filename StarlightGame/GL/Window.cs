using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using static OpenTK.Graphics.OpenGL4.GL;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarlightGame.StarlightLib.Types;
using StarlightGame.StarlightLib;

namespace StarlightGame.GL
{
    internal unsafe class Window : GameWindow
    {
        private int textureId;
        private int shaderProgram;
        private int vao, vbo;
        private Renderer renderer;
        private Simulation simulation;
        private FPSTracker fpsTracker;
        private Scene scene;

        public Window(Scene scene) : base(new GameWindowSettings(),
            new NativeWindowSettings()
            {
                ClientSize = new OpenTK.Mathematics.Vector2i(1920, 1080),
                Title = "Starlight"
            })
        {
            this.scene = scene;
            renderer = new Renderer();
            simulation = new Simulation();
            fpsTracker = new FPSTracker();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            // Create texture
            textureId = GenTexture();
            BindTexture(TextureTarget.Texture2D, textureId);
            TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Create shaders
            string vertexShaderSource = @"
#version 330 core

layout(location = 0) in vec2 aPosition;

out vec2 texCoord;

void main()
{
    texCoord = (aPosition + 1.0) / 2.0;
    gl_Position = vec4(aPosition, 0.0, 1.0);
}
";

            string fragmentShaderSource = @"
#version 330 core

in vec2 texCoord;

out vec4 FragColor;

uniform sampler2D textureSampler;

void main()
{
    FragColor = texture(textureSampler, texCoord);
}
";

            int vertexShader = CreateShader(ShaderType.VertexShader);
            ShaderSource(vertexShader, vertexShaderSource);
            CompileShader(vertexShader);

            int fragmentShader = CreateShader(ShaderType.FragmentShader);
            ShaderSource(fragmentShader, fragmentShaderSource);
            CompileShader(fragmentShader);

            shaderProgram = CreateProgram();
            AttachShader(shaderProgram, vertexShader);
            AttachShader(shaderProgram, fragmentShader);
            LinkProgram(shaderProgram);

            DeleteShader(vertexShader);
            DeleteShader(fragmentShader);

            // Create quad
            float[] vertices = {
                -1.0f, -1.0f,
                 1.0f, -1.0f,
                -1.0f,  1.0f,
                 1.0f,  1.0f
            };

            vao = GenVertexArray();
            BindVertexArray(vao);

            vbo = GenBuffer();
            BindBuffer(BufferTarget.ArrayBuffer, vbo);
            BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            EnableVertexAttribArray(0);
        }

        protected override void OnUnload()
        {
            DeleteTexture(textureId);
            DeleteProgram(shaderProgram);
            DeleteVertexArray(vao);
            DeleteBuffer(vbo);

            base.OnUnload();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            fpsTracker.Update(e.Time);

            Clear(ClearBufferMask.ColorBufferBit);

            var colors = new uint[ClientSize.X * ClientSize.Y];
            renderer.RenderEntities(scene, colors, (uint)ClientSize.X, (uint)ClientSize.Y);

            BindTexture(TextureTarget.Texture2D, textureId);
            TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ClientSize.X, ClientSize.Y, 0, PixelFormat.Rgba, PixelType.UnsignedByte, colors);

            UseProgram(shaderProgram);
            BindVertexArray(vao);
            DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var keyboard = KeyboardState.GetSnapshot();

            Entity player = scene.Entities[0];

            float speed = 50.0f * (float)e.Time;
            if (keyboard.IsKeyDown(Keys.W)) player.ApplyForwardForce(speed);
            if (keyboard.IsKeyDown(Keys.S)) player.ApplyBackwardForce(speed * 0.5f);
            if (keyboard.IsKeyDown(Keys.A)) player.ApplyLeftForce(speed * 0.15f);
            if (keyboard.IsKeyDown(Keys.D)) player.ApplyRightForce(speed * 0.15f);

            float rotSpeed = 50.0f * (float)e.Time;
            if (keyboard.IsKeyDown(Keys.Q)) player.RotationDeg -= rotSpeed;
            if (keyboard.IsKeyDown(Keys.E)) player.RotationDeg += rotSpeed;

            if (keyboard.IsKeyDown(Keys.F))
            {
                player.QueuedEvent = EntityEvent.FireCannon;
            }
            if (keyboard.IsKeyDown(Keys.G))
            {
                player.QueuedEvent = EntityEvent.FireMissile;
            }
            if (keyboard.IsKeyDown(Keys.H))
            {
                player.QueuedEvent = EntityEvent.Shields;
            }

            scene.Entities[0] = player;

            simulation.SimulateEntities(scene, (float)e.Time);

            scene.Camera.UpdateAttraction(player, (float)e.Time);
        }
    }
}
