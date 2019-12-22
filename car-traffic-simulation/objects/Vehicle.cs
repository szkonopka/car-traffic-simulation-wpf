using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace car_traffic_simulation.objects
{
    public enum Action
    {
        MoveForward,
        MoveBackward,
        TurnRight,
        TurnLeft
    };

    public enum State
    {
        InIntersectionQueue,
        OnIntersection,
        ReadyToLeaveIntersection,
        Move,
        NoIntersection
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
        public EdgeRoad NextEdge { get; set; } = null;
        public int NewPositionY { get; set; }
        public int NewPositionX { get; set; }
        public int PivotX { get; set; }
        public int PivotY { get; set; }
        public State State { get; set; }
        public int CurrentConnectorX { get; set; }
        public int CurrentConnectorY { get; set; }
        public int? CurrentIntersectionID { get; set; } = null;

        public Vehicle(int id, int offSetX, int offSetY, int velocity, int height, int width, EdgeRoad currentEdge)
        {
            CurrentEdge = currentEdge;
            ID = id;

            Position = new Point2D(currentEdge.From);

            Position.X += offSetX;
            Position.Y += offSetY;

            switch (currentEdge.Direction)
            {
                case CardinalDirection.East:
                    PivotX = Position.X;
                    PivotY = Position.Y;
                    break;
                case CardinalDirection.West:
                    PivotX = Position.X;
                    PivotY = Position.Y;
                    break;
                case CardinalDirection.North:
                    PivotX = Position.X;
                    PivotY = Position.Y;
                    break;
                case CardinalDirection.South:
                    PivotX = Position.X;
                    PivotY = Position.Y;
                    break;
            }

            NewPositionY = Position.Y;
            NewPositionX = Position.X;

            Velocity = velocity;
            OldVelocity = Velocity;
            Height = height;
            Width = width;
            State = State.Move;

            MovementVetorX = 0;
            MovementVetorY = 0;

            image = new Image();
        }

        public virtual void Draw()
        {
            Canvas.SetLeft(image, calculateStartDrawPointX());
            Canvas.SetTop(image, calculateStartDrawPointY());
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

        public int calculateStartDrawPointX()
        {
            switch (CurrentEdge.Direction)
            {
                case CardinalDirection.East:
                case CardinalDirection.West:
                    return Position.X - Width / 2;
                case CardinalDirection.North:
                case CardinalDirection.South:
                    return Position.X - Height / 2;
                default:
                    return Position.X;
            }
        }

        public int calculateStartDrawPointY()
        {
            switch (CurrentEdge.Direction)
            {
                case CardinalDirection.East:
                case CardinalDirection.West:
                    return Position.Y - Height / 2;
                case CardinalDirection.North:
                case CardinalDirection.South:
                    return Position.Y - Width / 2;
                default:
                    return Position.Y;
            }
        }

        public void act(car_traffic_simulation.engines.Environment environment)
        {
            MovementVetorX = 0;
            MovementVetorY = 0;

            if (State == State.NoIntersection)
            {
                /*
                CurrentEdge = environment.edgePipes
                    .SelectMany(e => e.Edges).Where(e => e.ID != CurrentEdge.ID);
                */

                var intersectionsPipes = environment.intersections
                    .SelectMany(ip => ip.intersectionPipes).ToList();

                Dictionary<int, List<IntersectionPipeType>> intersectionPipesPairs = new Dictionary<int, List<IntersectionPipeType>>();

                foreach (var ip in intersectionsPipes)
                {
                    if (!intersectionPipesPairs.ContainsKey(ip.EdgeRoad.ID))
                    {
                        intersectionPipesPairs.Add(ip.EdgeRoad.ID, new List<IntersectionPipeType>());
                    }

                    intersectionPipesPairs[ip.EdgeRoad.ID].Add(ip.IntersectionType);
                }
                Console.WriteLine("SIZE PAIRS: " + intersectionPipesPairs.Count());
                var distinctEdges = intersectionPipesPairs.Where(ipp => ipp.Value.Count() == 1 && ipp.Value.Contains(IntersectionPipeType.In));

                Console.WriteLine("SIZE DISTINCT: " + distinctEdges.Count());

                Random r = new Random();

                var randomEdge = distinctEdges.ElementAt(r.Next(0, distinctEdges.Count())).Key;
                Console.WriteLine("RANDOM EDGE ID: " + randomEdge);

                CurrentEdge = environment.edgePipes
                    .SelectMany(ep => ep.Edges)
                    .FirstOrDefault(e => e.ID == randomEdge);

                Velocity = OldVelocity;
                State = State.Move;

                Position.X = CurrentEdge.From.X;
                Position.Y = CurrentEdge.From.Y;
                NewPositionX = CurrentEdge.From.X;
                NewPositionY = CurrentEdge.From.Y;
            }

            if (NextEdge != null && State == State.OnIntersection)
            {
                Velocity = 1;

                if (NextEdge.Direction == CardinalDirection.East || NextEdge.Direction == CardinalDirection.West)
                {
                    if (NextEdge.From.Y == Position.Y && NextEdge.From.X == Position.X)
                    {
                        State = State.ReadyToLeaveIntersection;
                        Velocity = OldVelocity;
                        Console.WriteLine("Zwalniam auto " + ID);
                        rotateImage();
                        return;
                    }
                    else if (NextEdge.From.Y < Position.Y)
                    {
                        Position.Y += 0 - Velocity;
                    }
                    else if (NextEdge.From.Y > Position.Y)
                    {
                        Position.Y += Velocity;
                    }
                    else if (NextEdge.From.X < Position.X)
                    {
                        Position.X += 0 - Velocity;
                    }
                    else if (NextEdge.From.X > Position.X)
                    {
                        Position.X += Velocity;
                    }
                }
                else
                {
                    if (NextEdge.From.Y == Position.Y && NextEdge.From.X == Position.X)
                    {
                        State = State.ReadyToLeaveIntersection;
                        Velocity = OldVelocity;
                        Console.WriteLine("Zwalniam auto " + ID);
                        rotateImage();
                        return;
                    }
                    else if (NextEdge.From.X < Position.X)
                    {
                        Position.X += 0 - Velocity;
                    }
                    else if (NextEdge.From.X > Position.X)
                    {
                        Position.X += Velocity;
                    }
                    else if (NextEdge.From.Y < Position.Y)
                    {
                        Position.Y += 0 - Velocity;
                    }
                    else if (NextEdge.From.Y > Position.Y)
                    {
                        Position.Y += Velocity;
                    } 
                }

                return;
            }

            int CalculatedMovementVectorX = MovementVetorX;
            int CalculatedMovementVectorY = MovementVetorY;

            var restEdges = environment.edgePipes.Where(e => e.ID == CurrentEdge.PipeID).FirstOrDefault().Edges.Where(e => e.ID != CurrentEdge.ID);

            foreach (var vehicle in environment.vehicleRepository.Vehicles)
            {
                if (State == State.InIntersectionQueue)
                    continue;

                if (State == State.NoIntersection)
                    continue;

                if (!restEdges.Where(re => re.PipeID == vehicle.CurrentEdge.PipeID).Any())
                {
                    continue;
                }

                switch (CurrentEdge.Direction)
                {
                    case CardinalDirection.North:
                    case CardinalDirection.South:
                        if ((CurrentEdge.Direction == CardinalDirection.North || CurrentEdge.Direction == CardinalDirection.South) && NewPositionX < Position.X)
                            Position.X -= 1;
                        else if ((CurrentEdge.Direction == CardinalDirection.North || CurrentEdge.Direction == CardinalDirection.South) && NewPositionX > Position.X)
                            Position.X += 1;
                        break;
                    case CardinalDirection.West:
                    case CardinalDirection.East:
                        if ((CurrentEdge.Direction == CardinalDirection.East || CurrentEdge.Direction == CardinalDirection.West) && NewPositionY < Position.Y)
                            Position.Y -= 1;
                        else if ((CurrentEdge.Direction == CardinalDirection.East || CurrentEdge.Direction == CardinalDirection.West) && NewPositionY > Position.Y)
                            Position.Y += 1;
                        break;
                }

                bool isOnIntersection = false;

                switch (CurrentEdge.Direction)
                {
                    case CardinalDirection.East:
                        CurrentConnectorY = Position.Y;
                        CurrentConnectorX = calculateStartDrawPointX() + Width;
                        isOnIntersection = CurrentEdge.To.X <= CurrentConnectorX;
                        CurrentConnectorX++;
                        break;
                    case CardinalDirection.West:
                        CurrentConnectorY = Position.Y;
                        CurrentConnectorX = calculateStartDrawPointX();
                        isOnIntersection = CurrentEdge.To.X >= CurrentConnectorX;
                        break;
                    case CardinalDirection.North:
                        CurrentConnectorX = Position.X;
                        CurrentConnectorY = calculateStartDrawPointY() + Width;
                        isOnIntersection = CurrentEdge.To.Y <= CurrentConnectorY;
                        break;
                    case CardinalDirection.South:
                        CurrentConnectorX = Position.X;
                        CurrentConnectorY = calculateStartDrawPointY();
                        isOnIntersection = CurrentEdge.To.Y >= CurrentConnectorY;
                        break;
                }

                CurrentConnectorY = CurrentEdge.To.Y;
                CurrentConnectorX = CurrentEdge.To.X;

                if (isOnIntersection)
                {
                    OldVelocity = Velocity;
                    Velocity = 0;

                    bool noIntersection = !environment.intersections.Any(ip => ip.intersectionPipes.Any(p => p.EdgeRoad.ID == CurrentEdge.ID && p.IntersectionType == IntersectionPipeType.In));

                    if (noIntersection)
                        State = State.NoIntersection;
                    else
                    {
                        CurrentIntersectionID = environment.intersections.SingleOrDefault(ip => ip.intersectionPipes.Any(p => p.EdgeRoad.ID == CurrentEdge.ID && p.IntersectionType == IntersectionPipeType.In)).ID;
                        State = State.InIntersectionQueue;
                    }
                    
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
                        switch (CurrentEdge.Direction)
                        {
                            case CardinalDirection.South:
                            case CardinalDirection.North:
                                if (closestEdgeDistance == 0)
                                {
                                    closestEdgeDistance = Math.Abs(edge.From.X - CurrentEdge.From.X);
                                    closestEdge = edge;
                                }

                                if (closestEdgeDistance != 0 && Math.Abs(edge.From.X - CurrentEdge.From.X) < closestEdgeDistance)
                                {
                                    closestEdgeDistance = Math.Abs(edge.From.X - CurrentEdge.From.X);
                                    closestEdge = edge;
                                }
                                break;
                            case CardinalDirection.West:
                            case CardinalDirection.East:
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
                                break;
                        }
                    }

                    if (isNextLaneFreeForOutrun(closestEdge, environment.vehicleRepository.Vehicles))
                    {
                        switch (CurrentEdge.Direction)
                        {
                            case CardinalDirection.East:
                            case CardinalDirection.West:
                                NewPositionY = closestEdge.From.Y;
                                break;
                            case CardinalDirection.North:
                            case CardinalDirection.South:
                                NewPositionX = closestEdge.From.X;
                                break;
                        }

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

                switch (CurrentEdge.Direction)
                {
                    case CardinalDirection.North:
                    case CardinalDirection.South:
                        //if (doesVectorIntrudeOnVectorY(Position.Y, Position.Y + Height, vehicle.Position.Y, vehicle.Position.Y + vehicle.Height))
                        if (doesVectorIntrudeOnVectorY(Position.Y, Position.Y + Width, vehicle.Position.Y, vehicle.Position.Y + vehicle.Width))
                            return false;
                        break;
                    case CardinalDirection.West:
                    case CardinalDirection.East:
                        if (doesVectorIntrudeOnVectorX(Position.X, Position.X + Width, vehicle.Position.X, vehicle.Position.X + vehicle.Width))
                            return false;
                        break;
                }
            }

            return true;
        }

        private bool isVehicleInFront(Vehicle vehicle)
        {
            switch (CurrentEdge.Direction)
            {
                case CardinalDirection.East:
                    return vehicle.calculateStartDrawPointX() > calculateStartDrawPointX();
                case CardinalDirection.West:
                    return vehicle.calculateStartDrawPointX() < calculateStartDrawPointX();
                case CardinalDirection.North:
                    return vehicle.calculateStartDrawPointY() > calculateStartDrawPointY();
                case CardinalDirection.South:
                    return vehicle.calculateStartDrawPointY() < calculateStartDrawPointY();
                default:
                    return false;
            }
        }

        private bool isVehicleNCarWidthInFrontAtLeast(int nCarWidth, Vehicle vehicle)
        {
            switch (CurrentEdge.Direction)
            {
                case CardinalDirection.North:
                case CardinalDirection.South:
                    //return Math.Abs(vehicle.calculateStartDrawPointY() - calculateStartDrawPointY()) < 2 * Height;
                    return Math.Abs(vehicle.calculateStartDrawPointY() - calculateStartDrawPointY()) < 2 * Width;
                    break;
                case CardinalDirection.West:
                case CardinalDirection.East:
                    return Math.Abs(vehicle.calculateStartDrawPointX() - calculateStartDrawPointX()) < 2 * Width;
                    break;
                default:
                    return false;
            }
        }

        private bool doesVectorIntrudeOnVectorY(int firstVecFirst, int firstVecSec, int secVecFirst, int secVecSec)
        {
            //return !((secVecSec < firstVecFirst - Height / 2) || (secVecFirst > firstVecSec + Height / 2));
            return !((secVecSec < firstVecFirst - Width / 2) || (secVecFirst > firstVecSec + Width / 2));
        }

        private bool doesVectorIntrudeOnVectorX(int firstVecFirst, int firstVecSec, int secVecFirst, int secVecSec)
        {
            return !((secVecSec < firstVecFirst - Width/2) || (secVecFirst > firstVecSec + Width/2));
        }

        private void rotateImage()
        {
            RotateTransform rotateTransform = null;

            switch (CurrentEdge.Direction)
            {
                case CardinalDirection.North:
                    if (NextEdge.Direction == CardinalDirection.South) {
                        rotateTransform = new RotateTransform(180);
                        image.RenderTransformOrigin = new Point(0.25, 0.5);
                    }
                    else if (NextEdge.Direction == CardinalDirection.West) {
                        rotateTransform = new RotateTransform(90);
                        image.RenderTransformOrigin = new Point(1, 0.5);
                    }
                    else if (NextEdge.Direction == CardinalDirection.East) {
                        rotateTransform = new RotateTransform(-90);
                        image.RenderTransformOrigin = new Point(1, 0.5);
                    }
                    break;
                case CardinalDirection.South:
                    if (NextEdge.Direction == CardinalDirection.North) {
                        rotateTransform = new RotateTransform(180);
                        image.RenderTransformOrigin = new Point(0.25, 0.5);
                    }
                    else if (NextEdge.Direction == CardinalDirection.West) {
                        rotateTransform = new RotateTransform(-90);
                        image.RenderTransformOrigin = new Point(1, 0.5);
                    }
                    else if (NextEdge.Direction == CardinalDirection.East) {
                        rotateTransform = new RotateTransform(90);
                        image.RenderTransformOrigin = new Point(1, 0.5);
                    }
                    break;
                case CardinalDirection.East:
                    if (NextEdge.Direction == CardinalDirection.West) {
                        rotateTransform = new RotateTransform(180);
                        image.RenderTransformOrigin = new Point(1, 0.5);
                    }
                    else if (NextEdge.Direction == CardinalDirection.North) {
                        rotateTransform = new RotateTransform(-90);
                        image.RenderTransformOrigin = new Point(0.25, 0.5);
                    }
                    else if (NextEdge.Direction == CardinalDirection.South) {
                        rotateTransform = new RotateTransform(-90);
                        image.RenderTransformOrigin = new Point(0.5, 1);
                    }
                    break;
                case CardinalDirection.West:
                    if (NextEdge.Direction == CardinalDirection.East) {
                        rotateTransform = new RotateTransform(180);
                        image.RenderTransformOrigin = new Point(1, 0.5);
                    }
                    else if (NextEdge.Direction == CardinalDirection.North) {
                        rotateTransform = new RotateTransform(-90);
                        image.RenderTransformOrigin = new Point(0.25, 0.5);
                    }
                    else if (NextEdge.Direction == CardinalDirection.South) {
                        rotateTransform = new RotateTransform(90);
                        image.RenderTransformOrigin = new Point(0.25, 0.5);
                    }
                    break;
            }

            image.RenderTransform = rotateTransform;
        }
    }
}
