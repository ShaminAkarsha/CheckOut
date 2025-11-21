using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntegrationService.Models
{
    [Table("Adapter", Schema = "dbo")]
    public class Adapter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("adapter_id")]
        public int AdapterId { get; set; }

        [Column("adapter_name")]
        public string AdapterName { get; set; } = string.Empty;

        [Column("base_url")]
        public string BaseUrl { get; set; } = string.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("api_key")]
        public string? ApiKey { get; set; }

        [Column("custom_headers_json")]
        public string? CustomHeadersJson { get; set; }
    }
}