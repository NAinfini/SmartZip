using System.Configuration;
using System.Data;
using System.Windows;

namespace SmartZip
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            foreach (string arg in e.Args)
            {
                Helper.ZipManager zipManager = new Helper.ZipManager();
                zipManager.UnZipFiles(new string[] { arg });
            }
            Application.Current.Shutdown();
        }
    }
}