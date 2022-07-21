using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

using WpfDataGrid.PaintPreparationProcess;

namespace WpfDataGrid.SecondaryWindows
{
    /// <summary>
    /// Логика взаимодействия для UseProportionWindow.xaml
    /// </summary>
    public partial class UseProportionWindow : Window
    {
        private PaintRecipe standartRecipe;
        public PaintRecipe newRecipe;
        public UseProportionWindow(PaintRecipe p, bool newOrder)
        {
            InitializeComponent();
            standartRecipe = p;
            float standartTotalWeight = 100f;
            newRecipe = CalculateProportion.Calculate(standartRecipe, standartTotalWeight);
            this.DataContext = newRecipe;
            if (newOrder)
            {
                this.Title = "Создать заказ";
                ok_button.Content = "Создать";
            }
            else
            {
                this.Title = "Использовать пропорцию";
                ok_button.Content = "Ok";
            }    

        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void txtTotalWeight_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox text = sender as TextBox;
            try
            {
                float t = float.Parse(text.Text, CultureInfo.InvariantCulture.NumberFormat);
                newRecipe = CalculateProportion.Calculate(standartRecipe, t);
                this.DataContext = newRecipe;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
