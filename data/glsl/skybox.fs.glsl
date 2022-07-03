/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
#version 330 core
out vec4 FragColor;
in vec3 TexCoords;

uniform samplerCube aSkybox;

void main()
{    
  FragColor = texture (aSkybox, TexCoords);
}
