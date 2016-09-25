using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisenautEditor
{
    /// <summary>
    /// Game data manipulator.
    /// </summary>
    /// <remarks>
    /// All game data changes MUST go through this manipulator. It handles
    /// recording of Undo and Redo commands.
    /// </remarks>
    class Manipulator : IManipulator
    {
        private byte[] game_data;
        private Stack<IUndoAction> undo_stack;

        /// <summary>
        /// Create a new Manipulator attached to the given undo stack.
        /// </summary>
        public Manipulator(byte[] game_data, Stack<IUndoAction> undo_stack)
        {
            this.game_data = game_data;
            this.undo_stack = undo_stack;
        }

        public void SetByte(string name, int offset, byte value)
        {
            if (offset < 0 || offset >= game_data.Length)
                throw new ArgumentOutOfRangeException("offset", "Want [0.." + game_data.Length + "), got " + offset);
            if (value != game_data[offset])
            {
                undo_stack.Push(new ByteUndoAction(name, offset, game_data[offset]));
                game_data[offset] = value;
            }
        }
    }
}
