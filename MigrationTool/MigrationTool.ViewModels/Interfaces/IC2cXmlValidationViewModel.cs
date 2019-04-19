using System.Windows.Input;

using Property;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface IC2cXmlValidationViewModel
    {
        IProperty<string> ValidationResult { get; }

        IProperty<ICommand> StartValidationCommand { get; }

        IProperty<ICommand> BackCommand { get; }
    }
}