using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_SuperQuick : ArcaneWeapon_Flag
    {
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if(WeaponController.currentWeapon is WeaponSimple_GreatSword great)
            {
                great.superQuickSweep = true;
            }
            NetworkAvatar.AddCustomStat(ECustomStat.SpecialAttackSpeed, value);
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if (WeaponController.currentWeapon is WeaponSimple_GreatSword great)
            {
                great.superQuickSweep = false;
            }
            NetworkAvatar.AddCustomStat(ECustomStat.SpecialAttackSpeed, -value);
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            return new Loc.KeywordValue[]
            {
                new Loc.KeywordValue("VALUE", value.ToString().Replace("-", "") + "%")
            };
        }
    }
}
