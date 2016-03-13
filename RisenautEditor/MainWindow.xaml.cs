using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace RisenautEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string title_base;
        private GameFile game;

        public MainWindow()
        {
            InitializeComponent();
            title_base = Title;
        }

        public IReadOnlyList<Sprite> Blocks { get; set; }

        private void level_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            level_view.Level = (Level)e.AddedItems[0];
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.DefaultExt = "fil";
            dlg.DereferenceLinks = true;
            dlg.Filter = "Agat files (*.fil)|*.fil|All files|*.*";
            dlg.Multiselect = false;

            bool? result = dlg.ShowDialog(this);
            if (result != null && result.Value)
            {
                OpenGame(dlg.FileName);
            }
        }

        private void OpenGame(string file_name)
        {
            try
            {
                game = new GameFile(file_name);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
                return;
            }

            Title = Path.GetFileName(file_name) + " - " + title_base;

            Blocks = game.Blocks;
            foreach (var l in game.Levels)
            {
                level_list.Items.Add(l);
            }

            level_view.Blocks = game.Blocks;

            level_list.SelectedIndex = 0;
        }
    }
}
