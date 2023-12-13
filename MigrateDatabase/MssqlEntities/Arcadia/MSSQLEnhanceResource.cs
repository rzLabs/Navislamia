using Microsoft.EntityFrameworkCore;

namespace MigrateDatabase.MssqlEntities.Arcadia;

[PrimaryKey(nameof(enhance_id), nameof(local_flag))]
public class MSSQLEnhanceResource
{
    public int enhance_id { get; set; }
    public string enhance_type { get; set; }
    public string fail_result { get; set; }
    public byte max_enhance { get; set; }
    public int local_flag { get; set; }
    public int need_item { get; set; }
    public decimal percentage_1 { get; set; }
    public decimal percentage_2 { get; set; }
    public decimal percentage_3 { get; set; }
    public decimal percentage_4 { get; set; }
    public decimal percentage_5 { get; set; }
    public decimal percentage_6 { get; set; }
    public decimal percentage_7 { get; set; }
    public decimal percentage_8 { get; set; }
    public decimal percentage_9 { get; set; }
    public decimal percentage_10 { get; set; }
    public decimal percentage_11 { get; set; }
    public decimal percentage_12 { get; set; }
    public decimal percentage_13 { get; set; }
    public decimal percentage_14 { get; set; }
    public decimal percentage_15 { get; set; }
    public decimal percentage_16 { get; set; }
    public decimal percentage_17 { get; set; }
    public decimal percentage_18 { get; set; }
    public decimal percentage_19 { get; set; }
    public decimal percentage_20 { get; set; }
}