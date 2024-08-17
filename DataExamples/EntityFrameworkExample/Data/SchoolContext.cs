using EntityFrameworkExample.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkExample.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options): base(options)
        {
            
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Öğrenci ve kurs arasındaki ilişkiyi tanımlayalım
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Student)
                .WithMany(c => c.Courses)
                .HasForeignKey(c => c.StudentId);
        }
    }
}
