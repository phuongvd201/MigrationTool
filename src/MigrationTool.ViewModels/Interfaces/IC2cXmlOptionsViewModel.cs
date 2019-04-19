using System.Windows.Input;

using Property;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface IC2cXmlOptionsViewModel
    {
        IValidationInputProperty<string> C2cXmlPath { get; }

        IInputProperty<string> C2cDocumentsPath { get; }

        IProperty<ICommand> ReadConfigurationCommand { get; }

        IProperty<ICommand> BackCommand { get; }

        IProperty<ICommand> NextCommand { get; }
    }
}