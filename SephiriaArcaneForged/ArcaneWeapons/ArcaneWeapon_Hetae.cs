using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_Hetae : ArcaneWeapon_StatusInstance
    {
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
        }

        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
        }

        public override NewWeaponFireData FireData => SephiriaPrefabs.HetaeFireData;
        public override string DamageId => "ArcaneWeapon_SwordShieldEmber";
        public override float? DamageMultiplier => 1f;
        public override float AttackDashScale => 0f;
        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (damage.fromType != EDamageFromType.DirectAttack || damage.id == DamageId)
                return;
            int count = 0;
            foreach(var debuff in avatar.Debuffs)
            {
                if(debuff.ID == SephiriaPrefabs.Burn.ID)
                {
                    count = debuff.CurrentStack;
                    debuff.RequestEnd();
                    int per = count >= 4 ? 100 : 50;

                    Attack(avatar, count * per);
                    return;
                }
            }
        }
    }
}
