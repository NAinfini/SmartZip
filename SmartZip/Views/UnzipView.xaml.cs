using Catel.Logging;
using ConsequenceAnalysis.helper;
using ConsequenceAnalysis.View;
using SevenZip;
using SmartZip.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class UnzipView : Window, INotifyPropertyChanged
    {
        private PasswordStorage passwordStorage;
        private ILog logger = LogManager.GetCurrentClassLogger();

        public event PropertyChangedEventHandler? PropertyChanged;

        private string m_LastLog = "";

        public string LastLog
        {
            get => m_LastLog;
            set
            {
                m_LastLog = value;
                OnPropertyChanged(nameof(LastLog));
            }
        }
        private Window OpenedLog;
        public UnzipView(PasswordStorage p)
        {
            try
            {
                InitializeComponent();
                DataContext = this;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                passwordStorage = p;
                Closing += (s, e) => windowClosed();
                SystemLogs.Initialization(this);
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

        private void windowClosed()
        {
            try
            {
                OpenedLog?.Close();
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to save password storage");
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
        private void LogTextBar_ItemClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if(OpenedLog != null)
                {
                    OpenedLog.Focus();
                    return;
                }
                LogWindow logWindow = new LogWindow();
                logWindow.Show();
                OpenedLog = logWindow;
                logWindow.Closed += (s, e) => OpenedLog = null;
                logger.Info("Log Window Opened");
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
                logger.Info($"Unzipping file: {zippedFilePath} with {password}");
                if (!string.IsNullOrEmpty(password))
                {
                    SevenZipExtractor zipExtractor = new SevenZipExtractor(zippedFilePath, password);
                    // If there are multiple files, create a new folder and extract files there.
                    string directoryName = Path.GetFileNameWithoutExtension(zippedFilePath);
                    string extractPath = Path.Combine(Path.GetDirectoryName(zippedFilePath), directoryName);
                    Directory.CreateDirectory(extractPath);
                    zipExtractor.ExtractArchive(extractPath);
                }
                logger.Info("Unzipped file successfully");
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to unzip file");
                throw;
            }
        }

        private void OnPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        
    }
}