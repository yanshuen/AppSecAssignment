using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AppSecAssignment.ViewModels;

namespace AppSecAssignment.Model
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly DbContextOptions _options;

        private readonly IConfiguration _configuration;

        public AuthDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            _options = options;
            _configuration = configuration;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _configuration.GetConnectionString("AuthConnectionString"); optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<AuditLog> AuditLogs { get; set; }
    }
}
