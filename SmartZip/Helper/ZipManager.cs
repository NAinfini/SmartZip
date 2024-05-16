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
            if (File.Exists("C:\\Program Files\\7-Zip\\7z.dll"))
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
                    catch (ExtractionFailedException)
                    {
                        continue;
                    }
                }
            }
            return false;
        }

        private void UnZip(string zippedFilePath, string password)
        {
            SevenZipExtractor zipExtractor = null;

            if (!string.IsNullOrEmpty(password))
            {
                zipExtractor = new SevenZipExtractor(zippedFilePath, password);
                int topLevelFilesCount = GetTopLevelFilesCount(zipExtractor);
                if (topLevelFilesCount == 1)
                {
                    // If there's only one file, extract it to the folder of the zipped file.
                    string directoryPath = Path.GetDirectoryName(zippedFilePath);
                    zipExtractor.ExtractArchive(directoryPath);
                }
                else if (topLevelFilesCount > 1)
                {
                    // If there are multiple files, create a new folder and extract files there.
                    string directoryName = Path.GetFileNameWithoutExtension(zippedFilePath);
                    string extractPath = Path.Combine(Path.GetDirectoryName(zippedFilePath), directoryName);
                    Directory.CreateDirectory(extractPath);
                    zipExtractor.ExtractArchive(extractPath);
                }
            }
        }

        private int GetTopLevelFilesCount(SevenZipExtractor zipExtractor)
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
    }
}