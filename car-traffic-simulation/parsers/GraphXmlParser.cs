using car_traffic_simulation.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace car_traffic_simulation.parsers
{
    public class GraphXmlParser
    {
        private List<EdgePipe> edgePipeList;

        private readonly string ID_ELEM = "id";
        private readonly string EDGE_PIPE_ELEM = "edgePipe";
        private readonly string EDGE_ROAD_ELEM = "edgeRoad";
        private readonly string FROM_X_ELEM = "fromX";
        private readonly string FROM_Y_ELEM = "fromY";
        private readonly string TO_X_ELEM = "toX";
        private readonly string TO_Y_ELEM = "toY";

        public GraphXmlParser()
        {
            edgePipeList = new List<EdgePipe>();
        }

        private void Load(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            var edgePipesNode = from eP in doc.Descendants(EDGE_PIPE_ELEM) select eP;

            foreach (XElement edgePipeNode in edgePipesNode)
            {
                int edgePipeId = Int32.Parse(edgePipeNode.Element(ID_ELEM).Value);

                var edgeRoadsNode = from eR in edgePipeNode.Descendants(EDGE_ROAD_ELEM) select eR;

                AddEdgePipe(edgePipeId);

                foreach (XElement edgeRoadNode in edgeRoadsNode)
                {
                    int edgeRoadId = Int32.Parse(edgeRoadNode.Element(ID_ELEM).Value);

                    AddEdgeRoad(edgePipeId, edgeRoadId, Int32.Parse(edgeRoadNode.Element(FROM_X_ELEM).Value),
                        Int32.Parse(edgeRoadNode.Element(FROM_Y_ELEM).Value), Int32.Parse(edgeRoadNode.Element(TO_X_ELEM).Value), Int32.Parse(edgeRoadNode.Element(TO_Y_ELEM).Value));
                }
            }
        }

        private void AddEdgeRoad(int edgePipeID, int edgeRoadID, int fromX, int fromY, int toX, int toY)
        {
            if (edgePipeList.ElementAt(edgePipeID) == null)
                return;

            edgePipeList[edgePipeID].AddEdgeRoad(edgeRoadID, fromX, fromY, toX, toY);
        }

        private void AddEdgePipe(int id)
        {
            edgePipeList.Add(new EdgePipe(id));
        }

        public List<EdgePipe> LoadAndGet(string filePath, bool clearState = false)
        {
            if (clearState)
                edgePipeList.Clear();

            Load(filePath);

            return edgePipeList;
        }
    }
}
