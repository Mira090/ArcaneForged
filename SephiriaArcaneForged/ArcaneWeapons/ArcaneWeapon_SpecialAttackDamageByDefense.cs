using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_SpecialAttackDamageByDefense : ArcaneWeapon_StatusInstance
    {
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnSwingCreated_Special += OnSwingCreated_Special;
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnSwingCreated_Special -= OnSwingCreated_Special;
        }
        private void OnSwingCreated_Special(ProjectileBase projectile)
        {
            projectile.additionalDamagePercent += NetworkAvatar.GetCustomStat(ECustomStat.DamageReduction) / 2;
        }
    }
}
