using System;
using System.Windows.Forms;

namespace MigrationTool.Controls
{
    public class SelectDirectoryDialog : SelectStringDialog
    {
        protected override string ShowDialog()
        {
            var dlg = new FolderBrowserDialog()
            {
                RootFolder = Environment.SpecialFolder.MyComputer,
            };
            var result = dlg.ShowDialog();

            return result == DialogResult.OK ? dlg.SelectedPath : null;
        }
    }
}