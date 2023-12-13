using System.ComponentModel.DataAnnotations;

namespace MigrateDatabase.MssqlEntities.Arcadia;

public class MSSQLSummonResource
{
    [Key] public int id { get; set; }
    public int model_id { get; set; }
    public int name_id { get; set; }
    public int type { get; set; }
    public int magic_type { get; set; }
    public byte rate { get; set; }
    public int stat_id { get; set; }
    public decimal size { get; set; }
    public decimal scale { get; set; }
    public decimal target_fx_size { get; set; }
    public int standard_walk_speed { get; set; }
    public int standard_run_speed { get; set; }
    public int riding_speed { get; set; }
    public int run_speed { get; set; }
    public byte is_riding_only { get; set; }
    public int riding_motion_type { get; set; }
    public decimal attack_range { get; set; }
    public int walk_type { get; set; }
    public int slant_type { get; set; }
    public int material { get; set; }
    public int weapon_type { get; set; }
    public int attack_motion_speed { get; set; }
    public int form { get; set; }
    public int evolve_target { get; set; }
    public int camera_x { get; set; }
    public int camera_y { get; set; }
    public int camera_z { get; set; }
    public decimal target_x { get; set; }
    public decimal target_y { get; set; }
    public decimal target_z { get; set; }
    public string model { get; set; }
    public int motion_file_id { get; set; }
    public int face_id { get; set; }
    public string face_file_name { get; set; }
    public int card_id { get; set; }
    public string script_text { get; set; }
    public string illust_file_name { get; set; }
    public int text_feature_id { get; set; }
    public int text_name_id { get; set; }
    public int skill1_id { get; set; }
    public int skill1_text_id { get; set; }
    public int skill2_id { get; set; }
    public int skill2_text_id { get; set; }
    public int skill3_id { get; set; }
    public int skill3_text_id { get; set; }
    public int skill4_id { get; set; }
    public int skill4_text_id { get; set; }
    public int skill5_id { get; set; }
    public int skill5_text_id { get; set; }
    public int texture_group { get; set; }
    public int local_flag { get; set; }
}