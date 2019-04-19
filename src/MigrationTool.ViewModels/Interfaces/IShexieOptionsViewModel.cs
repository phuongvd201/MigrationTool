using System.Windows.Input;

using Property;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface IShexieOptionsViewModel
    {
        IInputProperty<string> DatabaseFilePath { get; }

        IProperty<ICommand> BackCommand { get; }

        IProperty<ICommand> NextCommand { get; }
    }
}