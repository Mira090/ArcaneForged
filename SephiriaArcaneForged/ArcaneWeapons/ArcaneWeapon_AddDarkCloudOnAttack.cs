using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_AddDarkCloudOnAttack : ArcaneWeapon_StatusInstance
    {
        public int swingCountToActivate = 6;

        private int currentSwingCount;

        public int addDarkCloudCount = 4;

        private Timer cooldownTimer = new Timer(0.07f);

        private bool isInCooldown;

        public override Loc.KeywordValue[] BuildKeywords()
        {
            return new Loc.KeywordValue[2]
            {
            new Loc.KeywordValue("SWING", swingCountToActivate.ToString()),
            new Loc.KeywordValue("COUNT", addDarkCloudCount.ToString())
            };
        }

        private void Update()
        {
            if (isInCooldown && cooldownTimer.Update(Time.deltaTime))
            {
                isInCooldown = false;
            }
        }

        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            WeaponController.OnBasicAttack += OnAttackUnit;
            WeaponController.OnDashAttack += OnAttackUnit;
            WeaponController.OnSpecialAttack += OnAttackUnit;
        }

        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnBasicAttack -= OnAttackUnit;
            WeaponController.OnDashAttack -= OnAttackUnit;
            WeaponController.OnSpecialAttack -= OnAttackUnit;
        }

        private void OnAttackUnit(CombatBehaviour behaviour, DamageInstance instance, ProjectileBase @base)
        {
            if (isInCooldown || instance.fromType != EDamageFromType.DirectAttack)
                return;

            isInCooldown = true;
            currentSwingCount++;
            if (currentSwingCount < swingCountToActivate)
                return;

            currentSwingCount = 0;
            if (NetworkAvatar.Inventory == null)
                return;
            ComboEffectBase comboEffectBase = NetworkAvatar.Inventory.FindComboEffect(ItemCategories.DarkCloud);
            if (comboEffectBase != null && comboEffectBase.isEnabled && comboEffectBase is ComboEffect_DarkCloud comboEffect_DarkCloud)
            {
                comboEffect_DarkCloud.AddCloud(addDarkCloudCount);
            }
        }
    }
}
