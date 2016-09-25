using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace RisenautEditor
{
    class FileService : IFileService
    {
        private Window owner;

        public FileService(Window owner)
        {
            this.owner = owner;
        }

        public GameFile OpenGame()
        {
            var dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.DefaultExt = "fil";
            dlg.DereferenceLinks = true;
            dlg.Filter = "Agat files (*.fil)|*.fil|All files|*.*";
            dlg.Multiselect = false;

            bool? result = dlg.ShowDialog(owner);
            if (!result.HasValue || !(bool)result)
            {
                return null;
            }

            try
            {
                return new GameFile(dlg.FileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(owner, e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public bool SaveGame(GameFile game)
        {
            try
            {
                game.Save(game.FileName);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(owner, e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool SaveGameAs(GameFile game)
        {
            var dlg = new SaveFileDialog();
            dlg.DefaultExt = "fil";
            dlg.DereferenceLinks = true;
            dlg.Filter = "Agat files (*.fil)|*.fil|All files|*.*";
            dlg.FileName = game.FileName;

            bool? result = dlg.ShowDialog(owner);
            if (!result.HasValue || !(bool)result)
            {
                return false;
            }

            try
            {
                game.Save(dlg.FileName);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(owner, e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool QueryGameModified(GameFile game)
        {
            string message = "Do you want to save changes to " + Path.GetFileName(game.FileName) + "?";
            MessageBoxResult result = MessageBox.Show(
                message, "Game Modified", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    return SaveGame(game);
                case MessageBoxResult.No:
                    return true;
            }
            return false;
        }
    }
}
