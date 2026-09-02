using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_LightningSpear : ArcaneWeapon_StatusInstance
    {
        public int consume = 5;
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
        public override NewWeaponFireData FireData => SephiriaPrefabs.LightningSpearFireData;
        public override string DamageId => "ArcaneWeapon_LightningSpear";
        public override float? DamageMultiplier => null;
        public override float AttackDashScale => 0f;
        private void OnSpecialAttackSwing(int idx)
        {
            if(NetworkAvatar == null || NetworkAvatar.Inventory == null)
                return;
            var combo = NetworkAvatar.Inventory.FindComboEffect(ItemCategories.DarkCloud);
            if (combo is ComboEffect_DarkCloud darkCloud && darkCloud.isEnabled && darkCloud.NetworkdarkCloud >= consume)
            {
                darkCloud.NetworkdarkCloud -= consume;
                Attack();
            }
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            var list = base.BuildKeywords().ToList();
            list.Add(new Loc.KeywordValue("CONSUME", consume.ToString()));
            return list.ToArray();
        }
    }
}
