using System;
using System.Collections.Generic;
using System.Text;
using static SephiriaArcaneForged.Registries.ModArcaneWeapon;

namespace SephiriaArcaneForged.Registries
{
    public static class RegistryExtensions
    {
        public static T SetDamageId<T>(this T item) where T : ModArcaneWeapon
        {
            item.DamageId = ModDamageId.CreateArcaneWeapon(item.Name);
            return item;
        }
        public static T SetEffect<T>(this T item, string key) where T : ModArcaneWeapon
        {
            item.EffectString = new LocalizedString(key);
            return item;
        }
        public static List<int> NoWhirlwindGreatSwords = new List<int> { 1119, 1102, 1103, 1121, 0017, 1117, 1118, 1120 };
        public static T SetCondition<T>(this T item, EConditionType type) where T : ModArcaneWeapon
        {
            item.ConditionText = new LocalizedString("ArcaneWeapon_ConditionType_" + type.ToString());
            if(type == EConditionType.HasGuard)
            {
                item.Condition = current => current.mainWeaponPrefab.TryGetComponent<WeaponSimple_SwordAndShield>(out var _) || current.mainWeaponPrefab.TryGetComponent<WeaponSimple_QuartterStaff>(out var _);
            }
            else if(type == EConditionType.HasReload)
            {
                item.Condition = current => current.mainWeaponPrefab.TryGetComponent<WeaponSimple_Crossbow>(out var _);
            }
            else if (type == EConditionType.HasPary || type == EConditionType.HasFury)
            {
                item.Condition = current => current.mainWeaponPrefab.TryGetComponent<WeaponSimple_Dagger>(out var _);
            }
            else if (type == EConditionType.HasSweep)
            {
                item.Condition = current => current.mainWeaponPrefab.TryGetComponent<WeaponSimple_SwordAndShield>(out var _);
            }
            else if (type == EConditionType.HasWhirlwind)
            {
                item.Condition = current => current.mainWeaponPrefab.TryGetComponent<WeaponSimple_GreatSword>(out var _);
            }
            else if (type == EConditionType.HasSealth)
            {
                item.Condition = current => current.mainWeaponPrefab.TryGetComponent<WeaponSimple_Katana>(out var _);
            }
            else if (type == EConditionType.UseWhirlwind)
            {
                item.Condition = current => current.mainWeaponPrefab.TryGetComponent<WeaponSimple_GreatSword>(out var _) && !NoWhirlwindGreatSwords.Contains(current.id);
            }
            else if (type == EConditionType.AdditionalElementalDamage)
            {
                item.Condition = current => current.mainWeaponPrefab.TryGetComponent<WeaponAddonCommon_AdditionalElementalDamage>(out var _);
            }
            return item;
        }
    }
}
