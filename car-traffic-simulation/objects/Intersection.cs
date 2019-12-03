using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_traffic_simulation.objects
{
    public class Intersection
    {
        public int ID;
        public List<EdgeRoad> roads;
        public bool IsBusy { get; set; } = false;

        public Intersection(int id)
        {
            roads = new List<EdgeRoad>();
            ID = id;
        }

        public void AddPipe(EdgeRoad pipe)
        {
            roads.Add(pipe);
        }
    }
}
