using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Network.Packets;

public enum ResultCode : ushort
{
	Success = 0,

	NotExist = 1,
	TooFar = 2,
	NotOwn = 3,
	Misc = 4,
	NotActable = 5,
	AccessDenied = 6,
	Unknown = 7,
	DBError = 8,
	AlreadyExist = 9,
	NotEnoughMoney = 10,

	TooHeavy = 11,
	NotEnoughJP = 12,
	NotEnoughLevel = 13,
	NotEnoughJobLevel = 14,
	NotEnoughSkill = 15,
	LimitMax = 16,
	LimitMin = 17,
	InvalidPassword = 18,
	InvalidText = 19,
	NotEnoughHP = 20,

	NotEnoughMP = 21,
	CoolTime = 22,
	LimitWeapon = 23,
	LimitRace = 24,
	LimitJob = 25,
	LimitTarget = 26,
	NoSkill = 27,
	InvalidArgument = 28,
	PKLimit = 29,
	NotEnoughHavoc = 30,

	NotEnoughEnergy = 31,
	NotEnoughBullet = 32,
	NotEnoughEXP = 33,
	NotEnoughItem = 34,
	LimitRiding = 35,
	NotEnoughSP = 36,
	AlreadyStaminaSaved = 37,
	NotEnoughAge = 38,
	WithdrawWaiting = 39,
	RealnameRequired = 40,

	GametimeTiredStaminaSaver = 41,
	GametimeHarmfulStaminaSaver = 42,

	NotActableInSiegeOrRaid = 44,
	NotActableInSecroute = 45,
	NotActableInEventMap = 46,
	TargetInSiegeOrRaid = 47,
	TargetInSecroute = 48,
	TargetInEventMap = 49,
	TooCheap = 50,

	NotActableWhileUsingStorage = 51,
	NotActableWhileTrading = 52,
	TooMuchMoney = 53,
	PasswordMismatch = 54,
	NotActableWhileUsingBooth = 55,
	NotActableInHuntaholic = 56,
	TargetInHuntaholic = 57,
	NotEnoughHuntaholicPoint = 58,
	ActableOnlyInHuntaholic = 59,
	IPBlocked = 60,

	AlreadyInCompete = 61,
	NotInCompete = 62,
	WaitingCompeteRequestAnswer = 63,
	NotInCompetablePlace = 64,
	TargetAlreadyInCompete = 65,
	TargetNotInCompete = 66,
	TargetWaitingCompeteRequestAnswer = 67,
	TargetNotInCompeteablePlace = 68,
	NotActableHere = 69,

	GametimeLimited = 71,
	NotActableInDeathmatch = 72,
	ActableOnlyInDeathMatch = 73,
	BlockChat = 74,
	EnhanceLimit = 76,
	Pending = 77,

	NotActableInSecretDungeon = 78,
	TargetInSecretDungeon = 79,

	AlreadySuperSaver = 80,
	GametimeTiredSuperSaver = 81,
	GametimeHarmfulSuperSaver = 82,

	NotEnoughTP = 83,

	NotActableInInstanceDungeon = 84,
	ActableOnlyInInstanceDungeon = 85,
	TargetInInstanceDungeon = 86,
	TargetInDeathmatch = 87,
	TargetIsUsingStorage = 88,

	NotEnoughAgePeriod = 89,

	AlreadyTaming = 70,
	NotTamable = 90,
	TargetAlreadyBeingTamed = 91,
	NotEnoughTargetHP = 92,
	NotEnoughSummonCard = 93,

	Max
}