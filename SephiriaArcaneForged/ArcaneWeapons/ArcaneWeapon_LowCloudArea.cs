using Mirror;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_LowCloudArea : ArcaneWeapon_StatusInstance
    {
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnGuardSucceeded += HandleGuard;
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            RemoveDarkCloudStatus();
            NetworkAvatar.OnGuardSucceeded -= HandleGuard;
        }

        public Timer durationTimer = new Timer(5f);

        [SyncVar(hook = "HandleLowCloudActiveChanged")]
        public bool isLowCloudActive;

        private Timer lowCloudTickTimer = new Timer(0.25f);

        public float lowCloudRadius = 2f;

        public int addDarkCloudSpeed = 500;

        private int addedDarkCloudSpeed;

        public int addDarkCloudDamage = 5;

        private int addedDarkCloudDamage;

        [Header("FX")]
        public ScriptableFx cloudFxPrefab => SephiriaPrefabs.LowCloudFxPrefab;

        private ScriptableFx _cloudFx;

        public Action<bool, bool> _Mirror_SyncVarHookDelegate_isLowCloudActive;

        public bool NetworkisLowCloudActive
        {
            get
            {
                return isLowCloudActive;
            }
            [param: In]
            set
            {
                GeneratedSyncVarSetter(value, ref isLowCloudActive, 4uL, _Mirror_SyncVarHookDelegate_isLowCloudActive);
            }
        }

        private void Update()
        {
            if (!base.isServer || !isLowCloudActive)
            {
                return;
            }

            if (lowCloudTickTimer.Update(Time.deltaTime))
            {
                if (CheckNearbyEnemy())
                {
                    AddDarkCloudStatus();
                }
                else
                {
                    RemoveDarkCloudStatus();
                }
            }

            if (durationTimer.Update(Time.deltaTime))
            {
                RemoveDarkCloudStatus();
                NetworkisLowCloudActive = false;
            }
        }

        private void OnDisable()
        {
            if ((bool)_cloudFx)
            {
                _cloudFx.Stop();
                _cloudFx = null;
            }
        }

        private void HandleGuard(DamageInstance instance, bool isPerfectGuard)
        {
            durationTimer.SetTimer(0f);
            NetworkisLowCloudActive = true;
            lowCloudRadius = KeywordDatabase.GetConstValue("lowCloudAreaRadius");
        }

        public override Loc.KeywordValue[] BuildKeywords()
        {
            return new List<Loc.KeywordValue>
        {
            new Loc.KeywordValue("DURATION", durationTimer.time.ToString())
        }.ToArray();
        }

        private bool CheckNearbyEnemy()
        {
            if (NetworkAvatar == null)
                return false;

            foreach (UnitAvatar allCreature in CombatManager.Instance.AllCreatures)
            {
                if ((bool)allCreature && !allCreature.IsDead && !allCreature.canBeTarget.IsFalse() && allCreature.gameObject.activeSelf && CombatManager.ContainsAttackableFaction(allCreature.GetHostileFactionLayers(EDamageFromType.None), NetworkAvatar.faction) && Vector3.Distance(allCreature.transform.position, NetworkAvatar.transform.position) <= lowCloudRadius)
                {
                    return true;
                }
            }

            return false;
        }

        private void AddDarkCloudStatus()
        {
            if (addedDarkCloudSpeed == 0 || addedDarkCloudDamage == 0)
            {
                addedDarkCloudSpeed = addDarkCloudSpeed;
                addedDarkCloudDamage = addDarkCloudDamage;
                NetworkAvatar.AddCustomStat("DARKCLOUDSPEED", addedDarkCloudSpeed);
                NetworkAvatar.AddCustomStat("DARKCLOUDDAMAGE", addedDarkCloudDamage);
            }
        }

        private void RemoveDarkCloudStatus()
        {
            if (addedDarkCloudSpeed != 0 || addedDarkCloudDamage != 0)
            {
                NetworkAvatar.AddCustomStat("DARKCLOUDSPEED", -addedDarkCloudSpeed);
                NetworkAvatar.AddCustomStat("DARKCLOUDDAMAGE", -addedDarkCloudDamage);
                addedDarkCloudSpeed = 0;
                addedDarkCloudDamage = 0;
            }
        }

        private void HandleLowCloudActiveChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                if ((bool)cloudFxPrefab && !_cloudFx && WeaponController != null)
                {
                    Transform transform = WeaponController.transform;
                    _cloudFx = UnityEngine.Object.Instantiate(cloudFxPrefab, transform ? transform.position : Vector3.zero, Quaternion.identity);
                    _cloudFx.SetFollowTarget(transform);
                    _cloudFx.Play();
                }
            }
            else if ((bool)_cloudFx)
            {
                _cloudFx.Stop();
                _cloudFx = null;
            }
        }

        public ArcaneWeapon_LowCloudArea()
        {
            _Mirror_SyncVarHookDelegate_isLowCloudActive = HandleLowCloudActiveChanged;
        }

        public override bool Weaved()
        {
            return true;
        }

        protected override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
        {
            base.SerializeSyncVars(writer, forceAll);
            if (forceAll)
            {
                writer.WriteBool(isLowCloudActive);
                return;
            }

            writer.WriteVarULong(syncVarDirtyBits);
            if ((syncVarDirtyBits & 4L) != 0L)
            {
                writer.WriteBool(isLowCloudActive);
            }
        }

        protected override void DeserializeSyncVars(NetworkReader reader, bool initialState)
        {
            base.DeserializeSyncVars(reader, initialState);
            if (initialState)
            {
                GeneratedSyncVarDeserialize(ref isLowCloudActive, _Mirror_SyncVarHookDelegate_isLowCloudActive, reader.ReadBool());
                return;
            }

            long num = (long)reader.ReadVarULong();
            if ((num & 4L) != 0L)
            {
                GeneratedSyncVarDeserialize(ref isLowCloudActive, _Mirror_SyncVarHookDelegate_isLowCloudActive, reader.ReadBool());
            }
        }
    }
}
