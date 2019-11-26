using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace car_traffic_simulation.objects
{
    enum RoadType
    {
        Straight,
        Turn,
        Intersection
    };

    public class Road
    {
        public Image image { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Point2D Position { get; set; }
        
        public Road(string imageSource, int width, int height, int x, int y)
        {
            image = new Image();

            Width = width;
            Height = height;

            Position = new Point2D(x, y);

            image.Width = Width;
            image.Height = Height;
        }

        public void RenderBitmap()
        {
            Canvas.SetTop(image, 0);
            Canvas.SetLeft(image, 0);
        }
    }
}
