using SimpleService;
using SimpleService.Tests.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SimpleService.Tests
{
    class Program : IRunnable
    {
        static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            WindowsService.Main(args);
        }

        public void Start() {
            _log.Info("Start");
            _log.Debug("Number : ", ConfigRoot.Default.Number);
            _log.Debug("String : ", ConfigRoot.Default.Test.Number);
        }

        public void Stop()
        {
            _log.Info("Stop");
        }
    }
}
