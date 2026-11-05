using HarmonyLib;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_MagicDuplicate : ArcaneWeapon_Flag
    {
        public static readonly string Duplicate = "ChangeFuryToMagicDuplicate".ToSephiriaUpperId();
        public static readonly string Release = "ChangeFuryToMagicRelease".ToSephiriaUpperId();

        [HarmonyPatch(typeof(WeaponSimple_Dagger))]
        public static class DaggerPatch
        {
            [HarmonyPatch(nameof(WeaponSimple_Dagger.GetSpecialAttack))]
            [HarmonyPostfix]
            static void GetBasicAttack(WeaponSimple_Dagger __instance, int idx, ref NewWeaponFireData __result)
            {
                if (!__instance.Networkowner || !__instance.Networkowner.unitAvatar)
                    return;
                if (idx == 0 && __instance.Networkowner.unitAvatar.GetCustomStatUnsafe(Duplicate) > 0)
                {
                    __result = SephiriaPrefabs.MagicDuplicateFireData;
                }
                else if (idx == 0 && __instance.Networkowner.unitAvatar.GetCustomStatUnsafe(Release) > 0)
                {
                    __result = SephiriaPrefabs.MagicReleaseFireData;
                }
                return;
            }
            [HarmonyPatch(nameof(WeaponSimple_Dagger.FuryCost), MethodType.Getter)]
            [HarmonyPostfix]
            static void FuryCost(WeaponSimple_Dagger __instance, ref int  __result)
            {
                if (!__instance.Networkowner || !__instance.Networkowner.unitAvatar)
                    return;
                if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe(Duplicate) > 0)
                {
                    __result = KeywordDatabase.GetConstValue("furyMagicCost");
                }
                else if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe(Release) > 0)
                {
                    __result = __instance.Networkowner.unitAvatar.MP;
                }
                return;
            }
        }
    }
}
