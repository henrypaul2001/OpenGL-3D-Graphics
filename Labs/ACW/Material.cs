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
    class Material
    {
        Vector3 mAmbientReflect, mDiffuseReflect, mSpecularReflect;
        float mShininess;

        public Material(Vector3 ambientReflect, Vector3 diffuseReflect, Vector3 specularReflect, float shininess)
        {
            SetAmbient(ambientReflect);
            SetDiffuse(diffuseReflect);
            SetSpecular(specularReflect);
            SetShininess(shininess);
        }

        public void SetAmbient(Vector3 ambient)
        {
            mAmbientReflect = ambient;
        }
        public void SetDiffuse(Vector3 diffuse)
        {
            mDiffuseReflect = diffuse;
        }
        public void SetSpecular(Vector3 specular)
        {
            mSpecularReflect = specular;
        }
        public void SetShininess(float shininess)
        {
            mShininess = shininess;
        }

        public void SetActiveMaterial(ref ShaderUtility mShader)
        {
            int uAmbientReflectivityLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.AmbientReflectivity");
            GL.Uniform3(uAmbientReflectivityLocation, mAmbientReflect);

            int uDiffuseReflectivityLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.DiffuseReflectivity");
            GL.Uniform3(uDiffuseReflectivityLocation, mDiffuseReflect);

            int uSpecularReflectivityLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.SpecularReflectivity");
            GL.Uniform3(uSpecularReflectivityLocation, mSpecularReflect);

            int uShineLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.Shininess");
            GL.Uniform1(uShineLocation, mShininess);
        }
    }
}
