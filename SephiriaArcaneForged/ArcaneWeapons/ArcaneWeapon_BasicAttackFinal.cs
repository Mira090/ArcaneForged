using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_BasicAttackFinal : ArcaneWeapon_StatusInstance
    {
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnBeginDashAttackAnimation += HandleBeginDashAnimation;
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnBeginDashAttackAnimation -= HandleBeginDashAnimation;
        }
        private void HandleBeginDashAnimation()
        {
            if (base.isServer && NetworkAvatar != null)
            {
                NetworkAvatar.ApplyBuff(SephiriaPrefabs.BasicAttackFinalBuffPrefab);
            }
        }
    }
}
