using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_traffic_simulation.models
{
    class IntersectionData
    {
        public int ID { get; set; }
        public int? CurrentCar { get; set; }
        public int? CarsAwaiting { get; set; }
        public IntersectionData(int id, int? currentCar, int? carsAwaiting)
        {
            ID = id;
            CurrentCar = currentCar;
            CarsAwaiting = carsAwaiting;
        }
    }
}
