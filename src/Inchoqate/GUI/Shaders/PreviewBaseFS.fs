#version 400
#extension GL_ARB_explicit_uniform_location : enable


// Constants
#define ID_NONE             0
#define ID_GRAY_SCALE       1
#define ID_NO_RED           2
#define ID_GAUSSIAN_BLUR    3
#define ID_BOX_BLUR         4

#define TAU                 6.28318530718



// Output 
out vec4 finalColor;



// Input 

// From Vertex Shader
in vec2 fragTexCoord;
in vec4 fragColor;

// From backend 
uniform sampler2D Image;
uniform sampler2D Filter;
layout(location = 3) uniform vec2 previewBounds;
layout(location = 20) uniform int PipelineWidth;



// Functions

void GrayScale(inout vec4 color) {
    float gray = color.x / 3 + color.y / 3 + color.z / 3;
    color = vec4(gray, gray, gray, 1);   
}


void NoRed(inout vec4 color) {
    color = vec4(0, color.y, color.z, 1);   
}


// https://xorshaders.weebly.com/tutorials/blur-shaders-5-part-2
// [default] directions = 16
// [default] quality = 3
// [default] size = 8
void GaussianBlur(inout vec4 color, in float directions, in float quality, in float size) {
    vec2 radius = size / previewBounds;
    vec4 result = vec4(color);
    for (float d = 0.0f; d < TAU; d += TAU / directions) {
        for (float i = 1.0f / quality; i <= 1.0f; i += 1.0f / quality) {
            // TODO: this needs to be applied to the filtered image, but doesn't 
            // no idea how to do this :D
            result += texture(Image, fragTexCoord + vec2(cos(d), sin(d)) * radius * i);
        }
    }

    color = result / quality * directions - 15.0;
}


void BoxBlur(inout vec4 color, in int size) {
    int factors = 0;
    for (int x = -size/2; x <= size/2; x++) {
        for (int y = -size/2; y <= size/2; y++) {
            vec2 normPos = fragTexCoord + vec2(x, y) / previewBounds;
            vec2 pos = normPos * previewBounds;
            if (pos.x < size/2 || 
                pos.y < size/2 || 
                pos.x + size/2 >= previewBounds.x ||
                pos.y + size/2 >= previewBounds.y) {
                continue;
            }
            color += texture(Image, normPos);
            factors++;
        }
    }

    if (factors != 0) {
        color /= factors;
    }
}


// [DEPRICATED: FORMAT] returns a value between 0 and 255 inclusively.
int indexFilters8bit(int n) {
    // TOOD: this fails sometimes of floating point inprecision,
    // that's the reason for the 'n+0.5'. This might cause some other bugs... 
    // If this fails once, the wrong filter will be selected and boom boom brr!
    // In case this happens, we can resort to throwing away 1/4 of the pixel and
    // this have a more precise indexing via only storing one value per texel.
    float numTexel = float(n+0.5f) / 4.0f / PipelineWidth;
    int offTexel = n % 4;
    vec4 FilterTexel = texture(Filter, vec2(numTexel, 0));
    float value;
    if (offTexel == 0) { value = FilterTexel.x; }
    else if (offTexel == 1) { value = FilterTexel.y; }
    else if (offTexel == 2) { value = FilterTexel.z; }
    else { value = FilterTexel.a; }
    return int(value * 255.0f);
}

// https://en.wikipedia.org/wiki/Single-precision_floating-point_format
float fromParts(int parts) {
    int sign = (parts >> 31) & 1;
    int exponent = (parts >> 23) & 0xFF;
    int fraction = parts & 0x7FFFFF;
    float value = (sign == 0) ? (1.0f) : (-1.0f);
    value *= pow(2, exponent - 127);
    // Instead of summing and scaling each bit, we can scale all at once.
    value *= 1 + fraction / float(0x800000); 
    return value;
}

// Returns the vec4 at index n interpreted as i32.
int Decode_i32(int n) {
    float numTexel = float(n+0.5f) / PipelineWidth;
    vec4 FilterTexel = texture(Filter, vec2(numTexel, 0));
    return int(
        int(FilterTexel.x * 255.0f) << 24 |
        int(FilterTexel.y * 255.0f) << 16 |
        int(FilterTexel.z * 255.0f) << 8  |
        int(FilterTexel.w * 255.0f)
    );
}

// Returns the first half of the texel at index 'n' (0xFFFF0000).
int Decode_i16_1(int n) {
    float numTexel = float(n+0.5f) / PipelineWidth;
    vec4 FilterTexel = texture(Filter, vec2(numTexel, 0));
    return (
        int(FilterTexel.x * 255.0f) << 8 |
        int(FilterTexel.y * 255.0f) << 0
    );
}

// Returns the second half of the texel at index 'n' (0x0000FFFF).
int Decode_i16_2(int n) {
    float numTexel = float(n+0.5f) / PipelineWidth;
    vec4 FilterTexel = texture(Filter, vec2(numTexel, 0));
    return (
        int(FilterTexel.z * 255.0f) << 8  |
        int(FilterTexel.w * 255.0f) << 0
    );
}

// Returns the vec4 at index n interpreted as f32.
float Decode_f32(int n) {
    float numTexel = float(n+0.5f) / PipelineWidth;
    vec4 FilterTexel = texture(Filter, vec2(numTexel, 0));
    return fromParts(int(
        int(FilterTexel.x * 255.0f) << 24 |
        int(FilterTexel.y * 255.0f) << 16 |
        int(FilterTexel.z * 255.0f) << 8  |
        int(FilterTexel.w * 255.0f)
    ));
}

int f_round_i (float value) {
    return int(round(value) + 0.5f);
}

void main()
{
    // Texel color fetching from texture sampler
    vec4 texelColor = texture(Image, fragTexCoord);


    // Decode filter pipeline
    int index = 0;
    while (index < PipelineWidth) {
        // Apply Filter.
        int filterID = Decode_i32(index++);
        switch(filterID) {

            case ID_GRAY_SCALE: 
                GrayScale(texelColor);
                break;

            case ID_NO_RED:
                NoRed(texelColor);
                break;

            case ID_GAUSSIAN_BLUR:
                GaussianBlur(
                    texelColor, 
                    // directions
                    Decode_f32(index++),
                    // quality
                    Decode_f32(index++),
                    // size
                    Decode_f32(index++));
                break;

            case ID_BOX_BLUR:
                BoxBlur(
                    texelColor,
                    // size
                    Decode_i32(index++));
                break;
        }
    }


    // Set output    
    finalColor = texelColor;
}
