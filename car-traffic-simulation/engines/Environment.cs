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
        public List<EdgePipe> edgePipes { get; set; }

        public Environment()
        {
            roadRepository = new RoadRepository();
            vehicleRepository = new VehicleRepository();

            edges = new List<EdgeRoad>();

            edgePipes = new List<EdgePipe>();

            edgePipes.Add(new EdgePipe(
                0,
                new List<EdgeRoad>
                {
                    new EdgeRoad(0, 0, 1366, 155, 0, 155),
                    new EdgeRoad(1, 0, 1366, 205, 0, 205)
                }
             ));

            edgePipes.Add(new EdgePipe(
                1,
                new List<EdgeRoad>
                {
                    new EdgeRoad(2, 1, 0, 270, 1366, 270),
                    new EdgeRoad(3, 1, 0, 325, 1366, 325)
                }
             ));
        }

        public void LoadExampleEnvironment()
        {
            roadRepository.LoadExampleRoadMap();
            vehicleRepository.LoadExampleVehicleSet(edgePipes);
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
