using car_traffic_simulation.objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Action = car_traffic_simulation.objects.Action;

namespace car_traffic_simulation.spawners
{
    public class VehicleInfo
    {
        public String ImgUrl { get; set; }
        public Rotation Rotation { get; set; }

        public VehicleInfo(String imgUrl, Rotation rotation)
        {
            ImgUrl = imgUrl;
            Rotation = rotation;
        }
    };

    public class VehicleRepository
    {
        public int CurrentVehicleInfoIndex { get; set; } = 0;
        public List<Vehicle> Vehicles { get; set; }

        private List<VehicleInfo> vehicleInfos;

        public VehicleRepository()
        {
            Vehicles = new List<Vehicle>();
        }

        private BitmapImage PrepareCarImage(VehicleInfo vehicleInfoIndex, int width, int height)
        {
            BitmapImage bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();

            bitmapImage.UriSource = new Uri("..\\..\\" + @vehicleInfoIndex.ImgUrl, UriKind.Relative);
            bitmapImage.Rotation = vehicleInfoIndex.Rotation;
            Console.WriteLine(bitmapImage.DecodePixelHeight);
            Console.WriteLine(bitmapImage.DecodePixelWidth);
            bitmapImage.DecodePixelHeight = height;
            bitmapImage.DecodePixelWidth = width;

            bitmapImage.EndInit();

            return bitmapImage;
        }

        public void GenerateCar(VehicleInfo vehicleInfo, int x, int y, int velocity, int height, int width, Action action)
        {
            var vehicle = new Vehicle(x, y, velocity, height, width);

            vehicle.decideAction(action);

            vehicle.image.Source = PrepareCarImage(vehicleInfo, height, width);
            vehicle.image.Height = height;
            vehicle.image.Width = width;

            Vehicles.Add(vehicle);
        }

        public void LoadExampleVehicleSet()
        {
            List<VehicleInfo> vehicleInfos = new List<VehicleInfo>
            {
                new VehicleInfo("assets/blue-car.png", Rotation.Rotate270),
                new VehicleInfo("assets/green-car.png", Rotation.Rotate270),
                new VehicleInfo("assets/green-car.png", Rotation.Rotate270),
                new VehicleInfo("assets/green-car.png", Rotation.Rotate90),
                new VehicleInfo("assets/blue-car.png", Rotation.Rotate90)
            };

            GenerateCar(vehicleInfos[0], -40, 295, 2, 35, 70, Action.MoveForward);
            GenerateCar(vehicleInfos[1], -240, 295, 3, 35, 70, Action.MoveForward);
            GenerateCar(vehicleInfos[2], -1200, 295, 5, 35, 70, Action.MoveForward);

            GenerateCar(vehicleInfos[3], 720, 180, 2, 35, 70, Action.MoveBackward);
            GenerateCar(vehicleInfos[4], 1440, 180, 3, 35, 70, Action.MoveBackward);
        }
    }
}
