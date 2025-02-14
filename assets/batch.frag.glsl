#version 330 core

layout (location = 0) out vec4 FragColor;

in vec2 uv;
in vec4 color;

uniform sampler2D tex;

void main() {
    // FragColor = vec4(uv, 0.0, 1.0);
    FragColor = texture(tex, uv) * color;
}
