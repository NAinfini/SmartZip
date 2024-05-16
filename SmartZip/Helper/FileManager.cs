using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartZip.Helper
{
    internal class FileManager
    {
        public static readonly string MainFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SmartZip\\";

        private string PasswordFileLocation { get; set; } = MainFolder + "PasswordList.json";

        private PasswordStorage passwordStorage;

        public FileManager()
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
                passwordStorage = new PasswordStorage();
                passwordStorage.passwords = new List<string>();
                string passwordJson = Newtonsoft.Json.JsonConvert.SerializeObject(passwordStorage);
                File.WriteAllText(PasswordFileLocation, passwordJson);
            }
            passwordStorage = Newtonsoft.Json.JsonConvert.DeserializeObject<PasswordStorage>(File.ReadAllText(PasswordFileLocation));
        }

        public List<string> GetPasswords()
        {
            return passwordStorage.passwords;
        }

        public void AddPassword(string password)
        {
            passwordStorage.passwords.Add(password);
            string passwordJson = Newtonsoft.Json.JsonConvert.SerializeObject(passwordStorage);
            File.WriteAllText(PasswordFileLocation, passwordJson);
        }

        public void RemovePassword(string password)
        {
            passwordStorage.passwords.Remove(password);
            string passwordJson = Newtonsoft.Json.JsonConvert.SerializeObject(passwordStorage);
            File.WriteAllText(PasswordFileLocation, passwordJson);
        }

        public void EditPassword(string oldPassword, string newPassword)
        {
            passwordStorage.passwords.Remove(oldPassword);
            passwordStorage.passwords.Add(newPassword);
            string passwordJson = Newtonsoft.Json.JsonConvert.SerializeObject(passwordStorage);
            File.WriteAllText(PasswordFileLocation, passwordJson);
        }
    }
}