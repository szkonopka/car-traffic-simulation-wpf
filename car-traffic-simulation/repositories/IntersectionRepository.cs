using car_traffic_simulation.objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace car_traffic_simulation.repositories
{
    public enum IntersectionPipeType
    {
        Out,
        In
    }

    public class IntersectionRepository
    {
        private readonly string INTERSECTIONS_ELEM = "intersection";
        private readonly string PIPES_ELEM = "pipe";
        private readonly string ID_ELEM = "id";
        private readonly string TYPE_ELEM = "type";

        private List<Intersection> intersectionList;

        public IntersectionRepository()
        {
            intersectionList = new List<Intersection>();
        }

        private void Load(string filePath, List<EdgePipe> edgePipes)
        {
            XDocument doc = XDocument.Load(filePath);

            var intersectionsNode = from i in doc.Descendants(INTERSECTIONS_ELEM) select i;

            foreach (XElement intersectionNode in intersectionsNode)
            {
                int intersectionId = Int32.Parse(intersectionNode.Element(ID_ELEM).Value);

                var pipesNode = from p in intersectionNode.Descendants(PIPES_ELEM) select p;

                AddIntersection(intersectionId);

                Console.WriteLine(pipesNode.ToString());

                foreach (XElement pipeNode in pipesNode)
                {
                    Console.WriteLine(pipeNode.ToString());
                    int pipeId = Int32.Parse(pipeNode.Element(ID_ELEM).Value);
                    string type = pipeNode.Element(TYPE_ELEM).Value.ToString();
                    var roads = (from ep in edgePipes select ep.Edges).SelectMany(e => e);
                    AddPipeToIntersection(intersectionId, roads.FirstOrDefault(r => r.ID == pipeId));
                }
            }
        }

        private void AddPipeToIntersection(int intersectionId, EdgeRoad road)
        {
            intersectionList[intersectionId].AddPipe(road);
        }

        private void AddIntersection(int intersectionId)
        {
            intersectionList.Add(new Intersection(intersectionId));
        }

        public List<Intersection> LoadAndGet(string filePath, List<EdgePipe> edgePipes, bool clearState = false)
        {
            if (clearState)
                intersectionList.Clear();

            Load(filePath, edgePipes);

            return intersectionList;
        }
    }
}
