using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SLBFEMS.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUserModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<AffiliationDataModel> AffiliationData { get; set; }
        public DbSet<QualificationsModel> Qualifications { get; set; }
        public DbSet<JobSeekerDataModel> JobSeekerData { get; set; }
        public DbSet<EducationDataModel> EducationData { get; set; }
        public DbSet<ComplaintDataModel> Complaints { get; set; }
        public DbSet<ComplaintMessagesModel> ComplaintMessages { get; set; }
    }
}
