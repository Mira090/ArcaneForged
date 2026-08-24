using SephiriaArcaneForged.ArcaneWeapons;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.Registries
{
    public class ArcaneWeaponEntity : ScriptableObject
    {
        [Serializable]
        public class StatusGroup
        {
            public string statusID = "AP";

            public int value = 0;
        }

        public int id;
        public WeaponEntity weapon;
        public LocalizedString affix;

        public GameObject resourcePrefab;

        public string GetEffectText()
        {
            if(resourcePrefab.TryGetComponent<ArcaneWeapon_Basic>(out var basic) && basic.effectsString != null)
            {
                return KeywordDatabase.Convert(Loc.Convert(KeywordDatabase.Convert(basic.effectsString.ToString()), basic.BuildKeywords()));
            }
            return "...";
        }
    }
}
