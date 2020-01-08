using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_traffic_simulation.models
{
    struct VehicleData
    {
        public int ID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public String State { get; set; }
        public String Heading { get; set; }
    }
}
