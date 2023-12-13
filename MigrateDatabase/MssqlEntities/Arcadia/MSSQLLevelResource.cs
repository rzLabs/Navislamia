using System.ComponentModel.DataAnnotations;

namespace MigrateDatabase.MssqlEntities.Arcadia;

public class MSSQLLevelResource
{
    [Key]
    public int level {get;set;}
    public long normal_exp {get;set;}
    public int jl1 {get;set;}
    public int jl2 {get;set;}
    public int jl3 {get;set;}
    public int jl4 {get;set;}
}