using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using StomaDesk.Data;
using StomaDesk.Diagnostics;
using StomaDesk.Forms;
using StomaDesk.Ui;

namespace StomaDesk
{
    internal static class Program
    {
        /// <summary>
        /// StomaDesk.exe                   normal start, data in the PostgreSQL database set in StomaDesk.exe.config
        /// StomaDesk.exe --db "Host=...;Database=...;Username=...;Password=..."   another database, for this run only
        /// StomaDesk.exe --import file.xml loads an XML backup (or the data file of the XML-only version) into an empty database
        /// StomaDesk.exe --selftest [--out folder]   checks without a window; exit code 0 = all passed
        /// StomaDesk.exe --smoke [--out folder]      opens every window and tab once on throwaway demo data
        ///                                           (with --out it also saves a screenshot of each screen)
        /// The self-test and the smoke test work in a temporary schema of the database and drop it at the end,
        /// so they never touch real patients.
        /// </summary>
        [STAThread]
        private static int Main(string[] args)
        {
            if (HasFlag(args, "--selftest"))
            {
                UseUtf8Console();
                return SelfTest.Run(Console.Out, Database(args), OptionValue(args, "--out")) ? 0 : 1;
            }

            string importPath = OptionValue(args, "--import");
            if (importPath != null)
            {
                UseUtf8Console();
                return Import(Database(args), importPath);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ToolStripManager.Renderer = Theme.MenuRenderer;   // menus, context menus and the status bar
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            if (HasFlag(args, "--smoke"))
            {
                UseUtf8Console();
                return SelfTest.RunUi(Console.Out, Database(args), OptionValue(args, "--out")) ? 0 : 1;
            }

            ClinicDatabase database = null;
            ClinicStore store;
            try
            {
                database = Database(args);
                store = ClinicStore.Open(database);
            }
            catch (Exception ex)
            {
                ErrorLog.Write(ex);
                MessageBox.Show(
                    "Baza de date nu poate fi deschisă:\n" + (database == null ? "" : database.Description) + "\n\n" + ex.Message +
                    "\n\nVerificați că serverul PostgreSQL pornește și conexiunea din StomaDesk.exe.config.",
                    "StomaDesk", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }

            Application.Run(new MainForm(store) { WindowState = FormWindowState.Maximized });
            return 0;
        }

        /// <summary>The database from --db when given, otherwise the one in StomaDesk.exe.config.</summary>
        private static ClinicDatabase Database(string[] args)
        {
            string connectionString = OptionValue(args, "--db");
            return connectionString != null ? new ClinicDatabase(connectionString) : ClinicDatabase.FromConfig();
        }

        private static int Import(ClinicDatabase database, string path)
        {
            try
            {
                if (!ClinicStore.Import(database, path))
                {
                    Console.WriteLine("{0} are deja o clinică. Importul se face doar într-o bază de date goală.", database.Description);
                    return 1;
                }
                Console.WriteLine("Datele din {0} au fost importate în {1}.", path, database.Description);
                return 0;
            }
            catch (Exception ex)
            {
                ErrorLog.Write(ex);
                Console.WriteLine("Importul nu a reușit: " + ex.Message);
                return 1;
            }
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
