using HarmonyLib;
using Kitchen;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace KitchenPlayerTeleporter.Patches
{
    [HarmonyPatch]
    static class LocalViewRouter_Patch
    {
        static GameObject _viewPrefabsContainer = null;

        static Dictionary<ViewType, GameObject> _viewPrefabs = new Dictionary<ViewType, GameObject>();

        static MethodInfo m_GetPrefab = typeof(LocalViewRouter).GetMethod("GetPrefab", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyPatch(typeof(LocalViewRouter), "GetPrefab")]
        [HarmonyPrefix]
        static bool GetPrefab_Prefix(ref LocalViewRouter __instance, ViewType view_type, ref GameObject __result)
        {
            if (_viewPrefabsContainer == null)
            {
                _viewPrefabsContainer = new GameObject("Player Teleporter View Prefabs");
                _viewPrefabsContainer.hideFlags = HideFlags.HideAndDontSave;
                _viewPrefabsContainer.SetActive(false);
            }

            if (_viewPrefabs.TryGetValue(view_type, out GameObject cachedPrefab))
            {
                __result = cachedPrefab;
                return false;
            }

            if (view_type == Main.PlayerTeleportBeamView)
            {
                GameObject pingPrefab = (GameObject) m_GetPrefab?.Invoke(__instance, new object[] { ViewType.Ping });

                GameObject playerTeleportBeamPrefab = null;
                if (pingPrefab != null)
                {
                    playerTeleportBeamPrefab = GameObject.Instantiate(pingPrefab);
                    playerTeleportBeamPrefab.name = "PlayerTeleportBeam";
                    playerTeleportBeamPrefab.transform.SetParent(_viewPrefabsContainer.transform);
                    playerTeleportBeamPrefab.transform.Reset();
                    for (int i = 0; i < playerTeleportBeamPrefab.transform.childCount; i++)
                    {
                        Transform childTransform = playerTeleportBeamPrefab.transform.GetChild(i);
                        if (childTransform?.name == "Ping")
                        {
                            Vector3 localScale = childTransform.transform.localScale;
                            localScale.x = 1f;
                            localScale.z = 1f;
                            childTransform.transform.localScale = localScale;
                            break;
                        }
                    }
                }
                _viewPrefabs.Add(view_type, playerTeleportBeamPrefab);
                __result = playerTeleportBeamPrefab;
                return false;
            }

            return true;
        }
    }
}
