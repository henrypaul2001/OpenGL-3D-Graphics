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
    class Light
    {
        public Light(Vector4 lightPos, Vector3 ambientLight, Vector3 diffuseLight, Vector3 specularLight, ref ShaderUtility mShader, ref Matrix4 mView, int index)
        {
            EditLightPosition(lightPos, index, ref mShader, ref mView);
            EditAmbientLight(ambientLight, index, ref mShader);
            EditDiffuseLight(diffuseLight, index, ref mShader);
            EditSpecularLight(specularLight, index, ref mShader);
        }

        public void EditSpecularLight(Vector3 specular, int index, ref ShaderUtility mShader)
        {
            int uSpecularLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[" + index + "].SpecularLight");
            GL.Uniform3(uSpecularLightLocation, specular);
        }

        public void EditDiffuseLight(Vector3 diffuse, int index, ref ShaderUtility mShader)
        {
            int uDiffuseLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[" + index + "].DiffuseLight");
            GL.Uniform3(uDiffuseLightLocation, diffuse);
        }

        public void EditAmbientLight(Vector3 colour, int index, ref ShaderUtility mShader)
        {
            int uAmbientLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[" + index + "].AmbientLight");
            GL.Uniform3(uAmbientLightLocation, colour);
        }

        public void EditLightPosition(Vector4 pLightPosition, int index, ref ShaderUtility mShader, ref Matrix4 mView)
        {
            int uLightPositionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[" + index + "].Position");
            pLightPosition = Vector4.Transform(pLightPosition, mView);
            GL.Uniform4(uLightPositionLocation, pLightPosition);
        }
    }
}