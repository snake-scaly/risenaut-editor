using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RisenautEditor
{
    class Bgr24Canvas : ICanvas
    {
        private byte[] pixels;
        private int pitch;
        private int width;
        private int height;

        private const int bytes_per_pixel = 3;

        public Bgr24Canvas(byte[] pixels, int pitch, int width, int height)
        {
            if (pitch < width * bytes_per_pixel)
                throw new ArgumentException("pitch, width", "Inconsistent: want pitch >= width, got pitch=" + pitch + ", width=" + width);
            if (pixels.Length < pitch * height)
                throw new ArgumentException("pixels, pitch, height", "Insufficient pixels: want " + (pitch * height) + ", got " + pixels.Length);

            this.pixels = pixels;
            this.pitch = pitch;
            this.width = width;
            this.height = height;
        }

        public void DrawPixel(int x, int y, int color4)
        {
            if (x < 0 || x >= width)
                throw new ArgumentOutOfRangeException("x", "Want [0.." + width + "), got " + x);
            if (y < 0 || y >= height)
                throw new ArgumentOutOfRangeException("y", "Want [0.." + height + "), got " + y);
            if (color4 < 0 || color4 >= palette.Length)
                throw new ArgumentOutOfRangeException("color4", "Want [0..15], got " + color4);

            int offset = pitch * y + x * bytes_per_pixel;
            Color c = palette[color4];
            pixels[offset + 0] = c.B;
            pixels[offset + 1] = c.G;
            pixels[offset + 2] = c.R;
        }

        private static readonly Color[] palette =
        {
            Color.FromRgb(  0,   0,   0),
            Color.FromRgb(217,   0,   0),
            Color.FromRgb(  0, 217,   0),
            Color.FromRgb(217, 217,   0),
            Color.FromRgb(  0,   0, 217),
            Color.FromRgb(217,   0, 217),
            Color.FromRgb(  0, 217, 217),
            Color.FromRgb(217, 217, 217),
            Color.FromRgb( 38,  38,  38),
            Color.FromRgb(255,  38,  38),
            Color.FromRgb( 38, 255,  38),
            Color.FromRgb(255, 255,  38),
            Color.FromRgb( 38,  38, 255),
            Color.FromRgb(255,  38, 255),
            Color.FromRgb( 38, 255, 255),
            Color.FromRgb(255, 255, 255),
        };
    }
}
