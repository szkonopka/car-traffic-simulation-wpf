using car_traffic_simulation.engines;
using car_traffic_simulation.models;
using car_traffic_simulation.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace car_traffic_simulation
{
    /// <summary>
    /// Logika interakcji dla klasy DataWindow.xaml
    /// </summary>
    public partial class DataWindow : Window
    {
        public DataWindow()
        {
            InitializeComponent();
        }

        private List<EdgeRoadData> loadEdgeRoadsData(List<EdgePipe> edgePipes, List<Vehicle> vehicles)
        {
            List<EdgeRoadData> edgeRoadsData = new List<EdgeRoadData>();

            var edgeRoads = edgePipes.SelectMany(ep => ep.Edges).ToList();

            foreach (var edgeRoad in edgeRoads)
            {

                edgeRoadsData.Add(new EdgeRoadData
                {
                    ID = edgeRoad.ID,
                    Cars = vehicles
                        .Where(v => v.CurrentEdge.ID == edgeRoad.ID)
                        .Count(),
                    CarsIdListStr = listToString(vehicles
                        .Where(v => v.CurrentEdge.ID == edgeRoad.ID)
                        .Select(v => v.ID)
                        .ToList())
                });
            }

            return edgeRoadsData;
        }

        private List<VehicleData> loadVehiclesData(List<Vehicle> vehicles)
        {
            List<VehicleData> vehiclesData = new List<VehicleData>();
            foreach (var vehicle in vehicles)
            {
                vehiclesData.Add(new VehicleData
                {
                    ID = vehicle.ID,
                    X = vehicle.Position.X,
                    Y = vehicle.Position.Y,
                    State = vehicle.StateToStr(vehicle.State),
                    Heading = vehicle.CardinalDirectionToStr(vehicle.Turn)
                });
            }

            return vehiclesData;
        }

        private List<IntersectionData> loadIntersectionsData(List<Intersection> intersections, List<Vehicle> vehicles)
        {
            List<IntersectionData> intersectionsData = new List<IntersectionData>();
            foreach (var intersection in intersections)
            {
                intersectionsData.Add(new IntersectionData
                {
                    ID = intersection.ID,
                    CurrentCar = intersection.CurrentVehicle == null ? -1 : intersection.CurrentVehicle.ID,
                    AwaitingCars = vehicles
                        .Where(v => v.CurrentIntersectionID == intersection.ID)
                        .Count(),
                    AwaitingCarsIdListStr = listToString(vehicles
                        .Where(v => v.CurrentIntersectionID == intersection.ID)
                        .Select(v => v.ID)
                        .ToList())
                });
            }
            
            return intersectionsData;
        }

        public void reloadData(SimulationState state)
        {
            var context = TaskScheduler.FromCurrentSynchronizationContext();
            var token = Task.Factory.CancellationToken;
            var loadingData = Task.Factory.StartNew(() =>
            {
                vehicleDataGrid.ItemsSource = loadVehiclesData(state.vehicles);
                intersectionDataGrid.ItemsSource = loadIntersectionsData(state.intersections, state.vehicles);
                roadDataGrid.ItemsSource = loadEdgeRoadsData(state.edgePipes, state.vehicles);
            }, token, TaskCreationOptions.RunContinuationsAsynchronously, context);
        }

        private static String listToString<T>(List<T> list) => list.Aggregate("", (i, j) => j.ToString() + ", " + i.ToString());
    }
}
