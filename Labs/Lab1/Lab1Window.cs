using System;
using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Labs.Lab1
{
    public class Lab1Window : GameWindow
    {
        private int[] mVertexBufferObjectIDArray = new int [2];
        private ShaderUtility mShader;

        #region house indices
        /*
        uint[] indices = new uint[] { 0, 1, 2,
                                      2, 3, 0,
                                      3, 4, 5,
                                      5, 6, 3,
                                      7, 8, 9,
                                      9, 8, 10,
                                      8, 11, 10,
                                      12, 13, 14,
                                      14, 15, 12,
                                      8, 16, 17,
                                      8, 17, 18,
                                      19, 20, 17,
                                      20, 21, 17 };
        // 39 indices
        */
        #endregion

        uint[] indices = new uint[] { 3, 0, 2,
                                      1,

                                      10, 9, 7,
                                      11,

                                      8, 16, 17,
                                      18,

                                      12, 13, 14,
                                      15,

                                      3, 4, 5,
                                      6,

                                      20, 21, 17,
                                      19 };
        // 24 indices


        public Lab1Window()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Lab 1 Hello, Triangle",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color4.Green);
            GL.Enable(EnableCap.CullFace);

            #region house vertices
            
            float[] vertices = new float[] { -0.6f, -0.6f,
                                              0.2f, -0.6f,
                                              0.2f, -0.2f,

                                              -0.6f, -0.2f,

                                              -0.4f, -0.2f,
                                              -0.4f, 0.2f,

                                              -0.6f, 0.2f,

                                              -0.8f, 0.2f,
                                              0.0f, 0.2f,
                                              -0.4f, 0.6f,

                                              0.4f, 0.6f,

                                              0.8f, 0.2f,

                                              -0.2f, 0.6f,
                                              0.0f, 0.6f,
                                              0.0f, 0.8f,

                                              -0.2f, 0.8f,

                                              0.0f, -0.2f,
                                              0.6f, -0.2f,

                                              0.6f, 0.2f,

                                              0.4f, -0.2f,
                                              0.4f, -0.6f,

                                              0.6f, -0.6f };
            
            #endregion

            /*
            float[] vertices = new float[] { 0.0f, 0.8f,
                                             0.8f, 0.4f,
                                             0.6f, -0.6f,
                                             -0.6f, -0.6f,
                                             -0.8f, 0.4f };
            */


            GL.GenBuffers(2, mVertexBufferObjectIDArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectIDArray[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);

            if (vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVertexBufferObjectIDArray[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);

            if (indices.Length * sizeof(uint) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            #region Shader Loading Code - Can be ignored for now

            mShader = new ShaderUtility( @"Lab1/Shaders/vSimple.vert", @"Lab1/Shaders/fSimple.frag");

            #endregion

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectIDArray[0]);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVertexBufferObjectIDArray[1]);

            // shader linking goes here
            #region Shader linking code - can be ignored for now

            GL.UseProgram(mShader.ShaderProgramID);
            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            #endregion

            GL.DrawElements(PrimitiveType.TriangleStrip, 4, DrawElementsType.UnsignedInt, 0);
            GL.DrawElements(PrimitiveType.TriangleFan, 4, DrawElementsType.UnsignedInt, 4 * sizeof(uint));
            GL.DrawElements(PrimitiveType.TriangleFan, 4, DrawElementsType.UnsignedInt, 8 * sizeof(uint));
            GL.DrawElements(PrimitiveType.TriangleFan, 4, DrawElementsType.UnsignedInt, 12 * sizeof(uint));
            GL.DrawElements(PrimitiveType.TriangleFan, 4, DrawElementsType.UnsignedInt, 16 * sizeof(uint));
            GL.DrawElements(PrimitiveType.TriangleFan, 4, DrawElementsType.UnsignedInt, 20 * sizeof(uint));

            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            GL.DeleteBuffers(2, mVertexBufferObjectIDArray);
            GL.UseProgram(0);
            mShader.Delete();
        }
    }
}