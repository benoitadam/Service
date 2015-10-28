using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAdam.Service
{
    public class ServiceLogger
    {
        public enum LoggerType { INFO, WARN, ERROR }
        public delegate void Log(Type origin, LoggerType type, Exception ex, string message, object[] args);
        public static Log LogMethod = null;
        
        private Type origin;

        public ServiceLogger(Type type)
        {
            origin = type;
        }

        public static ServiceLogger GetLogger(Type type)
        {
            return new ServiceLogger(type);
        }

        public void Error(Exception ex, string message, params object[] args)
        {
            if (LogMethod != null)
                LogMethod(origin, LoggerType.ERROR, ex, message, args);
        }

        public void Info(Exception ex, string message, params object[] args)
        {
            if (LogMethod != null)
                LogMethod(origin, LoggerType.INFO, ex, message, args);
        }

        public void Warn(Exception ex, string message, params object[] args)
        {
            if (LogMethod != null)
                LogMethod(origin, LoggerType.WARN, ex, message, args);
        }
    }
}
