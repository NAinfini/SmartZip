using Catel.Logging;
using DevExpress.Xpf.Core;
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
        public static readonly string MainFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\SmartZip\\";

        public static readonly string PasswordFileLocation = MainFolder + "PasswordList.json";

        private ILog logger = LogManager.GetCurrentClassLogger();

        public PasswordStorage()
        {
            try
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
                    File.WriteAllText(PasswordFileLocation, "Password");
                }
                string content = File.ReadAllText(PasswordFileLocation);
                content.Split(',').ToList().ForEach(x => pass.Add(new stringItem { Password = x }));
                if (!pass.Any(x => string.IsNullOrEmpty(x.Password)))
                {
                    pass.Add(new stringItem { Password = "" });
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "PasswordStorage Failed To Launch");
                throw;
            }
        }

        private List<stringItem> m_pass = new List<stringItem>();

        public List<stringItem> pass
        {
            get => m_pass;
            set
            {
                m_pass = value;
                OnPropertyChanged(nameof(pass));
            }
        }

        private void OnPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(PasswordFileLocation, string.Join(",", pass.Select(x => x.Password)));
                if (!pass.Any(x => string.IsNullOrEmpty(x.Password)))
                {
                    pass.Add(new stringItem { Password = "" });
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to save password");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class stringItem
    {
        public string Password { get; set; }
    }
}