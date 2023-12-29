using System;

namespace Navislamia.Game.DataAccess.Entities.Enums;

[Flags]
public enum GuildPermissions
{
    SetPermissionName = 1,
    MemberInvite = 2,
    MemberKick = 4,
    GrantRevokePermission = 8,
    DungeonManagement = 16,
    RequestDungeonRaid = 32,
    AttackTeamCreate = 64,
    AttackTeamJoin = 128,
    Notice = 256,
    MemberMemo = 512,
    AllianceManagement = 1024,
    AllianceJoin = 2048,
    AllianceLeave = 4096,
    UpdateGuildIcon = 8192,
    AdvertiseManagement = 16384
}