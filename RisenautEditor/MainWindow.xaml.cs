using GalaSoft.MvvmLight.Ioc;
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
        }
    }
}
