using Atracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Atracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<TaskFeedback> TaskFeedbacks { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<MaintenanceLog> MaintenanceLogs { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<SampleData> SampleDatas { get; set; }
        public DbSet<WarehouseSample> WarehouseSamples { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TaskAssignment>()
                .HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskAssignment>()
                .HasOne(t => t.AssignedBy)
                .WithMany()
                .HasForeignKey(t => t.AssignedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskFeedback>()
                .HasOne(f => f.Task)
                .WithMany()
                .HasForeignKey(f => f.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TaskFeedback>()
                .HasOne(f => f.SubmittedBy)
                .WithMany()
                .HasForeignKey(f => f.SubmittedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
