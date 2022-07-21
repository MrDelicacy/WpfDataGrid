using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfDataGrid.SQLiteOperation;

namespace WpfDataGrid.SecondaryWindows
{
    /// <summary>
    /// Логика взаимодействия для CreateOrderWindow.xaml
    /// </summary>
    public partial class CompleteOrderWindow : Window
    {
        public CompletedWork CompletedWork { get; set; }
        public class PointCoordinate
        {
            public float x;
            public float y;
            public PointCoordinate(float x_, float y_)
            {
                x = x_;
                y = y_;
            }

        }
        public CompleteOrderWindow(CompletedWork cWork)
        {
            
            InitializeComponent();
            SliderDigit.Content = "0";
            CompletedWork = cWork;
            //servInf = serviceInfo;
            //rating = Convert.ToInt32(SliderDigit.Content);
        }
        public void OnPaint(PointCoordinate[] p, int n)
        {

            double R = 5, r = 10;   // радиусы
            double alpha = 60;       // поворот
            double x0 = 20, y0 = 20; // центр


            double a = alpha, da = Math.PI / n, l;
            for (int k = 0; k < 2 * n + 1; k++)
            {
                l = k % 2 == 0 ? r : R;
                p[k] = new PointCoordinate((float)(x0 + l * Math.Cos(a)), (float)(y0 + l * Math.Sin(a)));
                a += da;
            }

        }

        private void AddStar()
        {
            Polygon p = new Polygon();
            int n = 5;  // число вершин
            PointCoordinate[] points = new PointCoordinate[2 * n + 1];
            OnPaint(points, n);
            PointCollection pc = new PointCollection();
            for (int i = 0; i < 11; i++)
            {
                Point point1 = new Point(points[i].x, points[i].y);
                pc.Add(point1);
            }
            p.Points = pc;
            p.Stroke = Brushes.Gray;
            p.Fill = Brushes.AliceBlue;
            MyPanel.Children.Add(p);
        }
        private void DeleteStar()
        {
            MyPanel.Children.RemoveAt(MyPanel.Children.Count - 1);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            int sliderDigit = Convert.ToInt32(Math.Round(slider.Value, 0));

            if (sliderDigit - Convert.ToInt32(SliderDigit.Content) > 0)
            {
                for (int i = 0; i < sliderDigit - Convert.ToInt32(SliderDigit.Content); i++)
                    AddStar();
            }
            if (sliderDigit - Convert.ToInt32(SliderDigit.Content) < 0)
            {
                for (int i = 0; i < Convert.ToInt32(SliderDigit.Content) - sliderDigit; i++)
                    DeleteStar();
            }
            SliderDigit.Content = Math.Round(slider.Value, 0).ToString();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox ch = new CheckBox();
            ch = sender as CheckBox;
            if (ch.Content.ToString() == "колеровка по спектрофотометру")
                CompletedWork.SpectroTinting = true;
            if (ch.Content.ToString() == "автомобиль на улице")
                CompletedWork.CarTinting = true;
            if (ch.Content.ToString() == "доколеровка")
                CompletedWork.Tinting = true;
            if (ch.Content.ToString() == "слив по спектрофотометру")
                CompletedWork.SpectroPreparation= true;
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox ch = new CheckBox();
            ch = sender as CheckBox;
            if (ch.Content.ToString() == "колеровка по спектрофотометру")
                CompletedWork.SpectroTinting = false;
            if (ch.Content.ToString() == "автомобиль на улице")
                CompletedWork.CarTinting = false;
            if (ch.Content.ToString() == "доколеровка")
                CompletedWork.Tinting = false;
            if (ch.Content.ToString() == "слив по спектрофотометру")
                CompletedWork.SpectroPreparation = false;
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = new RadioButton();
            rb = sender as RadioButton;
            if (rb.Content.ToString() == "колеровка")
                CompletedWork.TypeWork = "колеровка";
            if (rb.Content.ToString() == "приготовление по коду")
                CompletedWork.TypeWork = "приготовление по коду";
            if (rb.Content.ToString() == "транслак")
                CompletedWork.TypeWork = "транслак";
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            CompletedWork.RecipeRaiting = Convert.ToInt32(SliderDigit.Content);
            var context = new ValidationContext(CompletedWork);
            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (Validator.TryValidateObject(CompletedWork, context, results, true))
                this.DialogResult = true;
            else MessageBox.Show("Укажите вид услуги.");
        }
    }
}
