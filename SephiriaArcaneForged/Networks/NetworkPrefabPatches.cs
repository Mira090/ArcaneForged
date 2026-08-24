using HarmonyLib;
using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SephiriaArcaneForged.Networks
{
    public static class NetworkPrefabPatches
    {

        [HarmonyPatch(typeof(UnityEngine.Object))]
        public static class ObjectInstantiatePatch
        {
            [HarmonyPatch("Internal_CloneSingle")]
            [HarmonyPostfix]
            static void Postfix0(ref UnityEngine.Object __result, UnityEngine.Object data)
            {
                Patch(ref __result, data);
            }
            [HarmonyPatch(nameof(UnityEngine.Object.Instantiate), new Type[] { typeof(UnityEngine.Object), typeof(Vector3), typeof(Quaternion) })]
            [HarmonyPostfix]
            static void Postfix1(ref UnityEngine.Object __result, UnityEngine.Object original)
            {
                //Core.Logger(original.name + ": Patch");
                Patch(ref __result, original);
            }
            [HarmonyPatch(nameof(UnityEngine.Object.Instantiate), new Type[] { typeof(UnityEngine.Object), typeof(Vector3), typeof(Quaternion), typeof(Transform) })]
            [HarmonyPostfix]
            static void Postfix2(ref UnityEngine.Object __result, UnityEngine.Object original)
            {
                Patch(ref __result, original);
            }
            [HarmonyPatch(nameof(UnityEngine.Object.Instantiate), new Type[] { typeof(UnityEngine.Object) })]
            [HarmonyPostfix]
            static void Postfix3(ref UnityEngine.Object __result, UnityEngine.Object original)
            {
                Patch(ref __result, original);
            }
            [HarmonyPatch(nameof(UnityEngine.Object.Instantiate), new Type[] { typeof(UnityEngine.Object), typeof(Transform), typeof(bool) })]
            [HarmonyPostfix]
            static void Postfix4(ref UnityEngine.Object __result, UnityEngine.Object original)
            {
                Patch(ref __result, original);
            }
            [HarmonyPatch(nameof(UnityEngine.Object.Instantiate), new Type[] { typeof(UnityEngine.Object), typeof(Scene) })]
            [HarmonyPostfix]
            static void Postfix5(ref UnityEngine.Object __result, UnityEngine.Object original)
            {
                Patch(ref __result, original);
            }
            static void Patch(ref UnityEngine.Object __result, UnityEngine.Object original)
            {
                //Core.Logger(__result + ": " + original);
                if (original == null || __result == null)
                    return;
                //Core.Logger(__result.GetType() + ": " + (__result is GameObject g && g.TryGetComponent<ModAssetId>(out var _)));
                if (__result is GameObject gameObject && gameObject.TryGetComponent<ModAssetId>(out var mod))
                {
                    //Core.Logger(gameObject.name + ": " + mod.AssetId);
                    mod.ToIdentity();
                }
                else if (__result is MonoBehaviour behaviour && behaviour.gameObject.TryGetComponent<ModAssetId>(out var mod2))
                {
                    //Core.Logger(behaviour.name + ": " + mod2.AssetId);
                    mod2.ToIdentity();
                }
            }
        }
        [HarmonyPatch(typeof(NetworkClient), nameof(NetworkClient.GetPrefab))]
        public static class NetworkClientGetPrefabPatch
        {
            static void Postfix(uint assetId, ref GameObject prefab, ref bool __result)
            {
                if (!ModAssetId.CustomNetworkPrefabs.ContainsKey(assetId))
                    return;
                prefab = ModAssetId.CustomNetworkPrefabs[assetId];
                __result = prefab != null;
            }
        }
    }
}
