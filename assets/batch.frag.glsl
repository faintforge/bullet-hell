#version 400 core

layout (location = 0) out vec4 FragColor;

in vec2 uv;
in vec4 color;
in float textureIndex;

uniform sampler2D textures[32];

void main() {
    // FragColor = vec4(uv, 0.0, 1.0);
    // FragColor = texture(textures[textureIndex], uv) * color * textureIndex;
    FragColor = texture(textures[int(textureIndex)], uv) * color;
}
