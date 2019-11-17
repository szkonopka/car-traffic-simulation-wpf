using car_traffic_simulation.objects;
using car_traffic_simulation.parsers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
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
        VehicleXmlParser parser;
        public int CurrentVehicleIndex { get; set; } = 0;
        public List<Vehicle> Vehicles { get; set; }

        private List<VehicleInfo> vehicleInfos;

        public VehicleRepository()
        {
            parser = new VehicleXmlParser();
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

        public void GenerateCar(VehicleInfo vehicleInfo, int x, int y, int velocity, int height, int width, EdgeRoad edgeRoad)
        {
            var vehicle = new Vehicle(CurrentVehicleIndex, x, y, velocity, height, width, edgeRoad);
            CurrentVehicleIndex++;

            vehicle.decideAction();

            vehicle.image.Source = PrepareCarImage(vehicleInfo, height, width);
            vehicle.image.Height = height;
            vehicle.image.Width = width;

            Vehicles.Add(vehicle);
        }

        public void LoadVehiclesFromXml(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            var q = from b in doc.Descendants("vehicle")
                    select new
                    {
                        id = b.Element("name").Value,
                        type = b.Element("type").Value,
                        velocity = b.Element("velocity").Value,
                        width = b.Element("width").Value,
                        height = b.Element("height").Value,
                        initX = b.Element("initX").Value,
                        initY = b.Element("initY").Value,
                        texturePath = b.Element("texturePath").Value,
                        fromDirection = b.Element("directions").Attribute("from").Value,
                        toDirection = b.Element("directions").Attribute("to").Value,
                    };
        }

        public void LoadExampleVehicleSet(List<EdgePipe> edgePipes)
        {
            parser.Load(@"../../data/Vehicles.xml");

            List<VehicleInfo> vehicleInfos = new List<VehicleInfo>
            {
                new VehicleInfo("assets/blue-car.png", Rotation.Rotate270),
                new VehicleInfo("assets/green-car.png", Rotation.Rotate270),
                new VehicleInfo("assets/green-car.png", Rotation.Rotate270),
                new VehicleInfo("assets/blue-car.png", Rotation.Rotate270),
                new VehicleInfo("assets/green-car.png", Rotation.Rotate90),
                new VehicleInfo("assets/blue-car.png", Rotation.Rotate90),
                new VehicleInfo("assets/green-car.png", Rotation.Rotate90),
                new VehicleInfo("assets/blue-car.png", Rotation.Rotate90)
            };

            GenerateCar(vehicleInfos[0], 0, 325, 3, 35, 70, edgePipes[1].Edges[1]);
            GenerateCar(vehicleInfos[1], 140, 325, 2, 35, 70, edgePipes[1].Edges[1]);
            GenerateCar(vehicleInfos[2], 280, 325, 1, 35, 70, edgePipes[1].Edges[1]);
            GenerateCar(vehicleInfos[3], 0, 270, 1, 35, 70, edgePipes[1].Edges[0]);

            GenerateCar(vehicleInfos[4], 0, 155, 2, 35, 70, edgePipes[0].Edges[0]);
            GenerateCar(vehicleInfos[5], 0, 155, 3, 35, 70, edgePipes[0].Edges[1]);
            GenerateCar(vehicleInfos[6], -140, 155, 2, 35, 70, edgePipes[0].Edges[1]);
            GenerateCar(vehicleInfos[7], -320, 155, 1, 35, 70, edgePipes[0].Edges[1]);
        }
    }
}
