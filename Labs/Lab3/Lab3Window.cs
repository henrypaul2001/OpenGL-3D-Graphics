using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace Labs.Lab3
{
    public class Lab3Window : GameWindow
    {
        public Lab3Window()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Lab 3 Lighting and Material Properties",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        private int[] mVBO_IDs = new int[7];
        private int[] mVAO_IDs = new int[4];
        private ShaderUtility mShader;
        private ModelUtility mSphereModelUtility;
        private ModelUtility mCylinderModelUtility;
        private ModelUtility mModelUtility;
        private Matrix4 mView, mSphereModel, mGroundModel, mCylinderModel, mModel;
        private Vector4 light1Pos, light2Pos, light3Pos;

        protected override void OnLoad(EventArgs e)
        {
            // Set some GL state
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            mShader = new ShaderUtility(@"Lab3/Shaders/vPassThrough.vert", @"Lab3/Shaders/fLighting.frag");
            GL.UseProgram(mShader.ShaderProgramID);
            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            int vNormalLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vNormal");

            #region floor
            GL.GenVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);

            float[] vertices = new float[] {-10, 0, -10,0,1,0,
                                             -10, 0, 10,0,1,0,
                                             10, 0, 10,0,1,0,
                                             10, 0, -10,0,1,0,};

            GL.BindVertexArray(mVAO_IDs[0]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));
            #endregion

            #region sphere
            mSphereModelUtility = ModelUtility.LoadModel(@"Utility/Models/sphere.bin");

            GL.BindVertexArray(mVAO_IDs[1]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mSphereModelUtility.Vertices.Length * sizeof(float)), mSphereModelUtility.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[2]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mSphereModelUtility.Indices.Length * sizeof(float)), mSphereModelUtility.Indices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mSphereModelUtility.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mSphereModelUtility.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));
            #endregion

            #region cylinder

            mCylinderModelUtility = ModelUtility.LoadModel(@"Utility/Models/cylinder.bin");

            GL.BindVertexArray(mVAO_IDs[2]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[3]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mCylinderModelUtility.Vertices.Length * sizeof(float)), mCylinderModelUtility.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[4]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mCylinderModelUtility.Indices.Length * sizeof(float)), mCylinderModelUtility.Indices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mCylinderModelUtility.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mCylinderModelUtility.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));

            #endregion

            #region model

            mModelUtility = ModelUtility.LoadModel(@"Utility/Models/model.bin");

            GL.BindVertexArray(mVAO_IDs[3]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[5]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mModelUtility.Vertices.Length * sizeof(float)), mModelUtility.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[6]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mModelUtility.Indices.Length * sizeof(float)), mModelUtility.Indices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mModelUtility.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mModelUtility.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));

            #endregion

            GL.BindVertexArray(0);

            mView = Matrix4.CreateTranslation(0, -1.5f, 0);
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);

            Vector4 eyePosition = Vector4.Transform(new Vector4(0, 0, 0, 1), mView);
            int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
            GL.Uniform4(uEyePosition, eyePosition);

            #region light properties
            // Light 1
            light1Pos = new Vector4(10, 4, 0f, 1f);
            EditLightPosition(light1Pos, 0);

            EditAmbientLight(new Vector3(1f, 0f, 0f), 0);

            EditDiffuseLight(new Vector3(1f, 0f, 0f), 0);

            EditSpecularLight(new Vector3(1f, 0f, 0f), 0);

            // Light 2
            light2Pos = new Vector4(1, 4, -5f, 1);
            EditLightPosition(light2Pos, 1);

            EditAmbientLight(new Vector3(0f, 1f, 0f), 1);

            EditDiffuseLight(new Vector3(0f, 1f, 0f), 1);

            EditSpecularLight(new Vector3(0f, 1f, 0f), 1);

            // Light 3
            light3Pos = new Vector4(-10, 4, -5f, 1);
            EditLightPosition(light3Pos, 2);

            EditAmbientLight(new Vector3(0f, 0f, 1f), 2);

            EditDiffuseLight(new Vector3(0f, 0f, 1f), 2);

            EditSpecularLight(new Vector3(0f, 0f, 1f), 2);
            #endregion

            mGroundModel = Matrix4.CreateTranslation(0, 0, -5f);
            mSphereModel = Matrix4.CreateTranslation(-5, 1, -5f);
            mCylinderModel = Matrix4.CreateTranslation(0, 1, -5f);

            mModel = Matrix4.CreateTranslation(-0.12f, 3, -5f);
            Vector3 t = mModel.ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            mModel = mModel * inverseTranslation * Matrix4.CreateRotationY(-1.7f) * translation;

            base.OnLoad(e);
        }

        private void EditMaterialProperties(Vector3 ambientReflect, Vector3 diffuseReflect, Vector3 specularReflect, float shininess)
        {
            EditAmbientReflectivity(ambientReflect);

            EditDiffuseReflectivity(diffuseReflect);

            EditSpecularReflectivity(specularReflect);

            EditShininess(shininess);
        }

        private void EditShininess(float shininess)
        {
            int uShineLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.Shininess");
            GL.Uniform1(uShineLocation, shininess);
        }

        private void EditSpecularReflectivity(Vector3 specularReflectivity)
        {
            int uSpecularReflectivityLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.SpecularReflectivity");
            GL.Uniform3(uSpecularReflectivityLocation, specularReflectivity);
        }

        private void EditDiffuseReflectivity(Vector3 diffuseReflectivity)
        {
            int uDiffuseReflectivityLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.DiffuseReflectivity");
            GL.Uniform3(uDiffuseReflectivityLocation, diffuseReflectivity);
        }

        private void EditAmbientReflectivity(Vector3 ambientReflectivity)
        {
            int uAmbientReflectivityLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.AmbientReflectivity");
            GL.Uniform3(uAmbientReflectivityLocation, ambientReflectivity);
        }

        private void EditSpecularLight(Vector3 specular, int index)
        {
            int uSpecularLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[" + index + "].SpecularLight");
            GL.Uniform3(uSpecularLightLocation, specular);
        }

        private void EditDiffuseLight(Vector3 diffuse, int index)
        {
            int uDiffuseLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[" + index + "].DiffuseLight");
            GL.Uniform3(uDiffuseLightLocation, diffuse);
        }

        private void EditAmbientLight(Vector3 colour, int index)
        {
            int uAmbientLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[" + index + "].AmbientLight");
            GL.Uniform3(uAmbientLightLocation, colour);
        }

        private void EditLightPosition(Vector4 pLightPosition, int index)
        {
            int uLightPositionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[" + index + "].Position");
            pLightPosition = Vector4.Transform(pLightPosition, mView);
            GL.Uniform4(uLightPositionLocation, pLightPosition);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(this.ClientRectangle);
            if (mShader != null)
            {
                int uProjectionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uProjection");
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 25);
                GL.UniformMatrix4(uProjectionLocation, true, ref projection);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyChar == 'w')
            {
                MoveForward();
            }
            else if (e.KeyChar == 's')
            {
                MoveBack();
            }
            if (e.KeyChar == 'a')
            {
                RotateLeft();
            }
            else if (e.KeyChar == 'd')
            {
                RotateRight();
            }
            if (e.KeyChar == 'z')
            {
                RotateGroundLeft();
            }
            else if (e.KeyChar == 'x')
            {
                RotateGroundRight();
            }
            if (e.KeyChar == 'c')
            {
                Vector3 t = mSphereModel.ExtractTranslation();
                Matrix4 translation = Matrix4.CreateTranslation(t);
                Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
                mSphereModel = mSphereModel * inverseTranslation * Matrix4.CreateRotationY(-0.25f) * translation;
            }
            else if (e.KeyChar == 'v')
            {
                Vector3 t = mSphereModel.ExtractTranslation();
                Matrix4 translation = Matrix4.CreateTranslation(t);
                Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
                mSphereModel = mSphereModel * inverseTranslation * Matrix4.CreateRotationY(0.25f) * translation;
            }
        }

        private void RotateGroundRight()
        {
            Vector3 t = mGroundModel.ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            mGroundModel = mGroundModel * inverseTranslation * Matrix4.CreateRotationY(0.025f) * translation;
        }

        private void RotateGroundLeft()
        {
            Vector3 t = mGroundModel.ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            mGroundModel = mGroundModel * inverseTranslation * Matrix4.CreateRotationY(-0.025f) * translation;
        }

        private void RotateRight()
        {
            mView = mView * Matrix4.CreateRotationY(0.025f);
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);

            EditLightPosition(light1Pos, 0);
            EditLightPosition(light2Pos, 1);
            EditLightPosition(light3Pos, 2);

            Vector4 eyePosition = Vector4.Transform(new Vector4(0, 0, 0, 1), mView);
            int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
            GL.Uniform4(uEyePosition, eyePosition);
        }

        private void RotateLeft()
        {
            mView = mView * Matrix4.CreateRotationY(-0.025f);
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);

            EditLightPosition(light1Pos, 0);
            EditLightPosition(light2Pos, 1);
            EditLightPosition(light3Pos, 2);

            Vector4 eyePosition = Vector4.Transform(new Vector4(0, 0, 0, 1), mView);
            int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
            GL.Uniform4(uEyePosition, eyePosition);
        }

        private void MoveBack()
        {
            mView = mView * Matrix4.CreateTranslation(0.0f, 0.0f, -0.05f);
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);

            EditLightPosition(light1Pos, 0);
            EditLightPosition(light2Pos, 1);
            EditLightPosition(light3Pos, 2);

            Vector4 eyePosition = Vector4.Transform(new Vector4(0, 0, 0, 1), mView);
            int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
            GL.Uniform4(uEyePosition, eyePosition);
        }

        private void MoveForward()
        {
            mView = mView * Matrix4.CreateTranslation(0.0f, 0.0f, 0.05f);
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);

            EditLightPosition(light1Pos, 0);
            EditLightPosition(light2Pos, 1);
            EditLightPosition(light3Pos, 2);

            Vector4 eyePosition = Vector4.Transform(new Vector4(0, 0, 0, 1), mView);
            int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
            GL.Uniform4(uEyePosition, eyePosition);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Vector4 eyePosition = Vector4.Transform(new Vector4(0, 0, 0, 1), mView);
            int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
            GL.Uniform4(uEyePosition, eyePosition);

            int uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref mGroundModel);

            EditMaterialProperties(new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.7f, 0.7f, 0.7f), 60f);

            GL.BindVertexArray(mVAO_IDs[0]);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

            Matrix4 m = mSphereModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            //EditMaterialProperties(new Vector3(0.25f, 0.25f, 0.25f), new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.774597f, 0.774597f, 0.774597f), 30f); //30

            GL.BindVertexArray(mVAO_IDs[1]);
            GL.DrawElements(PrimitiveType.Triangles, mSphereModelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);

            m = mCylinderModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            //EditMaterialProperties(new Vector3(0.0215f, 0.1745f, 0.0215f), new Vector3(0.07568f, 0.61424f, 0.07568f), new Vector3(0.633f, 0.727811f, 0.633f), 10f); //10

            GL.BindVertexArray(mVAO_IDs[2]);
            GL.DrawElements(PrimitiveType.Triangles, mCylinderModelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);

            m = mModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            //EditMaterialProperties(new Vector3(0.0215f, 0.1745f, 0.0215f), new Vector3(0.07568f, 0.61424f, 0.07568f), new Vector3(0.633f, 0.727811f, 0.633f), 76.8f); //10

            GL.BindVertexArray(mVAO_IDs[3]);
            GL.DrawElements(PrimitiveType.Triangles, mModelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffers(mVBO_IDs.Length, mVBO_IDs);
            GL.DeleteVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            mShader.Delete();
            base.OnUnload(e);
        }
    }
}
