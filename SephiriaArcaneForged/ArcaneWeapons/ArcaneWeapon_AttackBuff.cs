using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_AttackBuff : ArcaneWeapon_StatusInstance
    {
        public CharacterBuff buffPrefab;

        public float buffPercent;

        public string buffName;

        public bool hudFlashWhenBuffed = true;

        public override Loc.KeywordValue[] BuildKeywords()
        {
            var list = base.BuildKeywords().ToList();
            list.Add(new Loc.KeywordValue("PERCENT", buffPercent.ToString()));
            list.Add(new Loc.KeywordValue("BUFF", "<tag=" + buffName + ">"));
            return list.ToArray();
        }

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
        private void OnAttackUnit(UnitAvatar avatar, DamageInstance damage)
        {
            if ((bool)NetworkAvatar && damage.fromType == EDamageFromType.DirectAttack && buffPercent.Percent())
            {
                NetworkAvatar.ApplyBuff(buffPrefab, 1f, null, hudFlashWhenBuffed);
            }
        }
    }
}
