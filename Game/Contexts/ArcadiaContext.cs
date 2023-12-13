using Microsoft.EntityFrameworkCore;
using Navislamia.Game.Models;
using Navislamia.Game.Models.Arcadia;

namespace Navislamia.Game.Contexts;

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

        ConfigureItemEffectResources(modelBuilder);
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
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.BaseTypes).HasMaxLength(4);
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.BaseValues).HasMaxLength(8).HasPrecision(12, 2); // 4 x 2 matrix
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.OptTypes).HasMaxLength(4);
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.OptValues).HasMaxLength(8).HasPrecision(12, 2);  // 4 x 2 matrix
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.EnhanceIds).HasMaxLength(2);
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.EnhanceValues).HasMaxLength(8).HasPrecision(10, 2); // 4 x 2 matrix
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.Range).HasPrecision(10, 2);
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.Weight).HasPrecision(10, 2);
        modelBuilder.Entity<ItemResourceEntity>().Property(i => i.ThrowRange).HasPrecision(10, 2);
    }
    
    private static void ConfigureEnhanceResource(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EnhanceResourceEntity>().HasKey(s => new { s.Id, s.LocalFlag }); // composite key
        modelBuilder.Entity<EnhanceResourceEntity>().Property(i => i.Percentage).HasMaxLength(20).HasPrecision(10,3);
    }


    private static void ConfigureSetItemEffectResources(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SetItemEffectResourceEntity>().HasKey(s => new { s.SetId, s.SetParts }); // composite key
        modelBuilder.Entity<SetItemEffectResourceEntity>().Property(i => i.BaseTypes).HasMaxLength(2);
        modelBuilder.Entity<SetItemEffectResourceEntity>().Property(i => i.BaseValues).HasMaxLength(8).HasPrecision(12, 2); // 2 x 4 matrix
        modelBuilder.Entity<SetItemEffectResourceEntity>().Property(i => i.OptTypes).HasMaxLength(2);
        modelBuilder.Entity<SetItemEffectResourceEntity>().Property(i => i.OptValues).HasMaxLength(8).HasPrecision(12, 2);  // 2 x 4 matrix
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
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.StateLevelPerSkill).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.StateLevelPerEnhance).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.StateSecond).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.StateSecondPerLevel).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.StateSecondPerEnhance).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.HateMod).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.HatePerSkill).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.HatePerEnhance).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.Values).HasMaxLength(19).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.ProjectileSpeed).HasPrecision(10, 2);
        modelBuilder.Entity<SkillResourceEntity>().Property(i => i.ProjectileAcceleration).HasPrecision(10, 2);
    }
    
    private static void ConfigureStateResources(ModelBuilder modelBuilder)
    {       
        modelBuilder.Entity<StateResourceEntity>().Property(i => i.AmplifyBase).HasPrecision(13, 3);
        modelBuilder.Entity<StateResourceEntity>().Property(i => i.AmplifyPerSkill).HasPrecision(13, 3);
        modelBuilder.Entity<StateResourceEntity>().Property(i => i.Values).HasMaxLength(20).HasPrecision(13, 3);
        modelBuilder.Entity<StateResourceEntity>().Property(i => i.DuplicateGroup).HasMaxLength(3);
    }

    private static void ConfigureSummonResources(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SummonResourceEntity>().Property(i => i.Size).HasPrecision(10, 2);
        modelBuilder.Entity<SummonResourceEntity>().Property(i => i.TargetFxSize).HasPrecision(10, 2);
        modelBuilder.Entity<SummonResourceEntity>().Property(i => i.Scale).HasPrecision(10, 2);
        modelBuilder.Entity<SummonResourceEntity>().Property(i => i.AttackRange).HasPrecision(10, 2);
        modelBuilder.Entity<SummonResourceEntity>().Property(i => i.TargetPosition).HasPrecision(10, 2);
    }
    
    private static void ConfigureEffectResources(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemEffectResourceEntity>().ToTable(i => i
                    .HasCheckConstraint(
                        $"CK_{nameof(ItemEffectResourceEntity)}_{nameof(ItemEffectResourceEntity.Values)}_MaxSize20", 
                        $"cardinality(\"{nameof(ItemEffectResourceEntity.Values)}\") <= 20"))
            .Property(i => i.Values)
            .HasPrecision(12, 2);

    }
}