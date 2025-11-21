namespace BokunAdapterWebAPI.Models
{
    public class BokunTour
    {
        public string title { get; set; }
        public string code { get; set; } = string.Empty;
        public decimal price { get; set; }
        public string description { get; set; }
        public string? duration { get; set; }
        public string? location { get; set; }
        public int? maxParticipants { get; set; }
        public string? difficultyLevel { get; set; }
        public List<string>? includes { get; set; }
        public List<string>? excludes { get; set; }
        public string? meetingPoint { get; set; }
        public string? cancellationPolicy { get; set; }
        public string? externalUrl { get; set; }
        public bool isActive { get; set; } = true;
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
