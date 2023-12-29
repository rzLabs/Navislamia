using AutoMapper;
using MigrateDatabase.MssqlEntities.Arcadia;
using Navislamia.Game.DataAccess.Entities;
using Navislamia.Game.DataAccess.Entities.Arcadia;
using Navislamia.Game.DataAccess.Entities.Enums;

namespace MigrateDatabase.Mappers;

public class ArcadiaResourcesMappingProfile : Profile
{
	public ArcadiaResourcesMappingProfile()
	{
		CreateMap<MSSQLItemResource, ItemResourceEntity>()
			.ForMember(src => src.Id, ex => ex.MapFrom(dst => dst.id))
			.ForMember(src => src.JobDepth, ex => ex.MapFrom(dst => dst.job_depth))
			.ForMember(src => src.UseMinLevel, ex => ex.MapFrom(dst => dst.use_min_level))
			.ForMember(src => src.UseMaxLevel, ex => ex.MapFrom(dst => dst.use_max_level))
			.ForMember(src => src.TargetMinLevel, ex => ex.MapFrom(dst => dst.target_min_level))
			.ForMember(src => src.TargetMaxLevel, ex => ex.MapFrom(dst => dst.target_max_level))
			.ForMember(src => src.Range, ex => ex.MapFrom(dst => dst.range))
			.ForMember(src => src.Weight, ex => ex.MapFrom(dst => dst.weight))
			.ForMember(src => src.Price, ex => ex.MapFrom(dst => dst.price))
			.ForMember(src => src.HuntaholicPoint, ex => ex.MapFrom(dst => dst.huntaholic_point))
			.ForMember(src => src.EtherealDurability, ex => ex.MapFrom(dst => dst.ethereal_durability))
			.ForMember(src => src.Endurance, ex => ex.MapFrom(dst => dst.endurance))
			.ForMember(src => src.Material, ex => ex.MapFrom(dst => dst.material))
			.ForMember(src => src.ItemUseFlag, ex => ex.MapFrom(dst => dst.item_use_flag))
			.ForMember(src => src.AvailablePeriod, ex => ex.MapFrom(dst => dst.available_period))
			.ForMember(src => src.DecreaseType, ex => ex.MapFrom(dst => dst.decrease_type))
			.ForMember(src => src.ThrowRange, ex => ex.MapFrom(dst => dst.throw_range))
			.ForMember(src => src.StateLevel, ex => ex.MapFrom(dst => dst.state_level))
			.ForMember(src => src.StateTime, ex => ex.MapFrom(dst => dst.state_time))
			.ForMember(src => src.CoolTime, ex => ex.MapFrom(dst => dst.cool_time))
			.ForMember(src => src.CoolTimeGroup, ex => ex.MapFrom(dst => dst.cool_time_group))
			.ForMember(src => src.ScriptText,
				ex => ex.MapFrom(dst => string.IsNullOrWhiteSpace(dst.script_text) ? null : dst.script_text))
			.ForMember(src => src.ItemBaseType, ex => ex.MapFrom(dst => dst.type))
			.ForMember(src => src.Group, ex => ex.MapFrom(dst => dst.group))
			.ForMember(src => src.ItemType, ex => ex.MapFrom(dst => dst.Class))
			.ForMember(src => src.WearType, ex => ex.MapFrom(dst => dst.wear_type))
			.ForMember(src => src.SetPart, ex => ex.MapFrom(dst => (SetParts)dst.set_part_flag))
			.ForMember(src => src.Grade, ex => ex.MapFrom(dst => dst.grade))
			.ForMember(src => src.Rank, ex => ex.MapFrom(dst => dst.rank))
			.ForMember(src => src.Level, ex => ex.MapFrom(dst => dst.level))
			.ForMember(src => src.Enhance, ex => ex.MapFrom(dst => dst.enhance))
			.ForMember(src => src.BaseTypes, ex => ex.MapFrom(dst =>  new[] { dst.base_type_0, dst.base_type_1, dst.base_type_2, dst.base_type_3 }))
			.ForMember(src => src.OptTypes, ex => ex.MapFrom(dst => new[] { dst.opt_type_0, dst.opt_type_1, dst.opt_type_2, dst.opt_type_3 }))
			.ForMember(src => src.SocketCount, ex => ex.MapFrom(dst => dst.socket))
			.ForMember(src => src.Status, ex => ex.MapFrom(dst => dst.status_flag))
			.ForMember(src => src.EnhanceIds, ex => ex.MapFrom(dst => new[] { dst.enhance_0_id, dst.enhance_1_id }))
			.ForMember(src => src.SetId, ex => ex.MapFrom(dst => dst.SetId != 0 ? dst.SetId : (int?)null))
			.ForMember(src => src.NameId, ex => ex.MapFrom(dst => dst.name_id != 0 ? dst.name_id : (int?)null))
			.ForMember(src => src.TooltipId, ex => ex.MapFrom(dst => dst.tooltip_id != 0 ? dst.tooltip_id : (int?)null))
			.ForMember(src => src.SummonId, ex => ex.MapFrom(dst => dst.summon_id != 0 ? dst.summon_id : (int?)null))
			.ForMember(src => src.EffectId, ex => ex.MapFrom(dst => dst.effect_id != 0 ? dst.effect_id : (int?)null))
			.ForMember(src => src.EnhanceIds,
				ex => ex.MapFrom(dst => new[]
				{
					dst.enhance_0_id != 0 ? dst.enhance_0_id : null, dst.enhance_1_id != 0 ? dst.enhance_1_id : (int?)null
				}))
			.ForMember(src => src.SkillId, ex => ex.MapFrom(dst => dst.skill_id != 0 ? dst.skill_id : (int?)null))
			.ForMember(src => src.StateId, ex => ex.MapFrom(dst => dst.state_id != 0 ? dst.state_id : (int?)null))
			.ReverseMap();

		CreateMap<MSSQLLevelResource, LevelResourceEntity>()
			.ForMember(src => src.Level, ex => ex.MapFrom(dst => dst.level))
			.ForMember(src => src.NormalExp, ex => ex.MapFrom(dst => dst.normal_exp))
			.ForMember(src => src.JLvs, ex => ex.MapFrom(dst => new[] { dst.jl1, dst.jl2, dst.jl3, dst.jl4 }))
			.ReverseMap();

		CreateMap<MSSQLStringResource, StringResourceEntity>()
			.ForMember(src => src.Name, ex => ex.MapFrom(dst => dst.name))
			.ForMember(src => src.Id, ex => ex.MapFrom(dst => dst.code))
			.ForMember(src => src.Value, ex => ex.MapFrom(dst => dst.value))
			.ReverseMap();

		CreateMap<MSSQLStatResource, StatResourceEntity>()
			.ForMember(src => src.Id, ex => ex.MapFrom(dst => dst.id))
			.ForMember(src => src.Strength, ex => ex.MapFrom(dst => dst.str))
			.ForMember(src => src.Vitality, ex => ex.MapFrom(dst => dst.vit))
			.ForMember(src => src.Dexterity, ex => ex.MapFrom(dst => dst.dex))
			.ForMember(src => src.Agility, ex => ex.MapFrom(dst => dst.agi))
			.ForMember(src => src.Intelligence, ex => ex.MapFrom(dst => dst.intelligence))
			.ForMember(src => src.Wisdom, ex => ex.MapFrom(dst => dst.men))
			.ForMember(src => src.Luck, ex => ex.MapFrom(dst => dst.luk))
			.ReverseMap();

		CreateMap<MSSQLChannelResource, ChannelResourceEntity>()
			.ForMember(src => src.Id, ex => ex.MapFrom(dst => dst.id))
			.ForMember(src => src.Left, ex => ex.MapFrom(dst => dst.left))
			.ForMember(src => src.Top, ex => ex.MapFrom(dst => dst.top))
			.ForMember(src => src.Right, ex => ex.MapFrom(dst => dst.right))
			.ForMember(src => src.Bottom, ex => ex.MapFrom(dst => dst.bottom))
			.ForMember(src => src.ChannelType, ex => ex.MapFrom(dst => dst.channel_type))
			.ReverseMap();

		CreateMap<MSSQLGlobalVariable, GlobalVariableEntity>()
			.ForMember(src => src.Id, ex => ex.MapFrom(dst => dst.sid))
			.ForMember(src => src.Name, ex => ex.MapFrom(dst => dst.name))
			.ForMember(src => src.Value, ex => ex.MapFrom(dst => dst.value))
			.ReverseMap();

		CreateMap<MSSQLEffectResource, EffectResourceEntity>()
			.ForMember(src => src.Id, ex => ex.MapFrom(dst => dst.resource_effect_file_id))
			.ForMember(src => src.FileName, ex => ex.MapFrom(dst => dst.resource_effect_file_name))
			.ReverseMap();

		CreateMap<MSSQLItemEffectResource, ItemEffectResourceEntity>()
			.ForMember(src => src.Id, ex => ex.MapFrom(dst => dst.id))
			.ForMember(src => src.OrdinalId, ex => ex.MapFrom(dst => dst.ordinal_id))
			.ForMember(src => src.TooltipId, ex => ex.MapFrom(dst => dst.tooltip_id))
			.ForMember(src => src.EffectTrigger, ex => ex.MapFrom(dst => (EffectTrigger)dst.effect_id))
			.ForMember(src => src.EffectType, ex => ex.MapFrom(dst => (EffectType)dst.effect_type))
			.ForMember(src => src.EffectLevel, ex => ex.MapFrom(dst => dst.effect_level))
			.ForMember(src => src.Values, ex => ex.MapFrom(dst => new decimal[]
			{
				dst.value_0, dst.value_1, dst.value_2, dst.value_3, dst.value_4,
				dst.value_5, dst.value_6, dst.value_7, dst.value_8, dst.value_9,
				dst.value_10, dst.value_11, dst.value_12, dst.value_13, dst.value_14,
				dst.value_15, dst.value_16, dst.value_17, dst.value_18, dst.value_19
			}))
			.ReverseMap();

		CreateMap<MSSQLSummonResource, SummonResourceEntity>()
			.ForMember(src => src.Id, ex => ex.MapFrom(dst => dst.id))
			.ForMember(src => src.Type, ex => ex.MapFrom(dst => dst.type))
			.ForMember(src => src.MagicType, ex => ex.MapFrom(dst => dst.magic_type))
			.ForMember(src => src.Size, ex => ex.MapFrom(dst => dst.size))
			.ForMember(src => src.Scale, ex => ex.MapFrom(dst => dst.scale))
			.ForMember(src => src.TargetFxSize, ex => ex.MapFrom(dst => dst.target_fx_size))
			.ForMember(src => src.StandardWalkSpeed, ex => ex.MapFrom(dst => dst.standard_walk_speed))
			.ForMember(src => src.StandardRunSpeed, ex => ex.MapFrom(dst => dst.standard_run_speed))
			.ForMember(src => src.IsRidingOnly, ex => ex.MapFrom(dst => dst.is_riding_only))
			.ForMember(src => src.RidingSpeed, ex => ex.MapFrom(dst => dst.riding_speed))
			.ForMember(src => src.RunSpeed, ex => ex.MapFrom(dst => dst.run_speed))
			.ForMember(src => src.RidingMotionType, ex => ex.MapFrom(dst => dst.riding_motion_type))
			.ForMember(src => src.AttackRange, ex => ex.MapFrom(dst => dst.attack_range))
			.ForMember(src => src.WalkType, ex => ex.MapFrom(dst => dst.walk_type))
			.ForMember(src => src.SlantType, ex => ex.MapFrom(dst => dst.slant_type))
			.ForMember(src => src.Material, ex => ex.MapFrom(dst => dst.material))
			.ForMember(src => src.WeaponType, ex => ex.MapFrom(dst => dst.weapon_type))
			.ForMember(src => src.AttackMotionSpeed, ex => ex.MapFrom(dst => dst.attack_motion_speed))
			.ForMember(src => src.EvolveTargetId, ex => ex.MapFrom(dst => dst.evolve_target))
			.ForMember(src => src.CameraPosition,
				ex => ex.MapFrom(dst => new[] { dst.camera_x, dst.camera_y, dst.camera_z }))
			.ForMember(src => src.TargetPosition,
				ex => ex.MapFrom(dst => new[] { dst.target_x, dst.target_y, dst.target_z }))
			.ForMember(src => src.ModelId, ex => ex.MapFrom(dst => dst.model_id))
			.ForMember(src => src.ModelName, ex => ex.MapFrom(dst => dst.model))
			.ForMember(src => src.MotionFileId, ex => ex.MapFrom(dst => dst.motion_file_id))
			.ForMember(src => src.FaceId, ex => ex.MapFrom(dst => dst.face_id))
			.ForMember(src => src.FaceFileName, ex => ex.MapFrom(dst => dst.face_file_name))
			.ForMember(src => src.CardId, ex => ex.MapFrom(dst => dst.card_id))
			.ForMember(src => src.IllustFileName, ex => ex.MapFrom(dst => dst.illust_file_name))
			.ForMember(src => src.TextFeatureId, ex => ex.MapFrom(dst => dst.text_feature_id))
			.ForMember(src => src.SkillIds,
				ex => ex.MapFrom(dst => new[] { dst.skill1_id, dst.skill2_id, dst.skill3_id, dst.skill4_id }))
			// .ForMember(src => src.SkillTextIds,
			// 	ex => ex.MapFrom(dst => new[]
			// 		{ dst.skill1_text_id, dst.skill2_text_id, dst.skill3_text_id, dst.skill4_text_id }))
			.ForMember(src => src.StatId, ex => ex.MapFrom(dst => dst.stat_id))
			.ForMember(src => src.NameId, ex => ex.MapFrom(dst => dst.name_id))
			.ForMember(src => src.TextureGroup, ex => ex.MapFrom(dst => dst.texture_group))
			.ForMember(src => src.Model, ex => ex.Ignore())
			.ForMember(src => src.EvolveTarget, ex => ex.Ignore())
			.ForMember(src => src.EvolveSource, ex => ex.Ignore())
			.ReverseMap();

		CreateMap<MSSQLSetItemEffectResource, SetItemEffectResourceEntity>()
			.ForMember(src => src.Id, ex => ex.MapFrom(dst => dst.set_id))
			.ForMember(src => src.Parts, ex => ex.MapFrom(dst => (SetParts)dst.set_part_id))
			.ForMember(src => src.TextId, ex => ex.MapFrom(dst => dst.text_id))
			.ForMember(src => src.TooltipId, ex => ex.MapFrom(dst => dst.tooltip_id))
			.ForMember(src => src.EffectId, ex => ex.MapFrom(dst => dst.effect_id))
			.ReverseMap();

		CreateMap<MSSQLEnhanceResource, EnhanceResourceEntity>()
			.ForMember(src => src.Id, ex => ex.MapFrom(dst => dst.enhance_id))
			.ForMember(src => src.EnhanceType, ex => ex.MapFrom(dst => (EnhanceType)int.Parse(dst.enhance_type)))
			.ForMember(src => src.FailResult, ex => ex.MapFrom(dst => dst.fail_result))
			.ForMember(src => src.LocalFlag, ex => ex.MapFrom(dst => dst.local_flag))
			.ForMember(src => src.RequiredItemId, ex => ex.MapFrom(dst => dst.need_item))
			.ForMember(src => src.MaxEnhance, ex => ex.MapFrom(dst => dst.max_enhance))
			.ForMember(src => src.Percentage,
				ex => ex.MapFrom(dst => new decimal[]
				{
					dst.percentage_1, dst.percentage_2, dst.percentage_3, dst.percentage_4, dst.percentage_5,
					dst.percentage_6, dst.percentage_7, dst.percentage_8, dst.percentage_9, dst.percentage_10,
					dst.percentage_11, dst.percentage_12, dst.percentage_13, dst.percentage_14, dst.percentage_15,
					dst.percentage_16, dst.percentage_17, dst.percentage_18, dst.percentage_19, dst.percentage_20
				}))
			.ReverseMap();

		CreateMap<MSSQLSkillResource, SkillResourceEntity>()
			.ForMember(src => src.Id, ex => ex.MapFrom(dst => dst.id))
			.ForMember(src => src.TextId, ex => ex.MapFrom(dst => dst.text_id))
			.ForMember(src => src.DescriptionId, ex => ex.MapFrom(dst => dst.desc_id))
			.ForMember(src => src.TooltipId, ex => ex.MapFrom(dst => dst.tooltip_id))
			.ForMember(src => src.ElementalType, ex => ex.MapFrom(dst => int.Parse(dst.elemental)))
			.ForMember(src => src.IsPassive, ex => ex.MapFrom(dst => int.Parse(dst.is_passive)))
			.ForMember(src => src.IsValid, ex => ex.MapFrom(dst => (SkillState)dst.is_valid))
			.ForMember(src => src.IsPhysicalAct, ex => ex.MapFrom(dst => int.Parse(dst.is_physical_act)))
			.ForMember(src => src.IsProjectile, ex => ex.MapFrom(dst => dst.is_projectile))
			.ForMember(src => src.IsHarmful, ex => ex.MapFrom(dst => int.Parse(dst.is_harmful)))
			.ForMember(src => src.RequiredTarget, ex => ex.MapFrom(dst => int.Parse(dst.is_need_target)))
			.ForMember(src => src.IsCorpse, ex => ex.MapFrom(dst => int.Parse(dst.is_corpse)))
			.ForMember(src => src.IsToggle, ex => ex.MapFrom(dst => int.Parse(dst.is_toggle)))
			.ForMember(src => src.ToggleGroup, ex => ex.MapFrom(dst => dst.toggle_group))
			.ForMember(src => src.CastingType, ex => ex.MapFrom(dst => dst.casting_type))
			.ForMember(src => src.CastingLevel, ex => ex.MapFrom(dst => dst.casting_level))
			.ForMember(src => src.CastRange, ex => ex.MapFrom(dst => dst.cast_range))
			.ForMember(src => src.ValidRange, ex => ex.MapFrom(dst => dst.valid_range))
			.ForMember(src => src.CostHp, ex => ex.MapFrom(dst => dst.cost_hp))
			.ForMember(src => src.CostHpPerSkl, ex => ex.MapFrom(dst => dst.hate_per_skl))
			.ForMember(src => src.CostMp, ex => ex.MapFrom(dst => dst.cost_mp))
			.ForMember(src => src.CostMpPerSkl, ex => ex.MapFrom(dst => dst.cost_mp_per_skl))
			.ForMember(src => src.CostMpPerEnhance, ex => ex.MapFrom(dst => dst.cost_mp_per_enhance))
			.ForMember(src => src.CostHpPer, ex => ex.MapFrom(dst => dst.cost_hp_per))
			.ForMember(src => src.CostHpPerSklPer, ex => ex.MapFrom(dst => dst.cost_hp_per_skl_per))
			.ForMember(src => src.CostMpPer, ex => ex.MapFrom(dst => dst.cost_mp_per))
			.ForMember(src => src.CostMpPerSklPer, ex => ex.MapFrom(dst => dst.cost_mp_per_skl_per))
			.ForMember(src => src.CostHavoc, ex => ex.MapFrom(dst => dst.cost_havoc))
			.ForMember(src => src.CostHavocPerSkl, ex => ex.MapFrom(dst => dst.cost_havoc_per_skl))
			.ForMember(src => src.CostEnergy, ex => ex.MapFrom(dst => dst.cost_energy))
			.ForMember(src => src.CostEnergyPerSkl, ex => ex.MapFrom(dst => dst.cost_energy_per_skl))
			.ForMember(src => src.CostExp, ex => ex.MapFrom(dst => dst.cost_exp))
			.ForMember(src => src.CostExpPerEnhance, ex => ex.MapFrom(dst => dst.cost_exp_per_enhance))
			.ForMember(src => src.CostJp, ex => ex.MapFrom(dst => dst.cost_jp))
			.ForMember(src => src.CostJpPerEnhance, ex => ex.MapFrom(dst => dst.cost_jp_per_enhance))
			.ForMember(src => src.CostItem, ex => ex.MapFrom(dst => dst.cost_item))
			.ForMember(src => src.CostItemCount, ex => ex.MapFrom(dst => dst.cost_item_count))
			.ForMember(src => src.CostItemCountPerSkl, ex => ex.MapFrom(dst => dst.cost_item_count_per_skl))
			.ForMember(src => src.RequiredLevel, ex => ex.MapFrom(dst => dst.need_level))
			.ForMember(src => src.RequiredHp, ex => ex.MapFrom(dst => dst.need_hp))
			.ForMember(src => src.RequiredMp, ex => ex.MapFrom(dst => dst.need_mp))
			.ForMember(src => src.RequiredHavoc, ex => ex.MapFrom(dst => dst.need_havoc))
			.ForMember(src => src.RequiredHavocBurst, ex => ex.MapFrom(dst => dst.need_havoc_burst))
			.ForMember(src => src.RequiredStateId, ex => ex.MapFrom(dst => dst.need_state_id))
			.ForMember(src => src.UseWithOneHandSword, ex => ex.MapFrom(dst => int.Parse(dst.vf_one_hand_sword)))
			.ForMember(src => src.UseWithTwoHandSword, ex => ex.MapFrom(dst => int.Parse(dst.vf_two_hand_sword)))
			.ForMember(src => src.UseWithDoubleSword, ex => ex.MapFrom(dst => int.Parse(dst.vf_double_sword)))
			.ForMember(src => src.UseWithDagger, ex => ex.MapFrom(dst => int.Parse(dst.vf_dagger)))
			.ForMember(src => src.UseWithDoubleDagger, ex => ex.MapFrom(dst => int.Parse(dst.vf_double_dagger)))
			.ForMember(src => src.UseWithSpear, ex => ex.MapFrom(dst => int.Parse(dst.vf_spear)))
			.ForMember(src => src.UseWithAxe, ex => ex.MapFrom(dst => int.Parse(dst.vf_axe)))
			.ForMember(src => src.UseWithOneHandAxe, ex => ex.MapFrom(dst => int.Parse(dst.vf_one_hand_axe)))
			.ForMember(src => src.UseWithDoubleAxe, ex => ex.MapFrom(dst => int.Parse(dst.vf_double_axe)))
			.ForMember(src => src.UseWithOneHandMace, ex => ex.MapFrom(dst => int.Parse(dst.vf_one_hand_mace)))
			.ForMember(src => src.UseWithTwoHandMace, ex => ex.MapFrom(dst => int.Parse(dst.vf_two_hand_mace)))
			.ForMember(src => src.UseWithLightbow, ex => ex.MapFrom(dst => int.Parse(dst.vf_lightbow)))
			.ForMember(src => src.UseWithHeavybow, ex => ex.MapFrom(dst => int.Parse(dst.vf_heavybow)))
			.ForMember(src => src.UseWithCrossbow, ex => ex.MapFrom(dst => int.Parse(dst.vf_crossbow)))
			.ForMember(src => src.UseWithOneHandStaff, ex => ex.MapFrom(dst => int.Parse(dst.vf_one_hand_staff)))
			.ForMember(src => src.UseWithTwoHandStaff, ex => ex.MapFrom(dst => int.Parse(dst.vf_two_hand_staff)))
			.ForMember(src => src.UseWithShieldOnly, ex => ex.MapFrom(dst => int.Parse(dst.vf_shield_only)))
			.ForMember(src => src.UseWithWeaponNotRequired,
				ex => ex.MapFrom(dst => int.Parse(dst.vf_is_not_need_weapon)))
			.ForMember(src => src.UseOnSelf, ex => ex.MapFrom(dst => int.Parse(dst.uf_self)))
			.ForMember(src => src.UseOnParty, ex => ex.MapFrom(dst => int.Parse(dst.uf_party)))
			.ForMember(src => src.UseOnGuild, ex => ex.MapFrom(dst => int.Parse(dst.uf_guild)))
			.ForMember(src => src.UseOnNeutral, ex => ex.MapFrom(dst => int.Parse(dst.uf_neutral)))
			.ForMember(src => src.UseOnPurple, ex => ex.MapFrom(dst => int.Parse(dst.uf_purple)))
			.ForMember(src => src.UseOnEnemy, ex => ex.MapFrom(dst => int.Parse(dst.uf_enemy)))
			.ForMember(src => src.UseOnCharacter, ex => ex.MapFrom(dst => int.Parse(dst.tf_avatar)))
			.ForMember(src => src.UseOnSummon, ex => ex.MapFrom(dst => int.Parse(dst.tf_summon)))
			.ForMember(src => src.UseOnMonster, ex => ex.MapFrom(dst => int.Parse(dst.tf_monster)))
			.ForMember(src => src.DelayCast, ex => ex.MapFrom(dst => dst.delay_cast))
			.ForMember(src => src.DelayCastPerSkl, ex => ex.MapFrom(dst => dst.delay_cast_per_skl))
			.ForMember(src => src.DelayCastModePerEnhance, ex => ex.MapFrom(dst => dst.delay_cast_mode_per_enhance))
			.ForMember(src => src.DelayCommon, ex => ex.MapFrom(dst => dst.delay_common))
			.ForMember(src => src.DelayCooltime, ex => ex.MapFrom(dst => dst.delay_cooltime))
			.ForMember(src => src.DelayCooltimePerSkl, ex => ex.MapFrom(dst => dst.delay_cooltime_per_skl))
			.ForMember(src => src.DelayCooltimeModePerEnhance, ex => ex.MapFrom(dst => dst.delay_cast_mode_per_enhance))
			.ForMember(src => src.CoolTimeGroupId, ex => ex.MapFrom(dst => dst.cool_time_group_id))
			.ForMember(src => src.SkillLvupLimit, ex => ex.MapFrom(dst => dst.skill_lvup_limit))
			.ForMember(src => src.Target, ex => ex.MapFrom(dst => (SkillTarget)dst.target))
			.ForMember(src => src.EffectType, ex => ex.MapFrom(dst => (EffectType)dst.effect_type))
			.ForMember(src => src.UpgradeIntoSkillId, ex => ex.MapFrom(dst => dst.skill_enchant_link_id))
			.ForMember(src => src.StateId, ex => ex.MapFrom(dst => dst.state_id))
			.ForMember(src => src.StateLevelBase, ex => ex.MapFrom(dst => dst.state_level_base))
			.ForMember(src => src.StateLevelPerSkill, ex => ex.MapFrom(dst => dst.state_level_per_skl))
			.ForMember(src => src.StateLevelPerEnhance, ex => ex.MapFrom(dst => dst.state_level_per_enhance))
			.ForMember(src => src.StateSecond, ex => ex.MapFrom(dst => dst.state_second))
			.ForMember(src => src.StateSecondPerLevel, ex => ex.MapFrom(dst => dst.state_second_per_level))
			.ForMember(src => src.StateSecondPerEnhance, ex => ex.MapFrom(dst => dst.state_second_per_enhance))
			.ForMember(src => src.ProbabilityOnHit, ex => ex.MapFrom(dst => dst.probability_on_hit))
			.ForMember(src => src.ProbabilityIncBySlv, ex => ex.MapFrom(dst => dst.probability_inc_by_slv))
			.ForMember(src => src.HitBonusPerEnhance, ex => ex.MapFrom(dst => dst.hit_bonus_per_enhance))
			.ForMember(src => src.HitBonus, ex => ex.MapFrom(dst => dst.hit_bonus))
			.ForMember(src => src.Percentage, ex => ex.MapFrom(dst => dst.percentage))
			.ForMember(src => src.HateMod, ex => ex.MapFrom(dst => dst.hate_mod))
			.ForMember(src => src.HateBasic, ex => ex.MapFrom(dst => dst.hate_basic))
			.ForMember(src => src.HatePerSkill, ex => ex.MapFrom(dst => dst.hate_per_skl))
			.ForMember(src => src.HatePerEnhance, ex => ex.MapFrom(dst => dst.hate_per_enhance))
			.ForMember(src => src.CriticalBonus, ex => ex.MapFrom(dst => dst.critical_bonus))
			.ForMember(src => src.CriticalBonusPerSkl, ex => ex.MapFrom(dst => dst.critical_bonus_per_skl))
			.ForMember(src => src.Values,
				ex => ex.MapFrom(dst => new decimal[]
				{
					dst.var1, dst.var2, dst.var3, dst.var4, dst.var5, dst.var6, dst.var7, dst.var8, dst.var9, dst.var10,
					dst.var11, dst.var12, dst.var13, dst.var14, dst.var15, dst.var16, dst.var17, dst.var18, dst.var19,
					dst.var20
				}))
			.ForMember(src => src.IconId, ex => ex.MapFrom(dst => dst.icon_id))
			.ForMember(src => src.IconFileName, ex => ex.MapFrom(dst => dst.icon_file_name))
			.ForMember(src => src.ProjectileAcceleration, ex => ex.MapFrom(dst => dst.projectile_acceleration))
			.ForMember(src => src.ProjectileSpeed, ex => ex.MapFrom(dst => dst.projectile_speed))
			.ReverseMap();

		CreateMap<MSSQLStateResource, StateResourceEntity>()
			.ForMember(src => src.Id, ex => ex.MapFrom(dst => dst.state_id))
			.ForMember(src => src.TextId, ex => ex.MapFrom(dst => dst.text_id))
			.ForMember(src => src.TooltipId, ex => ex.MapFrom(dst => dst.tooltip_id))
			.ForMember(src => src.IsHarmful, ex => ex.MapFrom(dst => int.Parse(dst.is_harmful)))
			.ForMember(src => src.StateTimeType, ex => ex.MapFrom(dst => (StateTimeType)dst.state_time_type))
			.ForMember(src => src.StateGroup, ex => ex.MapFrom(dst => (StateGroup)dst.state_group))
			.ForMember(src => src.TooltipId, ex => ex.MapFrom(dst => dst.tooltip_id))
			.ForMember(src => src.DuplicateGroup,
				ex => ex.MapFrom(dst => new int[]
					{ dst.duplicate_group_1, dst.duplicate_group_2, dst.duplicate_group_3 }))
			.ForMember(src => src.UseOnCharacter, ex => ex.MapFrom(dst => int.Parse(dst.uf_avatar)))
			.ForMember(src => src.UseOnSummon, ex => ex.MapFrom(dst => int.Parse(dst.uf_summon)))
			.ForMember(src => src.UseOnMonster, ex => ex.MapFrom(dst => int.Parse(dst.uf_monster)))
			.ForMember(src => src.ReiterationCount, ex => ex.MapFrom(dst => dst.reiteration_count))
			.ForMember(src => src.BaseEffect, ex => ex.MapFrom(dst => (StateBaseEffect)dst.base_effect_id))
			.ForMember(src => src.FireInterval, ex => ex.MapFrom(dst => dst.fire_interval))
			.ForMember(src => src.ElementalType, ex => ex.MapFrom(dst => (ElementalType)dst.elemental_type))
			.ForMember(src => src.EffectType, ex => ex.MapFrom(dst => (EffectType)dst.effect_type))
			.ForMember(src => src.AmplifyBase, ex => ex.MapFrom(dst => dst.amplify_base))
			.ForMember(src => src.AmplifyPerSkill, ex => ex.MapFrom(dst => dst.amplify_per_skl))
			.ForMember(src => src.AddDamageBase, ex => ex.MapFrom(dst => dst.add_damage_base))
			.ForMember(src => src.AddDamagePerSkl, ex => ex.MapFrom(dst => dst.add_damage_per_skl))
			.ForMember(src => src.EffectType, ex => ex.MapFrom(dst => (SkillEffectType)dst.elemental_type))
			.ForMember(src => src.IconId, ex => ex.MapFrom(dst => dst.icon_id))
			.ForMember(src => src.IconFileName, ex => ex.MapFrom(dst => dst.icon_file_name))
			.ForMember(src => src.FxId, ex => ex.MapFrom(dst => dst.fx_id))
			.ForMember(src => src.PosId, ex => ex.MapFrom(dst => dst.pos_id))
			.ForMember(src => src.SpecialOutputTimingId, ex => ex.MapFrom(dst => dst.special_output_timing_id))
			.ForMember(src => src.SpecialOutputFxId, ex => ex.MapFrom(dst => dst.special_output_fx_id))
			.ForMember(src => src.SpecialOutputFxPosId, ex => ex.MapFrom(dst => dst.special_output_fx_pos_id))
			.ForMember(src => src.Values,
				ex => ex.MapFrom(dst => new decimal[]
				{
					dst.value_0, dst.value_1, dst.value_2, dst.value_3, dst.value_4, dst.value_5, dst.value_6,
					dst.value_7, dst.value_8, dst.value_9, dst.value_10, dst.value_11, dst.value_12, dst.value_13,
					dst.value_14, dst.value_15, dst.value_16, dst.value_17, dst.value_18, dst.value_19
				}))
			.ReverseMap();

	}
}
