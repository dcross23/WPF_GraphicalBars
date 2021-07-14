using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GraphicalBars.src.model
{
    public class Model
    {
        private ObservableCollection<MyPoint> pointCollection;

        #region Constructors
        public Model()
        {
            this.pointCollection = new ObservableCollection<MyPoint>();
        }
        #endregion


        #region Functions to return representations of the point collection
        /**
         * Functions to returns copies of the observable collection
         */
        public ObservableCollection<MyPoint> GetPointCollection()
        {
            return this.pointCollection;
        }

        public List<MyPoint> GetPointCollectionAsList()
        {
            return new List<MyPoint>(this.pointCollection);
        }
        #endregion


        #region Functions for working with the point collection
        /**
         * Removes all the points that are not in the selected area
         */
        public void PurgePoints(double xMin, double xMax, double yMin, double yMax)
        {
            foreach(MyPoint mp in this.pointCollection.ToList())
            {
                if(mp.CoordinateX < xMin || mp.CoordinateX > xMax || mp.CoordinateY < yMin || mp.CoordinateY > yMax)
                    this.pointCollection.Remove(mp);
            }
        }

        /**
         * Adds a new point to the collection
         */
        public void AddNewPoint(MyPoint newPoint)
        {
            this.pointCollection.Add(newPoint);
        }

        /**
         * Deletes a point from the collection
         */
        public void DeletePoint(MyPoint newPoint)
        {
            this.pointCollection.Remove(newPoint);
        }

        /**
         * Sort collection ascending by X or Y using insertion sort algorithm
         */
        public void SortCollection(int SortByXorY)
        {
            for (int i = 1; i < this.pointCollection.Count; ++i)
            {
                MyPoint key = this.pointCollection[i];
                int j = i - 1;

                if(SortByXorY == 0)
                    while (j >= 0 && this.pointCollection[j].CoordinateX > key.CoordinateX)
                    {
                        pointCollection.Move(j + 1, j);
                        j = j - 1;
                    }
                else if (SortByXorY == 1)
                {
                    while (j >= 0 && this.pointCollection[j].CoordinateY > key.CoordinateY)
                    {
                        pointCollection.Move(j + 1, j);
                        j = j - 1;
                    }

                }

                pointCollection[j + 1] = key;
            }
        }

        /**
         * Sort collection descending by X or Y using insertion sort algorithm
         */
        public void SortRevCollection(int SortByXorY)
        {
            for (int i = 1; i < this.pointCollection.Count; ++i)
            {
                MyPoint key = this.pointCollection[i];
                int j = i - 1;

                if (SortByXorY == 0)
                    while (j >= 0 && this.pointCollection[j].CoordinateX < key.CoordinateX)
                    {
                        pointCollection.Move(j + 1, j);
                        j = j - 1;
                    }
                else if (SortByXorY == 1)
                {
                    while (j >= 0 && this.pointCollection[j].CoordinateY < key.CoordinateY)
                    {
                        pointCollection.Move(j + 1, j);
                        j = j - 1;
                    }

                }

                pointCollection[j + 1] = key;
            }
        }
        #endregion
    }
}
