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
    public class Road
    {
        public Image image { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        
        public Road(string imageSource, int width, int height, int x, int y)
        {
            image = new Image();

            Width = width;
            Height = height;

            X = x;
            Y = y;

            image.Width = Width;
            image.Height = Height;
        }

        public void Draw()
        {
            Canvas.SetTop(image, X);
            Canvas.SetLeft(image, Y);
        }
    }
}
