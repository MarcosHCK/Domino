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

    protected Texture[] textures;
    protected Mesh[] meshes;

#region Types

    protected class Texture
    {
      public List<Mesh> meshes { get; private set; }
      public int [] tios {get; private set; }
      private bool multiple = false;
      private bool direct = false;

      public int diffuse { get => tios [0]; }
      public int specular { get => tios [1]; }
      public int normals { get => tios [2]; }
      public int height { get => tios [3]; }

      public static readonly string [] uniforms =
      new string []
      {
        "aDiffuse",
        "aSpecular",
        "aNormals",
        "aHeight",
      };

      public static readonly TextureType [] aitype =
      new TextureType []
      {
        TextureType.Diffuse,
        TextureType.Specular,
        TextureType.Normals,
        TextureType.Height,
      };

      public void Switch ()
      {
        if (multiple)
          GL.BindTextures (0, tios.Length, tios);
        else if (direct)
          {
            int i;
            for (i = 0; i < tios.Length; i++)
            {
              GL.BindTextureUnit (i, tios [i]);
            }
          }
        else
          {
            int i;
            for (i = 0; i < tios.Length; i++)
            {
              GL.ActiveTexture (TextureUnit.Texture0 + i);
              GL.BindTexture (TextureTarget.Texture2DArray, tios [i]);
            }
          }
      }

      public Texture ()
      {
        var
        combined = GL.GetInteger (GetPName.MaxCombinedTextureImageUnits);
        if (combined < uniforms.Length)
          throw new Exception ("Too few image units");

        meshes = new List<Mesh> ();
        tios = new int [uniforms.Length];

        multiple = GameWindow.CheckVersion (4, 4);
        multiple ^= GameWindow.CheckExtension ("ARB_multi_bind");
        direct = GameWindow.CheckVersion (4, 5);
        direct ^= GameWindow.CheckExtension ("ARB_direct_state_access");

        GL.GenTextures (tios.Length, tios);
      }

      static Texture ()
      {
        if (uniforms.Length != aitype.Length)
          throw new Exception ();
      }
    }

    protected class Mesh
    {
      public int n_indices;
      public int indeces_offset;
      public int vertices_offset;
    }

#endregion

#region Static API

    public static void BindUnits (Program program)
    {
      bool
      separate  = GameWindow.CheckVersion (4, 1);
      separate |= GameWindow.CheckExtension ("ARB_separate_shader_objects");
      int unit = 0;

      if (!separate)
        {
          program.Use ();
        }

      foreach (var name in Texture.uniforms)
      {
        var loc = program.Uniform (name);
        if (separate)
          program.SetUniform (loc, unit++);
        else
          GL.Uniform1 (loc, unit++);
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
          steps |= PostProcessSteps.GenerateUVCoords;
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
        vertices_ = GL.MapBuffer (BufferTarget.ArrayBuffer, BufferAccess.ReadWrite);//BufferAccess.WriteOnly);
        indices_ = GL.MapBuffer (BufferTarget.ElementArrayBuffer, BufferAccess.ReadWrite);//BufferAccess.WriteOnly);
        vertices = new Span<Pencil.Vertex> ((void*) vertices_, n_vertices);
        indices = new Span<int> ((void*) indices_, n_indices);
      }

      vertices.Fill (default (Pencil.Vertex));
      indices.Fill (default (int));

      textures = new Texture [scene.MaterialCount];
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
                if (textures[mat] == null)
                {
                  var texture = new Texture ();
                  var material = scene.Materials [mat];
                  var tios = texture.tios;
                  textures[mat] = texture;

                  j = 0;
                  foreach (var tio in tios)
                  {
                    GL.ActiveTexture (TextureUnit.Texture0);
                    GL.BindTexture (TextureTarget.Texture2DArray, tio);

                    try
                    {
                      var type = Texture.aitype [j++];
                      var n_images = material.GetMaterialTextureCount (type);

                      if (n_images == 0)
                        continue;

                      var images = new Dds [n_images];
                      var format = (InternalFormat) (-1);
                      var width = (int) -1;
                      var height = (int) -1;
                      var mipmaps = (int) -1;

                      for (int k = 0; k < n_images; k++)
                      {
                        TextureSlot slot;
                        material.GetMaterialTexture (type, k, out slot);
                        var path = Path.Combine (basedir, slot.FilePath);
                        var image = new Dds (path);

                        images [k] = image;

                        if (format == (InternalFormat) (-1))
                          format = image.InternalFormat;
                        else if (format != image.InternalFormat)
                          throw new Exception ("Imcompatible material textures");

                        if (width == -1)
                          width = image.Width;
                        else if (width != image.Width)
                          throw new Exception ("Imcompatible material textures");

                        if (height == -1)
                          height = image.Height;
                        else if (height != image.Height)
                          throw new Exception ("Imcompatible material textures");

                        if (mipmaps == -1)
                          mipmaps = image.Mipmaps;
                        else if (mipmaps != image.Mipmaps)
                          throw new Exception ("Imcompatible material textures");
                      }

                      bool create = false;
                        create |= GameWindow.CheckVersion (4, 2);
                        create |= GameWindow.CheckExtension ("ARB_texture_storage");

                      if (create)
                        {
                          var format_ = (SizedInternalFormat) format;
                          GL.TexStorage3D (TextureTarget3d.Texture2DArray, mipmaps, format_, width, height, n_images);
                        }

                      for (int k = 0; k < n_images; k++)
                      {
                        images [k].Load3D (TextureTarget.Texture2DArray, k, !create);
                      }

                      GL.TexParameter (TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int) All.LinearMipmapLinear);
                      GL.TexParameter (TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int) All.Linear);
                      GL.TexParameter (TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int) All.ClampToEdge);
                      GL.TexParameter (TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int) All.ClampToEdge);
                    }
                    catch { GL.BindTexture (TextureTarget.Texture2DArray, 0); throw; }
                    finally { GL.BindTexture (TextureTarget.Texture2DArray, 0); } 
                  }
                }

                var
                list = textures [mat].meshes;
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
