using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_MagicBlade : ArcaneWeapon_Basic
    {
        [Header("Magic Blade")]
        public bool useMagicBlade = true;

        public GameObject magicBladePrefab => SephiriaPrefabs.MagicBladePrefab;

        public string magicBladeDamageId = "KatanaMagicBlade";

        public float magicBladeDamge = 20f;

        public float losedMpMultiplier = 0.5f;

        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if(WeaponController.TryGetComponent<SkillController>(out var skill))
            {
                skill.OnCreateMagicServerside += OnCreateMagicServerside;
            }
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            if (WeaponController.TryGetComponent<SkillController>(out var skill))
            {
                skill.OnCreateMagicServerside -= OnCreateMagicServerside;
            }
        }
        private void OnCreateMagicServerside(ActiveSkill skill)
        {
            if (useMagicBlade)
            {
                float num = ((NetworkAvatar.GetCustomStatUnsafe("INFINITYMP") > 0) ? 0f : ((float)(NetworkAvatar.MaxMp - NetworkAvatar.MP)));
                float num2 = magicBladeDamge + num * losedMpMultiplier;
                num2 += num2 * ((float)NetworkAvatar.GetCustomStat(ECustomStat.MagicDamageBonus) * 0.01f);
                Vector3 normalized = WeaponController.attackDirection.normalized;
                Vector3 vector = WeaponController.transform.position + -normalized * 1.5f + HorayUtility.GetVector3FromAngle(normalized.GetAngle() + 90f * Mathf.Sign(WeaponController.attackDirection.x)) * 0.3f + (Vector3)UnityEngine.Random.insideUnitCircle * 0.5f;
                Vector3 vector2 = (Vector3)NetworkAvatar.CurrentLookingPosition - vector;
                Bullet bullet = Bullet.Pool.Spawn(magicBladePrefab, vector, canBeTransparentOnMultiplayer: true, EDamageFromType.Magic, magicBladeDamageId, num2, 1, 1f, NetworkAvatar, NetworkAvatar.GetHostileFactionLayers(EDamageFromType.None), NetworkAvatar.TopdownActor.CenterYPos, vector, vector + vector2.normalized * 10f, null, null);
                bullet.SetDamage(num2);
                UnitAvatar unitAvatar2 = PlayerInputController.SearchTargetNearestPoint(NetworkAvatar, NetworkAvatar.CurrentLookingPosition, 5f);
                if (unitAvatar2 != null)
                {
                    bullet.SetHomingTarget(unitAvatar2);
                }
            }
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            return new Loc.KeywordValue[]
            {
                new Loc.KeywordValue("DAMAGE", magicBladeDamge.ToString()),
                new Loc.KeywordValue("PERCENT", (losedMpMultiplier * 100).ToString() + "%")
            };
        }
    }
}
