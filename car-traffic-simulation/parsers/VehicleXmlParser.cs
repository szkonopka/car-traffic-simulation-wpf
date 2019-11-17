using car_traffic_simulation.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace car_traffic_simulation.parsers
{
    public class VehicleXmlParser
    {
        public void Load(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            /*
            var q = from vehicle in doc.Descendants("vehicle")
                    select new Vehicle
                    {
                        ID = Int32.Parse(vehicle.Element("id").Value),
                        X = Int32.Parse(vehicle.Element("initX").Value),
                        Y = Int32.Parse(vehicle.Element("initY").Value),
                        Velocity = Int32.Parse(vehicle.Element("velocity").Value),
                        Width = Int32.Parse(vehicle.Element("width").Value),
                        Height = Int32.Parse(vehicle.Element("height").Value),
                        TexturePath = vehicle.Element("texturePath").Value.ToString(),
                       
                    };

            */
        }
    }
}
