using System.Collections.Generic;
using System.Windows.Input;

using Property;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface ISourceSystemSelectionViewModel
    {
        IProperty<IEnumerable<ISourceSystemOptionViewModel>> SourceSystemOptions { get; }

        IProperty<ICommand> SkipIfSingleCommand { get; }
    }
}