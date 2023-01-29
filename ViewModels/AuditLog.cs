using System.ComponentModel.DataAnnotations;

namespace AppSecAssignment.ViewModels
{
	public class AuditLog
	{
        [Key]
        public string auditId { get; set; } = string.Empty;

        public string userId { get; set; } = string.Empty;

        public string activity { get; set; } = string.Empty;

        public DateTime datetime { get; set; }
    }
}
