using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_ShieldThrowing : ArcaneWeapon_StatusInstance
    {
        public int per = 20;
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
        public override NewWeaponFireData FireData => SephiriaPrefabs.ShieldThrowingFireData;
        public override string DamageId => "ArcaneWeapon_SwordShieldShieldThrowing";
        public override float? DamageMultiplier => null;
        public override float AttackDashScale => 0f;
        private void OnSpecialAttackSwing(int idx)
        {
            if (NetworkAvatar == null || NetworkAvatar.Inventory == null)
                return;
            Attack();
        }
        /*
        public override int GetTriggerCount()
        {
            var damage = NetworkAvatar.GetCustomStat(ECustomStat.WeaponDamageBonus);
            return base.GetTriggerCount() + damage / per;
        }*/
        public override Loc.KeywordValue[] BuildKeywords()
        {
            var list = base.BuildKeywords().ToList();
            list.Add(new Loc.KeywordValue("PER", per.ToString()));
            return list.ToArray();
        }
        protected override float ModifyDamage(float damage)
        {
            damage += damage * (float)NetworkAvatar.GetCustomStat(ECustomStat.SpecialAttackDamageBonus) / 100f;
            return base.ModifyDamage(damage);
        }
    }
}
