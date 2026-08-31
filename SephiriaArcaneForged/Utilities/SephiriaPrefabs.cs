using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.Utilities
{
    public static class SephiriaPrefabs
    {
        public static ScriptableFx RingFxPrefab
        {
            get
            {
                if (_ringFxPrefab == null)
                    _ringFxPrefab = WeaponDatabase.FindWeaponById(518).mainWeaponPrefab.GetComponent<WeaponAddonCommon_BurnRing>().ringFxPrefab;
                return _ringFxPrefab;
            }
        }
        private static ScriptableFx _ringFxPrefab;
    }
}
