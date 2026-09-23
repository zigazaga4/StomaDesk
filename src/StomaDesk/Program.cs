using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using StomaDesk.Data;
using StomaDesk.Diagnostics;
using StomaDesk.Forms;

namespace StomaDesk
{
    internal static class Program
    {
        /// <summary>
        /// StomaDesk.exe                   normal start, data in %APPDATA%\StomaDesk\clinic.xml
        /// StomaDesk.exe --data file.xml   use another data file
        /// StomaDesk.exe --selftest [--out folder]   checks without a window; exit code 0 = all passed
        /// StomaDesk.exe --smoke [--out folder]      opens every window and tab once on a throwaway copy of the demo data
        ///                                           (with --out it also saves a screenshot of each screen)
        /// </summary>
        [STAThread]
        private static int Main(string[] args)
        {
            if (HasFlag(args, "--selftest"))
            {
                UseUtf8Console();
                return SelfTest.Run(Console.Out, OptionValue(args, "--out")) ? 0 : 1;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            if (HasFlag(args, "--smoke"))
            {
                UseUtf8Console();
                return SelfTest.RunUi(Console.Out, OptionValue(args, "--out")) ? 0 : 1;
            }

            string path = OptionValue(args, "--data") ?? ClinicStore.DefaultPath;
            ClinicStore store;
            try
            {
                store = ClinicStore.Open(path);
            }
            catch (Exception ex)
            {
                ErrorLog.Write(ex);
                MessageBox.Show("Fișierul de date nu poate fi deschis:\n" + path + "\n\n" + ex.Message,
                    "StomaDesk", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }

            Application.Run(new MainForm(store));
            return 0;
        }

        /// <summary>Last line of defence for exceptions thrown in UI event handlers: log, tell the user, keep running.</summary>
        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ErrorLog.Write(e.Exception);
            MessageBox.Show("A apărut o eroare neașteptată:\n" + e.Exception.Message + "\n\nDetalii în " + ErrorLog.FilePath,
                "StomaDesk", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
                ErrorLog.Write(exception);
        }

        /// <summary>So ș, ț and ă print correctly in the test output (a Windows console defaults to an old code page).</summary>
        private static void UseUtf8Console()
        {
            try
            {
                Console.OutputEncoding = new UTF8Encoding(false);
            }
            catch (IOException)
            {
                // Output redirected to a file or pipe without a console: nothing to change.
            }
        }

        private static bool HasFlag(string[] args, string flag)
        {
            return Array.IndexOf(args, flag) >= 0;
        }

        private static string OptionValue(string[] args, string option)
        {
            int index = Array.IndexOf(args, option);
            return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
        }
    }
}
