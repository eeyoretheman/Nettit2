using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nettit.Data.Entity;

namespace Nettit.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Chatroom> Chatrooms { get; set; } = default!;
    public DbSet<NettitUser> NettitUsers { get; set; } = default!;
   public DbSet<Message> Messages { get; set; } = default!;
}