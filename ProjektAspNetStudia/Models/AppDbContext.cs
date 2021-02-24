using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjektAspNetStudia.Models.Database;

namespace ProjektAspNetStudia.Models
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public DbSet<Database.Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ChatUser>()
                .HasKey(cu => new {cu.ChatId, cu.UserId});

            builder.Entity<ChatUser>()
                .HasOne(cu => cu.Chat)
                .WithMany(ch => ch.ChatUsers)
                .HasForeignKey(cu => cu.ChatId);

            builder.Entity<ChatUser>()
                .HasOne(cu => cu.AppUser)
                .WithMany(us => us.ChatsUsers)
                .HasForeignKey(cu => cu.UserId);

            builder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(ch => ch.Messages)
                .HasForeignKey(m => m.ChatId);
        }
    }
}
