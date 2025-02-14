#version 330 core

layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aUV;
layout (location = 2) in vec4 aColor;
layout (location = 3) in float aTextureIndex;

out vec2 uv;
out vec4 color;
out float textureIndex;

uniform mat4 projection;

void main() {
    uv = aUV;
    color = aColor;
    textureIndex = aTextureIndex;
    gl_Position = projection * vec4(aPos, 0.0, 1.0);
}
