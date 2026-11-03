using FMODUnity;
using HarmonyLib;
using Mirror;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_DualSlash : ArcaneWeapon_Basic
    {
        public static readonly string Stat = "ARCANEWEAPONDUALSLASH";
        public static readonly string ForceBasicStat = "BASICATTACKDUALSLASH";
        public static readonly string SpaceBasicStat = "BASICATTACKSPACESLASH";

        public string stackDamageBonusConstKey = "GreatSwordTwinStackDamageBonus";

        public int finishRequiredStacks = 10;
        public int maxStacks = 10;
        public float powerAttackDuration = 3f;
        public string stackHudId = "";
        public UI_KatanaDashStack stackUIPrefab => SephiriaPrefabs.StackUIPrefab;
        private UI_KatanaDashStack stackUI;
        private int shownStackCount = -1;
        public EventReference powerAttackReadySound;
        [SyncVar]
        private int stacks;
        private bool stackHudCreated;
        private int consumedStacksForFinisher;
        public WeaponSimple weaponSimple;
        public int Networkstacks
        {
            get
            {
                return stacks;
            }
            [param: In]
            set
            {
                GeneratedSyncVarSetter(value, ref stacks, 4L, null);
            }
        }

        public override NewWeaponFireData FireData => SephiriaPrefabs.DualSlashSecondFireData;
        public override string DamageId => "ArcaneWeapon_SwordShieldDualSlash";
        public override float? DamageMultiplier => null;
        public override float AttackDashScale => 0f;
        protected override float ModifyDamage(float damage)
        {
            damage += damage * (float)NetworkAvatar.GetCustomStat(ECustomStat.BasicAttackDamageBonus) / 100f;
            return base.ModifyDamage(damage);
        }

        private void Start()
        {
            if ((bool)stackUIPrefab && !stackUI)
            {
                stackUI = UnityEngine.Object.Instantiate(stackUIPrefab, base.transform);
                stackUI.SetStackCount(0);
                stackUI.transform.localPosition += new Vector3(4, 0, 0);
                shownStackCount = 0;
            }
        }
        private void OnDestroy()
        {
            if ((bool)stackUI)
            {
                UnityEngine.Object.Destroy(stackUI.gameObject);
                stackUI = null;
            }
        }

        private void LateUpdate()
        {
            if (!IsEffectEnabled || !stackUI || !base.isClient)
            {
                return;
            }

            WeaponControllerSimple owner = WeaponController;
            int side;
            if ((bool)owner)
            {
                side = (owner.isOwned ? 1 : 0);
                if (side != 0)
                {
                    stackUI.transform.position = owner.transform.position;
                }
            }
            else
            {
                side = 0;
            }

            int amountClientside = ((side != 0) ? stacks : 0);
            if (amountClientside != shownStackCount)
            {
                shownStackCount = amountClientside;
                stackUI.SetStackCount(amountClientside);
            }
        }
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Stat, 1);
            Networkstacks = 0;
            WeaponController.OnSwingCreated += OnSwingCreated;
            WeaponController.OnBaisAttackSwing += OnBasicAttackSwing;

            if (weaponSimple != null)
            {
                weaponSimple.OnStartAttackServerside -= OnStartAttackServerside;
                weaponSimple = null;
            }
            weaponSimple = WeaponController.currentWeapon;
            if (weaponSimple != null)
            {
                weaponSimple.OnStartAttackServerside += OnStartAttackServerside;
            }
        }

        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe(Stat, -1);
            consumedStacksForFinisher = 0;
            SetStacks(0);
            WeaponController.OnSwingCreated -= OnSwingCreated;
            WeaponController.OnBaisAttackSwing -= OnBasicAttackSwing;
            if (weaponSimple != null)
            {
                weaponSimple.OnStartAttackServerside -= OnStartAttackServerside;
                weaponSimple = null;
            }
            var force = NetworkAvatar.GetCustomStatUnsafe(ForceBasicStat);
            if (force > 0)
            {
                NetworkAvatar.AddCustomStatUnsafe(ForceBasicStat, -force);
            }
            NetworkAvatar.DestroyEffectHUD("DualSlash");
            var space = NetworkAvatar.GetCustomStatUnsafe(SpaceBasicStat);
            if (space > 0)
            {
                NetworkAvatar.AddCustomStatUnsafe(SpaceBasicStat, -space);
            }
        }

        private void OnBasicAttackSwing(int idx)
        {
            if (!IsEffectEnabled)
            {
                return;
            }

            //Core.LoggerError("OnBasicAttackSwing: " + idx + "  final: " + WeaponController.currentWeapon.finalComboIdx);

            var force = NetworkAvatar.GetCustomStatUnsafe(ForceBasicStat);
            var space = NetworkAvatar.GetCustomStatUnsafe(SpaceBasicStat);
            if (space > 0)
            {
                NetworkAvatar.AddCustomStatUnsafe(SpaceBasicStat, -space);
            }

            if (force > 0 && idx == WeaponController.currentWeapon.finalComboIdx)
            {
                NetworkAvatar.AddCustomStatUnsafe(ForceBasicStat, -force);
                NetworkAvatar.DestroyEffectHUD("DualSlash");
                this.Delay(0.1f, () =>
                {
                    if (!IsEffectEnabled || NetworkAvatar == null || WeaponController == null || NetworkAvatar.IsDead)
                        return;
                    Attack();
                });
            }
            if (force > 0 || idx != WeaponController.currentWeapon.finalComboIdx)
            {
                SetStacks(stacks + 1);
            }
        }
        protected override void OnCreateAttack(int idx, ProjectileBase projectile)
        {
            SetStacks(stacks + 1);
        }
        private void OnSwingCreated(bool last, ProjectileBase projectile)
        {
            if (!IsEffectEnabled || !projectile || consumedStacksForFinisher <= 0)
            {
                return;
            }

            if (last)
            {
                int num = KeywordDatabase.GetConstValue(stackDamageBonusConstKey) * consumedStacksForFinisher;
                if (num > 0)
                {
                    projectile.defaultDamageRatio *= 1f + (float)num / 100f;
                }
            }
        }

        private void OnStartAttackServerside(int idx)
        {
            var force = NetworkAvatar.GetCustomStatUnsafe(ForceBasicStat);
            consumedStacksForFinisher = 0;
            if (idx == WeaponController.currentWeapon.finalComboIdx && force <= 0)
            {
                consumedStacksForFinisher = stacks;
                SetStacks(0);
                if(consumedStacksForFinisher >= finishRequiredStacks)
                {
                    NetworkAvatar.AddCustomStatUnsafe(SpaceBasicStat, 1);
                }
            }
        }
        private void SetStacks(int value)
        {
            Networkstacks = Mathf.Clamp(value, 0, maxStacks);
            if (string.IsNullOrEmpty(stackHudId))
            {
                return;
            }

            WeaponControllerSimple owner = WeaponController;
            if (!owner || !owner.unitAvatar)
            {
                return;
            }

            if (stacks > 0)
            {
                if (!stackHudCreated)
                {
                    owner.unitAvatar.CreateEffectHUD(stackHudId, "GreatswordDualSlashStack");
                    stackHudCreated = true;
                }

                owner.unitAvatar.SetEffectHUDValue("GreatswordDualSlashStack", stacks.ToString());
            }
            else if (stackHudCreated)
            {
                owner.unitAvatar.DestroyEffectHUD("GreatswordDualSlashStack");
                stackHudCreated = false;
            }
        }
        protected override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
        {
            base.SerializeSyncVars(writer, forceAll);
            if (forceAll)
            {
                writer.WriteVarInt(stacks);
                return;
            }
            writer.WriteVarULong(syncVarDirtyBits);

            if ((syncVarDirtyBits & 0x4L) != 0L)
            {
                writer.WriteVarInt(stacks);
            }
        }
        protected override void DeserializeSyncVars(NetworkReader reader, bool initialState)
        {
            base.DeserializeSyncVars(reader, initialState);
            if (initialState)
            {
                GeneratedSyncVarDeserialize(ref stacks, null, reader.ReadVarInt());
                return;
            }
            long num = (long)reader.ReadVarULong();

            if ((num & 0x4L) != 0L)
            {
                GeneratedSyncVarDeserialize(ref stacks, null, reader.ReadVarInt());
            }
        }

        [HarmonyPatch(typeof(WeaponControllerSimple), nameof(WeaponControllerSimple.CancelAction))]
        public static class ControllerPatch
        {
            static void Prefix(WeaponControllerSimple __instance)
            {
                if (!__instance.unitAvatar)
                    return;

                
                var force = __instance.unitAvatar.GetCustomStatUnsafe(ForceBasicStat);
                if (force > 0)
                {
                    __instance.unitAvatar.AddCustomStatUnsafe(ForceBasicStat, -force);
                    __instance.unitAvatar.DestroyEffectHUD("DualSlash");
                }
                var space = __instance.unitAvatar.GetCustomStatUnsafe(SpaceBasicStat);
                if (space > 0)
                {
                    __instance.unitAvatar.AddCustomStatUnsafe(SpaceBasicStat, -space);
                }
            }
        }
        [HarmonyPatch(typeof(WeaponSimple))]
        public static class SimplePatch
        {
            [HarmonyPatch(nameof(WeaponSimple.SubAttackButtonDown))]
            [HarmonyPrefix]
            static bool Down(WeaponSimple __instance)
            {
                if (__instance.Networkowner && __instance.Networkowner.unitAvatar && __instance.Networkowner.unitAvatar.GetCustomStatUnsafe(Stat) > 0)
                {
                    if (__instance.Networkowner.currentWeaponSwing >= 0 && __instance.Networkowner.currentWeaponSwing < __instance.finalComboIdx)
                    {
                        AttackFirstIdx(__instance);
                        return false;
                    }
                }

                return true;
            }
            [HarmonyPatch(nameof(WeaponSimple.GetBasicAttack))]
            [HarmonyPostfix]
            static void GetBasicAttack(WeaponSimple __instance, int idx, ref NewWeaponFireData __result)
            {
                if (!__instance.Networkowner || !__instance.Networkowner.unitAvatar)
                    return;
                if (idx != __instance.finalComboIdx)
                    return;
                if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe(ForceBasicStat) > 0)
                {
                    __result = SephiriaPrefabs.DualSlashBasicFireData;
                }
                else if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe(SpaceBasicStat) > 0)
                {
                    __result = SephiriaPrefabs.DualSlashFireData;
                }
                return;
            }
        }
        [HarmonyPatch(typeof(WeaponSimple_SwordAndShield))]
        public static class SwordAndShieldPatch
        {
            [HarmonyPatch(nameof(WeaponSimple_SwordAndShield.SubAttackButtonDown))]
            [HarmonyPrefix]
            static bool Down(WeaponSimple_SwordAndShield __instance)
            {
                if(__instance.Networkowner && __instance.Networkowner.unitAvatar && __instance.Networkowner.unitAvatar.GetCustomStatUnsafe(Stat) > 0)
                {
                    if(__instance.Networkowner.currentWeaponSwing >= 0 && __instance.Networkowner.currentWeaponSwing < __instance.finalComboIdx)
                    {
                        AttackFirstIdx(__instance);
                        return false;
                    }
                }

                return true;
            }
            [HarmonyPatch(nameof(WeaponSimple_SwordAndShield.GetBasicAttack))]
            [HarmonyPostfix]
            static void GetBasicAttack(WeaponSimple_SwordAndShield __instance, int idx, ref NewWeaponFireData __result)
            {
                if (!__instance.Networkowner || !__instance.Networkowner.unitAvatar)
                    return;
                if (idx != __instance.finalComboIdx)
                    return;
                if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe(ForceBasicStat) > 0)
                {
                    __result = SephiriaPrefabs.DualSlashBasicFireData;
                }
                else if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe(SpaceBasicStat) > 0)
                {
                    __result = SephiriaPrefabs.DualSlashFireData;
                }
                return;
            }
        }
        [HarmonyPatch(typeof(WeaponSimple_GreatSword))]
        public static class GreatSwordPatch
        {
            [HarmonyPatch(nameof(WeaponSimple_GreatSword.SubAttackButtonDown))]
            [HarmonyPrefix]
            static bool Down(WeaponSimple_GreatSword __instance)
            {
                if (__instance.Networkowner && __instance.Networkowner.unitAvatar && __instance.Networkowner.unitAvatar.GetCustomStatUnsafe(Stat) > 0)
                {
                    if (__instance.Networkowner.currentWeaponSwing >= 0 && __instance.Networkowner.currentWeaponSwing < __instance.finalComboIdx)
                    {
                        AttackFirstIdx(__instance);
                        return false;
                    }
                }

                return true;
            }
            [HarmonyPatch(nameof(WeaponSimple_GreatSword.SubAttackButtonUp))]
            [HarmonyPrefix]
            static bool Up(WeaponSimple_GreatSword __instance)
            {
                if (__instance.Networkowner && __instance.Networkowner.unitAvatar && __instance.Networkowner.unitAvatar.GetCustomStatUnsafe(Stat) > 0)
                {
                    if (__instance.Networkowner.currentWeaponSwing >= 0 && __instance.Networkowner.currentWeaponSwing < __instance.finalComboIdx)
                    {
                        return false;
                    }
                }

                return true;
            }
            [HarmonyPatch(nameof(WeaponSimple_GreatSword.GetBasicAttack))]
            [HarmonyPostfix]
            static void GetBasicAttack(WeaponSimple_GreatSword __instance, int idx, ref NewWeaponFireData __result)
            {
                if (!__instance.Networkowner || !__instance.Networkowner.unitAvatar)
                    return;
                if (idx != __instance.finalComboIdx)
                    return;
                if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe(ForceBasicStat) > 0)
                {
                    __result = SephiriaPrefabs.DualSlashBasicFireData;
                }
                else if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe(SpaceBasicStat) > 0)
                {
                    __result = SephiriaPrefabs.DualSlashFireData;
                }
                return;
            }
        }
        [HarmonyPatch(typeof(WeaponSimple_Dagger))]
        public static class DaggerPatch
        {
            [HarmonyPatch(nameof(WeaponSimple_Dagger.SubAttackButtonDown))]
            [HarmonyPrefix]
            static bool Down(WeaponSimple_Dagger __instance)
            {
                if (__instance.Networkowner && __instance.Networkowner.unitAvatar && __instance.Networkowner.unitAvatar.GetCustomStatUnsafe(Stat) > 0)
                {
                    if (__instance.Networkowner.currentWeaponSwing >= 0 && __instance.Networkowner.currentWeaponSwing < __instance.finalComboIdx)
                    {
                        AttackFirstIdx(__instance);
                        return false;
                    }
                }

                return true;
            }
        }
        [HarmonyPatch(typeof(WeaponSimple_Katana))]
        public static class KatanaPatch
        {
            [HarmonyPatch(nameof(WeaponSimple_Katana.SubAttackButtonDown))]
            [HarmonyPrefix]
            static bool Down(WeaponSimple_Katana __instance)
            {
                if (__instance.Networkowner && __instance.Networkowner.unitAvatar && __instance.Networkowner.unitAvatar.GetCustomStatUnsafe(Stat) > 0)
                {
                    if (__instance.Networkowner.currentWeaponSwing >= 0 && __instance.Networkowner.currentWeaponSwing < __instance.finalComboIdx)
                    {
                        AttackFirstIdx(__instance);
                        return false;
                    }
                }

                return true;
            }
            [HarmonyPatch(nameof(WeaponSimple_Katana.GetBasicAttack))]
            [HarmonyPostfix]
            static void GetBasicAttack(WeaponSimple_Katana __instance, int idx, ref NewWeaponFireData __result)
            {
                if (!__instance.Networkowner || !__instance.Networkowner.unitAvatar)
                    return;
                if (idx != __instance.finalComboIdx)
                    return;
                if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe(ForceBasicStat) > 0)
                {
                    __result = SephiriaPrefabs.DualSlashBasicFireData;
                }
                else if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe(SpaceBasicStat) > 0)
                {
                    __result = SephiriaPrefabs.DualSlashFireData;
                }
                return;
            }
        }
        [HarmonyPatch(typeof(WeaponSimple_QuartterStaff))]
        public static class QuartterStaffPatch
        {
            [HarmonyPatch(nameof(WeaponSimple_QuartterStaff.SubAttackButtonDown))]
            [HarmonyPrefix]
            static bool Down(WeaponSimple_QuartterStaff __instance)
            {
                if (__instance.Networkowner && __instance.Networkowner.unitAvatar && __instance.Networkowner.unitAvatar.GetCustomStatUnsafe(Stat) > 0)
                {
                    if (__instance.Networkowner.currentWeaponSwing >= 0 && __instance.Networkowner.currentWeaponSwing < __instance.finalComboIdx)
                    {
                        AttackFirstIdx(__instance);
                        return false;
                    }
                }

                return true;
            }
            [HarmonyPatch(nameof(WeaponSimple_QuartterStaff.GetBasicAttack))]
            [HarmonyPostfix]
            static void GetBasicAttack(WeaponSimple_QuartterStaff __instance, int idx, ref NewWeaponFireData __result)
            {
                if (!__instance.Networkowner || !__instance.Networkowner.unitAvatar)
                    return;
                if (idx != __instance.finalComboIdx)
                    return;
                if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe(ForceBasicStat) > 0)
                {
                    __result = SephiriaPrefabs.DualSlashBasicFireData;
                }
                else if (__instance.Networkowner.unitAvatar.GetCustomStatUnsafe(SpaceBasicStat) > 0)
                {
                    __result = SephiriaPrefabs.DualSlashFireData;
                }
                return;
            }
        }
        private static void AttackFirstIdx(WeaponSimple __instance)
        {
            //Core.LoggerError("AttackFirstIdx");
            //__instance.Networkowner.SetAnimationTrigger(AnimHashContainer.Instance.SweepHash);
            //__instance.Networkowner.ResetAnimationTrigger(AnimHashContainer.Instance.AttackHash);
            //__instance.Networkowner.animator.Play(AnimHashContainer.Instance.AttackHash);
            //__instance.Networkowner.animator.ResetControllerState();
            //__instance.Networkowner.SetAnimationTrigger(AnimHashContainer.Instance.AttackHash);

            __instance.Networkowner.unitAvatar.AddCustomStatUnsafe(ForceBasicStat, 1);
            __instance.Networkowner.unitAvatar.CreateEffectHUD(Data.DualSlashHUD.Id, "DualSlash");

        }
    }
}
