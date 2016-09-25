using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisenautEditor
{
    /// <summary>
    /// A common undo action interface.
    /// </summary>
    interface IUndoAction
    {
        /// <summary>
        /// Get this action name, for display purposes.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Undo the action.
        /// </summary>
        /// <param name="manipulator">manipulator to use for undoing</param>
        void Undo(IManipulator manipulator);
    }
}
