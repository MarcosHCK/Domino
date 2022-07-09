/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
#version 330 core
out vec3 Normals;
out vec3 TexCoords;
out vec3 Tangent;
out vec3 Bitangent;

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec3 aTexCoords;
layout (location = 3) in vec3 aTangent;
layout (location = 4) in vec3 aBitangent;
uniform mat4 aJvp;
uniform mat4 aMvp;

void main()
{
  TexCoords = aTexCoords;
  gl_Position = aMvp * vec4 (aPos, 1.0);
}
