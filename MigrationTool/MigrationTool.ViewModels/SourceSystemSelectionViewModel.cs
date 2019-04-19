using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using MigrationTool.Services.Interfaces;
using MigrationTool.ViewModels.Interfaces;

using Property;

namespace MigrationTool.ViewModels
{
    internal class SourceSystemSelectionViewModel : ISourceSystemSelectionViewModel
    {
        public SourceSystemSelectionViewModel()
        {
            SourceSystemOptions = Reloadable<IEnumerable<ISourceSystemOptionViewModel>>
                .On().First().Get(x =>
                    SupportedEntitiesInfoProvider.SupportedSourceSystems
                        .Select(sourceSystem => new SourceSystemOptionViewModel(
                            sourceSystem,
                            new DelegateCommand(o =>
                            {
                                SelectedOptionsSettingsService.SelectedMigrationSourceSystem = sourceSystem;
                                ApplicationState.DoStateAction(ApplicationStateAction.Next, sourceSystem);
                            })))).Create();

            SkipIfSingleCommand = Reloadable<ICommand>
                .On().First().Get(x => new DelegateCommand(o =>
                {
                    if (SourceSystemOptions.Value.Take(2).Count() == 1)
                    {
                        SourceSystemOptions.Value.First().Command.Execute(null);
                    }
                }))
                .Create();
        }

        public IApplicationStateViewModel ApplicationState { get; set; }

        public ISelectedOptionsSettingsService SelectedOptionsSettingsService { get; set; }

        public ISupportedEntitiesInfoProvider SupportedEntitiesInfoProvider { get; set; }

        public IProperty<ICommand> SkipIfSingleCommand { get; private set; }

        public IProperty<IEnumerable<ISourceSystemOptionViewModel>> SourceSystemOptions { get; private set; }
    }
}