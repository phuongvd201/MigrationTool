using System.Windows.Input;

using Property;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface IZedmedOptionsViewModel
    {
        IValidationInputProperty<string> DatabaseFolderPath { get; }

        IInputProperty<string> DocumentsPath { get; }

        IInputProperty<string> Username { get; }

        IProperty<string[]> Errors { get; }

        IProperty<ICommand> ReadConfigurationCommand { get; }

        IProperty<ICommand> BackCommand { get; }

        IProperty<ICommand> NextCommand { get; }
    }
}