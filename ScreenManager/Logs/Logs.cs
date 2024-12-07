using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenManager.Logs
{
    public static class Logs
    {
        public static void LogError(Exception ex)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(documentsPath + "\\Screen Manager"))
            {
                Directory.CreateDirectory(documentsPath + "\\Screen Manager");
            }
            string logFilePath = Path.Combine(documentsPath + "\\Screen Manager", "error_log.txt");

            // Prepare the log message
            string logMessage = $"[{DateTime.Now}] {ex.GetType()}: {ex.Message}\n{ex.StackTrace}\n\n";

            // Write the log message to the file
            File.AppendAllText(logFilePath, logMessage);
        }
    }
}
