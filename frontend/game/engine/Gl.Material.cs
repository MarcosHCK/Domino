/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Graphics.OpenGL;
using Assimp;

namespace frontend.Gl
{
  public sealed class Material
  {
    
    private static bool storage = false;
    public float Shininess { get; private set; }
    public int [] Textures { get; private set; }

#region Data blocks

    public static readonly
    TextureType[] textureTypes =
    new TextureType []
    {
      TextureType.Diffuse,
      TextureType.Specular,
      TextureType.Normals,
      TextureType.Height,
    };

    public static readonly
    string[] samplerNames =
    new string []
    {
      "aDiffuse",
      "aSpecular",
      "aNormals",
      "aHeight",
    };

#endregion

    public static void BindUnits (int pid)
    {
      int unit = 0;
      foreach (var name in samplerNames)
      {
        var loc =
        GL.GetUniformLocation (pid, name);
        GL.Uniform1 (loc, unit++);
      }
    }

#region Constructor

    static string LocateTexture (string basedir, Assimp.Material loader, TextureType type, int idx)
    {
      TextureSlot slot;
      loader.GetMaterialTexture (type, idx, out slot);
      var filepath = slot.FilePath;

      if (filepath != null)
        return Path.Combine (basedir, filepath);
      else
        {
          if (slot.TextureIndex != idx)
            return LocateTexture (basedir, loader, type, slot.TextureIndex);
          else
            {
              throw new NotImplementedException ();
            }
        }
    }

    public Material ()
    {
      var
      combined = GL.GetInteger (GetPName.MaxCombinedTextureImageUnits);
      if (combined < textureTypes.Length)
        throw new Exception ("Too few image units");

      storage |= Game.Window.CheckVersion (4, 2);
      storage |= Game.Window.CheckExtension ("ARB_texture_storage");
      Textures = new int [textureTypes.Length];
      GL.GenTextures (Textures.Length, Textures);
    }

    public Material (Assimp.Material loader, string basedir) : this ()
    {
      Shininess = loader.Shininess;

      int i = 0, j;
      foreach (var tio in Textures)
      {
        GL.ActiveTexture (TextureUnit.Texture0);
        GL.BindTexture (TextureTarget.Texture2DArray, tio);

        try
        {
          var type = textureTypes [i++];
          var n_images = loader.GetMaterialTextureCount (type);

          if (n_images == 0)
            continue;

          var images = new Dds [n_images];
          var format = (InternalFormat) (-1);
          var width = (int) -1;
          var height = (int) -1;
          var mipmaps = (int) -1;

          for (j = 0; j < n_images; j++)
          {
            var path = LocateTexture (basedir, loader, type, j);
            var image = new Dds (path);

            images [j] = image;

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

          if (storage)
          {
            var format_ = (SizedInternalFormat) format;
            GL.TexStorage3D (TextureTarget3d.Texture2DArray, mipmaps, format_, width, height, n_images);
          }

          for (int k = 0; k < n_images; k++)
          {
            images [k].Load3D (TextureTarget.Texture2DArray, k, !storage);
          }

          GL.TexParameter (TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int) All.LinearMipmapLinear);
          GL.TexParameter (TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int) All.Linear);
          GL.TexParameter (TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int) All.ClampToEdge);
          GL.TexParameter (TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int) All.ClampToEdge);
        }
        catch (System.Exception)
        {
          GL.BindTexture (TextureTarget.Texture2DArray, 0);
          GL.ActiveTexture (TextureUnit.Texture0);
          throw;
        } finally
        {
          GL.BindTexture (TextureTarget.Texture2DArray, 0);
        }
      }
    }

#endregion
  }
}