using System.ComponentModel.DataAnnotations;

namespace MigrateDatabase.MssqlEntities.Arcadia;

public class MSSQLStateResource
{
	[Key] public int state_id { get; set; }
	public int text_id { get; set; }
	public int tooltip_id { get; set; }
	public string is_harmful { get; set; }
	public int state_time_type { get; set; }
	public int state_group { get; set; }
	public int duplicate_group_1 { get; set; }
	public int duplicate_group_2 { get; set; }
	public int duplicate_group_3 { get; set; }
	public string uf_avatar { get; set; }
	public string uf_summon { get; set; }
	public string uf_monster { get; set; }
	public string reiteration_count { get; set; }
	public int base_effect_id { get; set; }
	public int fire_interval { get; set; }
	public int elemental_type { get; set; }
	public decimal amplify_base { get; set; }
	public decimal amplify_per_skl { get; set; }
	public int add_damage_base { get; set; }
	public int add_damage_per_skl { get; set; }
	public int effect_type { get; set; } // always 0  - not required
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
	public int icon_id { get; set; }
	public string icon_file_name { get; set; }
	public int fx_id { get; set; }
	public int pos_id { get; set; }
	public int cast_skill_id { get; set; }
	public int cast_fx_id { get; set; }
	public int cast_fx_pos_id { get; set; }
	public int hit_fx_id { get; set; }
	public int hit_fx_pos_id { get; set; }
	public int special_output_timing_id { get; set; }
	public int special_output_fx_id { get; set; }
	public int special_output_fx_pos_id { get; set; }
	public int special_output_fx_delay { get; set; }
	public int state_fx_id { get; set; }
	public int state_fx_pos_id { get; set; }

	}