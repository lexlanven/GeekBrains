using Microsoft.EntityFrameworkCore;


namespace UdpChatServer;

public class ApplicationDbContext : DbContext
{
    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=12345678");
    }
}