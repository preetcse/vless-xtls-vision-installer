using System;
using System.IO;

namespace RoleplayOverhaul.Diagnostics
{
    public static class Logger
    {
        private static string _logFile = "RoleplayOverhaul_Debug.log";

        public static void Log(string message)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(_logFile))
                {
                    sw.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch { }
        }

        public static void Error(string message, Exception ex)
        {
            Log($"[ERROR] {message}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
