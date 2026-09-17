using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_PerfectGuardBuff : ArcaneWeapon_StatusInstance
    {
        public CharacterBuff buffPrefab;

        public string buffName;

        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnGuardSucceeded += HandleGuard;
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnGuardSucceeded -= HandleGuard;
        }
        private void HandleGuard(DamageInstance damage, bool isPerfectGuard)
        {
            if (isPerfectGuard)
            {
                NetworkAvatar.ApplyBuff(buffPrefab);
            }
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            var list = base.BuildKeywords().ToList();
            list.Add(new Loc.KeywordValue("BUFF", "<tag=" + buffName + ">"));
            return list.ToArray();
        }
    }
}
