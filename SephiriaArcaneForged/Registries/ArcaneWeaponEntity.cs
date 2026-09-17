using SephiriaArcaneForged.ArcaneWeapons;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.Registries
{
    public class ArcaneWeaponEntity : ScriptableObject
    {
        public int id;
        public WeaponEntity weapon;
        public LocalizedString affix;
        public Func<WeaponEntity, bool> condition;
        public LocalizedString conditionText;

        public GameObject resourcePrefab;

        public string GetEffectText(bool condition)
        {
            var text = string.Empty;
            if (condition && conditionText != null && !string.IsNullOrEmpty(conditionText.key))
                text = "\r\n" + KeywordDatabase.Convert(KeywordDatabase.Convert(conditionText.ToString()));

            if (resourcePrefab.TryGetComponent<ArcaneWeapon_Basic>(out var basic) && basic.effectsString != null)
            {
                return KeywordDatabase.Convert(Loc.Convert(KeywordDatabase.Convert(basic.effectsString.ToString()), basic.BuildKeywords())) + text;
            }
            return "..." + text;
        }
        public bool IsValid(WeaponEntity current)
        {
            return condition?.Invoke(current) ?? true;
        }
    }
}
