using Microsoft.EntityFrameworkCore;

namespace SocialCasino.Models;

public class MyDbContext : DbContext
{
    //Required for migration
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<CreditCard> CreditCards { get; set; }
}