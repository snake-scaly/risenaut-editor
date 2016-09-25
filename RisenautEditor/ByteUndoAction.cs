using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RisenautEditor
{
    class ByteUndoAction : IUndoAction
    {
        private int offset;
        private byte value;

        public ByteUndoAction(string name, int offset, byte value)
        {
            Name = name;
            this.offset = offset;
            this.value = value;
        }

        public string Name { get; private set; }

        public void Undo(IManipulator manipulator)
        {
            manipulator.SetByte(Name, offset, value);
        }
    }
}
