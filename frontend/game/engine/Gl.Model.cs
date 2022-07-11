/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Assimp;

namespace frontend.Gl
{
  public abstract class Model
  {
    protected int vbo;
    protected int ebo;

    protected MaterialGroup[] materials;
    protected Mesh[] meshes;

#region Type

    protected class MaterialGroup
    {
      public Material Material { get; private set; }
      public List<Mesh> Meshes { get; private set; }

      public MaterialGroup (Assimp.Material loader, string basedir)
      {
        Material = new Material (loader, basedir);
        Meshes = new List<Mesh> ();
      }
    }

    protected class Mesh
    {
      public int n_indices;
      public int indeces_offset;
      public int vertices_offset;
    }

#endregion

#region private API

    private string LocateTexture (string basedir, Assimp.Material material, TextureType type, int idx)
    {
      TextureSlot slot;
      material.GetMaterialTexture (type, idx, out slot);
      var filepath = slot.FilePath;

      if (filepath != null)
        return Path.Combine (basedir, filepath);
      else
        {
          if (slot.TextureIndex != idx)
            return LocateTexture (basedir, material, type, slot.TextureIndex);
          else
            {
              throw new NotImplementedException ();
            }
        }
    }

#endregion

#region Constructor

    public Model (string filename)
    {
      IntPtr vertices_, indices_;
      Span<Pencil.Vertex> vertices;
      Span<int> indices;
      int n_vertices, n_indices;
      int i, j, _v, _i;

      var import = new AssimpContext ();
      var steps  = PostProcessSteps.CalculateTangentSpace;
          steps |= PostProcessSteps.GenerateSmoothNormals;
          steps |= PostProcessSteps.GenerateNormals;
          steps |= PostProcessSteps.RemoveRedundantMaterials;
          steps |= PostProcessSteps.ImproveCacheLocality;
          steps |= PostProcessSteps.GenerateUVCoords;
          steps |= PostProcessSteps.FlipUVs;
          steps |= PostProcessSteps.JoinIdenticalVertices;
          steps |= PostProcessSteps.OptimizeGraph;
          steps |= PostProcessSteps.OptimizeMeshes;
          steps |= PostProcessSteps.Triangulate;
      var scene = import.ImportFile (filename, steps);

      var basedir = Path.GetDirectoryName (filename);
      if (basedir == null)
        basedir = "./";

      n_vertices = 0;
      n_indices = 0;

      foreach (var mesh in scene.Meshes)
        {
          n_vertices += mesh.VertexCount;
          if (mesh.VertexCount != mesh.Vertices.Count)
            throw new Exception ();

          foreach (var face in mesh.Faces)
          {
            n_indices += face.IndexCount;
            if (face.IndexCount != face.Indices.Count)
              throw new Exception ();
          }
        }

      vbo = GL.GenBuffer ();
      ebo = GL.GenBuffer ();

      GL.BindBuffer (BufferTarget.ArrayBuffer, vbo);
      GL.BufferData (BufferTarget.ArrayBuffer, n_vertices * Pencil.VertexSize, IntPtr.Zero, BufferUsageHint.StaticDraw);
      GL.BindBuffer (BufferTarget.ElementArrayBuffer, ebo);
      GL.BufferData (BufferTarget.ElementArrayBuffer, n_indices * sizeof (int), IntPtr.Zero, BufferUsageHint.StaticDraw);

      unsafe
      {
        vertices_ = GL.MapBuffer (BufferTarget.ArrayBuffer, BufferAccess.WriteOnly);
        indices_ = GL.MapBuffer (BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly);
        vertices = new Span<Pencil.Vertex> ((void*) vertices_, n_vertices);
        indices = new Span<int> ((void*) indices_, n_indices);
      }

      vertices.Fill (default (Pencil.Vertex));
      indices.Fill (default (int));

      materials = new MaterialGroup [scene.MaterialCount];
      meshes = new Mesh [scene.MeshCount];

      try
      {
        if (scene.Meshes.Count != scene.MeshCount)
          throw new Exception ();

        _v = 0;
        _i = 0;

        i = 0;
        foreach (var mesh in scene.Meshes)
          {
            meshes [i] = new Mesh ();
            meshes [i].n_indices = 0;
            meshes [i].indeces_offset = _i;
            meshes [i].vertices_offset = _v;

            j = 0;
            foreach (var vertex_ in mesh.Vertices)
            {
              Pencil.Vertex vertex;

              unsafe
              {
                vertex.position [0] = vertex_.X;
                vertex.position [1] = vertex_.Y;
                vertex.position [2] = vertex_.Z;

                if (mesh.HasNormals)
                {
                  var normal = mesh.Normals [j];
                  vertex.normal [0] = normal.X;
                  vertex.normal [1] = normal.Y;
                  vertex.normal [2] = normal.Z;
                }

                if (mesh.HasTextureCoords (0))
                {
                  switch (mesh.UVComponentCount [0])
                  {
                    case 3:
                      vertex.uvw [2] = mesh.TextureCoordinateChannels [0] [j].Z;
                      goto case 2;
                    case 2:
                      vertex.uvw [1] = mesh.TextureCoordinateChannels [0] [j].Y;
                      goto case 1;
                    case 1:
                      vertex.uvw [0] = mesh.TextureCoordinateChannels [0] [j].X;
                      break;
                    default:
                      throw new Exception ();
                  }

                  if (mesh.HasTangentBasis)
                  {
                    var tangent = mesh.Tangents [j];
                    vertex.tangent [0] = tangent.X;
                    vertex.tangent [1] = tangent.Y;
                    vertex.tangent [2] = tangent.Z;
                    var bitangent = mesh.BiTangents [j];
                    vertex.bitangent [0] = bitangent.X;
                    vertex.bitangent [1] = bitangent.Y;
                    vertex.bitangent [2] = bitangent.Z;
                  }
                }
              }

              vertices [_v++] = vertex;
              ++j;
            }

            foreach (var face in mesh.Faces)
            {
              foreach (var index in face.Indices)
                indices [_i++] = index;
              meshes [i].n_indices += face.IndexCount;
            }

            var mat = mesh.MaterialIndex;
            if (mat >= 0)
              {
                if (materials[mat] == null)
                {
                  var loader = scene.Materials [mat];
                  var material = new MaterialGroup (loader, basedir);
                  materials [mat] = material;
                }

                var
                list = materials [mat].Meshes;
                list.Add (meshes [i]);
              }

            ++i;
          }
      }
      catch (Exception)
      {
        GL.UnmapBuffer (BufferTarget.ArrayBuffer);
        GL.UnmapBuffer (BufferTarget.ElementArrayBuffer);
        GL.BindBuffer (BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer (BufferTarget.ElementArrayBuffer, 0);
        throw;
      }
      finally
      {
        GL.UnmapBuffer (BufferTarget.ArrayBuffer);
        GL.UnmapBuffer (BufferTarget.ElementArrayBuffer);
        GL.BindBuffer (BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer (BufferTarget.ElementArrayBuffer, 0);
      }
    }

    ~ Model ()
    {
      GL.DeleteBuffer (vbo);
      GL.DeleteBuffer (ebo);
    }

#endregion
  }
}
