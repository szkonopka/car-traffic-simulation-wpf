using car_traffic_simulation.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace car_traffic_simulation.parsers
{
    public class VehicleXmlParser
    {
        private List<Vehicle> vehicleList;

        private readonly string VEHICLES_ELEM = "vehicles";
        private readonly string ID_ELEM = "id";
        private readonly string VELOCITY_ELEM = "velocity";
        private readonly string WIDTH_ELEM = "width";
        private readonly string HEIGHT_ELEM = "height";
        private readonly string OFFSET_X_ELEM = "offsetX";
        private readonly string OFFSET_Y_ELEM = "offsetY";
        private readonly string TEXTURE_ELEM = "texturePath";
        private readonly string EDGE_PIPE_ID_ELEM = "edgePipeId";
        private readonly string EDGE_ROAD_ID_ELEM = "edgeRoadId";
        private readonly string ROTATION_ELEM = "edgeRoadId";

        private readonly string RORATION90 = "90", ROTATION270 = "270";

        public void Load(string filePath)
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
            }
        }
    }
}
