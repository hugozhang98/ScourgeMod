using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ScourgeMod.Common.DisplayWeapons
{
    public class DWRegistry
    {
        #region Mace ==================================================================================================================================
        public static readonly HashSet<int> DWVanillaID_Mace = new()
        {
            ItemID.Mace, //链锤
            ItemID.FlamingMace, //烈焰链锤
            ItemID.BallOHurt, //链球
            ItemID.TheMeatball, //血肉之球
            ItemID.BlueMoon, //蓝月
            ItemID.Sunfury, //阳炎之怒
            ItemID.DripplerFlail, //滴滴怪致残者
            ItemID.DaoofPow, //太极连枷
            ItemID.FlowerPow, //花之力
            ItemID.Flairon, //猪鲨链球
        };

        public static bool IsDW_Mace(Item item)
        {
            return item.ModItem is IDW_Mace || DWVanillaID_Mace.Contains(item.type);
        }
        #endregion

        #region ChainKnife ==================================================================================================================================
        public static readonly HashSet<int> DWVanillaID_ChainKnife = new()
        {
            ItemID.ChainKnife, //链刀
            ItemID.ChainGuillotines, //铁链血滴子
        };

        public static bool IsDW_ChainKnife(Item item)
        {
            return item.ModItem is IDW_ChainKnife || DWVanillaID_ChainKnife.Contains(item.type);
        }
        #endregion

        #region ChainGun ==================================================================================================================================
        public static readonly HashSet<int> DWVanillaID_ChainGun = new()
        {
            ItemID.Anchor, //锚
            ItemID.KOCannon, //致胜炮
            ItemID.GolemFist, //石巨人之拳
        };

        public static bool IsDW_ChainGun(Item item)
        {
            return item.ModItem is IDW_ChainGun || DWVanillaID_ChainGun.Contains(item.type);
        }
        #endregion

        #region Yoyo ==================================================================================================================================
        public static readonly HashSet<int> DWVanillaID_Yoyo = new()
        {
            ItemID.WoodYoyo, //木悠悠球
            ItemID.Rally, //对打球
            ItemID.CorruptYoyo, //抑郁球
            ItemID.CrimsonYoyo, //血脉球
            ItemID.JungleYoyo, //亚马逊球
            ItemID.Code1, //代码1球
            ItemID.HiveFive, //蜂巢球
            ItemID.Valor, //英勇球
            ItemID.Cascade, //喷流球
            ItemID.FormatC, //好胜球
            ItemID.Gradient, //渐变球
            ItemID.Chik, //吉克球
            ItemID.HelFire, //狱火球
            ItemID.Amarok, //冰雪悠悠球
            ItemID.Code2, //代码2球
            ItemID.Yelets, //叶列茨球
            ItemID.RedsYoyo, //Red的抛球
            ItemID.ValkyrieYoyo, //女武神悠悠球
            ItemID.Kraken, //克拉肯球
            ItemID.TheEyeOfCthulhu, //克苏鲁之眼
            ItemID.Terrarian, //泰拉悠悠球
        };

        public static bool IsDW_Yoyo(Item item)
        {
            return item.ModItem is IDW_Yoyo || DWVanillaID_Yoyo.Contains(item.type);
        }
        #endregion

        #region GreatSword ==================================================================================================================================
        public static readonly HashSet<int> DWVanillaID_GreatSword = new()
        {
            ItemID.BreakerBlade, //毁灭刃
            ItemID.IceSickle, //冰雪镰刀
            ItemID.DyeTradersScimitar, //异域弯刀
            ItemID.DeathSickle, //死神镰刀
            ItemID.Seedler, //种子弯刀
            ItemID.WaffleIron, //华夫饼烘烤模
            ItemID.OrichalcumSword, //山铜剑
            ItemID.MythrilSword, //秘银剑
            ItemID.TitaniumSword, //钛金剑
            ItemID.Cutlass, //短弯刀
            ItemID.CobaltSword, //钴剑
            ItemID.PalladiumSword, //钯金剑
            ItemID.AdamantiteSword, //精金剑
            ItemID.NightsEdge, //永夜刃
            ItemID.TrueNightsEdge, //真永夜刃
            ItemID.TheHorsemansBlade, //无头骑士剑
            ItemID.InfluxWaver, //波涌之刃
            ItemID.DD2SquireDemonSword, //地狱之剑
            ItemID.Excalibur, //断钢剑
            ItemID.TrueExcalibur, //真断钢剑
            ItemID.ChlorophyteSaber, //叶绿军刀
            ItemID.StarWrath, //狂星之怒
            ItemID.ChristmasTreeSword, //圣诞树剑
            ItemID.BloodButcherer, //血腥屠刀
            ItemID.FieryGreatsword, //火山
            ItemID.BladeofGrass, //草剑
            ItemID.Frostbrand, //寒霜剑
            ItemID.BeamSword, //光束剑
            ItemID.Bladetongue, //舌锋剑
            ItemID.ChlorophyteClaymore, //叶绿双刃刀
            ItemID.TerraBlade, //泰拉刃
            ItemID.DD2SquireBetsySword, //飞龙
            ItemID.Meowmere, //彩虹猫之刃
            ItemID.IceBlade, //冰雪刃
            ItemID.Starfury, //星怒
            ItemID.BeeKeeper, //养蜂人
            ItemID.NightsEdge, //永夜刃
            ItemID.CandyCaneSword, //糖棒剑
            ItemID.BoneSword, //骨剑
            ItemID.Flymeal, //臭虫剑
            ItemID.LightsBane, //魔光剑
            ItemID.EnchantedSword, //附魔剑
        };

        public static bool IsDW_GreatSword(Item item)
        {
            return item.ModItem is IDW_GreatSword || DWVanillaID_GreatSword.Contains(item.type);
        }
        #endregion

        #region Gun ==================================================================================================================================
        public static readonly HashSet<int> DWVanillaID_Gun = new()
        {
               ItemID.Minishark, //迷你鲨
               ItemID.RedRyder, //红莱德枪
               ItemID.Musket, //火枪
               ItemID.Boomstick,//三发猎枪
               ItemID.QuadBarrelShotgun, //四管霰弹枪
               ItemID.ClockworkAssaultRifle, //发条式突击步枪
               ItemID.Gatligator, //鳄鱼机关枪
               ItemID.Shotgun, //霰弹枪
               ItemID.OnyxBlaster, //玛瑙爆破枪
               ItemID.Megashark, //巨兽鲨
               ItemID.TacticalShotgun, //战术霰弹枪
               ItemID.SniperRifle,  //狙击步枪
               ItemID.CandyCornRifle, //玉米糖步枪
               ItemID.ChainGun, //链式机枪
               ItemID.Xenopopper,  //外星霰弹枪
               ItemID.VortexBeater,  //星旋机枪
               ItemID.SDMG, //太空海豚机枪
               ItemID.GrenadeLauncher, //榴弹发射器
               ItemID.ProximityMineLauncher, //感应雷发射器
               ItemID.RocketLauncher, //火箭发射器
               ItemID.NailGun,   //钉枪
               ItemID.Stynger,   //毒刺发射器
               ItemID.JackOLanternLauncher,  //杰克南瓜灯发射器
               ItemID.SnowmanCannon, //雪人炮
               ItemID.FireworksLauncher,//喜庆弹射器
               ItemID.ElectrosphereLauncher, //电圈发射器
               ItemID.Celeb2, //喜庆弹射器Mk2
               ItemID.Blowpipe,  //吹管
               ItemID.Sandgun,//沙枪
               ItemID.SnowballCannon,  //雪球炮
               ItemID.PainterPaintballGun, //彩弹枪
               ItemID.Harpoon, //鱼叉枪
               ItemID.StarCannon,    //星星炮
               ItemID.Blowgun, //吹箭筒
               //ItemID.DeadCellsBarrelLauncher,   //木桶发射器 rrzz
               ItemID.Toxikarp, //毒弹枪
               ItemID.DartPistol,   //飞镖手枪
               ItemID.DartRifle,  //飞镖步枪
               ItemID.CoinGun,  //钱币枪
               ItemID.SuperStarCannon,  //超级星星炮
               ItemID.Flamethrower,  //火焰喷射器
               ItemID.PiranhaGun,  //食人鱼枪
               ItemID.ElfMelter,  //精灵熔枪,
               ItemID.Uzi, //乌兹冲锋枪

        };

        public static bool IsDW_Gun(Item item)
        {
            return item.ModItem is IDW_Gun || DWVanillaID_Gun.Contains(item.type);
        }
        #endregion



        #region MiniGun ==================================================================================================================================
        public static readonly HashSet<int> DWVanillaID_MiniGun = new()
        {
               ItemID.FlintlockPistol, //燧发枪 
                  ItemID.TheUndertaker, //夺命枪 
               ItemID.Revolver, //左轮手枪 
                     ItemID.Handgun, //手枪 
               ItemID.PhoenixBlaster, //凤凰爆破枪 
               ItemID.PewMaticHorn, //气喇叭 
               ItemID.VenusMagnum,//维纳斯万能枪 

        };

        public static bool IsDW_MiniGun(Item item)
        {
            return item.ModItem is IDW_MiniGun || DWVanillaID_MiniGun.Contains(item.type);
        }
        #endregion

        //    #region BackSword ==================================================================================================================================
        //    public static readonly HashSet<int> VanillaBackSword = new()
        //    {
        //        //长矛
        //        ItemID.Spear,
        //        //三叉戟
        //        ItemID.Trident,
        //        //史莱姆长矛
        //        //ItemID.SlimeSpear, rrzz
        //        //风暴长矛
        //        ItemID.ThunderSpear,
        //        //腐叉
        //        ItemID.TheRottedFork,
        //        //剑鱼
        //        ItemID.Swordfish,
        //        //暗黑长枪
        //        ItemID.DarkLance,
        //        //钴薙刀
        //        ItemID.CobaltNaginata,
        //        //钯金刺矛
        //        ItemID.PalladiumPike,
        //        //秘银长戟
        //        ItemID.MythrilHalberd,
        //        //山铜长戟
        //        ItemID.OrichalcumHalberd,
        //        //精金关刀
        //        ItemID.AdamantiteGlaive,
        //        //钛金三叉戟
        //        ItemID.TitaniumTrident,
        //        //永恒之枪
        //        ItemID.Gungnir,
        //        //恐怖关刀
        //        ItemID.MonkStaffT2,
        //        //叶绿镋
        //        ItemID.ChlorophytePartisan,
        //        //蘑菇长矛
        //        ItemID.MushroomSpear,
        //        //黑曜石剑鱼
        //        ItemID.ObsidianSwordfish,
        //        //北极
        //        ItemID.NorthPole,
        //        //燧石
        //        //ItemID.DeadCellsFlint, rrzz
        //        //瞌睡章鱼
        //        ItemID.MonkStaffT1,
        //        //腐化者之戟
        //        ItemID.ScourgeoftheCorruptor,
        //        //破晓之光
        //        ItemID.DayBreak,
        //        //天顶剑
        //        ItemID.Zenith,
        //        //骑枪
        //        ItemID.JoustingLance,
        //        //暗影骑枪
        //        ItemID.ShadowJoustingLance,
        //        //神圣骑枪
        //        ItemID.HallowJoustingLance,
        //        //天龙之怒
        //        ItemID.MonkStaffT3,
        //        //瞌睡章鱼
        //        ItemID.MonkStaffT1,
        //    };

        //    public static bool IsBackSword(Item item) =>
        //        item.ModItem is IBackSword || VanillaBackSword.Contains(item.type);
        //    #endregion

        //    #region BackSword_Shoulder ==========================================================================================================================
        //    public static readonly HashSet<int> VanillaBackSword_Shoulder = new()
        //    {

        //        //针叶木剑
        //        ItemID.BorealWoodSword,
        //        //木剑
        //        ItemID.WoodenSword,
        //        //铜阔剑
        //        ItemID.CopperBroadsword,
        //        //棕榈木剑
        //        ItemID.PalmWoodSword,
        //        //红木剑
        //        ItemID.RichMahoganySword,
        //        //仙人掌剑
        //        ItemID.CactusSword,
        //        //锡宽剑
        //        ItemID.TinBroadsword,
        //        //暗影木剑
        //        ItemID.ShadewoodSword,
        //        //灰烬木剑
        //        ItemID.AshWoodSword,
        //        //芦苇呼吸管
        //        ItemID.BreathingReed,
        // ItemID.TentacleSpike, //触手钉锤

        //        //乌木剑
        //        ItemID.EbonwoodSword,
        //        //铅宽剑
        //        ItemID.LeadBroadsword,
        //        //钨宽剑
        //        ItemID.TungstenBroadsword,
        //        //金阔剑
        //        ItemID.GoldBroadsword,

        //        //猎鹰刃
        //        ItemID.FalconBlade,

        //        //铁阔剑
        //        ItemID.IronBroadsword,
        //        //银阔剑
        //        ItemID.SilverBroadsword,
        //

        //        //铂金宽剑
        //        ItemID.PlatinumBroadsword,
        //        //蝙蝠棍
        //        ItemID.BatBat,
        //        //武士刀
        //        ItemID.Katana,
        //        //村正
        //        ItemID.Muramasa,

        //        //颌骨剑
        //        ItemID.AntlionClaw,
        //        //珍珠木剑
        //        ItemID.PearlwoodSword,
        //        //时尚剪刀
        //        ItemID.StylistKilLaKillScissorsIWish,

        //变态人的刀
        //        ItemID.PsychoKnife,

        //钥匙剑
        //        ItemID.Keybrand,
        //        //精致手杖
        //        ItemID.TaxCollectorsStickOfDoom,
        //ItemID.SlapHand, //拍拍手
        //    ItemID.ZombieArm, //僵尸臂

        //ItemID.HamBat, //火腿棍
        //ItemID.PurpleClubberfish, //紫挥棒鱼
        //

        //        //蓝色陨石光剑
        //        ItemID.BluePhaseblade,
        //        //红色陨石光剑
        //        ItemID.RedPhaseblade,
        //        //绿色陨石光剑
        //        ItemID.GreenPhaseblade,
        //        //紫色陨石光剑
        //        ItemID.PurplePhaseblade,
        //        //白色陨石光剑
        //        ItemID.WhitePhaseblade,
        //        //黄色陨石光剑
        //        ItemID.YellowPhaseblade,
        //        //橙色陨石光剑
        //        ItemID.OrangePhaseblade,
        //        //蓝色晶光刃
        //        ItemID.BluePhasesaber,
        //        //红色晶光刃
        //        ItemID.RedPhasesaber,
        //        //绿色晶光刃
        //        ItemID.GreenPhasesaber,
        //        //紫色晶光刃
        //        ItemID.PurplePhasesaber,
        //        //白色晶光刃
        //        ItemID.WhitePhasesaber,
        //        //黄色晶光刃
        //        ItemID.YellowPhasesaber,
        //        //橙色晶光刃
        //        ItemID.OrangePhasesaber,
        //    };

        //    public static bool IsBackSword_Shoulder(Item item) =>
        //        item.ModItem is IBackSword_Shoulder || VanillaBackSword_Shoulder.Contains(item.type);
        //    #endregion

        //    #region BackSword_Waist ==========================================================================================================================
        //    public static readonly HashSet<int> VanillaBackSword_Waist = new()
        //    {
        //        //铜短剑
        //        ItemID.CopperShortsword,
        //        //锡短剑
        //        ItemID.TinShortsword,
        //        //铁短剑
        //        ItemID.IronShortsword,
        //        //铅短剑
        //        ItemID.LeadShortsword,
        //        //银短剑
        //        ItemID.SilverShortsword,
        //        //钨短剑
        //        ItemID.TungstenShortsword,
        //        //金短剑
        //        ItemID.GoldShortsword,
        //        //铂金短剑
        //        ItemID.PlatinumShortsword,
        //        //标尺
        //        ItemID.Ruler,
        //        //罗马短剑
        //        ItemID.Gladius,
        //        //泰拉魔刃
        //        ItemID.Terragrim,
        //        //Arkhalis剑
        //        ItemID.Arkhalis,
        //        //星光
        //        ItemID.PiercingStarlight,
        //        //日耀喷发剑
        //        ItemID.SolarEruption,
        //    };

        //    public static bool IsBackSword_Waist(Item item) =>
        //        item.ModItem is IBackSword_Waist || VanillaBackSword_Waist.Contains(item.type);
        //    #endregion

        //    #region BackMace ==========================================================================================================================
        //    public static readonly HashSet<int> VanillaBackMace = new()
        //    {

        //    };

        //    public static bool IsBackMace(Item item) =>
        //        item.ModItem is IBackMace || VanillaBackMace.Contains(item.type);
        //    #endregion

        //    #region BackYoyo ==========================================================================================================================
        //    public static readonly HashSet<int> VanillaBackYoyo = new()
        //    {
        //        //利刃手套
        //        ItemID.BladedGlove,
        //        //臭虎爪
        //        ItemID.FetidBaghnakhs,

        //    };

        //    public static bool IsBackYoyo(Item item) =>
        //        item.ModItem is IBackYoyo || VanillaBackYoyo.Contains(item.type);
        //    #endregion


    }
}


////悲剧雨伞
//ItemID.TragicUmbrella, rrzz
//    //伞
//ItemID.Umbrella,
////吸血鬼刀
//ItemID.VampireKnives,

////麦芽酒投掷器
//ItemID.AleThrowingGlove,

////真铜短剑
//ItemID.TrueCopperShortsword,

////木回旋镖
//ItemID.WoodenBoomerang,

////附魔回旋镖
//ItemID.EnchantedBoomerang,

////水果蛋糕旋刃
//ItemID.FruitcakeChakram,

////血腥砍刀
//ItemID.BloodyMachete,

////蘑菇回旋镖
//ItemID.Shroomerang,

////冰雪回旋镖
//ItemID.IceBoomerang,

////荆棘旋刃
//ItemID.ThornChakram,

////斧头回旋镖
//ItemID.Axearang,

////三尖回旋镖
//ItemID.Trimarang,

////战斗扳手
//ItemID.CombatWrench,

////烈焰回旋镖
//ItemID.Flamarang,

////飞刀
//ItemID.FlyingKnife,

////中士联盾
//ItemID.BouncingShield,

////光辉飞盘
//ItemID.LightDisc,

////香蕉回旋镖
//ItemID.Bananarang,

////圣骑士锤
//ItemID.PaladinsHammer,

////疯狂飞斧
//ItemID.PossessedHatchet,
