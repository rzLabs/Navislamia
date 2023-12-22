using System.ComponentModel.DataAnnotations;

namespace MigrateDatabase.MssqlEntities.Arcadia;

public class MSSQLGlobalVariable
{
    [Key]
    public int sid {get;set;}
    public string name {get;set;}
    public string value {get;set;}
}