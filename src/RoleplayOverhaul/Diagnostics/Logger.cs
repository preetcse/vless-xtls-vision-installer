using System;
using System.IO;

namespace RoleplayOverhaul.Diagnostics
{
    public static class Logger
    {
        private static string _logFile = "RoleplayOverhaul_Debug.log";
        private static object _lock = new object();

        static Logger()
        {
            // Clear old log on startup
            try { File.WriteAllText(_logFile, $"--- Log Started {DateTime.Now} ---\n"); } catch {}
        }

        public static void Trace(string message)
        {
            // Optional: Verbose logging
            Write($"[TRACE] {message}");
        }

        public static void Info(string message)
        {
            Write($"[INFO] {message}");
        }

        public static void Error(string message, Exception ex)
        {
            Write($"[ERROR] {message}: {ex.Message}\n{ex.StackTrace}");
        }

        private static void Write(string text)
        {
            try
            {
                lock (_lock)
                {
                    using (StreamWriter sw = File.AppendText(_logFile))
                    {
                        sw.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: {text}");
                    }
                }
            }
            catch { }
        }
    }
}
