using HarmonyLib;
using Mirror;
using Newtonsoft.Json.Linq;
using SephiriaArcaneForged.Registries;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace SephiriaArcaneForged.Networks
{
    public static class AnvilPatches
    {
        [HarmonyPatch(typeof(StageEntity), nameof(StageEntity.GenerateStage))]
        public static class GenerateStagePatch
        {
            static void Prefix(ref bool createAnvil)
            {
                if (createAnvil)
                    return;
                int count = 0;
                foreach (KeyValuePair<int, NetworkConnectionToClient> keyValuePair in NetworkServer.connections)
                {
                    if (keyValuePair.Value.identity && keyValuePair.Value.identity.TryGetComponent<WeaponControllerSimple>(out var player) && player.HasArcaneWeapon())
                    {
                        count++;
                    }
                }
                createAnvil = count < PlayerSpawner.MultiplayerList.Count;
                //DungeonManager.LocalLoadStage()で呼び出される
            }
        }
        [HarmonyPatch(typeof(StageEntity_Choice), nameof(StageEntity_Choice.GenerateStage))]
        public static class GenerateStageChoicePatch
        {
            static void Prefix(ref bool createAnvil)
            {
                if (createAnvil)
                    return;
                int count = 0;
                foreach (KeyValuePair<int, NetworkConnectionToClient> keyValuePair in NetworkServer.connections)
                {
                    if (keyValuePair.Value.identity && keyValuePair.Value.identity.TryGetComponent<WeaponControllerSimple>(out var player) && player.HasArcaneWeapon())
                    {
                        count++;
                    }
                }
                createAnvil = count < PlayerSpawner.MultiplayerList.Count;
                //DungeonManager.LocalLoadStage()で呼び出される
            }
        }
        [HarmonyPatch(typeof(StageEntity_GrasslandTown), nameof(StageEntity_GrasslandTown.GenerateStage))]
        public static class GenerateStageGlasslandTownPatch
        {
            static void Prefix(ref bool createAnvil)
            {
                if (createAnvil)
                    return;
                int count = 0;
                foreach (KeyValuePair<int, NetworkConnectionToClient> keyValuePair in NetworkServer.connections)
                {
                    if (keyValuePair.Value.identity && keyValuePair.Value.identity.TryGetComponent<WeaponControllerSimple>(out var player) && player.HasArcaneWeapon())
                    {
                        count++;
                    }
                }
                createAnvil = count < PlayerSpawner.MultiplayerList.Count;
                //DungeonManager.LocalLoadStage()で呼び出される
            }
        }
        [HarmonyPatch(typeof(StageEntity_Infinity), nameof(StageEntity_Infinity.GenerateStage))]
        public static class GenerateStageInfinityPatch
        {
            static void Prefix(ref bool createAnvil)
            {
                if (createAnvil)
                    return;
                int count = 0;
                foreach (KeyValuePair<int, NetworkConnectionToClient> keyValuePair in NetworkServer.connections)
                {
                    if (keyValuePair.Value.identity && keyValuePair.Value.identity.TryGetComponent<WeaponControllerSimple>(out var player) && player.HasArcaneWeapon())
                    {
                        count++;
                    }
                }
                createAnvil = count < PlayerSpawner.MultiplayerList.Count;
                //DungeonManager.LocalLoadStage()で呼び出される
            }
        }
        [HarmonyPatch(typeof(Anvil))]
        public static class AnvilPatch
        {
            [HarmonyPatch("HandleInteraction")]
            [HarmonyPrefix]
            static void HandleInteractionPatch(Anvil __instance, GameObject actor)
            {
                if (__instance.GetLocalEnhanced())
                    return;
                if (!actor.TryGetComponent<PlayerAvatar>(out var playerAvatar) || playerAvatar.IsInBattle)
                    return;
                if (!playerAvatar.TryGetComponent<WeaponControllerSimple>(out var controller))
                    return;
                var currentWeapon = controller.currentWeapon;
                if (currentWeapon == null)
                    return;
                var weaponEntity = WeaponDatabase.FindWeaponById(currentWeapon.entityId);
                if (weaponEntity == null)
                    return;
                if (!__instance.GetLocalWeaponListInitialized() && controller.CanEquipArcaneWeapon())
                {
                    __instance.SetLocalWeaponListInitialized(true);
                    int seed = __instance.RandomID + playerAvatar.RandomID + __instance.localRerollSeedOffset;
                    Debug.Log("Initialize local weapon list seed: " + seed.ToString());
                    Random random = new Random(seed);
                    int count = __instance.enhanceSlotCount;
                    if (playerAvatar)
                    {
                        count += playerAvatar.GetCustomStatUnsafe("EXTRAWEAPONCHOICES");
                    }
                    for (int i = 0; i < count; i++)
                    {
                        WeaponEntity[] alreadyList = __instance.localWeaponList.Select(x => x.enhanced).ToArray();
                        var randomEnhancement = ArcaneWeaponDatabase.GetRandomEnhancement(random, weaponEntity, alreadyList);
                        if (randomEnhancement.enhanced == null)
                            continue;
                        __instance.localWeaponList.Add(randomEnhancement);
                    }
                }
            }

            [HarmonyPatch(nameof(Anvil.Reroll))]
            [HarmonyPostfix]
            static void RerollPatch(Anvil __instance, PlayerAvatar player)
            {
                if (!NetworkClient.active)
                    return;
                if (player == null)
                    return;
                if (!player.TryGetComponent<WeaponControllerSimple>(out var controller))
                    return;
                WeaponSimple currentWeapon = controller.currentWeapon;
                if (currentWeapon == null)
                    return;
                WeaponEntity weaponEntity = WeaponDatabase.FindWeaponById(currentWeapon.entityId);
                if (weaponEntity == null)
                    return;
                if (!controller.CanEquipArcaneWeapon())
                    return;
                int seed = __instance.RandomID + player.RandomID + __instance.localRerollSeedOffset;
                Debug.Log("Reroll local weapon list seed: " + seed.ToString());
                Random random = new Random(seed);
                __instance.localWeaponList.Clear();
                int count = __instance.enhanceSlotCount;
                count += player.GetCustomStatUnsafe("EXTRAWEAPONCHOICES");
                for (int i = 0; i < count; i++)
                {
                    WeaponEntity[] alreadyList = __instance.localWeaponList.Select(x => x.enhanced).ToArray();
                    var randomEnhancement = ArcaneWeaponDatabase.GetRandomEnhancement(random, weaponEntity, alreadyList);
                    if (randomEnhancement.enhanced == null)
                        continue;
                    __instance.localWeaponList.Add(randomEnhancement);
                }
                if (UIManager.Instance)
                {
                    UIManager.Instance.GetElement<UI_WeaponEnhancementPanel>().UpdateList();
                }
            }
        }
        [HarmonyPatch(typeof(UI_WeaponEnhancementPanel))]
        public static class UI_WeaponEnhancementPanelPatch
        {
            public static readonly LocalizedString EnhancementMessage = new LocalizedString("UI_ArcaneWeapon_EnhancementMessage");
            [HarmonyPatch(nameof(UI_WeaponEnhancementPanel.Enhance))]
            [HarmonyPrefix]
            static bool EnhancePatch(UI_WeaponEnhancementPanel __instance, UI_WeaponEnhancementButton button)
            {
                var weapon = button.enhancementMetadata.enhanced;
                var controller = __instance.GetWeaponController();
                var anvil = __instance.GetAnvil();
                var anvilSound = __instance.GetAnvilSound();
                if (controller == null || !controller.CanEquipArcaneWeapon())
                    return true;

                UIManager.Instance.GetElement<UI_MessageBoxHolder>().OpenYesNo(Loc.Convert(EnhancementMessage.ToString(), "WEAPONNAME", weapon.Name), () =>
                {
                    int id = weapon.id;
                    controller.EquipArcaneWeapon(fromTownObject: false, id);
                    if (anvil != null)
                    {
                        anvil.EnhanceClient();
                    }

                    if (anvilSound != null)
                    {
                        anvilSound.PlayEnhanceSound();
                    }

                    __instance.Close();
                }, null);
                return false;
            }

            [HarmonyPatch(nameof(UI_WeaponEnhancementPanel.UpdateList))]
            [HarmonyPostfix]
            static void UpdateListPatch(UI_WeaponEnhancementPanel __instance)
            {
                var controller = __instance.GetWeaponController();
                if (controller == null || !controller.CanEquipArcaneWeapon())
                    return;
                __instance.rerollButtonGroup.gameObject.SetActive(true);
            }
        }
        [HarmonyPatch(typeof(UI_WeaponEnhancementButton))]
        public static class UI_WeaponEnhancementButtonPatch
        {
            [HarmonyPatch(nameof(UI_WeaponEnhancementButton.SetWeaponMethod))]
            [HarmonyPostfix]
            static void SetWeaponMethodPatch(UI_WeaponEnhancementButton __instance, UI_WeaponEnhancementPanel panel, WeaponSimple currentWeapon, EnhancementMetadata enhancementMetadata)
            {
                if (currentWeapon == null || enhancementMetadata == null || enhancementMetadata.enhanced == null)
                    return;
                if (currentWeapon.Networkowner == null || !currentWeapon.Networkowner.CanEquipArcaneWeapon())
                    return;
                var arcane = ArcaneWeaponDatabase.FindWeaponById(enhancementMetadata.enhanced);
                if (arcane == null)
                    return;
                __instance.effectText.text = arcane.GetEffectText();
            }
        }
        [HarmonyPatch(typeof(UI_WeaponTooltip))]
        public static class UI_WeaponTooltipPatch
        {
            public static UI_SimpleTextBox ArcaneTextBox;
            public static readonly LocalizedString AffixSpace = new LocalizedString("UI_ArcaneWeapon_AffixSpace");

            [HarmonyPatch(nameof(UI_WeaponTooltip.Open))]
            [HarmonyPostfix]
            static void OpenPatch(UI_WeaponTooltip __instance, IUITooltipOpener target, RectTransform attached, Vector2 offset, ITooltip data)
            {
                UI_WeaponIcon icon = target as UI_WeaponIcon;
                if (icon == null)
                    return;
                if(ArcaneTextBox != null)
                {
                    ArcaneTextBox.gameObject.SetActive(false);
                }

                WeaponEntity weaponEntity = data as WeaponEntity;
                if(data is WeaponSimple simple)
                    weaponEntity = WeaponDatabase.FindWeaponById(simple.entityId);
                if (weaponEntity == null)
                    return;

                if(ArcaneTextBox == null)
                {
                    var example = __instance.upgradeTextBoxes.LastOrDefault();
                    if (example == null)
                        return;
                    ArcaneTextBox = UnityEngine.Object.Instantiate(example, example.transform.parent);
                }
                for (int q = 0; q < ArcaneTextBox.transform.childCount; q++)
                {
                    var child = ArcaneTextBox.transform.GetChild(q);
                    Core.Logger("child: " + child.gameObject.name);
                    foreach (var comp in child.GetComponents<MonoBehaviour>())
                    {
                        Core.Logger("compornent: " + comp.GetType());
                    }
                }
                if(icon.WeaponSimple == null)//ジャーナルの場合
                {
                    var arcane = ArcaneWeaponDatabase.FindWeaponById(weaponEntity);
                    if (arcane == null)
                        return;
                    ArcaneTextBox.gameObject.SetActive(true);
                    ArcaneTextBox.SetText(arcane.GetEffectText());
                }
                else//装備中の場合
                {
                    var arcane = icon.WeaponSimple.Networkowner.GetCurrentArcaneWeapon();
                    if (arcane == null)
                        return;
                    ArcaneTextBox.gameObject.SetActive(true);
                    ArcaneTextBox.SetText(arcane.GetEffectText());
                    __instance.weaponNameText.text = $"<color=#E5D6FF>{arcane.GetAffixText()}{AffixSpace}{weaponEntity.aName}</color>";
                }
            }
        }
    }
}
