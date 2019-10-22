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
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int LastX { get; set; }
        public int LastY { get; set; }
        public int Velocity { get; set; }
        public int OldVelocity { get; set; }
        public float Angle { get; set; }

        public Vehicle(int x, int y, int velocity, int height, int width)
        {
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
            foreach(var vehicle in environment.vehicleRepository.Vehicles)
            {
                if (vehicle.calculatePivotY() == calculatePivotY() 
                    && ((CurrentAction == Action.MoveForward && vehicle.calculatePivotX() > calculatePivotX()) || (CurrentAction == Action.MoveBackward && vehicle.calculatePivotX() < calculatePivotX()))
                    && Math.Abs(vehicle.calculatePivotX() - calculatePivotX()) < 2 * Width)
                {
                    OldVelocity = Velocity;
                    Velocity = vehicle.Velocity;
                }
            }

            decideAction(CurrentAction);

            X += MovementVetor;
        }
    }
}
