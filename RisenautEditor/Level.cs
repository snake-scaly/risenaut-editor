﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RisenautEditor
{
    public class Level : ObservableObject
    {
        private byte[] game_data;
        private int level_offset;

        // Each level maintains its own undo buffer. This only works if levels
        // never change common data.
        private UndoManager undo_manager;

        /// <summary>
        /// For use by LevelCollection.
        /// </summary>
        internal Level(byte[] game_data, int level_offset, int level_id)
        {
            this.game_data = game_data;
            this.level_offset = level_offset;
            LevelId = level_id;

            undo_manager = new UndoManager(game_data);
            undo_manager.SetUnchanged();
            undo_manager.PropertyChanged += OnUndoManagerPropertyChanged;
        }

        public int Width { get { return 16; } }
        public int Height { get { return 16; } }
        public int LevelId { get; private set; }

        /// <summary>
        /// Gets tile code at the specified coordinates.
        /// </summary>
        public int GetTile(int x, int y)
        {
            ValidateCoordinates(x, y);
            int offset = level_offset + (Height - 1 - y) * Width + x;
            Debug.Assert(offset < game_data.Length, "Level beyond EOF");
            return game_data[offset];
        }

        /// <summary>
        /// Sets tile code at the specified coordinates.
        /// </summary>
        /// <param name="value">must fit in an unsigned byte</param>
        public void SetTile(int x, int y, int value)
        {
            ValidateCoordinates(x, y);
            if (value < 0 || value > 255)
                throw new ArgumentOutOfRangeException("value", "want [0..255], got " + value);
            int offset = level_offset + (Height - 1 - y) * Width + x;
            Debug.Assert(offset < game_data.Length, "Level beyond EOF");
            undo_manager.Manipulator.SetByte("Change Tile", offset, (byte)value);
        }

        private void ValidateCoordinates(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x", "want [0.." + (Width - 1) + "], got " + x);
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y", "want [0.." + (Height - 1) + "], got " + y);
        }

        //
        // Undo
        //

        public bool IsModified { get { return undo_manager.IsModified; } }
        public bool CanUndo { get { return undo_manager.CanUndo; } }
        public bool CanRedo { get { return undo_manager.CanRedo; } }

        public void SetUnchanged()
        {
            undo_manager.SetUnchanged();
        }

        public void Undo()
        {
            undo_manager.Undo();
        }

        public void Redo()
        {
            undo_manager.Redo();
        }

        private void OnUndoManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Pass through.
            RaisePropertyChanged(e.PropertyName);
        }
    }
}
