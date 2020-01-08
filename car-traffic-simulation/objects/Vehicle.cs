using car_traffic_simulation.engines;
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
    public class Vehicle : RenderedObject
    {
        public int Velocity { get; set; }
        public int OldVelocity { get; set; }
        public string TexturePath { get; set; }
        public EdgeRoad CurrentEdge { get; set; }
        public EdgeRoad NextEdge { get; set; } = null;
        public int NewPositionY { get; set; }
        public int NewPositionX { get; set; }
        public VehicleState State { get; set; }
        public int CurrentConnectorX { get; set; }
        public int CurrentConnectorY { get; set; }
        public int? CurrentIntersectionID { get; set; } = null;
        public string URI { get; set; }
        public CardinalDirection Turn { get; set; }

        public Vehicle(int id, int offSetX, int offSetY, int velocity, int height, int width, EdgeRoad currentEdge)
        {
            CurrentEdge = currentEdge;
            ID = id;

            Position = new Point2D(currentEdge.From);

            Position.X += offSetX;
            Position.Y += offSetY;

            NewPositionY = Position.Y;
            NewPositionX = Position.X;

            Velocity = velocity;
            OldVelocity = Velocity;
            Height = height;
            Width = width;
            State = VehicleState.Move;

            MovementVetorX = 0;
            MovementVetorY = 0;

            image = new Image();

            Turn = currentEdge.Direction;
        }

        private readonly static Dictionary<VehicleState, String> _stateToStr = new Dictionary<VehicleState, string>
        {
            { VehicleState.InIntersectionQueue, "In intersection queue" },
            { VehicleState.OnIntersection, "On intersection" },
            { VehicleState.ReadyToLeaveIntersection, "Leaving intersection" },
            { VehicleState.Move, "Moving" },
            { VehicleState.NoIntersection, "No intersection" }
        };

        private readonly static Dictionary<CardinalDirection, String> _cardinalDirectionToStr = new Dictionary<CardinalDirection, string>
        {
            { CardinalDirection.East, "East" },
            { CardinalDirection.West, "West" },
            { CardinalDirection.North, "North" },
            { CardinalDirection.South, "South" }
        };

        public String StateToStr(VehicleState state) => _stateToStr[state];
        public String CardinalDirectionToStr(CardinalDirection direction) => _cardinalDirectionToStr[direction];

        public override void draw()
        {
            Canvas.SetLeft(image, calculateStartDrawPointX());
            Canvas.SetTop(image, calculateStartDrawPointY());
        }

        public override void act(SimulationState state)
        {
            MovementVetorX = 0;
            MovementVetorY = 0;

            if (State == VehicleState.NoIntersection)
            {
                spawnFromEdgeWithNoIntersectionToNewEdge(state.intersections.SelectMany(ip => ip.intersectionPipes).ToList(), state.edgePipes);
                rotateTexture();
            }
            else if (NextEdge != null && State == VehicleState.OnIntersection)
            {
                moveToNewConnectors();
                rotateTexture();
                return;
            }

            var restEdges = state.edgePipes
                .Where(e => e.ID == CurrentEdge.PipeID)
                .FirstOrDefault().Edges
                .Where(e => e.ID != CurrentEdge.ID);

            foreach (var vehicle in state.vehicles)
            {
                if (State == VehicleState.InIntersectionQueue || State == VehicleState.NoIntersection || !restEdges.Where(re => re.PipeID == vehicle.CurrentEdge.PipeID).Any())
                    continue;

                setCloserPositionToOutrunNewPosition();

                bool isOnIntersection = isVehicleOnIntersection();

                CurrentConnectorY = CurrentEdge.To.Y;
                CurrentConnectorX = CurrentEdge.To.X;

                if (isOnIntersection)
                {
                    OldVelocity = Velocity;
                    Velocity = 0;

                    bool noIntersection = !state.intersections
                        .Any(ip => ip.intersectionPipes
                        .Any(p => p.EdgeRoad.ID == CurrentEdge.ID && p.IntersectionType == IntersectionPipeType.In));

                    if (noIntersection)
                        State = VehicleState.NoIntersection;
                    else
                    {
                        CurrentIntersectionID = state.intersections.SingleOrDefault(ip => ip.intersectionPipes.Any(p => p.EdgeRoad.ID == CurrentEdge.ID && p.IntersectionType == IntersectionPipeType.In)).ID;
                        State = VehicleState.InIntersectionQueue;
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

                    EdgeRoad closestEdge = null;
                    setClosestEdgeRoad(ref closestEdge, restEdges);

                    if (isNextLaneFreeForOutrun(closestEdge, state.vehicles))
                    {
                        setNewPositionForOutrun(ref closestEdge);
                        CurrentEdge = closestEdge; 
                        Velocity += 1;
                    }
                }
            }

            prepareMovementVector();

            Position.X += MovementVetorX;
            Position.Y += MovementVetorY;
        }

        private void prepareMovementVector()
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
                MovementVetorY = 0 - Velocity;
                break;
            case CardinalDirection.South:
                MovementVetorY = Velocity;
                break;
            }
        }

        protected override int calculateStartDrawPointX()
        {
            switch (Turn)
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

        protected override int calculateStartDrawPointY()
        {
            switch (Turn)
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

        private void spawnFromEdgeWithNoIntersectionToNewEdge(List<IntersectionPipe> intersectionsPipes, List<EdgePipe> edgePipes)
        {
            Dictionary<int, List<IntersectionPipeType>> intersectionPipesPairs = new Dictionary<int, List<IntersectionPipeType>>();

            foreach (var ip in intersectionsPipes)
            {
                if (!intersectionPipesPairs.ContainsKey(ip.EdgeRoad.ID))
                    intersectionPipesPairs.Add(ip.EdgeRoad.ID, new List<IntersectionPipeType>());

                intersectionPipesPairs[ip.EdgeRoad.ID].Add(ip.IntersectionType);
            }

            var distinctEdges = intersectionPipesPairs
                .Where(ipp => ipp.Value.Count() == 1 && ipp.Value.Contains(IntersectionPipeType.In));

            var randomEdge = distinctEdges
                .ElementAt(new Random().Next(0, distinctEdges.Count())).Key;

            CurrentEdge = edgePipes
                .SelectMany(ep => ep.Edges)
                .FirstOrDefault(e => e.ID == randomEdge);

            Velocity = OldVelocity;
            State = VehicleState.Move;

            Position.X = CurrentEdge.From.X;
            Position.Y = CurrentEdge.From.Y;
            NewPositionX = CurrentEdge.From.X;
            NewPositionY = CurrentEdge.From.Y;
            Turn = CurrentEdge.Direction;
        }

        private void moveToNewConnectors()
        {
            Velocity = 1;

            if (NextEdge.Direction == CardinalDirection.East || NextEdge.Direction == CardinalDirection.West)
            {
                if (NextEdge.From.Y == Position.Y && NextEdge.From.X == Position.X)
                {
                    State = VehicleState.ReadyToLeaveIntersection;
                    Velocity = OldVelocity;
                    rotateTexture();
                    return;
                }
                else if (NextEdge.From.Y < Position.Y)
                {
                    Position.Y += 0 - Velocity;
                    Turn = CardinalDirection.North;
                }
                else if (NextEdge.From.Y > Position.Y)
                {
                    Position.Y += Velocity;
                    Turn = CardinalDirection.South;
                }
                else if (NextEdge.From.X < Position.X)
                {
                    Position.X += 0 - Velocity;
                    Turn = CardinalDirection.West;
                }
                else if (NextEdge.From.X > Position.X)
                {
                    Position.X += Velocity;
                    Turn = CardinalDirection.East;
                }
            }
            else
            {
                if (NextEdge.From.Y == Position.Y && NextEdge.From.X == Position.X)
                {
                    State = VehicleState.ReadyToLeaveIntersection;
                    Velocity = OldVelocity;
                    rotateTexture();
                    return;
                }
                else if (NextEdge.From.X < Position.X)
                {
                    Position.X += 0 - Velocity;
                    Turn = CardinalDirection.West;
                }
                else if (NextEdge.From.X > Position.X)
                {
                    Position.X += Velocity;
                    Turn = CardinalDirection.East;
                }
                else if (NextEdge.From.Y < Position.Y)
                {
                    Position.Y += 0 - Velocity;
                    Turn = CardinalDirection.North;
                }
                else if (NextEdge.From.Y > Position.Y)
                {
                    Position.Y += Velocity;
                    Turn = CardinalDirection.South;
                }
            }
        }

        private bool isVehicleOnIntersection()
        {
            bool isOnIntersection = false;

            switch (CurrentEdge.Direction)
            {
            case CardinalDirection.East:
                CurrentConnectorY = Position.Y;
                CurrentConnectorX = calculateStartDrawPointX() + Width;
                isOnIntersection = CurrentEdge.To.X <= CurrentConnectorX;
                break;
            case CardinalDirection.West:
                CurrentConnectorY = Position.Y;
                CurrentConnectorX = calculateStartDrawPointX();
                isOnIntersection = CurrentEdge.To.X >= CurrentConnectorX;
                break;
            case CardinalDirection.North:
                CurrentConnectorX = Position.X;
                CurrentConnectorY = calculateStartDrawPointY();
                isOnIntersection = CurrentEdge.To.Y >= CurrentConnectorY;
                break;
            case CardinalDirection.South:
                CurrentConnectorX = Position.X;
                CurrentConnectorY = calculateStartDrawPointY() + Width;
                isOnIntersection = CurrentEdge.To.Y <= CurrentConnectorY;
                break;
            }

            return isOnIntersection;
        }

        private void setCloserPositionToOutrunNewPosition()
        {
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
        }

        private void setNewPositionForOutrun(ref EdgeRoad closestEdge)
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
        }

        private void setClosestEdgeRoad(ref EdgeRoad edgeRoadToSet, IEnumerable<EdgeRoad> restEdges)
        {
            int closestEdgeDistance = 0;

            foreach (var edge in restEdges)
            {
                switch (CurrentEdge.Direction)
                {
                case CardinalDirection.South:
                case CardinalDirection.North:
                    if (closestEdgeDistance == 0)
                    {
                        closestEdgeDistance = Math.Abs(edge.From.X - CurrentEdge.From.X);
                        edgeRoadToSet = edge;
                    }
                    else if (closestEdgeDistance != 0 && Math.Abs(edge.From.X - CurrentEdge.From.X) < closestEdgeDistance)
                    {
                        closestEdgeDistance = Math.Abs(edge.From.X - CurrentEdge.From.X);
                        edgeRoadToSet = edge;
                    }
                    break;
                case CardinalDirection.West:
                case CardinalDirection.East:
                    if (closestEdgeDistance == 0)
                    {
                        closestEdgeDistance = Math.Abs(edge.From.Y - CurrentEdge.From.Y);
                        edgeRoadToSet = edge;
                    }
                    else if (closestEdgeDistance != 0 && Math.Abs(edge.From.Y - CurrentEdge.From.Y) < closestEdgeDistance)
                    {
                        closestEdgeDistance = Math.Abs(edge.From.Y - CurrentEdge.From.Y);
                        edgeRoadToSet = edge;
                    }
                    break;
                }
            }
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
                return vehicle.calculateStartDrawPointY() < calculateStartDrawPointY();
            case CardinalDirection.South:
                return vehicle.calculateStartDrawPointY() > calculateStartDrawPointY();
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
                return Math.Abs(vehicle.calculateStartDrawPointY() - calculateStartDrawPointY()) < 2 * Width;
            case CardinalDirection.West:
            case CardinalDirection.East:
                return Math.Abs(vehicle.calculateStartDrawPointX() - calculateStartDrawPointX()) < 2 * Width;
            default:
                return false;
            }
        }

        private void rotateTexture()
        {
            BitmapImage bitmapImage = new BitmapImage();

            image.BeginInit();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("..\\..\\" + URI, UriKind.Relative);
            bitmapImage.DecodePixelHeight = 70;
            bitmapImage.DecodePixelWidth = 35;
            bitmapImage.DecodePixelHeight = (int)Width;
            bitmapImage.DecodePixelWidth = (int)Height;

            if (Turn == CardinalDirection.West || Turn == CardinalDirection.East)
            {
                image.Height = (int)Height;
                image.Width = (int)Width;
            }
            else
            {
                image.Height = (int)Width;
                image.Width = (int)Height;
            }

            switch (Turn)
            {
            case CardinalDirection.East:
                bitmapImage.Rotation = Rotation.Rotate270;
                break;
            case CardinalDirection.West:
                bitmapImage.Rotation = Rotation.Rotate90;
                break;
            case CardinalDirection.North:
                bitmapImage.Rotation = Rotation.Rotate180;
                break;
            case CardinalDirection.South:
                bitmapImage.Rotation = Rotation.Rotate0;
                break;
            }

            bitmapImage.EndInit();

            image.Source = bitmapImage;
            image.EndInit();
        }
    }
}
