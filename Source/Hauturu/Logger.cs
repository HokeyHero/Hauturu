namespace Hauturu
{
    internal class Logger
    {
        private readonly bool _enableLogger;

        public void LogInformation(string message)
        {
            if (_enableLogger)
                Console.WriteLine(message);
        }

        public Logger(bool enableLogger)
        {
            _enableLogger = enableLogger;
        }
    }
}
