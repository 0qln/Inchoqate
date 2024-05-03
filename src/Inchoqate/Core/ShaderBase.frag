#version 330 core

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;

void main()
{
    outputColor = vec4(texCoord, 0.0f, 1.0f);
    // outputColor = texture(texture0, texCoord);
}