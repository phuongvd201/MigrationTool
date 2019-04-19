using Property;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface IMigrationEntityOptionViewModel
    {
        IInputProperty<bool> IsChecked { get; }

        MigrationEntity Value { get; }
    }
}