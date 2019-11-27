using car_traffic_simulation.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;

namespace car_traffic_simulation.spawners
{
    public class RoadRepository
    {
        public List<Road> Roads { get; set; }

        public RoadRepository()
        {
            Roads = new List<Road>();
        }

        public void LoadRoadsFromXml(string filePath)
        {
            XmlDocument xml = new XmlDocument();

            xml.Load(filePath);

            foreach (XmlNode node in xml.DocumentElement)
            {

            }
        }

        public void LoadExampleRoadMap()
        {
            //var road = new Road("", 1366, 241, 0, 138);
            var road = new Road("", 1866, 941, 0, 0);

            road.image.Source = new BitmapImage(new Uri("..\\..\\" + @"assets/straight-road.png", UriKind.Relative));

            Roads.Add(road);
        }
    }
}
