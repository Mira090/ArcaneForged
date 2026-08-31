using Mirror;
using SephiriaArcaneForged.Networks;
using SephiriaArcaneForged.Registries;
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
    }
}
