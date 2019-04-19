using MigrationTool.ViewModels.Interfaces;

using Property;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.ViewModels
{
    internal class MigrationEntityOptionViewModel : IMigrationEntityOptionViewModel
    {
        public MigrationEntityOptionViewModel(MigrationEntity value)
        {
            Value = value;

            IsChecked = Reloadable<bool>.On().Each().Input().Create();
        }

        public IInputProperty<bool> IsChecked { get; private set; }

        public MigrationEntity Value { get; private set; }
    }
}