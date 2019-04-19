using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using MigrationTool.Services.Interfaces;
using MigrationTool.ViewModels.Interfaces;

using Property;
using Property.Windows;

namespace MigrationTool.ViewModels
{
    internal class MigrationOptionsViewModel : IMigrationOptionsViewModel
    {
        public MigrationOptionsViewModel(bool testMode = false)
        {
            ShowValidationResults = Reloadable<bool>.On().Each().Call(x => !x).Create(false);

            ShowValidationResults.OnChanged(() =>
            {
                if (!ShowValidationResults.Value)
                {
                    return;
                }
                MigrationEntityOptions.Validate();
                SelectedSourceName.Validate();
                ShowValidationResults.Go();
            });

            MigrationEntityOptions = Reloadable<IMigrationEntityOptionViewModel[]>
                .On().Each().ValidationInput(x =>
                    ShowValidationResults.Value
                        ? ValidationService.ValidateSettings(
                            SelectedOptionsSettingsService.SelectedMigrationSourceSystem,
                            x.Where(e => e.IsChecked.Value).Select(e => e.Value).ToArray())
                        : Enumerable.Empty<string>())
                .Create(new IMigrationEntityOptionViewModel[] { });

            localMigrationEntityOptions = Reloadable<IMigrationEntityOptionViewModel[]>
                .On().Worker(testMode).Each().Call(x =>
                    GetEntityOptions()).Create();

            localMigrationEntityOptions.OnChanged(() =>
            {
                MigrationEntityOptions.Input = localMigrationEntityOptions.Value;
                localEntitiesLoaded.Go();
                localLoading.Go();
            });

            localPreviousSourceNames = Reloadable<string[]>
                .On().Worker(testMode).Each().Call(x =>
                    MigrationSourceService.GetPreviousSourceNames(
                        SelectedOptionsSettingsService.SelectedMigrationSourceSystem))
                .Create();

            localPreviousSourceNames.OnChanged(() =>
            {
                if (SelectedSourceName.Input == null)
                {
                    SelectedSourceName.Input = localPreviousSourceNames.Value.Length == 1
                        ? localPreviousSourceNames.Value.First()
                        : string.Empty;
                }

                localSourceNamesLoaded.Go();
                localLoading.Go();
            });

            localEntitiesLoaded = Reloadable<bool>
                .On().Each().Call(x =>
                    localMigrationEntityOptions.IsLoading)
                .Create(true);

            localSourceNamesLoaded = Reloadable<bool>
                .On().Each().Call(x =>
                    localPreviousSourceNames.IsLoading)
                .Create(true);

            localLoading = Reloadable<bool>
                .On().Each().Call(x =>
                    localEntitiesLoaded.Value || localSourceNamesLoaded.Value)
                .Create(true);

            SelectedSourceName = Reloadable<string>
                .On().Delayed(TimeSpan.FromMilliseconds(200), testMode)
                .ValidationInput(x =>
                    ShowValidationResults.Value
                        ? GetDataSourceNameErrors(x)
                        : Enumerable.Empty<string>())
                .Create();

            SelectAll = Reloadable<bool>.On().Each().Input().Create();

            ParallelReadOption = Reloadable<bool>.On().Each().Input().Create();

            NextCommand = Reloadable<ICommand>
                .On().First().Get(x => new DelegateCommand(o =>
                {
                    ShowValidationResults.Go();

                    var selectedSourceName = SelectedSourceName.Input;
                    SelectedOptionsSettingsService.SelectedMigrationSourceName = selectedSourceName;

                    var selectedEntities = MigrationEntityOptions.Value.Where(e => e.IsChecked.Value).Select(e => e.Value).ToArray();
                    SelectedOptionsSettingsService.SelectedMigrationEntities = selectedEntities;

                    var selectedDate = MigrationStartDateTime.Value;
                    SelectedOptionsSettingsService.SelectedMigrationDateTime = selectedDate;

                    var selectedParallel = ParallelReadOption.Value;
                    SelectedOptionsSettingsService.SelectedParallelReadOption = selectedParallel;

                    if (SelectedSourceName.HasErrors)
                    {
                        return;
                    }
                    if (MigrationEntityOptions.HasErrors)
                    {
                        return;
                    }

                    ApplicationState.DoStateAction(ApplicationStateAction.Next);
                }))
                .Create();

            RefreshEntityOptionsCommand = Reloadable<ICommand>
                .On().First().Get(x => new DelegateCommand(o =>
                {
                    SelectedSourceName.Input = null;
                    SelectAll.Input = false;
                    localMigrationEntityOptions.Go();
                    localEntitiesLoaded.Go();
                    localLoading.Go();
                    localPreviousSourceNames.Go();
                    localSourceNamesLoaded.Go();
                }))
                .Create();

            BackCommand = Reloadable<ICommand>
                .On().First().Get(x => new DelegateCommand(o =>
                    ApplicationState.DoStateAction(ApplicationStateAction.Back)))
                .Create();

            MigrationStartDateTime = Reloadable<DateTime>.On().Each().Input().Create();

            SelectAll.OnChanged(() =>
            {
                var allSelected = MigrationEntityOptions.Value.All(v => v.IsChecked.Value);
                var selectAll = SelectAll.Value;
                if ((!selectAll && allSelected) || (selectAll && !allSelected))
                {
                    foreach (var o in MigrationEntityOptions.Value)
                    {
                        o.IsChecked.Input = selectAll;
                    }
                }
            });
        }

        private IMigrationEntityOptionViewModel[] GetEntityOptions()
        {
            var results = SupportedEntitiesInfoProvider
                .GetSupportedEntityTypes(SelectedOptionsSettingsService.SelectedMigrationSourceSystem)
                .Select(o =>
                {
                    IMigrationEntityOptionViewModel option = new MigrationEntityOptionViewModel(o);
                    option.IsChecked.OnChanged(() =>
                    {
                        MigrationEntityOptions.Validate();
                        if (option.IsChecked.Value && MigrationEntityOptions.Value.All(v => v.IsChecked.Value))
                        {
                            SelectAll.Input = true;
                        }
                        else if (SelectAll.Value)
                        {
                            SelectAll.Input = false;
                        }
                    });
                    return option;
                }).ToArray();

            return results;
        }

        private IEnumerable<string> GetDataSourceNameErrors(string value)
        {
            var nameValidationResult = ValidationService.ValidateDataSourceName(value);

            if (nameValidationResult.Length > 0)
            {
                return nameValidationResult;
            }
            if (PreviousSourceNames.Value.Contains(value))
            {
                return new string[] { };
            }

            var creationResult = MigrationSourceService.CreateSource(
                SelectedOptionsSettingsService.SelectedMigrationSourceSystem,
                value);

            localPreviousSourceNames.Go();
            localSourceNamesLoaded.Go();
            localLoading.Go();

            return string.IsNullOrWhiteSpace(creationResult)
                ? new string[] { }
                : new[] { creationResult };
        }

        private ICallProperty<bool> ShowValidationResults { get; set; }

        private readonly ICallProperty<bool> localSourceNamesLoaded;

        private readonly ICallProperty<bool> localEntitiesLoaded;

        private readonly ICallProperty<bool> localLoading;

        private readonly ICallProperty<string[]> localPreviousSourceNames;

        private readonly ICallProperty<IMigrationEntityOptionViewModel[]> localMigrationEntityOptions;

        public IApplicationStateViewModel ApplicationState { get; set; }

        public ISupportedEntitiesInfoProvider SupportedEntitiesInfoProvider { get; set; }

        public IMigrationSourceService MigrationSourceService { get; set; }

        public ISelectedOptionsSettingsService SelectedOptionsSettingsService { get; set; }

        public IValidationService ValidationService { get; set; }

        public IInputProperty<bool> SelectAll { get; private set; }

        public IProperty<ICommand> RefreshEntityOptionsCommand { get; private set; }

        public IInputProperty<bool> ParallelReadOption { get; private set; }

        public IValidationInputProperty<string> SelectedSourceName { get; private set; }

        public IInputProperty<DateTime> MigrationStartDateTime { get; private set; }

        public IValidationInputProperty<IMigrationEntityOptionViewModel[]> MigrationEntityOptions { get; private set; }

        public IProperty<bool> Loading
        {
            get { return localLoading; }
        }

        public IProperty<ICommand> BackCommand { get; private set; }

        public IProperty<ICommand> NextCommand { get; private set; }

        public IProperty<string[]> PreviousSourceNames
        {
            get { return localPreviousSourceNames; }
        }
    }
}