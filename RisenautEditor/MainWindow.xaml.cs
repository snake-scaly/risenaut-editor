using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace RisenautEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameFile game;

        public MainWindow()
        {
            InitializeComponent();
            game = new GameFile("E:\\home\\projects\\risenaut-editor\\.etc\\KLAD.FIL");

            Blocks = game.Blocks;
            foreach (var l in game.Levels)
            {
                level_list.Items.Add(l);
            }

            level_view.Blocks = game.Blocks;

            level_list.SelectedIndex = 0;
        }

        public IReadOnlyList<Sprite> Blocks { get; set; }

        private void level_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            level_view.Level = (Level)e.AddedItems[0];
        }
    }
}
