using System.Windows.Input;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface ISourceSystemOptionViewModel
    {
        MigrationSourceSystem SourceSystem { get; }

        ICommand Command { get; }
    }
}