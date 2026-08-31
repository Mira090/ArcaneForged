using HarmonyLib;
using Mirror;
using Mirror.RemoteCalls;
using SephiriaArcaneForged.ArcaneWeapons;
using SephiriaArcaneForged.Registries;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.Networks
{
    public static class ArcaneWeaponExtensions
    {
        public static bool HasArcaneWeapon(this UnitAvatar avatar)
        {
            if (!avatar.TryGetComponent<WeaponControllerSimple>(out var controller))
                return false;
            return controller.HasArcaneWeapon();
        }
        public static bool HasArcaneWeapon(this WeaponControllerSimple controller)
        {
            return CurrentArcaneWeapons.ContainsKey(controller) && CurrentArcaneWeapons[controller].Count > 0;
        }
        public static bool CanEquipArcaneWeapon(this WeaponControllerSimple controller)
        {
            WeaponEntity weaponEntity = WeaponDatabase.FindWeaponById(controller.currentWeapon.entityId);
            if (weaponEntity == null)
                return false;
            var weaponEnhancements = WeaponDatabase.GetWeaponEnhancements(weaponEntity.id);
            if (weaponEnhancements != null && weaponEnhancements.Count > 0)
                return false;
            return !controller.HasArcaneWeapon();
        }

        public static readonly Dictionary<WeaponControllerSimple, SyncList<ArcaneWeapon_Basic>> CurrentArcaneWeapons = new Dictionary<WeaponControllerSimple, SyncList<ArcaneWeapon_Basic>>();
        public static ArcaneWeapon_Basic GetCurrentArcaneWeapon(this WeaponControllerSimple controller)
        {
            if (!CurrentArcaneWeapons.ContainsKey(controller) || CurrentArcaneWeapons[controller].Count == 0)
                return null;
            return CurrentArcaneWeapons[controller].FirstOrDefault();
        }
        public static int GetCurrentArcaneWeaponId(this WeaponControllerSimple controller)
        {
            var current = GetCurrentArcaneWeapon(controller);
            if (current == null)
                return -1;
            var entity = current.GetEntity();
            if(entity == null)
                return -1;
            return entity.id;
        }
        public static void EquipArcaneWeapon(this WeaponControllerSimple controller, bool fromTownObject, int weaponID, bool showFx = false)
        {
            if (controller.isServer)
            {
                LocalEquipArcaneWeapon(controller, fromTownObject, weaponID, showFx);
            }
            else
            {
                CmdEquipArcaneWeapon(controller, fromTownObject, weaponID, showFx);
            }
        }
        [Command]
        public static void CmdEquipArcaneWeapon(this WeaponControllerSimple controller, bool fromTownObject, int weaponID, bool showFx)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteBool(fromTownObject);
            writer.WriteVarInt(weaponID);
            writer.WriteBool(showFx);
            var func = "System.Void WeaponControllerSimple::CmdEquipArcaneWeapon(System.Boolean,System.Int32,System.Boolean)";
            controller.InvokeSendCommandInternal(func, func.ToFunctionHashCode(), writer, 0);
            NetworkWriterPool.Return(writer);
        }

        [Server]
        public static void LocalEquipArcaneWeapon(this WeaponControllerSimple controller, bool fromTownObject, int weaponID, bool showFx)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void WeaponControllerSimple::LocalEquipWeapon(System.Boolean,System.Int32,System.Boolean)' called when server was not active");
                return;
            }

            var current = controller.GetCurrentArcaneWeapon();
            if (current != null)
            {
                controller.LocalUnequipArcaneWeapon();
            }

            var weaponEntity = ArcaneWeaponDatabase.FindWeaponById(weaponID);
            GameObject gameObject = UnityEngine.Object.Instantiate(weaponEntity.resourcePrefab);
            var arcane = gameObject.GetComponent<ArcaneWeapon_Basic>();
            if (controller.connectionToClient == null)
            {
                NetworkServer.Spawn(gameObject);
            }
            else
            {
                NetworkServer.Spawn(gameObject, controller.gameObject);
            }

            arcane.Connect(controller.unitAvatar, ItemDatabase.GenerateInstanceID(new System.Random()), weaponEntity.id);
            CurrentArcaneWeapons[controller].Add(arcane);
            //OnWeaponEquippeedServerside?.Invoke(component);
            controller.InvokeRpcEquipWeapon(controller.currentWeapon);
            if (showFx)
            {
                Vector3 position = controller.unitAvatar.transform.position;
                float centerYPos = controller.unitAvatar.TopdownActor.CenterYPos;
                //controller.RpcCreateWeaponEquipFx(position, centerYPos);
            }

            if (controller.unitAvatar.Inventory == null)
                return;

            using (new GridInventory.Permission(controller.unitAvatar.Inventory))
            {
                //このusingが解放されると、OnCharmEffectRefreshed()などが呼ばれる
            }
        }
        public static void UnequipArcaneWeapon(this WeaponControllerSimple controller)
        {
            if (controller.isServer)
            {
                LocalUnequipArcaneWeapon(controller);
            }
            else
            {
                CmdUnequipArcaneWeapon(controller);
            }
        }
        [Command]
        public static void CmdUnequipArcaneWeapon(this WeaponControllerSimple controller)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            var func = "System.Void WeaponControllerSimple::CmdUnequipArcaneWeapon()";
            controller.InvokeSendCommandInternal(func, func.ToFunctionHashCode(), writer, 0);
            NetworkWriterPool.Return(writer);
        }

        [Server]
        public static void LocalUnequipArcaneWeapon(this WeaponControllerSimple controller)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void WeaponControllerSimple::LocalUnequipArcaneWeapon()' called when server was not active");
                return;
            }

            var current = controller.GetCurrentArcaneWeapon();
            Core.Logger("current: " + current);
            if (current != null)
            {
                current.Disconnect();
                CurrentArcaneWeapons[controller].Remove(current);
                NetworkServer.Destroy(current.gameObject);
                //OnWeaponEquippeedServerside?.Invoke(null);
            }
        }
        public static void UserCode_CmdEquipArcaneWeapon__Boolean__Int32__Boolean(this WeaponControllerSimple controller, bool fromTownObject, int weaponID, bool showFx)
        {
            controller.LocalEquipArcaneWeapon(fromTownObject, weaponID, showFx);
        }

        public static void InvokeUserCode_CmdEquipArcaneWeapon__Boolean__Int32__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkServer.active)
            {
                Debug.LogError("Command CmdEquipWeapon called on client.");
            }
            else
            {
                ((WeaponControllerSimple)obj).UserCode_CmdEquipArcaneWeapon__Boolean__Int32__Boolean(reader.ReadBool(), reader.ReadVarInt(), reader.ReadBool());
            }
        }
        public static void UserCode_CmdArcaneUnequipArcaneWeapon(this WeaponControllerSimple controller)
        {
            controller.LocalUnequipArcaneWeapon();
        }

        public static void InvokeUserCode_CmdArcaneUnequipArcaneWeapon(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkServer.active)
            {
                Debug.LogError("Command CmdUnequipWeapon called on client.");
            }
            else
            {
                ((WeaponControllerSimple)obj).UserCode_CmdArcaneUnequipArcaneWeapon();
            }
        }
        static ArcaneWeaponExtensions()
        {
            RemoteProcedureCalls.RegisterCommand(typeof(WeaponControllerSimple), "System.Void WeaponControllerSimple::CmdEquipArcaneWeapon(System.Boolean,System.Int32,System.Boolean)", InvokeUserCode_CmdEquipArcaneWeapon__Boolean__Int32__Boolean, requiresAuthority: true);
            RemoteProcedureCalls.RegisterCommand(typeof(WeaponControllerSimple), "System.Void WeaponControllerSimple::CmdUnequipArcaneWeapon()", InvokeUserCode_CmdArcaneUnequipArcaneWeapon, requiresAuthority: true);
        }

        [HarmonyPatch(typeof(WeaponControllerSimple))]
        public static class WeaponControllerSimplePatch
        {
            [HarmonyPatch("LocalEquipWeapon")]
            [HarmonyPostfix]
            static void LocalEquipWeaponPatch(WeaponControllerSimple __instance)
            {
                Core.Logger("LocalEquipWeaponPatch");
                __instance.LocalUnequipArcaneWeapon();
            }
            [HarmonyPatch(MethodType.Constructor)]
            [HarmonyPostfix]
            static void ConstructorPatch(WeaponControllerSimple __instance)
            {
                if (CurrentArcaneWeapons.ContainsKey(__instance))
                    return;
                CurrentArcaneWeapons[__instance] = new SyncList<ArcaneWeapon_Basic>();
                __instance.InvokeInitSyncObject(CurrentArcaneWeapons[__instance]);
            }
        }
        [HarmonyPatch(typeof(PlayerSpawner))]
        public static class PlayerSpawnerPatch
        {
            [HarmonyPatch("Initialize")]
            [HarmonyPostfix]
            static void InitPatch(PlayerSpawner __instance)
            {
                int version = SaveManager.CurrentRun.GetInt("SaveVersion", 0);
                int arcaneId = -1;
                if (__instance.PlayerAvatar.NetworkisInDungeon != 0)
                {
                    string key = (version == 0) ? "PlayerArcaneWeapon" : string.Format("Player{0}ArcaneWeapon", __instance.currentPlayerIdxForSave);
                    arcaneId = SaveManager.CurrentRun.GetInt(key, arcaneId);
                }
                if (ArcaneWeaponDatabase.FindWeaponById(arcaneId) != null)
                {
                    __instance.GetWeaponController().EquipArcaneWeapon(false, arcaneId, false);
                }
            }
            [HarmonyPatch(nameof(PlayerSpawner.SaveCurrentSessionData))]
            [HarmonyPostfix]
            static void SavePatch(PlayerSpawner __instance)
            {
                if (SaveManager.SaveVersion == 0)
                {
                    var controller = __instance.GetWeaponController();
                    if (__instance.GetWeaponController().currentWeapon && controller.HasArcaneWeapon())
                    {
                        SaveManager.CurrentRun.SetInt("PlayerArcaneWeapon", controller.GetCurrentArcaneWeaponId());
                    }
                    else
                    {
                        SaveManager.CurrentRun.SetInt("PlayerArcaneWeapon", -1);
                    }
                }
                else
                {
                    var controller = __instance.GetWeaponController();
                    if (__instance.GetWeaponController().currentWeapon && controller.HasArcaneWeapon())
                    {
                        SaveManager.CurrentRun.SetInt(string.Format("Player{0}ArcaneWeapon", __instance.currentPlayerIdxForSave), controller.GetCurrentArcaneWeaponId());
                    }
                    else
                    {
                        SaveManager.CurrentRun.SetInt(string.Format("Player{0}ArcaneWeapon", __instance.currentPlayerIdxForSave), -1);
                    }
                }
            }
        }
    }
}
