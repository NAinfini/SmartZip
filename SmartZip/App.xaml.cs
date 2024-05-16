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
        private ZipManager zipManager;

        private void App_Startup(object sender, StartupEventArgs e)
        {
            zipManager = new ZipManager();
            if (e.Args.Length > 0)
            {
                foreach (string arg in e.Args)
                {
                    if (!zipManager.UnZipFile(arg))
                    {
                        PasswordManager manager = new PasswordManager(zipManager);
                        manager.ShowDialog();
                    }
                }
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                PasswordManager manager = new PasswordManager(zipManager);
                manager.Show();
            }
        }
    }
}