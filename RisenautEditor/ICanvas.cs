using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisenautEditor
{
    /// <summary>
    /// A canvas for drawing level elements.
    /// </summary>
    interface ICanvas
    {
        /// <summary>
        /// Draw a pixel on the canvas.
        /// </summary>
        /// <param name="x">absolute X coordinate</param>
        /// <param name="y">absolute Y coordinate</param>
        /// <param name="color4">4-bit Agat color to draw the pixel with</param>
        void DrawPixel(int x, int y, int color4);
    }
}
