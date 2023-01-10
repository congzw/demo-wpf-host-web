using System.Windows;
using Demo.Web;

namespace Demo.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            WpfAppControl.Instance.Exit();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //_webManager = WebManager.Instance;
            //await Task.Run(() => _webManager.Start());
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            TheWebHelper.Instance.OpenBrowserIf();
        }
    }
}
