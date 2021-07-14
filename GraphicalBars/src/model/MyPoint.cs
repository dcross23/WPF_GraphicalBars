using System.ComponentModel;
using System.Windows.Media;

namespace GraphicalBars.src.model
{
    /* Elements of the observable collection */
    public class MyPoint: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double coordinateX;
        private double coordinateY;
        private Color pointColor;

        #region Constructors
        public MyPoint(double x, double y, Color color)
        {
            this.coordinateX = x;
            this.coordinateY = y;
            this.pointColor = color;
        }
        #endregion

        #region Properties
        public double CoordinateX
        {
            get { return this.coordinateX; }
            set { this.coordinateX = value; OnPropertyChanged("CoordinateX"); }
        }

        public double CoordinateY
        {
            get { return this.coordinateY; }
            set { this.coordinateY = value; OnPropertyChanged("CoordinateY"); }
        }

        public Color PointColor
        {
            get { return this.pointColor; }
            set { this.pointColor = value; OnPropertyChanged("PointColor"); }
        }
        #endregion

        #region INotifyPropertyChanged Method
        protected virtual void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        #endregion


        public override string ToString()
        {
            return (CoordinateX + "/" + CoordinateY + "/" + pointColor.ToString());
        }
    }
}
