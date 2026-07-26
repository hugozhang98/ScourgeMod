using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ScourgeMod.Common.BackWeapons
{
    public class BackWeaponRegistry
    {
        #region BackSword ==================================================================================================================================
        public static readonly HashSet<int> VanillaBackSword = new()
        {
            //长矛
            ItemID.Spear,
            //三叉戟
            ItemID.Trident,
            //史莱姆长矛
            //ItemID.SlimeSpear,

            //风暴长矛
            ItemID.ThunderSpear,
            //腐叉
            ItemID.TheRottedFork,
            //剑鱼
            ItemID.Swordfish,
            //暗黑长枪
            ItemID.DarkLance,
            //钴薙刀
            ItemID.CobaltNaginata,
            //钯金刺矛
            ItemID.PalladiumPike,
            //秘银长戟
            ItemID.MythrilHalberd,
            //山铜长戟
            ItemID.OrichalcumHalberd,
            //精金关刀
            ItemID.AdamantiteGlaive,
            //钛金三叉戟
            ItemID.TitaniumTrident,
            //永恒之枪
            ItemID.Gungnir,
            //恐怖关刀
            ItemID.MonkStaffT2,
            //叶绿镋
            ItemID.ChlorophytePartisan,
            //蘑菇长矛
            ItemID.MushroomSpear,
            //黑曜石剑鱼
            ItemID.ObsidianSwordfish,
            //北极
            ItemID.NorthPole,
            //燧石
            //ItemID.DeadCellsFlint,
            //瞌睡章鱼
            ItemID.MonkStaffT1,
            //腐化者之戟
            ItemID.ScourgeoftheCorruptor,
            //破晓之光
            ItemID.DayBreak,
            //天顶剑
            ItemID.Zenith,
        };

        public static bool IsBackSword(Item item) =>
            item.ModItem is IBackSword || VanillaBackSword.Contains(item.type);
        #endregion

        #region BackSword_Shoulder ==========================================================================================================================
        public static readonly HashSet<int> VanillaBackSword_Shoulder = new()
        {
            //毁灭刃
            ItemID.BreakerBlade,
            //针叶木剑
            ItemID.BorealWoodSword,
            //木剑
            ItemID.WoodenSword,
            //铜阔剑
            ItemID.CopperBroadsword,
            //棕榈木剑
            ItemID.PalmWoodSword,
            //红木剑
            ItemID.RichMahoganySword,
            //仙人掌剑
            ItemID.CactusSword,
            //锡宽剑
            ItemID.TinBroadsword,
            //暗影木剑
            ItemID.ShadewoodSword,
            //灰烬木剑
            ItemID.AshWoodSword,
            //芦苇呼吸管
            ItemID.BreathingReed,
            //触手钉锤
            ItemID.TentacleSpike,
            //冰雪刃
            ItemID.IceBlade,
            //星怒
            ItemID.Starfury,
            //养蜂人
            ItemID.BeeKeeper,
            //乌木剑
            ItemID.EbonwoodSword,
            //铅宽剑
            ItemID.LeadBroadsword,
            //钨宽剑
            ItemID.TungstenBroadsword,
            //金阔剑
            ItemID.GoldBroadsword,
            //骨剑
            ItemID.BoneSword,
            //糖棒剑
            ItemID.CandyCaneSword,
            //魔光剑
            ItemID.LightsBane,
            //附魔剑
            ItemID.EnchantedSword,
            //猎鹰刃
            ItemID.FalconBlade,
            //永夜刃
            ItemID.NightsEdge,
            //铁阔剑
            ItemID.IronBroadsword,
            //银阔剑
            ItemID.SilverBroadsword,
            //僵尸臂
            ItemID.ZombieArm,
            //臭虫剑
            ItemID.Flymeal,
            //铂金宽剑
            ItemID.PlatinumBroadsword,
            //蝙蝠棍
            ItemID.BatBat,
            //武士刀
            ItemID.Katana,
            //村正
            ItemID.Muramasa,
            //血腥屠刀
            ItemID.BloodButcherer,
            //紫挥棒鱼
            ItemID.PurpleClubberfish,
            //草剑
            ItemID.BladeofGrass,
            //颌骨剑
            ItemID.AntlionClaw,
            //异域弯刀
            ItemID.DyeTradersScimitar,
            //时尚剪刀
            ItemID.StylistKilLaKillScissorsIWish,
            //火山
            ItemID.FieryGreatsword,
            //珍珠木剑
            ItemID.PearlwoodSword,
            //钴剑
            ItemID.CobaltSword,
            //冰雪镰刀
            ItemID.IceSickle,
            //山铜剑
            ItemID.OrichalcumSword,
            //寒霜剑
            ItemID.Frostbrand,
            //钛金剑
            ItemID.TitaniumSword,
            //火腿棍
            ItemID.HamBat,
            //真永夜刃
            ItemID.TrueNightsEdge,
            //真断钢剑
            ItemID.TrueExcalibur,
            //变态人的刀
            ItemID.PsychoKnife,
            //无头骑士剑
            ItemID.TheHorsemansBlade,
            //波涌之刃
            ItemID.InfluxWaver,
            //精致手杖
            ItemID.TaxCollectorsStickOfDoom,
            //钯金剑
            ItemID.PalladiumSword,
            //地狱之剑
            ItemID.DD2SquireDemonSword,
            //毁灭刃
            ItemID.BreakerBlade,
            //精金剑
            ItemID.AdamantiteSword,
            //断钢剑
            ItemID.Excalibur,
            //叶绿军刀
            ItemID.ChlorophyteSaber,
            //种子弯刀
            ItemID.Seedler,
            //钥匙剑
            ItemID.Keybrand,
            //圣诞树剑
            ItemID.ChristmasTreeSword,
            //狂星之怒
            ItemID.StarWrath,
            //拍拍手
            ItemID.SlapHand,
            //秘银剑
            ItemID.MythrilSword,
            //短弯刀
            ItemID.Cutlass,
            //光束剑
            ItemID.BeamSword,
            //舌锋剑
            ItemID.Bladetongue,
            //华夫饼烘烤模
            ItemID.WaffleIron,
            //叶绿双刃刀
            ItemID.ChlorophyteClaymore,
            //死神镰刀
            ItemID.DeathSickle,
            //泰拉刃
            ItemID.TerraBlade,
            //飞龙
            ItemID.DD2SquireBetsySword,
            //彩虹猫之刃
            ItemID.Meowmere,
            //蓝色陨石光剑
            ItemID.BluePhaseblade,
            //红色陨石光剑
            ItemID.RedPhaseblade,
            //绿色陨石光剑
            ItemID.GreenPhaseblade,
            //紫色陨石光剑
            ItemID.PurplePhaseblade,
            //白色陨石光剑
            ItemID.WhitePhaseblade,
            //黄色陨石光剑
            ItemID.YellowPhaseblade,
            //橙色陨石光剑
            ItemID.OrangePhaseblade,
            //蓝色晶光刃
            ItemID.BluePhasesaber,
            //红色晶光刃
            ItemID.RedPhasesaber,
            //绿色晶光刃
            ItemID.GreenPhasesaber,
            //紫色晶光刃
            ItemID.PurplePhasesaber,
            //白色晶光刃
            ItemID.WhitePhasesaber,
            //黄色晶光刃
            ItemID.YellowPhasesaber,
            //橙色晶光刃
            ItemID.OrangePhasesaber,
        };

        public static bool IsBackSword_Shoulder(Item item) =>
            item.ModItem is IBackSword_Shoulder || VanillaBackSword_Shoulder.Contains(item.type);
        #endregion

        #region BackSword_Waist ==========================================================================================================================
        public static readonly HashSet<int> VanillaBackSword_Waist = new()
        {
            //铜短剑
            ItemID.CopperShortsword,
            //锡短剑
            ItemID.TinShortsword,
            //铁短剑
            ItemID.IronShortsword,
            //铅短剑
            ItemID.LeadShortsword,
            //银短剑
            ItemID.SilverShortsword,
            //钨短剑
            ItemID.TungstenShortsword,
            //金短剑
            ItemID.GoldShortsword,
            //铂金短剑
            ItemID.PlatinumShortsword,
            //标尺
            ItemID.Ruler,
            //罗马短剑
            ItemID.Gladius,
            //泰拉魔刃
            ItemID.Terragrim,
            //Arkhalis剑
            ItemID.Arkhalis,
            //星光
            ItemID.PiercingStarlight,
            //日耀喷发剑
            ItemID.SolarEruption,
        };

        public static bool IsBackSword_Waist(Item item) =>
            item.ModItem is IBackSword_Waist || VanillaBackSword_Waist.Contains(item.type);
        #endregion

        #region BackMace ==========================================================================================================================
        public static readonly HashSet<int> VanillaBackMace = new()
        {
            //链锤
            ItemID.Mace,
            //烈焰链锤
            ItemID.FlamingMace,
            //链球
            ItemID.BallOHurt,
            //血肉之球
            ItemID.TheMeatball,
            //蓝月
            ItemID.BlueMoon,
            //阳炎之怒
            ItemID.Sunfury,
            //滴滴怪致残者
            ItemID.DripplerFlail,
            //太极连枷
            ItemID.DaoofPow,
            //花之力
            ItemID.FlowerPow,
            //猪鲨链球
            ItemID.Flairon,
            //链刀
            ItemID.ChainKnife,
            //锚
            ItemID.Anchor,
            //铁链血滴子
            ItemID.ChainGuillotines,
            //致胜炮
            ItemID.KOCannon,
            //石巨人之拳
            ItemID.GolemFist,
        };

        public static bool IsBackMace(Item item) =>
            item.ModItem is IBackMace || VanillaBackMace.Contains(item.type);
        #endregion
    }
}


//利刃手套
//ItemID.BladedGlove,
//臭虎爪
//ItemID.FetidBaghnakhs,

////悲剧雨伞
//ItemID.TragicUmbrella,
//    //伞
//ItemID.Umbrella,
