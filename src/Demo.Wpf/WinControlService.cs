using Demo.Web.Api;
using System;
using System.Windows;

namespace Demo.Wpf
{
    public class WinControlService : IWinControlService
    {
        public static Window TheMainWindow { get; set; }

        public string SendMessage(string msg)
        {
            var theWindow = TheMainWindow;
            //var theWindow = Application.Current.MainWindow; //thread not safe, not work!

            var result = "";
            theWindow.Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        if (msg == "min")
                        {
                            theWindow.WindowState = WindowState.Minimized;
                        }
                        else if (msg == "max")
                        {
                            theWindow.WindowState = WindowState.Maximized;
                        }
                        else if (msg == "hide")
                        {
                            theWindow.Hide();
                        }
                        else if (msg == "show")
                        {
                            theWindow.Show();
                        }
                        else if (msg == "close")
                        {
                            theWindow.Close();
                        }
                        else
                        {
                            msg = "normal";
                            theWindow.WindowState = WindowState.Normal;
                        }

                        result = $"{msg} => {theWindow.WindowState}";
                    }));

            return $"{result}";
        }
    }
}