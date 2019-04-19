using Property;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface IApplicationStateViewModel
    {
        IProperty<ApplicationState> State { get; }

        void DoStateAction(ApplicationStateAction action);

        void DoStateAction(ApplicationStateAction action, MigrationSourceSystem optionSourceSystem);
    }
}