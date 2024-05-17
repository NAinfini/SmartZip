using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SevenZip;

namespace SmartZip.Helper
{
    public class ZipManager
    {
        public static readonly string MainFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SmartZip\\";

        public static readonly string PasswordFileLocation = MainFolder + "PasswordList.json";

        public PasswordStorage passwordStorage;

        public ZipManager()
        {
            if (!Directory.Exists(MainFolder))
            {
                Directory.CreateDirectory(MainFolder);
            }

            if (!File.Exists(PasswordFileLocation))
            {
                File.Create(PasswordFileLocation).Close();
            }

            if (string.IsNullOrEmpty(File.ReadAllText(PasswordFileLocation)))
            {
                File.WriteAllText(PasswordFileLocation, "{\"passwords\":[]}");
            }
            passwordStorage = Newtonsoft.Json.JsonConvert.DeserializeObject<PasswordStorage>(File.ReadAllText(PasswordFileLocation));

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
        }

        public bool UnZipFile(string file)
        {
            foreach (string pass in passwordStorage.GetPasswords())
            {
                try
                {
                    UnZip(file, pass);
                    return true;
                }
                catch (ExtractionFailedException)
                {
                    continue;
                }
            }
            return false;
        }

        public bool UnZipFiles(string[] files)
        {
            foreach (string file in files)
            {
                foreach (string pass in passwordStorage.GetPasswords())
                {
                    try
                    {
                        UnZip(file, pass);
                        return true;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return false;
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
                throw;
            }
        }

        private int GetTopLevelFilesCount(SevenZipExtractor zipExtractor)
        {
            try
            {
                int count = 0;

                foreach (var entry in zipExtractor.ArchiveFileData)
                {
                    if (string.IsNullOrEmpty(Path.GetDirectoryName(entry.FileName)))
                    {
                        count++;
                    }
                }

                return count;
            }
            catch (Exception)
            {
                return 2;
            }
        }
    }
}