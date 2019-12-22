using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_traffic_simulation.models
{
    class VehicleData
    {
        public int ID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public VehicleData(int id, int x, int y)
        {
            ID = id;
            X = x;
            Y = x;
        }
    }
}
