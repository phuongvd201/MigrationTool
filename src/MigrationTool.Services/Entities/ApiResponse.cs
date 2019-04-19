namespace MigrationTool.Services.Entities
{
    internal class ApiResponse<T>
    {
        public T Result { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}