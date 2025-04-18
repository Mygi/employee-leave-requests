using Microsoft.EntityFrameworkCore;
using Vypex.CodingChallenge.Domain;
using Vypex.CodingChallenge.Domain.Models;

namespace Vypex.CodingChallenge.Infrastructure.Data
{
    public class CodingChallengeContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; } = default!;

        public CodingChallengeContext()
        {
        }

        public CodingChallengeContext(DbContextOptions<CodingChallengeContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
            .UseSeeding((context, _) =>
            {
                var seeded_employees = FakeEmployeesSeed.Generate(20);
                context.Set<Employee>().AddRange(seeded_employees);
                
                context.SaveChanges();
            });

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Name)
                .HasMaxLength(100);

            // Shouldn't really be able to hard delete an employee with history but we will cascade delete leave
            modelBuilder.Entity<Employee>()
                .HasMany( e => e.AllocatedLeave)
                .WithOne( a => a.Employee)
                .HasForeignKey( a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeLeave>()
                        .HasKey(a => a.Id);

            modelBuilder.Entity<EmployeeLeave>()
                .Property(e => e.EmployeeId)
                .HasMaxLength(100);

            modelBuilder.Entity<EmployeeLeave>()
                .Property(e => e.StartDate)
                .IsRequired()
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            modelBuilder.Entity<EmployeeLeave>()
                .Property(e => e.EndDate)
                .IsRequired()
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            
            modelBuilder.Entity<EmployeeLeave>()
                .Property(e => e.CalculatedLeaveDays)
                .IsRequired()
                .HasDefaultValue(0);

            modelBuilder.Entity<EmployeeLeave>()
                .HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId);
        }
    }
}
