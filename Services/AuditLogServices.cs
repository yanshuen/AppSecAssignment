using AppSecAssignment.Model;
using AppSecAssignment.ViewModels;

namespace AppSecAssignment.Services
{
	public class AuditLogServices
	{
        private readonly AuthDbContext _context;

        public AuditLogServices(AuthDbContext context)
        {
            _context = context;
        }

        public void AddAuditLog(AuditLog auditLog)
        {
            _context.Add(auditLog);
            _context.SaveChanges();
        }
    }
}
