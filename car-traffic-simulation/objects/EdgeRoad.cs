﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_traffic_simulation.objects
{
    public class EdgeRoad
    {
        public static readonly Dictionary<CardinalDirection, CardinalDirection> OppositeDirections =
            new Dictionary<CardinalDirection, CardinalDirection>
            {
                    { CardinalDirection.North, CardinalDirection.South },
                    { CardinalDirection.South, CardinalDirection.North },
                    { CardinalDirection.East, CardinalDirection.West },
                    { CardinalDirection.West, CardinalDirection.East }
            };

        public int ID { get; set; }
        public int PipeID { get; set; }
        public Point2D From { get; set; }
        public Point2D To { get; set; }
        public CardinalDirection Direction { get; set; }
        public int Width { get; set; }

        public EdgeRoad(int id, int pipeID, int width, int fromX, int fromY, int toX, int toY)
        {
            if (!IsEdgeParallelToPlane(fromX, fromY, toX, toY))
                throw new Exception("Edge parameters indicate that it is not parallel to plane!");

            ID = id;
            PipeID = pipeID;
            From = new Point2D(fromX, fromY);
            To = new Point2D(toX, toY);
        }

        private bool IsEdgeParallelToPlane(int fromX, int fromY, int toX, int toY)
        {
            if (fromX == toX && fromY != toY)
            {
                Direction = fromY < toY ? CardinalDirection.South : CardinalDirection.North;
                return true;
            }

            if (fromX != toX && fromY == toY)
            {
                Direction = fromX < toX ? CardinalDirection.East : CardinalDirection.West;
                return true;
            }               

            return false;
        }
    }
}
