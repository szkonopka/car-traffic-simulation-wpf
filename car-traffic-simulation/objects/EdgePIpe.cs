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

        public EdgePipe(int id, List<EdgeRoad> edges)
        {
            ID = id;
            Edges = edges;
        }
    }
}
