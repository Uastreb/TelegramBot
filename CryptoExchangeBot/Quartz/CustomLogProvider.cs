using Quartz.Logging;
using Serilog;
using LogLevel = Quartz.Logging.LogLevel;

namespace CryptoExchangeBot.Quartz
{
    public class CustomLogProvider : ILogProvider
    {
        /// <summary>
        /// Перехватывает ошибки при выполнении джобы и логгирует их.
        /// </summary>
        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) =>
            {
                if (func == null)
                    return true;

                switch (level)
                {
                    case LogLevel.Warn:
                        Log.Warning($"{exception.Message} {exception.StackTrace}");
                        break;
                    case LogLevel.Error:
                        Log.Error($"{exception.Message} {exception.StackTrace}");
                        break;
                    case LogLevel.Fatal:
                        Log.Fatal($"{exception.Message} {exception.StackTrace}");
                        break;
                }

                return true;
            };
        }

        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }
    }
}
