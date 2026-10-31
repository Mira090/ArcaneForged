using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_EnhancedAttackSpeed : ArcaneWeapon_Basic
    {
        public float attackSpeedAmplify = 0.2f;
        public WeaponSimple weaponSimple;

        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if(weaponSimple != null)
            {
                WeaponController.currentWeapon.NetworkattackSpeedAmplify -= attackSpeedAmplify;
                weaponSimple = null;
            }
            weaponSimple = WeaponController.currentWeapon;
            if(weaponSimple != null)
            {
                WeaponController.currentWeapon.NetworkattackSpeedAmplify += attackSpeedAmplify;
            }
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if (weaponSimple != null)
            {
                WeaponController.currentWeapon.NetworkattackSpeedAmplify -= attackSpeedAmplify;
                weaponSimple = null;
            }
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            return new Loc.KeywordValue[]
            {
                new Loc.KeywordValue("PERCENT", (attackSpeedAmplify * 100).ToString() + "%")
            };
        }
    }
}
