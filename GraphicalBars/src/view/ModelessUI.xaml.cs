using GraphicalBars.src.model;
using GraphicalBars.src.utils;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GraphicalBars.src.view
{
    public partial class ModelessUI : Window
    {
        private ViewModel viewModel;
        public EventHandler SelectionChangedEvent;    //Event to highlight bar/circle point in canvas when row selected 

        #region Constructors
        public ModelessUI(ViewModel viewModel)
        {
            InitializeComponent();

            /* Reference to the viewModel */
            this.viewModel = viewModel;

            /* Bind data grid with our point observable collection */
            SheetDataGrid.ItemsSource = viewModel.GetPointCollection();
        }

        #endregion


        #region CRUD actions
        /* Opens dialog to add new points*/
        private void Button_AddCoordinates_Click(object sender, RoutedEventArgs e)
        {
            AddPointWindow apw = new AddPointWindow(this.viewModel.ZoomedRepresentationParams);
            apw.Owner = this;
            apw.ShowDialog();

            if(apw.DialogResult == true)
            {
                foreach (MyPoint mp in apw.CreatedPoints)
                {
                    viewModel.AddPoint(mp);

                }
            }
        }

        /* Deletes the point */
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MyPoint mp = (MyPoint)SheetDataGrid.SelectedItem;
            if (mp != null) viewModel.DeletePoint(mp);
        }

        /* Deletes all points */
        private void Button_RemovePoints_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(this, "¿Está seguro de eliminar todos los puntos?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    viewModel.DeleteAllPoints();
                    break;

                case MessageBoxResult.No:
                    break;
            }
        }

        #endregion


        #region Select graphs to draw
        private void Graphs_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItem_Bars)
            {
                if(viewModel != null) viewModel.HaveToDrawBars = true;
            }
            else if(sender == MenuItem_Polyline)
            {
                if (viewModel != null)  viewModel.HaveToDrawPolyline = true;
            }
            else if(sender == MenuItem_CirclePoints){
                if (viewModel != null) viewModel.HaveToDrawCirclePoints = true;               
            }
        }

        private void Graphs_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender == MenuItem_Bars)
            {
                this.viewModel.HaveToDrawBars = false;
            }
            else if (sender == MenuItem_Polyline)
            {
                this.viewModel.HaveToDrawPolyline = false;
            }
            else if(sender == MenuItem_CirclePoints){
                if (viewModel != null) viewModel.HaveToDrawCirclePoints = false;               
            }
        }

        #endregion


        #region Selection change actions
        private void SheetDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SheetDataGrid.SelectedItem == null)
            {
                TextBlock_PointSelected.Text = "";
                TextBlock_PointSelected.Visibility = Visibility.Hidden;
                BorderTextBlock_PointSelected.Visibility = Visibility.Hidden;
            }
            else
            {
                MyPoint mp = (MyPoint)SheetDataGrid.SelectedItem;
                //Gives format to X and Y coordinates of the selected point
                //" 0.00" means: doubles with no neg sign, will be formated as " 0.00" (double with space where neg sign would be and 2 decimals)
                //"-0.00" means: doubles with neg sign, will be formated as "-0.00" (double with neg sign and 2 decimals)
                TextBlock_PointSelected.Text = "(" + String.Format(ConfigConstants.cultureInfoUsed,"{0: 0.00;-0.00}", mp.CoordinateX) + " , " + String.Format(ConfigConstants.cultureInfoUsed, "{0: 0.00;-0.00}", mp.CoordinateY) + ")";
                TextBlock_PointSelected.Visibility = Visibility.Visible;
                BorderTextBlock_PointSelected.Visibility = Visibility.Visible;
            }

            OnSelectionChangedEvent(new EventArgs());
        }

        protected virtual void OnSelectionChangedEvent(EventArgs e)
        {
            SelectionChangedEvent?.Invoke(this, e);
        }
        #endregion


        #region Input/Output
        /** Opens a file dialog to select file path and tries to import it 
         *   File should be .txt (txt file) or .gbars (custom extension json file)
         */
        private void MenuItem_Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog importDialog = new OpenFileDialog()
            {
                Title = "Importar hoja",
                FileName = "Hoja", 
                DefaultExt = ConfigConstants.AppFileExtension, 
                Filter = "GraphicalBars Sheet (*"+ ConfigConstants.AppFileExtension + ")|*" + ConfigConstants.AppFileExtension +"| Documento de texto (*.txt)|*.txt",
                Multiselect = false
            };

            if ((bool)importDialog.ShowDialog())
            {
                bool imported = false;
                if (importDialog.FileName.EndsWith(".txt"))
                {
                    imported = viewModel.ImportTxt(importDialog.FileName);

                }
                else
                {
                    imported = viewModel.ImportJson(importDialog.FileName);
                }

                if (!imported)
                    _ = MessageBox.Show(this, "No se ha podido importar la hoja", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /** Opens a file dialog to select a file path and tries to import it 
         *   File can be saves as .txt (txt file) or .gbars (custom extension json file)
         */
        private void MenuItem_Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog exportDialog = new SaveFileDialog()
            {
                Title = "Exportar hoja",
                FileName = "Hoja",
                DefaultExt = ConfigConstants.AppFileExtension,
                Filter = "GraphicalBars Sheet (*" + ConfigConstants.AppFileExtension + ")|*" + ConfigConstants.AppFileExtension + "| Documento de texto (*.txt)|*.txt",
                AddExtension = true
            };


            if ((bool)exportDialog.ShowDialog())
            {
                bool imported = false;
                if (exportDialog.FileName.EndsWith(".txt"))
                {
                    imported = viewModel.ExportTxt(exportDialog.FileName);
                }
                else
                {
                    imported = viewModel.ExportJson(exportDialog.FileName);
                }

                if (!imported)
                    _ = MessageBox.Show(this, "No se ha podido exportar la hoja", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion


        #region Config menu
        private void MenuItem_Config_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow cw = new ConfigWindow(viewModel._BarsConfig, viewModel._PolylineConfig, viewModel._PointsConfig, viewModel._CanvasConfig, viewModel.SortByXorY);
            cw.BarsConfigChangedEvent += ConfigWindow_BarsConfigChanged;
            cw.PolylineConfigChangedEvent += ConfigWindow_PolylineConfigChanged;
            cw.PointsConfigChangedEvent += ConfigWindow_PointsConfigChanged;
            cw.CanvasConfigChangedEvent += ConfigWindow_CanvasConfigChanged;
            cw.ShowDialog();
            viewModel.SortByXorY = cw.SortByXorY;
        }


        private void ConfigWindow_BarsConfigChanged(object sender, BarsConfigArgs e)
        {
            viewModel._BarsConfig = e.NewBarsConfig;
        }

        private void ConfigWindow_PolylineConfigChanged(object sender, PolylineConfigArgs e)
        {
            viewModel._PolylineConfig = e.NewPolylineConfig;
        }

        private void ConfigWindow_PointsConfigChanged(object sender, PointsConfigArgs e)
        {
            viewModel._PointsConfig = e.NewPointsConfig;
        }

        private void ConfigWindow_CanvasConfigChanged(object sender, CanvasConfigArgs e)
        {
            viewModel._CanvasConfig = e.NewCanvasConfig;
        }

        #endregion


        #region Move point buttons
        private void Button_MovePointUp_Click(object sender, RoutedEventArgs e)
        {
            if (SheetDataGrid.SelectedItem != null)
            {
                if(SheetDataGrid.SelectedIndex > 0)
                {
                    viewModel.GetPointCollection().Move(SheetDataGrid.SelectedIndex, SheetDataGrid.SelectedIndex - 1);
                }
            }
        }

        private void Button_MovePointDown_Click(object sender, RoutedEventArgs e)
        {
            if (SheetDataGrid.SelectedItem != null)
            {
                if (SheetDataGrid.SelectedIndex < viewModel.GetPointCollection().Count - 1)
                {
                    viewModel.GetPointCollection().Move(SheetDataGrid.SelectedIndex, SheetDataGrid.SelectedIndex + 1);
                }
            }
        }


        #endregion

        private void Button_Sort_Click(object sender, RoutedEventArgs e)
        {
            if (sender == Button_Sort)
                this.viewModel.SortCollection();
            else if (sender == Button_SortRev)
                this.viewModel.SortRevCollection();
        }
    }
}
