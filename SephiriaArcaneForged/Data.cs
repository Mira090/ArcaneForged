using FMOD.Studio;
using Miniscript;
using Mirror;
using SephiriaArcaneForged.ArcaneWeapons;
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
        /// 紅蛇の粉砕
        /// ArcaneWeapon_GreatSwordEmber_Affix
        /// 紅蛇の
        /// ArcaneWeapon_GreatSwordEmber_Effect
        /// <tag=WeaponAction_DirectAttack>時に<tag=Artifact>赤い蛇の目のクールダウンが1.5秒減少します。
        /// </summary>
        public static ModArcaneWeapon GreatSwordEmber { get; } = ModArcaneWeapon.CreateFlag("GreatSwordEmber", 1123, "REDSNAKEEYEATKCOOLDOWNBONUS", 15);
        /// <summary>
        /// ソリス・シネリス
        /// ArcaneWeapon_GreatSwordFlameSword_Affix
        /// 太陽の
        /// ArcaneWeapon_GreatSwordFlameSword_Effect
        /// <tag=FlameSword>が<tag=WeaponAction_DirectAttack>時に追加で{VALUE}回発動します。
        /// </summary>
        public static ModArcaneWeapon GreatSwordFlameSword { get; } = ModArcaneWeapon.CreateFlag("GreatSwordFlameSword", 1124, "FLAMESWORDADDITIONALATTACKFROMWEAPON");
        /// <summary>
        /// つららの剣
        /// ArcaneWeapon_GreatSwordGlacier_Affix
        /// つららの
        /// ArcaneWeapon_GreatSwordGlacier_Effect
        /// <tag=WeaponAction_DirectAttack>時、命中した敵に{PERCENT}の確率で<tag=Frostbite>を付与します。<tag=Freeze>のダメージが{VAL0}増加します。
        /// </summary>
        public static ModArcaneWeapon GreatSwordGlacier { get; } = ModArcaneWeapon.CreateDebuff("GreatSwordGlacier", 1106, "FROSTBITE", 50, "FREEZE_DAMAGE/20");
        /// <summary>
        /// 祓魔の大剣
        /// ArcaneWeapon_GreatSwordFrostRelic_Affix
        /// 祓魔の
        /// ArcaneWeapon_GreatSwordFrostRelic_Effect
        /// <tag=Artifact>祝詞の鞘で与えるダメージが{VALUE}%増加します。
        /// </summary>
        public static ModArcaneWeapon GreatSwordFrostRelic { get; } = ModArcaneWeapon.CreateFlag("GreatSwordFrostRelic", 1114, "AIRSLASHDAMAGE", 33);
        /// <summary>
        /// 深紅の竜巻
        /// ArcaneWeapon_GreatSwordWound_Affix
        /// 深紅の
        /// ArcaneWeapon_GreatSwordWound_Effect
        /// <tag=WeaponAction_DirectAttack>時、命中した敵に<tag=Debuff_Wound>を付与します。<tag=Debuff_Wound>の最大スタックが{VALUE}増加します。
        /// </summary>
        public static ModArcaneWeapon GreatSwordWound { get; } = ModArcaneWeapon.CreateDebuffFlag("GreatSwordWound", 1108, "WOUNDSTACK", 3, "WOUND", 100);
        /// <summary>
        /// 電撃大剣「S3G」
        /// ArcaneWeapon_GreatSwordMagitech_Affix
        /// 放電する
        /// ArcaneWeapon_GreatSwordMagitech_Effect
        /// <tag=WeaponAction_DirectAttack>時、命中した敵に{PERCENT}の確率で<tag=Electric>を付与します。<tag=ElectricDamage>が{VAL0}増加します。
        /// </summary>
        public static ModArcaneWeapon GreatSwordMagitech { get; } = ModArcaneWeapon.CreateDebuff("GreatSwordMagitech", 1113, "ELECTRIC", 50, "ELECTRIC_DAMAGE/20");
        /// <summary>
        /// 光無き刃
        /// ArcaneWeapon_DaggerDash_Affix
        /// 光無き
        /// ArcaneWeapon_DaggerDash_Effect
        /// <tag=Dash>を使用すると、<tag=WeaponAction_Dagger_BladeZone>を放ちます。
        /// </summary>
        public static ModArcaneWeapon DaggerDash { get; } = ModArcaneWeapon.CreateStats<ArcaneWeapon_EnhancedDashAttack>("DaggerDash", 26);
        /// <summary>
        /// 絶対羨望
        /// ArcaneWeapon_DaggerFinal_Affix
        /// 羨望の
        /// ArcaneWeapon_DaggerFinal_Effect
        /// <tag=WeaponAction_DashAttack>時、10秒間、<tag=WeaponAction_BasicAttack>ダメージ+20%、<tag=WeaponAction_DashAttack>ダメージ+30%のバフを獲得します。
        /// </summary>
        public static ModArcaneWeapon DaggerFinal { get; } = ModArcaneWeapon.CreateStats<ArcaneWeapon_BasicAttackFinal>("DaggerFinal", 27);
        /// <summary>
        /// 垂れ込める雷雲
        /// ArcaneWeapon_DaggerDarkCloud_Affix
        /// 垂れ込める
        /// ArcaneWeapon_DaggerDarkCloud_Effect
        /// <tag=DarkCloudDamage>が{VAL0}、<tag=LightningDamage>が{VAL1}増加します。
        /// </summary>
        public static ModArcaneWeapon DaggerDarkCloud { get; } = ModArcaneWeapon.CreateStats("DaggerDarkCloud", 1204, "DARK_CLOUD_DAMAGE/15", "LIGHTNING_DAMAGE/5");
        /// <summary>
        /// 雷の怒り
        /// ArcaneWeapon_DaggerMagitech_Affix
        /// 怒りの
        /// ArcaneWeapon_DaggerMagitech_Effect
        /// <tag=WeaponAction_DirectAttack>時、命中した敵に{PERCENT}の確率で<tag=Electric>を付与します。<tag=ElectricStack>が{VAL0}増加します。
        /// </summary>
        public static ModArcaneWeapon DaggerMagitech { get; } = ModArcaneWeapon.CreateDebuff("DaggerMagitech", 1205, "ELECTRIC", 40, "ELECTRIC_STACK/1");
        /// <summary>
        /// ソリス・ユバル
        /// ArcaneWeapon_CrossbowFlameSword_Affix
        /// 晴天の
        /// ArcaneWeapon_CrossbowFlameSword_Effect
        /// <tag=FinalWeaponDamage>が{VAL0}減少しますが、<tag=FlameSwordDamage>が{VAL1}増加します。
        /// </summary>
        public static ModArcaneWeapon CrossbowFlameSword { get; } = ModArcaneWeapon.CreateStats("CrossbowFlameSword", 124, "FINAL_WEAPONDAMAGE/-15", "FLAME_SWORD_DAMAGE/35");
        /// <summary>
        /// 巨大クロスボウ：急冷結晶
        /// ArcaneWeapon_CrossbowFrostRelic_Affix
        /// 極寒の
        /// ArcaneWeapon_CrossbowFrostRelic_Effect
        /// <tag=FrostRelicDamage>が{VAL0}増加します。
        /// </summary>
        public static ModArcaneWeapon CrossbowFrostRelic { get; } = ModArcaneWeapon.CreateStats("CrossbowFrostRelic", 113, "FROST_RELIC_DAMAGE/20");
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
        /// 剣斬刀：飛柳
        /// ArcaneWeapon_KatanaCritical_Affix
        /// 居合の
        /// ArcaneWeapon_KatanaCritical_Effect
        /// <tag=CriticalChance>が{VAL0}増加します。
        /// </summary>
        public static ModArcaneWeapon KatanaCritical { get; } = ModArcaneWeapon.CreateStats("KatanaCritical", 410, "CRITICAL/2000");
        /// <summary>
        /// ユイの短剣
        /// ArcaneWeapon_KatanaMagicCritical_Affix
        /// 煌びやかな
        /// ArcaneWeapon_KatanaMagicCritical_Effect
        /// <tag=MagicCriticalChance>が{VAL0}増加します。
        /// </summary>
        public static ModArcaneWeapon KatanaMagicCritical { get; } = ModArcaneWeapon.CreateStats("KatanaMagicCritical", 413, "MAGIC_CRITICAL/3000");
        /// <summary>
        /// ピアオラ議事槌
        /// ArcaneWeapon_StaffEmber_Affix
        /// 議事の
        /// ArcaneWeapon_StaffEmber_Effect
        /// <tag=BurnDamage>が{VAL0}減少しますが、{DEBUFF}付与時に{NAME}を生成します。
        /// </summary>
        public static ModArcaneWeapon StaffEmber { get; } = ModArcaneWeapon.CreateStats<ArcaneWeapon_BurnRing>("StaffEmber", 518, "BURN_DAMAGE/-15");
        /// <summary>
        /// ソリス・ブラカ
        /// ArcaneWeapon_StaffFlameSword_Affix
        /// 照り輝く
        /// ArcaneWeapon_StaffFlameSword_Effect
        /// <tag=FlameSword>回収時に追加で{VALUE}個回収されます。
        /// </summary>
        public static ModArcaneWeapon StaffFlameSword { get; } = ModArcaneWeapon.CreateFlag("StaffFlameSword", 519, "FLAMESWORDPICKBONUS");
        /// <summary>
        /// 巫女の予言
        /// ArcaneWeapon_StaffFrostRelic_Affix
        /// 巫女の
        /// ArcaneWeapon_StaffFrostRelic_Effect
        /// <tag=FrostRelicDamage>が{VAL0}減少しますが、<tag=WeaponAction_DirectAttack>を3回命中させると、狙った方向に<tag=Artifact>ヴォルスパを発動します。
        /// </summary>
        public static ModArcaneWeapon StaffFrostRelic { get; } = ModArcaneWeapon.CreateStatsFlag("StaffFrostRelic", 522, "ICESPEARWITHWEAPONATTACK", 1, "FROST_RELIC_DAMAGE/-15");
        /// <summary>
        /// マラスピナ
        /// ArcaneWeapon_StaffGlacier_Affix
        /// 凍える
        /// ArcaneWeapon_StaffGlacier_Effect
        /// <tag=WeaponAction_DirectAttack>時、命中した敵に{PERCENT}の確率で<tag=Frostbite>を付与します。<tag=IceDamage>が{VAL0}増加します。
        /// </summary>
        public static ModArcaneWeapon StaffGlacier { get; } = ModArcaneWeapon.CreateDebuff("StaffGlacier", 523, "FROSTBITE", 30, "ICE_DAMAGE/5");


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
