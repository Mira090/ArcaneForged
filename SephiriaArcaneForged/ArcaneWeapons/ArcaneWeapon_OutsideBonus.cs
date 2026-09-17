using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_OutsideBonus : ArcaneWeapon_StatusInstance
    {
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if (WeaponController.currentWeapon is WeaponSimple_GreatSword great)
            {
                great.NetworkoutsideBonus = true;
            }
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if (WeaponController.currentWeapon is WeaponSimple_GreatSword great)
            {
                great.NetworkoutsideBonus = false;
            }
        }
        [HarmonyPatch(typeof(WeaponSimple_GreatSword), "OnSweepSwingChanged")]
        public static class OutsideBonusPatch
        {
            static void Prefix(WeaponSimple_GreatSword __instance)
            {
                if (!__instance.outsideBonus || !__instance.isOwned)
                {
                    return;
                }
                if(__instance.outsideBonusFXPrefab == null)
                {
                    var outside = WeaponDatabase.FindWeaponById(1111);
                    if (outside == null || outside.mainWeaponPrefab == null)
                        return;
                    if(outside.mainWeaponPrefab.TryGetComponent<WeaponSimple_GreatSword>(out var great))
                    {
                        __instance.outsideBonusFXPrefab = great.outsideBonusFXPrefab;
                    }
                }
            }
        }
    }
}
