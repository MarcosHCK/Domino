/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace frontend.Gl
{
  public class SingleModel : Model, IDrawable
  {
    private List<Group> groups;
    public bool Visible { get; set; }

    struct Group
    {
      public Material material;
      public int[] counts;
      public int[] indices;
      public int[] vertices;
    }

    public void Draw (Frame frame)
    {
      if (Visible)
      {
        var target = BufferTarget.ElementArrayBuffer;
        var pencil = frame.Pencil;

        pencil.BindBuffer (vbo);
        GL.BindBuffer (target, ebo);

        foreach (var group in groups)
        {
          frame.Material = group.material;
          GL.MultiDrawElementsBaseVertex<int>
          (PrimitiveType.Triangles,
           group.counts,
           DrawElementsType.UnsignedInt,
           group.indices,
           group.counts.Length,
           group.vertices);
        }

        GL.BindBuffer (target, 0);
        pencil.UnbindBuffer ();
      }
    }

    public SingleModel (string filename)
      : base (filename)
    {
      groups = new List<Group> ();

      foreach (var materialGroup in materials)
        {
          if (materialGroup == null)
            continue;

          var group = new Group ();
          var counts = new List <int> ();
          var indices = new List <int> ();
          var vertices = new List <int> ();

          foreach (var mesh in materialGroup.Meshes)
          {
            if (mesh == null)
              continue;

            counts.Add (mesh.n_indices);
            indices.Add (mesh.indeces_offset);
            vertices.Add (mesh.vertices_offset);
          }

          if (counts.Count == 0)
            continue;

          group.material = materialGroup.Material;
          group.counts = counts.ToArray ();
          group.indices = indices.ToArray ();
          group.vertices = vertices.ToArray ();
          groups.Add (group);
        }
    }
  }
}
