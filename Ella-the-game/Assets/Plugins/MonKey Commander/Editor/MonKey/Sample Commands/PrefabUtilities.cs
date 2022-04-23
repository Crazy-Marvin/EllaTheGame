using MonKey;
using MonKey.Editor.Internal;
using MonKey.Extensions;
using MonKey.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonKey.Editor.Commands
{
    public static class PrefabUtilities
    {
        [Command("Select Prefab Root", QuickName = "SPR",
            Help = "Selects the prefab roots of the selected objects",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            Category = "Prefab")]
        public static void SelectPrefabRoots()
        {
            List<Object> roots = new List<Object>();

            foreach (GameObject o in Selection.gameObjects)
            {
                roots.Add(PrefabUtility.GetOutermostPrefabInstanceRoot(o));
            }

            Selection.objects = roots.ToArray();
        }

        [Command("New Prefab", QuickName = "NPR",
            Help = "Create a prefab out of the selected objects in a folder you can specify",
            ValidationMethodName = "ValidateCreatePrefabs",
            Category = "Prefab")]
        public static void CreatePrefabs()
        {
            CreatePrefabs(Selection.gameObjects);
        }

        [CommandValidation("You must select at least one GameObject")]
        public static bool ValidateCreatePrefabs()
        {
            return (Selection.activeGameObject != null);
        }

        [Command("New Prefab Variants", "Creates prefab variants of the selected prefab", Category = "Assets",
            QuickName = "PV")]
        public static void CreatePrefabVariant(int duplicateAmount = 1)
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(gameObject);
                string path = AssetDatabase.GetAssetPath(gameObject);

                for (int i = 0; i < duplicateAmount; i++)
                {
                    string newPath = AssetDatabase.GenerateUniqueAssetPath(path);
                    PrefabUtility.SaveAsPrefabAsset(obj,newPath);
                }

                Object.DestroyImmediate(obj);
            }
        }

        private static void CreatePrefabs(GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0)
                return;

            // Ask user for the asset path to store prefabs
            string savePath = EditorUtility.SaveFilePanelInProject("Prefab Creation", "Prefab",
                "prefab", "Select the prefab's folder", "Assets");

            if (savePath.Length <= 0)
                return;
            if (!savePath.Contains(".prefab"))
                savePath += ".prefab";

            int i = 0;
            foreach (GameObject go in gameObjects)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Prefab creation", "In Progress!" +
                                                                                  go.name,
                    (i + .5f) / Selection.gameObjects.Length))
                    break;
                i++;

                string savePathArranged = savePath.Insert(savePath.IndexOf(".",
                        StringComparison.Ordinal), "-" + go.name);
                PrefabUtility.SaveAsPrefabAssetAndConnect(go,savePathArranged, InteractionMode.AutomatedAction);
            }

            EditorUtility.ClearProgressBar();
        }

        public static GameObject[] CreatePrefabDefaultObjects()
        {
            return Selection.gameObjects;
        }

        [Command("Apply Prefab", Help = "Applies the changes on the selected prefabs",
            QuickName = "AP",
            Category = "Prefab")]
        public static void ApplyPrefabs()
        {
            int i = 0;
            List<GameObject> parentApplied = new List<GameObject>();
            foreach (GameObject go in Selection.gameObjects.Where(_ => _.scene.IsValid()
                                                                       && _.hideFlags == HideFlags.None))
            {
                if (EditorUtility.DisplayCancelableProgressBar("Applying Prefabs", "Processing..." + go.name,
                    (i + .5f) / Selection.gameObjects.Length))
                    break;
                i++;
                GameObject applied = ApplySinglePrefab(parentApplied, go);

                if (applied != null)
                    parentApplied.Add(applied);
            }

            EditorUtility.ClearProgressBar();
        }

        [CommandValidation("Select Some Prefabs First")]
        private static bool ValidateApplyPrefabs()
        {
            return Selection.activeGameObject != null;
        }

        internal static Object GetPrefabParentPlatformIndependant(GameObject child)
        {
#if UNITY_2018_3_OR_NEWER
            GameObject prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(child);

#elif UNITY_2018_2_OR_NEWER
            Type pUtilityType = typeof(PrefabUtility);
            MethodInfo info = pUtilityType.GetMethod("GetCorrespondingObjectFromSource");
            //stupid ass reflection because of some method signature change in the last Unity
            GameObject prefabParent = info.Invoke(null,new object[]{child}) as GameObject;
#else
            GameObject prefabParent = PrefabUtility.GetPrefabParent(child) as GameObject;
#endif

            return prefabParent;
        }


        [Command("Select Instances", "Selects all instances of the specified prefab", QuickName = "SI",
            AlwaysShow = true, Order = -5, MenuItemLink = "SelectPrefabInstances",
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Prefab")]
        public static void SelectInstancesOfPrefab(
            [CommandParameter(Help = "The prefab to find the instances of on the opened scenes",
                ForceAutoCompleteUsage = true,
                DefaultValueNameOverride = "Prefab Selected",
                AutoCompleteMethodName = "PrefabAutoComplete")]
            string prefabName = "")
        {
            GameObject prefab;
            if (prefabName == "")
                prefab = Selection.objects.First(_ => _ is GameObject) as GameObject;
            else
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabName);

            if (prefab == null)
            {
                Debug.LogWarning("Monkey Error: the prefab was not recognized");
                return;
            }

            EditorUtility.DisplayProgressBar("Finding Prefab Instances...",
                "Please Wait while Monkey is looking for the prefab instances!", 0);
            List<Transform> transs = TransformExt.GetAllTransformedOrderUpToDown();
            int i = 0;

            List<GameObject> selectedInstances = new List<GameObject>();

            foreach (Transform transform in transs)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Finding Prefab Instances...",
                    "Please Wait while Monkey is looking for the prefab instances!",
                    ((float)i / transs.Count) * .5f + .5f))
                    break;

                if (prefab.IsPrefab())
                {
#if UNITY_2018_3_OR_NEWER
                    GameObject prefabRoot = PrefabUtility.GetNearestPrefabInstanceRoot(transform.gameObject);
#else
                    GameObject prefabRoot = PrefabUtility.FindPrefabRoot(transform.gameObject);
#endif
                    // Update the prefab of the game object
                    Object prefabParent = prefabRoot ? GetPrefabParentPlatformIndependant(prefabRoot) : null;

                    if (prefabParent == prefab && !selectedInstances.Contains(prefabRoot))
                        selectedInstances.Add(prefabRoot);
                }
                else
                {
                    //then it's a model
                    MeshFilter filter = transform.gameObject.GetComponent<MeshFilter>();
                    if (filter)
                    {
                        MeshFilter[] filters = prefab.GetComponentsInChildren<MeshFilter>();
                        if (filters.Any(_ => _.sharedMesh == filter.sharedMesh))
                            selectedInstances.Add(filter.gameObject);
                    }
                }

                i++;
            }

            Selection.objects = selectedInstances.Convert(_ => _ as Object).ToArray();
            EditorUtility.ClearProgressBar();
        }

        public static AssetNameAutoComplete PrefabAutoComplete()
        {
            return new AssetNameAutoComplete() { CustomType = "GameObject" };
        }

        public static GameObject ApplySinglePrefab(List<GameObject> excludedPrefabs, GameObject gameObject,
            bool forceActive = false, bool resetTransform = false)
        {
            GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
            // Update the prefab of the game object
            GameObject prefabParent = (GameObject)GetPrefabParentPlatformIndependant(prefabRoot);

            if (excludedPrefabs.Contains(prefabParent))
                return null;


            if (prefabParent != null)
            {
                bool isActive = gameObject.activeSelf;
                Transform t = prefabRoot.transform;
                Vector3 localPosition = t.localPosition;
                Quaternion localRotation = t.localRotation;

                if (forceActive)
                    gameObject.SetActive(true);

                if (resetTransform)
                {
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                }

                PrefabUtility.SavePrefabAsset(prefabRoot);

                if (forceActive)
                    gameObject.SetActive(isActive);

                if (resetTransform)
                {
                    t.localPosition = localPosition;
                    t.localRotation = localRotation;
                }

                return prefabParent;
            }

            return null;
        }

        [Command("Revert Prefab Instances",
            "Reverts all the changes made to the prefab instance to the base prefab",
            QuickName = "RPI",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            Category = "Prefab")]
        public static void RevertToPrefabInstances()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                if (PrefabUtility.GetPrefabInstanceStatus(o) != PrefabInstanceStatus.NotAPrefab)
                    PrefabUtility.RevertObjectOverride(o,InteractionMode.AutomatedAction);
            }
        }

#if UNITY_2018_3_OR_NEWER || UNITY_2019

        [Command("Open Prefab", "Opens a prefab", AlwaysShow = true, Category = "Prefab", QuickName = "OP", DefaultValidation = DefaultValidation.IN_EDIT_MODE)]
        private static void OpenPrefab([CommandParameter(AutoCompleteMethodName = "PrefabNameAutoComplete", ForceAutoCompleteUsage = true, Help = "The name of the prefab to open",
                OverrideName = "Prefab Name", PreventDefaultValueUsage = true)]
            string prefabPath)
        {
            Type utils = typeof(PrefabStageUtility);
            var methods = utils.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            if (methods.Length == 0)
                Debug.Log("The Open Prefab Method wasn't found: Unity may have changed its API, " +
                          "please contact the MonKey support");

            foreach (var info in methods)
            {
                if (info.Name == "OpenPrefab" && info.GetParameters().Length == 1)
                {
                    info.Invoke(null, new object[] { prefabPath });
                    return;
                }

            }


        }

        private static AssetNameAutoComplete PrefabNameAutoComplete()
        {
            return new AssetNameAutoComplete()
            {
                CustomType = "GameObject",
                IncludeDirectories = false,
                PopulateOnInit = true,
            };
        }

        [Command("Remove Component Of Type From Prefabs In Folder",
      Help = "Removes all the component of the given type on all the prefabs present in a given folder, recursively")]
        public static void RemoveComponentsFromPrefabsInFolder(
      [CommandParameter(AutoCompleteMethodName = "FolderAutoComplete", Help = "The name of the folder",
            ForceAutoCompleteUsage = true)]
        string folderName,
      [CommandParameter(Help = "The Type of Component", AutoCompleteMethodName = "ComponentTypeAuto")]
        Type componentType)
        {
            MonKey.Editor.Commands.SelectionUtilities.FindFolder(folderName);
            string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string[] folders = AssetDatabase.GetSubFolders(folderName);

            List<string> foldersList = new List<string>(folders);

            foldersList.Add(folderName);

            string[] prefabPaths = AssetDatabase.FindAssets("t:GameObject", foldersList.ToArray());
            foreach (var path in prefabPaths)
            {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(path));
                if (!go)
                    return;

                var instance = (GameObject)PrefabUtility.InstantiatePrefab(go);

                var comps = instance.GetComponentsInChildren(componentType);
                var originalComps = go.GetComponentsInChildren(componentType);
                for (var index = 0; index < comps.Length; index++)
                {
                    var component = comps[index];
                    var prefabComponent = originalComps[index];
                    Object.DestroyImmediate(component);
                    PrefabUtility.ApplyRemovedComponent(instance, prefabComponent, InteractionMode.AutomatedAction);
                }

                Object.DestroyImmediate(instance);
            }

            AssetDatabase.SaveAssets();
        }

        public static AssetNameAutoComplete FolderAutoComplete()
        {
            return new AssetNameAutoComplete() { DirectoryMode = true };
        }

        public static TypeAutoComplete ComponentTypeAuto()
        {
            return new TypeAutoComplete(false, true, true, false, false);
        }

#endif
    }
}