using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Input;
using WpfDataGrid.SQLiteOperation;

namespace WpfDataGrid.SecondaryWindows.CreateOrder
{
    /// <summary>
    /// Логика взаимодействия для AddManufacturerWindow.xaml
    /// </summary>
    public partial class AddManufacturerWindow : Window
    {
        string[]category = new string[] { "автомобили", "мопеды/мотоциклы", "комерческий транспорт",
            "водный транспорт","мебель/архитектура/дизайн","прочее"};
        public Manufacturer NewManufacturer { get; set; }
        public AddManufacturerWindow(Manufacturer manuf)
        {
            InitializeComponent();
            NewManufacturer = manuf;
            this.DataContext = NewManufacturer;
            cmbCategory.ItemsSource = category;
            btnOk.IsEnabled = false;
            NewManufacturer.Category = cmbCategory.SelectedItem.ToString();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void txtManufacturerName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            NewManufacturer.ManufacturerName = txtManufacturerName.Text;
            var context = new ValidationContext(NewManufacturer);
            var results = new List<ValidationResult>();
            btnOk.IsEnabled = Validator.TryValidateObject(NewManufacturer, context, results, true);
        }
    }
}
