using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
