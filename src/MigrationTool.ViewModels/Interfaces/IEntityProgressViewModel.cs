using Property;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.ViewModels.Interfaces
{
    public interface IEntityProgressViewModel
    {
        MigrationEntity Entity { get; }

        int EntitiesCount { get; set; }

        int PreparedBatchesCount { get; set; }

        int SentBatchesCount { get; set; }

        IInputProperty<string> Status { get; }

        IInputProperty<bool> Running { get; }

        IInputProperty<double> Progress { get; }

        IInputProperty<double> Total { get; }
    }
}