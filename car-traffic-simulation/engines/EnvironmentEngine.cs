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
    public class EnvironmentEngine
    {
        DispatcherTimer environmentTimer;
        Environment state;
        
        public EnvironmentEngine(Environment environment, uint framePerSecond = 60)
        {
            environmentTimer = new DispatcherTimer();

            int tickMsFrequency = 1000 / (int) framePerSecond;

            environmentTimer.Tick += Render;
            environmentTimer.Interval = new TimeSpan(0, 0, 0, 0, tickMsFrequency);

            state = environment;
        }

        private void Render(object sender, EventArgs e)
        {
            foreach (var vehicle in state.vehicleRepository.Vehicles)
            {
                vehicle.act(state);
            }

            foreach (var vehicle in state.vehicleRepository.Vehicles)
            {
                vehicle.Draw();
            }
        }

        public void Start()
        {
            environmentTimer.Start();
        }

        public void Stop()
        {
            environmentTimer.Stop();
        }

    }
}
