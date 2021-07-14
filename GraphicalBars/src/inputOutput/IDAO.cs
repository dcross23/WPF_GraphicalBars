using GraphicalBars.src.model;
using System.Collections.Generic;
using System.Windows.Controls;

namespace GraphicalBars.src.inputOutput
{
    interface IDAO
    {
        bool Export(string path, List<MyPoint> pointCollection);
        bool Import(string path, out List<MyPoint> pointCollection);
        bool ExportCanvas(string path, Canvas canvas);
    }
}
