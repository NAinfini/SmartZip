using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SevenZip;

namespace SmartZip.Helper
{
    public class ZipManager
    {
        private FileManager fileManager = new FileManager();

        public ZipManager()
        {
            SevenZipBase.SetLibraryPath("C:\\Program Files\\7-Zip\\7z.dll");
            fileManager.GetPasswords();
        }

        public bool UnZipFile(string file)
        {
            foreach (string password in fileManager.GetPasswords())
            {
                try
                {
                    UnZip(file, password);
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
                foreach (string password in fileManager.GetPasswords())
                {
                    try
                    {
                        UnZip(file, password);
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
                zipExtractor.Extracting += ZipExtractor_Extracting;
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

        private static void ZipExtractor_Extracting(object sender, ProgressEventArgs e)
        {
        }
    }
}