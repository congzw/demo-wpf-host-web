using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public IHost Host { get; set; }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var scope = Host.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILogger<MainWindow>>();
                var hostLifetime = scope.ServiceProvider.GetService<IHostLifetime>();
                var appLifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
                logger.LogInformation("StopApplication");
                appLifetime.StopApplication();
                logger.LogInformation("StopApplication >>>>>");
                //not work!
            }
        }
    }
}
