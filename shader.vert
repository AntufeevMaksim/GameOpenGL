#version 440 core

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 tex_coord;

out vec2 texCoord;
out vec3 Normal;
out vec3 FragPos;
out vec3 Specular;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;




void main(void)
{
    texCoord = tex_coord;
    Normal = normal * mat3(model);
    FragPos = vec3(vec4(position, 1.0f) * model);
    gl_Position = vec4(position, 1.0) * model * view * projection;
}