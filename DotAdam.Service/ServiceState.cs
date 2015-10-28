using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAdam.Service
{
    [Flags]
    public enum ServiceState
    {
        None = 0,
        Initialised = 1 << 0,
        Started = 1 << 1,
        Paused = 1 << 2,
        Stopped = 1 << 3
    }
}
