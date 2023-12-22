using System.ComponentModel.DataAnnotations;

namespace MigrateDatabase.MssqlEntities.Arcadia;

public class MSSQLStringResource
{
    [Key]
    public int code { get; set; }
    public string name { get; set; }
    public int group_id { get; set; }
    public string value { get; set; }
}