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
        public List<EdgeRoad> edges { get; set; }

        public Environment()
        {
            roadRepository = new RoadRepository();
            vehicleRepository = new VehicleRepository();

            edges = new List<EdgeRoad>();

            edges.Add(new EdgeRoad(0, 1366, 155, 0, 155));
            edges.Add(new EdgeRoad(1, 1366, 205, 0, 205));
            edges.Add(new EdgeRoad(2, 0, 270, 1366, 270));
            edges.Add(new EdgeRoad(3, 0, 325, 1366, 325));
        }

        public void LoadExampleEnvironment()
        {
            roadRepository.LoadExampleRoadMap();
            vehicleRepository.LoadExampleVehicleSet(edges);
        }

        public void Redraw()
        {
            foreach (var road in roadRepository.Roads)
            {
                road.RenderBitmap();
            }

            foreach (var vehicle in vehicleRepository.Vehicles)
            {
                vehicle.Draw();
            }
        }
    }
}
