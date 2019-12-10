using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_traffic_simulation.objects
{
    public enum IntersectionPipeType
    {
        Out,
        In
    }

    public class IntersectionPipe
    {
        public IntersectionPipe(EdgeRoad edgeRoad, IntersectionPipeType intersectionType)
        {
            EdgeRoad = edgeRoad;
            IntersectionType = intersectionType;
        }

        public EdgeRoad EdgeRoad { get; set; }
        public IntersectionPipeType IntersectionType { get; set; }
    };

    public class Intersection
    {
        public int ID;
        public List<IntersectionPipe> intersectionPipes;
        public bool IsBusy { get; set; } = false;
        public List<Vehicle> Queue { get; set; }
        public Vehicle CurrentVehicle { get; set; }
        public List<Point2D> Connectors { get; set; }
        public Intersection(int id)
        {
            Queue = new List<Vehicle>();
            Connectors = new List<Point2D>();
            intersectionPipes = new List<IntersectionPipe>();
            ID = id;
        }

        public void AddPipe(EdgeRoad pipe, IntersectionPipeType type)
        {
            intersectionPipes.Add(new IntersectionPipe(pipe, type));

            if (type == IntersectionPipeType.In)
                Connectors.Add(new Point2D(pipe.To.X, pipe.To.Y));
            else if (type == IntersectionPipeType.Out)
                Connectors.Add(new Point2D(pipe.From.X, pipe.From.Y));
        }

        public void act(car_traffic_simulation.engines.Environment environment)
        {
            foreach (var vehicle in environment.vehicleRepository.Vehicles)
            {
                if (vehicle.State == State.Move || vehicle.State == State.OnIntersection)
                    continue;

                if (vehicle.State == State.InIntersectionQueue && !Queue.Contains(vehicle))
                {
                    if (carOnInput(vehicle))
                        Queue.Add(vehicle);
                }

                if (vehicle.State == State.ReadyToLeaveIntersection && Queue.Contains(vehicle))
                {
                    
                    vehicle.CurrentEdge = environment.edgePipes.FirstOrDefault(ep => ep.Edges.Any(e => e.ID == vehicle.NextEdge.ID)).Edges.FirstOrDefault(e => e.ID == vehicle.NextEdge.ID);
                    Console.WriteLine("CurrentEdge X: " + vehicle.CurrentEdge.From.X + " Y: " + vehicle.CurrentEdge.From.Y);
                    vehicle.Position.X = vehicle.CurrentEdge.From.X;
                    vehicle.Position.Y = vehicle.CurrentEdge.From.Y;
                    vehicle.NewPositionY = vehicle.Position.Y;
                    //vehicle.NextEdge = null;
                    vehicle.State = State.Move;

                    Queue.Remove(vehicle);
                    IsBusy = false;
                }
            }

            if (!IsBusy && Queue.Count() != 0)
            {
                CurrentVehicle = Queue.First();
                CurrentVehicle.State = State.OnIntersection;
                CurrentVehicle.NextEdge = intersectionPipes.FirstOrDefault(ip => ip.IntersectionType == IntersectionPipeType.Out).EdgeRoad;

                IsBusy = true;
            }
        }

        public bool carOnInput(Vehicle vehicle)
        {
            var tmpPoint2D = new Point2D(vehicle.CurrentConnectorX, vehicle.CurrentConnectorY);
            foreach (var connector in Connectors)
            {
                Console.WriteLine("connector: " + connector.toString());
            }
            Console.WriteLine("Car in queue: " + (vehicle.State == State.InIntersectionQueue ? "true" : "false"));
            Console.WriteLine("Car point: " + tmpPoint2D.toString());
            Console.WriteLine("Cat position in connectors: " + (Connectors.Any(p => p == tmpPoint2D) ? "true" : "false"));
            return Connectors.Any(p => p == tmpPoint2D);
        }

        public bool CurrentVehicleStillOnIntersection()
        {
            return false;
        }
    }
}
