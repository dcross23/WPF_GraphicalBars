using GraphicalBars.src.model;
using GraphicalBars.src.utils;
using System;
using System.Windows;
using System.Windows.Media;

namespace GraphicalBars.src.view
{
    /* Event args for configurations change events */
    public class BarsConfigArgs : EventArgs
    {
        public BarsConfig NewBarsConfig { get; set; }
    }

    public class PolylineConfigArgs : EventArgs
    {
        public PolylineConfig NewPolylineConfig { get; set; }
    }

    public class PointsConfigArgs : EventArgs
    {
        public PointsConfig NewPointsConfig { get; set; }
    }

    public class CanvasConfigArgs : EventArgs
    {
        public CanvasConfig NewCanvasConfig { get; set; }
    }


    public partial class ConfigWindow : Window
    {
        private BarsConfig actualBarsConfig;
        private PolylineConfig actualPolylineConfig;
        private PointsConfig actualPointsConfig;
        private CanvasConfig actualCanvasConfig;

        public EventHandler<BarsConfigArgs> BarsConfigChangedEvent;
        public EventHandler<PolylineConfigArgs> PolylineConfigChangedEvent;
        public EventHandler<PointsConfigArgs> PointsConfigChangedEvent;
        public EventHandler<CanvasConfigArgs> CanvasConfigChangedEvent;

        public int SortByXorY { get; set; }

        public ConfigWindow(BarsConfig actualBarsConfig, PolylineConfig actualPolylineConfig, PointsConfig actualPointsConfig, CanvasConfig actualCanvasConfig, int SortByXorY)
        {
            InitializeComponent();
            this.SortByXorY = SortByXorY;
            if (SortByXorY == 0) RadioButton_SortX.IsChecked = true;
            else if (SortByXorY == 1) RadioButton_SortY.IsChecked = true;

            this.actualBarsConfig = actualBarsConfig;
            this.actualPolylineConfig = actualPolylineConfig;
            this.actualPointsConfig = actualPointsConfig;
            this.actualCanvasConfig = actualCanvasConfig;

            ColorPicker_Borders.SelectedColor = ((SolidColorBrush) Application.Current.FindResource("MenuBordersBrush")).Color ;
            ColorPicker_Background.SelectedColor = ((SolidColorBrush)Application.Current.FindResource("MenuBackgroudBrush")).Color;
            
            ColorPicker_Bars.SelectedColor = ((SolidColorBrush)Application.Current.FindResource("BarsBrush")).Color;
            ColorPicker_Polyline.SelectedColor = ((SolidColorBrush)Application.Current.FindResource("PolylineBrush")).Color;
            ColorPicker_Points.SelectedColor = ((SolidColorBrush)Application.Current.FindResource("PointsBrush")).Color;

            //Bars config set up
            CheckBox_UseColorPointForBar.IsChecked = actualBarsConfig.UsePointColor;
            if (actualBarsConfig.UsePointColor) 
                ColorPicker_Bars.IsEnabled = false;

            CheckBox_DashedBar.IsChecked = actualBarsConfig.DashedBars;
            CheckBox_HighlightBar.IsChecked = actualBarsConfig.HighlightWhenSelectPoint;
            DoubleUpDown_BarWidth.Value = actualBarsConfig.BarsWidth;

            //Polyline config set up
            DoubleUpDown_PolylineWidth.Value = actualPolylineConfig.PolylineWidth;
            CheckBox_DashedPolyline.IsChecked = actualPolylineConfig.DashedPolyline;

            //Points config set up
            CheckBox_UseColorPointForPoint.IsChecked = actualPointsConfig.UsePointColor;
            if (actualPointsConfig.UsePointColor)
                ColorPicker_Points.IsEnabled = false;

            CheckBox_HighlightPoint.IsChecked = actualPointsConfig.HighlightWhenSelectPoint;
            DoubleUpDown_PointSize.Value = actualPointsConfig.PointsSize;

            //Canvas config set up
            DoubleUpDown_MinZoom.Value = actualCanvasConfig.MinZoom;
            DoubleUpDown_MaxZoom.Value = actualCanvasConfig.MaxZoom;
            NumericTextBox_xMin.Text = actualCanvasConfig.RepParams.XMin.ToString();
            NumericTextBox_xMax.Text = actualCanvasConfig.RepParams.XMax.ToString();
            NumericTextBox_yMin.Text = actualCanvasConfig.RepParams.YMin.ToString();
            NumericTextBox_yMax.Text = actualCanvasConfig.RepParams.YMax.ToString();
        }

        #region Menu options
        private void ColorPicker_Borders_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Application.Current.Resources["MenuBordersBrush"] = new SolidColorBrush(ColorPicker_Borders.SelectedColor.Value);
        }


        private void ColorPicker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Application.Current.Resources["MenuBackgroudBrush"] = new SolidColorBrush(ColorPicker_Background.SelectedColor.Value);
        }

        private void RadioButton_Sort_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == RadioButton_SortX)
                this.SortByXorY = 0;
            else if (sender == RadioButton_SortY)
                this.SortByXorY = 1;
        }
        #endregion

        #region Bars options
        private void ColorPicker_Bars_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Application.Current.Resources["BarsBrush"] = new SolidColorBrush(ColorPicker_Bars.SelectedColor.Value);
        }

        private void CheckBox_UseColorPointForBar_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckBox_UseColorPointForBar.IsChecked)
            {
                this.actualBarsConfig.UsePointColor = true;
                ColorPicker_Bars.IsEnabled = false;            
            }
            else
            {
                this.actualBarsConfig.UsePointColor = false;
                ColorPicker_Bars.IsEnabled = true;
            }
            BarsConfigArgs bca = new BarsConfigArgs();
            bca.NewBarsConfig = this.actualBarsConfig;
            OnBarsConfigChangedEvent(bca);
        }


        private void CheckBox_DashedBar_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckBox_DashedBar.IsChecked)
                this.actualBarsConfig.DashedBars = true;
            else
                this.actualBarsConfig.DashedBars = false;
            BarsConfigArgs bca = new BarsConfigArgs();
            bca.NewBarsConfig = this.actualBarsConfig;
            OnBarsConfigChangedEvent(bca);
        }

        private void CheckBox_HighlightBar_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckBox_HighlightBar.IsChecked)
                this.actualBarsConfig.HighlightWhenSelectPoint = true;
            else
                this.actualBarsConfig.HighlightWhenSelectPoint = false;
            BarsConfigArgs bca = new BarsConfigArgs();
            bca.NewBarsConfig = this.actualBarsConfig;
            OnBarsConfigChangedEvent(bca);
        }

        private void DoubleUpDown_BarWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DoubleUpDown_BarWidth.Value != null)
            {
                this.actualBarsConfig.BarsWidth = (double)DoubleUpDown_BarWidth.Value;
                BarsConfigArgs bca = new BarsConfigArgs();
                bca.NewBarsConfig = this.actualBarsConfig;
                OnBarsConfigChangedEvent(bca);
            }
        }
        #endregion

        #region Polyline options
        /**
         * Polyline color selector
         */
        private void ColorPicker_Polyline_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Application.Current.Resources["PolylineBrush"] = new SolidColorBrush(ColorPicker_Polyline.SelectedColor.Value);
        }


        private void DoubleUpDown_PolylineWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DoubleUpDown_PolylineWidth.Value != null)
            {
                this.actualPolylineConfig.PolylineWidth = (double)DoubleUpDown_PolylineWidth.Value;
                PolylineConfigArgs pca = new PolylineConfigArgs();
                pca.NewPolylineConfig = this.actualPolylineConfig;
                OnPolylineConfigChangedEvent(pca);
            }
        }

        private void CheckBox_DashedPolyline_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckBox_DashedPolyline.IsChecked)
                this.actualPolylineConfig.DashedPolyline = true;
            else
                this.actualPolylineConfig.DashedPolyline = false;
            PolylineConfigArgs pca = new PolylineConfigArgs();
            pca.NewPolylineConfig = this.actualPolylineConfig;
            OnPolylineConfigChangedEvent(pca);
        }

        #endregion

        #region Points options
        private void ColorPicker_Points_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Application.Current.Resources["PointsBrush"] = new SolidColorBrush(ColorPicker_Points.SelectedColor.Value);
        }

        private void CheckBox_UseColorPointForPoint_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckBox_UseColorPointForPoint.IsChecked)
            {
                this.actualPointsConfig.UsePointColor = true;
                ColorPicker_Points.IsEnabled = false;
            }
            else
            {
                this.actualPointsConfig.UsePointColor = false;
                ColorPicker_Points.IsEnabled = true;
            }
                
            PointsConfigArgs pca = new PointsConfigArgs();
            pca.NewPointsConfig = this.actualPointsConfig;
            OnPointsConfigChangedEvent(pca);
        }

        private void CheckBox_HighlightPoint_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if ((bool)CheckBox_HighlightPoint.IsChecked)
                this.actualPointsConfig.HighlightWhenSelectPoint = true;
            else
                this.actualPointsConfig.HighlightWhenSelectPoint = false;

            PointsConfigArgs pca = new PointsConfigArgs();
            pca.NewPointsConfig = this.actualPointsConfig;
            OnPointsConfigChangedEvent(pca);
        }

        private void DoubleUpDown_PointSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DoubleUpDown_PointSize.Value != null)
            {
                this.actualPointsConfig.PointsSize = (double)DoubleUpDown_PointSize.Value;
                PointsConfigArgs pca = new PointsConfigArgs();
                pca.NewPointsConfig = this.actualPointsConfig;
                OnPointsConfigChangedEvent(pca);
            }
        }
        #endregion

        #region Canvas options
        private void DoubleUpDown_MinZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DoubleUpDown_MinZoom.Value != null)
            {
                this.actualCanvasConfig.MinZoom = (double)DoubleUpDown_MinZoom.Value;
                CanvasConfigArgs cca = new CanvasConfigArgs();
                cca.NewCanvasConfig = this.actualCanvasConfig;
                OnCanvasConfigChangedEvent(cca);
            }
        }

        private void DoubleUpDown_MaxZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DoubleUpDown_MaxZoom.Value != null)
            {
                this.actualCanvasConfig.MaxZoom = (double)DoubleUpDown_MaxZoom.Value;
                CanvasConfigArgs cca = new CanvasConfigArgs();
                cca.NewCanvasConfig = this.actualCanvasConfig;
                OnCanvasConfigChangedEvent(cca);
            }
        }

        private void NumericTextBox_RepParams_LostFocus(object sender, RoutedEventArgs e)
        {
            setNewRepParams();
        }

        private void NumericTextBox_RepParams_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
                setNewRepParams();
        }

        /**
         * Changes parameters according with the params text boxes
         */
        private void setNewRepParams()
        {
            RepresentationParams newRepParams = new RepresentationParams()
            {
                XMin = NumericTextBox_xMin.DoubleValue,
                XMax = NumericTextBox_xMax.DoubleValue,
                YMin = NumericTextBox_yMin.DoubleValue,
                YMax = NumericTextBox_yMax.DoubleValue
            };

            this.actualCanvasConfig.RepParams = newRepParams;
            CanvasConfigArgs cca = new CanvasConfigArgs();
            cca.NewCanvasConfig = this.actualCanvasConfig;
            OnCanvasConfigChangedEvent(cca);
        }
        #endregion

        #region Event invokers
        protected virtual void OnBarsConfigChangedEvent(BarsConfigArgs e)
        {
            this.BarsConfigChangedEvent?.Invoke(this, e);
        }

        protected virtual void OnPolylineConfigChangedEvent(PolylineConfigArgs e)
        {
            this.PolylineConfigChangedEvent?.Invoke(this, e);
        }

        protected virtual void OnPointsConfigChangedEvent(PointsConfigArgs e)
        {
            this.PointsConfigChangedEvent?.Invoke(this, e);
        }

        protected virtual void OnCanvasConfigChangedEvent(CanvasConfigArgs e)
        {
            this.CanvasConfigChangedEvent?.Invoke(this, e);
        }
        #endregion
    }
}
