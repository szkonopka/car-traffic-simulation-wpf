using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace car_traffic_simulation.engines
{
    public class SimulationCoordinator
    {
        private readonly int ONE_SECOND_MS = 1000;
        private DispatcherTimer _environmentTimer;
        public SimulationState state { get; }
        
        public SimulationCoordinator(SimulationState _state, uint framePerSecond = 60)
        {
            _environmentTimer = new DispatcherTimer();

            _environmentTimer.Interval = new TimeSpan(0, 0, 0, 0, ONE_SECOND_MS / (int)framePerSecond);
            _environmentTimer.Tick += RenderState;
            
            state = _state;
        }

        private void RenderState(object sender, EventArgs e)
        {
            foreach (var intersection in state.intersections)
                intersection.act(state);

            foreach (var vehicle in state.vehicles)
            {
                vehicle.act(state);
                vehicle.draw();
            }   
        }

        public void Start()
        {
            _environmentTimer.Start();
        }

        public void Stop()
        {
            _environmentTimer.Stop();
        }

    }
}
