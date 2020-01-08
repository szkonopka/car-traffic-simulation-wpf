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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using car_traffic_simulation.engines;
using SimulationState = car_traffic_simulation.engines.SimulationState;
using car_traffic_simulation.spawners;
using car_traffic_simulation.repositories;

namespace car_traffic_simulation
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SimulationState state;
        private SimulationCoordinator coordinator;
        private DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer dataGridTimer = new DispatcherTimer();
        private Dictionary<int, Button> vehicleLabelButtons = new Dictionary<int, Button>();
        private DataWindow dataWindow;
        private readonly int LABEL_BTN_SIZE = 20;
        public MainWindow()
        {   
            InitializeComponent();

            dataWindow = new DataWindow();
            dataWindow.Show();

            state = SimulationState.GetState();
            state.LoadExamplestate();

            coordinator = new SimulationCoordinator(state);

            GenerateRoads();
            GenerateVehicles();
            
            coordinator.Start();

            timer.Tick += Render;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Start();

            dataGridTimer.Tick += ReloadDataGrid;
            dataGridTimer.Interval = new TimeSpan(0, 0, 0, 1);
            dataGridTimer.Start();
        }

        private void drawOnCanvas(int topOffset, int leftOffset, UIElement controlObject)
        {
            Canvas.SetTop(controlObject, topOffset);
            Canvas.SetLeft(controlObject, leftOffset);
        }

        public void Render(object sender, EventArgs e)
        {
            foreach (var vehicle in state.vehicles)
                drawOnCanvas(vehicle.Position.Y - 10, vehicle.Position.X - 10, vehicleLabelButtons[vehicle.ID]);
        }

        public void ReloadDataGrid(object sender, EventArgs e) => dataWindow.reloadData(state);

        public void GenerateRoads()
        {
            foreach(var road in state.roadTextures)
            {
                drawOnCanvas(road.Position.X, road.Position.Y, road.image);
                Roads.Children.Add(road.image);
            }
        }

        public void GenerateVehicles()
        {
            foreach (var vehicle in state.vehicles)
            {
                var button = new Button
                {
                    Height = LABEL_BTN_SIZE,
                    Width = LABEL_BTN_SIZE,
                    Content = vehicle.ID.ToString().Length == 1 ? "0" + vehicle.ID.ToString() : vehicle.ID.ToString(),
                    FontWeight = FontWeights.Bold
                };
                
                vehicleLabelButtons.Add(key: vehicle.ID, value: button);

                drawOnCanvas(vehicle.Position.Y - LABEL_BTN_SIZE / 2, vehicle.Position.X, vehicle.image);
                drawOnCanvas(vehicle.Position.Y - LABEL_BTN_SIZE / 2, vehicle.Position.X, vehicleLabelButtons[vehicle.ID]);

                Vehicles.Children.Add(vehicle.image);
                Vehicles.Children.Add(button);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e) => coordinator.Start();

        private void StopButton_Click(object sender, RoutedEventArgs e) => coordinator.Stop();

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            coordinator.Stop();

            state.vehicles.Clear();
            state.intersections.Clear();
            vehicleLabelButtons.Clear();
            Vehicles.Children.Clear();

            state.vehicles = VehicleRepository
                .InitializeVehicleRepository()
                .GetAllFromFile("../../data/Vehicles.xml", state.edgePipes);

            state.intersections = IntersectionRepository
                .InitializeIntersectionRepository()
                .GetAllFromFile("../../data/Intersections.xml", state.edgePipes);

            GenerateVehicles();

            coordinator.Start();
        }
    }
}
