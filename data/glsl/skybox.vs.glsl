/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
#version 330 core
out vec3 TexCoords;

layout (location = 0) in vec3 aPos;
uniform mat4 aJvp;

void main()
{
  TexCoords = aPos;
  gl_Position = aJvp * vec4 (aPos * 50, 1.0);
}
