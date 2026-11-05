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
                if (NetworkAvatar == null)
                    return null;
                var weapon = NetworkAvatar.gameObject.GetComponent<WeaponControllerSimple>();
                if (weapon == null || weapon.currentWeapon == null)
                    return null;
                var idx = weapon.currentWeapon.finalComboIdx;
                return weapon.currentWeapon.GetBasicAttack(idx, weapon.currentWeapon.GetBasicAttackParameter(idx));
            }
        }
        public override string DamageId => "ArcaneWeapon_GreatSwordBasic";
        public override float? DamageMultiplier => null;
        public override float AttackDashScale => 0.5f;
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnBaisAttackSwing += OnBaisAttackSwing;
            WeaponController.OnSwingCreated += OnSwingCreated;
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnBaisAttackSwing -= OnBaisAttackSwing;
            WeaponController.OnSwingCreated -= OnSwingCreated;
        }
        private void OnSwingCreated(bool last, ProjectileBase projectile)
        {
            if (!projectile.forceFinalCombo)
                return;
            this.Delay(GetTriggerDelay(), () =>
            {
                if (!IsEffectEnabled || NetworkAvatar == null || WeaponController == null || NetworkAvatar.IsDead)
                    return;
                Attack();
            });
        }
        private void OnBaisAttackSwing(int idx)
        {
            if (idx != WeaponController.currentWeapon.finalComboIdx)
                return;
            this.Delay(GetTriggerDelay(), () =>
            {
                if (!IsEffectEnabled || NetworkAvatar == null || WeaponController == null || NetworkAvatar.IsDead)
                    return;
                Attack();
            });
        }
        protected override void OnCreateAttack(int idx, ProjectileBase projectile)
        {
            projectile.forceFinalCombo = false;
            WeaponController.OnSwingCreated?.Invoke(true, projectile);
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
        public override int GetTriggerCount()
        {
            return 2;
        }
    }
}
