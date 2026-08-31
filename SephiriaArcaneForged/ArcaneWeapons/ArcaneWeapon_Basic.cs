using Mirror;
using Mirror.RemoteCalls;
using SephiriaArcaneForged.Networks;
using SephiriaArcaneForged.Registries;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_Basic : NetworkBehaviour
    {
        [Header("Effect")]
        public LocalizedString effectsString = new LocalizedString("ArcaneWeapon_XXXX_Effect");
        [Header("Effect Icon")]
        public string effectHUD_ID = "";

        [SyncVar]
        public bool IsEffectEnabled;
        [SyncVar]
        public UnitAvatar Avatar;

        private int itemInstanceID;
        public int entityID;

        public ArcaneWeaponEntity GetEntity()
        {
            return ArcaneWeaponDatabase.FindWeaponById(entityID);
        }
        public string GetAffixText()
        {
            var entity = GetEntity();
            if (entity == null)
                return string.Empty;
            return entity.affix.ToString();
        }
        public string GetEffectText()
        {
            if (effectsString != null)
            {
                return KeywordDatabase.Convert(Loc.Convert(KeywordDatabase.Convert(effectsString.ToString()), BuildKeywords()));
            }
            return "...";
        }
        public bool NetworkIsEffectEnabled
        {
            get
            {
                return IsEffectEnabled;
            }
            [param: In]
            set
            {
                GeneratedSyncVarSetter(value, ref IsEffectEnabled, 1uL, null);
            }
        }
        protected NetworkBehaviourSyncVar ___AvatarNetId;
        public UnitAvatar NetworkAvatar
        {
            get
            {
                return GetSyncVarNetworkBehaviour(___AvatarNetId, ref Avatar);
            }
            [param: In]
            set
            {
                GeneratedSyncVarSetter_NetworkBehaviour(value, ref Avatar, 2uL, null, ref ___AvatarNetId);
            }
        }

        public WeaponControllerSimple WeaponController { get; private set; }

        protected override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
        {
            base.SerializeSyncVars(writer, forceAll);
            if (forceAll)
            {
                writer.WriteBool(IsEffectEnabled);
                writer.WriteNetworkBehaviour(NetworkAvatar);
                return;
            }

            writer.WriteVarULong(syncVarDirtyBits);

            if ((syncVarDirtyBits & 1L) != 0L)
            {
                writer.WriteBool(IsEffectEnabled);
            }

            if ((syncVarDirtyBits & 0x2L) != 0L)
            {
                writer.WriteNetworkBehaviour(NetworkAvatar);
            }
        }

        protected override void DeserializeSyncVars(NetworkReader reader, bool initialState)
        {
            base.DeserializeSyncVars(reader, initialState);
            if (initialState)
            {
                GeneratedSyncVarDeserialize(ref IsEffectEnabled, null, reader.ReadBool());
                GeneratedSyncVarDeserialize_NetworkBehaviour(ref Avatar, null, reader, ref ___AvatarNetId);
                return;
            }

            long num = (long)reader.ReadVarULong();

            if ((num & 1L) != 0L)
            {
                GeneratedSyncVarDeserialize(ref IsEffectEnabled, null, reader.ReadBool());
            }

            if ((num & 0x2L) != 0L)
            {
                GeneratedSyncVarDeserialize_NetworkBehaviour(ref Avatar, null, reader, ref ___AvatarNetId);
            }
        }


        [Server]
        public void Connect(UnitAvatar avatar, int instanceID, int entity)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void ArcaneWeapon_Basic::Connect(UnitAvatar,System.Int32)' called when server was not active");
                return;
            }

            NetworkAvatar = avatar;
            WeaponController = NetworkAvatar.GetComponent<WeaponControllerSimple>();
            itemInstanceID = instanceID;

            EnableEffect();

            OnConnected(instanceID);
        }

        protected virtual void OnConnected(int instanceID)
        {

        }

        [Server]
        public void Disconnect()
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void ArcaneWeapon_Basic::Disconnect()' called when server was not active");
                return;
            }

            DisableEffect();

            OnDisconnected();
            WeaponController = null;
            NetworkAvatar = null;
        }
        private void OnDestroy()
        {
            if (NetworkServer.active && WeaponController != null)
            {
                Core.Logger("OnDestroy");
                if (ArcaneWeaponExtensions.CurrentArcaneWeapons.ContainsKey(WeaponController))
                {
                    var syncList = ArcaneWeaponExtensions.CurrentArcaneWeapons[WeaponController];
                    if(syncList != null && syncList.Contains(this))
                    {
                        Disconnect();
                        syncList.Remove(this);
                        //NetworkServer.Destroy(current.gameObject);
                    }
                }
            }
        }
        protected virtual void OnDisconnected()
        {
        }
        [Server]
        public void EnableEffect()
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void ArcaneWeapon_Basic::EnableEffect()' called when server was not active");
            }
            else if (!IsEffectEnabled)
            {

                NetworkIsEffectEnabled = true;


                CreateEffectHUD();
                LoadItemOnServer(SaveManager.CurrentRun);
                OnEnabledEffect();
            }
        }

        [Server]
        public void DisableEffect()
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void ArcaneWeapon_Basic::DisableEffect(System.Int32,System.Boolean)' called when server was not active");
                return;
            }

            if (IsEffectEnabled)
            {
                RemoveEffectHUD();
                OnDisabledEffect();

                NetworkIsEffectEnabled = false;
            }
        }
        public virtual void CreateEffectHUD()
        {
            if (!string.IsNullOrEmpty(effectHUD_ID))
            {
                NetworkAvatar.CreateEffectHUD(effectHUD_ID, GetCharmHUDID());
            }
        }

        public void RemoveEffectHUD()
        {
            if (!string.IsNullOrEmpty(effectHUD_ID))
            {
                NetworkAvatar.DestroyEffectHUD(GetCharmHUDID());
            }
        }

        public virtual string GetCharmHUDID()
        {
            return $"Charm_{itemInstanceID}";
        }
        public virtual void LoadItemOnServer(ISaveData saveData)
        {
        }

        public virtual void SaveItemOnServer(ISaveData saveData)
        {
        }

        public virtual void OnEnabledEffect()
        {
            
        }
        public virtual void OnDisabledEffect()
        {

        }
        public virtual Loc.KeywordValue[] BuildKeywords()
        {
            return null;
        }
        public override bool Weaved()
        {
            return true;
        }



        public virtual NewWeaponFireData FireData => null;
        public virtual string DamageId => string.Empty;
        public virtual float AttackDashScale => 1f;
        public virtual int MpConsumed => 0;
        public virtual float RangeBonus => 0f;
        public virtual float TargetNoiseScale => 0.2f;
        public virtual float? DamageMultiplier => 1f;
        public virtual float GetDamage(UnitAvatar avatar)
        {
            return GetDamage(avatar, WeaponController);
        }
        public virtual float GetDamage(UnitAvatar avatar, WeaponControllerSimple weapon)
        {
            if (avatar == null || weapon == null || FireData == null)
                return 0;
            return weapon.currentWeapon.InvokeGetRelatedStatMultiplier(avatar, GetDamageElementalType(FireData) ?? EDamageElementalType.Physical, GetRelatedStatFormula(FireData), out var _);
        }
        public virtual EDamageElementalType? GetDamageElementalType(NewWeaponFireData fireData)
        {
            if (fireData == null)
                return EDamageElementalType.Physical;
            return fireData.damageElementalType;
        }
        /// <summary>
        /// (EMPTY), FIREDAMAGE, ICEDAMAGE, LIGHTNINGDAMAGE, PHYSICALDAMAGE, HIGHEST, LOWEST, AVERAGE, AVERAGEALL
        /// </summary>
        /// <param name="fireData"></param>
        /// <returns></returns>
        public virtual string GetRelatedStatFormula(NewWeaponFireData fireData)
        {
            if (fireData == null)
                return string.Empty;
            return fireData.relatedStatFormula;
        }
        public virtual Vector3 GetAimedDelta()
        {
            return WeaponController.attackDirection;
        }
        public virtual int GetTriggerCount()
        {
            int count = 1;
            return count;
        }
        public void Attack(float percent = 100f)
        {
            Attack(WeaponController.attackDirection, percent);
        }
        public virtual void Attack(Vector3 aimedDelta, float percent = 100)
        {
            Attack(GetTriggerCount(), aimedDelta, new List<CombatBehaviour>(), percent);
        }
        public virtual void Attack(CombatBehaviour target, float percent = 100)
        {
            var delta = target.transform.position - FirePosition(WeaponController);
            delta += (Vector3)UnityEngine.Random.insideUnitCircle * TargetNoiseScale;
            Attack(GetTriggerCount(), delta, new List<CombatBehaviour>(), percent);
        }
        public virtual void Attack(int count, Vector3 aimedDelta, List<CombatBehaviour> sharedTarget, float percent)
        {
            if (WeaponController.currentWeapon == null)
                return;
            RpcAttack();

            Vector3 vector = FirePosition(WeaponController);
            float y = WeaponController.shoulder.Position.y;
            NewWeaponFireData attack = FireData;
            if (attack == null)
                return;
            //float damage = WeaponController.currentWeapon.InvokeGetRelatedStatMultiplier(NetworkAvatar, GetDamageElementalType(attack), GetRelatedStatFormula(attack), out var elemental);
            float damage = GetDamage(NetworkAvatar);
            if (damage <= 0)
                return;
            damage = ModifyDamage(damage);
            if (AttackDashScale > 0f)
            {
                GameCamera.Instance.targetTracker.CreateCameraShaking(WeaponController.transform.position, EShakeCameraType.Continous, attack.cameraShakeVelocityOnFire, 0.08f, 0.0625f);
            }
            damage = damage * percent / 100f;
            float rangeBonus = (float)NetworkAvatar.GetCustomStat(ECustomStat.WeaponRange) / 100f + RangeBonus;
            var temp = attack.damageElementalType;
            var tempMultiplier = attack.damageMultiplier;
            var elemental = GetDamageElementalType(FireData);
            if (elemental.HasValue)
                attack.damageElementalType = elemental.Value;
            attack.damageMultiplier = DamageMultiplier ?? tempMultiplier;

            attack.CreateAttack(EDamageFromType.DirectAttack, damage, DamageId, true, NetworkAvatar, vector, vector + aimedDelta, y, OnCreateAttack, sharedTarget, AttackDashScale, null, false, rangeBonus, 1f, MpConsumed, elemental);
            attack.damageElementalType = temp;
            attack.damageMultiplier = tempMultiplier;

            if (count - 1 > 0)
            {
                this.Delay(0.05f, () =>
                {
                    if (IsEffectEnabled && NetworkAvatar != null && !NetworkAvatar.IsDead)
                    {
                        Attack(count - 1, aimedDelta, sharedTarget, percent);
                    }
                });
            }
        }
        protected virtual float ModifyDamage(float damage)
        {
            damage += damage * (float)NetworkAvatar.GetCustomStat(ECustomStat.WeaponDamageBonus) / 100f;
            if (MpConsumed > 0)
            {
                damage += damage * ((float)NetworkAvatar.GetCustomStatUnsafe("MPSKILLDAMAGE") / 100f);
            }
            damage += damage * ((float)NetworkAvatar.GetCustomStat(ECustomStat.FinalWeaponDamage) / 100f);
            return damage;
        }

        protected virtual void OnCreateAttack(int idx, ProjectileBase projectile)
        {

        }
        public virtual Vector3 FirePosition(WeaponControllerSimple simple)
        {
            return simple.shoulder.swingPoint.position - new Vector3(0f, simple.shoulder.Position.y, 0f);
        }
        [ClientRpc]
        public void RpcAttack()
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            var func = "System.Void ArcaneWeapon_Basic::RpcAttack()";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }
        protected virtual void UserCode_RpcAttack()
        {
            try
            {
                if (NetworkAvatar == null)
                    return;
                var weapon = NetworkAvatar.GetComponent<WeaponControllerSimple>();
                if (weapon == null)
                    return;
                float fxScale = 1f + (float)NetworkAvatar.GetCustomStat(ECustomStat.WeaponRange) / 100f + NetworkAvatar.GetCustomStatUnsafe("MACHINARANGE") / 100f + RangeBonus;
                NewWeaponFireData basicAttack = FireData;
                //Core.Logger("FireData: " + basicAttack);
                if (basicAttack == null)
                    return;
                bool flag = false;
                int ownerIndex = -1;
                foreach (PlayerSpawner playerSpawner in PlayerSpawner.MultiplayerList)
                {
                    if (playerSpawner && (weapon.gameObject == playerSpawner.gameObject || (NetworkAvatar.NetworkLeader && NetworkAvatar.NetworkLeader.gameObject == playerSpawner.gameObject)))
                    {
                        flag = true;
                        ownerIndex = (playerSpawner.isOwned ? 1 : 0);
                        break;
                    }
                }
                bool canBeTransparentOnMultiplayer = false;
                if (flag)
                {
                    canBeTransparentOnMultiplayer = true;
                }
                Vector3 position = this.FirePosition(weapon) + new Vector3(0f, weapon.shoulder.Position.y, 0f);
                basicAttack.CreateSwingFx(canBeTransparentOnMultiplayer, weapon.transform, position, weapon.shoulder.transform.eulerAngles, fxScale, ownerIndex, 0f);
            }
            catch (Exception e)
            {
                Core.LoggerWarning(e);
            }
        }

        protected static void InvokeUserCode_RpcAttack(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC RpcAttack called on server.");
            }
            else
            {
                ((ArcaneWeapon_Basic)obj).UserCode_RpcAttack();
            }
        }


        static ArcaneWeapon_Basic()
        {
            RemoteProcedureCalls.RegisterRpc(typeof(ArcaneWeapon_Basic), "System.Void ArcaneWeapon_Basic::RpcAttack()", InvokeUserCode_RpcAttack);
        }
    }
}
