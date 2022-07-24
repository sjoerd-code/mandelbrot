using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using ImGuiNET;

namespace Project
{
    public class Window : GameWindow
    {
        private int vertexBufferHandle;
        private int shaderProgramHandle;
        private int vertexArrayHandle;

        ImGuiController imguiController;
        private float zoom = 0.5f;
        private int iter = 100;

        private float timePassed;
        private Vector2 cameraPosition = new Vector2(-0.5f, 0);
        private System.Numerics.Vector3 color = new System.Numerics.Vector3(0, 1, 1);

        public Window() : base(GameWindowSettings.Default, new NativeWindowSettings(){ Size = new Vector2i(1280, 720), APIVersion = new Version(3, 3) })
        {
            imguiController = new ImGuiController(ClientSize.X, ClientSize.Y);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);
            GL.ClearColor(new Color4(0.3f, 0.4f, 0.5f, 1f));
            CreateFullScreenQuadFromTwoPolygons();
            CreateShaderProgram("resources/shader.vert", "resources/shader.frag");
            ImGui.SetWindowPos(new System.Numerics.Vector2(16, 16));
            ImGui.SetWindowSize(new System.Numerics.Vector2(256, 128));
        }
        
        protected override void OnResize(ResizeEventArgs eventArgs)
        {
            base.OnResize(eventArgs);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            imguiController.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnRenderFrame(FrameEventArgs eventArgs)
        {
            base.OnRenderFrame(eventArgs);
            timePassed += (float)eventArgs.Time;
            imguiController.Update(this, (float)eventArgs.Time);

            GL.ClearColor(new Color4(0, 32, 48, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GL.UseProgram(shaderProgramHandle);

            GL.Uniform2(GL.GetUniformLocation(shaderProgramHandle, "resolution"), (float)ClientSize.X, (float)ClientSize.Y);
            GL.Uniform1(GL.GetUniformLocation(shaderProgramHandle, "time"), timePassed);
            GL.Uniform1(GL.GetUniformLocation(shaderProgramHandle, "zoom"), zoom);
            GL.Uniform1(GL.GetUniformLocation(shaderProgramHandle, "iter"), iter);
            GL.Uniform2(GL.GetUniformLocation(shaderProgramHandle, "cameraPosition"), cameraPosition.X, cameraPosition.Y);
            GL.Uniform3(GL.GetUniformLocation(shaderProgramHandle, "color"), (float)color.X, (float)color.Y, (float)color.Z);

            GL.UseProgram(shaderProgramHandle);
            GL.BindVertexArray(vertexArrayHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            ImGui.Text("fps: " + (1 / (float)eventArgs.Time).ToString("#"));
            ImGui.Text("zoom: " + zoom.ToString("0.0"));
            ImGui.SliderInt("iter", ref iter, 10, 200);
            ImGui.ColorEdit3("color", ref color);

            imguiController.Render();
            ImGuiController.CheckGLError("End of frame");
            SwapBuffers();
        }

         protected override void OnUpdateFrame(FrameEventArgs eventArgs)
        {
            base.OnUpdateFrame(eventArgs);
            if (!IsFocused) return;

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape)) Close();

            if (input.IsKeyDown(Keys.W)) cameraPosition += Vector2.UnitY * (float)eventArgs.Time / zoom;
            if (input.IsKeyDown(Keys.A)) cameraPosition += -Vector2.UnitX * (float)eventArgs.Time / zoom;
            if (input.IsKeyDown(Keys.S)) cameraPosition += -Vector2.UnitY * (float)eventArgs.Time / zoom;
            if (input.IsKeyDown(Keys.D)) cameraPosition += Vector2.UnitX * (float)eventArgs.Time / zoom;

            if (input.IsKeyDown(Keys.Up) && zoom < 10000) zoom += (float)eventArgs.Time * zoom;
            if (input.IsKeyDown(Keys.Down) && zoom > 0.1f) zoom -= (float)eventArgs.Time * zoom;
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexBufferHandle);
            GL.UseProgram(0);
            GL.DeleteProgram(shaderProgramHandle);
            base.OnUnload();
        }

        protected override void OnTextInput(TextInputEventArgs eventArgs)
        {
            base.OnTextInput(eventArgs);
            imguiController.PressChar((char)eventArgs.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs eventArgs)
        {
            base.OnMouseWheel(eventArgs);
            imguiController.MouseScroll(eventArgs.Offset);
        }

        private void CreateShaderProgram(string vertexShaderPath, string fragmentShaderPath)
        {
            string vertexShaderCode = File.ReadAllText(vertexShaderPath);
            string fragmentShaderCode = File.ReadAllText(fragmentShaderPath);

            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);

            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderCode);
            GL.CompileShader(fragmentShaderHandle);

            shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.LinkProgram(shaderProgramHandle);

            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, fragmentShaderHandle);

            GL.DeleteShader(vertexBufferHandle);
            GL.DeleteShader(fragmentShaderHandle);
        }

        private void CreateFullScreenQuadFromTwoPolygons()
        {
            float[] vertices = new float[]
            {
                -1f, 1f, 0f,
                1f, 1f, 0f,
                -1f, -1f, 0f,
                1f, 1f, 0f,
                1f, -1f, 0f,
                -1f, -1f, 0f,
            };

            vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }
    }
}
