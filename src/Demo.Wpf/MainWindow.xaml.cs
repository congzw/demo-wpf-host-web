using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Demo.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            this.Closed += MainWindow_Closed;
        }

        //private WebManager _webManager;

        public IHost Host { get; set; }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //_webManager = WebManager.Instance;
            //await Task.Run(() => _webManager.Start());
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Host == null)
            {
                return;
            }

            using (Host)
            {
                using (var scope = Host.Services.CreateScope())
                {
                    var lifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
                    lifetime.StopApplication();
                }
                //await Host.StopAsync();
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            TheWebHelper.Instance.OpenBrowserIf();
        }
    }
}
