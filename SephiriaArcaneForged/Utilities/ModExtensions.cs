using Mirror;
using SephiriaArcaneForged.Networks;
using SephiriaArcaneForged.Registries;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.Utilities
{
    public static class ModExtensions
    {
        /// <summary>
        /// ネットワークプレハブの親オブジェクト
        /// </summary>
        /// <remarks>非アクティブのゲームオブジェクトの子オブジェクトにNetworkIdentityを追加すると、Awake()が呼ばれない。</remarks>
        public static GameObject PrefabHolder
        {
            get
            {
                if (_prefabHolder == null)
                {
                    _prefabHolder = new GameObject(Core.ModName + "_PrefabHolder");
                    _prefabHolder.SetActive(false);
                    UnityEngine.Object.DontDestroyOnLoad(_prefabHolder);
                }
                return _prefabHolder;
            }
        }
        private static GameObject _prefabHolder;

        public static ushort ToFunctionHashCode(this string function)
        {
            return (ushort)((uint)function.GetStableHashCode() & 0xFFFFu);
        }
        public static GameObject SetAssetId(this GameObject gameObject, uint assetId)
        {
            if (gameObject == null)
            {
                Core.LoggerError($"GameObject is null!");
                return gameObject;
            }
            //Core.Logger(gameObject.name + ": " + assetId);
            if (gameObject.TryGetComponent<NetworkIdentity>(out var identity))
            {
                UnityEngine.Object.DestroyImmediate(identity);
            }
            /*
            if (gameObject.TryGetComponent<ModAssetId>(out var mod))
            {
                Core.LoggerError($"GameObject {gameObject} has already ModAssetId");
                mod.AssetId = assetId;
                return gameObject;
            }*/
            //gameObject.AddComponent<ModAssetId>().AssetId = assetId;
            gameObject.transform.SetParent(PrefabHolder.transform);
            gameObject.AddComponent<NetworkIdentity>();
            return gameObject;
        }
    }
}
