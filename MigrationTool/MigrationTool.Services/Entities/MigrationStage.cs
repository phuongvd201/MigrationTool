namespace MigrationTool.Services.Entities
{
    public enum MigrationStage
    {
        Preparation,
        BatchPrepared,
        PreparationReady,
        BatchProcessed,
        Error,
        Skipping,
    }
}