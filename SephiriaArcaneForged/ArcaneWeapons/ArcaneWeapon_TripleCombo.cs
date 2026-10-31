using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_TripleCombo : ArcaneWeapon_Basic
    {
        public override NewWeaponFireData FireData
        {
            get
            {
                if (WeaponController == null)
                    return null;
                if (WeaponController.currentWeapon == null)
                    return null;
                var idx = WeaponController.currentWeapon.finalComboIdx;
                return WeaponController.currentWeapon.GetBasicAttack(idx, WeaponController.currentWeapon.GetBasicAttackParameter(idx));
            }
        }
        public override string DamageId => "ArcaneWeapon_GreatSwordBasic";
        public override float? DamageMultiplier => null;
        public override float AttackDashScale => 0.5f;
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
        private void OnSwingCreated(bool last, ProjectileBase projectile)
        {
            if (!last)
                return;
            this.Delay(GetTriggerDelay(), () =>
            {
                if (!IsEffectEnabled || NetworkAvatar == null || WeaponController == null || NetworkAvatar.IsDead)
                    return;
                Attack();
            });
        }
        protected override float ModifyDamage(float damage)
        {
            damage += damage * (float)NetworkAvatar.GetCustomStat(ECustomStat.BasicAttackDamageBonus) / 100f;
            return base.ModifyDamage(damage);
        }
        protected override float GetTriggerDelay()
        {
            return 0.1f;
        }
        protected override bool GetForceFinalCombo()
        {
            return true;
        }
        public override int GetTriggerCount()
        {
            return 2;
        }
    }
}
