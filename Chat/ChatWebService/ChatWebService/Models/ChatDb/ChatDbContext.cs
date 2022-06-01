using Microsoft.EntityFrameworkCore;

namespace ChatWebService.Models.ChatDb
{
    //TODO: add "Chat" entity for more concise and consistent result in future
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        { }

        public DbSet<Operator> Operators { get; set; }
        public DbSet<Player> Players { get; set; }
        public  DbSet<Message> Messages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().HasAlternateKey(player => player.Email);

            modelBuilder.Entity<Operator>().HasAlternateKey(oper => oper.Email);
        }

    }
}
