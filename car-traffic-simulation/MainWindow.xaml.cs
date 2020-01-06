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
        private SimulationCoordinator engine;
        private DispatcherTimer timer = new DispatcherTimer();
        private DispatcherTimer dataGridTimer = new DispatcherTimer();
        private Dictionary<int, Rectangle> rectangles = new Dictionary<int, Rectangle>();
        private Dictionary<int, Button> buttons = new Dictionary<int, Button>();
        private DataWindow dataWindow;

        public MainWindow()
        {   
            InitializeComponent();

            dataWindow = new DataWindow();
            dataWindow.Show();

            state = SimulationState.GetState();
            state.LoadExamplestate();

            engine = new SimulationCoordinator(state);

            GenerateRoads();
            GenerateVehicles();

            engine.Start();

            timer.Tick += Render;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Start();

            dataGridTimer.Tick += ReloadDataGrid;
            dataGridTimer.Interval = new TimeSpan(0, 0, 0, 2);
            dataGridTimer.Start();
        }

        public void Render(object sender, EventArgs e)
        {
            foreach (var vehicle in state.vehicles)
            {
                Canvas.SetTop(buttons[vehicle.ID], vehicle.Position.Y - 10);
                Canvas.SetLeft(buttons[vehicle.ID], vehicle.Position.X - 10);
            }
        }

        public void ReloadDataGrid(object sender, EventArgs e) => dataWindow.reloadData(engine);

        public void GenerateRoads()
        {
            foreach(var road in state.roadTextures)
            {
                Canvas.SetTop(road.image, road.Position.Y);
                Canvas.SetLeft(road.image, road.Position.X);

                Roads.Children.Add(road.image);
            }
        }

        public void GenerateVehicles()
        {
            foreach (var vehicle in state.vehicles)
            {
                var button = new Button();
                button.Height = 20;
                button.Width = 20;
                button.Content += vehicle.ID.ToString().Length == 1 ? "0" + vehicle.ID.ToString() : vehicle.ID.ToString();
                button.FontWeight = FontWeights.Bold;

                buttons.Add(key: vehicle.ID, value: button);

                Canvas.SetTop(vehicle.image, vehicle.Position.Y);
                Canvas.SetLeft(vehicle.image, vehicle.Position.X);

                Canvas.SetTop(buttons[vehicle.ID], vehicle.Position.Y - 10);
                Canvas.SetLeft(buttons[vehicle.ID], vehicle.Position.X - 10);

                Vehicles.Children.Add(vehicle.image);
                Vehicles.Children.Add(button);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e) => engine.Start();

        private void StopButton_Click(object sender, RoutedEventArgs e) => engine.Stop();

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            engine.Stop();

            Vehicles.Children.Clear();
            state.vehicles.Clear();
            rectangles.Clear();
            state.intersections.Clear();

            state.vehicles = VehicleRepository
                .InitializeVehicleRepository()
                .GetAllFromFile("../../data/Vehicles.xml", state.edgePipes);

            state.intersections = IntersectionRepository
                .InitializeIntersectionRepository()
                .GetAllFromFile("../../data/Intersections.xml", state.edgePipes);

            GenerateVehicles();

            engine.Start();
        }
    }
}
