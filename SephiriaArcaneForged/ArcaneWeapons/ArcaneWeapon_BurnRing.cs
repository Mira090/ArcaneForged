using Mirror;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_BurnRing : ArcaneWeapon_StatusInstance
    {
        public string debuffKeywordName = "Burn";

        public string debuffID = "BURN";

        public string damageId = "Weapon_BurnRing";

        public LocalizedString burnRingName = new LocalizedString("WeaponAddon_BurnRing_Name");

        [Header("Burn Ring")]
        public float burnRingRadius = 5f;

        public float burnRingDuration = 10f;

        [SyncVar(hook = "HookRingEnabled")]
        public bool isRingEnabled;

        public Timer burnRingTickTimer = new Timer(0.5f);

        public string relatedTickStatName = "BURNSPEED";

        [SyncVar(hook = "HookAddedBurnSpeed")]
        public int addedBurnSpeed;

        [Header("FX")]
        public ScriptableFx ringFxPrefab => SephiriaPrefabs.RingFxPrefab;

        [Header("Damage")]
        public EDamageElementalType elementalType = EDamageElementalType.Fire;

        public string relatedStatKeywordName = "FireDamage";

        public string relatedStatUnsafe = "FIREDAMAGE";

        public int damagePercent = 50;

        private float remainingDuration;

        private ScriptableFx _ringFx;

        public Action<bool, bool> _Mirror_SyncVarHookDelegate_isRingEnabled;

        public Action<int, int> _Mirror_SyncVarHookDelegate_addedBurnSpeed;

        public bool NetworkisRingEnabled
        {
            get
            {
                return isRingEnabled;
            }
            [param: In]
            set
            {
                GeneratedSyncVarSetter(value, ref isRingEnabled, 1uL, _Mirror_SyncVarHookDelegate_isRingEnabled);
            }
        }

        public int NetworkaddedBurnSpeed
        {
            get
            {
                return addedBurnSpeed;
            }
            [param: In]
            set
            {
                GeneratedSyncVarSetter(value, ref addedBurnSpeed, 2uL, _Mirror_SyncVarHookDelegate_addedBurnSpeed);
            }
        }

        private void Update()
        {
            if (!base.isServer || !isRingEnabled)
            {
                return;
            }

            remainingDuration -= Time.deltaTime;
            if (remainingDuration <= 0f)
            {
                NetworkisRingEnabled = false;
                return;
            }

            if (NetworkAvatar != null)
            {
                NetworkaddedBurnSpeed = NetworkAvatar.GetCustomStatUnsafe("BURNSPEED");
            }

            float num = 1f;
            num += (float)addedBurnSpeed / 100f;
            if (burnRingTickTimer.Update(Time.deltaTime * num))
            {
                DamageNearbyEnemies();
            }
        }

        private void DamageNearbyEnemies()
        {
            if (!NetworkAvatar)
            {
                return;
            }

            float num = (float)(NetworkAvatar.GetCustomStatUnsafe(relatedStatUnsafe) * damagePercent) / 100f;
            num += num * (float)NetworkAvatar.GetCustomStat(ECustomStat.WeaponDamageBonus) / 100f;
            num += num * (float)NetworkAvatar.GetCustomStat(ECustomStat.FinalWeaponDamage) / 100f;
            num += num * (float)NetworkAvatar.GetCustomStatUnsafe("BURNDAMAGE") / 100f;
            List<UnitAvatar> allCreatures = CombatManager.Instance.AllCreatures;
            for (int num2 = allCreatures.Count - 1; num2 >= 0; num2--)
            {
                UnitAvatar unitAvatar2 = allCreatures[num2];
                if ((bool)unitAvatar2 && !unitAvatar2.IsDead && !unitAvatar2.canBeTarget.IsFalse() && unitAvatar2.gameObject.activeSelf && CombatManager.ContainsAttackableFaction(unitAvatar2.GetHostileFactionLayers(EDamageFromType.None), NetworkAvatar.faction) && !(Vector3.Distance(unitAvatar2.transform.position, NetworkAvatar.transform.position) > burnRingRadius))
                {
                    DamageInstance damage = DamageInstance.GetDamage(NetworkAvatar, damageId, unitAvatar2.transform.position, 4294967295L, num, EDamageType.Slice, EDamageFromType.None, Vector2.zero, 0, 0f);
                    damage.elementalType = elementalType;
                    unitAvatar2.ApplyDamage(damage);
                }
            }
        }

        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            NetworkAvatar.OnAddedDebuffOnTarget += OnAddedDebuffOnTarget;
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            NetworkAvatar.OnAddedDebuffOnTarget -= OnAddedDebuffOnTarget;
        }

        private void OnDisable()
        {
            if ((bool)_ringFx)
            {
                _ringFx.Stop();
                _ringFx = null;
            }
        }

        private void OnAddedDebuffOnTarget(CharacterDebuff debuff, string id)
        {
            if ((bool)debuff && debuff.CompareID(debuffID))
            {
                remainingDuration = burnRingDuration;
                if (!isRingEnabled)
                {
                    NetworkisRingEnabled = true;
                }
            }
        }

        private void HookRingEnabled(bool oldValue, bool newValue)
        {
            if (!WeaponController || !NetworkAvatar)
            {
                return;
            }

            if (newValue)
            {
                if (!ringFxPrefab || (bool)_ringFx)
                {
                    return;
                }

                Transform transform = WeaponController.transform;
                _ringFx = UnityEngine.Object.Instantiate(ringFxPrefab, transform ? transform.position : Vector3.zero, Quaternion.identity);
                _ringFx.SetFollowTarget(transform);
                if (_ringFx is ScriptableFx_Sprite scriptableFx_Sprite)
                {
                    scriptableFx_Sprite.SetSpeed(1f + (float)addedBurnSpeed / 100f);
                }

                _ringFx.Play();
                bool flag = false;
                int ownerIndex = -1;
                foreach (PlayerSpawner multiplayer in PlayerSpawner.MultiplayerList)
                {
                    if ((bool)multiplayer && (WeaponController.gameObject == multiplayer.gameObject || ((bool)NetworkAvatar.NetworkLeader && NetworkAvatar.NetworkLeader.gameObject == multiplayer.gameObject)))
                    {
                        flag = true;
                        ownerIndex = (multiplayer.isOwned ? 1 : 0);
                        break;
                    }
                }

                bool isTransparent = false;
                if (flag)
                {
                    isTransparent = true;
                }

                _ringFx.SetMultiplayerTransparent(isTransparent, ownerIndex);
            }
            else if ((bool)_ringFx)
            {
                _ringFx.Stop();
                _ringFx = null;
            }
        }

        private void HookAddedBurnSpeed(int oldValue, int newValue)
        {
            if (_ringFx is ScriptableFx_Sprite scriptableFx_Sprite)
            {
                scriptableFx_Sprite.SetSpeed(1f + (float)newValue / 100f);
            }
        }

        public override Loc.KeywordValue[] BuildKeywords()
        {
            var list = new List<Loc.KeywordValue>
        {
            new Loc.KeywordValue("NAME", burnRingName.ToString()),
            new Loc.KeywordValue("RADIUS", burnRingRadius.ToString()),
            new Loc.KeywordValue("DURATION", burnRingDuration.ToString()),
            new Loc.KeywordValue("DEBUFF", "<tag=" + debuffKeywordName + ">"),
            new Loc.KeywordValue("TICK_TIME", burnRingTickTimer.time.ToString()),
            new Loc.KeywordValue("RELATED_STAT", "<tag=" + relatedStatKeywordName + ">"),
            new Loc.KeywordValue("DAMAGE", damagePercent.ToString())
        };
            list.AddRange(base.BuildKeywords());
            return list.ToArray();
        }

        public ArcaneWeapon_BurnRing()
        {
            _Mirror_SyncVarHookDelegate_isRingEnabled = HookRingEnabled;
            _Mirror_SyncVarHookDelegate_addedBurnSpeed = HookAddedBurnSpeed;
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
                writer.WriteBool(isRingEnabled);
                writer.WriteVarInt(addedBurnSpeed);
                return;
            }

            writer.WriteVarULong(syncVarDirtyBits);
            if ((syncVarDirtyBits & 4L) != 0L)
            {
                writer.WriteBool(isRingEnabled);
            }

            if ((syncVarDirtyBits & 8L) != 0L)
            {
                writer.WriteVarInt(addedBurnSpeed);
            }
        }

        protected override void DeserializeSyncVars(NetworkReader reader, bool initialState)
        {
            base.DeserializeSyncVars(reader, initialState);
            if (initialState)
            {
                GeneratedSyncVarDeserialize(ref isRingEnabled, _Mirror_SyncVarHookDelegate_isRingEnabled, reader.ReadBool());
                GeneratedSyncVarDeserialize(ref addedBurnSpeed, _Mirror_SyncVarHookDelegate_addedBurnSpeed, reader.ReadVarInt());
                return;
            }

            long num = (long)reader.ReadVarULong();
            if ((num & 4L) != 0L)
            {
                GeneratedSyncVarDeserialize(ref isRingEnabled, _Mirror_SyncVarHookDelegate_isRingEnabled, reader.ReadBool());
            }

            if ((num & 8L) != 0L)
            {
                GeneratedSyncVarDeserialize(ref addedBurnSpeed, _Mirror_SyncVarHookDelegate_addedBurnSpeed, reader.ReadVarInt());
            }
        }
    }
}
