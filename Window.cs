using System;
using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;

using System.IO;
using System.Globalization;

namespace OpenTKLab

{
    public class Window : GameWindow
    {
        
        private float[] vertices = { 
            -1f,-1f,0f,
            1f,-1f,0f,
            -1f,1f,0f,

            1f,-1f,0f,
            -1f,1f,0f,
            1f,1f,0f
        };
 
        private int _vertexBufferObject;

        private int _vaoModel;

        Vector3 Position = new Vector3(0f, 0f, -4.9f);

        private Shader shader;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);
            { 
                _vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            }

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            {
                _vaoModel = GL.GenVertexArray();
                GL.BindVertexArray(_vaoModel);

                var positionLocation = shader.GetAttribLocation("vPosition");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            }

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(_vaoModel);
            
            shader.Use();

            shader.SetVector3("uCamera.Position", Position);
            Vector3 view = new Vector3(0f, 0f, 1f);
            shader.SetVector3("uCamera.View", view);

            Vector3 side = Vector3.Normalize(Vector3.Cross(-view, Vector3.UnitY));
            Vector3 up = Vector3.Normalize(Vector3.Cross(side, -view));

            shader.SetVector3("uCamera.Up", up);
            shader.SetVector3("uCamera.Side", side);

            if (Size.X > Size.Y)
            {
                shader.SetVector2("uCamera.Scale", new Vector2((float)Size.X / Size.Y, 1f));
            }
            else { 
                shader.SetVector2("uCamera.Scale", new Vector2(1f,(float)Size.Y / Size.X));
            }

            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 6);

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
                return;

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            var input = KeyboardState;

            const float cameraSpeed = 1.0f;

            if (input.IsKeyDown(Keys.W))
            {
                Position.Z += cameraSpeed * (float)e.Time; // Forward
                Position.Z = Math.Min(Position.Z, 4.99f);
            }
            if (input.IsKeyDown(Keys.S))
            {
                Position.Z -= cameraSpeed * (float)e.Time; // Backwards
                Position.Z = Math.Max(Position.Z, -4.99f);
            }
            if (input.IsKeyDown(Keys.D))
            {
                Position.X += cameraSpeed * (float)e.Time; // Right
                Position.X = Math.Min(Position.X, 4.99f);
            }
            if (input.IsKeyDown(Keys.A))
            {
                Position.X -= cameraSpeed * (float)e.Time; // Left
                Position.X = Math.Max(Position.X, -4.99f);
            }
            if (input.IsKeyDown(Keys.Space))
            {
                Position.Y += cameraSpeed * (float)e.Time; // Up
                Position.Y = Math.Min(Position.Y, 4.99f);
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                Position.Y -= cameraSpeed * (float)e.Time; // Down
                Position.Y = Math.Max(Position.Y, -4.99f);
            }

        }


        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }

    }
}
