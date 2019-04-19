using System.Linq;
using System.Windows.Input;

using MigrationTool.Services.Entities;
using MigrationTool.Services.Interfaces;
using MigrationTool.ViewModels.Interfaces;

using Property;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.ViewModels
{
    internal class MigrationProgressViewModel : IMigrationProgressViewModel
    {
        public MigrationProgressViewModel(bool testMode = false)
        {
            localEntitiesProgress = Reloadable<IEntityProgressViewModel[]>
                .On().Each().Call(x =>
                    SelectedOptionsSettingsService.SelectedMigrationEntities
                        .Select(o => new EntityProgressViewModel(o))
                        .Cast<IEntityProgressViewModel>().ToArray())
                .Create();

            localMigrationResult = Reloadable<bool>
                .On().Worker().Each().Call(x =>
                    MigrationService.Migrate(UpdateMigrationProgress))
                .Create();

            ReadyCommand = Reloadable<ICommand>
                .On().First().Get(x => new DelegateCommand(o =>
                    ApplicationState.DoStateAction(ApplicationStateAction.Back)))
                .Create();

            StartCommand = Reloadable<ICommand>
                .On().First().Get(x => new DelegateCommand(o =>
                    localEntitiesProgress.Go()))
                .Create();

            localEntitiesProgress.OnChanged(() => localMigrationResult);

            localMigrationResult.OnChanged(() =>
                ApplicationState.DoStateAction(ApplicationStateAction.Next));
        }

        private readonly ICallProperty<IEntityProgressViewModel[]> localEntitiesProgress;

        private readonly ICallProperty<bool> localMigrationResult;

        public IMigrationService MigrationService { get; set; }

        public IApplicationStateViewModel ApplicationState { get; set; }

        public ISelectedOptionsSettingsService SelectedOptionsSettingsService { get; set; }

        private void UpdateMigrationProgress(MigrationEntity migrationEntity, MigrationStage migrationStage)
        {
            var progress = localEntitiesProgress.Value.First(x => x.Entity == migrationEntity);

            switch (migrationStage)
            {
                case MigrationStage.Preparation:
                    progress.EntitiesCount++;
                    progress.Running.Input = true;

                    progress.Status.Input = progress.EntitiesCount + " entities processed.";
                    break;

                case MigrationStage.BatchPrepared:
                    progress.PreparedBatchesCount++;
                    progress.Running.Input = true;
                    progress.Total.Input = 1;
                    progress.Progress.Input = 0;
                    progress.Status.Input = progress.PreparedBatchesCount + " batches prepared.";
                    break;

                case MigrationStage.BatchProcessed:
                    progress.SentBatchesCount++;
                    progress.Total.Input = progress.PreparedBatchesCount;
                    progress.Progress.Input = progress.SentBatchesCount;
                    progress.Status.Input =
                        progress.SentBatchesCount != progress.PreparedBatchesCount
                            ? progress.SentBatchesCount + " batches sent."
                            : progress.EntitiesCount == 0 ?
                                "Completed. No items sent."
                                : "Completed. " + progress.EntitiesCount + " sent.";
                    break;

                case MigrationStage.PreparationReady:
                    progress.Running.Input = false;
                    break;

                case MigrationStage.Error:
                    progress.Running.Input = false;
                    progress.Total.Input = 1;
                    progress.Progress.Input = 0;
                    progress.Status.Input = "An error occured. See log file.";
                    break;

                case MigrationStage.Skipping:
                    progress.Running.Input = true;
                    progress.Total.Input = 1;
                    progress.Progress.Input = 0;
                    progress.Status.Input = "Skipping entities.";
                    break;
            }
        }

        public IProperty<IEntityProgressViewModel[]> EntitiesProgress
        {
            get { return localEntitiesProgress; }
        }

        public IProperty<bool> MigrationResult
        {
            get { return localMigrationResult; }
        }

        public IProperty<ICommand> ReadyCommand { get; private set; }

        public IProperty<ICommand> StartCommand { get; private set; }
    }
}