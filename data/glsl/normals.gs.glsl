/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
#version 330 core
layout (triangles) in;
layout (line_strip, max_vertices = 6) out;

const float MAGNITUDE = 0.4;
uniform mat4 aProjection;

void GenLine (int index)
{
  gl_Position = aProjection * gl_in [index].gl_Position;
  EmitVertex();

  gl_Position = aProjection * (gl_in [index].gl_Position + vec4 (/* normal */, 0.0) * MAGNITUDE);
  EmitVertex();

  EndPrimitive();
}

void main()
{    
  GenerateLine(0); // first vertex normal
  GenerateLine(1); // second vertex normal
  GenerateLine(2); // third vertex normal
}
