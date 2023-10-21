using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace KitchenPlayerTeleporter.Customs
{
    public class Portal : CustomAppliance
    {
        private GameObject _prefab;
        private GameObject PrefabCache
        {
            get
            {
                if (_prefab == null)
                {
                    GameObject temp = ((Appliance)GDOUtils.GetExistingGDO(459840623))?.Prefab;
                    if (temp != null)
                    {
                        _prefab = GameObject.Instantiate(temp);

                        GameObject container = new GameObject("PortalContainer");
                        container.SetActive(false);
                        container.hideFlags = HideFlags.HideAndDontSave;

                        _prefab.transform.SetParent(container.transform);
                        _prefab.transform.Reset();

                        _prefab.name = "Portal";
                        DestroyComponentsInChildren<Collider>(_prefab);
                        DestroyComponentsInChildren<NavMeshObstacle>(_prefab);

                        Transform label1 = GetChildTransform(_prefab, "Label 1");
                        if (label1 != null)
                        {
                            label1.localPosition = new Vector3(0f, 0.12f, -0.05f);
                            label1.localRotation = Quaternion.Euler(90f, 0f, 0f);
                        }

                        Transform label2 = GetChildTransform(_prefab, "Label 2");
                        if (label2 != null)
                        {
                            GameObject.DestroyImmediate(label2.gameObject);
                        }

                        Transform teleporter = GetChildTransform(_prefab, "Teleporter");
                        if (teleporter != null)
                        {
                            string[] activeNames = new string[]
                            {
                                "Arrows",
                                "Rubber",
                                "Surface"
                            };

                            teleporter.localPosition = new Vector3(0f, -0.37f, 0f);
                            for (int i = 0; i < teleporter.childCount; i++)
                            {
                                GameObject child = teleporter.GetChild(i).gameObject;
                                if (activeNames.Contains(child.name))
                                    continue;
                                child.SetActive(false);
                            }
                        }

                        void DestroyComponentsInChildren<T>(GameObject gameObject) where T : Component, new()
                        {
                            T[] components = _prefab.GetComponentsInChildren<T>();
                            for (int i = components.Length - 1; i > -1; i--)
                            {
                                Component.DestroyImmediate(components[i]);
                            }
                        }

                        Transform GetChildTransform(GameObject gameObject, string path)
                        {
                            Transform transform = gameObject.transform;
                            string[] subpaths = path.Split('/');
                            for (int i = 0; i < subpaths.Length && transform != null; i++)
                            {
                                transform = transform.Find(subpaths[i]);
                            }
                            return transform;
                        }
                    }
                }
                return _prefab;
            }
        }

        public override string UniqueNameID => "portal";

        public override GameObject Prefab => PrefabCache;

        public override List<IApplianceProperty> Properties => new List<IApplianceProperty>()
        {
            new CConveyTeleport(),
            new CPortal()
        };

        public override List<(Locale, ApplianceInfo)> InfoList => new List<(Locale, ApplianceInfo)>()
        {
            (Locale.English, LocalisationUtils.CreateApplianceInfo("Portal", "Interacting teleports player to a connected portal. Requires two.", new List<Appliance.Section>(), new List<string>()))
        };

        public override PriceTier PriceTier => PriceTier.ExtremelyExpensive;

        public override ShoppingTags ShoppingTags => ShoppingTags.Misc;

        public override List<Appliance> Upgrades => new List<Appliance>()
        {
            (Appliance)GDOUtils.GetExistingGDO(459840623) // Teleporter
        };
    }
}
