using GraphicalBars.src.model;
using GraphicalBars.src.utils;
using GraphicalBars.src.utils.functionsUtils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GraphicalBars.src.view
{

    public partial class AddPointWindow : Window
    {
        private RepresentationParams repParams;
        private AbstractFunction function;

        #region Constructors
        public AddPointWindow(RepresentationParams repParams)
        {
            InitializeComponent();
            this.repParams = repParams;
            CreatedPoints = new List<MyPoint>();
        }
        #endregion


        #region Properties
        public List<MyPoint> CreatedPoints { get; }
        #endregion


        #region Random generation
        /* Random x,y generation when clicking generating button */
        private void ButtonGenerar_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            double x = Math.Round(r.NextDouble() * (repParams.XMax - repParams.XMin) + repParams.XMin, 5);
            double y = Math.Round(r.NextDouble() * (repParams.YMax - repParams.YMin) + repParams.YMin, 5);
            Color c = Color.FromArgb(255, (byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256));

            CreatedPoints.Clear();
            CreatedPoints.Add(new MyPoint(x, y, c));

            TextBlock_Result.Text = "( " + x + " , " + y + " )";
            TextBlock_Color.Background = new SolidColorBrush(c);
        }
        #endregion


        #region Auto generation
        /**
         * Calculates the points and add them to the list that is returned by the dialog
         */
        private void CreateAutoPoints()
        {
            string s = (string)(ComboBoxFunctions.SelectedItem as ComboBoxItem).Content;

            if (s != null)
            {
                function.Values = new double[6]
                {
                    TextBox_A.DoubleValue,TextBox_B.DoubleValue,TextBox_C.DoubleValue,TextBox_D.DoubleValue,TextBox_E.DoubleValue,TextBox_F.DoubleValue
                };
                CreatedPoints.Clear();

                Color c = ColorPicker_Auto.SelectedColor.Value;

                for (double x = TextBox_RangeXMin.DoubleValue; x <= TextBox_RangeXMax.DoubleValue; x += ConfigConstants.GapBetweenAutoPoints)
                {
                    double y = function.calculate(x);
                    CreatedPoints.Add(new MyPoint(x, y, c));
                }
            }
        }
        #endregion


        //Manual generation is included in "add button click"
        #region Cancel/Add buttons
        /**
         * Closes dialog window and returns DialogResult false
         */
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /**
         * Closes dialog window and returns DialogResult true and the list with the new items (in case they are created)
         */
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (TabManualGeneration.IsSelected)
            {
                CreatedPoints.Clear();
                CreatedPoints.Add(new MyPoint(TextBox_X.DoubleValue, TextBox_Y.DoubleValue, ColorPicker_Manual.SelectedColor.Value));

            }else if (TabAutoGeneration.IsSelected)
            {
                CreateAutoPoints();
            }
            this.DialogResult = true;
        }
        #endregion


        /**
         * Function combo box changed
         */
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBoxFunctions.SelectedIndex)
            {
                case 0:
                    if (TextBox_A != null && TextBox_B != null && TextBox_C != null && TextBox_D != null && TextBox_E != null && TextBox_F != null)
                    { 
                        TextBox_A.IsEnabled = true; TextBox_B.IsEnabled = true; TextBox_C.IsEnabled = true;
                        TextBox_D.IsEnabled = true; TextBox_E.IsEnabled = true; TextBox_F.IsEnabled = true;
                    }
                    this.function = new PolynomialFunction();
                    break;

                case 1:
                    if (TextBox_A != null && TextBox_B != null && TextBox_C != null && TextBox_D != null && TextBox_E != null && TextBox_F != null)
                    {
                        TextBox_A.IsEnabled = true; TextBox_B.IsEnabled = true; TextBox_C.IsEnabled = false;
                        TextBox_D.IsEnabled = false; TextBox_E.IsEnabled = false; TextBox_F.IsEnabled = false;
                    }
                    this.function = new CosFunction();
                    break;

                case 2:
                    if (TextBox_A != null && TextBox_B != null && TextBox_C != null && TextBox_D != null && TextBox_E != null && TextBox_F != null)
                    {
                        TextBox_A.IsEnabled = true; TextBox_B.IsEnabled = true; TextBox_C.IsEnabled = false;
                        TextBox_D.IsEnabled = false; TextBox_E.IsEnabled = false; TextBox_F.IsEnabled = false;
                    }
                    this.function = new SinFunction();
                    break;

                case 3:
                    if (TextBox_A != null && TextBox_B != null && TextBox_C != null && TextBox_D != null && TextBox_E != null && TextBox_F != null)
                    {
                        TextBox_A.IsEnabled = false; TextBox_B.IsEnabled = false; TextBox_C.IsEnabled = false;
                        TextBox_D.IsEnabled = false; TextBox_E.IsEnabled = false; TextBox_F.IsEnabled = false;
                    }
                    this.function = new AbsFunction();
                    break;
            }   
        }

    }
}
