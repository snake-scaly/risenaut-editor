using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RisenautEditor
{
    class UndoManager : ObservableObject
    {
        private Stack<IUndoAction> undo;
        private Stack<IUndoAction> redo;
        private IManipulator undo_manipulator;
        private IManipulator redo_manipulator;

        // Position in the undo buffer which corresponds to the Unchanged data state.
        // Four general situations are possible:
        // 1. unchanged_pos is equal to undo.Count. The data is unchanged since the
        //    last call to SetUnchanged().
        // 2. unchanged_pos is less than undo.Count. Data can be reverted to the
        //    Unchanged state by undoing undo.Count - unchanged_pos times.
        // 3. unchanged_pos is greater than undo.Count. Data can be reverted to the
        //    Unchanged state by redoing unchanged_pos - undo.Count times.
        // 4. unchanged_pos is -1. No amount of undoing or redoing can revert to the
        //    Unchanged state. This happens when Manipulator is used to modify data
        //    while undo.Count < unchanged_pos.
        private int unchanged_pos;

        private bool prev_is_changed;
        private bool prev_can_undo;
        private bool prev_can_redo;

        public UndoManager(byte[] game_data)
        {
            undo = new Stack<IUndoAction>();
            redo = new Stack<IUndoAction>();

            undo_manipulator = new Manipulator(game_data, undo);
            redo_manipulator = new Manipulator(game_data, redo);
            Manipulator = new Invalidator(undo_manipulator, this);

            unchanged_pos = -1;
        }

        /// <summary>
        /// Get a manipulator for changing game data.
        /// </summary>
        /// <remarks>
        /// All data changes MUST go through this manipulator. This guarantees that
        /// the undo mechanism functions properly.
        /// </remarks>
        public IManipulator Manipulator { get; private set; }

        public bool IsModified { get { return undo.Count != unchanged_pos; } }
        public bool CanUndo { get { return undo.Count != 0; } }
        public bool CanRedo { get { return redo.Count != 0; } }

        public void SetUnchanged()
        {
            unchanged_pos = undo.Count;
            Notify();
        }

        private void Notify()
        {
            if (IsModified != prev_is_changed)
            {
                prev_is_changed = IsModified;
                RaisePropertyChanged("IsModified");
            }
            if (CanUndo != prev_can_undo)
            {
                prev_can_undo = CanUndo;
                RaisePropertyChanged("CanUndo");
            }
            if (CanRedo != prev_can_redo)
            {
                prev_can_redo = CanRedo;
                RaisePropertyChanged("CanRedo");
            }
        }

        public void Undo()
        {
            if (undo.Count != 0)
            {
                undo.Pop().Undo(redo_manipulator);
                Notify();
            }
        }

        public void Redo()
        {
            if (redo.Count != 0)
            {
                redo.Pop().Undo(undo_manipulator);
                Notify();
            }
        }

        /// <summary>
        /// Invalidate the redo buffer and possibly the Unchanged state.
        /// </summary>
        private void OnExternalDataChange()
        {
            redo.Clear();
            if (unchanged_pos > undo.Count)
            {
                unchanged_pos = -1;
            }
            Notify();
        }

        /// <summary>
        /// An IManipulator adapter that notifies the owner UndoManager about
        /// external data manipulations.
        /// </summary>
        private class Invalidator : IManipulator
        {
            private IManipulator manipulator;
            private UndoManager owner;

            public Invalidator(IManipulator manipulator, UndoManager owner)
            {
                this.manipulator = manipulator;
                this.owner = owner;
            }

            public void SetByte(string name, int offset, byte value)
            {
                manipulator.SetByte(name, offset, value);
                owner.OnExternalDataChange();
            }
        }
    }
}
