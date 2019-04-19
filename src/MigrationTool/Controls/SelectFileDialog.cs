using System;
using System.Windows.Forms;

namespace MigrationTool.Controls
{
    public class SelectFileDialog : SelectStringDialog
    {
        protected override string ShowDialog()
        {
            var dlg = new OpenFileDialog()
            {
                InitialDirectory = Environment.SpecialFolder.MyComputer.ToString(),
            };
            var result = dlg.ShowDialog();

            return result == DialogResult.OK ? dlg.FileName : null;
        }
    }
}