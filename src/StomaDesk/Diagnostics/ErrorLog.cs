using System;
using System.IO;
using StomaDesk.Data;

namespace StomaDesk.Diagnostics
{
    /// <summary>Appends unexpected exceptions to erori.log next to the data file.</summary>
    internal static class ErrorLog
    {
        public static string FilePath
        {
            get { return Path.Combine(Path.GetDirectoryName(ClinicStore.DefaultPath), "erori.log"); }
        }

        public static void Write(Exception exception)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.AppendAllText(FilePath, string.Format("[{0:yyyy.MM.dd HH:mm:ss}] {1}{2}{2}", DateTime.Now, exception, Environment.NewLine));
            }
            catch (IOException)
            {
                // Logging must never become a second failure.
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
