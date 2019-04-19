using MigrationTool.ViewModels.Interfaces;

using Property;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.ViewModels
{
    internal class EntityProgressViewModel : IEntityProgressViewModel
    {
        public EntityProgressViewModel(MigrationEntity entity)
        {
            Entity = entity;

            Running = Reloadable<bool>.On().Each().Input().Create();
            Progress = Reloadable<double>.On().Each().Input().Create();
            Total = Reloadable<double>.On().Each().Input().Create(1);
            Status = Reloadable<string>.On().Each().Input().Create("Pending...");
        }

        public MigrationEntity Entity { get; private set; }

        public int EntitiesCount { get; set; }

        public int PreparedBatchesCount { get; set; }

        public int SentBatchesCount { get; set; }

        public IInputProperty<bool> Running { get; private set; }

        public IInputProperty<string> Status { get; private set; }

        public IInputProperty<double> Progress { get; private set; }

        public IInputProperty<double> Total { get; private set; }
    }
}