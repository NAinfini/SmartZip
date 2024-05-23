using Catel.Logging;
using DevExpress.CodeParser;
using Orc.Controls;
using SmartZip.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ConsequenceAnalysis.helper
{
    public class SystemLogs
    {
        private static ILog logger = LogManager.GetCurrentClassLogger();
        private static readonly string MainFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SmartZip\\";
        private static string FileName;
        public static UnzipView zv;

        /// <summary>
        /// Initialization of the System Logs
        /// </summary>
        public static void Initialization(UnzipView av)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                zv = av;
                FileName = MainFolder + "Logs\\SystemLogs ";
                if (!Directory.Exists(MainFolder + "Logs"))
                {
                    Directory.CreateDirectory(MainFolder + "Logs");
                }
                LogManager.IgnoreCatelLogging = true;
                FileLogListener fileListener = new FileLogListener(FileName, 1000000);
                LogViewerLogListener logViewer = new LogViewerLogListener();
                CustomLastLogListener lastLogListener = new CustomLastLogListener();
                LogManager.AddListener(fileListener);
                LogManager.AddListener(logViewer);
                LogManager.AddListener(lastLogListener);
                stopwatch.Stop();
                logger.Info("System Log Initialization Completed in " + stopwatch.ElapsedMilliseconds + "ms");
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show("System Log Initialization Failed due to: " + e.Message, "Exception", MessageBoxButton.OK);
                throw;
            }
        }

        public static void OpenLogFile()
        {
            try
            {
                Process.Start(FileName + ".log");
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to open Log File due to: ");
                throw;
            }
        }

        public static void Dispose()
        {
            logger.Info("Systemlogs Disposed");
            LogManager.ClearListeners();
        }
    }

    public class CustomLastLogListener : LogListenerBase
    {
        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            SystemLogs.zv.LastLog = message;
        }
    }
}