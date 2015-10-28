Here's a simple example:

	class Program : IRunnable
	{
		static void Main(string[] args)
		{
			WindowsService.Main(args);
		}
		
		// on service start or test start
		public void Start()
		{
            Console.WriteLine("start");
		}
		
		// on service stop or test stop
		public void Stop()
		{
            Console.WriteLine("stop");
		}
	}

A example with timer:

	class Program : IRunnable
	{
        private readonly BusyTimer _timer;

        static void Main(string[] args)
        {
            WindowsService.Main(args);
        }

        public Program()
        {
            _timer = new BusyTimer(Update);
        }
        
        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
        
		// called only one time both
        public void Update()
        {
            Console.WriteLine("Update start");
            System.Threading.Thread.Sleep(10000);
            Console.WriteLine("Update end");
        }
    }