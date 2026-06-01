using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Unyo.Models;

namespace Unyo.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Venue> Venues { get; set; }

    // We do not include DbSet<User> anymore since it is included automatically.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Rule: When an Event is deleted, its Tickets remain (EventId becomes NULL), but the tickets are not deleted.
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Event)
            .WithMany(e => e.Tickets)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.SetNull);

        // Rule: When a User is deleted, their EventRegistrations remain (UserId becomes NULL).
        modelBuilder.Entity<EventRegistration>()
            .HasOne(er => er.User)
            .WithMany(u => u.Registrations)
            .HasForeignKey(er => er.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}