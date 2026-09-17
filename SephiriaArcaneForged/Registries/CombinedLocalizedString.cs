using System;
using System.Collections.Generic;
using System.Text;
using static SephiriaArcaneForged.Networks.AnvilPatches;

namespace SephiriaArcaneForged.Registries
{
    public class CombinedLocalizedString : LocalizedString
    {
        public string affix;

        public CombinedLocalizedString()
        {
            affix = "";
            key = "";
        }

        public CombinedLocalizedString(string key)
        {
            affix = "";
            this.key = key;
        }
        public CombinedLocalizedString(string affix, string key)
        {
            this.affix = affix;
            this.key = key;
        }

        public override string ToString()
        {
            return Loc._(affix) + UI_WeaponTooltipPatch.LocalizedAffixSpace + Loc._(key);
        }
    }
}
