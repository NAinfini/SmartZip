using Catel.Logging;
using ConsequenceAnalysis.helper;
using ConsequenceAnalysis.View;
using SevenZip;
using SmartZip.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmartZip.Views
{
    /// <summary>
    /// Interaction logic for UnzipVIew.xaml
    /// </summary>
    public partial class UnzipView : Window
    {
        private PasswordStorage passwordStorage;
        private ILog logger = LogManager.GetCurrentClassLogger();

        public UnzipView(PasswordStorage p)
        {
            try
            {
                InitializeComponent();
                DataContext = this;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                passwordStorage = p;
                SystemLogs.Initialization();
                if (File.Exists("7z.dll"))
                {
                    SevenZipBase.SetLibraryPath("7z.dll");
                }
                else if (File.Exists("C:\\Program Files\\7-Zip\\7z.dll"))
                {
                    SevenZipBase.SetLibraryPath("C:\\Program Files\\7-Zip\\7z.dll");
                }
                else
                {
                    System.Windows.MessageBox.Show("7-Zip library not found. Please install 7-Zip to use this application.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                stopwatch.Stop();
                logger.Info("UnzipView initialized in " + stopwatch.ElapsedMilliseconds + "ms");
            }
            catch (Exception e)
            {
                logger.Error(e, "UnzipView Failed To Launch");
            }
        }

        private void btnPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PasswordManager manager = new PasswordManager(passwordStorage);
                manager.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to open Password Manager");
            }
        }

        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogWindow logWindow = new LogWindow();
                logWindow.Show();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to open Log Window");
            }
        }

        private void UnZip(string zippedFilePath, string password)
        {
            try
            {
                if (!string.IsNullOrEmpty(password))
                {
                    SevenZipExtractor zipExtractor = new SevenZipExtractor(zippedFilePath, password);
                    // If there are multiple files, create a new folder and extract files there.
                    string directoryName = Path.GetFileNameWithoutExtension(zippedFilePath);
                    string extractPath = Path.Combine(Path.GetDirectoryName(zippedFilePath), directoryName);
                    Directory.CreateDirectory(extractPath);
                    zipExtractor.ExtractArchive(extractPath);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to unzip file");
                throw;
            }
        }
    }
}