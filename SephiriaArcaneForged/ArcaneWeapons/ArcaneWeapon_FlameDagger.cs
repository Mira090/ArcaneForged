using SephiriaArcaneForged.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_FlameDagger : ArcaneWeapon_StatusInstance
    {
        public Timer cooldownTimer = new Timer(0.5f);
        public bool isInCooldown;
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnSpecialAttackSwing += OnSpecialAttackSwing;
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnSpecialAttackSwing -= OnSpecialAttackSwing;
        }
        public override NewWeaponFireData FireData => SephiriaPrefabs.FlameDaggerFireData;
        public override string DamageId => "ArcaneWeapon_DaggerEmber";
        public override float? DamageMultiplier => null;
        public override float AttackDashScale => 1f;
        private void OnSpecialAttackSwing(int idx)
        {
            if (NetworkAvatar == null || NetworkAvatar.Inventory == null)
                return;
            if (isInCooldown)
                return;
            isInCooldown = true;
            Attack();
        }
        protected override void OnAttack(CombatBehaviour combat, DamageInstance damage, ProjectileBase projectile)
        {
            if(combat is UnitAvatar avatar)
            {
                avatar.ApplyDebuff(SephiriaPrefabs.Burn, NetworkAvatar);
            }
        }
        private void Update()
        {
            if (!IsEffectEnabled || !base.isClient || !NetworkAvatar)
                    return;
            if (isInCooldown && !NetworkAvatar.IsDead && cooldownTimer.Update(Time.deltaTime))
            {
                isInCooldown = false;
            }
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            var list = base.BuildKeywords().ToList();
            list.Add(new Loc.KeywordValue("COOLDOWN", cooldownTimer.time.ToString()));
            return list.ToArray();
        }
    }
}
