using Mirror;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_CrystalExplosion : ArcaneWeapon_StatusInstance
    {
        [Header("Normal Attack : Crystal Explosion")]
        public bool isCrystalExplosion = true;

        public bool canCrystalExplosion;

        public NewWeaponFireData crystalExplosionFireData;


        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if (NetworkAvatar)
            {
                NetworkAvatar.DestroyEffectHUD(GetWeaponHUDID() + "_STAFF_CRYSTAL_EXPLOSION");
                NetworkAvatar.OnGuardSucceeded += OnGuardSucceeded;
            }
            if (WeaponController)
            {
                WeaponController.OnBaisAttackSwing += OnBaisAttackSwing;
            }
        }

        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if (NetworkAvatar)
            {
                NetworkAvatar.DestroyEffectHUD(GetWeaponHUDID() + "_STAFF_CRYSTAL_EXPLOSION");
                NetworkAvatar.OnGuardSucceeded -= OnGuardSucceeded;
            }
            if (WeaponController)
            {
                WeaponController.OnBaisAttackSwing -= OnBaisAttackSwing;
            }
        }

        private void OnGuardSucceeded(DamageInstance damage, bool perfect)
        {
            if (isCrystalExplosion)
            {
                canCrystalExplosion = true;
                NetworkAvatar.CreateEffectHUD("STAFF_CRYSTAL_EXPLOSION", GetWeaponHUDID() + "_STAFF_CRYSTAL_EXPLOSION");
            }
        }
        public override NewWeaponFireData FireData => SephiriaPrefabs.CrystalExplosionFireData;
        public override string DamageId => "ArcaneWeapon_StaffSpecial";
        public override float? DamageMultiplier => null;
        public override float AttackDashScale => 0f;

        private void OnBaisAttackSwing(int idx)
        {
            if (!canCrystalExplosion)
                return;
            canCrystalExplosion = false;
            NetworkAvatar.DestroyEffectHUD(GetWeaponHUDID() + "_STAFF_CRYSTAL_EXPLOSION");
            Attack();
        }
        public override Vector3 FirePosition(WeaponControllerSimple simple)
        {
            return simple.shoulder.aimedPosition - new Vector3(0f, simple.shoulder.Position.y, 0f);
        }
        protected override float ModifyDamage(float damage)
        {
            var d = base.ModifyDamage(damage);
            d += d * (NetworkAvatar.GetCustomStat(ECustomStat.BasicAttackDamageBonus) / 100f);
            d += d * (NetworkAvatar.GetCustomStat(ECustomStat.SpecialAttackDamageBonus) / 100f);
            return d;
        }
    }
}
