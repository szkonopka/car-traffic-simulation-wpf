using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using car_traffic_simulation.objects;

namespace car_traffic_simulation.repositories
{
    using IntersectionList = List<Intersection>;
    using EdgePipeList = List<EdgePipe>;

    public class IntersectionRepository
    {
        private readonly string INTERSECTIONS_ELEM = "intersection";
        private readonly string PIPES_ELEM = "pipe";
        private readonly string ID_ELEM = "id";
        private readonly string TYPE_ELEM = "type";

        public static IntersectionRepository InitializeIntersectionRepository() => new IntersectionRepository();

        public IntersectionRepository() { }

        private IntersectionPipeType stringToIntersectionType(string type)
        {
            if (type == "in")
                return IntersectionPipeType.In;
            else
                return IntersectionPipeType.Out;
        }

        private void LoadAllFromXmlFileToList(string filePath, EdgePipeList edgePipes, ref IntersectionList intersectionList)
        {
            XDocument doc = XDocument.Load(filePath);

            var intersectionsNode = from i in doc.Descendants(INTERSECTIONS_ELEM) select i;

            foreach (XElement intersectionNode in intersectionsNode)
            {
                int intersectionId = Int32.Parse(intersectionNode.Element(ID_ELEM).Value);

                var pipesNode = from p in intersectionNode.Descendants(PIPES_ELEM) select p;

                intersectionList.Add(new Intersection(intersectionId));

                Console.WriteLine(pipesNode.ToString());

                foreach (XElement pipeNode in pipesNode)
                {
                    Console.WriteLine(pipeNode.ToString());
                    int pipeId = Int32.Parse(pipeNode.Element(ID_ELEM).Value);
                    string type = pipeNode.Element(TYPE_ELEM).Value.ToString();
                    var roads = (from ep in edgePipes select ep.Edges).SelectMany(e => e);

                    intersectionList[intersectionId].AddPipe(roads.FirstOrDefault(r => r.ID == pipeId), stringToIntersectionType(type));
                }
            }
        }

        public IntersectionList GetAllFromFile(string filePath, EdgePipeList edgePipes)
        {
            IntersectionList intersectionList = new IntersectionList();

            LoadAllFromXmlFileToList(filePath, edgePipes, ref intersectionList);

            return intersectionList;
        }
    }
}
