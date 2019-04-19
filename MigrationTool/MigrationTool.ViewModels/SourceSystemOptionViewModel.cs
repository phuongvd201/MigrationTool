using System.Windows.Input;

using MigrationTool.ViewModels.Interfaces;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.ViewModels
{
    internal class SourceSystemOptionViewModel : ISourceSystemOptionViewModel
    {
        public SourceSystemOptionViewModel(MigrationSourceSystem sourceSystem, ICommand command)
        {
            SourceSystem = sourceSystem;
            Command = command;
        }

        public MigrationSourceSystem SourceSystem { get; private set; }

        public ICommand Command { get; private set; }
    }
}