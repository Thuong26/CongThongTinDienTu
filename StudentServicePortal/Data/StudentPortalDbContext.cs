using Microsoft.EntityFrameworkCore;
using StudentServicePortal.Models;

namespace StudentServicePortal.Data
{
    public class StudentPortalDbContext : DbContext
    {
        public StudentPortalDbContext(DbContextOptions<StudentPortalDbContext> options) : base(options) { }

        public DbSet<StudentLogin> StudentLogins { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    }
}
