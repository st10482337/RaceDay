using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Models;

namespace RaceDayAPI.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserID);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // USER -> USER_PROFILE (1:1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProfile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserID);

            // USER/ORGANISER -> EVENT (1:M)
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser)
                .WithMany(u => u.Events)
                .HasForeignKey(e => e.OrganiserID)
                .OnDelete(DeleteBehavior.Restrict);

            // EVENT -> CATEGORY (1:M)
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventID);

            // PARTICIPANT/USER -> ENROLMENT (1:M)
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Participant)
                .WithMany(u => u.Enrolments)
                .HasForeignKey(e => e.ParticipantID)
                .OnDelete(DeleteBehavior.Restrict);

            // EVENT -> ENROLMENT (1:M)
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Event)
                .WithMany(ev => ev.Enrolments)
                .HasForeignKey(e => e.EventID)
                .OnDelete(DeleteBehavior.Restrict);

            // CATEGORY -> ENROLMENT (1:M)
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Enrolments)
                .HasForeignKey(e => e.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent same participant entering same event twice
            modelBuilder.Entity<Enrolment>()
                .HasIndex(e => new { e.ParticipantID, e.EventID })
                .IsUnique();

            // ENROLMENT -> RESULT (1:0..1)
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Result)
                .WithOne(r => r.Enrolment)
                .HasForeignKey<Result>(r => r.EnrolmentID);

            // Restrict valid roles
            modelBuilder.Entity<User>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_User_Role",
                    "[Role] IN ('Organiser', 'Participant')"));

            // Restrict event types
            modelBuilder.Entity<Event>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Event_Type",
                    "[EventType] IN ('Run', 'Walk', 'Cycle')"));

            // Restrict category types
            modelBuilder.Entity<Category>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Category_Type",
                    "[CategoryType] IN ('Age', 'Distance')"));

            // Restrict enrolment status
            modelBuilder.Entity<Enrolment>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Enrolment_Status",
                    "[Status] IN ('Enrolled', 'Cancelled', 'Completed')"));

            // Finishing position must be positive
            modelBuilder.Entity<Result>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_FinishingPosition",
                    "[FinishingPosition] > 0"));
        }
    }
}