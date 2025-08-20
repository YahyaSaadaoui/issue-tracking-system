using IssueDesk.Domain.Abstractions;
using IssueDesk.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IssueDesk.Infrastructure.Persistence;

public class IssueDeskDbContext : DbContext
{
      private readonly IMediator? _mediator;

      public IssueDeskDbContext(DbContextOptions<IssueDeskDbContext> options, IMediator? mediator = null)
          : base(options)
      {
            _mediator = mediator;
      }

      public DbSet<Project> Projects => Set<Project>();
      public DbSet<Ticket> Tickets => Set<Ticket>();
      public DbSet<Comment> Comments => Set<Comment>();

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
            // Project
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

            // Ticket
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

            // Comment
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

      public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
      {
            var result = await base.SaveChangesAsync(cancellationToken);

            // If mediator isn't available (design-time/migrations), skip event publishing
            if (_mediator is null) return result;

            var entitiesWithEvents = ChangeTracker
                .Entries<IHasDomainEvents>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToArray();

            var events = entitiesWithEvents.SelectMany(e => e.DomainEvents).ToList();

            foreach (var entity in entitiesWithEvents)
                  entity.ClearDomainEvents();

            foreach (var domainEvent in events)
                  await _mediator.Publish(domainEvent, cancellationToken);

            return result;
      }
}
