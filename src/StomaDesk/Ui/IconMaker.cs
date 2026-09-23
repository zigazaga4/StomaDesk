using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace StomaDesk.Ui
{
    /// <summary>
    /// Builds a Windows .ico in memory from a GDI+ drawing. Every size is stored as a 32-bit bitmap with an
    /// alpha channel, the icon format that every Windows version and Mono can read.
    /// </summary>
    public static class IconMaker
    {
        public static Icon Create(Action<Graphics, RectangleF> draw, params int[] sizes)
        {
            var images = new byte[sizes.Length][];
            for (int i = 0; i < sizes.Length; i++)
                images[i] = Dib(draw, sizes[i]);

            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((short)0);               // reserved
                writer.Write((short)1);               // 1 = icon
                writer.Write((short)sizes.Length);

                int offset = 6 + 16 * sizes.Length;
                for (int i = 0; i < sizes.Length; i++)
                {
                    byte side = (byte)(sizes[i] >= 256 ? 0 : sizes[i]);
                    writer.Write(side);               // width
                    writer.Write(side);               // height
                    writer.Write((byte)0);            // no palette
                    writer.Write((byte)0);            // reserved
                    writer.Write((short)1);           // colour planes
                    writer.Write((short)32);          // bits per pixel
                    writer.Write(images[i].Length);
                    writer.Write(offset);
                    offset += images[i].Length;
                }
                foreach (byte[] image in images)
                    writer.Write(image);

                writer.Flush();
                stream.Position = 0;
                return new Icon(stream);
            }
        }

        /// <summary>
        /// One icon image: a BITMAPINFOHEADER (height doubled, as .ico requires), the BGRA pixels bottom-up,
        /// then an all-zero AND mask, since the alpha channel already says what is transparent.
        /// </summary>
        private static byte[] Dib(Action<Graphics, RectangleF> draw, int size)
        {
            using (var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Transparent);
                    draw(g, new RectangleF(0f, 0f, size, size));
                }

                int maskStride = (size + 31) / 32 * 4;
                using (var stream = new MemoryStream())
                {
                    var writer = new BinaryWriter(stream);
                    writer.Write(40);                 // header size
                    writer.Write(size);
                    writer.Write(size * 2);
                    writer.Write((short)1);
                    writer.Write((short)32);
                    writer.Write(0);                  // no compression
                    writer.Write(size * size * 4 + maskStride * size);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);

                    BitmapData data = bitmap.LockBits(new Rectangle(0, 0, size, size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    try
                    {
                        var row = new byte[size * 4];
                        for (int y = size - 1; y >= 0; y--)
                        {
                            Marshal.Copy(new IntPtr(data.Scan0.ToInt64() + (long)y * data.Stride), row, 0, row.Length);
                            writer.Write(row);
                        }
                    }
                    finally
                    {
                        bitmap.UnlockBits(data);
                    }

                    writer.Write(new byte[maskStride * size]);
                    writer.Flush();
                    return stream.ToArray();
                }
            }
        }
    }
}
