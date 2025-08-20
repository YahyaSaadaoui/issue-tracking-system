using IssueDesk.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueDesk.Infrastructure.Persistence;

public class IssueDeskDbContext : DbContext
{
      public IssueDeskDbContext(DbContextOptions<IssueDeskDbContext> options) : base(options) { }

      public DbSet<Project> Projects => Set<Project>();
      public DbSet<Ticket> Tickets => Set<Ticket>();
      public DbSet<Comment> Comments => Set<Comment>();

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
            modelBuilder.Entity<Project>(b =>
            {
                  b.ToTable("Projects");
                  b.HasKey(x => x.Id);
                  b.Property(x => x.Name).IsRequired().HasMaxLength(200);
                  b.Property(x => x.Key).IsRequired().HasMaxLength(10);
                  b.Property(x => x.CreatedAt).IsRequired();

                  b.HasMany(x => x.Tickets)
               .WithOne(t => t.Project!)
               .HasForeignKey(t => t.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Ticket>(b =>
            {
                  b.ToTable("Tickets");
                  b.HasKey(x => x.Id);
                  b.Property(x => x.Title).IsRequired().HasMaxLength(200);
                  b.Property(x => x.Description).HasMaxLength(5000);
                  b.Property(x => x.Assignee).HasMaxLength(200);
                  b.Property(x => x.CreatedAt).IsRequired();
                  b.Property(x => x.UpdatedAt).IsRequired();
            });

            modelBuilder.Entity<Comment>(b =>
            {
                  b.ToTable("Comments");
                  b.HasKey(x => x.Id);
                  b.Property(x => x.Author).IsRequired().HasMaxLength(200);
                  b.Property(x => x.Body).IsRequired().HasMaxLength(5000);
                  b.Property(x => x.CreatedAt).IsRequired();

                  b.HasOne(c => c.Ticket!)
               .WithMany(t => t.Comments)
               .HasForeignKey(c => c.TicketId)
               .OnDelete(DeleteBehavior.Cascade);
            });
      }
}
