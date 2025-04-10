using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<HL7Message> test2Tbl { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}

