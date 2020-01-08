﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_traffic_simulation.models
{
    struct IntersectionData
    {
        public int ID { get; set; }
        public int? CurrentCar { get; set; }
        public int? AwaitingCars { get; set; }
        public String AwaitingCarsIdListStr { get; set; }
    }
}
