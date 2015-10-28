using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace DotAdam.Service
{
    /// <summary>
    /// Interface for create windows service
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        /// Service start method
        /// </summary>
        void Start();

        /// <summary>
        /// Service stop method
        /// </summary>
        void Stop();
    }
}
