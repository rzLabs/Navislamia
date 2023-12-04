using DevConsole.Models.Arcadia;
using Microsoft.EntityFrameworkCore;

namespace Navislamia.Database.Contexts;

public class ArcadiaContext : DbContext
{
    public ArcadiaContext(DbContextOptions<ArcadiaContext> options) : base(options) { }

    public DbSet<ChannelResourceEntity> ChannelResources { get; set; }
    public DbSet<GlobalVariableEntity> GlobalVariables { get; set; }
    public DbSet<ItemResourceEntity> ItemResources { get; set; }
    public DbSet<ItemEffectResourceEntity> ItemEffectResources { get; set; }
    public DbSet<StringResourceEntity> StringResources { get; set; }
    public DbSet<SetItemEffectResourceEntity> SetItemEffectResources { get; set; }
    public DbSet<SummonResourceEntity> SummonResources { get; set; }
    public DbSet<EnhanceResourceEntity> EnhanceResources { get; set; }
    public DbSet<EffectResourceEntity> EffectResources { get; set; }
    public DbSet<LevelResourceEntity> LevelResources { get; set; }
    public DbSet<SkillResourceEntity> SkillResources { get; set; }
    public DbSet<StateResourceEntity> StateResources { get; set; }
    public DbSet<StatResourceEntity> StatResources { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        ConfigureGlobalVariable(modelBuilder);
        ConfigureItemEffectResources(modelBuilder);
        ConfigureStringResource(modelBuilder);
        ConfigureSetItemEffectResources(modelBuilder);
        ConfigureSummonResources(modelBuilder);
        ConfigureStateResources(modelBuilder);
        ConfigureSkillResources(modelBuilder);
        ConfigureEffectResources(modelBuilder);
        ConfigureEnhanceResource(modelBuilder);
        ConfigureLevelResource(modelBuilder);
    }

    private static void ConfigureItemEffectResources(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.BaseType).HasMaxLength(2);
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.BaseVar).HasMaxLength(8).HasPrecision(12, 2); // 2 x 4 matrix
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.OptType).HasMaxLength(2);
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.OptVar).HasMaxLength(8).HasPrecision(12, 2);  // 2 x 4 matrix
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.EnchanceId).HasMaxLength(2);
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.EnchanceVar).HasMaxLength(8).HasPrecision(10, 2); // 2 x 4 matrix
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.Range).HasPrecision(10, 2);
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.Weight).HasPrecision(10, 2);
        
    }
    
    private static void ConfigureEnhanceResource(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EnhanceResourceEntity>().Property(i => i.Percentage).HasMaxLength(20).HasPrecision(10,3);
    }


    private static void ConfigureSetItemEffectResources(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<SetItemEffectResourceEntity>().HasKey(s => new { s.SetId, s.SetParts }); // composite key
    }
    
    
    private static void ConfigureLevelResource(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LevelResourceEntity>().HasKey(l => l.Level);
    }
    
    
    private static void ConfigureSkillResources(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.CostHpPer).HasPrecision(10, 2); 
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.CostHpPerSklPer).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.CostMpPer).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.CostMpPerSklPer).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.CostEnergy).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.CostEnergyPerSkl).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.DelayCast).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.DelayCastPerSkl).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.DelayCastModePerEnhance).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.DelayCommon).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.DelayCooltime).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.DelayCooltimePerSkl).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.DelayCooltimeModePerEnhance).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.StateLevelPerSkl).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.StateLevelPerEnhance).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.StateSecond).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.StateSecondPerLevel).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.StateSecondPerEnhance).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.HateMod).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.HatePerSkill).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.HatePerEnhance).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.Var).HasMaxLength(19).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.ProjectileSpeed).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.ProjectileAcceleration).HasPrecision(10, 2);
    }
    
    private static void ConfigureStateResources(ModelBuilder modelBuilder)
    {       
        modelBuilder.Entity<StateResourceEntity>().Property(i => i.AmplifyBase).HasPrecision(13, 3);
        modelBuilder.Entity<StateResourceEntity>().Property(i => i.AmplifyPerSkill).HasPrecision(13, 3);
        modelBuilder.Entity<StateResourceEntity>().Property(i => i.Value).HasMaxLength(20).HasPrecision(13, 3);
        modelBuilder.Entity<StateResourceEntity>().Property(i => i.DuplicateGroup).HasMaxLength(3);
    }
    
    private static void ConfigureGlobalVariable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GlobalVariableEntity>()
            .HasKey(g => g.Name);
    }
    
    private static void ConfigureStringResource(ModelBuilder modelBuilder)
    {
        
    }

    private static void ConfigureSummonResources(ModelBuilder modelBuilder)
    {

    }
    
    private static void ConfigureEffectResources(ModelBuilder modelBuilder)
    {

    }
}
