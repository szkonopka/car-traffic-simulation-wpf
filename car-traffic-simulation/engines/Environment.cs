using car_traffic_simulation.objects;
using car_traffic_simulation.parsers;
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

            GraphXmlParser graphParser = new GraphXmlParser();

            edgePipes = graphParser.LoadAndGet("../../data/Roads.xml");
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
