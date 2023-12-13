using System.ComponentModel.DataAnnotations;

namespace MigrateDatabase.MssqlEntities.Arcadia;

public class MSSQLItemEffectResource
{
    [Key] public int id { get; set; }
    public int ordinal_id { get; set; }
    public int tooltip_id { get; set; }
    public byte effect_type { get; set; }
    public short effect_id { get; set; }
    public short effect_level { get; set; }
    public decimal value_0 { get; set; }
    public decimal value_1 { get; set; }
    public decimal value_2 { get; set; }
    public decimal value_3 { get; set; }
    public decimal value_4 { get; set; }
    public decimal value_5 { get; set; }
    public decimal value_6 { get; set; }
    public decimal value_7 { get; set; }
    public decimal value_8 { get; set; }
    public decimal value_9 { get; set; }
    public decimal value_10 { get; set; }
    public decimal value_11 { get; set; }
    public decimal value_12 { get; set; }
    public decimal value_13 { get; set; }
    public decimal value_14 { get; set; }
    public decimal value_15 { get; set; }
    public decimal value_16 { get; set; }
    public decimal value_17 { get; set; }
    public decimal value_18 { get; set; }
    public decimal value_19 { get; set; }
}