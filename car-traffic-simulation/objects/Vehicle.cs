using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace car_traffic_simulation.objects
{
    public enum Action
    {
        MoveForward,
        MoveBackward,
        TurnRight,
        TurnLeft
    };

    public class Vehicle
    {
        public Image image;
        protected int MovementVetorX;
        protected int MovementVetorY;
        public Action CurrentAction { get; set; }
        public int ID { get; set; }
        public Point2D Position { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Velocity { get; set; }
        public int OldVelocity { get; set; }
        public string TexturePath { get; set; }
        public EdgeRoad CurrentEdge { get; set; }
        public int NewPositionY { get; set; }

        public Vehicle(int id, int offSetX, int offSetY, int velocity, int height, int width, EdgeRoad currentEdge)
        {
            CurrentEdge = currentEdge;
            ID = id;

            Position = new Point2D(currentEdge.From);

            Position.X += offSetX;
            Position.Y += offSetY;

            NewPositionY = Position.Y;

            Velocity = velocity;
            OldVelocity = Velocity;
            Height = height;
            Width = width;

            MovementVetorX = 0;
            MovementVetorY = 0;

            image = new Image();
        }

        public virtual void Draw()
        {
            Canvas.SetLeft(image, calculatePivotX());
            Canvas.SetTop(image, calculatePivotY());
        }

        public virtual void Draw(Action action) { }

        public void decideAction()
        {
            switch (CurrentEdge.Direction)
            {
                case CardinalDirection.East:
                    MovementVetorX = Velocity;
                    break;
                case CardinalDirection.West:
                    MovementVetorX = 0 - Velocity;
                    break;
                case CardinalDirection.North:
                    MovementVetorY = Velocity;
                    break;
                case CardinalDirection.South:
                    MovementVetorY = 0 - Velocity;
                    break;
            }
        }

        public int calculatePivotX()
        {
            return Position.X + Width / 2;
        }

        public int calculatePivotY()
        {
            return Position.Y - Height / 2;
        }

        public void act(car_traffic_simulation.engines.Environment environment)
        {
            int CalculatedMovementVectorX = MovementVetorX;
            //int CalculatedMovementVectorY = MovementVetor;

            var restEdges = environment.edgePipes.Where(e => e.ID == CurrentEdge.PipeID).FirstOrDefault().Edges.Where(e => e.ID != CurrentEdge.ID);

            foreach (var vehicle in environment.vehicleRepository.Vehicles)
            {
                /*
                if (!restEdges.Where(re => re.ID == vehicle.CurrentEdge.ID).Any())
                {
                    continue;
                }
                */

                if (NewPositionY < Position.Y)
                {
                    Position.Y -= 1;
                }
                else if (NewPositionY > Position.Y)
                {
                    Position.Y += 1;
                }

                if ((CurrentEdge.Direction == CardinalDirection.East && CurrentEdge.To.X <= calculatePivotX()) || 
                    (CurrentEdge.Direction == CardinalDirection.West && CurrentEdge.To.X >= calculatePivotX()))
                {
                    Velocity = 0;
                    if (vehicle.CurrentEdge.ID == CurrentEdge.ID && isVehicleInFront(vehicle) && isVehicleNCarWidthInFrontAtLeast(2, vehicle))
                        Velocity = vehicle.Velocity;
                    continue;
                }

                if ((CurrentEdge.Direction == CardinalDirection.North && CurrentEdge.To.Y <= calculatePivotY()) ||
                    (CurrentEdge.Direction == CardinalDirection.South && CurrentEdge.To.Y >= calculatePivotY()))
                {
                    Velocity = 0;
                    if (vehicle.CurrentEdge.ID == CurrentEdge.ID && isVehicleInFront(vehicle) && isVehicleNCarWidthInFrontAtLeast(2, vehicle))
                        Velocity = vehicle.Velocity;
                    continue;
                }

                if (vehicle.ID == ID)
                    continue;

                if (vehicle.CurrentEdge.ID == CurrentEdge.ID && isVehicleInFront(vehicle) && isVehicleNCarWidthInFrontAtLeast(2, vehicle))
                {
                    Velocity = vehicle.Velocity;

                    if (restEdges.Count() == 0)
                        continue;

                    int closestEdgeDistance = 0;
                    EdgeRoad closestEdge = null;

                    foreach (var edge in restEdges)
                    {
                        if (closestEdgeDistance == 0)
                        {
                            closestEdgeDistance = Math.Abs(edge.From.Y - CurrentEdge.From.Y);
                            closestEdge = edge;
                        }

                        if (closestEdgeDistance != 0 && Math.Abs(edge.From.Y - CurrentEdge.From.Y) < closestEdgeDistance)
                        {
                            closestEdgeDistance = Math.Abs(edge.From.Y - CurrentEdge.From.Y);
                            closestEdge = edge;
                        }
                    }

                    if (isNextLaneFreeForOutrun(closestEdge, environment.vehicleRepository.Vehicles))
                    {
                        //Position.Y = closestEdge.From.Y;
                        NewPositionY = closestEdge.From.Y;
                        CurrentEdge = closestEdge; 
                        Velocity += 1;
                    }
                }
            }
 
            decideAction();

            Position.X += MovementVetorX;
            Position.Y += MovementVetorY;
        }

        private bool isNextLaneFreeForOutrun(EdgeRoad closestEdge, List<Vehicle> vehicles)
        {
            if (closestEdge == null)
                return false;

            foreach (var vehicle in vehicles)
            {
                if (vehicle.ID == ID)
                    continue;

                if (vehicle.CurrentEdge.ID != closestEdge.ID)
                    continue;

                if (doesVectorIntrudeOnVector(Position.X, Position.X + Width, vehicle.Position.X, vehicle.Position.X + vehicle.Width))
                    return false;
            }

            return true;
        }

        public bool outrun(ref int CalculatedMovementVectorX, ref int CalculatedMovementVectorY, Vehicle toOutrun)
        {
            if (Position.Y <= (toOutrun.Position.Y + toOutrun.Height))
            {
                CalculatedMovementVectorY = 0 - Velocity;
            }

            if (Position.X >= (toOutrun.Position.X + 2 * toOutrun.Width))
                return true;

            return false;
        }

        private bool doesVehicleMoveToTheSameDirection(Vehicle vehicle)
        {
            return vehicle.calculatePivotY() + 20 + vehicle.Height >= calculatePivotY() && calculatePivotY() >= vehicle.calculatePivotY() - 20 - vehicle.Height;
        }

        private bool isVehicleInFront(Vehicle vehicle)
        {
            switch (CurrentEdge.Direction)
            {
                case CardinalDirection.East:
                    return vehicle.calculatePivotX() > calculatePivotX();
                case CardinalDirection.West:
                    return vehicle.calculatePivotX() < calculatePivotX();
                default:
                    return false;
            }
        }

        private bool isVehicleNCarWidthInFrontAtLeast(int nCarWidth, Vehicle vehicle)
        {
            return Math.Abs(vehicle.calculatePivotX() - calculatePivotX()) < 2 * Width;
        }

        private bool isLeftLaneFreeForOutrun(car_traffic_simulation.engines.Environment environment)
        {
            foreach (var vehicle in environment.vehicleRepository.Vehicles)
            {
                if (vehicle.ID == ID)
                    continue;

                if (vehicle.Position.Y < (Position.Y - 2 * Height))              
                    continue;

                if (doesVectorIntrudeOnVector(Position.X, Position.X + Width, vehicle.Position.X, vehicle.Position.X + vehicle.Width))
                    return false;
            }

            return true;
        }

        private bool doesVectorIntrudeOnVector(int firstVecFirstX, int firstVecSecX, int secVecFirstX, int secVecSecX)
        {
            return !((secVecSecX < firstVecFirstX - Width/2) || (secVecFirstX > firstVecSecX + Width/2));
        }
    }
}
