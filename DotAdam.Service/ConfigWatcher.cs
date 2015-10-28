using Newtonsoft.Json;
using System;
using System.IO;

namespace SimpleService
{
    /// <summary>
    /// This class get json value and update this automatically when the file change.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConfigWatcher<T>
    {
        private static readonly ServiceLogger _log = ServiceLogger.GetLogger(typeof(ConfigWatcher<T>));
        protected FileSystemWatcher _watcher;
        protected string _path;

        /// <summary>
        /// Get an object where this properties are updated automatically when the file change.
        /// The childs of this object are instantiated one time when their values are defined.
        /// </summary>
        public T Config { get; private set; }

        /// <summary>
        /// When the config file change
        /// </summary>
        public event EventHandler ConfigChanged;

        /// <summary>
        /// Instantiate new config watcher
        /// </summary>
        /// <param name="path"></param>
        public ConfigWatcher(string path)
        {
            _path = path;
                
            try
            {
                var dir = Path.GetDirectoryName(_path);
                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }

                _watcher = new FileSystemWatcher();
                _watcher.Path = dir;
                _watcher.NotifyFilter = NotifyFilters.LastWrite;
                _watcher.Changed += new FileSystemEventHandler(OnChanged);
                _watcher.Filter = Path.GetFileName(_path);
                _watcher.EnableRaisingEvents = true;

                ReadConfig();
            }
            catch (Exception ex) { _log.Error(ex, "Watcher path = {0}", _path); }
        }

        protected virtual void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                ReadConfig();

                if (ConfigChanged != null)
                    ConfigChanged(this, new EventArgs());
            }
            catch (Exception ex) { _log.Error(ex, "On config file change"); }
        }

        protected virtual void ReadConfig()
        {
            try
            {
                if (!File.Exists(_path))
                    WriteDefault();

                string json = File.ReadAllText(_path);

                if(Config == null)
                {
                    Config = JsonConvert.DeserializeObject<T>(json);
                    return;
                }

                JsonConvert.PopulateObject(json, Config);
            }
            catch (Exception ex) { _log.Error(ex, "Read config path = {0}", _path); }
        }

        protected virtual void WriteDefault()
        {
            try
            {
                string dir = Path.GetDirectoryName(_path);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                T config = JsonConvert.DeserializeObject<T>("{}");
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(_path, json);
            }
            catch (Exception ex) { _log.Warn(ex, "Write default config path = {0}", _path); }
        }
    }
}
