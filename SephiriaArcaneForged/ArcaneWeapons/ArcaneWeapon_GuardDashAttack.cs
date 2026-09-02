using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_GuardDashAttack : ArcaneWeapon_StatusInstance
    {
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnBeginDashAttackAnimation += OnBeginDashAttackAnimation;
            WeaponController.OnEndAttackAnimation += OnEndAttackAnimation;
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnBeginDashAttackAnimation -= OnBeginDashAttackAnimation;
            WeaponController.OnEndAttackAnimation -= OnEndAttackAnimation;
        }
        private void OnBeginDashAttackAnimation()
        {
            WeaponController.StartGuardCustomAngle(200f);
        }
        private void OnEndAttackAnimation()
        {
            WeaponController.StopGuard();
        }
    }
}
