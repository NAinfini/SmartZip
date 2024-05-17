using SmartZip.Helper;
using SmartZip.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SmartZip
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            PasswordStorage passwordStorage = new PasswordStorage();
            UnzipView unzipView = new UnzipView(passwordStorage);
            unzipView.Show();
        }
    }
}