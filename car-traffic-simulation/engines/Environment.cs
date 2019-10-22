using car_traffic_simulation.objects;
using car_traffic_simulation.spawners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_traffic_simulation.engines
{
    public class Environment
    {
        public RoadRepository roadRepository;
        public VehicleRepository vehicleRepository;

        public Environment()
        {
            roadRepository = new RoadRepository();
            vehicleRepository = new VehicleRepository();
        }

        public void LoadExampleEnvironment()
        {
            roadRepository.LoadExampleRoadMap();
            vehicleRepository.LoadExampleVehicleSet();
        }

        public void Redraw()
        {
            foreach (var road in roadRepository.Roads)
            {
                road.Draw();
            }

            foreach (var vehicle in vehicleRepository.Vehicles)
            {
                vehicle.Draw();
            }
        }
    }
}
