using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_Flags : ArcaneWeapon_Basic
    {
        public string[] stat;
        public int[] value;

        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            for(int q = 0; q < stat.Length; q++)
            {
                if (string.IsNullOrEmpty(stat.SafeRandomAccess(q)))
                    return;
                NetworkAvatar.AddCustomStatUnsafe(stat.SafeRandomAccess(q), value.SafeRandomAccess(q));
            }
            //Core.Logger($"{stat}: +{value}");
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            for (int q = 0; q < stat.Length; q++)
            {
                if (string.IsNullOrEmpty(stat.SafeRandomAccess(q)))
                    return;
                NetworkAvatar.AddCustomStatUnsafe(stat.SafeRandomAccess(q), -value.SafeRandomAccess(q));
            }
            //Core.Logger($"{stat}: {-value}");
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            var list = new List<Loc.KeywordValue>();
            for(int q = 0; q < stat.Length; q++)
            {
                list.Add(new Loc.KeywordValue("VAL" + q, value.SafeRandomAccess(q).ToString()));
            }
            return list.ToArray();
        }
    }
}
