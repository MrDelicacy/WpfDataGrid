using System;
using System.Windows.Media;
using System.Windows;
using WpfDataGrid.SQLiteOperation;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WpfDataGrid.SecondaryWindows.CreateOrder
{
    /// <summary>
    /// Логика взаимодействия для AddCustomerWindow.xaml
    /// </summary>
    public partial class AddCustomerWindow : Window
    {
        public Customer NewCustomer { get; set; }
        public AddCustomerWindow(Customer c)
        {
            InitializeComponent();
            NewCustomer = c;
            this.DataContext = NewCustomer;
            btnOk.IsEnabled = false;
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void txt_LostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            var context = new ValidationContext(NewCustomer);
            var results = new List<ValidationResult>();
            btnOk.IsEnabled = Validator.TryValidateObject(NewCustomer, context, results, true);

        }

    }
}
