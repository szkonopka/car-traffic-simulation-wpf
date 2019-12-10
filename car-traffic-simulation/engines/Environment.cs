using car_traffic_simulation.objects;
using car_traffic_simulation.parsers;
using car_traffic_simulation.repositories;
using car_traffic_simulation.spawners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace car_traffic_simulation.engines
{
    public class Environment
    {
        public RoadRepository roadRepository;
        public VehicleRepository vehicleRepository;
        public IntersectionRepository intersectionRepository;
        public List<EdgeRoad> edges { get; set; }
        public List<EdgePipe> edgePipes { get; set; }
        public List<Intersection> intersections { get; set; }

        public Environment()
        {
            roadRepository = new RoadRepository();
            vehicleRepository = new VehicleRepository();
            intersectionRepository = new IntersectionRepository();

            GraphXmlParser graphParser = new GraphXmlParser();

            edgePipes = graphParser.LoadAndGet("../../data/Roads.xml");
        }

        public void LoadExampleEnvironment()
        {
            roadRepository.LoadExampleRoadMap();
            vehicleRepository.LoadFromXml("../../data/Vehicles.xml", edgePipes);
            intersections = intersectionRepository.LoadAndGet("../../data/Intersections.xml", edgePipes);
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
