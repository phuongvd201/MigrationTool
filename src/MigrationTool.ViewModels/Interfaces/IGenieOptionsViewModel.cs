using System.Windows.Input;

using Property;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface IGenieOptionsViewModel
    {
        IInputProperty<string> DocumentsPath { get; }

        IInputProperty<string> XmlExportPath { get; }

        IInputProperty<string> Md3XmlPath { get; }

        IInputProperty<string> Ip { get; }

        IInputProperty<string> Port { get; }

        IInputProperty<string> Username { get; }

        IProperty<string[]> Errors { get; }

        IProperty<ICommand> ReadConfigurationCommand { get; }

        IProperty<ICommand> BackCommand { get; }

        IProperty<ICommand> NextCommand { get; }
    }
}