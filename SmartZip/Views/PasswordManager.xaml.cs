using SmartZip.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartZip.Views
{
    /// <summary>
    /// Interaction logic for PasswordManager.xaml
    /// </summary>
    public partial class PasswordManager : Window
    {
        public ZipManager zipManager { get; set; }

        public PasswordStorage passwordStorage { get; set; }

        public PasswordManager(ZipManager m)
        {
            InitializeComponent();
            DataContext = this;
            zipManager = m;
            passwordStorage = zipManager.passwordStorage;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnpropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TableView_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "Password")
            {
                passwordStorage.UpdateFile();
            }
            CtrlPass.RefreshData();
        }

        private void passTable_AddingNewRow(object sender, AddingNewEventArgs e)
        {
            passwordStorage.AddEmptyPair();
        }

        private void passTable_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                passwordStorage.RemovePair((NamePassPair)CtrlPass.SelectedItem);
                CtrlPass.RefreshData();
            }
        }
    }
}