using HarmonyLib;
using Mirror;
using SephiriaArcaneForged.ArcaneWeapons;
using SephiriaArcaneForged.Registries;
using SephiriaArcaneForged.Utilities;
using System;
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
    }
}
