using Microsoft.EntityFrameworkCore;
using Navislamia.Game.Models.Telecaster;

namespace Navislamia.Game.Contexts;

public class TelecasterContext : DbContext
{
    public TelecasterContext(DbContextOptions<TelecasterContext> options) : base(options) { }
    
    public DbSet<ItemEntity> Items { get; set; }
    public DbSet<AuctionEntity> Auctions { get; set; }
    public DbSet<ItemStorageEntity> ItemStorages { get; set; }
    public DbSet<GuildEntity> Guilds { get; set; }
    public DbSet<SummonEntity> Summons { get; set; }
    public DbSet<AllianceEntity> Alliances { get; set; }
    public DbSet<PetEntity> Pets { get; set; }
    public DbSet<PartyEntity> Parties { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureAuctions(modelBuilder);
        ConfigureCharacters(modelBuilder);
        ConfigureItems(modelBuilder);
        ConfigureItemStorages(modelBuilder);
        ConfigureSummons(modelBuilder);
        ConfigureParties(modelBuilder);
        ConfigureGuilds(modelBuilder);
    }
    
    private static void ConfigureItems(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemEntity>()
            .HasOne(c => c.Summon)
            .WithOne(i => i.CardItem)
            .HasForeignKey<SummonEntity>(s => s.CardItemId);
        
        modelBuilder.Entity<ItemEntity>()
            .HasOne(c => c.Auction)
            .WithOne(a => a.Item)
            .HasForeignKey<AuctionEntity>(a => a.ItemId);
        
        modelBuilder.Entity<ItemEntity>()
            .HasOne(c => c.ItemStorageEntity)
            .WithOne(a => a.Item)
            .HasForeignKey<ItemStorageEntity>(a => a.ItemId);
    }
    
    private static void ConfigureCharacters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CharacterEntity>()
            .HasMany(c => c.Items)
            .WithOne(i => i.Character)
            .HasForeignKey(i => i.CharacterId);
        
        modelBuilder.Entity<CharacterEntity>()
            .HasOne(c => c.Party)
            .WithMany(i => i.PartyMembers)
            .HasForeignKey(c => c.PartyId);
        
        modelBuilder.Entity<CharacterEntity>()
            .HasOne(c => c.Party)
            .WithOne(i => i.Leader)
            .HasForeignKey<PartyEntity>(c => c.LeaderId);
        
        modelBuilder.Entity<CharacterEntity>()
            .HasOne(c => c.Guild)
            .WithMany(g => g.Members)
            .HasForeignKey(c => c.GuildId);
        
        modelBuilder.Entity<CharacterEntity>()
            .HasOne(c => c.PreviousGuild)
            .WithMany(g => g.PreviousMembers)
            .HasForeignKey(c => c.PreviousGuildId);
        
        modelBuilder.Entity<CharacterEntity>()
            .HasOne(c => c.MainSummon)
            .WithOne(g => g.Character)
            .HasForeignKey<CharacterEntity>(c => c.MainSummonId);
        
        modelBuilder.Entity<CharacterEntity>()
            .HasOne(c => c.SubSummon)
            .WithOne(g => g.Character)
            .HasForeignKey<CharacterEntity>(c => c.SubSummonId);
        
        modelBuilder.Entity<CharacterEntity>()
            .HasOne(c => c.Pet)
            .WithOne(g => g.Character)
            .HasForeignKey<CharacterEntity>(c => c.PetId);
    }
    
    private static void ConfigureAuctions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuctionEntity>()
            .HasOne(a => a.Seller)
            .WithMany(c => c.Auctions)
            .HasForeignKey(a => a.SellerId);
        
        modelBuilder.Entity<AuctionEntity>()
            .HasOne(a => a.HighestBidder)
            .WithMany(c => c.Auctions)
            .HasForeignKey(a => a.HighestBidderId);
    }
    
    private static void ConfigureItemStorages(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemStorageEntity>()
            .HasOne(s => s.Character)
            .WithOne(c => c.ItemStorage)
            .HasForeignKey<ItemStorageEntity>(s => s.CharacterId);
        
        modelBuilder.Entity<ItemStorageEntity>()
            .HasOne(s => s.RelatedAuction)
            .WithOne(a => a.ItemStorage)
            .HasForeignKey<ItemStorageEntity>(s => s.RelatedAuctionId);
    }
    
    private static void ConfigureSummons(ModelBuilder modelBuilder)
    {

    }
    
    private static void ConfigureParties(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PartyEntity>()
            .HasOne(s => s.LeadParty)
            .WithMany(a => a.RaidParties)
            .HasForeignKey(s => s.LeadPartyId);
    }
    
    private static void ConfigureGuilds(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GuildEntity>()
            .HasOne(s => s.Alliance)
            .WithOne(a => a.LeadGuild)
            .HasForeignKey<GuildEntity>(g => g.AllianceId);
        
        modelBuilder.Entity<GuildEntity>()
            .HasOne(s => s.Dungeon)
            .WithOne(a => a.OwnerGuild)
            .HasForeignKey<GuildEntity>(g => g.DungeonId);
        
        modelBuilder.Entity<GuildEntity>()
            .HasOne(s => s.Dungeon)
            .WithOne(a => a.OwnerGuild)
            .HasForeignKey<GuildEntity>(g => g.DungeonId);
        
        modelBuilder.Entity<GuildEntity>()
            .HasOne(s => s.Leader)
            .WithOne(a => a.Guild)
            .HasForeignKey<GuildEntity>(g => g.LeaderId);
        
        modelBuilder.Entity<GuildEntity>()
            .HasOne(s => s.RaidLeader)
            .WithOne(a => a.Guild)
            .HasForeignKey<GuildEntity>(g => g.RaidLeaderId);
    }
}