/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
#version 330 core

/* Pencil layout */
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec3 vNormal;
layout (location = 2) in vec3 vTexCoords;
layout (location = 3) in vec3 vTangent;
layout (location = 4) in vec3 vBitangent;

/* Output */
out vec3 Position;
out vec3 Normal;
out vec3 TexCoords;
out mat3 Tbn;

/* Uniforms */
layout (std140) uniform aMatrices
{
  mat4 aProjection;
  mat4 aProjectionInverse;
  mat4 aView;
  mat4 aViewInverse;
  mat4 aModel;
  mat4 aModelInverse;
  mat4 aJvp;
  mat4 aMvp;
};

mat3 CalculateTbn ()
{
  vec3 t = normalize (vec3 (aModel * vec4 (vTangent, 0.0)));
  vec3 b = normalize (vec3 (aModel * vec4 (vBitangent, 0.0)));
  vec3 n = normalize (vec3 (aModel * vec4 (vNormal, 0.0)));
  return mat3 (t, b, n);
}

void main()
{
  Position = vec3 (aModel * vec4 (vPos, 1.0));
  Normal = mat3 (transpose (aModelInverse)) * vNormal;
  TexCoords = vTexCoords;
  Tbn = CalculateTbn ();

  gl_Position = aMvp * vec4 (vPos, 1.0);
}
