using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MigrateDatabase.MssqlEntities.Arcadia;

public class MSSQLStatResource
{
    [Key] public int id { get; set; }
    public int str { get; set; }
    public int vit { get; set; }
    public int dex { get; set; }
    public int agi { get; set; }
    [Column("int")]
    public int intelligence { get; set; }
    public int men { get; set; }
    public int luk { get; set; }

}