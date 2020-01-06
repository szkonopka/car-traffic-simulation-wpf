using car_traffic_simulation.objects;
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
    using IntersectionList = List<Intersection>;
    using EdgeRoadList = List<EdgeRoad>;
    using EdgePipeList = List<EdgePipe>;
    using VehicleList = List<Vehicle>;
    using RoadTextureList = List<RoadTexture>;
    
    public class SimulationState
    {
        public IntersectionList intersections;
        public EdgeRoadList edges { get; set; }
        public EdgePipeList edgePipes { get; set; }
        public VehicleList vehicles { get; set; }

        public RoadTextureList roadTextures { get; set; }

        public static SimulationState GetState() => new SimulationState();

        private SimulationState() { }

        public void LoadExamplestate()
        {
            roadTextures = RoadRepository
                .InitializeRoadRepository()
                .GetOneRoadTextureMap("assets/straight-road.png");

            edgePipes = RoadRepository
                .InitializeRoadRepository()
                .GetAllFromFile("../../data/Roads.xml");

            edgePipes = RoadRepository
                .InitializeRoadRepository()
                .GetAllFromFile("../../data/Roads.xml");

            vehicles = VehicleRepository
                .InitializeVehicleRepository()
                .GetAllFromFile("../../data/Vehicles.xml", edgePipes);

            intersections = IntersectionRepository
                .InitializeIntersectionRepository()
                .GetAllFromFile("../../data/Intersections.xml", edgePipes);
        }

        public void Redraw()
        {
            foreach (var road in roadTextures)
                road.draw();

            foreach (var vehicle in vehicles)
                vehicle.draw();
        }
    }
}
