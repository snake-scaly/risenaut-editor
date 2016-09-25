using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisenautEditor
{
    /// <summary>
    /// Interface for objects manipulating game bytes.
    /// </summary>
    interface IManipulator
    {
        /// <summary>
        /// Set a byte to a certain value.
        /// </summary>
        /// <param name="name">display name of the operation</param>
        /// <param name="offset">byte offset</param>
        /// <param name="value">new byte value</param>
        void SetByte(string name, int offset, byte value);
    }
}
