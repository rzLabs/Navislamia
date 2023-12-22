using System.ComponentModel.DataAnnotations;

namespace MigrateDatabase.MssqlEntities.Arcadia;

public class MSSQLChannelResource
{
    [Key]
    public int id {get;set;}
    public int left {get;set;}
    public int top {get;set;}
    public int right {get;set;}
    public int bottom {get;set;}
    public int channel_type {get;set;}
}