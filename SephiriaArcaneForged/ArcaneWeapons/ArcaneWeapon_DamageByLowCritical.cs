using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_DamageByLowCritical : ArcaneWeapon_StatusInstance
    {
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAttackUnitBeforeOperation += OnAttackUnitBeforeOperation;
        }

        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAttackUnitBeforeOperation -= OnAttackUnitBeforeOperation;
        }

        private void OnAttackUnitBeforeOperation(UnitAvatar avatar, DamageInstance damage)
        {
            if(NetworkAvatar.GetCustomStat(ECustomStat.Critical) <= 3000)
            {
                damage.damage *= 1.2f;
            }
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            StatusEntity statusEntity = StatusDatabase.GetStatusEntity("CRITICAL");
            if (statusEntity != null)
            {
                float num = (float)3000 / (float)statusEntity.divideForDisplay;
                return new Loc.KeywordValue[2]
                {
                new Loc.KeywordValue("TARGET", num.ToString("0.#") + statusEntity.symbol),
                new Loc.KeywordValue("ADDDAMAGE", 20 + "%")
                };
            }

            Debug.LogError("Source stat not found: " + "CRITICAL");
            return new Loc.KeywordValue[0];
        }
    }
}
