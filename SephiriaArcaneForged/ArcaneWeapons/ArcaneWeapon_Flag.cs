using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_Flag : ArcaneWeapon_Basic
    {
        public string stat;
        public int value;

        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if (string.IsNullOrEmpty(stat))
                return;
            NetworkAvatar.AddCustomStatUnsafe(stat, value);
            //Core.Logger($"{stat}: +{value}");
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if (string.IsNullOrEmpty(stat))
                return;
            NetworkAvatar.AddCustomStatUnsafe(stat, -value);
            //Core.Logger($"{stat}: {-value}");
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            return new Loc.KeywordValue[]
            {
                new Loc.KeywordValue("VALUE", value.ToString())
            };
        }
    }
}
