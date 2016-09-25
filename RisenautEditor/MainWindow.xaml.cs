using GalaSoft.MvvmLight.Ioc;
using RisenautEditor.ViewModel;
using System.Windows;

namespace RisenautEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            SimpleIoc.Default.Register<IFileService>(() => new FileService(this));
            InitializeComponent();
            Closing += ((MainViewModel)DataContext).OnClosing;
        }

        private void MenuItem_ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
