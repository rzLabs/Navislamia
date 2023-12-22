using Microsoft.EntityFrameworkCore;
using MigrateDatabase.MssqlEntities.Arcadia;

namespace MigrateDatabase.MigrationContexts;

public class MssqlArcadiaContext : DbContext
{
    public MssqlArcadiaContext(DbContextOptions<MssqlArcadiaContext> options) : base(options){ }
    
    public DbSet<MSSQLItemResource> ItemResource { get; set; }
    public DbSet<MSSQLLevelResource> LevelResource { get; set; }
    public DbSet<MSSQLStringResource> StringResource { get; set; }
    public DbSet<MSSQLStatResource> StatResource { get; set; }
    public DbSet<MSSQLChannelResource> ChannelResource { get; set; }
    public DbSet<MSSQLGlobalVariable> GlobalVariable { get; set; }
    public DbSet<MSSQLEffectResource> EffectResource { get; set; }
    public DbSet<MSSQLItemEffectResource> ItemEffectResource { get; set; }
    public DbSet<MSSQLSummonResource> SummonResource { get; set; }
    public DbSet<MSSQLSetItemEffectResource> SetItemEffectResource { get; set; }
    public DbSet<MSSQLEnhanceResource> EnhanceResource { get; set; }
    public DbSet<MSSQLSkillResource> SkillResource { get; set; }
    public DbSet<MSSQLStateResource> StateResource { get; set; }
    
}