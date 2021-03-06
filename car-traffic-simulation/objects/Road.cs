﻿using System;
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

    public class RoadTexture
    {
        public Image image { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Point2D Position { get; set; }
        
        public RoadTexture(string imageSource, int width, int height, int x, int y)
        {
            image = new Image();

            Width = width;
            Height = height;

            Position = new Point2D(x, y);

            image.Width = Width;
            image.Height = Height;
        }

        public void draw(int topOffset = 0, int leftOffset = 0)
        {
            Canvas.SetTop(image, topOffset);
            Canvas.SetLeft(image, leftOffset);
        }
    }
}
