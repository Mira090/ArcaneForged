using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.Utilities
{
    public static class SephiriaPrefabs
    {
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
