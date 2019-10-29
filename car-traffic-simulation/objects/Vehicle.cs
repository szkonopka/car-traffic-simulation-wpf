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
        protected int MovementVetor;
        public Action CurrentAction { get; set; }
        public int ID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int LastX { get; set; }
        public int LastY { get; set; }
        public int Velocity { get; set; }
        public int OldVelocity { get; set; }
        public float Angle { get; set; }

        public Vehicle(int id, int x, int y, int velocity, int height, int width)
        {
            ID = id;
            X = x;
            Y = y;
            Velocity = velocity;
            OldVelocity = Velocity;
            Height = height;
            Width = width;

            MovementVetor = 0;
            image = new Image();
        }

        public virtual void Draw()
        {
            Canvas.SetLeft(image, X);
            Canvas.SetTop(image, Y);
        }

        public virtual void Draw(Action action) { }

        public void decideAction(Action action)
        {
            CurrentAction = action;

            switch (action)
            {
                case Action.MoveForward:
                    MovementVetor = Velocity;
                    break;
                case Action.MoveBackward:
                    MovementVetor = 0 - Velocity;
                    break;
                case Action.TurnLeft:
                    break;
                case Action.TurnRight:
                    break;
            }
        }

        public int calculatePivotX()
        {
            return X + Width / 2;
        }

        public int calculatePivotY()
        {
            return Y - Height / 2;
        }

        public void act(car_traffic_simulation.engines.Environment environment)
        {
            int CalculatedMovementVectorX = MovementVetor;
            int CalculatedMovementVectorY = 0;

            foreach (var vehicle in environment.vehicleRepository.Vehicles)
            {
                if (doesVehicleMoveToTheSameDirection(vehicle) && isVehicleInFront(vehicle) && isVehicleNCarWidthInFrontAtLeast(2, vehicle))
                {
                    if (isLeftLaneFreeForOutrun(environment))
                    {
                        if (outrun(ref CalculatedMovementVectorX, ref CalculatedMovementVectorY, vehicle))
                        {
                            Velocity = 0;
                        }
                        else
                        {
                            Velocity = vehicle.Velocity + 1;
                        }                        
                    }
                    else
                    {
                        OldVelocity = Velocity;
                        Velocity = vehicle.Velocity;
                    }
                }
            }

            decideAction(CurrentAction);

            X += MovementVetor;
            Y += CalculatedMovementVectorY;
        }

        public bool outrun(ref int CalculatedMovementVectorX, ref int CalculatedMovementVectorY, Vehicle toOutrun)
        {
            if (Y <= (toOutrun.Y + toOutrun.Height))
            {
                CalculatedMovementVectorY = 0 - Velocity;
            }

            if (X >= (toOutrun.X + 2 * toOutrun.Width))
                return true;

            return false;
        }

        private bool doesVehicleMoveToTheSameDirection(Vehicle vehicle)
        {
            return vehicle.calculatePivotY() + 20 + vehicle.Height >= calculatePivotY() && calculatePivotY() >= vehicle.calculatePivotY() - 20 - vehicle.Height;
        }

        private bool isVehicleInFront(Vehicle vehicle)
        {
            switch (CurrentAction)
            {
                case Action.MoveForward:
                    return vehicle.calculatePivotX() > calculatePivotX();
                case Action.MoveBackward:
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

                if (vehicle.Y < (Y - 2 * Height))              
                    continue;

                if (doesVectorIntrudeOnVector(X, X + Width, vehicle.X, vehicle.X + vehicle.Width))
                    return false;
            }

            return true;
        }

        private bool doesVectorIntrudeOnVector(int firstVecFirstX, int firstVecSecX, int secVecFirstX, int secVecSecX)
        {
            return !((secVecSecX < firstVecFirstX) || (secVecFirstX > firstVecSecX));
        }
    }
}
