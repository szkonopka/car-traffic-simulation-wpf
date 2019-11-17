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
        private readonly string ROTATION90 = "90", ROTATION270 = "270";

        public String ImgUrl { get; set; }
        public Rotation Rotation { get; set; }

        public VehicleInfo(String imgUrl, string rotation)
        {
            ImgUrl = imgUrl;
            Console.WriteLine(rotation);
            if (rotation.Equals(ROTATION90))
                Rotation = Rotation.Rotate90;
            else if (rotation.Equals(ROTATION270))
                Rotation = Rotation.Rotate270;
            else
                Rotation = Rotation.Rotate0;
        }
    };

    public class VehicleRepository
    {
        private readonly string VEHICLES_ELEM = "vehicle";
        private readonly string ID_ELEM = "id";
        private readonly string VELOCITY_ELEM = "velocity";
        private readonly string WIDTH_ELEM = "width";
        private readonly string HEIGHT_ELEM = "height";
        private readonly string OFFSET_X_ELEM = "offsetX";
        private readonly string OFFSET_Y_ELEM = "offsetY";
        private readonly string TEXTURE_ELEM = "texturePath";
        private readonly string EDGE_PIPE_ID_ELEM = "edgePipeId";
        private readonly string EDGE_ROAD_ID_ELEM = "edgeRoadId";
        private readonly string ROTATION_ELEM = "rotation";

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

        public void LoadFromXml(string filePath, List<EdgePipe> edgePipes)
        {
            XDocument doc = XDocument.Load(filePath);

            var vehiclesNode = from v in doc.Descendants(VEHICLES_ELEM) select v;

            foreach (XElement vehicleNode in vehiclesNode)
            {
                int id = Int32.Parse(vehicleNode.Element(ID_ELEM).Value);
                int velocity = Int32.Parse(vehicleNode.Element(VELOCITY_ELEM).Value);
                int width = Int32.Parse(vehicleNode.Element(WIDTH_ELEM).Value);
                int height = Int32.Parse(vehicleNode.Element(HEIGHT_ELEM).Value);
                int offsetX = Int32.Parse(vehicleNode.Element(OFFSET_X_ELEM).Value);
                int offsetY = Int32.Parse(vehicleNode.Element(OFFSET_Y_ELEM).Value);
                int edgePipeId = Int32.Parse(vehicleNode.Element(EDGE_PIPE_ID_ELEM).Value);
                int edgeRoadId = Int32.Parse(vehicleNode.Element(EDGE_ROAD_ID_ELEM).Value);
                string texturePath = vehicleNode.Element(TEXTURE_ELEM).Value.ToString();
                string rotation = vehicleNode.Element(ROTATION_ELEM).Value.ToString();

                GenerateCar(new VehicleInfo(texturePath, rotation), offsetX, offsetY, velocity, height, width, edgePipes[edgePipeId].Edges[edgeRoadId]);
            }
        }
        
        /*
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

            GenerateCar(vehicleInfos[0], 0, 0, 3, 35, 70, edgePipes[1].Edges[1]);
            GenerateCar(vehicleInfos[1], 140, 0, 2, 35, 70, edgePipes[1].Edges[1]);
            GenerateCar(vehicleInfos[2], 280, 0, 1, 35, 70, edgePipes[1].Edges[1]);
            GenerateCar(vehicleInfos[3], 0, 0, 1, 35, 70, edgePipes[1].Edges[0]);

            GenerateCar(vehicleInfos[4], 0, 0, 2, 35, 70, edgePipes[0].Edges[0]);
            GenerateCar(vehicleInfos[5], 0, 0, 3, 35, 70, edgePipes[0].Edges[1]);
            GenerateCar(vehicleInfos[6], -140, 0, 2, 35, 70, edgePipes[0].Edges[1]);
            GenerateCar(vehicleInfos[7], -320, 0, 1, 35, 70, edgePipes[0].Edges[1]);
        }
        */
    }
}
