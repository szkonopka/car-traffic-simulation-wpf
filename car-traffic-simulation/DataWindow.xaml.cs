using car_traffic_simulation.engines;
using car_traffic_simulation.models;
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

        public void reloadData(EnvironmentEngine engine)
        {
            List<VehicleData> vehicles = new List<VehicleData>();
            foreach (var vehicle in engine.state.vehicleRepository.Vehicles)
            {
                vehicles.Add(new VehicleData(vehicle.ID, vehicle.Position.X, vehicle.Position.Y));
            }

            vehicleDataGrid.ItemsSource = vehicles;

            List<IntersectionData> intersections = new List<IntersectionData>();

            foreach (var intersection in engine.state.intersections)
            {
                intersections.Add(
                    new IntersectionData(
                        intersection.ID,
                        intersection.CurrentVehicle == null ? -1 : intersection.CurrentVehicle.ID,
                        engine.state.vehicleRepository.Vehicles.Where(v => v.CurrentIntersectionID == intersection.ID).Count()));
            }

            intersectionDataGrid.ItemsSource = intersections;
        }
    }
}
