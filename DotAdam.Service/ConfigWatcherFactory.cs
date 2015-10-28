using System;
using System.Collections.Generic;

namespace SimpleService
{/// <summary>
 /// Get or create ConfigWatcher
 /// </summary>
    public class ConfigWatcherFactory
    {
        private static readonly ServiceLogger _log = ServiceLogger.GetLogger(typeof(ConfigWatcherFactory));
        protected static readonly Dictionary<Type, object> _watchers = new Dictionary<Type, object>();
        protected static readonly Object _lock = new Object();

        /// <summary>
        /// Get singleton ConfigWatcher of this type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ConfigWatcher<T> GetWatcher<T>(string path)
        {
            ConfigWatcher<T> watcher;

            // Find or create ConfigWatcher
            if ((watcher = GetWatcherOrDefault<T>()) == null)
            {
                lock (_lock)
                {
                    if ((watcher = GetWatcherOrDefault<T>()) == null)
                    {
                        watcher = new ConfigWatcher<T>(path);
                        _watchers[typeof(T)] = watcher;
                    }
                }
            }

            return watcher;
        }

        private static ConfigWatcher<T> GetWatcherOrDefault<T>()
        {
            object watcher;
            return (_watchers.TryGetValue(typeof(T), out watcher)) ? watcher as ConfigWatcher<T> : null;
        }
    }
}
