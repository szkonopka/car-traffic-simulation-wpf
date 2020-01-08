using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_traffic_simulation.objects
{
    public enum VehicleState
    {
        InIntersectionQueue,
        OnIntersection,
        ReadyToLeaveIntersection,
        Move,
        NoIntersection
    };
}
