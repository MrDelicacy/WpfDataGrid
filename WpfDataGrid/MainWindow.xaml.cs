using System.ComponentModel;
using System.Windows;
using WpfDataGrid.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;
using WpfDataGrid.PaintPreparationProcess;
using System.Globalization;
using WpfDataGrid.ScaleConnection;
using System;

namespace WpfDataGrid
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AppViewModel();
            this.Closing += MainWindow_Closing;
        }

        private void ScaleInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e != null && e.Key == Key.Right && e.Source is TextBox)
            {
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                request.Wrapped = true;
                ((Control)e.Source).MoveFocus(request);
            }
            if (e != null && e.Key == Key.Left && e.Source is TextBox)
            {
                if (((Control)e.Source).TabIndex != 0)
                {
                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Previous);
                    request.Wrapped = true;
                    ((Control)e.Source).MoveFocus(request);
                }
            }
            if (e != null && e.Key == Key.Space)
            {
                if (e.Source is DataGrid)
                {
                    try
                    {
                        DataGrid source = e.Source as DataGrid;
                        source.BeginInit();
                        UsedInCalculationPaintComponent component = source.CurrentItem as UsedInCalculationPaintComponent;
                        component.AddAmount = float.Parse(Readout.ReadoutAmount, CultureInfo.InvariantCulture.NumberFormat);
                        source.EndInit();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                if (e.Source is TextBox)
                {
                    TextBox textBox = e.Source as TextBox;
                    textBox.Text = Readout.ReadoutAmount;

                    TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                    request.Wrapped = true;
                    ((Control)e.Source).MoveFocus(request);

                }
            }
            if (e != null && e.Key == Key.Space)
            {
                if (e.Source is DataGrid)
                {
                    try
                    {
                        DataGrid source = e.Source as DataGrid;
                        source.BeginInit();
                        UsedInCalculationPaintComponent component = source.CurrentItem as UsedInCalculationPaintComponent;
                        component.AddAmount = float.Parse(Readout.ReadoutAmount, CultureInfo.InvariantCulture.NumberFormat);
                        source.EndInit();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            //if (e != null && e.Key == Key.Delete)
            //{
            //    if (e.Source is DataGrid)
            //    {
            //        try
            //        {
            //            DataGrid source = e.Source as DataGrid;
                        
            //            source.BeginInit();
            //            //UsedInCalculationPaintComponent component = source.CurrentItem as UsedInCalculationPaintComponent;
            //            var result = source.ItemsSource;
                        
            //            source.EndInit();
            //        }
            //        catch (Exception ex)
            //        {
            //            MessageBox.Show(ex.Message);
            //        }
            //    }
            //}

        }


        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти?", "Внимание!", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
                e.Cancel = true;
        }

    }

}
