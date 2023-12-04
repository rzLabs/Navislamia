using DevConsole.Models.Arcadia;
using Microsoft.Extensions.Logging;
using Navislamia.Database.Contexts;
using Navislamia.Game.Contexts;
using Navislamia.Game.Repositories;

namespace Navislamia.Game.Services;

public class LevelResourceRepository : EfRepository<LevelResourceEntity>
{
    public LevelResourceRepository(ArcadiaContext context, ILogger<EfRepository<LevelResourceEntity>> logger) : base(context, logger)
    {
    }
}