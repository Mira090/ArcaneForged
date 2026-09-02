using Miniscript;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_ApplyFreeze_OnlySpecialAttack : ArcaneWeapon_StatusInstance
    {
        public int percent = 100;
        public string debuff = "FROSTBITE";
        public int count = 1;
        public CharacterDebuff debuffPrefab;
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            debuffPrefab = UnitDatabase.GetDebuff(debuff);
            WeaponController.OnSpecialAttack += OnSpecialAttack;
        }

        private void OnSpecialAttack(CombatBehaviour combat, DamageInstance damage, ProjectileBase projectile)
        {
            if (debuffPrefab == null || combat == null || damage.ignoreDebuff)
                return;
            if (combat is UnitAvatar avatar && !avatar.IsDead && percent.Percent())
            {
                for(int q = 0; q < count; q++)
                {
                    avatar.ApplyDebuff(debuffPrefab, NetworkAvatar);
                }
            }
        }

        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            WeaponController.OnSpecialAttack -= OnSpecialAttack;
        }

        public override Loc.KeywordValue[] BuildKeywords()
        {
            var list = base.BuildKeywords().ToList();
            list.Add(new Loc.KeywordValue("COUNT", count.ToString()));
            list.Add(new Loc.KeywordValue("DEBUFF", "<tag=Frostbite>"));
            return list.ToArray();
        }
    }
}
