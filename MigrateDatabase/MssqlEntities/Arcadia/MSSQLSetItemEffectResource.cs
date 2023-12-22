
using Microsoft.EntityFrameworkCore;

namespace MigrateDatabase.MssqlEntities.Arcadia;

[PrimaryKey(nameof(set_id), nameof(set_part_id))]
public class MSSQLSetItemEffectResource
{
    public int set_id { get; set; }
    public int set_part_id { get; set; }
    public int text_id { get; set; }
    public int tooltip_id { get; set; }
    public short base_type_0 { get; set; }
    public decimal base_var1_0 { get; set; }
    public decimal base_var2_0 { get; set; }
    public short base_type_1 { get; set; }
    public decimal base_var1_1 { get; set; }
    public decimal base_var2_1 { get; set; }
    public short base_type_2 { get; set; }
    public decimal base_var1_2 { get; set; }
    public decimal base_var2_2 { get; set; }
    public short base_type_3 { get; set; }
    public decimal base_var1_3 { get; set; }
    public decimal base_var2_3 { get; set; }
    public short opt_type_0 { get; set; }
    public decimal opt_var1_0 { get; set; }
    public decimal opt_var2_0 { get; set; }
    public short opt_type_1 { get; set; }
    public decimal opt_var1_1 { get; set; }
    public decimal opt_var2_1 { get; set; }
    public short opt_type_2 { get; set; }
    public decimal opt_var1_2 { get; set; }
    public decimal opt_var2_2 { get; set; }
    public short opt_type_3 { get; set; }
    public decimal opt_var1_3 { get; set; }
    public decimal opt_var2_3 { get; set; }
    public int effect_id { get; set; }
}