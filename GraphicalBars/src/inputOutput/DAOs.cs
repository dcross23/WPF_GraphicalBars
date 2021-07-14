using GraphicalBars.src.model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GraphicalBars.src.inputOutput
{
    /* Imports/Exports working with .txt files */
    class TextDAO : IDAO
    {
        public bool Export(string path, List<MyPoint> pointCollection)
        {
            try
            {
                using (TextWriter tw = new StreamWriter(path))
                {
                    foreach (MyPoint mp in pointCollection)
                    {
                        tw.WriteLine(mp.ToString());
                    }
                }

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool Import(string path, out List<MyPoint> pointCollection)
        {
            pointCollection = new List<MyPoint>();

            try
            {
                using (TextReader tr = new StreamReader(path))
                {
                    string line;
                    while ((line = tr.ReadLine()) != null)
                    {
                        string[] properties = line.Split('/');

                        double x = Double.Parse(properties[0]);
                        double y = Double.Parse(properties[1]);
                        Color c = (Color)ColorConverter.ConvertFromString(properties[2]);

                        pointCollection.Add(new MyPoint(x, y, c));
                        Console.WriteLine(line);
                    }
                }

            }
            catch (Exception)
            {
                pointCollection = null;
                return false;
            }

            return true;
        }

        //Nothing to be done in this DAO
        public bool ExportCanvas(string path, Canvas canvas)
        {
            throw new NotImplementedException();
        }
    }


    /* Imports/Exports using custom extension json files */
    class JsonDAO : IDAO
    {
        public bool Export(string path, List<MyPoint> pointCollection)
        {
            try
            {
                File.WriteAllText(@path, JsonConvert.SerializeObject(pointCollection, Formatting.Indented));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool Import(string path, out List<MyPoint> pointCollection)
        {
            try
            {
                pointCollection = new List<MyPoint>(JsonConvert.DeserializeObject<List<MyPoint>>(@File.ReadAllText(@path)));
            }
            catch (Exception)
            {
                pointCollection = null;
                return false;
            }
            return true;
        }

        //Nothing to be done in this DAO
        public bool ExportCanvas(string path, Canvas canvas)
        {
            throw new NotImplementedException();
        }
    }


    /* Exports canvas as a image */
    class ImageDAO : IDAO
    {
        //Nothing to be done in this DAO
        public bool Export(string path, List<MyPoint> canvas)
        {
            throw new NotImplementedException();
        }
        public bool Import(string path, out List<MyPoint> pointCollection)
        {
            throw new NotImplementedException();
        }


        //Here we export the canvas as image -> getted from: https://www.iditect.com/how-to/55131083.html
        public bool ExportCanvas(string path, Canvas canvas)
        {
            try
            {
                //Renders a visual object (canvas) as a bit map
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width, (int)canvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
                rtb.Render(canvas);

                //Get a bitmap cutter (croppedbitmap cuts the de bitmap) and then we code this bitmap with the png encoder
                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                CroppedBitmap cb = new CroppedBitmap(rtb, new Int32Rect(0, 0, (int)canvas.ActualWidth, (int)canvas.ActualHeight));
                pngEncoder.Frames.Add(BitmapFrame.Create(cb));

                //Write the encoded image in a file
                using (FileStream fs = File.OpenWrite(@path))
                {
                    pngEncoder.Save(fs);
                }
            }catch(Exception)
            {
                return false;
            }
            return true;

        }

    }
}
