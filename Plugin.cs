using BepInEx;
using HarmonyLib;
using Pigeon.Math;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CraniusMissionMod
{
    [BepInPlugin("com.yourname.craniushunt", "CraniusHunt", "1.0.0")]
    public class CraniusMissionMod : BaseUnityPlugin
    {
        private void Awake()
        {
            var harmony = new Harmony("com.yourname.craniushunt");
            harmony.PatchAll(typeof(MissionSelectWindowInitPatch));
            harmony.PatchAll(typeof(AmalgamationObjectivePatch));
            harmony.PatchAll(typeof(MissionSelectWindowPatch));
            Logger.LogInfo($"{harmony.Id} loaded!");
        }
    }

    // Custom mission class for Cranius Hunt
    public class CraniusMission : AmalgamationMission
    {
        // Inherits all behavior from AmalgamationMission

        // Override to force visibility in the mission select menu
        public override bool CanShowInMissionSelect(out bool showLocked, out string lockedMessage)
        {
            showLocked = false;
            lockedMessage = null;
            return true; // Force true for testing; can add conditions later
        }
    }

    // Patch to create and configure the CraniusMission in MissionSelectWindow.Awake
    [HarmonyPatch(typeof(MissionSelectWindow), "Awake")]
    public class MissionSelectWindowInitPatch
    {
        public static CraniusMission craniusMission;
        public static EnemyClass craniusClass;

        [HarmonyPostfix]
        public static void Postfix(MissionSelectWindow __instance)
        {
            //Debug.Log("CraniusMissionMod: Entering MissionSelectWindow.Awake postfix.");

            // Find the original AmalgamationMission
            AmalgamationMission original = Resources.FindObjectsOfTypeAll<AmalgamationMission>()[0];
            if (original == null)
            {
                //Debug.LogError("CraniusMissionMod: Could not find AmalgamationMission!");
                return;
            }

            // Create CraniusMission
            craniusMission = ScriptableObject.CreateInstance<CraniusMission>();

            // Copy all fields from original AmalgamationMission to CraniusMission
            System.Type missionType = typeof(AmalgamationMission);
            FieldInfo[] missionFields = missionType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in missionFields)
            {
                field.SetValue(craniusMission, field.GetValue(original));
            }

            // Override specific fields for CraniusMission (customize as needed)
            FieldInfo nameField = typeof(Mission).GetField("_missionName", BindingFlags.NonPublic | BindingFlags.Instance);
            if (nameField != null)
            {
                nameField.SetValue(craniusMission, "cranius_hunt"); // Unique ID; assume TextBlocks has entries, or it will use this string
            }

            FieldInfo descriptionField = typeof(Mission).GetField("_description", BindingFlags.NonPublic | BindingFlags.Instance);
            if (descriptionField != null)
            {
                descriptionField.SetValue(craniusMission, "Hunt down the secret boss Cranius!"); // Custom description
            }

            craniusMission.MissionIcon = original.MissionIcon; // Copy icon, or load a custom Sprite if available
            craniusMission.MissionColor = Color.magenta; // Custom color for visibility in UI

            // Set to show in replay menu
            craniusMission.ShowInReplayMenu = true;
            craniusMission.Selectable = true;

            // Find Cranius EnemyClass by checking the spawner
            EnemyClass[] allEnemyClasses = Resources.FindObjectsOfTypeAll<EnemyClass>();
            foreach (EnemyClass ec in allEnemyClasses)
            {
                if (ec.customSpawner is ScrapBossSpawner)
                {
                    craniusClass = ec;
                    break;
                }
            }

            if (craniusClass == null)
            {
                //Debug.LogError("CraniusMissionMod: Could not find Cranius EnemyClass!");
                return;
            }

            // Add CraniusMission to Global.Instance.Missions if not already present
            FieldInfo missionsField = typeof(Global).GetField("Missions", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (missionsField == null)
            {
                //Debug.LogError("CraniusMissionMod: Could not find Missions field in Global!");
                return;
            }

            Mission[] missions = (Mission[])missionsField.GetValue(Global.Instance);
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
                missionsField.SetValue(Global.Instance, newMissions);
                //Debug.Log("CraniusMissionMod: Added Cranius to Global.Missions.");
            }
            else
            {
                //Debug.Log("CraniusMissionMod: Cranius already in Global.Missions; skipping add.");
            }

            // For testing: Set the completion flag for "cranius" to make the mission visible if checks depend on it
            if (PlayerData.Instance != null && PlayerData.Instance.GetFlag("cranius") == 0)
            {
                PlayerData.Instance.SetFlag("cranius", 1);
                //Debug.Log("CraniusMissionMod: Set cranius flag to 1 for testing visibility.");
            }
            if (PlayerData.Instance.GetFlag("cranius") != 1)
            {
                PlayerData.Instance.SetFlag("cranius", 1);
                //Debug.Log("CraniusMissionMod: Set cranius flag to 1 for testing visibility.");
            }
        }
    }

    // Patch to modify the AmalgamationObjective at runtime if the current mission is CraniusMission
    [HarmonyPatch(typeof(AmalgamationObjective), "Setup")]
    public class AmalgamationObjectivePatch
    {
        [HarmonyPostfix]
        public static void Postfix(AmalgamationObjective __instance)
        {
            if (MissionManager.Instance.Mission is CraniusMission)
            {
                // Set enemy class to Cranius
                FieldInfo classField = typeof(AmalgamationObjective).GetField("amalgamationClass", BindingFlags.NonPublic | BindingFlags.Instance);
                if (classField != null)
                {
                    classField.SetValue(__instance, MissionSelectWindowInitPatch.craniusClass);
                }
                else
                {
                    //Debug.LogError("CraniusMissionMod: Could not find amalgamationClass field in AmalgamationObjective!");
                }
            }
        }
    }

    // Patch to force add CraniusMission in the replay window if not present
    [HarmonyPatch(typeof(MissionSelectWindow), "ToggleReplayWindow")]
    public class MissionSelectWindowPatch
    {
        [HarmonyPostfix]
        public static void Postfix(MissionSelectWindow __instance)
        {
            //Debug.Log("CraniusMissionMod: Entering ToggleReplayWindow postfix.");

            if (MissionSelectWindowInitPatch.craniusMission == null)
            {
                //Debug.LogWarning("CraniusMissionMod: craniusMission is null; skipping add.");
                return;
            }

            // Use reflection to access replayMissionContainer
            FieldInfo containerField = typeof(MissionSelectWindow).GetField("replayMissionContainer", BindingFlags.NonPublic | BindingFlags.Instance);
            if (containerField == null)
            {
                //Debug.LogError("CraniusMissionMod: Could not find replayMissionContainer field in MissionSelectWindow!");
                return;
            }

            RectTransform replayMissionContainer = (RectTransform)containerField.GetValue(__instance);
            if (replayMissionContainer == null)
            {
                //Debug.LogError("CraniusMissionMod: replayMissionContainer is null!");
                return;
            }

            // Clean up any duplicate Cranius buttons (destroy extras)
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
                //Debug.Log("CraniusMissionMod: Removed duplicate Cranius buttons in replay window.");
            }

            bool exists = craniusButtons.Count > 0;
            if (exists)
            {
                //Debug.Log("CraniusMissionMod: Cranius button already exists in replay window.");
            }
            else
            {
                // Use reflection to access missionButtonPrefab
                FieldInfo prefabField = typeof(MissionSelectWindow).GetField("missionButtonPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
                if (prefabField == null)
                {
                    //Debug.LogError("CraniusMissionMod: Could not find missionButtonPrefab field in MissionSelectWindow!");
                    return;
                }

                MissionSelectButton prefab = (MissionSelectButton)prefabField.GetValue(__instance);
                if (prefab == null)
                {
                    //Debug.LogError("CraniusMissionMod: missionButtonPrefab is null!");
                    return;
                }

                // Instantiate and setup the button
                MissionSelectButton missionSelectButton = UnityEngine.Object.Instantiate(prefab, replayMissionContainer);
                missionSelectButton.gameObject.SetActive(true);
                Pigeon.Math.Random random = new Pigeon.Math.Random(Global.MissionSelectSeed + (MissionSelectWindowInitPatch.craniusMission.ID.GetHashCode() + 3173) * 5390921);
                WorldRegion worldRegion = MissionSelectWindowInitPatch.craniusMission.OverrideRegion() ?? Global.Instance.Regions[random.Next(Global.Instance.Regions.Length)];
                string sceneName;
                string scene = (MissionSelectWindowInitPatch.craniusMission.OverrideScene(out sceneName) ? sceneName : worldRegion.SceneNames[random.Next(worldRegion.SceneNames.Length)]);
                missionSelectButton.Setup(new MissionData(random.Next(), MissionSelectWindowInitPatch.craniusMission, worldRegion, scene, null), __instance, allowInteraction: true);

                // Force center position for visibility
                RectTransform rect = missionSelectButton.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchorMin = new Vector2(0.5f, 0.5f);
                    rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.anchoredPosition = Vector2.zero;
                    rect.sizeDelta = new Vector2(100f, 100f); // Slightly larger for visibility
                    rect.localScale = Vector3.one * 1.5f; // Scale up
                }

                //Debug.Log("CraniusMissionMod: Manually added CraniusMission button to replay window and centered it.");
            }
        }
    }
}