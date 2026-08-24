using FMOD.Studio;
using Miniscript;
using Mirror;
using SephiriaArcaneForged.Registries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SephiriaArcaneForged
{
    public static class Data
    {
        public static List<ModArcaneWeapon> ArcaneWeapons { get; private set; } = new List<ModArcaneWeapon>();


        /// <summary>
        /// 火炎の視線
        /// ArcaneWeapon_WandFire_Affix
        /// 火炎の
        /// ArcaneWeapon_WandFire_Effect
        /// <tag=WeaponAction_DirectAttack>が<tag=FireDamage>ベースに変更されます。
        /// </summary>
        public static ModArcaneWeapon WandFire { get; } = ModArcaneWeapon.CreateFlag("WandFire", 1008, Events.ChangeToFire);
        /// <summary>
        /// 氷の息吹
        /// ArcaneWeapon_WandIce_Affix
        /// 氷の
        /// ArcaneWeapon_WandIce_Effect
        /// <tag=WeaponAction_DirectAttack>が<tag=IceDamage>ベースに変更されます。
        /// </summary>
        public static ModArcaneWeapon WandIce { get; } = ModArcaneWeapon.CreateFlag("WandIce", 1009, Events.ChangeToIce);
        /// <summary>
        /// 雷の羽ばたき
        /// ArcaneWeapon_WandLightning_Affix
        /// 雷の
        /// ArcaneWeapon_WandLightning_Effect
        /// <tag=WeaponAction_DirectAttack>が<tag=LightningDamage>ベースに変更されます。
        /// </summary>
        public static ModArcaneWeapon WandLightning { get; } = ModArcaneWeapon.CreateFlag("WandLightning", 1010, Events.ChangeToLightning);
        /// <summary>
        /// 垂れ込める雷雲
        /// ArcaneWeapon_DaggerDarkCloud_Affix
        /// 垂れ込める
        /// ArcaneWeapon_DaggerDarkCloud_Effect
        /// <tag=DarkCloudDamage>が{VAL0}、<tag=LightningDamage>が{VAL1}増加します。
        /// </summary>
        public static ModArcaneWeapon DaggerDarkCloud { get; } = ModArcaneWeapon.CreateStats("DaggerDarkCloud", 1204, "DARK_CLOUD_DAMAGE/15", "LIGHTNING_DAMAGE/5");
        /// <summary>
        /// 紫甲蜂弩
        /// ArcaneWeapon_CrossbowPoison_Affix
        /// 毒々しい
        /// ArcaneWeapon_CrossbowPoison_Effect
        /// <tag=Debuff_Poison>のスタックが{VALUE}増加します。
        /// </summary>
        public static ModArcaneWeapon CrossbowPoison { get; } = ModArcaneWeapon.CreateFlag("CrossbowPoison", 121, "PoisonStack".ToUpperInvariant(), 3);
        /// <summary>
        /// 燃え盛るヘルバヌス
        /// ArcaneWeapon_KatanaEmber_Affix
        /// 燃え盛る
        /// ArcaneWeapon_KatanaEmber_Effect
        /// <tag=BurnStack>が{VAL0}増加し、<tag=FireDamage>が{VAL1}増加します。
        /// </summary>
        public static ModArcaneWeapon KatanaEmber { get; } = ModArcaneWeapon.CreateStats("KatanaEmber", 407, "BURN_STACK/2", "FIRE_DAMAGE/4");
        /// <summary>
        /// ソリス・ブラカ
        /// ArcaneWeapon_StaffFlameSword_Affix
        /// 太陽槍の
        /// ArcaneWeapon_StaffFlameSword_Effect
        /// <tag=FlameSword>回収時に追加で{VALUE}個回収されます。
        /// </summary>
        public static ModArcaneWeapon StaffFlameSword { get; } = ModArcaneWeapon.CreateFlag("StaffFlameSword", 519, "FLAMESWORDPICKBONUS");


        public static void Init()
        {
            var type = typeof(Data);
            uint assetId = GetFirstAssetId();
            var pros6 = type.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModArcaneWeapon) || p.PropertyType.IsSubclassOf(typeof(ModArcaneWeapon)));
            foreach (var pro in pros6)
            {
                var moditem = pro.GetValue(type) as ModArcaneWeapon;
                Core.Logger("New Arcane Weapon: " + pro.Name);
                moditem.Init(assetId);
                assetId = GetNextAssetId(assetId);
                ArcaneWeapons.Add(moditem);
            }
        }

        public static void RegisterArcaneWeapons()
        {
            foreach(var mod in ArcaneWeapons)
            {
                ArcaneWeaponDatabase.Register(mod.ArcaneWeaponEntity);
            }
        }
        public static uint GetFirstAssetId()
        {
            return 520;
        }
        public static uint GetNextAssetId(uint previous)
        {
            do
            {
                previous++;
            }
            while (NetworkClient.prefabs.ContainsKey(previous));
            return previous;
        }
    }
}
