using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Labs.ACW
{
    class Camera
    {
        private static bool cameraIsControllable = true;
        private static Matrix4 lastPos;
        public static void InitialiseCamera(ref Matrix4 mView, ref ShaderUtility mShader)
        {
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);

            Vector4 eyePosition = Vector4.Transform(new Vector4(0, 0, 0, 1), mView);
            int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
            GL.Uniform4(uEyePosition, eyePosition);
        }

        public static void ChangeCameraType(ref Matrix4 mView, ref ShaderUtility mShader)
        {
            if (cameraIsControllable)
            {
                // Set static camera
                cameraIsControllable = false;

                lastPos = mView;

                Matrix4 origin = Matrix4.CreateTranslation(0, -1.5f, 0);
                mView = origin * Matrix4.CreateTranslation(0f, -2f, -15f);
                int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                GL.UniformMatrix4(uView, true, ref mView);

                Vector4 eyePosition = Vector4.Transform(new Vector4(0, 0, 0, 1), mView);
                int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
                GL.Uniform4(uEyePosition, eyePosition);
            }
            else
            {
                // Set controllable camera
                cameraIsControllable = true;
                mView = lastPos;
                mView = mView * Matrix4.CreateTranslation(0.0f, 0f, 0f);
                int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                GL.UniformMatrix4(uView, true, ref mView);

                Vector4 eyePosition = Vector4.Transform(new Vector4(0, 0, 0, 1), mView);
                int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
                GL.Uniform4(uEyePosition, eyePosition);
            }
        }

        public static void RotateRight(ref Matrix4 mView, ref ShaderUtility mShader)
        {
            if (cameraIsControllable)
            {
                mView = mView * Matrix4.CreateRotationY(0.025f);
                int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                GL.UniformMatrix4(uView, true, ref mView);

                Vector4 eyePosition = Vector4.Transform(new Vector4(0, 0, 0, 1), mView);
                int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
                GL.Uniform4(uEyePosition, eyePosition);
            }
        }

        public static void RotateLeft(ref Matrix4 mView, ref ShaderUtility mShader)
        {
            if (cameraIsControllable)
            {
                mView = mView * Matrix4.CreateRotationY(-0.025f);
                int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                GL.UniformMatrix4(uView, true, ref mView);

                Vector4 eyePosition = Vector4.Transform(new Vector4(0, 0, 0, 1), mView);
                int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
                GL.Uniform4(uEyePosition, eyePosition);
            }
        }

        public static void MoveBack(ref Matrix4 mView, ref ShaderUtility mShader)
        {
            if (cameraIsControllable)
            {
                mView = mView * Matrix4.CreateTranslation(0.0f, 0.0f, -0.05f);
                int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                GL.UniformMatrix4(uView, true, ref mView);

                Vector4 eyePosition = Vector4.Transform(new Vector4(0, 0, 0, 1), mView);
                int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
                GL.Uniform4(uEyePosition, eyePosition);
            }
        }

        public static void MoveForward(ref Matrix4 mView, ref ShaderUtility mShader)
        {
            if (cameraIsControllable)
            {
                mView = mView * Matrix4.CreateTranslation(0.0f, 0.0f, 0.05f);
                int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
                GL.UniformMatrix4(uView, true, ref mView);

                Vector4 eyePosition = Vector4.Transform(new Vector4(0, 0, 0, 1), mView);
                int uEyePosition = GL.GetUniformLocation(mShader.ShaderProgramID, "uEyePosition");
                GL.Uniform4(uEyePosition, eyePosition);
            }
        }
    }
}