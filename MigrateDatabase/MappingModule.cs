using AutoMapper;
using MigrateDatabase.Mappers;

namespace MigrateDatabase;

public static class MappingModule
{
    public static void LoadProfiles(IMapperConfigurationExpression config)
    {
        config.AddProfile<ArcadiaResourcesMappingProfile>();
    }
}