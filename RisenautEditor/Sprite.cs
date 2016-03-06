using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RisenautEditor
{
    /// <summary>
    /// A rectangular, upside-down sprite.
    /// </summary>
    class Sprite
    {
        private byte[] game_data;
        private int sprite_offset;

        /// <summary>
        /// For use by GameFile.
        /// </summary>
        internal Sprite(byte[] game_data, int sprite_offset, int width, int height)
        {
            this.game_data = game_data;
            this.sprite_offset = sprite_offset;
            PixelWidth = width;
            PixelHeight = height;
        }

        public int PixelWidth { get; private set; }
        public int PixelHeight { get; private set; }

        public void Draw(ICanvas canvas, int x, int y)
        {
            int offset = sprite_offset;

            for (int sy = y + PixelHeight - 1; sy >= y; sy--)
            {
                int sx = x;
                int n = PixelWidth;

                while (n != 0)
                {
                    int b = game_data[offset++];
                    canvas.DrawPixel(sx++, sy, b >> 4);
                    if (--n != 0)
                    {
                        canvas.DrawPixel(sx++, sy, b & 15);
                        --n;
                    }
                }
            }
        }
    }
}
