using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_traffic_simulation.objects
{
    public class EdgePipe
    {
        public int ID { get; set; }
        public List<EdgeRoad> Edges { get; set; }

        public EdgePipe(int id)
        {
            ID = id;
            Edges = new List<EdgeRoad>();
        }

        public EdgePipe(int id, List<EdgeRoad> edges)
        {
            ID = id;
            Edges = edges;
        }

        public void AddEdgeRoad(int edgeRoadID, int width, int fromX, int fromY, int toX, int toY)
        {
            Edges.Add(new EdgeRoad(edgeRoadID, ID, width, fromX, fromY, toX, toY));
        }
    }
}
