using HarmonyLib;
using Mirror;
using Myevan;
using Newtonsoft.Json.Linq;
using SephiriaArcaneForged.Registries;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using static UnityEngine.InputSystem.HID.HID;
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
            public static readonly int EnhanceSlotCount = 8;
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
                    int count = EnhanceSlotCount;
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
                int count = EnhanceSlotCount;
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
                var current = controller.currentWeapon;
                var weaponEntity = current == null ? null : WeaponDatabase.FindWeaponById(current.entityId);
                var buttons = __instance.GetButtons();
                if (controller == null || !controller.CanEquipArcaneWeapon() || buttons == null)
                    return;
                if(__instance.GetAnvil() == null)
                {
                    foreach (EnhancementMetadata item in ArcaneWeaponDatabase.GetAll().Select(x => new EnhancementMetadata() { enhanced = x.weapon }))
                    {
                        if (weaponEntity != null && item.enhanced.id == weaponEntity.id)
                            continue;
                        UI_WeaponEnhancementButton button = ((!(controller.currentWeapon is WeaponSimple_Crossbow)) ? UnityEngine.Object.Instantiate(__instance.buttonPrefab, __instance.tableZone) : UnityEngine.Object.Instantiate(__instance.buttonPrefab_Crossbow, __instance.tableZone));
                        button.SetWeaponMethod(__instance, controller.currentWeapon, item);
                        button.OnSelect += __instance.OnSelect;
                        button.OnDeselect += __instance.OnDeselect;
                        buttons.Add(button);
                    }
                    if (buttons.Count > 0)
                    {
                        __instance.defaultSelectable = buttons[0].gameObject;
                    }
                }
                else
                {
                    __instance.rerollButtonGroup.gameObject.SetActive(true);
                }
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
                if (ArcaneTextBox != null)
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
                    //Core.Logger("child: " + child.gameObject.name);
                    if(child.gameObject.name == "Bullet")
                    {
                        for(int q2 = 0; q2 < child.childCount; q2++)
                        {
                            var bullet = child.GetChild(q2);
                            if (bullet.gameObject.TryGetComponent<Image>(out var bulletImage))
                            {
                                bulletImage.color = Color.red;
                            }
                        }
                    }
                }
                if (icon.WeaponSimple == null)//ジャーナルの場合
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
                    __instance.weaponNameText.text = $"<color=#E5D6FF>{arcane.GetAffixText()}{AffixSpace.ToString().Replace("-", "")}{weaponEntity.aName}</color>";
                }
            }
            [HarmonyPatch("OpenKeywordsCoroutine")]
            [HarmonyPostfix]
            static void OpenKeywordsCoroutinePatch(UI_WeaponTooltip __instance, ref IEnumerator __result)
            {
                Core.Logger("OpenKeywordsCoroutinePatch");
                __result = Add(__result, () =>
                {
                    Core.Logger("OpenKeywordsCoroutine!!!!");
                    if (ArcaneTextBox == null)
                        return;
                    HashSet<string> hashSet = new HashSet<string>();
                    for (int j = 0; j < ArcaneTextBox.text.textInfo.linkCount; j++)
                    {
                        TMP_LinkInfo linkInfo = ArcaneTextBox.text.textInfo.linkInfo[j];
                        hashSet.Add(linkInfo.GetLinkID());
                        KeywordEntity[] keyword = KeywordDatabase.GetConnecteDetailEntity(linkInfo.GetLinkID());
                        for (int k = 0; k < keyword.Length; k++)
                        {
                            MatchCollection matches = Regex.Matches(keyword[k].description.ToString(), "<tag=(.*?)>");
                            List<string> list = new List<string>();
                            foreach (Match item in matches)
                            {
                                if (item.Groups.Count > 1)
                                {
                                    list.Add(item.Groups[1].Value);
                                }
                            }

                            foreach (string item2 in list)
                            {
                                KeywordEntity entity = KeywordDatabase.GetEntity(item2);
                                if (entity != null && entity.displayDetails)
                                {
                                    hashSet.Add(entity.keyword);
                                }
                            }
                        }
                    }

                    foreach (string item3 in hashSet)
                    {
                        KeywordEntity keyword = KeywordDatabase.GetEntity(item3);
                        if (keyword != null)
                        {
                            string text = KeywordDatabase.Convert(keyword.GetRawDescription(), useColor: false, useSprite: false);
                            if (text.Contains("("))
                            {
                                text = Korean.ReplaceJosa(text);
                            }

                            __instance.keywordViewer.AddKeyword(keyword.Convert_Details(useColor: false, useSprite: false), text);
                        }
                    }
                });
            }

            static IEnumerator Add(IEnumerator original, Action added)
            {
                yield return original;
                added?.Invoke();
            }
        }
        [HarmonyPatch(typeof(UI_WeaponIcon))]
        public static class UI_WeaponIconPatch
        {
            public static Sprite NormalFrameSprite;
            public static Sprite ForgedFrameSprite;
            public static Sprite NormalSelectedFrameSprite;
            public static Sprite ForgedSelectedFrameSprite;
            [HarmonyPatch(nameof(UI_WeaponIcon.SetWeapon), new Type[] { typeof(WeaponSimple) })]
            [HarmonyPostfix]
            static void SetWeaponPatch(UI_WeaponIcon __instance, WeaponSimple weapon)
            {
                if (weapon == null)
                    return;
                var arcane = weapon.Networkowner.GetCurrentArcaneWeapon();

                if (__instance.gameObject.TryGetComponent<UI_HorayButton>(out var button))
                {
                    var state = button.spriteState;

                    if (NormalFrameSprite == null)
                        NormalFrameSprite = AssetLoader.LoadSprite(AssetLoader.UIPath + "normal");
                    if (NormalSelectedFrameSprite == null)
                        NormalSelectedFrameSprite = AssetLoader.LoadSprite(AssetLoader.UIPath + "normal_selected");
                    if (ForgedFrameSprite == null)
                        ForgedFrameSprite = AssetLoader.LoadSprite(AssetLoader.UIPath + "forged");
                    if (ForgedSelectedFrameSprite == null)
                        ForgedSelectedFrameSprite = AssetLoader.LoadSprite(AssetLoader.UIPath + "forged_selected");

                    state.disabledSprite = arcane == null ? NormalFrameSprite : ForgedFrameSprite;
                    state.highlightedSprite = arcane == null ? NormalFrameSprite : ForgedFrameSprite;
                    state.pressedSprite = arcane == null ? NormalFrameSprite : ForgedFrameSprite;
                    state.selectedSprite = arcane == null ? NormalSelectedFrameSprite : ForgedSelectedFrameSprite;

                    button.spriteState = state;
                }
                if (__instance.transform.childCount < 1)
                    return;

                if (__instance.transform.GetChild(0).gameObject.TryGetComponent<Image>(out var iconImage))
                {
                    if (NormalFrameSprite == null)
                        NormalFrameSprite = AssetLoader.LoadSprite(AssetLoader.UIPath + "normal");
                    if (NormalFrameSprite != null)
                        iconImage.sprite = NormalFrameSprite;

                    if (arcane == null)
                        return;

                    if (ForgedFrameSprite == null)
                        ForgedFrameSprite = AssetLoader.LoadSprite(AssetLoader.UIPath + "forged");
                    if (ForgedFrameSprite != null)
                        iconImage.sprite = ForgedFrameSprite;
                }
            }
            //[HarmonyPatch(nameof(UI_WeaponIcon.OnSelect))]
            //[HarmonyPostfix]
            [Obsolete]
            static void OnSelectPatch(UI_WeaponIcon __instance)
            {
                if (__instance.WeaponSimple == null)
                    return;

                if (__instance.transform.childCount < 1)
                    return;

                if (__instance.transform.GetChild(0).gameObject.TryGetComponent<Image>(out var iconImage))
                {
                    var arcane = __instance.WeaponSimple.Networkowner.GetCurrentArcaneWeapon();
                    if (arcane == null)
                    {
                        if (NormalFrameSprite == null)
                            NormalFrameSprite = AssetLoader.LoadSprite(AssetLoader.UIPath + "normal");
                        if (NormalFrameSprite != null)
                            iconImage.sprite = NormalFrameSprite;
                    }
                    else
                    {
                        if (ForgedFrameSprite == null)
                            ForgedFrameSprite = AssetLoader.LoadSprite(AssetLoader.UIPath + "forged");
                        if (ForgedFrameSprite != null)
                            iconImage.sprite = ForgedFrameSprite;
                    }
                }
            }
        }
    }
}
