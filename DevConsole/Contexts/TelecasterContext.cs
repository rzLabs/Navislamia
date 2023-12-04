using Microsoft.EntityFrameworkCore;

namespace Navislamia.Database.Contexts;

public class TelecasterContext : DbContext
{
    public TelecasterContext(DbContextOptions<TelecasterContext> options) : base(options) { }
    
    // These also set the table name - if nothing was set the entities full type name is used
    // public DbSet<ChannelResourceEntity> ChannelResource { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    
        // ConfigureChannelResource(modelBuilder);
    }
    //
    //
    // // private static void ConfigureChannelResource(ModelBuilder modelBuilder)
    // // {
    // // }
}