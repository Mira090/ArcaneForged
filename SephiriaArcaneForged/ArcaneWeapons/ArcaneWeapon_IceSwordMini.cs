using Miniscript;
using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_IceSwordMini : ArcaneWeapon_Basic
    {
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("AIRSLASHMINI", 1);
            NetworkAvatar.AddCustomStatUnsafe("AIRSLASHHASTE", 40);
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.AddCustomStatUnsafe("AIRSLASHMINI", -1);
            NetworkAvatar.AddCustomStatUnsafe("AIRSLASHHASTE", -40);
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            return new Loc.KeywordValue[]
            {
                new Loc.KeywordValue("VAL0", "40")
            };
        }
    }
}
