using DevExpress.Xpf.Dialogs;
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
        public PasswordStorage ps { get; set; }

        public PasswordManager(PasswordStorage p)
        {
            InitializeComponent();
            DataContext = this;
            ps = p;
        }

        private void passTable_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                stringItem item = CtrlPass.SelectedItem as stringItem;
                var res = System.Windows.MessageBox.Show($"Are you sure you want to delete \'{item.Password}\' ?", "Delete Password", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    ps.pass.Remove(item);
                    ps.Save();
                    CtrlPass.RefreshData();
                }
            }
        }

        private void passTable_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty((string)e.Value))
            {
                ps.Save();
                CtrlPass.RefreshData();
            }
           ;
        }
    }
}