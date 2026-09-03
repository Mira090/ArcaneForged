using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_DarkCloudByDashAttack : ArcaneWeapon_StatusInstance
    {
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnDashAttack += OnDashAttack;
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnDashAttack -= OnDashAttack;
        }
        private void OnDashAttack(CombatBehaviour victim, DamageInstance damage, ProjectileBase projectile)
        {
            if (NetworkAvatar == null || NetworkAvatar.Inventory == null)
                return;
            UnitAvatar unitAvatar = victim as UnitAvatar;
            if (unitAvatar == null && unitAvatar.IsDead)
                return;
            ComboEffectBase comboEffectBase = NetworkAvatar.Inventory.FindComboEffect("DARKCLOUD");
            if (comboEffectBase != null)
            {
                ComboEffect_DarkCloud comboEffect_DarkCloud = comboEffectBase as ComboEffect_DarkCloud;
                if (comboEffect_DarkCloud != null)
                {
                    comboEffect_DarkCloud.UseCloudToTarget(unitAvatar, KeywordDatabase.GetConstValue("throwCloudBottleDamagePercent", true));
                }
            }
        }
    }
}
