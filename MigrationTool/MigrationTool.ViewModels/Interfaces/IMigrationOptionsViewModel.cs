using System;
using System.Windows.Input;

using Property;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface IMigrationOptionsViewModel
    {
        IProperty<string[]> PreviousSourceNames { get; }

        IValidationInputProperty<string> SelectedSourceName { get; }

        IValidationInputProperty<IMigrationEntityOptionViewModel[]> MigrationEntityOptions { get; }

        IInputProperty<DateTime> MigrationStartDateTime { get; }

        IInputProperty<bool> SelectAll { get; }

        IInputProperty<bool> ParallelReadOption { get; }

        IProperty<bool> Loading { get; }

        IProperty<ICommand> RefreshEntityOptionsCommand { get; }

        IProperty<ICommand> BackCommand { get; }

        IProperty<ICommand> NextCommand { get; }
    }
}