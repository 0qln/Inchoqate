#version 330
#extension GL_ARB_explicit_uniform_location : enable

// Input vertex attributes
in vec3 vertexPosition;
in vec2 vertexTexCoord;
in vec3 vertexNormal;
in vec4 vertexColor;

// Input uniform values
uniform mat4 mvp;
layout(location = 2) uniform vec2 textureBounds;
layout(location = 3) uniform vec2 previewBounds;
layout(location = 4) uniform vec2 texturePosition;

// Output vertex attributes (to fragment shader)
out vec2 fragTexCoord;
out vec4 fragColor;

void main()
{
    // Send vertex attributes to fragment shader
    // Preserve texture aspect ratio

    fragTexCoord = 
        vertexTexCoord 
        * (textureBounds / previewBounds) 
        - (texturePosition / previewBounds);

    fragColor = vertexColor;

    // Calculate final vertex position
    gl_Position = mvp*vec4(vertexPosition, 1.0);
}
