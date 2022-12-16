using System.Threading.Tasks;
using System.Windows;
using Demo.Web;

namespace Demo.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        //private WebManager _webManager;

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //_webManager = WebManager.Instance;
            //await Task.Run(() => _webManager.Start());
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //_webManager.Close();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            TheWebHelper.Instance.OpenBrowserIf();
        }
    }
}
