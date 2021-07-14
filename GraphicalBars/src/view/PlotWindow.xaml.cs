using GraphicalBars.src.model;
using GraphicalBars.src.utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GraphicalBars.src.view
{

    public partial class PlotWindow : Window
    {
        private ViewModel viewModel;
        private ModelessUI mui;

        /* Floating menus variables */
        private Border floatingMenu;
        private Label zoomLabel, cursorPositionLabel;
        private Button purgeButton, saveCanvasImageButton;
        private StackPanel stackPanelForButtons;

        /* Purge variables */
        private Label purgeAdviseLabel;
        private Boolean purgeActivated;
        private Point[] corners;
        private Rectangle purgeSelection;

        #region Constructors
       public PlotWindow()
        {
            InitializeComponent();

            /* Create ViewModel and floating menus*/
            viewModel = new ViewModel();
            CreateFloatingMenus();

            /* Create modeless dialog for UI */
            mui = new ModelessUI(viewModel);
            mui.Show();

            /* Handlers for window events */
            this.Closed += Windows_FinishProgram;
            mui.Closed += Windows_FinishProgram;

            /* Register ModelessUI events */
            mui.SelectionChangedEvent += Mui_SelectionChangedEvent;

            /* Register view model events */
            viewModel.AddPointEvent += ViewModel_AddPoint;             //Event when adding new point
            viewModel.DeletePointEvent += ViewModel_DeletePointEvent;  //Event when deleting a point
            viewModel.UpdatePointEvent += ViewModel_UpdatePointEvent;  //Event when a point is updated (modified)
            viewModel.PurgePointsEvent += ViewModel_PurgePointsEvent;  //Event when points are being purged
            viewModel.ChangedGraphsEvent += ViewModel_ChangedGraphs;   //Event when checking/unchecking graphs to draw
            viewModel.BarsConfigChangedEvent += ViewModel_BarsConfigChangedEvent;         //Event when bars config has changed
            viewModel.PolylineConfigChangedEvent += ViewModel_PolylineConfigChangedEvent; //Event when polyline config has changed
            viewModel.PointsConfigChangedEvent += ViewModel_PointsConfigChangedEvent;     //Event when points config has changed
            viewModel.CanvasConfigChangedEvent += ViewModel_CanvasConfigChangedEvent;     //Event when canvas config has changed


            /* Initialize other variables */
            this.purgeActivated = false;
            this.corners = new Point[2];
       }

        #endregion




        #region ModelessUI Events
        private void Mui_SelectionChangedEvent(object sender, EventArgs e)
        {
            redraw();
        }
        #endregion


        #region View Model Events
        //Most/All of them just redraw all canvas. I could have done just one method, 
        //  but I have left them separate in case they need to be changed

        private void ViewModel_AddPoint(object sender, EventArgs e)
        {
            redraw();
        }

        private void ViewModel_DeletePointEvent(object sender, EventArgs e)
        {
            redraw();
        }

        private void ViewModel_UpdatePointEvent(object sender, EventArgs e)
        {
            redraw();
        }

        private void ViewModel_PurgePointsEvent(object sender, EventArgs e)
        {
            redraw();
        }

        private void ViewModel_ChangedGraphs(object sender, EventArgs e)
        {
            redraw();
        }

        private void ViewModel_CanvasConfigChangedEvent(object sender, EventArgs e)
        {
            this.zoomLabel.Content = "Zoom " + Math.Round(viewModel.Zoom * 100) + " %";
            redraw();
        }

        private void ViewModel_BarsConfigChangedEvent(object sender, EventArgs e)
        {
            redraw();
        }

        private void ViewModel_PointsConfigChangedEvent(object sender, EventArgs e)
        {
            redraw();
        }

        private void ViewModel_PolylineConfigChangedEvent(object sender, EventArgs e)
        {
            redraw();
        }
        #endregion


        #region Other methods
        /**
         * Program finishes when any of the 2 windows is closed
         */
        private void Windows_FinishProgram(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        /**
         * Redraws all elements in the canvas with the new size
         */
        private void PlotCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            redraw();
        }

        /**
         * Creates floating labels and menus for canvas
         */
        private void CreateFloatingMenus()
        {
            /* Create zoom label */
            this.zoomLabel = new Label()
            {
                BorderThickness = new Thickness(1), Content = "Zoom 100%",
                FontSize = 10, FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromArgb(255, 241, 241, 241)),
                Visibility = Visibility.Hidden
            };
            this.zoomLabel.SetResourceReference(Label.BorderBrushProperty, "MenuBordersBrush"); //Dynamic resource "binding"
            this.zoomLabel.SetResourceReference(Label.ForegroundProperty, "MenuBordersBrush");
            Canvas.SetLeft(zoomLabel, 5);
            Canvas.SetBottom(zoomLabel, 5);


            /* Create cursor position label */
            this.cursorPositionLabel = new Label()
            {
                BorderThickness = new Thickness(1),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromArgb(255, 241, 241, 241)),
                Visibility = Visibility.Hidden
            };
            this.cursorPositionLabel.SetResourceReference(Label.BorderBrushProperty, "MenuBordersBrush"); //Dynamic resource "binding"
            this.cursorPositionLabel.SetResourceReference(Label.ForegroundProperty, "MenuBordersBrush");
            Canvas.SetRight(cursorPositionLabel, 5);
            Canvas.SetBottom(cursorPositionLabel, 5);


            /* Create purge advise label */
            this.purgeAdviseLabel = new Label()
            {
                Content = "Purga activada",
                BorderThickness = new Thickness(1),
                FontSize = 8,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromArgb(255, 241, 241, 241)),
                Visibility = Visibility.Hidden
            };
            this.purgeAdviseLabel.SetResourceReference(Label.BorderBrushProperty, "MenuBordersBrush"); //Dynamic resource "binding"
            this.purgeAdviseLabel.SetResourceReference(Label.ForegroundProperty, "MenuBordersBrush");
            Canvas.SetRight(purgeAdviseLabel, 5);
            Canvas.SetTop(purgeAdviseLabel, 5);
            


            //Create purge button for floating menu (outside to add Click event
            this.purgeButton = new Button()
            {
                Name = "ButtonPurge",
                Content = new Image()
                {
                    Source = new BitmapImage(new Uri(@"pack://application:,,,/"
                                                     + Assembly.GetExecutingAssembly().GetName().Name
                                                     + ";component/icons/SelectZoneButton.png", UriKind.RelativeOrAbsolute))
                },
                Width = 20,
                Height = 20,
                ToolTip = "Purgar puntos"
            };
            purgeButton.Click += PurgeButton_Click;

            this.saveCanvasImageButton = new Button()
            {
                Name = "ButtonSaveImageCanvas",
                Content = new Image()
                {
                    Source = new BitmapImage(new Uri(@"pack://application:,,,/"
                                                    + Assembly.GetExecutingAssembly().GetName().Name
                                                    + ";component/icons/ImageIcon.png", UriKind.RelativeOrAbsolute))
                },
                Width = 20,
                Height = 20,
                Margin = new Thickness(2,0,0,0),
                ToolTip = "Guardar canvas como imagen"
            };
            saveCanvasImageButton.Click += SaveCanvasImageButton_Click;



            //Create floating menu
            this.stackPanelForButtons = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                    {
                        //Button to select and purge points
                        purgeButton,

                        //Boton para guardar imagen
                        saveCanvasImageButton
                    }
            };
            this.stackPanelForButtons.SetResourceReference(StackPanel.BackgroundProperty, "MenuBordersBrush"); //Dynamic resource "binding"

            this.floatingMenu = new Border()
            {
                BorderThickness = new Thickness(2), Visibility = Visibility.Hidden,
                Child = stackPanelForButtons
            };
            this.floatingMenu.SetResourceReference(Border.BorderBrushProperty, "MenuBordersBrush"); //Dynamic resource "binding"
            Canvas.SetLeft(floatingMenu, 5);
            Canvas.SetTop(floatingMenu, 5);
        }

        /**
         * Opens file dialog to select directory to save, and saves the canvas as a image
         */
        private void SaveCanvasImageButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog exportDialog = new SaveFileDialog()
            {
                Title = "Guardar imagen",
                FileName = "Imagen",
                DefaultExt = ".png",
                Filter = "Imagen (*.png)|*.png",
                AddExtension = true
            };


            if ((bool)exportDialog.ShowDialog())
            {        
                if (!viewModel.ExportCanvas(exportDialog.FileName, PlotCanvas))
                    _ = MessageBox.Show(this, "No se ha podido exportar la imagen", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /**
         * Handler for purge button click.
         */
        private void PurgeButton_Click(object sender, RoutedEventArgs e)
        {
            if (purgeActivated)
            {
                purgeActivated = false;
                purgeAdviseLabel.Visibility = Visibility.Hidden;
                if (PlotCanvas.Children.Contains(purgeSelection))
                    PlotCanvas.Children.Remove(purgeSelection);
            }
            else
            {
                purgeActivated = true;
                purgeAdviseLabel.Visibility = Visibility.Visible;
            }
         }

        /**
         * Function that performs the purge
         */
        private void Purge()
        {
            string msg = "Se eliminarán los puntos fuera del area seleccionada: \n";

            double x1 = PlotUtils.xPant_to_xReal(corners[0].X, PlotCanvas.ActualWidth, viewModel.ZoomedRepresentationParams.XMin, viewModel.ZoomedRepresentationParams.XMax);   //1rst corner x
            double y1 = PlotUtils.yPant_to_yReal(corners[0].Y, PlotCanvas.ActualHeight, viewModel.ZoomedRepresentationParams.YMin, viewModel.ZoomedRepresentationParams.YMax);  //1rst corner y
            double x2 = PlotUtils.xPant_to_xReal(corners[1].X, PlotCanvas.ActualWidth, viewModel.ZoomedRepresentationParams.XMin, viewModel.ZoomedRepresentationParams.XMax);   //2nd  corner x
            double y2 = PlotUtils.yPant_to_yReal(corners[1].Y, PlotCanvas.ActualHeight, viewModel.ZoomedRepresentationParams.YMin, viewModel.ZoomedRepresentationParams.YMax);  //2nd  corner y

            string x1S = String.Format(ConfigConstants.cultureInfoUsed, "{0:0.00}", x1);
            string y1S = String.Format(ConfigConstants.cultureInfoUsed, "{0:0.00}", y1);
            string x2S = String.Format(ConfigConstants.cultureInfoUsed, "{0:0.00}", x2);
            string y2S = String.Format(ConfigConstants.cultureInfoUsed, "{0:0.00}", y2);

            if (x1 < x2)  msg += "   " + x1S + "<= X <=" + x2S + "      ";
            else          msg += "   " + x2S + "<= X <=" + x1S + "      ";

            //Y is inversed because we are working with cartesian plane (y grows going upper) but the canvas works the other way around
            // Solution -> instead of comparing the corners coordinates, compare the corners coordinates but transformed to work in our space system 
            //   (comapate y1, y2 instead of corners y coord).
            if (y1 < y2)  msg += "   " + y1S + "<= Y <=" + y2S + "\n";
            else          msg += "   " + y2S + "<= Y <=" + y1S + "\n";

            msg += "¿Esta de acuerdo?";

            this.mui.IsEnabled = false; //Disable modeless window while message box is still activa
            MessageBoxResult askIfPurge = MessageBox.Show(this, msg, "¿Purgar puntos?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            switch (askIfPurge)
            {
                case MessageBoxResult.OK:
                    this.viewModel.PurgePoints(x1, y1, x2, y2);
                    break;

                case MessageBoxResult.Cancel:
                    break;
            }

            this.mui.IsEnabled = true;
            PlotCanvas.Children.Remove(purgeSelection);
            purgeAdviseLabel.Visibility = Visibility.Hidden;
            purgeActivated = false;
        }

        #endregion


        #region Drawing functions
        /**
         * Draws the axys in the canvas
         */
        private void drawAxys()
        {
            if (PlotCanvas != null)
            {
                if (viewModel._CanvasConfig.RepParams.YMin < 0 && viewModel._CanvasConfig.RepParams.YMax > 0) {
                    Line xAxys = PlotUtils.GetXAxys(PlotCanvas.ActualWidth, PlotCanvas.ActualHeight, viewModel._CanvasConfig.RepParams);
                    xAxys.Opacity = 0.4;
                    PlotCanvas.Children.Add(xAxys);
                }

                if (viewModel._CanvasConfig.RepParams.XMin < 0 && viewModel._CanvasConfig.RepParams.XMax > 0)
                {
                    Line yAxys = PlotUtils.GetYAxys(PlotCanvas.ActualWidth, PlotCanvas.ActualHeight, viewModel._CanvasConfig.RepParams);
                    yAxys.Opacity = 0.4;
                    PlotCanvas.Children.Add(yAxys);
                }                               
            }
        }


        /**
         * Draws the points with the graphs
         */
        private void drawGraphs()
        {
            if (PlotCanvas != null)
            {
                //Orders it for polyline to be displayed correctly and dont depend on the order of the points
                List<MyPoint> list = viewModel.GetPointCollection().OrderBy(mp => mp.CoordinateX).ToList();

                //Point collection for polyline
                PointCollection pc = new PointCollection();

                //List to store the circle points and add them after adding polyline to 
                // draw the points over the polyline
                List<Ellipse> ellipses = new List<Ellipse>();

                foreach (MyPoint mp in list)
                {
                    //Point from my point (for circle points and polyline)
                    Point newPoint = new Point()
                    {
                        X = PlotUtils.xReal_to_xPant(mp.CoordinateX, PlotCanvas.ActualWidth, viewModel.ZoomedRepresentationParams.XMin, viewModel.ZoomedRepresentationParams.XMax),
                        Y = PlotUtils.yReal_to_yPant(mp.CoordinateY, PlotCanvas.ActualHeight, viewModel.ZoomedRepresentationParams.YMin, viewModel.ZoomedRepresentationParams.YMax)
                    };
                    pc.Add(newPoint);


                    //Circle points
                    if (viewModel.HaveToDrawCirclePoints)
                    {
                        Ellipse newCircle = new Ellipse()
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 1
                        };

                        newCircle.SetResourceReference(Ellipse.FillProperty, "PointsBrush");
                        if (viewModel._PointsConfig.UsePointColor)
                            newCircle.Fill = new SolidColorBrush(mp.PointColor);

                        //Depends on zoom because here we are not using representation
                        //  params for nothing, just drawing it
                        newCircle.Width = newCircle.Height = viewModel._PointsConfig.PointsSize * 1 / viewModel.Zoom;
                        if (viewModel._PointsConfig.HighlightWhenSelectPoint && mui.SheetDataGrid.SelectedItem!=null && mp == (MyPoint)(mui.SheetDataGrid.SelectedItem))
                            newCircle.Width = newCircle.Height += 4 * 1 / viewModel.Zoom;

                        
                        Canvas.SetLeft(newCircle, newPoint.X - newCircle.Width / 2);
                        Canvas.SetTop(newCircle, newPoint.Y - newCircle.Height / 2);
                        ellipses.Add(newCircle);
                    }

                    //Bars
                    if (viewModel.HaveToDrawBars)
                    {
                        Line newBar = new Line()
                        {
                            X1 = newPoint.X,
                            Y1 = newPoint.Y,
                            X2 = newPoint.X,
                            Y2 = PlotUtils.yReal_to_yPant(0, PlotCanvas.ActualHeight, viewModel.ZoomedRepresentationParams.YMin, viewModel.ZoomedRepresentationParams.YMax)
                        };

                        newBar.SetResourceReference(Line.StrokeProperty, "BarsBrush");
                        if (viewModel._BarsConfig.UsePointColor)
                            newBar.Stroke = new SolidColorBrush(mp.PointColor);

                        newBar.StrokeThickness = viewModel._BarsConfig.BarsWidth;
                        if (viewModel._BarsConfig.HighlightWhenSelectPoint && mui.SheetDataGrid.SelectedItem != null && mp == (MyPoint)(mui.SheetDataGrid.SelectedItem))
                            newBar.StrokeThickness += 2;

                        if (viewModel._BarsConfig.DashedBars)
                            newBar.StrokeDashArray = new DoubleCollection() { 2 };
                        else
                            newBar.StrokeDashArray = null;

                        PlotCanvas.Children.Add(newBar);
                    }
                }

                //Polyline
                if (viewModel.HaveToDrawPolyline)
                {
                    Polyline newPolyline = new Polyline()
                    {
                        StrokeThickness = viewModel._PolylineConfig.PolylineWidth,
                        Points = pc
                    };
                    newPolyline.SetResourceReference(Polyline.StrokeProperty, "PolylineBrush"); //Dynamic resource for color

                    if (viewModel._PolylineConfig.DashedPolyline)
                        newPolyline.StrokeDashArray = new DoubleCollection() { 2 };
                        
                    PlotCanvas.Children.Add(newPolyline);
                }

                //Circle points
                if (viewModel.HaveToDrawCirclePoints) {
                    foreach (Ellipse e in ellipses)
                        PlotCanvas.Children.Add(e);
                }
            }
        }

        /**
         * Redraws all elements
         */
        public void redraw()
        {
            PlotCanvas.Children.Clear();
            drawAxys();
            drawGraphs();

            PlotCanvas.Children.Add(floatingMenu);
            PlotCanvas.Children.Add(zoomLabel);
            PlotCanvas.Children.Add(cursorPositionLabel);
            PlotCanvas.Children.Add(purgeAdviseLabel);
        }
        #endregion


        #region PlotCanvas mouse events
        private void PlotCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            /* Enables all floating labels/menus */
            floatingMenu.Visibility = Visibility.Visible;
            zoomLabel.Visibility = Visibility.Visible;
            cursorPositionLabel.Visibility = Visibility.Visible;
            
            /* If purge is activated, it also enables purge activaded advise label */
            if(purgeActivated) 
                purgeAdviseLabel.Visibility = Visibility.Visible;
        }

        private void PlotCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            /* Disables all floating labels/menus */
            floatingMenu.Visibility = Visibility.Hidden;
            zoomLabel.Visibility = Visibility.Hidden;
            cursorPositionLabel.Visibility = Visibility.Hidden;
            purgeAdviseLabel.Visibility = Visibility.Hidden;

            /* Problem if we are selecting and leave canvas -> purge selection is finished*/
            if (purgeActivated && e.LeftButton == MouseButtonState.Pressed)
                Purge();
        }

        private void PlotCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            /* Get mouse position to show in label */
            Point mousePos = e.GetPosition(PlotCanvas);
            double x = PlotUtils.xPant_to_xReal(mousePos.X, PlotCanvas.ActualWidth, viewModel.ZoomedRepresentationParams.XMin, viewModel.ZoomedRepresentationParams.XMax);
            double y = PlotUtils.yPant_to_yReal(mousePos.Y, PlotCanvas.ActualHeight, viewModel.ZoomedRepresentationParams.YMin, viewModel.ZoomedRepresentationParams.YMax);
            this.cursorPositionLabel.Content = "(" + String.Format(ConfigConstants.cultureInfoUsed,"{0: 0.00;-0.00}", x) + "," + String.Format(ConfigConstants.cultureInfoUsed, "{0: 0.00;-0.00}", y) + ")";

            /* If purge is selected and we are selecting a zone (left button is pressed) -> gets the mouse position for the 2nd corner */
            if (purgeActivated && e.LeftButton == MouseButtonState.Pressed)
            {
                corners[1] = e.GetPosition(PlotCanvas);

                //If the 2nd corner is on the left of the first corner, the rectangle will be 
                // drawn setting his left to the corner more in the left (the 2nd corner)
                purgeSelection.Width = Math.Abs(corners[0].X - corners[1].X);
                if (corners[0].X > corners[1].X)
                    Canvas.SetLeft(purgeSelection, corners[1].X);


                //If the 2nd corner is above of the first corner, the rectangle will be 
                // drawn setting his top to the corner that is above (the 2nd corner)
                purgeSelection.Height = Math.Abs(corners[0].Y - corners[1].Y);
                if (corners[0].Y > corners[1].Y)
                    Canvas.SetTop(purgeSelection, corners[1].Y);

            }
        }

        private void PlotCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (purgeActivated && e.LeftButton == MouseButtonState.Pressed)
            {
                corners[0] = corners[1] = e.GetPosition(PlotCanvas);
                purgeSelection = new Rectangle
                {
                    Stroke = Brushes.Navy,
                    StrokeDashArray = new DoubleCollection() { 4 },
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent
                };
                Canvas.SetLeft(purgeSelection, corners[0].X);
                Canvas.SetTop(purgeSelection, corners[0].Y);

                PlotCanvas.Children.Add(purgeSelection);
            }
        }

        private void PlotCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (purgeActivated)
            {
                corners[1] = e.GetPosition(PlotCanvas);
                Purge();
            }
        }


        /**
         * Handler to zoom in/out when mouse wheel is moved
         */
        private void PlotCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double addToZoom = (e.Delta > 0) ? (-0.1) : (0.1);
         
            if ((addToZoom>0 && viewModel.Zoom <= viewModel.MaxZoom-0.1) || (addToZoom<0 && viewModel.Zoom >= viewModel.MinZoom+0.1))
                viewModel.Zoom += addToZoom;

            zoomLabel.Content = "Zoom " + Math.Round(viewModel.Zoom * 100) + " %";
            redraw();
        }
        #endregion


    }
}
