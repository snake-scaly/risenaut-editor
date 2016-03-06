﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RisenautEditor
{
    public class LevelView : UserControl
    {
        private Level level;
        private ImageSource rendered_level;
        private IReadOnlyCollection<Sprite> blocks;

        private const int block_width = 8;
        private const int block_height = 8;

        internal Level Level
        {
            get { return level; }
            set
            {
                level = value;
                RenderLevel();
                InvalidateVisual();
            }
        }

        internal IReadOnlyCollection<Sprite> Blocks
        {
            get { return blocks; }
            set
            {
                blocks = value;
                RenderLevel();
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(Brushes.Black, null, new Rect(0, 0, ActualWidth, ActualHeight));

            const double aspect = 4.0 / 3.0;
            Rect level_rect;
            if (ActualWidth / ActualHeight <= aspect)
            {
                double hh = ActualWidth / aspect;
                level_rect = new Rect(0, (ActualHeight - hh) / 2, ActualWidth, hh);
            }
            else
            {
                double ww = ActualHeight * aspect;
                level_rect = new Rect((ActualWidth - ww) / 2, 0, ww, ActualHeight);
            }
            drawingContext.DrawImage(rendered_level, level_rect);

            double tile_width = level_rect.Width / Level.Width;
            double tile_height = level_rect.Height / Level.Height;

            for (int y = 0; y < Level.Height; y++)
            {
                for (int x = 0; x < Level.Width; x++)
                {
                    int b = Level.GetTile(x, y);
                    if (b < 0 || b >= Blocks.Count)
                    {
                        Rect rect = new Rect(level_rect.X + tile_width * x, level_rect.Y + tile_height * y, tile_width, tile_height);
                        const int padding = 2;
                        rect.Inflate(-padding, -padding);
                        if (!rect.IsEmpty)
                        {
                            drawingContext.DrawRectangle(null, new Pen(Brushes.Red, 1), rect);
                            rect.Inflate(-padding, -padding);
                            if (!rect.IsEmpty)
                            {
                                var str = string.Format("{0:X2}", b);
                                var ft = new FormattedText(
                                    str,
                                    CultureInfo.GetCultureInfo("en-us"),
                                    FlowDirection.LeftToRight,
                                    new Typeface("Arial"),
                                    10,
                                    Brushes.White);
                                drawingContext.DrawText(ft, rect.TopLeft);
                            }
                        }
                    }
                }
            }
        }

        private void RenderLevel()
        {
            if (Level == null || Blocks == null)
                return;

            //var canvas = new DrawingContextCanvas(drawingContext, ActualWidth, ActualHeight);
            int screen_width = Level.Width * block_width;
            int screen_height = Level.Height * block_height;
            const int bpp = 3;
            var pixels = new byte[bpp * screen_width * screen_height];
            var canvas = new Bgr24Canvas(pixels, bpp * screen_width, screen_width, screen_height);

            for (int y = 0; y < Level.Height; y++)
            {
                for (int x = 0; x < Level.Width; x++)
                {
                    int b = Level.GetTile(x, y);
                    if (b >= 0 && b < Blocks.Count)
                    {
                        Blocks.ElementAt(b).Draw(canvas, x * block_width, y * block_height);
                    }
                }
            }

            var bmp = new WriteableBitmap(screen_width, screen_height, 96, 96, PixelFormats.Bgr24, null);

            bmp.Lock();
            bmp.WritePixels(new Int32Rect(0, 0, screen_width, screen_height), pixels, bpp * screen_width, 0);
            bmp.Unlock();

            rendered_level = bmp;
        }
    }
}
