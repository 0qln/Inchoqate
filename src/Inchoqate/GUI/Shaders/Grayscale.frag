#version 330 core

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform float _intensity;
uniform vec3 _weights;

void main()
{
    vec4 color = texture(texture0, texCoord);
    float gray = dot(color.rgb, _weights);
    vec3 gray3 = vec3(gray);
    vec3 finalColor = mix(color.rgb, gray3, _intensity);
    outputColor = vec4(finalColor, color.a);
}