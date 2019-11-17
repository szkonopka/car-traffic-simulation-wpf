﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_traffic_simulation.objects
{
    public enum CardinalDirection
    {
        North,
        East,
        South,
        West
    }

    public enum MovementAxis
    {
        X, 
        Y
    };

    public class EdgeRoad
    {
        public int ID { get; set; }
        public Point2D From { get; set; }
        public Point2D To { get; set; }
        public MovementAxis MovementAxis { get; set; }
        public CardinalDirection Direction { get; set; }

        public EdgeRoad(int id, int fromX, int fromY, int toX, int toY)
        {
            if (!IsEdgeParallelToPlane(fromX, fromY, toX, toY))
                throw new Exception("Edge parameters indicate that it is not parallel to plane!");

            ID = id;
            From = new Point2D(fromX, fromY);
            To = new Point2D(toX, toY);
        }

        private bool IsEdgeParallelToPlane(int fromX, int fromY, int toX, int toY)
        {
            if (fromX == toX && fromY != toY)
            {
                MovementAxis = MovementAxis.Y;

                if (fromY < toY)
                {
                    Direction = CardinalDirection.North;
                }
                else
                {
                    Direction = CardinalDirection.South;
                }

                return true;
            }


            if (fromX != toX && fromY == toY)
            {
                MovementAxis = MovementAxis.X;

                if (fromX < toX)
                {
                    Direction = CardinalDirection.East;
                }
                else
                {
                    Direction = CardinalDirection.West;
                }

                return true;
            }               

            return false;
        }
    }
}
