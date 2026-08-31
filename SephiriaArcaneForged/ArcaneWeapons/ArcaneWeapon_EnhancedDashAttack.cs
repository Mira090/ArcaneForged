using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_EnhancedDashAttack : ArcaneWeapon_StatusInstance
    {
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnDashServerside += OnDashServerside;
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnDashServerside -= OnDashServerside;
        }
        public override NewWeaponFireData FireData => SephiriaPrefabs.DashAttackFireData_BladeZone;
        public override string DamageId => "ArcaneWeapon_EnhancedDashAttack";
        public override float? DamageMultiplier => null;
        public override float AttackDashScale => 0f;
        private void OnDashServerside(Vector2 motionTo, bool consumed)
        {
            if (consumed)
                Attack();
        }
    }
}
