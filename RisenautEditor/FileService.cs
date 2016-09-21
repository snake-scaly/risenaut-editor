using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
            if (result == null || !result.Value)
            {
                return null;
            }

            try
            {
                return new GameFile(dlg.FileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(owner, e.Message, "Error");
                return null;
            }
        }
    }
}
