using Microsoft.EntityFrameworkCore;

namespace API;

public class ChatContext : DbContext
{
    public ChatContext(DbContextOptions<ChatContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<ChatMessage> Messages { get; set; } = null!;
}