using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public class SparrohPlugin : BaseUnityPlugin
{
    public const string PluginGUID = "sparroh.craniushunt";
    public const string PluginName = "CraniusHunt";
    public const string PluginVersion = "1.0.3";

    public static ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        try
        {
            var harmony = new Harmony(PluginGUID);
            harmony.PatchAll(typeof(MissionSelectWindowInitPatch));
            harmony.PatchAll(typeof(AmalgamationObjectivePatch));
            harmony.PatchAll(typeof(MissionSelectWindowPatch));
            Logger.LogInfo($"{PluginName} loaded successfully.");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to initialize {PluginName}: {ex}");
        }
    }
}

public class CraniusMission : AmalgamationMission
{
    public override bool CanShowInMissionSelect(out bool showLocked, out string lockedMessage)
    {
        showLocked = false;
        lockedMessage = null;
        return true;
    }
}

[HarmonyPatch(typeof(MissionSelectWindow), "Awake")]
public class MissionSelectWindowInitPatch
{
    public static CraniusMission craniusMission;
    public static EnemyClass craniusClass;

    [HarmonyPostfix]
    public static void Postfix(MissionSelectWindow __instance)
    {
        try
        {
            AmalgamationMission original = null;
            foreach (Mission m in Global.Instance.Missions)
            {
                if (m is AmalgamationMission am)
                {
                    original = am;
                    break;
                }
            }
            if (original == null)
            {
                original = ScriptableObject.CreateInstance<AmalgamationMission>();
                original.MissionIcon = Global.Instance.Missions[0].MissionIcon;
                original.MissionColor = Color.magenta;
            }

            craniusMission = ScriptableObject.CreateInstance<CraniusMission>();

            System.Type missionType = typeof(AmalgamationMission);
            FieldInfo[] missionFields = missionType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in missionFields)
            {
                field.SetValue(craniusMission, field.GetValue(original));
            }

            FieldInfo nameField = typeof(Mission).GetField("_missionName", BindingFlags.NonPublic | BindingFlags.Instance);
            if (nameField != null)
            {
                nameField.SetValue(craniusMission, "cranius_hunt");
            }

            FieldInfo descriptionField = typeof(Mission).GetField("_description", BindingFlags.NonPublic | BindingFlags.Instance);
            if (descriptionField != null)
            {
                descriptionField.SetValue(craniusMission, "Hunt down the secret boss Cranius!");
            }

            craniusMission.MissionIcon = original.MissionIcon;
            craniusMission.MissionColor = Color.magenta;

            craniusMission.ShowInReplayMenu = true;
            craniusMission.Selectable = true;

            foreach (EnemyClass ec in Global.Instance.EnemyClasses)
            {
                if (ec.customSpawner is ScrapBossSpawner)
                {
                    craniusClass = ec;
                    break;
                }
            }

            if (craniusClass == null)
            {
                return;
            }

            Mission[] missions = Global.Instance.Missions;
            bool alreadyAdded = false;
            foreach (Mission m in missions)
            {
                if (m is CraniusMission)
                {
                    alreadyAdded = true;
                    break;
                }
            }

            if (!alreadyAdded)
            {
                Mission[] newMissions = new Mission[missions.Length + 1];
                Array.Copy(missions, newMissions, missions.Length);
                newMissions[missions.Length] = craniusMission;
                Global.Instance.Missions = newMissions;
            }

            if (PlayerData.Instance != null && PlayerData.Instance.GetFlag("cranius") == 0)
            {
                PlayerData.Instance.SetFlag("cranius", 1);
            }
            if (PlayerData.Instance.GetFlag("cranius") != 1)
            {
                PlayerData.Instance.SetFlag("cranius", 1);
            }
        }
        catch (Exception ex)
        {
            SparrohPlugin.Logger.LogError($"Error in MissionSelectWindowInitPatch: {ex}");
        }
    }
}

[HarmonyPatch(typeof(AmalgamationObjective), "Setup")]
public class AmalgamationObjectivePatch
{
    [HarmonyPostfix]
    public static void Postfix(AmalgamationObjective __instance)
    {
        try
        {
            if (MissionManager.Instance.Mission is CraniusMission)
            {
                FieldInfo classField = typeof(AmalgamationObjective).GetField("amalgamationClass", BindingFlags.NonPublic | BindingFlags.Instance);
                if (classField != null)
                {
                    classField.SetValue(__instance, MissionSelectWindowInitPatch.craniusClass);
                }
            }
        }
        catch (Exception ex)
        {
            SparrohPlugin.Logger.LogError($"Error in AmalgamationObjectivePatch: {ex}");
        }
    }
}

[HarmonyPatch(typeof(MissionSelectWindow), "ToggleReplayWindow")]
public class MissionSelectWindowPatch
{
    [HarmonyPostfix]
    public static void Postfix(MissionSelectWindow __instance)
    {
        try
        {
            if (MissionSelectWindowInitPatch.craniusMission == null)
            {
                return;
            }

            FieldInfo containerField = typeof(MissionSelectWindow).GetField("replayMissionContainer", BindingFlags.NonPublic | BindingFlags.Instance);
            if (containerField == null)
            {
                return;
            }

            RectTransform replayMissionContainer = (RectTransform)containerField.GetValue(__instance);
            if (replayMissionContainer == null)
            {
                return;
            }

            List<MissionSelectButton> craniusButtons = new List<MissionSelectButton>();
            for (int i = 0; i < replayMissionContainer.childCount; i++)
            {
                MissionSelectButton btn = replayMissionContainer.GetChild(i).GetComponent<MissionSelectButton>();
                if (btn != null && btn.Mission.Mission == MissionSelectWindowInitPatch.craniusMission)
                {
                    craniusButtons.Add(btn);
                }
            }

            if (craniusButtons.Count > 1)
            {
                for (int i = 1; i < craniusButtons.Count; i++)
                {
                    UnityEngine.Object.Destroy(craniusButtons[i].gameObject);
                }
            }

            bool exists = craniusButtons.Count > 0;
            if (exists)
            {
            }
            else
            {
                FieldInfo prefabField = typeof(MissionSelectWindow).GetField("missionButtonPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
                if (prefabField == null)
                {
                    return;
                }

                MissionSelectButton prefab = (MissionSelectButton)prefabField.GetValue(__instance);
                if (prefab == null)
                {
                    return;
                }

                MissionSelectButton missionSelectButton = UnityEngine.Object.Instantiate(prefab, replayMissionContainer);
                missionSelectButton.gameObject.SetActive(true);
                Pigeon.Math.Random random = new Pigeon.Math.Random(Global.MissionSelectSeed + (MissionSelectWindowInitPatch.craniusMission.ID.GetHashCode() + 3173) * 5390921);
                WorldRegion worldRegion = MissionSelectWindowInitPatch.craniusMission.OverrideRegion() ?? Global.Instance.Regions[random.Next(Global.Instance.Regions.Length)];
                int seed = random.Next();
                SceneData sceneData;
                if (MissionSelectWindowInitPatch.craniusMission.OverrideScene(out sceneData, worldRegion, seed))
                {
                    missionSelectButton.Setup(new MissionData(seed, MissionSelectWindowInitPatch.craniusMission, worldRegion, sceneData, null, 0), __instance, allowInteraction: true);
                }
                else
                {
                    SceneData sceneDataFallback = worldRegion.Scenes[random.Next(worldRegion.Scenes.Length)];
                    missionSelectButton.Setup(new MissionData(seed, MissionSelectWindowInitPatch.craniusMission, worldRegion, sceneDataFallback, null, 0), __instance, allowInteraction: true);
                }

                RectTransform rect = missionSelectButton.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchorMin = new Vector2(0.5f, 0.5f);
                    rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.anchoredPosition = Vector2.zero;
                    rect.sizeDelta = new Vector2(100f, 100f);
                    rect.localScale = Vector3.one * 1.5f;
                }
            }
        }
        catch (Exception ex)
        {
            SparrohPlugin.Logger.LogError($"Error in MissionSelectWindowPatch: {ex}");
        }
    }
}
