using System.Windows.Input;

using Property;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface ILoginViewModel
    {
        IInputProperty<string> Username { get; }

        IProperty<ICommand> LoginCommand { get; }

        IApplicationStateViewModel ApplicationState { get; }

        IProperty<string> LoginResultMessage { get; }
    }
}