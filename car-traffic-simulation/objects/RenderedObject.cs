using car_traffic_simulation.engines;
using System.Windows.Controls;

namespace car_traffic_simulation.objects
{
    public abstract class RenderedObject
    {
        public Image image;
        public Point2D Position { get; set; }
        public int ID { get; set; }
        protected int MovementVetorX;
        protected int MovementVetorY;
        protected int Height;
        protected int Width;

        protected abstract int calculateStartDrawPointX();
        protected abstract int calculateStartDrawPointY();

        protected bool doesVectorIntrudeOnVectorY(int firstVecFirst, int firstVecSec, int secVecFirst, int secVecSec) =>
            !((secVecSec < firstVecFirst - Width / 2) || (secVecFirst > firstVecSec + Width / 2));

        protected bool doesVectorIntrudeOnVectorX(int firstVecFirst, int firstVecSec, int secVecFirst, int secVecSec) =>
            !((secVecSec < firstVecFirst - Width / 2) || (secVecFirst > firstVecSec + Width / 2));

        public abstract void act(SimulationState state);
        public virtual void draw()
        {
            Canvas.SetLeft(image, calculateStartDrawPointX());
            Canvas.SetTop(image, calculateStartDrawPointY());
        }
    }
}
