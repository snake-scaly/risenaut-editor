using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RisenautEditor
{
    class DrawingContextCanvas : ICanvas
    {
        private DrawingContext context;
        private Size pixel_size;
        private Point offset;

        private const int logical_width = 128;
        private const int logical_height = 128;
        private const double aspect = 4.0 / 3.0;

        public DrawingContextCanvas(DrawingContext context, double width, double height)
        {
            this.context = context;

            if (width / height <= aspect)
            {
                double h = width / aspect;
                pixel_size = new Size(width / logical_width, h / logical_height);
                offset = new Point(0, (height - h) / 2);

            }
            else
            {
                double w = height * aspect;
                pixel_size = new Size(w / logical_width, height / logical_height);
                offset = new Point((width - w) / 2, 0);
            }
        }

        public void DrawPixel(int x, int y, int color4)
        {
            if (x < 0 || x >= logical_width)
                throw new ArgumentOutOfRangeException("x", "Want [0..127], got " + x);
            if (y < 0 || y >= logical_height)
                throw new ArgumentOutOfRangeException("y", "Want [0..127], got " + y);
            if (color4 < 0 || color4 >= palette.Length)
                throw new ArgumentOutOfRangeException("color4", "Want [0..15], got " + color4);

            var rect = new Rect(GetPixelOrigin(x, y), pixel_size);
            var fill = new SolidColorBrush(palette[color4]);
            context.DrawRectangle(fill, null, rect);
        }

        public Point GetPixelOrigin(int x, int y)
        {
            var pos = new Vector(pixel_size.Width * x, pixel_size.Height * y);
            return offset + pos;
        }

        public Rect GetPixelRect(int x, int y)
        {
            return new Rect(GetPixelOrigin(x, y), pixel_size);
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
