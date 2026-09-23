using System.Windows.Forms;

namespace StomaDesk.Ui
{
    public static class Dialogs
    {
        private const string Caption = "StomaDesk";

        public static void Info(IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, Caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Error(IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static bool Confirm(IWin32Window owner, string message)
        {
            return MessageBox.Show(owner, message, Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}
