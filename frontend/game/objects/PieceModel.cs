/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Graphics.OpenGL;
using Engine;

namespace frontend.Game.Objects
{
  public class PieceModel : Gl.SingleModel
  {
    static readonly string modelPath;

#region Constructors

    private static readonly double textureSize = 256;
    private static readonly double canvasWidth = (480d / 2048d) * textureSize;
    private static readonly double canvasHeight = (420d / 2048d) * textureSize;
    private static readonly double topX = (190d / 2048d) * textureSize;
    private static readonly double topY = (530d / 2048d) * textureSize;
    private static readonly double bottomX = (190d / 2048d) * textureSize;
    private static readonly double bottomY = (1330d / 2048d) * textureSize;
    private static readonly double middleX = (190d / 2048d) * textureSize;
    private static readonly double middleX2 = (750d / 2048d) * textureSize;
    private static readonly double middleY = (717d / 2048d) * textureSize;
    private static readonly double middleW = (70d / 2048d) * textureSize;

    private static Cairo.Surface surface;
    private static Cairo.Context cairo;
    private static Cairo.Matrix matrix;
    private static double lineWidth;

    private void DrawText (string text)
    {
      Cairo.TextExtents ext;
      double fontSize = 12;
      double incSign = 10;
      int increases = 0;
      int reduces = 0;

      while (true)
      {
              cairo.SetFontSize (fontSize);
        ext = cairo.TextExtents (text);
        if (ext.Width > canvasWidth
          || ext.Height > canvasHeight)
          {
            if (incSign > 0)
            {
              incSign = -incSign;
              reduces = 0;
            }
            else
            {
              ++reduces;
              if (reduces > 3)
              {
                throw new Exception ();
              }
            }
          }
        else if (ext.Width < canvasWidth
          && ext.Height < canvasHeight)
          {
            if (incSign < 0)
            {
              if (reduces == 0)
                incSign -= incSign;
              else
                incSign /= -2;
              increases = 0;
            }
            else
            {
              ++increases;
            }
          }

        if (incSign < 0.01)
          break;

        fontSize += incSign;
        if (fontSize < 0)
        {
          throw new Exception ();
        }
      }

      matrix.InitScale (-fontSize, fontSize);
      matrix.X0 += ext.Width;

      cairo.FontMatrix = matrix;
      cairo.TextPath (text);

      matrix.InitIdentity ();
      cairo.Matrix = matrix;
    }

    public PieceModel (params int[] faces)
      : base (modelPath)
    {
      if (faces.Length != 2)
        {
          throw new Exception ("can't handle more than two faces");
        }

      lock (cairo)
      {
        cairo.SetSourceRGBA (1, 1, 1, 1);
        cairo.Paint ();

        cairo.SetSourceRGBA (0, 0, 0, 1);
        cairo.LineWidth = lineWidth;

        cairo.MoveTo ((int) topX, (int) topY);
        DrawText (faces [0].ToString ());
        cairo.Fill ();

        cairo.MoveTo ((int) bottomX, (int) bottomY);
        DrawText (faces [1].ToString ());
        cairo.Fill ();

        cairo.LineWidth = middleW;
        cairo.MoveTo ((int) middleX, (int) middleY);
        cairo.LineTo ((int) middleX2, (int) middleY);
        cairo.Fill ();

        surface.Flush ();

        var tio = GL.GenTexture ();
        var image = (Cairo.ImageSurface) surface;
        var intr = PixelInternalFormat.CompressedRgba;
        var width = (int) image.Width;
        var height = (int) image.Height;
        var fmtr = PixelFormat.Bgra;
        var ptyr = PixelType.UnsignedByte;
        var buffer = image.DataPtr;
        var stride = image.Stride;

        GL.ActiveTexture (TextureUnit.Texture0);
        GL.BindTexture (TextureTarget.Texture2DArray, tio);
        GL.PixelStore (PixelStoreParameter.UnpackAlignment, 1);
        GL.TexImage3D (TextureTarget.Texture2DArray, 0, intr, width, height, 1, 0, fmtr, ptyr, buffer);

        GL.GenerateMipmap (GenerateMipmapTarget.Texture2DArray);
        GL.TexParameter (TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int) All.Linear);
        GL.TexParameter (TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int) All.Linear);
        GL.TexParameter (TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int) All.ClampToEdge);
        GL.TexParameter (TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int) All.ClampToEdge);
        GL.BindTexture (TextureTarget.Texture2DArray, 0);
        GL.ActiveTexture (TextureUnit.Texture0);

        materials.Single ().Material.Textures [0] = tio;
      }
    }

    static PieceModel ()
    {
      var parent = Application.DataDir;
      var relative = "models/piece/scene.gltf";

      modelPath = System.IO.Path.Combine (parent, relative);

      var fmt = Cairo.Format.Argb32;
      var width = (int) textureSize;
      var height = (int) textureSize;
      var slant = Cairo.FontSlant.Normal;
      var weight = Cairo.FontWeight.Normal;
      var fontSize = 12;

      surface = new Cairo.ImageSurface (fmt, width, height);
      cairo = new Cairo.Context (surface);
      matrix = new Cairo.Matrix ();

      lineWidth = cairo.LineWidth;

      cairo.SelectFontFace ("Sans", slant, weight);
      cairo.SetFontSize (fontSize);
      matrix.InitIdentity ();
    }

#endregion
  }
}
