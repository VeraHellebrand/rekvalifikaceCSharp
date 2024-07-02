using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace RekvalifikaceApp.Models
{
    public class RekvalifikaceDbContext : IdentityDbContext<AppUser>
    {
        public RekvalifikaceDbContext(DbContextOptions<RekvalifikaceDbContext> options) : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
    }

}

