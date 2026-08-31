using Mirror;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SephiriaArcaneForged.Utilities
{
    public static class ReflectionExtensions
    {
        public static void SetAssetId(this NetworkIdentity instance, uint assetId)
        {
            var prop = instance.GetType().GetProperty(nameof(NetworkIdentity.assetId));
            prop.SetValue(instance, assetId);
        }
        public static void InvokeSendRPCInternal(this NetworkBehaviour instance, string functionFullName, int functionHashCode, NetworkWriter writer, int channelId, bool requiresAuthority = true)
        {
            var type = typeof(NetworkBehaviour);
            var method = type.GetMethod("SendRPCInternal", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { functionFullName, functionHashCode, writer, channelId, requiresAuthority });
        }
        public static void InvokeSendCommandInternal(this NetworkBehaviour instance, string functionFullName, int functionHashCode, NetworkWriter writer, int channelId, bool requiresAuthority = true)
        {
            var type = typeof(NetworkBehaviour);
            var method = type.GetMethod("SendCommandInternal", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { functionFullName, functionHashCode, writer, channelId, requiresAuthority });
        }
        public static void InvokeInitSyncObject(this NetworkBehaviour instance, SyncObject syncObject)
        {
            var type = typeof(NetworkBehaviour);
            var method = type.GetMethod("InitSyncObject", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { syncObject });
        }
        public static bool GetLocalWeaponListInitialized(this Anvil instance)
        {
            return (bool)typeof(Anvil).GetField("localWeaponListInitialized", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(instance);
        }
        public static void SetLocalWeaponListInitialized(this Anvil instance, bool value)
        {
            typeof(Anvil).GetField("localWeaponListInitialized", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(instance, value);
        }
        public static bool GetLocalEnhanced(this Anvil instance)
        {
            var prop = instance.GetType().GetProperty("LocalEnhanced", BindingFlags.Instance | BindingFlags.NonPublic);
            return (bool)prop.GetValue(instance);
        }
        public static WeaponControllerSimple GetWeaponController(this UI_WeaponEnhancementPanel instance)
        {
            return (WeaponControllerSimple)typeof(UI_WeaponEnhancementPanel).GetField("weaponController", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static WeaponControllerSimple GetWeaponController(this PlayerSpawner instance)
        {
            return (WeaponControllerSimple)typeof(PlayerSpawner).GetField("weaponController", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static Anvil GetAnvil(this UI_WeaponEnhancementPanel instance)
        {
            return (Anvil)typeof(UI_WeaponEnhancementPanel).GetField("anvil", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static IAnvilSound GetAnvilSound(this UI_WeaponEnhancementPanel instance)
        {
            return (IAnvilSound)typeof(UI_WeaponEnhancementPanel).GetField("anvilSound", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static List<UI_WeaponEnhancementButton> GetButtons(this UI_WeaponEnhancementPanel instance)
        {
            return (List<UI_WeaponEnhancementButton>)typeof(UI_WeaponEnhancementPanel).GetField("buttons", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }
        public static void InvokeRpcEquipWeapon(this WeaponControllerSimple instance, WeaponSimple weapon)
        {
            var type = typeof(WeaponControllerSimple);
            var method = type.GetMethod("RpcEquipWeapon", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { weapon });
        }
        public static float InvokeGetRelatedStatMultiplier(this WeaponSimple instance, UnitAvatar owner, EDamageElementalType elementalType, string relatedStatFormula, out EDamageElementalType result)
        {
            try
            {
                var type = typeof(WeaponSimple);
                var method = type.GetMethod("GetRelatedStatMultiplier", BindingFlags.Instance | BindingFlags.NonPublic);
                var parameters = new object[] { owner, elementalType, relatedStatFormula, null };
                var value = method.Invoke(instance, parameters);
                result = (EDamageElementalType)parameters[^1];
                return (float)value;
            }
            catch (Exception ex)
            {
                Core.Logger("InvokeGetRelatedStatMultiplier failed");
                Core.LoggerError(ex);
                result = EDamageElementalType.Normal;
                return 0f;
            }
        }
    }
}
