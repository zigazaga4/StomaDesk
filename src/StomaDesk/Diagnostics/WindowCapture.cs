using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace StomaDesk.Diagnostics
{
    /// <summary>
    /// Takes a picture of a window for the UI smoke test.
    /// On Windows (and Wine) it copies from the window's own device context, because copying from the screen
    /// returns a black image on Wayland desktops. Mono on Linux has no user32.dll; there the window is an
    /// X11 window, so its pixels are read with XGetImage.
    /// </summary>
    internal static class WindowCapture
    {
        public static Bitmap Take(Form form)
        {
            return Environment.OSVersion.Platform == PlatformID.Unix ? FromX11(form.Handle) : FromGdi(form);
        }

        private static Bitmap FromGdi(Form form)
        {
            Rectangle bounds = form.Bounds;
            var image = new Bitmap(bounds.Width, bounds.Height);
            IntPtr source = Gdi.GetWindowDC(form.Handle);
            try
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    IntPtr target = g.GetHdc();
                    Gdi.BitBlt(target, 0, 0, bounds.Width, bounds.Height, source, 0, 0, Gdi.SrcCopy);
                    g.ReleaseHdc(target);
                }
            }
            finally
            {
                Gdi.ReleaseDC(form.Handle, source);
            }
            return image;
        }

        private static Bitmap FromX11(IntPtr window)
        {
            IntPtr display = X11.XOpenDisplay(IntPtr.Zero);
            if (display == IntPtr.Zero)
                throw new InvalidOperationException("Nu se poate deschide conexiunea X11 (variabila DISPLAY).");

            try
            {
                // Mono's Form.Handle is the client window, which sits inside an outer window of its own and can
                // be larger than it. XGetImage refuses any area that is not visible, so read the outer window,
                // clipped to the screen.
                IntPtr root, parent, children;
                uint count;
                X11.XQueryTree(display, window, out root, out parent, out children, out count);
                if (children != IntPtr.Zero)
                    X11.XFree(children);
                if (parent != IntPtr.Zero && parent != root)
                    window = parent;

                Rectangle visible = VisibleArea(display, window, root);
                if (visible.Width <= 0 || visible.Height <= 0)
                    throw new InvalidOperationException("Fereastra nu este vizibilă pe ecran.");

                IntPtr ximage = X11.XGetImage(display, window, visible.X, visible.Y, (uint)visible.Width, (uint)visible.Height, X11.AllPlanes, X11.ZPixmap);
                if (ximage == IntPtr.Zero)
                    throw new InvalidOperationException("XGetImage nu a returnat nicio imagine.");

                try
                {
                    return CopyPixels(ximage, visible.Width, visible.Height);
                }
                finally
                {
                    X11.XDestroyImage(ximage);
                }
            }
            finally
            {
                X11.XCloseDisplay(display);
            }
        }

        /// <summary>The part of the window that lies on the screen, in window coordinates.</summary>
        private static Rectangle VisibleArea(IntPtr display, IntPtr window, IntPtr root)
        {
            IntPtr ignoredRoot;
            int x, y;
            uint width, height, border, depth;
            X11.XGetGeometry(display, window, out ignoredRoot, out x, out y, out width, out height, out border, out depth);
            uint screenWidth, screenHeight;
            X11.XGetGeometry(display, root, out ignoredRoot, out x, out y, out screenWidth, out screenHeight, out border, out depth);

            int left, top;
            IntPtr child;
            X11.XTranslateCoordinates(display, window, root, 0, 0, out left, out top, out child);

            Rectangle onScreen = Rectangle.Intersect(
                new Rectangle(left, top, (int)width, (int)height),
                new Rectangle(0, 0, (int)screenWidth, (int)screenHeight));
            onScreen.Offset(-left, -top);
            return onScreen;
        }

        /// <summary>
        /// Reads the XImage struct by offset: four ints, the data pointer, five ints, then bytes_per_line and
        /// bits_per_pixel. A 24-bit X visual stores pixels as B, G, R, unused, the same order as Format32bppRgb.
        /// </summary>
        private static Bitmap CopyPixels(IntPtr ximage, int width, int height)
        {
            int p = IntPtr.Size;
            IntPtr data = Marshal.ReadIntPtr(ximage, 16);
            int bytesPerLine = Marshal.ReadInt32(ximage, 16 + p + 20);
            int bitsPerPixel = Marshal.ReadInt32(ximage, 16 + p + 24);
            if (bitsPerPixel != 32)
                throw new NotSupportedException("Captura X11 suportă doar ecrane pe 32 de biți, nu " + bitsPerPixel + ".");

            var image = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            BitmapData target = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            try
            {
                var row = new byte[width * 4];
                for (int line = 0; line < height; line++)
                {
                    Marshal.Copy(new IntPtr(data.ToInt64() + (long)line * bytesPerLine), row, 0, row.Length);
                    Marshal.Copy(row, 0, new IntPtr(target.Scan0.ToInt64() + (long)line * target.Stride), row.Length);
                }
            }
            finally
            {
                image.UnlockBits(target);
            }
            return image;
        }

        private static class Gdi
        {
            public const int SrcCopy = 0x00CC0020;

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr window);

            [DllImport("user32.dll")]
            public static extern int ReleaseDC(IntPtr window, IntPtr dc);

            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool BitBlt(IntPtr target, int x, int y, int width, int height, IntPtr source, int sourceX, int sourceY, int operation);
        }

        private static class X11
        {
            private const string Lib = "libX11.so.6";
            public const int ZPixmap = 2;
            public static readonly UIntPtr AllPlanes = UIntPtr.Size == 8 ? new UIntPtr(ulong.MaxValue) : new UIntPtr(uint.MaxValue);

            [DllImport(Lib)]
            public static extern IntPtr XOpenDisplay(IntPtr name);

            [DllImport(Lib)]
            public static extern int XCloseDisplay(IntPtr display);

            [DllImport(Lib)]
            public static extern int XGetGeometry(IntPtr display, IntPtr drawable, out IntPtr root, out int x, out int y,
                out uint width, out uint height, out uint border, out uint depth);

            [DllImport(Lib)]
            public static extern int XQueryTree(IntPtr display, IntPtr window, out IntPtr root, out IntPtr parent,
                out IntPtr children, out uint count);

            [DllImport(Lib)]
            public static extern int XTranslateCoordinates(IntPtr display, IntPtr source, IntPtr target, int x, int y,
                out int targetX, out int targetY, out IntPtr child);

            [DllImport(Lib)]
            public static extern int XFree(IntPtr data);

            [DllImport(Lib)]
            public static extern IntPtr XGetImage(IntPtr display, IntPtr drawable, int x, int y, uint width, uint height,
                UIntPtr planeMask, int format);

            [DllImport(Lib)]
            public static extern int XDestroyImage(IntPtr image);
        }
    }
}
