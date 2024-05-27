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
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

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

        private string[] filenames;
        private Window OpenedLog;

        public UnzipView(string[] args)
        {
            try
            {
                InitializeComponent();
                DataContext = this;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                passwordStorage = new PasswordStorage();
                if (args.Length > 0)
                {
                    filenames = args;
                    logger.Info("File names added:");
                    foreach (var item in filenames)
                    {
                        logger.Info(item);
                    }
                }

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
            }
        }

        private void btnPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PasswordManager passwordManager = new PasswordManager(passwordStorage);
                passwordManager.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to open Password Manager");
            }
        }

        private void btnUnzip_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (filenames == null || filenames.Length <= 0)
                {
                    Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                    openFileDialog.Filter = "All Files|*.*";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        try
                        {
                            UnZip(openFileDialog.FileName);
                        }
                        catch
                        {
                            foreach (var item in passwordStorage.pass)
                            {
                                bool result = false;
                                Thread thread = new Thread(() =>
                                {
                                    result = UnZip(openFileDialog.FileName, item.Password);
                                });
                                thread.Start();
                                Thread.Sleep(50);
                                if (thread.IsAlive || result)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item in filenames)
                    {
                        try
                        {
                            UnZip(item);
                        }
                        catch
                        {
                            foreach (var pass in passwordStorage.pass)
                            {
                                bool result = false;
                                Thread thread = new Thread(() =>
                                {
                                    result = UnZip(item, pass.Password);
                                });
                                thread.Start();
                                Thread.Sleep(50);
                                if (thread.IsAlive || result)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to unzip file");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to close UnzipView");
            }
        }

        private void LogTextBar_ItemClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (OpenedLog != null)
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

        private void UnZip(string zippedFilePath)
        {
            try
            {
                string fileName = Path.GetFileName(zippedFilePath);
                logger.Info($"Attempting to unzip file: {fileName} without password");
                SevenZipExtractor zipExtractor = new SevenZipExtractor(zippedFilePath);
                // If there are multiple files, create a new folder and extract files there.
                string directoryName = Path.GetFileNameWithoutExtension(zippedFilePath);
                string extractPath = Path.Combine(Path.GetDirectoryName(zippedFilePath), directoryName);
                Directory.CreateDirectory(extractPath);
                zipExtractor.Extracting += ZipExtractor_Extracting;
                zipExtractor.ExtractionFinished += (s, e) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        progressBar.Value = 0;
                        progressLbl.Content = "Extraction finished";
                    });
                };
                zipExtractor.ExtractArchive(extractPath);
                logger.Info($"File successfully unzipped");
            }
            catch (Exception e)
            {
                logger.Error($"Failed to unzip file without password");
                throw;
            }
        }

        private bool UnZip(string zippedFilePath, string password)
        {
            try
            {
                if (!string.IsNullOrEmpty(password))
                {
                    string fileName = Path.GetFileName(zippedFilePath);
                    logger.Info($"Attempting to unzip file: {fileName} with password: {password}");
                    SevenZipExtractor zipExtractor = new SevenZipExtractor(zippedFilePath, password);
                    // If there are multiple files, create a new folder and extract files there.
                    string directoryName = Path.GetFileNameWithoutExtension(zippedFilePath);
                    string extractPath = Path.Combine(Path.GetDirectoryName(zippedFilePath), directoryName);
                    Directory.CreateDirectory(extractPath);
                    zipExtractor.Extracting += ZipExtractor_Extracting;
                    zipExtractor.ExtractionFinished += (s, e) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            progressBar.Value = 0;
                            progressLbl.Content = "Extraction finished";
                        });
                    };
                    zipExtractor.ExtractArchive(extractPath);
                    logger.Info($"File unzipped with password {password}");
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                logger.Error($"Failed to unzip file with Password: {password}");
                return false;
            }
        }

        private void ZipExtractor_Extracting(object? sender, ProgressEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = e.PercentDone;
                progressLbl.Content = e.PercentDone + "%";
            });
        }

        private void OnPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }
    }
}