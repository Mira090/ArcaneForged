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
                UnityEngine.Object.Destroy(identity);
            }
            if (gameObject.TryGetComponent<ModAssetId>(out var mod))
            {
                Core.LoggerError($"GameObject {gameObject} has already ModAssetId");
                mod.AssetId = assetId;
                return gameObject;
            }
            gameObject.AddComponent<ModAssetId>().AssetId = assetId;
            return gameObject;
        }
    }
}
