#nullable disable

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Navislamia.Game.DataAccess.Migrations.Telecaster
{
    /// <inheritdoc />
    public partial class Version0001_TheBeginning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alliances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    LeadGuildId = table.Column<long>(type: "bigint", nullable: false),
                    MaxAllianceCount = table.Column<int>(type: "integer", nullable: false),
                    NameChanged = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alliances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    SellerId = table.Column<long>(type: "bigint", nullable: false),
                    SellerName = table.Column<string>(type: "text", nullable: true),
                    IsHiddenVillageOnly = table.Column<bool>(type: "boolean", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InstantPurchasePrice = table.Column<long>(type: "bigint", nullable: false),
                    RegistrationTax = table.Column<long>(type: "bigint", nullable: false),
                    BiddersIds = table.Column<long[]>(type: "bigint[]", nullable: true),
                    HighestBiddingPrice = table.Column<long>(type: "bigint", nullable: false),
                    HighestBidderId = table.Column<long>(type: "bigint", nullable: false),
                    HighestBidderName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterName = table.Column<string>(type: "text", nullable: true),
                    AccountName = table.Column<string>(type: "text", nullable: true),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    PartyId = table.Column<long>(type: "bigint", nullable: true),
                    GuildId = table.Column<long>(type: "bigint", nullable: true),
                    PreviousGuildId = table.Column<long>(type: "bigint", nullable: true),
                    Slot = table.Column<int>(type: "integer", nullable: false),
                    Permission = table.Column<int>(type: "integer", nullable: false),
                    Position = table.Column<int[]>(type: "integer[]", maxLength: 3, nullable: true),
                    Layer = table.Column<int>(type: "integer", nullable: false),
                    Race = table.Column<int>(type: "integer", nullable: false),
                    Sex = table.Column<int>(type: "integer", nullable: false),
                    Lv = table.Column<int>(type: "integer", nullable: false),
                    MaxReachedLv = table.Column<int>(type: "integer", nullable: false),
                    Exp = table.Column<long>(type: "bigint", nullable: false),
                    LastDecreasedExp = table.Column<long>(type: "bigint", nullable: false),
                    Hp = table.Column<int>(type: "integer", nullable: false),
                    Mp = table.Column<int>(type: "integer", nullable: false),
                    Stamina = table.Column<int>(type: "integer", nullable: false),
                    Havoc = table.Column<int>(type: "integer", nullable: false),
                    CurrentJob = table.Column<int>(type: "integer", nullable: false),
                    PreviousJobs = table.Column<int[]>(type: "integer[]", maxLength: 3, nullable: true),
                    JobDepth = table.Column<short>(type: "smallint", nullable: false),
                    Jlv = table.Column<int>(type: "integer", nullable: false),
                    Jp = table.Column<long>(type: "bigint", nullable: false),
                    TotalJp = table.Column<long>(type: "bigint", nullable: false),
                    TalentPoint = table.Column<int>(type: "integer", nullable: false),
                    JobLvs = table.Column<int[]>(type: "integer[]", maxLength: 3, nullable: true),
                    ImmoralPoint = table.Column<decimal>(type: "numeric", nullable: false),
                    Charisma = table.Column<int>(type: "integer", nullable: false),
                    PkCount = table.Column<int>(type: "integer", nullable: false),
                    DkCount = table.Column<int>(type: "integer", nullable: false),
                    HuntaholicPoint = table.Column<int>(type: "integer", nullable: false),
                    HuntaholicEnterCount = table.Column<int>(type: "integer", nullable: false),
                    EtherealStoneDurability = table.Column<int>(type: "integer", nullable: false),
                    Gold = table.Column<long>(type: "bigint", nullable: false),
                    Chaos = table.Column<int>(type: "integer", nullable: false),
                    SkinColor = table.Column<int>(type: "integer", nullable: false),
                    Models = table.Column<int[]>(type: "integer[]", maxLength: 5, nullable: true),
                    HairColorIndex = table.Column<int>(type: "integer", nullable: false),
                    HairColorRgb = table.Column<int>(type: "integer", nullable: false),
                    HideEquipFlag = table.Column<int>(type: "integer", nullable: false),
                    TextureId = table.Column<int>(type: "integer", nullable: false),
                    BeltItemIds = table.Column<long[]>(type: "bigint[]", maxLength: 6, nullable: true),
                    SummonSlotItemIds = table.Column<long[]>(type: "bigint[]", maxLength: 6, nullable: true),
                    MainSummonId = table.Column<long>(type: "bigint", nullable: true),
                    SubSummonId = table.Column<long>(type: "bigint", nullable: true),
                    RemainSummonTime = table.Column<int>(type: "integer", nullable: false),
                    PetId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LoginTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LoginCount = table.Column<int>(type: "integer", nullable: false),
                    LogoutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlayTime = table.Column<int>(type: "integer", nullable: false),
                    ChatBlockTime = table.Column<int>(type: "integer", nullable: false),
                    AdvChatCount = table.Column<int>(type: "integer", nullable: false),
                    WasNameChanged = table.Column<bool>(type: "boolean", nullable: false),
                    AutoUsed = table.Column<bool>(type: "boolean", nullable: false),
                    GuildBlockTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PkMode = table.Column<bool>(type: "boolean", nullable: false),
                    OtpValue = table.Column<int>(type: "integer", nullable: false),
                    OtpVerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FlagList = table.Column<string[]>(type: "text[]", nullable: true),
                    ClientInfo = table.Column<string[]>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterId = table.Column<long>(type: "bigint", nullable: true),
                    AccountId = table.Column<int>(type: "integer", nullable: true),
                    EquippedBySummonId = table.Column<int>(type: "integer", nullable: true),
                    AuctionId = table.Column<int>(type: "integer", nullable: true),
                    RelatedAuctionId = table.Column<long>(type: "bigint", nullable: true),
                    StorageId = table.Column<int>(type: "integer", nullable: true),
                    Idx = table.Column<int>(type: "integer", nullable: false),
                    ItemResourceId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Enhance = table.Column<int>(type: "integer", nullable: false),
                    EtherealDurability = table.Column<int>(type: "integer", nullable: false),
                    Endurance = table.Column<int>(type: "integer", nullable: false),
                    Flag = table.Column<int>(type: "integer", nullable: false),
                    GenerateBySource = table.Column<int>(type: "integer", nullable: false),
                    WearInfo = table.Column<int>(type: "integer", nullable: false),
                    SocketItemIds = table.Column<long[]>(type: "bigint[]", maxLength: 4, nullable: true),
                    RemainingTime = table.Column<int>(type: "integer", nullable: false),
                    ElementalEffectType = table.Column<int>(type: "integer", nullable: false),
                    ElementalEffectExpireTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ElementalEffectAttackPoint = table.Column<int>(type: "integer", nullable: false),
                    ElementalEffectMagicPoint = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Auctions_RelatedAuctionId",
                        column: x => x.RelatedAuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Items_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Parties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    LeaderId = table.Column<long>(type: "bigint", nullable: false),
                    ShareMode = table.Column<int>(type: "integer", nullable: false),
                    PartyType = table.Column<int>(type: "integer", nullable: false),
                    LeadPartyId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parties_Characters_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Parties_Parties_LeadPartyId",
                        column: x => x.LeadPartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemStorages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StorageType = table.Column<int>(type: "integer", nullable: false),
                    RelatedAuctionId = table.Column<long>(type: "bigint", nullable: false),
                    RelatedItemId = table.Column<long>(type: "bigint", nullable: false),
                    RelatedItemEnhance = table.Column<int>(type: "integer", nullable: false),
                    RelatedItemLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemStorages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemStorages_Auctions_RelatedAuctionId",
                        column: x => x.RelatedAuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemStorages_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemStorages_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemStorages_Items_RelatedItemId",
                        column: x => x.RelatedItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    PetResourceId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    WasNameChanged = table.Column<bool>(type: "boolean", nullable: false),
                    CoolTime = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pets_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Summons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint", nullable: false),
                    SummonResourceId = table.Column<int>(type: "integer", nullable: false),
                    CardItemId = table.Column<long>(type: "bigint", nullable: false),
                    Exp = table.Column<long>(type: "bigint", nullable: false),
                    Jp = table.Column<int>(type: "integer", nullable: false),
                    LastDecreasedExp = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Transform = table.Column<int>(type: "integer", nullable: false),
                    Lv = table.Column<int>(type: "integer", nullable: false),
                    Jlv = table.Column<int>(type: "integer", nullable: false),
                    MaxLevel = table.Column<int>(type: "integer", nullable: false),
                    Fp = table.Column<int>(type: "integer", nullable: false),
                    PreviousLevel = table.Column<int[]>(type: "integer[]", maxLength: 2, nullable: true),
                    PreviousSummonResourceIds = table.Column<long[]>(type: "bigint[]", maxLength: 2, nullable: true),
                    Sp = table.Column<int>(type: "integer", nullable: false),
                    Hp = table.Column<int>(type: "integer", nullable: false),
                    Mp = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Summons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Summons_Items_CardItemId",
                        column: x => x.CardItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dungeons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerGuildId = table.Column<long>(type: "bigint", nullable: true),
                    RaidGuildId = table.Column<long>(type: "bigint", nullable: true),
                    BestRaidTime = table.Column<int>(type: "integer", nullable: false),
                    LastDungeonSiegeFinishTime = table.Column<int>(type: "integer", nullable: false),
                    LastDungeonRaidWrapUpTime = table.Column<int>(type: "integer", nullable: false),
                    TaxRate = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dungeons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Notice = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    IconSize = table.Column<int>(type: "integer", nullable: false),
                    Banner = table.Column<string>(type: "text", nullable: true),
                    BannerSize = table.Column<int>(type: "integer", nullable: false),
                    AdvertiseType = table.Column<int>(type: "integer", nullable: false),
                    AdvertiseEndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdvertiseComment = table.Column<string>(type: "text", nullable: true),
                    Recruiting = table.Column<bool>(type: "boolean", nullable: false),
                    MinRecruitLevel = table.Column<short>(type: "smallint", nullable: false),
                    MaxRecruitLevel = table.Column<short>(type: "smallint", nullable: false),
                    NameChanged = table.Column<bool>(type: "boolean", nullable: false),
                    DungeonId = table.Column<long>(type: "bigint", nullable: false),
                    DungeonBlockTime = table.Column<long>(type: "bigint", nullable: false),
                    Gold = table.Column<long>(type: "bigint", nullable: false),
                    Chaos = table.Column<int>(type: "integer", nullable: false),
                    AllianceId = table.Column<long>(type: "bigint", nullable: false),
                    AllianceBlockTime = table.Column<long>(type: "bigint", nullable: false),
                    DonationPoint = table.Column<int>(type: "integer", nullable: false),
                    PermissionNames = table.Column<string[]>(type: "text[]", maxLength: 6, nullable: true),
                    PermissionSets = table.Column<int[]>(type: "integer[]", maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guilds_Alliances_AllianceId",
                        column: x => x.AllianceId,
                        principalTable: "Alliances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Guilds_Dungeons_DungeonId",
                        column: x => x.DungeonId,
                        principalTable: "Dungeons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_HighestBidderId",
                table: "Auctions",
                column: "HighestBidderId");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_ItemId",
                table: "Auctions",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_SellerId",
                table: "Auctions",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_GuildId",
                table: "Characters",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_MainSummonId",
                table: "Characters",
                column: "MainSummonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_PartyId",
                table: "Characters",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_PetId",
                table: "Characters",
                column: "PetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_PreviousGuildId",
                table: "Characters",
                column: "PreviousGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_SubSummonId",
                table: "Characters",
                column: "SubSummonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dungeons_RaidGuildId",
                table: "Dungeons",
                column: "RaidGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_AllianceId",
                table: "Guilds",
                column: "AllianceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guilds_DungeonId",
                table: "Guilds",
                column: "DungeonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CharacterId",
                table: "Items",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_RelatedAuctionId",
                table: "Items",
                column: "RelatedAuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemStorages_CharacterId",
                table: "ItemStorages",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemStorages_ItemId",
                table: "ItemStorages",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemStorages_RelatedAuctionId",
                table: "ItemStorages",
                column: "RelatedAuctionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemStorages_RelatedItemId",
                table: "ItemStorages",
                column: "RelatedItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parties_LeaderId",
                table: "Parties",
                column: "LeaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parties_LeadPartyId",
                table: "Parties",
                column: "LeadPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_ItemId",
                table: "Pets",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Summons_CardItemId",
                table: "Summons",
                column: "CardItemId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Characters_HighestBidderId",
                table: "Auctions",
                column: "HighestBidderId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Characters_SellerId",
                table: "Auctions",
                column: "SellerId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Items_ItemId",
                table: "Auctions",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Guilds_GuildId",
                table: "Characters",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Guilds_PreviousGuildId",
                table: "Characters",
                column: "PreviousGuildId",
                principalTable: "Guilds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Parties_PartyId",
                table: "Characters",
                column: "PartyId",
                principalTable: "Parties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Pets_PetId",
                table: "Characters",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Summons_MainSummonId",
                table: "Characters",
                column: "MainSummonId",
                principalTable: "Summons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Summons_SubSummonId",
                table: "Characters",
                column: "SubSummonId",
                principalTable: "Summons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dungeons_Guilds_RaidGuildId",
                table: "Dungeons",
                column: "RaidGuildId",
                principalTable: "Guilds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Characters_HighestBidderId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Characters_SellerId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Characters_CharacterId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Parties_Characters_LeaderId",
                table: "Parties");

            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Items_ItemId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Dungeons_Guilds_RaidGuildId",
                table: "Dungeons");

            migrationBuilder.DropTable(
                name: "ItemStorages");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Parties");

            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropTable(
                name: "Summons");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Auctions");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "Alliances");

            migrationBuilder.DropTable(
                name: "Dungeons");
        }
    }
}
