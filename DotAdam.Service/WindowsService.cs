using System;
using System.Linq;
using System.Collections;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Security.Permissions;

namespace SimpleService
{
    /// <summary>
    /// Classe for create and simple WebService
    /// </summary>
    public class WindowsService : ServiceBase
    {
        const string CONFIG_PATH = "./Config/settings.config";

        static readonly ServiceLogger _log = ServiceLogger.GetLogger(typeof(WindowsService));
        private Thread _thread;
        private readonly EventWaitHandle _eventStop = new EventWaitHandle(false, EventResetMode.AutoReset);
        private static IRunnable _runnable = null;
        private static WindowsService _default;

        #region Properties

        private static Assembly ServiceAssembly
        {
            get
            {
                return Runnable.GetType().Assembly;
            }
        }

        /// <summary>
        /// Name of service
        /// </summary>
        public static string Name
        {
            get
            {
                return ServiceAssembly.GetName().Name;
            }
        }

        /// <summary>
        /// Resume of service
        /// </summary>
        public static string Description
        {
            get
            {
                var attributes = ServiceAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length > 0)
                    return ((AssemblyDescriptionAttribute)attributes[0]).Description;
                return "";
            }
        }

        /// <summary>
        /// If the service running and not paused
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return State.HasFlag(ServiceState.Started) && !State.HasFlag(ServiceState.Paused);
            }
        }

        /// <summary>
        /// Get service state flags
        /// </summary>
        public ServiceState State { get; protected set; }

        /// <summary>
        /// All runnable type
        /// </summary>
        public static IRunnable Runnable
        {
            get
            {
                if (_runnable == null)
                {
                    try
                    {
                        var runnableType = typeof(IRunnable);

                        var runnables = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .Where(p => runnableType.IsAssignableFrom(p) && p != runnableType)
                            .ToArray();

                        _log.Info(null, "runnables : {0}", String.Join<Type>(",", runnables));

                        if (runnables.Length == 0)
                            throw new Exception("You need implement 'MyService.IMyRunnable' in your project");

                        if (runnables.Length != 1)
                            throw new Exception("You need implement juste one class of 'MyService.IMyRunnable' in your project");

                        _runnable = (IRunnable)Activator.CreateInstance(runnables[0]);
                    }
                    catch (Exception ex)
                    {
                        var msg = "A problem occurred while instantiating the interface IMyRunnable";
                        _log.Error(ex, msg);
                        throw new Exception(msg, ex);
                    }
                }
                return _runnable;
            }
        }

        /// <summary>
        /// Get default instance of WindowsService
        /// </summary>
        public static WindowsService Default { get { return _default ?? (_default = new WindowsService()); } }

        #endregion

        public static void Main(string[] args)
        {
            _default = new WindowsService();
        }

        public WindowsService()
        {
            _log.Info(null, "ProjectService : Construction du service");

            this.ServiceName = Name;
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
            _log.Info(null, "ServiceName = {0}", this.ServiceName);

            try
            {
                string baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                _log.Info(null, "SetCurrentDirectory({0})", baseDirectory);
                System.IO.Directory.SetCurrentDirectory(baseDirectory);
            }
            catch (Exception ex) { _log.Error(ex, "Error in SetCurrentDirectory"); }

            if (Environment.UserInteractive)
            {
                // Console mode
                ConsoleStart();
            }
            else
            {
                // Service mode
                ServiceBase.Run(this);
            }
        }

        #region Methods

        /// <summary>
        /// Start signal of service
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            _log.Info(null, "OnStart : On service start");

            _thread = new Thread(new ThreadStart(Run));
            _thread.IsBackground = true;
            _thread.Start();
        }

        /// <summary>
        /// Body of service
        /// </summary>
        protected virtual void Run()
        {
            State = ServiceState.Started;
            _log.Info(null, "Run : start service");

            try
            {
                Runnable.Start();
            }
            catch (Exception ex) { _log.Error(ex, "Error in start methode"); }

            // Attente de l'événement Stop du service
            _eventStop.WaitOne();
            State = ServiceState.Stopped;
        }

        /// <summary>
        /// Stop signal of service
        /// </summary>
        protected override void OnStop()
        {
            _log.Info(null, "OnStop : Stop of service");

            _eventStop.Set();

            try
            {
                Runnable.Stop();
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error during the stop of service");
                if (System.Diagnostics.Debugger.IsAttached)
                    throw ex;
            }

            _thread = null;
        }

        protected override void OnPause()
        {
            _log.Info(null, "OnPause : Service is paused");
            State = ServiceState.Started & ServiceState.Paused;
        }

        protected override void OnContinue()
        {
            _log.Info(null, "OnContinue : Service is resumed");
            State = ServiceState.Started;
        }

        /// <summary>
        /// Console menu
        /// </summary>
        protected virtual void ConsoleMenu()
        {
            Console.WriteLine("");
            Console.WriteLine("++==============================================++");
            Console.WriteLine("|| You need administrator permission to install ||");
            Console.WriteLine("|+----------------------------------------------+|");
            Console.WriteLine("|| i : Install service                          ||");
            Console.WriteLine("|| u : Uninstall service                        ||");
            Console.WriteLine("|| t : Test service (in console)                ||");
            Console.WriteLine("|| e : End of service test (in console)         ||");
            Console.WriteLine("|| p : Pause of service test (in console)       ||");
            Console.WriteLine("|| r : Resume of service test (in console)      ||");
            Console.WriteLine("|| k : Close console                            ||");
            Console.WriteLine("++==============================================++");
            Console.Write("Please enter key : ");
        }

        private void Write(string line, ConsoleColor color = ConsoleColor.Yellow)
        {
            Write(new[] { line }, color);
        }

        private void Write(string[] lines, ConsoleColor color = ConsoleColor.Yellow)
        {
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            var length = lines.Max(p => p.Length);
            var padding = "+".PadRight(length + 3, '=') + '+';

            Console.WriteLine(padding);

            foreach(var line in lines)
                Console.WriteLine("| " + line + " |");

            Console.WriteLine(padding);
            Console.ForegroundColor = foregroundColor;
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void Install()
        {
            Write("Install service");
            using (AssemblyInstaller inst = new AssemblyInstaller(ServiceAssembly, new string[0]))
            {
                IDictionary state = new Hashtable();
                try
                {
                    inst.UseNewContext = true;
                    inst.Installers.Add(new WindowsServiceInstaller());
                    inst.Install(state);
                    inst.Commit(state);
                    Write("Service has been successfully installed", ConsoleColor.Green);
                }
                catch (Exception ex)
                {
                    Write(new[] { "Error during install", ex.Message }, ConsoleColor.Red);
                    inst.Rollback(state);
                }

                try
                {
                    var ctrl = new ServiceController();
                    ctrl.MachineName = ".";
                    ctrl.ServiceName = Name;
                    ctrl.Start();
                    Write("Service runs", ConsoleColor.Green);
                }
                catch (Exception ex)
                {
                    Write(new[] { "Error during start", ex.Message }, ConsoleColor.Red);
                }
            }
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void Uninstall()
        {
            Write("Uninstall service");
            using (AssemblyInstaller inst = new AssemblyInstaller(ServiceAssembly, new string[0]))
            {
                IDictionary state = new Hashtable();
                try
                {
                    inst.UseNewContext = true;
                    inst.Installers.Add(new WindowsServiceInstaller());
                    inst.Uninstall(state);
                    Write("Service has been successfully uninstalled", ConsoleColor.Green);
                }
                catch (Exception ex)
                {
                    Write(new [] { "Error during uninstall", ex.Message }, ConsoleColor.Red);
                }
            }
        }

        /// <summary>
        /// Method use for console mode
        /// </summary>
        /// <param name="args"></param>
        public virtual void ConsoleStart()
        {
            bool isClosed = false;

            Console.BufferWidth = 1024;
            Console.BufferHeight = 1024;

            ConsoleMenu();

            ConsoleKey key = ConsoleKey.NoName;
            while (!isClosed)
            {
                try
                {
                    key = Console.ReadKey().Key;
                    Console.WriteLine("");
                    switch (key)
                    {
                        case ConsoleKey.I:
                            Install();
                            break;

                        case ConsoleKey.U:
                            Uninstall();
                            break;

                        case ConsoleKey.T:
                            OnStart(new string[0]);
                            Write("Service started", ConsoleColor.Green);
                            break;

                        case ConsoleKey.E:
                            OnStop();
                            Write("Service stopped", ConsoleColor.Green);
                            break;

                        case ConsoleKey.P:
                            OnPause();
                            Write("Service paused", ConsoleColor.Green);
                            break;

                        case ConsoleKey.R:
                            OnContinue();
                            Write("Service resume", ConsoleColor.Green);
                            break;

                        case ConsoleKey.K:
                            Write("Close console");
                            isClosed = true;
                            break;

                        default:
                            ConsoleMenu();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Write(new[] { "Error with key : " + key, ex.Message }, ConsoleColor.Red);
                    Console.Error.WriteLine(ex.Message);
                }
            }
            
            OnStop();
        }

        #endregion
    }
}
