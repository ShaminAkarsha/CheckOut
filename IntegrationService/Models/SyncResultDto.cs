namespace IntegrationService.Models.DTOs
{
    public class SyncResultDto
    {
        public int ProductsSynced { get; set; }
        public List<string> FailedAdapters { get; set; } = new();
    }
}
