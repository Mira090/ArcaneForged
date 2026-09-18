using HarmonyLib;
using Mirror;
using SephiriaArcaneForged.ArcaneWeapons;
using SephiriaArcaneForged.Registries;
using SephiriaArcaneForged.Utilities;
using System;
using System.Linq;
using UnityEngine;

namespace SephiriaArcaneForged
{
    public class Core : HorayModBase
    {
        public static readonly string ModName = "ArcaneForged";
        public static void Logger(string message)
        {
            Debug.Log($"[{ModName}] " + message);
        }
        public static void LoggerWarning(string message)
        {
            Debug.LogWarning($"[{ModName}] " + message);
        }
        public static void LoggerWarning(System.Exception message)
        {
            Debug.LogWarning($"[{ModName}] " + message);
        }
        public static void LoggerError(string message)
        {
            Debug.LogError($"[{ModName}] " + message);
        }
        public static void LoggerError(System.Exception message)
        {
            Debug.LogError($"[{ModName}] " + message);
        }
        public static Core Instance { get; private set; }
        public static Harmony ModPatches { get; private set; }
        public static bool IsInitialized { get; private set; } = false;
        protected override void OnModLoaded()
        {
            base.OnModLoaded();

            if (!IsInitialized)
            {
                IsInitialized = true;
                Instance = this;

                ModPatches = new Harmony("com.Mira." + ModName);
                ModPatches.PatchAll();

                Data.Init();

                HorayModAPI.OnLoadWeaponDatabase += OnLoadWeaponDatabase;
                HorayModAPI.OnLocalizationReady += AssetLoader.LoadLocalization;

                Writer<ArcaneWeapon_Basic>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
                Reader<ArcaneWeapon_Basic>.read = NetworkReaderExtensions.ReadNetworkBehaviour<ArcaneWeapon_Basic>;
            }
        }

        private void OnLoadWeaponDatabase()
        {
            ArcaneWeaponDatabase.Initialize();
        }

        protected override void OnModUnloaded()
        {
            IsInitialized = false;

            HorayModAPI.OnLoadWeaponDatabase -= OnLoadWeaponDatabase;
            HorayModAPI.OnLocalizationReady -= AssetLoader.LoadLocalization;

            if (ModPatches != null)
            {
                ModPatches.UnpatchSelf();
            }

            base.OnModUnloaded();
        }
        [HarmonyPatch(typeof(Resources), nameof(Resources.LoadAll), new Type[] { typeof(string), typeof(Type) })]
        public static class ResourcesLoadAllPatch
        {
            static void Postfix(string path, Type systemTypeInstance, ref UnityEngine.Object[] __result)
            {
                if (systemTypeInstance == typeof(EffectHUDEntity) && path == "EffectHUD")
                {
                    var list = __result.ToList();

                    Data.RegisterEffectHUDs(list);

                    __result = list.ToArray();
                }
                if (systemTypeInstance == typeof(DamageIdEntity) && path == "DamageId")
                {
                    //一回
                    var list = __result.ToList();

                    Data.RegisterDamageIds(list);

                    __result = list.ToArray();
                }
                if (systemTypeInstance == typeof(StatusEntity) && path == "Status")
                {
                    //複数回
                    var list = __result.ToList();

                    //Data.RegisterStatuses(list);

                    __result = list.ToArray();
                }
                if (systemTypeInstance == typeof(KeywordEntity) && path == "Keyword")
                {
                    var list = __result.ToList();

                    //Data.RegisterKeywords(list);

                    __result = list.ToArray();
                }
            }
        }
    }
}
