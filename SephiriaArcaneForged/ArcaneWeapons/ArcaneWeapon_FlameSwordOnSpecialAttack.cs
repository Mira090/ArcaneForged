using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_FlameSwordOnSpecialAttack : ArcaneWeapon_StatusInstance
    {
        public Timer cooldownTimer = new Timer(0.5f);
        public bool isInCooldown;
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnSpecialAttack += OnSpecialAttack;
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnSpecialAttack -= OnSpecialAttack;
        }
        private void OnSpecialAttack(CombatBehaviour combat, DamageInstance damage, ProjectileBase projectile)
        {
            if (!NetworkAvatar || !NetworkAvatar.Inventory || !WeaponController)
                return;
            ComboEffectBase combo = NetworkAvatar.Inventory.FindComboEffect("FLAMESWORD");
            if (combo != null && combo.isEnabled && combo is ComboEffect_FlameSword flameSword)
            {
                if (isInCooldown)
                    return;
                isInCooldown = true;
                flameSword.ServerFireSword(WeaponController.transform.position, false, false, flameSword.currentSword, true);
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
