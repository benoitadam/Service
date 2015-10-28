using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAdam.Service
{
    /// <summary>
    /// Config of timer
    /// </summary>
    public class BusyTimerConfig
    {
        public TimeSpan DueTime { get; set; }
        public TimeSpan Period { get; set; }
        public TimeSpan BusyRelease { get; set; }

        public BusyTimerConfig()
        {
            DueTime = TimeSpan.FromMilliseconds(100);
            Period = TimeSpan.FromSeconds(1000);
            BusyRelease = TimeSpan.FromMinutes(5);
        }
    }
}
