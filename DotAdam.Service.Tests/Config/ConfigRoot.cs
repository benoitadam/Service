
namespace SimpleService.Tests.Config
{
    public class ConfigRoot
    {
        private static ConfigWatcher<ConfigRoot> _watcher = ConfigWatcherFactory.GetWatcher<ConfigRoot>("Config/config.json");

        public static ConfigRoot Default { get { return _watcher.Config; } }

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

        public ConfigRoot()
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
