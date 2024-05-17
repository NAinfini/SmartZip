using Catel.Logging;
using ConsequenceAnalysis.helper;
using ControlzEx.Theming;
using Orc.Controls;
using Orc.Controls.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConsequenceAnalysis.View
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window, INotifyPropertyChanged
    {
        private ILog logger = LogManager.GetCurrentClassLogger();

        private LogFilterGroup m_logFilterGroup;

        public event PropertyChangedEventHandler PropertyChanged;

        public LogFilterGroup LogFilterGroup
        {
            get { return m_logFilterGroup; }
            set
            {
                m_logFilterGroup = value;
                OnPropertyChanged(nameof(LogFilterGroup));
            }
        }

        public LogWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogViewerController.Clear();
            }
            catch (Exception ex)
            {
                logger.Error("Failed to clear Log File due to: " + ex.Message);
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogViewerController.CopyToClipboard();
            }
            catch (Exception ex)
            {
                logger.Error("Failed to copy Log File due to: " + ex.Message);
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemLogs.OpenLogFile();
            }
            catch (Exception ex)
            {
                logger.Error("Failed to open Log File due to: " + ex.Message);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LogViewerController_LogEntryDoubleClick(object sender, LogEntryDoubleClickEventArgs e)
        {
            if (e.LogEntry.ExtraData != null)
            {
                if (e.LogEntry.ExtraData is string)
                {
                    string FileName = e.LogEntry.ExtraData.ToString();
                    if (FileName != null)
                    {
                        string ext = Path.GetExtension(FileName).ToLower();
                        if (ext == ".json" || ext == ".log" || ext == ".txt" || ext == ".computationprofile")
                            try
                            {
                                System.Diagnostics.Process.Start(FileName);
                            }
                            catch (Exception ex)
                            {
                                logger.Error("Failed to open Log File due to: " + ex.Message);
                            }
                    }
                }
            }
        }
    }
}