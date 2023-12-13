using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MigrateDatabase.MigrationContexts;
using MigrateDatabase.MssqlEntities.Arcadia;
using Navislamia.Game.Contexts;
using Navislamia.Game.Models;
using Navislamia.Game.Models.Arcadia;
using Navislamia.Game.Models.Enums;
using Serilog;

namespace MigrateDatabase;

public class Worker : BackgroundService
{
    private readonly DbContextOptions<MssqlArcadiaContext> _mssqlOptions;
    private readonly DbContextOptions<ArcadiaContext> _psqlOptions;
    private readonly IMapper _mapper;
    
    private readonly List<string> _finishedTransfers = new();
    
    public Worker(DbContextOptions<MssqlArcadiaContext> mssqlOptions, DbContextOptions<ArcadiaContext> psqlOptions, IMapper mapper)
    {
        _mssqlOptions = mssqlOptions;
        _psqlOptions = psqlOptions;
        _mapper = mapper;
    }
    
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        Log.Logger.Warning("Starting transfer from MSSQL to PSQL");
        
        await TransferItemResource(token);
        await TransferLevelResource(token);
        await TransferStringResource(token);
        await TransferStatResource(token);
        await TransferChannelResource(token);
        await TransferGlobalVariables(token);
        await TransferEffectResource(token);
        await TransferItemEffectResource(token);
        await TransferSummonResource(token);
        await TransferSetItemEffectResource(token);
        await TransferEnhanceResource(token);
        await TransferSkillResource(token);
        await TransferStateResource(token);
        
        Log.Logger.Warning("Transfer finished. You can now close this window.");

    }

    private async Task TransferItemResource(CancellationToken token)
    {
        await using var context = new MssqlArcadiaContext(_mssqlOptions);
        var items = context.ItemResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount}",nameof(MSSQLItemResource), items.Count);
        
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
    
            var enhanceIds = new List<long?>();
            if (item.enhance_0_id != 0 || item.enhance_1_id != 0)
            {
                enhanceIds.Add(item.enhance_0_id != 0 ? item.enhance_0_id : null);
                enhanceIds.Add(item.enhance_1_id != 0 ? item.enhance_1_id : null);
            }
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
            mappedItem.EnhanceIds = enhanceIds.ToArray().Length != 0 ? enhanceIds.ToArray() : null;
    
            await using (var psqlContext = new ArcadiaContext(_psqlOptions))
            {
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
            
    
            await using (var psqlContext = new ArcadiaContext(_psqlOptions))
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
        
        foreach (var item in items)
        {
            Log.Information("Processing... {processed}/{amount}", processed, items.Count);
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
    
            var mappedItem = _mapper.Map<StringResourceEntity>(item);
    
            await using (var psqlContext = new ArcadiaContext(_psqlOptions))
            {
                var existingEntity = psqlContext.StringResources.FirstOrDefault(i => i.Id == item.code);
                if (existingEntity != null)
                {
                    psqlContext.StringResources.Update(mappedItem);
                }
                else
                {
                    psqlContext.StringResources.Add(mappedItem);
                }
                await psqlContext.SaveChangesAsync(token);
            }
            processed++;
            ClearCurrentConsoleLine();
        }
        
        _finishedTransfers.Add(nameof(MSSQLStringResource));
        UpdateConsole();
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
    
            await using (var psqlContext = new ArcadiaContext(_psqlOptions))
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
    
            await using (var psqlContext = new ArcadiaContext(_psqlOptions))
            {
                var existingEntity = psqlContext.StatResources.FirstOrDefault(i => i.Id == item.id);
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
    
    private async Task TransferGlobalVariables(CancellationToken token)
    {
        await using var mssqlContext = new MssqlArcadiaContext(_mssqlOptions);

        var items = mssqlContext.GlobalVariable.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount}",nameof(MSSQLGlobalVariable), items.Count);
        
        foreach (var item in items)
        {
            Log.Information("Processing... {processed}/{amount}", processed, items.Count);
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
    
            var mappedItem = _mapper.Map<GlobalVariableEntity>(item);
    
            await using (var psqlContext = new ArcadiaContext(_psqlOptions))
            {
                var existingEntity = psqlContext.GlobalVariables.FirstOrDefault(i => i.Id == item.sid);
                if (existingEntity != null)
                {
                    psqlContext.GlobalVariables.Update(mappedItem);
                }
                else
                {
                    psqlContext.GlobalVariables.Add(mappedItem);
                }
                await psqlContext.SaveChangesAsync(token);
            }
            processed++;
            ClearCurrentConsoleLine();
        }
        
        _finishedTransfers.Add(nameof(MSSQLGlobalVariable));
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
    
            await using (var psqlContext = new ArcadiaContext(_psqlOptions))
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

            await using (var psqlContext = new ArcadiaContext(_psqlOptions))
            {
                // DO NOT USE context.AddRange(entities) too many entities will crash the worker
                var existingEntity = psqlContext.ItemEffectResources.FirstOrDefault(i => i.Id == item.id);
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
    
    private async Task TransferSummonResource(CancellationToken token)
    {
        await using var mssqlContext = new MssqlArcadiaContext(_mssqlOptions);

        var items = mssqlContext.SummonResource.ToList();
        var processed = 1;
        
        Log.Logger.Information("Transferring {type}: {amount} entities",nameof(MSSQLSummonResource), items.Count);
        
        foreach (var item in items)
        {
            Log.Information("Processing... {processed}/{amount}", processed, items.Count);
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }

            var mappedItem = _mapper.Map<SummonResourceEntity>(item);

            await using (var psqlContext = new ArcadiaContext(_psqlOptions))
            {
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

            await using (var psqlContext = new ArcadiaContext(_psqlOptions))
            {
                // DO NOT USE context.AddRange(entities) too many entities will crash the worker
                var existingEntity = psqlContext.SetItemEffectResources.FirstOrDefault(i => i.SetId == item.set_id && (int)i.SetParts == item.set_part_id);
                if (existingEntity != null)
                {
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

            var mappedItem = _mapper.Map<EnhanceResourceEntity>(item);
            
            await using (var psqlContext = new ArcadiaContext(_psqlOptions))
            {
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
        
        foreach (var item in items)
        {
            if (token.IsCancellationRequested)
            {
                Log.Logger.Warning("Stopping...");
                return;
            }
            Log.Information("Processing... {processed}/{amount}", processed, items.Count);

            var mappedItem = _mapper.Map<SkillResourceEntity>(item);
            
            await using (var psqlContext = new ArcadiaContext(_psqlOptions))
            {
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
            
            await using (var psqlContext = new ArcadiaContext(_psqlOptions))
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

    private void UpdateConsole()
    {
        Console.Clear();
        _finishedTransfers.ForEach(l => Log.Logger.Information("Finished transferring {name}", l));
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