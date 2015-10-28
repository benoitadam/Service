using DotAdam.Service.Config;
using Newtonsoft.Json;
using SimpleService.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAdam.Service.Tests
{
    public class Config
    {
        private static ConfigWatcher<Config> _watcher = ConfigWatcherFactory.GetWatcher<Config>("Config/settings.config");

        public static Config Default { get { return _watcher.Config; } }

        private int _number;
        private TestConfig _test;

        public int Number {
            get { return _number; }
            set { _number = value; }
        }

        public TestConfig Test
        {
            get { return _test; }
            set { _test = value; }
        }

        public TestConfig[] Tests { get; set; }

        public Config()
        {
            Number = 5;
            Test = new TestConfig()
            {
                Number = 10
            };
        }
    }

    public class TestConfig
    {
        private int _number;

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }
    }
}
