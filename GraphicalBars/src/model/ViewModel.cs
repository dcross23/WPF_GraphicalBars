using GraphicalBars.src.inputOutput;
using GraphicalBars.src.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace GraphicalBars.src.model
{

    /**
     * Holds the data for the representation in the plot
     */
    public class ViewModel
    {
        //Events
        public event EventHandler AddPointEvent;
        public event EventHandler DeletePointEvent;
        public event EventHandler UpdatePointEvent;
        public event EventHandler PurgePointsEvent;
        public event EventHandler ChangedGraphsEvent;
        public event EventHandler CanvasConfigChangedEvent;
        public event EventHandler PointsConfigChangedEvent;
        public event EventHandler BarsConfigChangedEvent;
        public event EventHandler PolylineConfigChangedEvent;

        //Variables
        private Model model;
        private IDAO dao;

        private double plotZoom;
        private Boolean drawBarsFlag;
        private Boolean drawPolylineFlag;
        private Boolean drawCirclePointsFlag;

        private BarsConfig barsConfig;
        private PolylineConfig polylineConfig;
        private PointsConfig pointsConfig;
        private CanvasConfig canvasConfig;


        #region Constructors
        public ViewModel()
        {
            //Creates the model
            this.model = new Model();

            //Default representation params
            this.barsConfig = ConfigConstants.DefaultBarsConfig;
            this.polylineConfig = ConfigConstants.DefaultPolylineConfig;
            this.pointsConfig = ConfigConstants.DefaultPointsConfig;
            this.canvasConfig = ConfigConstants.DefaultCanvasConfig;

            //Default zoom -> 1 is 100%
            plotZoom = 1;

            //Default settings (draw only bars)
            this.drawBarsFlag = true;
            this.drawPolylineFlag = false;

            //Default sort
            this.SortByXorY = ConfigConstants.SortByXorY;
        }
        #endregion


        #region Properties
        public RepresentationParams ZoomedRepresentationParams
        {
            get {
                return new RepresentationParams()
                {
                    XMin = this.canvasConfig.RepParams.XMin * plotZoom,
                    XMax = this.canvasConfig.RepParams.XMax * plotZoom,
                    YMin = this.canvasConfig.RepParams.YMin * plotZoom,
                    YMax = this.canvasConfig.RepParams.YMax * plotZoom
                };
            }
        }

        public double Zoom
        {
            get { return this.plotZoom; }
            set { this.plotZoom = value; }
        }

        public double MinZoom
        {
            get { return this.canvasConfig.MinZoom; }
        }

        public double MaxZoom
        {
            get { return this.canvasConfig.MaxZoom; }
        }

        public Boolean HaveToDrawBars
        {
            get { return this.drawBarsFlag; }
            set { 
                this.drawBarsFlag = value;
                OnChangedGraphsEvent(new EventArgs());
            }
        }

        public Boolean HaveToDrawPolyline
        {
            get { return this.drawPolylineFlag; }
            set {
                this.drawPolylineFlag = value;
                OnChangedGraphsEvent(new EventArgs());
            }
        }

        public Boolean HaveToDrawCirclePoints
        {
            get { return this.drawCirclePointsFlag; }
            set
            {
                this.drawCirclePointsFlag = value;
                OnChangedGraphsEvent(new EventArgs());
            }
        }
        
        public BarsConfig _BarsConfig
        {
            get { return this.barsConfig; }
            set { this.barsConfig = value;
                OnBarsConfigChangedEvent(new EventArgs());
                }
        }

        public PolylineConfig _PolylineConfig
        {
            get { return this.polylineConfig; }
            set { this.polylineConfig = value;
                OnPolylineConfigChangedEvent(new EventArgs());
            }
        }

        public PointsConfig _PointsConfig
        {
            get { return this.pointsConfig; }
            set { this.pointsConfig = value;
                OnPointsConfigChangedEvent(new EventArgs());
            }
        }

        public CanvasConfig _CanvasConfig
        {
            get { return this.canvasConfig; }
            set { this.canvasConfig = value;
                if (this.Zoom <= canvasConfig.MinZoom)
                    this.Zoom = canvasConfig.MinZoom;
                else if (this.Zoom >= canvasConfig.MaxZoom)
                    this.Zoom = canvasConfig.MaxZoom;

                OnCanvasConfigChangedEvent(new EventArgs());
            }
        }
       
        public int SortByXorY { get; set; }
        #endregion


        #region Get pointColection function for binding with datagrid
        public ObservableCollection<MyPoint> GetPointCollection()
        {
            return model.GetPointCollection();
        }
        #endregion


        #region Actions with points
        public void AddPoint(MyPoint newPoint)
        {
            newPoint.PropertyChanged += NewPoint_PropertyChanged;
            this.model.AddNewPoint(newPoint);
            OnAddPointEvent(new EventArgs());
        }

        public void DeletePoint(MyPoint mp)
        {
            this.model.DeletePoint(mp);
            OnDeletePointEvent(new EventArgs());
        }

        public void DeleteAllPoints()
        {
            foreach (MyPoint mp in model.GetPointCollectionAsList())
            {
                DeletePoint(mp);
            }
        }

        //When property changed -> point has been modified
        private void NewPoint_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnUpdatePointEvent(new EventArgs());
        }
 
        public void PurgePoints(double x1, double y1, double x2, double y2)
        {
            double xMin = x1 < x2 ? x1 : x2;
            double xMax = x1 > x2 ? x1 : x2;
            double yMin = y1 < y2 ? y1 : y2;
            double yMax = y1 > y2 ? y1 : y2;

            this.model.PurgePoints(xMin, xMax, yMin, yMax);
            OnPurgePointsEvent(new EventArgs());
        }

        //Sorting collection methods
        public void SortCollection()
        {
            model.SortCollection(this.SortByXorY);
        }

        public void SortRevCollection()
        {
            model.SortRevCollection(this.SortByXorY);
        }
        #endregion


        #region Input/Output

        /**
         * Exports point collection to a delimited txt
         */
        public bool ExportTxt(string path)
        {
            if (!(dao is TextDAO)) 
                dao = new TextDAO();
            return dao.Export(path, new List<MyPoint>(GetPointCollection()));         
        }

        /**
         * Imports point collection from a delimited txt
         */
        public bool ImportTxt(string path)
        {
            if (!(dao is TextDAO))
                dao = new TextDAO();
            List<MyPoint> pointsLoaded;
            if (dao.Import(path, out pointsLoaded))
            {
                model.GetPointCollection().Clear();
                foreach (MyPoint mp in pointsLoaded)
                {
                    AddPoint(mp);
                }
                return true;
            }
            else
                return false;
        }

        /**
         * Exports point collection to a json string
         */
        public bool ExportJson(string path)
        {
            if (!(dao is JsonDAO))
                dao = new JsonDAO();
            return dao.Export(path, new List<MyPoint>(GetPointCollection()));
        }

        /**
         * Imports point collection from a json string
         */
        public bool ImportJson(string path)
        {
            if (!(dao is JsonDAO))
                dao = new JsonDAO();

            List<MyPoint> pointsLoaded;
            if (dao.Import(path, out pointsLoaded))
            {
                model.GetPointCollection().Clear();
                foreach (MyPoint mp in pointsLoaded)
                {
                    AddPoint(mp);
                }
                return true;
            }
            else
                return false;
        }


        public bool ExportCanvas(string path, Canvas canvas)
        {
            if (!(dao is ImageDAO))
                dao = new ImageDAO();

            return dao.ExportCanvas(path, canvas);
        }

        #endregion


        #region Event Handlers
        protected virtual void OnAddPointEvent(EventArgs e)
        {
            AddPointEvent?.Invoke(this, e); 
        }

        protected virtual void OnDeletePointEvent(EventArgs e)
        {
            DeletePointEvent?.Invoke(this, e);
        }

        protected virtual void OnChangedGraphsEvent(EventArgs e)
        {
            ChangedGraphsEvent?.Invoke(this, e);
        }

        protected virtual void OnPurgePointsEvent(EventArgs e)
        {
            PurgePointsEvent?.Invoke(this, e);
        }

        protected virtual void OnUpdatePointEvent(EventArgs e)
        {
            UpdatePointEvent?.Invoke(this, e);
        }

        protected virtual void OnBarsConfigChangedEvent(EventArgs e)
        {
            BarsConfigChangedEvent?.Invoke(this, e);
        }

        protected virtual void OnPointsConfigChangedEvent(EventArgs e)
        {
            PointsConfigChangedEvent?.Invoke(this, e);
        }

        protected virtual void OnCanvasConfigChangedEvent(EventArgs e)
        {
            CanvasConfigChangedEvent?.Invoke(this, e);
        }

        protected virtual void OnPolylineConfigChangedEvent(EventArgs e)
        {
            PolylineConfigChangedEvent?.Invoke(this, e);
        }
        #endregion

    }
}
