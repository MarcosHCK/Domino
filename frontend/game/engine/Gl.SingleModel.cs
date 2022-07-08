/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace frontend.Gl
{
  public sealed class SingleModel : Model, IDrawable, ILocalizable
  {
    private List<Group> groups;
    public Vector3 Position { get; set; }
    public bool Visible { get; set; }

    struct Group
    {
      public Model.Texture texture;
      public int[] counts;
      public int[] indices;
      public int[] vertices;
    }

    public void Draw (Pencil pencil)
    {
      if (Visible)
      {
        var target = BufferTarget.ElementArrayBuffer;

        pencil.BindBuffer (vbo);
        GL.BindBuffer (target, ebo);

        foreach (var group in groups)
        {
          group.texture.Switch ();
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

      foreach (var texture in textures)
        {
          if (texture == null)
            continue;

          var group = new Group ();
          var counts = new List <int> ();
          var indices = new List <int> ();
          var vertices = new List <int> ();

          foreach (var mesh in texture.meshes)
          {
            if (mesh == null)
              continue;

            counts.Add (mesh.n_indices);
            indices.Add (mesh.indeces_offset);
            vertices.Add (mesh.vertices_offset);
          }

          if (counts.Count == 0)
            continue;

          group.texture = texture;
          group.counts = counts.ToArray ();
          group.indices = indices.ToArray ();
          group.vertices = vertices.ToArray ();
          groups.Add (group);
        }
    }
  }
}
