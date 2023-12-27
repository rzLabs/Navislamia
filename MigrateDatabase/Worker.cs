using System.Reflection.Metadata.Ecma335;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MigrateDatabase.MigrationContexts;
using MigrateDatabase.MssqlEntities.Arcadia;
using Navislamia.Configuration.Options;
using Navislamia.Game.Contexts;
using Navislamia.Game.Models;
using Navislamia.Game.Models.Arcadia;
using Navislamia.Game.Models.Arcadia.Enums;
using Navislamia.Game.Models.Enums;
using Navislamia.Game.Models.Telecaster;
using Serilog;

namespace MigrateDatabase;

public class Worker : BackgroundService
{
    private readonly DbContextOptions<MssqlArcadiaContext> _mssqlOptions;
    private readonly DbContextOptions<ArcadiaContext> _psqlArcadiaContext;

    private readonly DbContextOptions<TelecasterContext> _psqlTelecasterContext;
    private readonly IMapper _mapper;
    
    private readonly List<string> _finishedTransfers = new();
    private readonly List<string> _finishedSeeds = new();
    
    public Worker(DbContextOptions<MssqlArcadiaContext> mssqlOptions, 
        DbContextOptions<ArcadiaContext> psqlArcadiaContext, 
        DbContextOptions<TelecasterContext> psqlTelecasterContext, IMapper mapper)
    {
        _mssqlOptions = mssqlOptions;
        _psqlArcadiaContext = psqlArcadiaContext;
        _psqlTelecasterContext = psqlTelecasterContext;
        _mapper = mapper;
    }
    
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        // Log.Logger.Information("Welcome! What do you want to do?");
        // Log.Logger.Information("1 = Transfer Arcadia, 2 = Seed Testdata");

        // var options = Console.ReadKey();
        // Console.Clear();
        //
        // switch (options.Key)
        // {
        //     case ConsoleKey.D1: case ConsoleKey.NumPad1:
        //         Log.Logger.Information("Transfer Arcadia");
        //         Log.Logger.Information("What do you wish to transfer?");
        //         Log.Logger.Information("1 = Everything, 2 = Choose tables");
        //         
        //         var transferOptions = Console.ReadKey();
        //         Console.Clear();
        //
        //         switch (transferOptions.Key)
        //         {
        //             case ConsoleKey.D1:
        //             case ConsoleKey.NumPad1:
        //                 await TransferArcadia(token, TransferTables.All);
        //                 break;
        //             case ConsoleKey.D2:
        //             case ConsoleKey.NumPad2:
        //                 var selecting = true;
        //                 var selection = TransferTables.Unset;
        //                 Log.Logger.Information("Choose your tables");
        //                 while (selecting)
        //                 {
        //                     Console.Clear();
        //                     Log.Logger.Information("1: {type}" + (selection.HasFlag(TransferTables.ItemResource) ? " • " : ""), "ItemResource");
        //                     Log.Logger.Information("2: {type}", "LevelResource");
        //                     Log.Logger.Information("3: {type}", "StringResource");
        //                     Log.Logger.Information("4: {type}", "StatResource");
        //                     Log.Logger.Information("5: {type}", "ChannelResource");
        //                     Log.Logger.Information("6: {type}", "GlobalVariables");
        //                     Log.Logger.Information("7: {type}", "EffectResource");
        //                     Log.Logger.Information("8: {type}", "ItemEffectResource");
        //                     Log.Logger.Information("9: {type}", "SummonResource");
        //                     Log.Logger.Information("10: {type}", "SetItemEffectResource");
        //                     Log.Logger.Information("11: {type}", "EnhanceResource");
        //                     Log.Logger.Information("12: {type}", "SkillResource");
        //                     Log.Logger.Information("13: {type}", "StateResource");
        //                     Console.WriteLine();
        //                     Log.Logger.Warning("Selected: {selection}", selection);
        //                     Log.Logger.Information("go: {type}", "StartClient transferring");
        //                     Log.Logger.Information("stop: {type}", "Abort");
        //
        //                     var selected = Console.ReadLine();
        //                     switch (selected)
        //                     {
        //                         case "1":
        //                             selection |= TransferTables.ItemResource;
        //                             break;
        //                         case "2":
        //                             selection |= TransferTables.LevelResource;                       
        //                             break;
        //                         case "3":
        //                             selection |= TransferTables.StringResource;
        //                             break;
        //                         case "4":
        //                             selection |= TransferTables.StatResource;
        //                             break;
        //                         case "5":
        //                             selection |= TransferTables.ChannelResource;
        //                             break;
        //                         case "6":
        //                             selection |= TransferTables.GlobaleVariables;
        //                             break;
        //                         case "7":
        //                             selection |= TransferTables.EffectResource;
        //                             break;
        //                         case "8":
        //                             selection |= TransferTables.ItemEffectResource;
        //                             break;
        //                         case "9":
        //                             selection |= TransferTables.SummonResource;
        //                             break;
        //                         case "10":
        //                             selection |= TransferTables.SetItemEffectResource;
        //                             break;
        //                         case "11":
        //                             selection |= TransferTables.EnhanceResource;
        //                             break;
        //                         case "12":
        //                             selection |= TransferTables.SkillResource;
        //                             break;
        //                         case "13":
        //                             selection |= TransferTables.StateResource;
        //                             break;
        //                     }
        //                     
        //                     if (selected == "stop")
        //                     {
        //                         return;
        //                     }
        //
        //                     selecting = selected != "go";
        //                 }
        //
        //                 await TransferArcadia(token, selection);
        //
        //                 break;
        //         }
        //         break;
        //     case ConsoleKey.D2: case ConsoleKey.NumPad2:
        //         await SeedTelecaster(token);
        //         break;
        // }

        await TransferArcadia(token, null);
        await SeedTelecaster(token);
        
        Log.Logger.Warning("Done. You can now close this window.");

    }

    private async Task SeedTelecaster(CancellationToken token)
    {
        await SeedDungeons(token);
        await SeedCharacters(token);
        await SeedItems(token);
    }

    private async Task SeedDungeons(CancellationToken token)
    {
        var entities = new List<DungeonEntity>
        {
            new() { Id = 120700, TaxRate = 5 },
            new() { Id = 121000, TaxRate = 5 },
            new() { Id = 122000, TaxRate = 5 },
            new() { Id = 123000, TaxRate = 5 },
            new() { Id = 130000, TaxRate = 5 },
            new() { Id = 130300, TaxRate = 5 },
            new() { Id = 130400, TaxRate = 5 },
            new() { Id = 130500, TaxRate = 5 },
            new() { Id = 130600, TaxRate = 5 },
            new() { Id = 130700, TaxRate = 5 },
            new() { Id = 130800, TaxRate = 5 },
            new() { Id = 130900, TaxRate = 5 }
        };
        
        await using var context = new TelecasterContext(_psqlTelecasterContext);
        
        Log.Logger.Information("Seeding {type}: {amount}", nameof(DungeonEntity), entities.Count);

        var processed = 1;
        foreach (var entity in entities)
        {
            Log.Information("Processing... {processed}/{amount}", processed, entities.Count);

            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
            
            var existingEntity = context.Dungeons.FirstOrDefault(d => d.Id == entity.Id);
            if (existingEntity != null)
            {
                context.Dungeons.Update(entity);
            }
            else
            {
                context.Dungeons.Add(entity);
            }
                
            await context.SaveChangesAsync(token);
            
            processed++;
            ClearCurrentConsoleLine();
        }
        
        _finishedSeeds.Add(nameof(DungeonEntity));
    }
    
    private async Task SeedCharacters(CancellationToken token)
    {
        var entities = new List<CharacterEntity>
        {
            new()
            {
                Id = 1,
                CharacterName = "Aodai",
                AccountName = "admin",
                AccountId = 1,
                Position = new[] { 153307, 77343, 0 },
                Race = 5,
                Sex = 2,
                Lv = 30,
                MaxReachedLv = 30,
                Exp = 130000,
                Hp = 1253,
                Mp = 1290,
                Stamina = 5000,
                CurrentJob = Job.Strider,
                PreviousJobs = new[] { Job.Stepper },
                Jlv = 50,
                Jp = 500000,
                JobLvs = new[] { 10 },
                HuntaholicEnterCount = 10,
                Gold = 100000000,
                Chaos = 500,
                Models = new[] { 104, 210, 301, 401, 501 },
                HairColorIndex = 2,
                TextureId = 10,
                FlagList = new[] { "rx:168339", "ry:55413", "wbx:168356", "lvup_armor:1", "wby:55399", "lvup_weapon:1" },
                ClientInfo = new[]
                {
                    "QS2=0,2,0", "QS2=1,2,2", "QS2=11,2,1", "QS2=24,2,7", "QS2=25,2,8", "QS2=35,2,28",
                    "KMT=0,0,0,0,192", "KMT=1,0,0,0,73", "KMT=2,0,0,0,83", "KMT=3,0,0,0,67", "KMT=4,0,0,0,89",
                    "KMT=5,0,0,0,69", "KMT=6,0,0,0,82", "KMT=7,0,0,0,70", "KMT=8,0,0,0,71", "KMT=9,0,0,0,80",
                    "KMT=10,0,0,0,81", "KMT=11,0,0,0,77", "KMT=12,0,0,0,84", "KMT=13,0,0,0,72", "KMT=14,0,0,0,90",
                    "KMT=15,0,0,0,79", "KMT=16,0,0,0,88", "KMT=17,0,0,0,86", "KMT=18,0,0,0,78", "KMT=19,0,0,1,115",
                    "KMT=20,0,0,1,70", "KMT=21,0,0,1,72", "KMT=22,0,0,1,219", "KMT=23,0,0,1,221", "KMT=24,0,0,1,80",
                    "KMT=25,0,0,0,9", "KMT=26,0,0,0,32", "KMT=27,0,0,0,49", "KMT=28,0,0,0,50", "KMT=29,0,0,0,51",
                    "KMT=30,0,0,0,52", "KMT=31,0,0,0,53", "KMT=32,0,0,0,54", "KMT=33,0,0,0,55", "KMT=34,0,0,0,56",
                    "KMT=35,0,0,0,57", "KMT=36,0,0,0,48", "KMT=37,0,0,0,189", "KMT=38,0,0,0,187", "KMT=39,0,1,0,49",
                    "KMT=40,0,1,0,50", "KMT=41,0,1,0,51", "KMT=42,0,1,0,52", "KMT=43,0,1,0,53", "KMT=44,0,1,0,54",
                    "KMT=45,0,1,0,55", "KMT=46,0,1,0,56", "KMT=47,0,1,0,57", "KMT=48,0,1,0,48", "KMT=49,0,1,0,189",
                    "KMT=50,0,1,0,187", "KMT=51,0,0,1,49", "KMT=52,0,0,1,50", "KMT=53,0,0,1,51", "KMT=54,0,0,1,52",
                    "KMT=55,0,0,1,53", "KMT=56,0,0,1,54", "KMT=57,0,0,1,55", "KMT=58,0,0,1,56", "KMT=59,0,0,1,57",
                    "KMT=60,0,0,1,48", "KMT=61,0,0,1,189", "KMT=62,0,0,1,187", "KMT=63,0,0,0,49", "KMT=64,0,0,0,50",
                    "KMT=65,0,0,0,51", "KMT=66,0,0,0,52", "KMT=67,0,0,0,53", "KMT=68,0,0,0,54", "KMT=69,0,0,0,55",
                    "KMT=70,0,0,0,56", "KMT=71,0,0,0,57", "KMT=72,0,0,0,48", "KMT=73,0,0,0,189", "KMT=74,0,0,0,220",
                    "KMT=75,0,0,0,0", "KMT=76,0,0,0,0", "KMT=77,0,0,0,0", "KMT=78,0,0,0,0", "KMT=79,0,0,0,0",
                    "KMT=80,0,0,0,0", "KMT=81,0,0,0,0", "KMT=82,0,0,0,0", "KMT=83,0,0,0,0", "KMT=84,0,0,0,0",
                    "KMT=85,0,0,0,0", "KMT=86,0,0,0,0", "KMT=87,0,0,0,0", "KMT=88,0,0,0,0", "KMT=89,0,0,0,0",
                    "KMT=90,0,0,0,0", "KMT=91,0,0,0,0", "KMT=92,0,0,0,0", "KMT=93,0,0,0,0", "KMT=94,0,0,0,0",
                    "KMT=95,0,0,0,0", "KMT=96,0,0,0,0", "KMT=97,0,0,0,0", "KMT=98,0,0,0,0", "KMT=99,0,0,0,0",
                    "KMT=100,0,0,0,0", "KMT=101,0,0,0,0", "KMT=102,0,0,0,0", "KMT=103,0,0,0,0", "KMT=104,0,0,0,0",
                    "KMT=105,0,0,0,0", "KMT=106,0,0,0,0", "KMT=107,0,0,0,0", "KMT=108,0,0,0,0", "KMT=109,0,0,0,0",
                    "KMT=110,0,0,0,0", "KMT=111,0,0,0,0", "KMT=112,0,0,0,0", "KMT=113,0,0,0,0", "KMT=114,0,0,0,0",
                    "KMT=115,0,0,0,0", "KMT=116,0,0,0,0", "KMT=117,0,0,0,0", "KMT=118,0,0,0,0", "KMT=119,0,0,0,0",
                    "KMT=120,0,0,0,0", "KMT=121,0,0,0,0", "KMT=122,0,0,0,0", "KMT=123,0,0,0,66", "KMT=124,0,0,0,68",
                    "KMT=125,0,0,0,85", "KMT=126,0,0,0,74", "KMT=127,0,0,0,75", "KMT=128,0,0,0,76", "ENTERCHATMODE=1",
                    "ENTERCHATMODE2=1", "PREVINSTANCEGAME=0", "CLIENTVER=1"
                }
            },
            new()
            {
                Id = 2,
                CharacterName = "iSmokeDrow",
                AccountName = "admin",
                AccountId = 1,
                Position = new[] { 152456, 76550, 0 },
                Race = 3,
                Sex = 1,
                Lv = 30,
                MaxReachedLv = 30,
                Exp = 130000,
                Hp = 1682,
                Mp = 2086,
                Stamina = 5000,
                CurrentJob = Job.Kahuna,
                PreviousJobs = new[] { Job.Rogue },
                Jlv = 50,
                Jp = 500000,
                JobLvs = new[] { 10 },
                HuntaholicEnterCount = 10,
                Gold = 100000000,
                Chaos = 500,
                Models = new[] { 102, 209, 301, 401, 501 },
                HairColorIndex = 2,
                TextureId = 20,
                FlagList =
                    new[] { "rx:164325", "ry:49528", "wbx:164335", "lvup_armor:1", "wby:49510", "lvup_weapon:1" },
                ClientInfo = new[]
                {
                    "QS2=0,2,0", "QS2=1,2,2", "QS2=11,2,1", "QS2=24,2,7", "QS2=25,2,8", "QS2=35,2,28",
                    "KMT=0,0,0,0,192", "KMT=1,0,0,0,73", "KMT=2,0,0,0,83", "KMT=3,0,0,0,67", "KMT=4,0,0,0,89",
                    "KMT=5,0,0,0,69", "KMT=6,0,0,0,82", "KMT=7,0,0,0,70", "KMT=8,0,0,0,71", "KMT=9,0,0,0,80",
                    "KMT=10,0,0,0,81", "KMT=11,0,0,0,77", "KMT=12,0,0,0,84", "KMT=13,0,0,0,72", "KMT=14,0,0,0,90",
                    "KMT=15,0,0,0,79", "KMT=16,0,0,0,88", "KMT=17,0,0,0,86", "KMT=18,0,0,0,78", "KMT=19,0,0,1,115",
                    "KMT=20,0,0,1,70", "KMT=21,0,0,1,72", "KMT=22,0,0,1,219", "KMT=23,0,0,1,221", "KMT=24,0,0,1,80",
                    "KMT=25,0,0,0,9", "KMT=26,0,0,0,32", "KMT=27,0,0,0,49", "KMT=28,0,0,0,50", "KMT=29,0,0,0,51",
                    "KMT=30,0,0,0,52", "KMT=31,0,0,0,53", "KMT=32,0,0,0,54", "KMT=33,0,0,0,55", "KMT=34,0,0,0,56",
                    "KMT=35,0,0,0,57", "KMT=36,0,0,0,48", "KMT=37,0,0,0,189", "KMT=38,0,0,0,187", "KMT=39,0,1,0,49",
                    "KMT=40,0,1,0,50", "KMT=41,0,1,0,51", "KMT=42,0,1,0,52", "KMT=43,0,1,0,53", "KMT=44,0,1,0,54",
                    "KMT=45,0,1,0,55", "KMT=46,0,1,0,56", "KMT=47,0,1,0,57", "KMT=48,0,1,0,48", "KMT=49,0,1,0,189",
                    "KMT=50,0,1,0,187", "KMT=51,0,0,1,49", "KMT=52,0,0,1,50", "KMT=53,0,0,1,51", "KMT=54,0,0,1,52",
                    "KMT=55,0,0,1,53", "KMT=56,0,0,1,54", "KMT=57,0,0,1,55", "KMT=58,0,0,1,56", "KMT=59,0,0,1,57",
                    "KMT=60,0,0,1,48", "KMT=61,0,0,1,189", "KMT=62,0,0,1,187", "KMT=63,0,0,0,49", "KMT=64,0,0,0,50",
                    "KMT=65,0,0,0,51", "KMT=66,0,0,0,52", "KMT=67,0,0,0,53", "KMT=68,0,0,0,54", "KMT=69,0,0,0,55",
                    "KMT=70,0,0,0,56", "KMT=71,0,0,0,57", "KMT=72,0,0,0,48", "KMT=73,0,0,0,189", "KMT=74,0,0,0,220",
                    "KMT=75,0,0,0,0", "KMT=76,0,0,0,0", "KMT=77,0,0,0,0", "KMT=78,0,0,0,0", "KMT=79,0,0,0,0",
                    "KMT=80,0,0,0,0", "KMT=81,0,0,0,0", "KMT=82,0,0,0,0", "KMT=83,0,0,0,0", "KMT=84,0,0,0,0",
                    "KMT=85,0,0,0,0", "KMT=86,0,0,0,0", "KMT=87,0,0,0,0", "KMT=88,0,0,0,0", "KMT=89,0,0,0,0",
                    "KMT=90,0,0,0,0", "KMT=91,0,0,0,0", "KMT=92,0,0,0,0", "KMT=93,0,0,0,0", "KMT=94,0,0,0,0",
                    "KMT=95,0,0,0,0", "KMT=96,0,0,0,0", "KMT=97,0,0,0,0", "KMT=98,0,0,0,0", "KMT=99,0,0,0,0",
                    "KMT=100,0,0,0,0", "KMT=101,0,0,0,0", "KMT=102,0,0,0,0", "KMT=103,0,0,0,0", "KMT=104,0,0,0,0",
                    "KMT=105,0,0,0,0", "KMT=106,0,0,0,0", "KMT=107,0,0,0,0", "KMT=108,0,0,0,0", "KMT=109,0,0,0,0",
                    "KMT=110,0,0,0,0", "KMT=111,0,0,0,0", "KMT=112,0,0,0,0", "KMT=113,0,0,0,0", "KMT=114,0,0,0,0",
                    "KMT=115,0,0,0,0", "KMT=116,0,0,0,0", "KMT=117,0,0,0,0", "KMT=118,0,0,0,0", "KMT=119,0,0,0,0",
                    "KMT=120,0,0,0,0", "KMT=121,0,0,0,0", "KMT=122,0,0,0,0", "KMT=123,0,0,0,66", "KMT=124,0,0,0,68",
                    "KMT=125,0,0,0,85", "KMT=126,0,0,0,74", "KMT=127,0,0,0,75", "KMT=128,0,0,0,76", "ENTERCHATMODE=1",
                    "ENTERCHATMODE2=1", "PREVINSTANCEGAME=0", "CLIENTVER=1"
                }
            },
            new()
            {
                Id = 3,
                CharacterName = "Nexitis",
                AccountName = "admin",
                AccountId = 1,
                Position = new[] { 153375, 77314, 0 },
                Race = 4,
                Sex = 2,
                Lv = 30,
                MaxReachedLv = 30,
                Exp = 130000,
                Hp = 914,
                Mp = 992,
                Stamina = 5000,
                CurrentJob = Job.Tamer,
                PreviousJobs = new[] { Job.Guide },
                Jlv = 50,
                Jp = 500000,
                JobLvs = new[] { 10 },
                HuntaholicEnterCount = 10,
                Gold = 100000000,
                Chaos = 500,
                Models = new[] { 101, 210, 301, 401, 501 },
                HairColorIndex = 2,
                TextureId = 2,
                FlagList = new[] { "rx:164482", "ry:52951", "wbx:164464", "lvup_armor:1", "wby:52942", "lvup_weapon:1" },
                ClientInfo = new[]
                {
                    "QS2=0,2,0", "QS2=1,2,2", "QS2=11,2,1", "QS2=24,2,7", "QS2=25,2,8", "QS2=35,2,28",
                    "KMT=0,0,0,0,192", "KMT=1,0,0,0,73", "KMT=2,0,0,0,83", "KMT=3,0,0,0,67", "KMT=4,0,0,0,89",
                    "KMT=5,0,0,0,69", "KMT=6,0,0,0,82", "KMT=7,0,0,0,70", "KMT=8,0,0,0,71", "KMT=9,0,0,0,80",
                    "KMT=10,0,0,0,81", "KMT=11,0,0,0,77", "KMT=12,0,0,0,84", "KMT=13,0,0,0,72", "KMT=14,0,0,0,90",
                    "KMT=15,0,0,0,79", "KMT=16,0,0,0,88", "KMT=17,0,0,0,86", "KMT=18,0,0,0,78", "KMT=19,0,0,1,115",
                    "KMT=20,0,0,1,70", "KMT=21,0,0,1,72", "KMT=22,0,0,1,219", "KMT=23,0,0,1,221", "KMT=24,0,0,1,80",
                    "KMT=25,0,0,0,9", "KMT=26,0,0,0,32", "KMT=27,0,0,0,49", "KMT=28,0,0,0,50", "KMT=29,0,0,0,51",
                    "KMT=30,0,0,0,52", "KMT=31,0,0,0,53", "KMT=32,0,0,0,54", "KMT=33,0,0,0,55", "KMT=34,0,0,0,56",
                    "KMT=35,0,0,0,57", "KMT=36,0,0,0,48", "KMT=37,0,0,0,189", "KMT=38,0,0,0,187", "KMT=39,0,1,0,49",
                    "KMT=40,0,1,0,50", "KMT=41,0,1,0,51", "KMT=42,0,1,0,52", "KMT=43,0,1,0,53", "KMT=44,0,1,0,54",
                    "KMT=45,0,1,0,55", "KMT=46,0,1,0,56", "KMT=47,0,1,0,57", "KMT=48,0,1,0,48", "KMT=49,0,1,0,189",
                    "KMT=50,0,1,0,187", "KMT=51,0,0,1,49", "KMT=52,0,0,1,50", "KMT=53,0,0,1,51", "KMT=54,0,0,1,52",
                    "KMT=55,0,0,1,53", "KMT=56,0,0,1,54", "KMT=57,0,0,1,55", "KMT=58,0,0,1,56", "KMT=59,0,0,1,57",
                    "KMT=60,0,0,1,48", "KMT=61,0,0,1,189", "KMT=62,0,0,1,187", "KMT=63,0,0,0,49", "KMT=64,0,0,0,50",
                    "KMT=65,0,0,0,51", "KMT=66,0,0,0,52", "KMT=67,0,0,0,53", "KMT=68,0,0,0,54", "KMT=69,0,0,0,55",
                    "KMT=70,0,0,0,56", "KMT=71,0,0,0,57", "KMT=72,0,0,0,48", "KMT=73,0,0,0,189", "KMT=74,0,0,0,220",
                    "KMT=75,0,0,0,0", "KMT=76,0,0,0,0", "KMT=77,0,0,0,0", "KMT=78,0,0,0,0", "KMT=79,0,0,0,0",
                    "KMT=80,0,0,0,0", "KMT=81,0,0,0,0", "KMT=82,0,0,0,0", "KMT=83,0,0,0,0", "KMT=84,0,0,0,0",
                    "KMT=85,0,0,0,0", "KMT=86,0,0,0,0", "KMT=87,0,0,0,0", "KMT=88,0,0,0,0", "KMT=89,0,0,0,0",
                    "KMT=90,0,0,0,0", "KMT=91,0,0,0,0", "KMT=92,0,0,0,0", "KMT=93,0,0,0,0", "KMT=94,0,0,0,0",
                    "KMT=95,0,0,0,0", "KMT=96,0,0,0,0", "KMT=97,0,0,0,0", "KMT=98,0,0,0,0", "KMT=99,0,0,0,0",
                    "KMT=100,0,0,0,0", "KMT=101,0,0,0,0", "KMT=102,0,0,0,0", "KMT=103,0,0,0,0", "KMT=104,0,0,0,0",
                    "KMT=105,0,0,0,0", "KMT=106,0,0,0,0", "KMT=107,0,0,0,0", "KMT=108,0,0,0,0", "KMT=109,0,0,0,0",
                    "KMT=110,0,0,0,0", "KMT=111,0,0,0,0", "KMT=112,0,0,0,0", "KMT=113,0,0,0,0", "KMT=114,0,0,0,0",
                    "KMT=115,0,0,0,0", "KMT=116,0,0,0,0", "KMT=117,0,0,0,0", "KMT=118,0,0,0,0", "KMT=119,0,0,0,0",
                    "KMT=120,0,0,0,0", "KMT=121,0,0,0,0", "KMT=122,0,0,0,0", "KMT=123,0,0,0,66", "KMT=124,0,0,0,68",
                    "KMT=125,0,0,0,85", "KMT=126,0,0,0,74", "KMT=127,0,0,0,75", "KMT=128,0,0,0,76", "ENTERCHATMODE=1",
                    "ENTERCHATMODE2=1", "PREVINSTANCEGAME=0", "CLIENTVER=1"
                }
            }
        };
        
        await using var context = new TelecasterContext(_psqlTelecasterContext);
        
        Log.Logger.Information("Seeding {type}: {amount}", nameof(CharacterEntity), entities.Count);

        var processed = 1;
        foreach (var entity in entities)
        {
            Log.Information("Processing... {processed}/{amount}", processed, entities.Count);

            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
            
            var existingEntity = context.Characters.FirstOrDefault(d => d.Id == entity.Id);
            if (existingEntity != null)
            {
                context.Characters.Update(entity);
            }
            else
            {
                context.Characters.Add(entity);
            }
                
            await context.SaveChangesAsync(token);
            
            processed++;
            ClearCurrentConsoleLine();
        }
        
        _finishedSeeds.Add(nameof(CharacterEntity));
    }
    
    private async Task SeedItems(CancellationToken token)
    {

        var entities = new List<ItemEntity>();

        var ids = new []
            { 2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34 };
        var characterIds = new[]
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 2 };
        var accountids = new[]
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 };
        var index = new[]
            { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 11 };
        var itemResourceIds = new[]
        {
            103100, 230100, 490001, 3600118, 3600168, 3600172, 3600176, 3600164, 3600094, 3600094, 3600102, 540071,
            112100, 240109, 490001, 3600136, 3600168, 3600172, 3600176, 3600164, 3600189, 540049, 106100, 220109,
            490001, 3600149, 3600168, 3600172, 3600176, 3600164, 3600110, 0, 403101
        };
        var amounts = new[]
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 };
        var levels = new[]
        {
            1, 1, 1, 10, 10, 10, 10, 10, 10, 10, 10, 1, 1, 1, 1, 10, 10, 10, 10, 10, 10, 1, 1, 1, 1, 10, 10, 10, 10, 10,
            10, 0, 1
        };
        var enhanced = new[]
        {
            0, 0, 0, 10, 10, 10, 10, 10, 10, 10, 10, 0, 0, 0, 0, 10, 10, 10, 10, 10, 10, 0, 0, 0, 0, 10, 10, 10, 10, 10,
            10, 0, 0
        };
        var flag = new[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -2147483648, 0, 0, 0, 0, 0, 0, 0, 0, 0, -2147483648, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0
        };
        var gcode = new[]
            { 6, 6, 6, 3, 3, 3, 3, 3, 3, 3, 3, 3, 6, 6, 6, 3, 3, 3, 3, 3, 3, 3, 6, 6, 6, 3, 3, 3, 3, 3, 3, 126, 1 };
        var wearinfos = new[]
        {
            -1, -1, 23, 2, 5, 4, 3, 6, 0, 1, -1, -1, -1, -1, 23, 2, 5, 4, 3, 6, 0, -1, -1, -1, 23, 2, 5, 4, 3, 6, 0, -1,
            8
        };
        var sockets = new long[]
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        var remainTime = new[]
        {
            0, 0, 0, 1703715848, 1703715848, 1703715848, 1703715848, 1703715848, 1703715852, 1703715852, 1703715852, 0,
            0, 0, 0, 1703716142, 1703716142, 1703716142, 1703716142, 1703716142, 1703716146, 0, 0, 0, 0, 1703716425,
            1703716425, 1703716425, 1703716425, 1703716425, 1703716429, 0, 0
        };
        
        for (var i = 0; i <= 32; i++)
        {
            entities.Add(new ItemEntity
            {
                Id = ids[i],
                CharacterId = characterIds[i] == 0 ? null : characterIds[i],
                AccountId = accountids[i],
                Idx = index[i],
                ItemResourceId = itemResourceIds[i],
                Amount = amounts[i],
                Level = (uint)levels[i],
                Enhance = (uint)enhanced[i],
                Flag = (ItemFlag)flag[i],
                GenerateBySource = (ItemGenerateSource)gcode[i],
                WearInfo = (ItemWearType)wearinfos[i],
                SocketItemIds = new [] { sockets[i] },
                RemainingTime = remainTime[i],
                ElementalEffectExpireTime = DateTime.UtcNow.AddYears(-5),
            });
        }
        
        Log.Logger.Information("Seeding {type}: {amount}", nameof(ItemEntity), entities.Count);
        
        var processed = 1;
        foreach (var entity in entities)
        {
            Log.Information("Processing... {processed}/{amount}", processed, entities.Count);
        
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
            
            await using var context = new TelecasterContext(_psqlTelecasterContext);
            var existingEntity = context.Items.FirstOrDefault(d => d.Id == entity.Id);
            if (existingEntity != null)
            {
                context.Items.Update(entity);
            }
            else
            {
                context.Items.Add(entity);
            }
                
            await context.SaveChangesAsync(token);
            
            processed++;
            ClearCurrentConsoleLine();
        }
        
        _finishedSeeds.Add(nameof(ItemEntity));
    }
    
    private async Task TransferArcadia(CancellationToken token, TransferTables? tableChoice)
    {
        await TransferChannelResource(token);
        await TransferLevelResource(token);
        await TransferStringResource(token);
        await TransferEffectResource(token);
        await TransferStatResource(token);
        await TransferStateResource(token);
        await TransferSummonResource(token, false);
        await TransferSkillResource(token);
        await TransferSetItemEffectResource(token);
        await TransferItemResource(token, false);
        await TransferSummonResource(token, true);
        await TransferItemResource(token, true);
        await TransferItemEffectResource(token);
        await TransferEnhanceResource(token);
    }

    private async Task TransferItemResource(CancellationToken token, bool updateRelations)
    {
        await using var context = new MssqlArcadiaContext(_mssqlOptions);
        var items = context.ItemResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount}",nameof(MSSQLItemResource), items.Count);

        if (updateRelations)
        {
            Log.Logger.Information("Reapplying relations for ItemResource");
        }
        
        foreach (var item in items)
        {
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
            Log.Information("Processing... {processed}/{amount}", processed, items.Count);
    
            var mappedItem = _mapper.Map<ItemResourceEntity>(item);
    
            // multi dimensional arrays cannot be instantiated inside the mapper, gotta do it seperatly like down below
            var baseTypes = new[] { item.base_type_0, item.base_type_1, item.base_type_2, item.base_type_3 };
            var baseValues = new[,]
            {
                { item.base_var1_0, item.base_var2_0 }, { item.base_var1_1, item.base_var2_1 },
                { item.base_var1_2, item.base_var2_2 }, { item.base_var1_3, item.base_var2_3 }
            };
    
            var optTypes = new[] { item.opt_type_0, item.opt_type_1, item.opt_type_2, item.opt_type_3 };
            var optValues = new[,]
            {
                { item.opt_var1_0, item.opt_var2_0 }, { item.opt_var1_1, item.opt_var2_1 },
                { item.opt_var1_2, item.opt_var2_2 }, { item.opt_var1_3, item.opt_var2_3 }
            };

            var enhanceIds = new long [] { item.enhance_0_id, item.enhance_1_id };
            var enhanceValues = new[,]
            {
                { item.enhance_0_01, item.enhance_0_02, item.enhance_0_03, item.enhance_0_04 },
                { item.enhance_1_01, item.enhance_1_02, item.enhance_1_03, item.enhance_1_04 },
            };
    
            var itemRaceRestriction = ItemRaceRestriction.None;
            if (item.limit_deva != "0")
            {
                itemRaceRestriction |= ItemRaceRestriction.Deva;
            }
    
            if (item.limit_asura != "0")
            {
                itemRaceRestriction |= ItemRaceRestriction.Asura;
            }
    
            if (item.limit_gaia != "0")
            {
                itemRaceRestriction |= ItemRaceRestriction.Gaia;
            }
    
            var itemJobRestriction = ItemJobRestriction.None;
            if (item.limit_hunter != "0")
            {
                itemJobRestriction |= ItemJobRestriction.Hunter;
            }
    
            if (item.limit_fighter != "0")
            {
                itemJobRestriction |= ItemJobRestriction.Fighter;
            }
    
            if (item.limit_magician != "0")
            {
                itemJobRestriction |= ItemJobRestriction.Magician;
            }
    
            if (item.limit_summoner != "0")
            {
                itemJobRestriction |= ItemJobRestriction.Summoner;
            }
    
            mappedItem.BaseTypes = baseTypes;
            mappedItem.BaseValues = baseValues;
            mappedItem.OptTypes = optTypes;
            mappedItem.OptValues = optValues;
            mappedItem.RaceRestriction = itemRaceRestriction;
            mappedItem.JobRestriction = itemJobRestriction;
            mappedItem.EnhanceValues = enhanceValues;
            mappedItem.EnhanceIds = enhanceIds.Length != 0 ? enhanceIds : null;
    
            await using (var psqlContext = new ArcadiaContext(_psqlArcadiaContext))
            {
                var summonExists = psqlContext.SummonResources.Any(s => s.Id == item.summon_id);
                if (!summonExists)
                {
                    mappedItem.SummonId = null;
                }
                
                var effectExists = psqlContext.EffectResources.Any(s => s.Id == item.effect_id);
                if (!effectExists)
                {
                    mappedItem.EffectId = null;
                }
                
                var skillExists = psqlContext.SkillResources.Any(s => s.Id == item.skill_id);
                if (!skillExists)
                {
                    mappedItem.SkillId = null;
                }
                
                var existingEntity = psqlContext.ItemResources.FirstOrDefault(i => i.Id == item.id);
                if (existingEntity != null)
                {
                    psqlContext.ItemResources.Update(mappedItem);
                }
                else
                {
                    psqlContext.ItemResources.Add(mappedItem);
                }
                
                await psqlContext.SaveChangesAsync(token);
            }
    
            processed++;
            ClearCurrentConsoleLine();
        }
        _finishedTransfers.Add(nameof(MSSQLItemResource));
    }
    
    private async Task TransferLevelResource(CancellationToken token)
    {
        await using var mssqlContext = new MssqlArcadiaContext(_mssqlOptions);

        var items = mssqlContext.LevelResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount}",nameof(MSSQLLevelResource), items.Count);
        
        foreach (var item in items)
        {
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
            Log.Information("Processing... {processed}/{amount}", processed, items.Count);
    
            var mappedItem = _mapper.Map<LevelResourceEntity>(item);
            
    
            await using (var psqlContext = new ArcadiaContext(_psqlArcadiaContext))
            {
                var existingEntity = psqlContext.LevelResources.FirstOrDefault(i => i.Level == item.level);
                if (existingEntity != null)
                {
                    psqlContext.LevelResources.Update(mappedItem);
                }
                else
                {
                    psqlContext.LevelResources.Add(mappedItem);
                }
                
                await psqlContext.SaveChangesAsync(token);
            }
    
            processed++;
            ClearCurrentConsoleLine();
        }
        
        _finishedTransfers.Add(nameof(MSSQLLevelResource));
        UpdateConsole();
    }
    
    private async Task TransferStringResource(CancellationToken token)
    {
        await using var mssqlContext = new MssqlArcadiaContext(_mssqlOptions);

        var items = mssqlContext.StringResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount}",nameof(MSSQLStringResource), items.Count);
        var batches = new List<List<StringResourceEntity>>();
        while(items.Count != 0)
        {
            var it = items.Take(1000);
            var mappedList = _mapper.Map<List<StringResourceEntity>>(it);

            batches.Add(mappedList);
            items.RemoveRange(0, items.Count < 1000 ? items.Count : 1000);
        }

        var runner = 1;
        var tasks = new List<Task>();
        foreach (var batch in batches)
        {
            tasks.Add(Task.Run(() => TransferEntities(batch, runner += 1), token));
        }

        tasks.Add(Task.Run(() =>
        {
            Log.Logger.Information("Waiting for runners to finish");
        }, token));
        await Task.WhenAll(tasks.AsParallel());
        
        _finishedTransfers.Add(nameof(MSSQLStringResource));
        UpdateConsole();
    }

    private async Task TransferEntities<T>(List<T> entities, int? runner = null) where T: class, IEntity
    {
        if (runner != null)
        {
            Log.Logger.Information("Runner: {num}", runner);
        }
        foreach (var entity in entities)
        {
            await using var psqlContext = new ArcadiaContext(_psqlArcadiaContext);
            
            var existingEntity = await psqlContext.Set<T>().FindAsync(entity.Id);
            if (existingEntity != null)
            {
                psqlContext.Set<T>().Update(entity);
            }
            else
            {
                psqlContext.Set<T>().Add(entity);
            }
            await psqlContext.SaveChangesAsync();
        }
    }
    
    private async Task TransferStatResource(CancellationToken token)
    {
        await using var mssqlContext = new MssqlArcadiaContext(_mssqlOptions);

        var items = mssqlContext.StatResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount}",nameof(MSSQLStatResource), items.Count);
        
        foreach (var item in items)
        {
            Log.Information("Processing... {processed}/{amount}", processed, items.Count);
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
    
            var mappedItem = _mapper.Map<StatResourceEntity>(item);
    
            await using (var psqlContext = new ArcadiaContext(_psqlArcadiaContext))
            {
                var existingEntity = psqlContext.StatResources.FirstOrDefault(i => i.Id == item.id);
                if (existingEntity != null)
                {
                    psqlContext.StatResources.Update(mappedItem);
                }
                else
                {
                    psqlContext.StatResources.Add(mappedItem);
                }
                await psqlContext.SaveChangesAsync(token);
            }
            processed++;
            ClearCurrentConsoleLine();
        }
        
        _finishedTransfers.Add(nameof(MSSQLStatResource));
        UpdateConsole();
    }
    
    private async Task TransferChannelResource(CancellationToken token)
    {
        await using var mssqlContext = new MssqlArcadiaContext(_mssqlOptions);

        var items = mssqlContext.ChannelResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount}",nameof(MSSQLChannelResource), items.Count);
        foreach (var item in items)
        {
            Log.Information("Processing... {processed}/{amount}", processed, items.Count);
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
    
            var mappedItem = _mapper.Map<ChannelResourceEntity>(item);
    
            await using (var psqlContext = new ArcadiaContext(_psqlArcadiaContext))
            {
                var existingEntity = psqlContext.ChannelResources.FirstOrDefault(i => i.Id == item.id);
                if (existingEntity != null)
                {
                    psqlContext.ChannelResources.Update(mappedItem);
                }
                else
                {
                    psqlContext.ChannelResources.Add(mappedItem);
                }
                await psqlContext.SaveChangesAsync(token);
            }
            processed++;
            ClearCurrentConsoleLine();
        }
        
        _finishedTransfers.Add(nameof(MSSQLChannelResource));
        UpdateConsole();
    }
    
    private async Task TransferEffectResource(CancellationToken token)
    {
        await using var mssqlContext = new MssqlArcadiaContext(_mssqlOptions);

        var items = mssqlContext.EffectResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount}",nameof(MSSQLEffectResource), items.Count);
        
        foreach (var item in items)
        {
            Log.Information("Processing... {processed}/{amount}", processed, items.Count);
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
    
            var mappedItem = _mapper.Map<EffectResourceEntity>(item);
    
            await using (var psqlContext = new ArcadiaContext(_psqlArcadiaContext))
            {
                // DO NOT USE context.AddRange(entities) too many entities will crash the worker
                var existingEntity = psqlContext.EffectResources.FirstOrDefault(i => i.Id == item.resource_effect_file_id);
                if (existingEntity != null)
                {
                    psqlContext.EffectResources.Update(mappedItem);
                }
                else
                {
                    psqlContext.EffectResources.Add(mappedItem);
                }
                await psqlContext.SaveChangesAsync(token);
            }
            processed++;
            ClearCurrentConsoleLine();
        }
        
        _finishedTransfers.Add(nameof(MSSQLEffectResource));
        UpdateConsole();
    }
    
    private async Task TransferItemEffectResource(CancellationToken token)
    {
        await using var mssqlContext = new MssqlArcadiaContext(_mssqlOptions);

        var items = mssqlContext.ItemEffectResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount} entities",nameof(MSSQLItemEffectResource), items.Count);
        
        foreach (var item in items)
        {
            Log.Information("Processing... {processed}/{amount}", processed, items.Count);
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }

            var mappedItem = _mapper.Map<ItemEffectResourceEntity>(item);
            
            await using (var psqlContext = new ArcadiaContext(_psqlArcadiaContext))
            {
                var existingEntity = psqlContext.ItemEffectResources.FirstOrDefault(i => i.Id == item.id && i.OrdinalId == item.ordinal_id);
                if (existingEntity != null)
                {
                    psqlContext.ItemEffectResources.Update(mappedItem);
                }
                else
                {
                    psqlContext.ItemEffectResources.Add(mappedItem);
                }
                await psqlContext.SaveChangesAsync(token);
            }
            processed++;
            ClearCurrentConsoleLine();
        }

        _finishedTransfers.Add(nameof(MSSQLItemEffectResource));
        UpdateConsole();
    }
    
    private async Task TransferSummonResource(CancellationToken token, bool updateRelations)
    {
        await using var mssqlContext = new MssqlArcadiaContext(_mssqlOptions);

        var items = mssqlContext.SummonResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount} entities",nameof(MSSQLSummonResource), items.Count);

        var loops = 2;
        if (updateRelations)
        {
            loops = 1;
            Log.Logger.Information("Reapplying relations for summons");
        }
        while (loops != 0)
        {
            foreach (var item in items)
            {
                Log.Information("Processing... {processed}/{amount}", processed, items.Count);
                if (token.IsCancellationRequested)
                {
                    Log.Logger.Warning("Stopping...");
                    return;
                }

                var mappedItem = _mapper.Map<SummonResourceEntity>(item);

                await using (var psqlContext = new ArcadiaContext(_psqlArcadiaContext))
                {
                    // ModelResource doesnt exist yet so id will be null 
                    var existingModel = psqlContext.ModelEffectResources.Any(m => m.Id == mappedItem.ModelId);
                    if (!existingModel)
                    {
                        mappedItem.ModelId = null;
                    }
                
                    var existingStat = psqlContext.StatResources.Any(m => m.Id == mappedItem.StatId);
                    if (!existingStat)
                    {
                        mappedItem.StatId = null;
                    }
                
                    var existingCard = psqlContext.ItemResources.Any(m => m.Id == mappedItem.CardId);
                    if (!existingCard)
                    {
                        mappedItem.CardId = null;
                    }
                
                    var existingSummonTarget = psqlContext.SummonResources.Any(m => m.Id == mappedItem.EvolveTargetId);
                    if (!existingSummonTarget)
                    {
                        mappedItem.EvolveTargetId = null;
                    }
                    
                    var existingEntity = psqlContext.SummonResources.FirstOrDefault(i => i.Id == item.id);
                    if (existingEntity != null)
                    {
                        psqlContext.SummonResources.Update(mappedItem);
                    }
                    else
                    {
                        psqlContext.SummonResources.Add(mappedItem);
                    }
                    await psqlContext.SaveChangesAsync(token);
                }
                processed++;
                ClearCurrentConsoleLine();

            }
            
            Log.Logger.Information("Repeating transfer to adjust relations: #{loop}", loops);
            --loops;
            processed = 1;
        }
        ClearCurrentConsoleLine();

        _finishedTransfers.Add(nameof(MSSQLSummonResource));
        UpdateConsole();
    }
    
    private async Task TransferSetItemEffectResource(CancellationToken token)
    {
        await using var mssqlContext = new MssqlArcadiaContext(_mssqlOptions);

        var items = mssqlContext.SetItemEffectResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount} entities",nameof(MSSQLSetItemEffectResource), items.Count);
        
        foreach (var item in items)
        {
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
            Log.Information("Processing... {processed}/{amount}", processed, items.Count);

            var mappedItem = _mapper.Map<SetItemEffectResourceEntity>(item);

            // multi dimensional arrays cannot be instantiated inside the mapper, gotta do it seperatly like down below
            var baseValues = new[,]
            {
                { item.base_var1_0, item.base_var2_0 }, { item.base_var1_1, item.base_var2_1 },
                { item.base_var1_2, item.base_var2_2 }, { item.base_var1_3, item.base_var2_3 }
            };

            var baseTypes = new[] { item.base_type_0, item.base_type_1, item.base_type_2, item.base_type_3 };
            var optTypes = new[] { item.opt_type_0, item.opt_type_1, item.opt_type_2, item.opt_type_3 };

            var optValues = new[,]
            {
                { item.opt_var1_0, item.opt_var2_0 }, { item.opt_var1_1, item.opt_var2_1 },
                { item.opt_var1_2, item.opt_var2_2 }, { item.opt_var1_3, item.opt_var2_3 }
            };

            mappedItem.BaseTypes = baseTypes;
            mappedItem.BaseValues = baseValues;
            mappedItem.OptTypes = optTypes;
            mappedItem.OptValues = optValues;

            await using (var psqlContext = new ArcadiaContext(_psqlArcadiaContext))
            {
                // DO NOT USE context.AddRange(entities) too many entities will crash the worker
                var existingEffect = psqlContext.EffectResources.Any(e => e.Id == mappedItem.EffectId);
                if (!existingEffect)
                {
                    mappedItem.EffectId = null;
                }
                
                var existingEntity = psqlContext.SetItemEffectResources.FirstOrDefault(i => i.Id == item.set_id && (int)i.Parts == item.set_part_id);
                if (existingEntity != null)
                {
                    mappedItem.Id = existingEntity.Id;
                    psqlContext.SetItemEffectResources.Update(mappedItem);
                }
                else
                {
                    psqlContext.SetItemEffectResources.Add(mappedItem);
                }
                
                await psqlContext.SaveChangesAsync(token);
            }

            processed++;
            ClearCurrentConsoleLine();
        }
        
        _finishedTransfers.Add(nameof(MSSQLSetItemEffectResource));
        UpdateConsole();
    }
    
    private async Task TransferEnhanceResource(CancellationToken token)
    {
        await using var mssqlContext = new MssqlArcadiaContext(_mssqlOptions);

        var items = mssqlContext.EnhanceResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount} entities",nameof(MSSQLEnhanceResource), items.Count);
        
        foreach (var item in items)
        {
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
            Log.Information("Processing... {processed}/{amount}", processed, items.Count);

            if (item.fail_result != "1" || item.fail_result != "2" || item.fail_result != "3")
            {
                item.fail_result = "1";
            }
            
            var mappedItem = _mapper.Map<EnhanceResourceEntity>(item);

            await using (var psqlContext = new ArcadiaContext(_psqlArcadiaContext))
            {
                var requiredItemExists = psqlContext.ItemResources.Any(i => i.Id == mappedItem.RequiredItemId);
                if (!requiredItemExists)
                {
                    mappedItem.RequiredItemId = null;
                }

                var existingEntity = psqlContext.EnhanceResources.FirstOrDefault(i => i.Id == item.enhance_id && (int)i.LocalFlag == item.local_flag);
                if (existingEntity != null)
                {
                    psqlContext.EnhanceResources.Update(mappedItem);
                }
                else
                {
                    psqlContext.EnhanceResources.Add(mappedItem);
                }
                
                await psqlContext.SaveChangesAsync(token);
            }

            processed++;
            ClearCurrentConsoleLine();
        }
        _finishedTransfers.Add(nameof(MSSQLEnhanceResource));
        UpdateConsole();
    }
    
    private async Task TransferSkillResource(CancellationToken token)
    {
        await using var mssqlContext = new MssqlArcadiaContext(_mssqlOptions);

        var items = mssqlContext.SkillResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount} entities", nameof(MSSQLSkillResource), items.Count);

        var loops = 2;
        while (loops != 0)
        {
            foreach (var item in items)
            {
                if (token.IsCancellationRequested)
                {
                    Log.Logger.Warning("Stopping...");
                    return;
                }
                Log.Information("Processing... {processed}/{amount}", processed, items.Count);

                var mappedItem = _mapper.Map<SkillResourceEntity>(item);
            
                await using (var psqlContext = new ArcadiaContext(_psqlArcadiaContext))
                {
                    var existingSkillResource = psqlContext.SkillResources.Any(s => s.UpgradeIntoSkillId == mappedItem.Id);
                    if (!existingSkillResource)
                    {
                        mappedItem.UpgradeIntoSkillId = null;
                    }
                    
                    var existingStateResource = psqlContext.StateResources.Any(s => s.Id == mappedItem.StateId);
                    if (!existingStateResource)
                    {
                        mappedItem.StateId = null;
                    }
                    
                    var existingRequiredStateResource = psqlContext.StateResources.Any(s => s.Id == mappedItem.RequiredStateId);
                    if (!existingRequiredStateResource)
                    {
                        mappedItem.RequiredStateId = null;
                    }
                    
                    var existingDescription = psqlContext.StringResources.Any(s => s.Id == mappedItem.DescriptionId);
                    if (!existingDescription)
                    {
                        mappedItem.DescriptionId = null;
                    }
                    
                    var existingText = psqlContext.StringResources.Any(s => s.Id == mappedItem.TextId);
                    if (!existingText)
                    {
                        mappedItem.TextId = null;
                    }
                    
                    var existingTooltip = psqlContext.StringResources.Any(s => s.Id == mappedItem.TooltipId);
                    if (!existingTooltip)
                    {
                        mappedItem.TooltipId = null;
                    }
                    
                    var existingEntity = psqlContext.SkillResources.FirstOrDefault(i => i.Id == item.id);
                    if (existingEntity != null)
                    {
                        psqlContext.SkillResources.Update(mappedItem);
                    }
                    else
                    {
                        psqlContext.SkillResources.Add(mappedItem);
                    }
                
                    await psqlContext.SaveChangesAsync(token);
                }

                processed++;
                ClearCurrentConsoleLine();
            }
            Log.Logger.Information("Repeating transfer to adjust relations: #{loop}", loops);
            --loops;
            processed = 1;
        }
        ClearCurrentConsoleLine();

        _finishedTransfers.Add(nameof(MSSQLSkillResource));
        UpdateConsole();
    }
    
    private async Task TransferStateResource(CancellationToken token)
    {
        await using var mssqlContext = new MssqlArcadiaContext(_mssqlOptions);

        var items = mssqlContext.StateResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount} entities",nameof(MSSQLStateResource), items.Count);
        
        foreach (var item in items)
        {
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
            Log.Information("Processing... {processed}/{amount}", processed, items.Count);

            var mappedItem = _mapper.Map<StateResourceEntity>(item);
            
            await using (var psqlContext = new ArcadiaContext(_psqlArcadiaContext))
            {
                // DO NOT USE context.AddRange(entities) too many entities will crash the worker
                var existingEntity = psqlContext.StateResources.FirstOrDefault(i => i.Id == item.state_id);
                if (existingEntity != null)
                {
                    psqlContext.StateResources.Update(mappedItem);
                }
                else
                {
                    psqlContext.StateResources.Add(mappedItem);
                }
                
                await psqlContext.SaveChangesAsync(token);
            }

            processed++;
            ClearCurrentConsoleLine();
        }
        
        _finishedTransfers.Add(nameof(MSSQLStateResource));
        UpdateConsole();
    }
    
    // TODO transfer ModelEffectResource

    private void UpdateConsole()
    {
        Console.Clear();
        _finishedTransfers.ForEach(l => Log.Logger.Information("Finished transferring {name}", l));
        _finishedSeeds.ForEach(l => Log.Logger.Information("Finished seeding {name}", l));
    }

    private static void ClearCurrentConsoleLine()
    {
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth)); 
        Console.SetCursorPosition(0, currentLineCursor);
        Console.SetCursorPosition(0, Console.CursorTop - 1);
    }
}