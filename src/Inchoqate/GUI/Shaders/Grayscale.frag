#version 330 core

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform float intensity
uniform vec3 grayscaleWeights;;

void main()
{
    vec4 color = texture(texture0, texCoord);
    float gray = dot(color.rgb, grayscaleWeights);
    vec3 gray3 = vec3(gray);
    vec3 finalColor = mix(color.rgb, gray3, intensity);
    outputColor = vec4(finalColor, color.a;
}