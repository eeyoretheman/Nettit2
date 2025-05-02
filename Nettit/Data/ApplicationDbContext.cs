using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nettit.Data.Entity;

namespace Nettit.Data
{
    public class ApplicationDbContext : IdentityDbContext<NettitUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Chatroom> Chatrooms { get; set; } = default!;
        public DbSet<Message> Messages { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Chatroom>()
                .HasMany(c => c.Users)
                .WithMany(u => u.Chatrooms)
                .UsingEntity(j => j.ToTable("ChatroomNettitUser"));
        }
    }
}