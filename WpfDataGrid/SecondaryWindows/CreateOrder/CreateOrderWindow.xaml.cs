using System.Windows;
using WpfDataGrid.SecondaryWindows.CreateOrder;

namespace WpfDataGrid.SecondaryWindows
{
    /// <summary>
    /// Логика взаимодействия для CreateOrderWindow.xaml
    /// </summary>
    public partial class CreateOrderWindow : Window
    {
        public CreateOrderWindow(CreateOrderViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            
            DialogResult = true;
        }
    }
}
