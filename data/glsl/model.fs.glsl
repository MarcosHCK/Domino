/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
#version 330 core
out vec4 FragColor;
in vec3 Normals;
in vec3 TexCoords;
in vec3 Tangent;
in vec3 Bitangent;

uniform sampler2DArray aDiffuse;
uniform sampler2DArray aSpecular;
uniform sampler2DArray aNormals;
uniform sampler2DArray aHeight;

void main()
{
  FragColor = texture (aDiffuse, TexCoords);
}
