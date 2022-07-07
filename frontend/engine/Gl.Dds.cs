/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */
using OpenTK.Graphics.OpenGL;

namespace frontend.Gl
{
  public sealed class Dds
  {
    private File file;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Mipmaps { get; private set; }
    public FormatType Format { get; private set; }
    public InternalFormat InternalFormat { get => glformat [(int) Format]; }
    private byte[] data;

    static readonly InternalFormat[] glformat =
    new InternalFormat []
    {
      0,
      InternalFormat.CompressedRgbaS3tcDxt1Ext,
      InternalFormat.CompressedRgbaS3tcDxt3Ext,
      InternalFormat.CompressedRgbaS3tcDxt5Ext,
    };

#region Types

    unsafe struct FourCC
    {
      public fixed byte magic [3];
      public fixed byte bit [1];

      public string Magic
      {
        get
        {
          var val = new char [3];
          val [0] = (char) magic [0];
          val [1] = (char) magic [1];
          val [2] = (char) magic [2];
          return new string (val);
        }
      }
    }

    unsafe struct PixelFormat
    {
      public fixed System.UInt32 size [1];
      public fixed System.UInt32 flags [1];
      public FourCC fourcc;
      public fixed System.UInt32 rgb_bits [1];
      public fixed System.UInt32 red_mask [1];
      public fixed System.UInt32 green_mask [1];
      public fixed System.UInt32 blue_mask [1];
      public fixed System.UInt32 alpha_mask [1];
    }

    unsafe struct Header
    {
      public fixed System.UInt32 size [1];
      public fixed System.UInt32 flags [1];
      public fixed System.UInt32 height [1];
      public fixed System.UInt32 width [1];
      public fixed System.UInt32 linearsz [1];
      public fixed System.UInt32 depth [1];
      public fixed System.UInt32 n_mipmaps [1];
      public fixed System.UInt32 reserved_1 [11];
      public PixelFormat format;
      public fixed System.UInt32 caps [1];
      public fixed System.UInt32 caps2 [1];
      public fixed System.UInt32 caps3 [1];
      public fixed System.UInt32 caps4 [1];
      public fixed System.UInt32 reserved_2 [1];
    }

    unsafe struct File
    {
      public FourCC magic;
      public Header header;
    }

    public enum FormatType
    {
      NONE = 0,
      DXT1,
      DXT3,
      DXT5,
    }

#endregion

    public void Load2D (TextureTarget target, bool create)
    {
      var header = file.header;
      var fmt = header.format;

      var width = this.Width;
      var height = this.Height;
      var n_mipmap = this.Mipmaps;
      var format = this.InternalFormat;
      var blocksz = (Format == FormatType.DXT1) ? 8 : 16;
      var pixels = new Memory<byte> (data);
      var pin = pixels.Pin ();
      int i, size, offset = 0;

      for (i = 0; i < n_mipmap && (width > 0 || height > 0); i++)
        {
          size = ((width + 3) / 4) * ((height + 3) / 4) * blocksz;

          if (create)
            unsafe
            {
              var pointer = (IntPtr) pin.Pointer;
              GL.CompressedTexImage2D (target, i, format, width, height, 0, size, pointer + offset);
            }
          else
            {
              var pixel = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
              unsafe
              {
                var pointer = (IntPtr) pin.Pointer;
                GL.CompressedTexSubImage2D (target, i, 0, 0, width, height, pixel, size, pointer + offset);
              }
            }

          offset += size;
          height >>= 1;
          width >>= 1;
        }
    }

    public void Load3D (TextureTarget target, int depth, bool create)
    {
      var header = file.header;
      var fmt = header.format;

      var width = this.Width;
      var height = this.Height;
      var n_mipmap = this.Mipmaps;
      var format = this.InternalFormat;
      var blocksz = (Format == FormatType.DXT1) ? 8 : 16;
      var pixels = new Memory<byte> (data);
      var pin = pixels.Pin ();
      int i, size, offset = 0;

      for (i = 0; i < n_mipmap && (width > 0 || height > 0); i++)
        {
          size = ((width + 3) / 4) * ((height + 3) / 4) * blocksz;

          if (create)
            unsafe
            {
              var pointer = (IntPtr) pin.Pointer;
              GL.CompressedTexImage3D (target, i, format, width, height, depth, 0, size, pointer + offset);
            }
          else
            {
              var pixel = (OpenTK.Graphics.OpenGL.PixelFormat) format;
              unsafe
              {
                var pointer = (IntPtr) pin.Pointer;
                GL.CompressedTexSubImage3D (target, i, 0, 0, depth, width, height, 1, pixel, size, pointer + offset);
              }
            }

          offset += size;
          height >>= 1;
          width >>= 1;
        }
    }

#region Constructors

    static Dds ()
    {
      unsafe
      {
        if (sizeof (FourCC) != 4) throw new Exception ("static assert failed");
        if (sizeof (PixelFormat) != 32) throw new Exception ("static assert failed");
        if (sizeof (Header) != 124) throw new Exception ("static assert failed");
        if (sizeof (File) != 128) throw new Exception ("static assert failed");
      }
    }

    public unsafe Dds (string filename)
    {
      using (var stream = new FileStream (filename, FileMode.Open))
        {
          File file;
          var size = sizeof (FourCC);
          var span = new Span<byte> (&file.magic, size);
          var length = stream.Length;

          stream.Read (span);
          if (file.magic.Magic == "DDS")
            {
              length -= size;
              size = sizeof (Header);
              span = new Span<byte> (&file.header, size);

              stream.Read (span);
              if (file.header.size [0] == size
                && file.header.format.fourcc.Magic == "DXT")
                {
                  length -= size;

                  switch ((char) file.header.format.fourcc.bit [0])
                  {
                  case '1':
                    Format = FormatType.DXT1;
                    break;
                  case '3':
                    Format = FormatType.DXT3;
                    break;
                  case '5':
                    Format = FormatType.DXT5;
                    break;
                  default:
                    var message = "Inconsistent file data";
                    throw new Exception (message);
                  }

                  Width = (int) file.header.width [0];
                  Height = (int) file.header.height [0];
                  Mipmaps = (int) file.header.n_mipmaps [0];
                  this.file = file;

                  data = new byte [length];
                  stream.Read (data);
                  stream.Close ();
                }
              else
                {
                  stream.Close ();
                  var message = "Inconsistent file data";
                  throw new Exception (message);
                }
            }
          else
            {
              stream.Close ();
              var message = $"Invalid head, got '{file.magic.Magic}'";
              throw new Exception (message);
            }
        }
    }

#endregion
  }
}
