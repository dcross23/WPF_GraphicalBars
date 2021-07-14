using System.Globalization;

namespace GraphicalBars.src.utils
{
    public class ConfigConstants
    {
        public static readonly string AppName = "Graphical Bars";
        public static readonly string AppFileExtension = ".gbars";

        public static readonly double GapBetweenAutoPoints = 0.2; //To improve performance, increase this gap(polyline will have less resolution)

        public static readonly BarsConfig DefaultBarsConfig = new BarsConfig() {
            UsePointColor = true,
            DashedBars = false,
            HighlightWhenSelectPoint = true,
            BarsWidth = 1
        };
        public static readonly PolylineConfig DefaultPolylineConfig = new PolylineConfig()
        {
            DashedPolyline = false,
            PolylineWidth = 2
        };
        public static readonly PointsConfig DefaultPointsConfig = new PointsConfig()
        {
            UsePointColor = true,
            HighlightWhenSelectPoint = true,
            PointsSize = 4
        };
        public static readonly CanvasConfig DefaultCanvasConfig = new CanvasConfig()
        {
            MinZoom = 0.1,
            MaxZoom = 5,
            RepParams = new RepresentationParams()
            {
                XMin = -10,
                XMax = 10,
                YMin = -10,
                YMax = 10
            }
        };

        public static readonly int SortByXorY = 0; //Sort by X by default

        public static readonly CultureInfo cultureInfoUsed = new CultureInfo("en-US"); //Have point as default decimal separator
    }



    /**
     * Struct that stores the data of the representation parameters for the plot
     */
    public struct RepresentationParams
    {
        public double XMin, XMax, YMin, YMax;
    };

    /**
     * Config structs
     */
    public struct BarsConfig
    {
        public bool UsePointColor { get; set; }
        public bool DashedBars { get; set; }
        public bool HighlightWhenSelectPoint { get; set; }
        public double BarsWidth { get; set; }
    }

    public struct PolylineConfig
    {
        public bool DashedPolyline { get; set; }
        public double PolylineWidth { get; set; }
    }

    public struct PointsConfig
    {
        public bool UsePointColor { get; set; }
        public bool HighlightWhenSelectPoint { get; set; }
        public double PointsSize { get; set; }
    }

    public struct CanvasConfig
    {
        public double MinZoom { get; set; }
        public double MaxZoom { get; set; }
        public RepresentationParams RepParams { get; set; }
    }
}
