using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using WpfDataGrid.SQLiteOperation;

namespace WpfDataGrid.SecondaryWindows
{
    /// <summary>
    /// Логика взаимодействия для EditCustomerWindow.xaml
    /// </summary>
    public partial class EditCustomerWindow : Window
    {

        public EditCustomerWindow()
        {
            InitializeComponent();
            this.DataContext = new EditCustomerViewModel();
        }

    }
}
