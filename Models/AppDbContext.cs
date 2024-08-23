using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FootBallShop.Models;

namespace FootBallShop.Models
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Jerseys> Jersey { get; set; }
        public DbSet<Leagues> League { get; set; }
        public DbSet<Clubs> Club { get; set; }
        public DbSet<InterLeagues> InterLeague { get; set; }
        public DbSet<Nations> Nation { get; set; }
        public DbSet<Cart> CartItems { get; set; }
        public DbSet<Wishlist> WishlistItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Add this to ensure Identity tables are created

            // Cascade delete on Nation
            modelBuilder.Entity<Jerseys>()
                .HasOne(j => j.Nation)
                .WithMany(n => n.Jerseys)
                .HasForeignKey(j => j.NationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cascade delete on Club
            modelBuilder.Entity<Jerseys>()
                .HasOne(j => j.Club)
                .WithMany(c => c.Jersey)
                .HasForeignKey(j => j.ClubId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restrict delete on League
            modelBuilder.Entity<Jerseys>()
                .HasOne(j => j.League)
                .WithMany(l => l.Jersey)
                .HasForeignKey(j => j.LeagueId)
                .OnDelete(DeleteBehavior.Restrict);

            // Restrict delete on InterLeague
            modelBuilder.Entity<Jerseys>()
                .HasOne(j => j.InterLeague)
                .WithMany(il => il.Jersey)
                .HasForeignKey(j => j.interLeaguesId)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
