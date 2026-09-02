using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged
{
    public static class Events
    {
        #region Version
        [HarmonyPatch(typeof(Application), nameof(Application.version), MethodType.Getter)]
        public static class GameVersionPatch
        {
            static void Postfix(ref string __result)
            {
                __result += ".: " + Core.Instance.metadata.modName + " v" + Core.Instance.metadata.modVersion;
            }
        }
        #endregion
        

        public static readonly string ChangeToFire = "WeaponToFireDamage".ToUpperInvariant();
        public static readonly string ChangeToIce= "WeaponToIceDamage".ToUpperInvariant();
        public static readonly string ChangeToLightning = "WeaponToLightningDamage".ToUpperInvariant();
        public static readonly string ChangeToChaos = "WeaponToChaosDamage".ToUpperInvariant();

        [HarmonyPatch(typeof(WeaponSimple), "GetRelatedStatMultiplier")]
        public static class RelatedStatPatch
        {
            static void Prefix(UnitAvatar owner, ref string relatedStatFormula)
            {
                if (owner.GetCustomStatUnsafe(ChangeToFire) > 0)
                    relatedStatFormula = "FireDamage".ToUpperInvariant();
                if (owner.GetCustomStatUnsafe(ChangeToIce) > 0)
                    relatedStatFormula = "IceDamage".ToUpperInvariant();
                if (owner.GetCustomStatUnsafe(ChangeToLightning) > 0)
                    relatedStatFormula = "LightningDamage".ToUpperInvariant();
            }
        }
        static void ModifyElementalType(UnitAvatar owner, ref EDamageElementalType elementalType)
        {
            if (owner == null)
                return;
            if (owner.GetCustomStatUnsafe(ChangeToFire) > 0)
                elementalType = EDamageElementalType.Fire;
            if (owner.GetCustomStatUnsafe(ChangeToIce) > 0)
                elementalType = EDamageElementalType.Ice;
            if (owner.GetCustomStatUnsafe(ChangeToLightning) > 0)
                elementalType = EDamageElementalType.Lightning;
            if (owner.GetCustomStatUnsafe(ChangeToChaos) > 0)
                elementalType = EDamageElementalType.Chaos;
        }
        [HarmonyPatch(typeof(NewWeaponFireData), "InstantiateProjectile")]
        public static class ProjectilePatch
        {
            static void Prefix(UnitAvatar owner, ref EDamageElementalType elementalType)
            {
                ModifyElementalType(owner, ref elementalType);
            }
        }
        [HarmonyPatch(typeof(NewWeaponFireData_MeleeAttack), "InstantiateProjectile")]
        public static class ProjectilePatch2
        {
            static void Prefix(UnitAvatar owner, ref EDamageElementalType elementalType)
            {
                ModifyElementalType(owner, ref elementalType);
            }
        }
        [HarmonyPatch(typeof(NewWeaponFireData_Bullet), "InstantiateProjectile")]
        public static class ProjectilePatch3
        {
            static void Prefix(UnitAvatar owner, ref EDamageElementalType elementalType)
            {
                ModifyElementalType(owner, ref elementalType);
            }
        }
        [HarmonyPatch(typeof(NewWeaponFireData_BulletBurst), "InstantiateProjectile")]
        public static class ProjectilePatch4
        {
            static void Prefix(UnitAvatar owner, ref EDamageElementalType elementalType)
            {
                ModifyElementalType(owner, ref elementalType);
            }
        }
        [HarmonyPatch(typeof(NewWeaponFireData_BulletSpread), "InstantiateProjectile")]
        public static class ProjectilePatch5
        {
            static void Prefix(UnitAvatar owner, ref EDamageElementalType elementalType)
            {
                ModifyElementalType(owner, ref elementalType);
            }
        }
        //[HarmonyPatch(typeof(NewWeaponFireData_Bullet_MiniDrone), "InstantiateProjectile")]
        public static class ProjectilePatch6
        {
            static void Prefix(UnitAvatar owner, ref EDamageElementalType elementalType)
            {
                ModifyElementalType(owner, ref elementalType);
            }
        }
        [HarmonyPatch(typeof(NewWeaponFireData_SpecialProjectile), "InstantiateProjectile")]
        public static class ProjectilePatch7
        {
            static void Prefix(UnitAvatar owner, ref EDamageElementalType elementalType)
            {
                ModifyElementalType(owner, ref elementalType);
            }
        }
        [HarmonyPatch(typeof(NewWeaponFireData_Summon), "InstantiateProjectile")]
        public static class ProjectilePatch8
        {
            static void Prefix(UnitAvatar owner, ref EDamageElementalType elementalType)
            {
                ModifyElementalType(owner, ref elementalType);
            }
        }
    }
}
