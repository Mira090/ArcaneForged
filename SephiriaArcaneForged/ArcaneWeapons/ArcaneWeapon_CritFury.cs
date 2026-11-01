using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_CritFury : ArcaneWeapon_StatusInstance
    {
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if (WeaponController.currentWeapon is WeaponSimple_Dagger dagger)
            {
                dagger.critFury = true;
            }
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if (WeaponController.currentWeapon is WeaponSimple_Dagger dagger)
            {
                dagger.critFury = false;
            }
        }
    }
}
