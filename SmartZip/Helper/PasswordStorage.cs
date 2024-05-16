using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartZip.Helper
{
    public class PasswordStorage : INotifyPropertyChanged
    {
        private List<NamePassPair> m_passwords { get; set; }

        public List<NamePassPair> passwords
        {
            get => m_passwords;
            set
            {
                m_passwords = value;
                if (!m_passwords.Exists(x => x.Password == ""))
                {
                    AddEmptyPair();
                }
                OnPropertyChanged(nameof(passwords));
            }
        }

        public PasswordStorage()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<string> GetPasswords()
        {
            return passwords.Select(x => x.Password).ToList();
        }

        public void UpdateFile()
        {
            if (!m_passwords.Exists(x => x.Password == ""))
            {
                AddEmptyPair();
            }
            string passwordJson = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            File.WriteAllText(ZipManager.PasswordFileLocation, passwordJson);
        }

        public void RemovePair(NamePassPair pair)
        {
            passwords.Remove(pair);
            UpdateFile();
        }

        public void AddEmptyPair()
        {
            passwords.Add(new NamePassPair(0, ""));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}