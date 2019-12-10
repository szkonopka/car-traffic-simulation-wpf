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
using Environment = car_traffic_simulation.engines.Environment;
using car_traffic_simulation.parsers;

namespace car_traffic_simulation
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            public static extern IntPtr GetConsoleWindow();

            [DllImport("user32.dll")]
            public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            public const int SW_HIDE = 0;
            public const int SW_SHOW = 5;
        }

        Environment environment;
        EnvironmentEngine engine;
        DispatcherTimer timer = new DispatcherTimer();
        Dictionary<int, Rectangle> rectangles = new Dictionary<int, Rectangle>();

        public MainWindow()
        {   
            var handle = NativeMethods.GetConsoleWindow();

            InitializeComponent();

            environment = new Environment();
            environment.LoadExampleEnvironment();

            engine = new EnvironmentEngine(environment);

            GenerateRoads();
            GenerateVehicles();

            engine.Start();

            timer.Tick += Render;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Start();
        }

        public void Render(object sender, EventArgs e)
        {
            foreach (var vehicle in environment.vehicleRepository.Vehicles)
            {
                Canvas.SetTop(rectangles[vehicle.ID], vehicle.Position.Y);
                Canvas.SetLeft(rectangles[vehicle.ID], vehicle.Position.X);
            }            
        }

        public void GenerateRoads()
        {
            foreach(var road in environment.roadRepository.Roads)
            {
                Canvas.SetTop(road.image, road.Position.Y);
                Canvas.SetLeft(road.image, road.Position.X);

                Roads.Children.Add(road.image);
            }
        }

        public void GenerateVehicles()
        {
            foreach (var vehicle in environment.vehicleRepository.Vehicles)
            {
                var myRect = new Rectangle
                {
                    Stroke = Brushes.White,
                    StrokeThickness = 2
                };
                myRect.Height = 5;
                myRect.Width = 5;

                rectangles.Add(key: vehicle.ID, value: myRect);

                Canvas.SetTop(vehicle.image, vehicle.Position.Y);
                Canvas.SetLeft(vehicle.image, vehicle.Position.X);

                Canvas.SetTop(rectangles[vehicle.ID], vehicle.Position.Y);
                Canvas.SetLeft(rectangles[vehicle.ID], vehicle.Position.X);

                Vehicles.Children.Add(vehicle.image);
                Vehicles.Children.Add(myRect);
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            engine.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            engine.Stop();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            engine.Stop();

            Vehicles.Children.Clear();
            environment.vehicleRepository.Vehicles.Clear();
            rectangles.Clear();

            environment.vehicleRepository.LoadFromXml("../../data/Vehicles.xml", environment.edgePipes);
            GenerateVehicles();

            engine.Start();
        }
    }
}
