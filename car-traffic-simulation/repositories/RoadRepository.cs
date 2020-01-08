using car_traffic_simulation.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace car_traffic_simulation.spawners
{
    using EdgePipeList = List<EdgePipe>;
    using RoadTextureList = List<RoadTexture>;

    public class RoadRepository
    {
        private readonly string ID_ELEM = "id";
        private readonly string EDGE_PIPE_ELEM = "edgePipe";
        private readonly string EDGE_ROAD_ELEM = "edgeRoad";
        private readonly string FROM_X_ELEM = "fromX";
        private readonly string FROM_Y_ELEM = "fromY";
        private readonly string TO_X_ELEM = "toX";
        private readonly string TO_Y_ELEM = "toY";
        private readonly string WIDTH_ELEM = "width";

        public static RoadRepository InitializeRoadRepository() => new RoadRepository();

        public RoadRepository() { }

        public RoadTextureList GetOneRoadTextureMap(string texturePath)
        {
            RoadTextureList roadTextures = new RoadTextureList();

            var road = new RoadTexture("", 1866, 941, 0, 0);

            road.image.Source = new BitmapImage(new Uri("..\\..\\" + @texturePath, UriKind.Relative));

            roadTextures.Add(road);

            return roadTextures;
        }

        private void LoadAllFromXmlFileToList(string filePath, ref EdgePipeList edgePipes)
        {
            XDocument doc = XDocument.Load(filePath);

            var edgePipesNode = from eP in doc.Descendants(EDGE_PIPE_ELEM) select eP;

            foreach (XElement edgePipeNode in edgePipesNode)
            {
                int edgePipeId = Int32.Parse(edgePipeNode.Element(ID_ELEM).Value);

                var edgeRoadsNode = from eR in edgePipeNode.Descendants(EDGE_ROAD_ELEM) select eR;

                AddEdgePipe(edgePipeId, ref edgePipes);

                foreach (XElement edgeRoadNode in edgeRoadsNode)
                {
                    int edgeRoadId = Int32.Parse(edgeRoadNode.Element(ID_ELEM).Value);

                    AddEdgeRoad(edgePipeId, edgeRoadId,
                        Int32.Parse(edgeRoadNode.Element(WIDTH_ELEM).Value), Int32.Parse(edgeRoadNode.Element(FROM_X_ELEM).Value),
                        Int32.Parse(edgeRoadNode.Element(FROM_Y_ELEM).Value), Int32.Parse(edgeRoadNode.Element(TO_X_ELEM).Value), 
                        Int32.Parse(edgeRoadNode.Element(TO_Y_ELEM).Value), ref edgePipes);
                }
            }
        }

        private void AddEdgeRoad(int edgePipeID, int edgeRoadID, int width, int fromX, int fromY, int toX, int toY, ref EdgePipeList edgePipes)
        {
            if (edgePipes.ElementAt(edgePipeID) == null)
                return;

            var ers = edgePipes.SelectMany(ep => ep.Edges);
            var fer = ers.FirstOrDefault(er => (er.From.X == fromX && er.To.X == toX) && (er.From.Y == fromY && er.To.Y == toY));

            if (ers.Any(er => (er.From.X == fromX && er.To.X == toX) && (er.From.Y == fromY && er.To.Y == toY)))
            {
                Console.WriteLine("New ID: " + edgeRoadID + " fromX: " + fromX + " toX: " + toX + " fromY: " + fromY + " toY: " + toY);
                throw new Exception("Edge already exists ID: " + fer.ID + " fromX: " + fer.From.X + " toX: " + fer.To.X + " fromY: " + fer.From.Y + " toY: " + fer.To.Y);
            }
                

            edgePipes[edgePipeID].AddEdgeRoad(edgeRoadID, width, fromX, fromY, toX, toY);
        }

        private void AddEdgePipe(int id, ref EdgePipeList edgePipes)
        {
            edgePipes.Add(new EdgePipe(id));
        }

        public EdgePipeList GetAllFromFile(string filePath)
        {
            EdgePipeList edgePipes = new EdgePipeList();

            LoadAllFromXmlFileToList(filePath, ref edgePipes);

            return edgePipes;
        }
    }

}
