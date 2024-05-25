#version 330 core

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;

void main()
{
    vec4 color = texture(texture0, texCoord);
    float gray = (color.x + color.y + color.z) / 3.0;
    outputColor = vec4(gray, gray, gray, color.w);
}