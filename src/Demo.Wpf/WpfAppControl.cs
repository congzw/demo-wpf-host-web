using Microsoft.Extensions.Logging;
using System.Windows;

namespace Demo.Wpf
{
    public class WpfAppControl
    {
        public static WpfAppControl Instance = new WpfAppControl();

        public Window TheMainWindow { get; set; }

        public ILogger Logger { get; set; }

        public void Start()
        {
            if (TheMainWindow == null)
            {
                TheMainWindow = new FakeMainWindow();
                //not show!
            }
            Log("start the main window: " + TheMainWindow.GetType().Name);
        }

        public void Exit()
        {
            Log("Application.Current.Shutdown: " + TheMainWindow.GetType().Name);
            Application.Current.Shutdown();
        }

        public void Log(string msg)
        {
            Logger?.LogInformation(msg);
        }

        class FakeMainWindow : Window
        {
            public FakeMainWindow()
            {
                Width = 1;
                Height = 1;
                //this.Visibility = Visibility.Collapsed;                
            }
        }
    }
}