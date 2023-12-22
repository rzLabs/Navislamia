using System.ComponentModel.DataAnnotations;

namespace MigrateDatabase.MssqlEntities.Arcadia;

public class MSSQLEffectResource
{
    [Key]
    public int resource_effect_file_id { get; set; }
    public string resource_effect_file_name { get; set; }
}