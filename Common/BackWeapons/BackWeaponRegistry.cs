using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace ScourgeMod.Common.BackWeapons
{
    public class BackWeaponRegistry
    {
        /// <summary>
        /// 背负剑类武器的原版武器 ID 集合
        /// </summary>
        public static readonly HashSet<int> VanillaBackSword = new()
        {
            //木剑
            ItemID.WoodenSword,
        };

        /// <summary>
        /// 是否是背负剑类武器
        /// </summary>
        public static bool IsBackSword(Item item)
        {
            // 模组物品：检查接口
            if (item.ModItem is IBackSword)
                return true;

            // 原版物品：检查 ID 集合
            return VanillaBackSword.Contains(item.type);
        }

        /// <summary>
        /// 背负剑类武器的原版武器 ID 集合（肩扛）
        /// </summary>
        public static readonly HashSet<int> VanillaBackSword_Shoulder = new()
        {
            //毁灭刃
            ItemID.BreakerBlade,
        };

        /// <summary>
        /// 是否是背负剑类武器
        /// </summary>
        public static bool IsBackSword_Shoulder(Item item)
        {
            // 模组物品：检查接口
            if (item.ModItem is IBackSword_Shoulder)
                return true;

            //HashSet<int> VanillaBackSword = new()
            //{
            //    ItemID.WoodenSword,
            //}; //rrzz

            // 原版物品：检查 ID 集合
            return VanillaBackSword_Shoulder.Contains(item.type);
        }
    }
}
