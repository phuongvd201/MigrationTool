using System.Windows.Input;

using Property;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface IMedicalDirectorOptionsViewModel
    {
        IInputProperty<string> DocumentsPath { get; }

        IInputProperty<string> DatabaseServerName { get; }

        IProperty<string[]> Errors { get; }

        IProperty<ICommand> ReadConfigurationCommand { get; }

        IProperty<ICommand> BackCommand { get; }

        IProperty<ICommand> NextCommand { get; }
    }
}