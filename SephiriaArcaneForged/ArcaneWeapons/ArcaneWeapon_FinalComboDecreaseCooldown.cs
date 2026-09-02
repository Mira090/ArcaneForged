using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_FinalComboDecreaseCooldown : ArcaneWeapon_Basic
    {
        public float lastAttackDecreaseCooldownRatio = 0.1f;

        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnSwingCreated += OnSwingCreated;
        }

        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnSwingCreated -= OnSwingCreated;
        }

        private void OnSwingCreated(bool isFinalCombo, ProjectileBase projectile)
        {
            if (!isFinalCombo || NetworkAvatar == null || NetworkAvatar.Inventory == null)
                return;

            foreach (Charm_Basic value in NetworkAvatar.Inventory.charms.Values)
            {
                if (value is Charm_Magic { CooldownRatio: var cooldownRatio } charm_Magic)
                {
                    charm_Magic.AddCooldownBonus(cooldownRatio * lastAttackDecreaseCooldownRatio);
                }
                else if (value is IActiveCasting activeCasting)
                {
                    float cooldownRatio2 = activeCasting.GetCooldownRatio();
                    activeCasting.CooldownDecrease(cooldownRatio2 * lastAttackDecreaseCooldownRatio);
                }
            }
        }
    }
}
