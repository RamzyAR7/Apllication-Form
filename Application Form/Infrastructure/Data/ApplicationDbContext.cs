using Application_Form.Domain.Entities;
using Application_Form.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Application_Form.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ApplicationForm> ApplicationForms { get; set; }
        public DbSet<Client> Clients { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ApplicationFormConfiguration());
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }

}
    
