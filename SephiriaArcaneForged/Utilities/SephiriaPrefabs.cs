using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.Utilities
{
    public static class SephiriaPrefabs
    {
        public static CharacterDebuff Burn => CombatManager.Instance.burnDebuffPrefab;
        public static CharacterDebuff Electric => CombatManager.Instance.electricDebuffPrefab;
        public static CharacterDebuff Frostbite => CombatManager.Instance.frostbiteDebuffPrefab;
        public static CharacterDebuff Freeze => (Frostbite as CharacterDebuff_Frostbite).freezeDebuffPrefab;
        /// <summary>
        /// ピアオラ議事槌
        /// </summary>
        public static ScriptableFx RingFxPrefab
        {
            get
            {
                if (_ringFxPrefab == null)
                    _ringFxPrefab = WeaponDatabase.FindWeaponById(518).mainWeaponPrefab.GetComponent<WeaponAddonCommon_BurnRing>().ringFxPrefab;
                return _ringFxPrefab;
            }
        }
        private static ScriptableFx _ringFxPrefab;
        /// <summary>
        /// 灰の舞い
        /// </summary>
        public static GameObject DashAttackFxPrefab
        {
            get
            {
                if (_dashAttackFxPrefab == null)
                    _dashAttackFxPrefab = WeaponDatabase.FindWeaponById(6).mainWeaponPrefab.GetComponent<WeaponAddon_DashAttack>().dashAttackFxPrefab;
                return _dashAttackFxPrefab;
            }
        }
        private static GameObject _dashAttackFxPrefab;
        /// <summary>
        /// ソリス・ミッシオ
        /// </summary>
        public static GameObject[] SolisMissioFxPrefabs
        {
            get
            {
                if (_solisMissioFxPrefabs == null || _solisMissioFxPrefabs.Length == 0)
                    _solisMissioFxPrefabs = WeaponDatabase.FindWeaponById(1025).mainWeaponPrefab.GetComponent<WeaponSimple_SwordAndShield>().solisMissioFxPrefabs;
                return _solisMissioFxPrefabs;
            }
        }
        private static GameObject[] _solisMissioFxPrefabs;
        /// <summary>
        /// 敷居跨ぎのバフ効果
        /// </summary>
        public static CharacterBuff BasicAttackFinalBuffPrefab
        {
            get
            {
                if (_basicAttackFinalBuffPrefab == null)
                {
                    var weapon = WeaponDatabase.FindWeaponById(22);
                    if (weapon == null)
                        return null;
                    if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_Dagger>(out var dagger))
                        return null;
                    _basicAttackFinalBuffPrefab = dagger.basicAttackFinalBuffPrefab;
                }
                return _basicAttackFinalBuffPrefab;
            }
        }
        private static CharacterBuff _basicAttackFinalBuffPrefab;
        public static NewWeaponFireData DashAttackFireData_BladeZone
        {
            get
            {
                if (_dashAttackFireData_BladeZone == null)
                {
                    var weapon = WeaponDatabase.FindWeaponById(26);
                    if (weapon == null)
                        return null;
                    if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_Dagger>(out var dagger))
                        return null;
                    _dashAttackFireData_BladeZone = dagger.dashAttackFireData_BladeZone;
                }
                return _dashAttackFireData_BladeZone;
            }
        }
        private static NewWeaponFireData _dashAttackFireData_BladeZone;
        public static NewWeaponFireData LightningSpearFireData
        {
            get
            {
                if (_lightningSpearFireData == null)
                {
                    var weapon = WeaponDatabase.FindWeaponById(1018);
                    if (weapon == null)
                        return null;
                    if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_SwordAndShield>(out var sword))
                        return null;
                    _lightningSpearFireData = sword.specialAttacks.FirstOrDefault();
                }
                return _lightningSpearFireData;
            }
        }
        private static NewWeaponFireData _lightningSpearFireData;
        public static NewWeaponFireData CrystalExplosionFireData
        {
            get
            {
                if (_crystalExplosionFireData == null)
                {
                    var weapon = WeaponDatabase.FindWeaponById(510);
                    if (weapon == null)
                        return null;
                    if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_QuartterStaff>(out var sword))
                        return null;
                    _crystalExplosionFireData = sword.crystalExplosionFireData;
                }
                return _crystalExplosionFireData;
            }
        }
        private static NewWeaponFireData _crystalExplosionFireData;
        public static NewWeaponFireData HetaeFireData
        {
            get
            {
                if (_hetaeFireData == null)
                {
                    var weapon = WeaponDatabase.FindWeaponById(1026);
                    if (weapon == null)
                        return null;
                    if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponSimple_SwordAndShield>(out var sword))
                        return null;
                    _hetaeFireData = sword.haetaeStrikeAttackExplosionFireData;
                }
                return _hetaeFireData;
            }
        }
        private static NewWeaponFireData _hetaeFireData;
        /// <summary>
        /// 杖を伸ばすバフ効果
        /// </summary>
        public static CharacterBuff StaffExtendBuffPrefab
        {
            get
            {
                if (_staffExtendBuffPrefab == null)
                {
                    var weapon = WeaponDatabase.FindWeaponById(506);
                    if (weapon == null)
                        return null;
                    if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponAddonCommon_AttackBuff>(out var addon))
                        return null;
                    _staffExtendBuffPrefab = addon.buffPrefab;
                }
                return _staffExtendBuffPrefab;
            }
        }
        private static CharacterBuff _staffExtendBuffPrefab;
        /// <summary>
        /// 支配バフ
        /// </summary>
        public static CharacterBuff ThrowCompBuffPrefab
        {
            get
            {
                if (_throwCompBuffPrefab == null)
                {
                    var weapon = WeaponDatabase.FindWeaponById(1210);
                    if (weapon == null)
                        return null;
                    if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponAddonCommon_AttackBuff>(out var addon))
                        return null;
                    _throwCompBuffPrefab = addon.buffPrefab;
                }
                return _throwCompBuffPrefab;
            }
        }
        private static CharacterBuff _throwCompBuffPrefab;
        /// <summary>
        /// 14秒間ダメージ増幅20%
        /// </summary>
        public static CharacterBuff PerfectGuardBuffPrefab
        {
            get
            {
                if (_perfectGuardBuffPrefab == null)
                {
                    var weapon = WeaponDatabase.FindWeaponById(0005);
                    if (weapon == null)
                        return null;
                    if (!weapon.mainWeaponPrefab.TryGetComponent<WeaponAddon_PerfectGuardBuff>(out var addon))
                        return null;
                    _perfectGuardBuffPrefab = addon.buffPrefab;
                }
                return _perfectGuardBuffPrefab;
            }
        }
        private static CharacterBuff _perfectGuardBuffPrefab;
        /// <summary>
        /// LowCloudArea
        /// </summary>
        public static ScriptableFx LowCloudFxPrefab
        {
            get
            {
                if (_lowCloudFxPrefab == null)
                {
                    var gameObject = Resources.Load<GameObject>("ScriptableFx/LowCloudFx");
                    if (gameObject != null && gameObject.TryGetComponent<ScriptableFx_Sprite>(out var fx))
                        _lowCloudFxPrefab = fx;
                }
                return _lowCloudFxPrefab;
            }
        }
        private static ScriptableFx _lowCloudFxPrefab;
    }
}
