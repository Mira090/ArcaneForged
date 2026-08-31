using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_ApplyDebuff : ArcaneWeapon_StatusInstance
    {
        public string stat;
        public int value;
        public int percent;
        public string debuff;
        public CharacterDebuff debuffPrefab;
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            debuffPrefab = UnitDatabase.GetDebuff(debuff);
            NetworkAvatar.OnAttackUnit += OnAttackUnit;
            if (string.IsNullOrEmpty(stat))
                return;
            NetworkAvatar.AddCustomStatUnsafe(stat, value);
        }

        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if (debuffPrefab == null || avatar == null || avatar.IsDead || damage.fromType != EDamageFromType.DirectAttack)
                return;
            if (percent.Percent())
            {
                avatar.ApplyDebuff(debuffPrefab, NetworkAvatar);
            }
        }

        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnit -= OnAttackUnit;
            if (string.IsNullOrEmpty(stat))
                return;
            NetworkAvatar.AddCustomStatUnsafe(stat, -value);
        }

        public override Loc.KeywordValue[] BuildKeywords()
        {
            var list = base.BuildKeywords().ToList();
            list.Add(new Loc.KeywordValue("PERCENT", percent + "%"));
            list.Add(new Loc.KeywordValue("VALUE", value.ToString()));
            return list.ToArray();
        }
    }
}
