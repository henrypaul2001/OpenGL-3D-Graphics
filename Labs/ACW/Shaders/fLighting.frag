#version 330

uniform vec4 uEyePosition;

uniform sampler2D uWallTextureSampler;
uniform sampler2D uWoodTextureSampler;

in vec4 oNormal;

in vec4 oSurfacePosition;

in vec2 oTexCoords;

in float oIsTexture;

out vec4 FragColour;

struct LightProperties {
	vec4 Position;
	vec3 AmbientLight;
	vec3 DiffuseLight;
	vec3 SpecularLight;
};

uniform LightProperties[3] uLight;

struct MaterialProperties {
	vec3 AmbientReflectivity;
	vec3 DiffuseReflectivity;
	vec3 SpecularReflectivity;
	float Shininess;
};

uniform MaterialProperties uMaterial;

void main()
{
	for(int i = 0; i < 3; ++i)
	{
		vec4 eyeDirection = normalize(uEyePosition - oSurfacePosition);
		vec4 lightDir = normalize(uLight[i].Position - oSurfacePosition);

		vec4 reflectedVector = reflect(-lightDir, oNormal);
		float diffuseFactor = max(dot(oNormal, lightDir), 0);
		float specularFactor = pow(max(dot( reflectedVector, eyeDirection), 0.0), uMaterial.Shininess);

		if (oIsTexture == 1)
		{
			FragColour = texture(uWallTextureSampler, oTexCoords);
		}
		else if (oIsTexture == 2)
		{
			FragColour = texture(uWoodTextureSampler, oTexCoords);
		}
		
		FragColour = FragColour + vec4(uLight[i].AmbientLight * uMaterial.AmbientReflectivity + uLight[i].DiffuseLight 
		* uMaterial.DiffuseReflectivity * diffuseFactor + uLight[i].SpecularLight * uMaterial.SpecularReflectivity * 
		specularFactor, 1);
	}
};
