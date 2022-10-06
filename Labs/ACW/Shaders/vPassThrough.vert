#version 330

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

in float isTexture;

in vec3 vPosition;

in vec3 vNormal;

in vec2 vTexCoords;

out vec4 oNormal;

out vec4 oSurfacePosition;

out vec2 oTexCoords;

out float oIsTexture;

void main()
{
	gl_Position = vec4(vPosition, 1) * uModel * uView * uProjection;
	oSurfacePosition = vec4(vPosition, 1) * uModel * uView;
	oTexCoords = vTexCoords;
	oNormal = vec4(normalize(vNormal * mat3(transpose(inverse(uModel * uView)))), 1);
	oIsTexture = isTexture;
};