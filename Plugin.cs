using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public class SparrohPlugin : BaseUnityPlugin
{
    public const string PluginGUID = "sparroh.craniushunt";
    public const string PluginName = "CraniusHunt";
    public const string PluginVersion = "1.1.0";

    public static ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        try
        {
            var harmony = new Harmony(PluginGUID);
            harmony.PatchAll(typeof(MissionSelectWindowInitPatch));
            Logger.LogInfo($"{PluginName} loaded successfully.");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to initialize {PluginName}: {ex}");
        }
    }
}

public class CraniusMission : FlatTundraMission
{
    public override bool OverrideScene(out SceneData scene, WorldRegion region, int seed)
    {
        scene = new SceneData("Tundra1Flat");
        return true;
    }

    public override void ModifyTeamRevives(ref int revives)
    {
        revives = 0;
    }

    public override bool GetResetDateTime(out long unixTime)
    {
        unixTime = 0;
        return false;
    }

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

    [HarmonyPostfix]
    public static void Postfix(MissionSelectWindow __instance)
    {
        try
        {
            Mission original = Global.Instance.Missions[0];

            foreach (WorldRegion region in Global.Instance.Regions)
            {
                string sceneNames = string.Join(", ", System.Linq.Enumerable.Select(region.Scenes, s => s.scene));
            }

            craniusMission = ScriptableObject.CreateInstance<CraniusMission>();

            System.Type missionType = typeof(Mission);
            FieldInfo[] missionFields = missionType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in missionFields)
            {
                field.SetValue(craniusMission, field.GetValue(original));
            }

            FieldInfo nameField = typeof(Mission).GetField("_missionName", BindingFlags.NonPublic | BindingFlags.Instance);
            if (nameField != null)
            {
                nameField.SetValue(craniusMission, "Cranius Hunt");
            }

            FieldInfo descriptionField = typeof(Mission).GetField("_description", BindingFlags.NonPublic | BindingFlags.Instance);
            if (descriptionField != null)
            {
                descriptionField.SetValue(craniusMission, "Hunt down the secret boss Cranius!");
            }

            craniusMission.MissionIcon = original.MissionIcon;
            craniusMission.MissionColor = Color.magenta;

            craniusMission.ShowInReplayMenu = false;
            craniusMission.Selectable = true;

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
