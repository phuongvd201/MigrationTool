using System.Windows.Input;

using Property;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface IMigrationProgressViewModel
    {
        IProperty<IEntityProgressViewModel[]> EntitiesProgress { get; }

        IProperty<bool> MigrationResult { get; }

        IProperty<ICommand> ReadyCommand { get; }

        IProperty<ICommand> StartCommand { get; }
    }
}