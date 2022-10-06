using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Labs.ACW
{
    public class ACWWindow : GameWindow
    {
        public ACWWindow()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Assessed Coursework",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        private ShaderUtility mShader;
        private int[] mVBO_IDs = new int[10];
        private int[] mVAO_IDs = new int[6];
        private Matrix4 mView, mGroundModel, mCylinderModel, mCylinderModelRight, mCylinderModelLeft, mMonsterModel, mCubeModel, mSphereModel;

        private Light redLight;
        private Light greenLight;
        private Light blueLight;

        #region textures
        private int mWall_TextureID;
        private int mWood_TextureID;
        #endregion

        #region models
        private ModelUtility cylinderModelUtility;
        private ModelUtility monsterModelUtility;
        private ModelUtility sphereModelUtility;
        #endregion

        #region materials
        private Material mChrome = new Material(new Vector3(0.25f, 0.25f, 0.25f), new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0.774597f, 0.774597f, 0.774597f), 76.8f);
        private Material mObsidian = new Material(new Vector3(0.05375f, 0.05f, 0.06625f), new Vector3(0.18275f, 0.17f, 0.22525f), new Vector3(0.332741f, 0.328634f, 0.346435f), 38.4f);
        private Material mBrass = new Material(new Vector3(0.329412f, 0.223529f, 0.027451f), new Vector3(0.780392f, 0.568627f, 0.113725f), new Vector3(0.992157f, 0.941176f, 0.807843f), 27.89743616f);
        private Material mSilver = new Material(new Vector3(0.19225f, 0.19225f, 0.19225f), new Vector3(0.50754f, 0.50754f, 0.50754f), new Vector3(0.508273f, 0.508273f, 0.508273f), 51.2f);
        private Material mWood = new Material(new Vector3(0.3f, 0.2f, 0.1f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.6f, 0.6f, 0.5f), 32f); //32
        private Material mStone = new Material(new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.7f, 0.7f, 0.7f), 10f);
        #endregion

        protected override void OnLoad(EventArgs e)
        {
            // Set some GL state
            GL.ClearColor(Color4.White);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            mShader = new ShaderUtility(@"ACW/Shaders/vPassThrough.vert", @"ACW/Shaders/fLighting.frag");
            GL.UseProgram(mShader.ShaderProgramID);
            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            int vNormalLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vNormal");
            int vTextureCoordsLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vTexCoords");
            int vIsTextureLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "isTexture");

            #region TextureLoading
            //Wall
            Bitmap TextureBitmap;
            BitmapData TextureData;
            // image address https://www.tilingtextures.com/wp-content/uploads/2019/06/53b43254-0143-0096-scaled.jpg
            string filepath = @"ACW/stone.jpg";
            if (System.IO.File.Exists(filepath))
            {
                TextureBitmap = new Bitmap(filepath);
                TextureBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                TextureData = TextureBitmap.LockBits(new Rectangle(0, 0, TextureBitmap.Width, TextureBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            }
            else
            {
                throw new Exception("Could not find file " + filepath);
            }

            int uTextureSamplerLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uWallTextureSampler");
            GL.Uniform1(uTextureSamplerLocation, 0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.GenTextures(1, out mWall_TextureID);
            GL.BindTexture(TextureTarget.Texture2D, mWall_TextureID);
            GL.TexImage2D(TextureTarget.Texture2D,
            0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height,
            0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
            PixelType.UnsignedByte, TextureData.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat);
            TextureBitmap.UnlockBits(TextureData);

            //Floor
            // image address https://www.goodtextures.com/cache/c821c3ec/av8a7d12f4d47f4c0dd96.jpg
            filepath = @"ACW/wood.jpg";
            if (System.IO.File.Exists(filepath))
            {
                TextureBitmap = new Bitmap(filepath);
                //TextureBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                TextureData = TextureBitmap.LockBits(new System.Drawing.Rectangle(0, 0, TextureBitmap.Width, TextureBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            }
            else
            {
                throw new Exception("Could not find file " + filepath);
            }

            int uWoodTextureSamplerLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uWoodTextureSampler");
            GL.Uniform1(uWoodTextureSamplerLocation, 1);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.GenTextures(1, out mWood_TextureID);
            GL.BindTexture(TextureTarget.Texture2D, mWood_TextureID);
            GL.TexImage2D(TextureTarget.Texture2D,
            0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height,
            0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
            PixelType.UnsignedByte, TextureData.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Linear);
            TextureBitmap.UnlockBits(TextureData);
            #endregion

            GL.GenVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);

            #region floor

            float[] floorVertices = new float[] {-10, 0, -10, 0, 1, 0, 0, 1, 0,
                                                 -10, 0, 10, 0, 1, 0, 0, 0, 0,
                                                  10, 0, -10, 0, 1, 0, 1, 1, 0,
                                                 -10, 0, 10, 0, 1, 0, 0, 0, 0,
                                                  10, 0, 10, 0, 1, 0, 1, 0, 0,
                                                  10, 0, -10, 0, 1, 0, 1, 1, 0 };

            GL.BindVertexArray(mVAO_IDs[0]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(floorVertices.Length * sizeof(float)), floorVertices, BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (floorVertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 9 * sizeof(float), 3 * sizeof(float));

            GL.EnableVertexAttribArray(vTextureCoordsLocation);
            GL.VertexAttribPointer(vTextureCoordsLocation, 2, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));

            GL.EnableVertexAttribArray(vIsTextureLocation);
            GL.VertexAttribPointer(vIsTextureLocation, 1, VertexAttribPointerType.Float, false, 9 * sizeof(float), 8 * sizeof(float));
            #endregion

            #region cylinder

            cylinderModelUtility = ModelUtility.LoadModel(@"Utility/Models/cylinder.bin");

            GL.BindVertexArray(mVAO_IDs[1]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(cylinderModelUtility.Vertices.Length * sizeof(float)), cylinderModelUtility.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[2]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(cylinderModelUtility.Indices.Length * sizeof(float)), cylinderModelUtility.Indices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (cylinderModelUtility.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (cylinderModelUtility.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));

            #endregion

            #region monster
            monsterModelUtility = ModelUtility.LoadModel(@"Utility/Models/model.bin");

            GL.BindVertexArray(mVAO_IDs[2]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[3]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(monsterModelUtility.Vertices.Length * sizeof(float)), monsterModelUtility.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[4]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(monsterModelUtility.Indices.Length * sizeof(float)), monsterModelUtility.Indices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (monsterModelUtility.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (monsterModelUtility.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));
            #endregion
            
            #region cube
            float[] cubeVertices = new float[] { -1, 1, -1, 1, 1, 1,
                                                 -1, -1, -1, 1, 1, 1,
                                                 1, 1, -1, 1, 1, 1,
                                                 1, -1, -1, 1, 1, 1,
                                                 1, 1, 1, 1, 1, 1,
                                                 1, -1, 1, 1, 1, 1,
                                                 -1, 1, 1, 1, 1, 1,
                                                 -1, -1, 1, 1, 1, 1 };

            int[] cubeIndices = new int[] { 0, 1, 2,
                                            2, 1, 3,

                                            2, 3, 4,
                                            4, 3, 5,

                                            4, 5, 6,
                                            6, 5, 7,

                                            6, 7, 0,
                                            0, 7, 1,

                                            1, 7, 3,
                                            3, 7, 5,

                                            6, 0, 4,
                                            4, 0, 2 };

            GL.BindVertexArray(mVAO_IDs[3]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[5]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(cubeVertices.Length * sizeof(float)), cubeVertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[6]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(cubeIndices.Length * sizeof(int)), cubeIndices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (cubeVertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (cubeIndices.Length * sizeof(int) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));
            #endregion

            #region sphere
            sphereModelUtility = ModelUtility.LoadModel(@"Utility/Models/sphere.bin");

            GL.BindVertexArray(mVAO_IDs[4]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[7]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sphereModelUtility.Vertices.Length * sizeof(float)), sphereModelUtility.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[8]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sphereModelUtility.Indices.Length * sizeof(float)), sphereModelUtility.Indices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (sphereModelUtility.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (sphereModelUtility.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));
            #endregion

            #region wall

            float[] wallVertices = new float[] {-10, 10, -10, 0, 1, 0, 0, 1, 0,
                                                -10, 0, -10, 0, 1, 0, 0, 0, 0,
                                                10, 10, -10, 0, 1, 0, 1, 1, 0,
                                                -10, 0, -10, 0, 1, 0, 0, 0, 0,
                                                10, 0, -10, 0, 1, 0, 1, 0, 0,
                                                10, 10, -10, 0, 1, 0, 1, 1, 0};

            GL.BindVertexArray(mVAO_IDs[5]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[9]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(wallVertices.Length * sizeof(float)), wallVertices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (wallVertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 9 * sizeof(float), 3 * sizeof(float));

            GL.EnableVertexAttribArray(vTextureCoordsLocation);
            GL.VertexAttribPointer(vTextureCoordsLocation, 2, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));

            GL.EnableVertexAttribArray(vIsTextureLocation);
            GL.VertexAttribPointer(vIsTextureLocation, 1, VertexAttribPointerType.Float, false, 9 * sizeof(float), 8 * sizeof(float));
            #endregion

            GL.BindVertexArray(0);

            // Camera stuff
            mView = Matrix4.CreateTranslation(0, -1.5f, 0);
            Camera.InitialiseCamera(ref mView, ref mShader);

            // Projection stuff
            int uProjectionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uProjection");
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 50);
            GL.UniformMatrix4(uProjectionLocation, true, ref projection);

            #region lights
            redLight = new Light(new Vector4(10, 6, -5, 1), new Vector3(0.2f, 0.2f, 0.2f), new Vector3(1f, 0f, 0f), new Vector3(1f, 0f, 0f), ref mShader, ref mView, 0);
            greenLight = new Light(new Vector4(0, 6, -2, 1), new Vector3(0.2f, 0.2f, 0.2f), new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f), ref mShader, ref mView, 1);
            blueLight = new Light(new Vector4(-8, 6, -5, 1), new Vector3(0.2f, 0.2f, 0.2f), new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 1f), ref mShader, ref mView, 2);
            #endregion

            mGroundModel = Matrix4.CreateTranslation(0, 0, -5);
            mCylinderModel = Matrix4.CreateTranslation(0, 1, -5);
            mCylinderModelRight = Matrix4.CreateTranslation(5, 1, -5);
            mCylinderModelLeft = Matrix4.CreateTranslation(-5, 1, -5);

            mMonsterModel = Matrix4.CreateTranslation(-0.12f, 3, -5);
            Vector3 t = mMonsterModel.ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            mMonsterModel = mMonsterModel * inverseTranslation * Matrix4.CreateRotationY(-1.7f) * translation;

            mCubeModel = Matrix4.CreateTranslation(-5, 4, -5);
            mSphereModel = Matrix4.CreateTranslation(5, 4, -5);

            base.OnLoad(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyChar == 'w')
            {
                Camera.MoveForward(ref mView, ref mShader);
            }
            else if (e.KeyChar == 's')
            {
                Camera.MoveBack(ref mView, ref mShader);
            }
            else if (e.KeyChar == 'a')
            {
                Camera.RotateLeft(ref mView, ref mShader);
            }
            else if (e.KeyChar == 'd')
            {
                Camera.RotateRight(ref mView, ref mShader);
            }
            else if (e.KeyChar == 'c')
            {
                Camera.ChangeCameraType(ref mView, ref mShader);
            }
            redLight.EditLightPosition(new Vector4(10, 6, -5, 1), 0, ref mShader, ref mView);
            greenLight.EditLightPosition(new Vector4(0, 6, -2, 1), 1, ref mShader, ref mView);
            blueLight.EditLightPosition(new Vector4(-10, 6, -5f, 1), 2, ref mShader, ref mView);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(this.ClientRectangle);
            if (mShader != null)
            {
                int uProjectionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uProjection");
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 50);
                GL.UniformMatrix4(uProjectionLocation, true, ref projection);
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
 	        base.OnUpdateFrame(e);

            // Spin cube
            Vector3 t = mCubeModel.ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            mCubeModel = mCubeModel * inverseTranslation * Matrix4.CreateRotationY(0.02f) * translation;

            // Spin sphere
            t = mSphereModel.ExtractTranslation();
            translation = Matrix4.CreateTranslation(t);
            inverseTranslation = Matrix4.CreateTranslation(-t);
            mSphereModel = mSphereModel * inverseTranslation * Matrix4.CreateRotationY(-0.02f) * translation;

            // Spin armadillo
            t = mMonsterModel.ExtractTranslation();
            translation = Matrix4.CreateTranslation(t);
            inverseTranslation = Matrix4.CreateTranslation(-t);
            mMonsterModel = mMonsterModel * inverseTranslation * Matrix4.CreateRotationY(0.002f) * translation;
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

            mWood.SetActiveMaterial(ref mShader);

            // Floor
            GL.BindVertexArray(mVAO_IDs[0]);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 6);

            mSilver.SetActiveMaterial(ref mShader);

            // Middle cylinder
            Matrix4 m = mCylinderModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            GL.BindVertexArray(mVAO_IDs[1]);
            GL.DrawElements(PrimitiveType.Triangles, cylinderModelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Right cylinder
            m = mCylinderModelRight * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            GL.DrawElements(PrimitiveType.Triangles, cylinderModelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Left cylinder
            m = mCylinderModelLeft * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            GL.DrawElements(PrimitiveType.Triangles, cylinderModelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);

            mBrass.SetActiveMaterial(ref mShader);

            // Armadillo Monster
            m = mMonsterModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            GL.BindVertexArray(mVAO_IDs[2]);
            GL.DrawElements(PrimitiveType.Triangles, monsterModelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);

            mObsidian.SetActiveMaterial(ref mShader);

            // Floating cube
            m = mCubeModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            GL.BindVertexArray(mVAO_IDs[3]);
            GL.DrawElements(PrimitiveType.Triangles, 48, DrawElementsType.UnsignedInt, 0);

            // Set new material
            mChrome.SetActiveMaterial(ref mShader);

            // Floating sphere
            m = mSphereModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            GL.BindVertexArray(mVAO_IDs[4]);
            GL.DrawElements(PrimitiveType.Triangles, sphereModelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);

            mStone.SetActiveMaterial(ref mShader);

            // Walls

            // North wall
            m = mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            GL.BindVertexArray(mVAO_IDs[5]);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 6);

            // East wall
            Vector3 t = mGroundModel.ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            mGroundModel = mGroundModel * inverseTranslation * Matrix4.CreateRotationY(-1.5708f) * translation;

            m = mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            GL.BindVertexArray(mVAO_IDs[5]);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 6);

            // South wall
            t = mGroundModel.ExtractTranslation();
            translation = Matrix4.CreateTranslation(t);
            inverseTranslation = Matrix4.CreateTranslation(-t);
            mGroundModel = mGroundModel * inverseTranslation * Matrix4.CreateRotationY(-1.5708f) * translation;

            m = mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            GL.BindVertexArray(mVAO_IDs[5]);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 6);

            // West wall
            t = mGroundModel.ExtractTranslation();
            translation = Matrix4.CreateTranslation(t);
            inverseTranslation = Matrix4.CreateTranslation(-t);
            mGroundModel = mGroundModel * inverseTranslation * Matrix4.CreateRotationY(-1.5708f) * translation;

            m = mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);

            GL.BindVertexArray(mVAO_IDs[5]);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 6);

            mGroundModel = Matrix4.CreateTranslation(0, 0, -5);

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
            GL.DeleteTexture(mWall_TextureID); 
            GL.DeleteTexture(mWood_TextureID);
            mShader.Delete();
            base.OnUnload(e);
        }
    }
}
