using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    /// <summary>
    /// Represents a critical error that requires the application to exit gracefully.
    /// </summary>
    [Serializable]
    public class CriticalError : Exception
    {
        /// <summary>
        /// Exit code to return when application exits due to this error.
        /// </summary>
        public int ExitCode { get; }

        public CriticalError() : base()
        {
            ExitCode = 1;
            GracefulExit();
        }

        public CriticalError(string message, int exitCode = 1) : base(message)
        {
            ExitCode = exitCode;
            GracefulExit();
        }

        public CriticalError(string message, Exception innerException, int exitCode = 1) : base(message, innerException)
        {
            ExitCode = exitCode;
            GracefulExit();
        }

        protected CriticalError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ExitCode = 1; // or deserialize from info if you want
        }

        private void GracefulExit()
        {
            // Implement your graceful exit logic here
            // For example, logging the error, cleanup, then exit:
            Console.WriteLine($"Critical error occurred: {Message}");
            Console.WriteLine("Exiting application gracefully...");
            Environment.Exit(ExitCode);
        }
    }

    /// <summary>
    /// Represents a soft error from which the application can recover.
    /// Allows optional message output, delay, and formatting.
    /// </summary>
    [Serializable]
    public class SoftError : Exception
    {
        public bool OutputMessage { get; }
        public bool BoldMessage { get; }
        public int HeadLines { get; }
        public int TailLines { get; }
        public double DelaySeconds { get; }

        public SoftError(
            string message,
            bool outputMessage = true,
            bool boldMessage = false,
            int headLines = 1,
            int tailLines = 1,
            double delaySeconds = 0
            ) : base(message)
        {
            OutputMessage = outputMessage;
            BoldMessage = boldMessage;
            HeadLines = headLines;
            TailLines = tailLines;
            DelaySeconds = delaySeconds;

            HandleSoftError();
        }

        protected SoftError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            OutputMessage = true;
            BoldMessage = false;
            HeadLines = 1;
            TailLines = 1;
            DelaySeconds = 0;
        }

        private void HandleSoftError()
        {
            if (OutputMessage)
            {
                for (int i = 0; i < HeadLines; i++)
                    Console.WriteLine();

                if (BoldMessage)
                {
                    // Simple console bold simulation (limited in Windows console)
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(Message);

                if (BoldMessage)
                    Console.ResetColor();

                for (int i = 0; i < TailLines; i++)
                    Console.WriteLine();

                if (DelaySeconds > 0)
                {
                    Task.Delay(TimeSpan.FromSeconds(DelaySeconds)).Wait();
                }
            }

            // Otherwise, silently recover; no exit or crash.
        }
    }
}
