using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_traffic_simulation.objects
{
    public class Point2D
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point2D(Point2D point2D)
        {
            X = point2D.X;
            Y = point2D.Y;
        }

        public static bool operator !=(Point2D pointA, Point2D pointB)
        {
            return !(pointA.X == pointB.X && pointA.Y == pointB.Y);
        }

        public static bool operator ==(Point2D pointA, Point2D pointB)
        {
            return (pointA.X == pointB.X && pointA.Y == pointB.Y);
        }

        public string toString()
        {
            return "X: " + X + ", Y: " + Y;
        }
    }
}
