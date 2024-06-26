﻿#version 330 core

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;

void main()
{
    vec4 color = texture(texture0, texCoord);
    outputColor = vec4(color.x, 0.0, color.zw);
}