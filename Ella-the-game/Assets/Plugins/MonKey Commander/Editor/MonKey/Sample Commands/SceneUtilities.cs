
using System;
using System.Collections.Generic;
using System.Linq;
using MonKey.Editor.Internal;
using MonKey.Extensions;
using MonKey.Internal;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace MonKey.Editor.Commands
{
    public static class SceneUtilities
    {

        private static void AddAllObjectsInHierarchy(List<GameObject> list, GameObject current, string name)
        {
            if (name.IsNullOrEmpty() || current.name.Contains(name))
                list.Add(current);
            for (int i = 0; i < current.transform.childCount; i++)
            {
                AddAllObjectsInHierarchy(list, current.transform.GetChild(i).gameObject, name);
            }
        }

        public static List<GameObject> GetAllGameObjectsWithName(string name = "")
        {
            List<GameObject> sceneObjects = new List<GameObject>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                foreach (var root in SceneManager.GetSceneAt(i).GetRootGameObjects())
                {
                    AddAllObjectsInHierarchy(sceneObjects, root, name);
                }
            }
            return sceneObjects;
        }

        [Command("Open Scene", "Opens a scene, by default additive, and by default active",
            AlwaysShow = true, Order = -6, QuickName = "OS", ValidationMethodName = "EditModeValidation",
            MenuItemLink = "OpenScene", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Scene")]
        public static void OpenScene(
            [CommandParameter(Help = "The name of the scene to open",
                AutoCompleteMethodName = "SceneNameAutoComplete",ForceAutoCompleteUsage = true,
                OverrideName = "Scene Name",PreventDefaultValueUsage = true)]
            string scenePath,
            OpenSceneMode openMode = OpenSceneMode.Single,
            bool activate = true)
        {
            EditorUtility.
                DisplayProgressBar("Opening " + scenePath.GetAssetNameFromPath(), "Opening Scene...", 1);

            if (openMode == OpenSceneMode.Single)
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            Scene scene = EditorSceneManager.OpenScene(scenePath, openMode);
            if (activate)
                SceneManager.SetActiveScene(scene);
            EditorUtility.ClearProgressBar();
        }

        [Command("Close Scene", QuickName = "CSC", Help = "Closes the scene specified",
            ValidationMethodName = "EditModeValidation",
            Category = "Scene")]
        public static void CloseScene(
             [CommandParameter(Help = "The name of the scene to close"
           ,ForceAutoCompleteUsage = true,
                OverrideName = "Scene Name",PreventDefaultValueUsage = true)]
            Scene scene)
        {
            EditorSceneManager.CloseScene(scene, true);
        }

        [Command("Activate Scene", "Activate the scene of the first selected Object", QuickName = "AS",
            ValidationMethodName = "ObjectSelection", MenuItemLink = "ActivateScene",
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Scene")]
        public static void SetSceneActive()
        {
            SceneManager.SetActiveScene(Selection.activeTransform.gameObject.scene);
        }

        [CommandValidation("Select a scene GameObject first")]
        private static bool ObjectSelection()
        {
            return Selection.activeTransform != null;
        }

        private static AssetNameAutoComplete SceneNameAutoComplete()
        {
            return new AssetNameAutoComplete() { CustomType = "SceneAsset", PopulateOnInit = true };
        }

        [CommandValidation(DefaultValidationMessages.DEFAULT_EDIT_MODE_ONLY)]
        private static bool EditModeValidation()
        {
            return !Application.isPlaying;
        }

        [Command("Move to Scene", "Moves the selected objects to another scene", QuickName = "MS",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "Scene")]
        public static void MoveToScene(
            [CommandParameter(Help = "The name of the scene to move the objects to",
                AutoCompleteMethodName = "SceneNameAutoComplete",ForceAutoCompleteUsage = true,
                OverrideName = "Scene Name",PreventDefaultValueUsage = true)]
            string sceneName)
        {
            EditorUtility.DisplayProgressBar("Moving Objects to Scene...",
                "MonKey is moving your objects to another scene, please wait!", .5f);
            Scene scene = SceneManager.GetSceneByPath(sceneName);
            bool unloadWhenDone;

            if (scene.IsValid())
            {
                unloadWhenDone = false;
            }
            else
            {
                unloadWhenDone = true;
                scene = EditorSceneManager.OpenScene(sceneName, OpenSceneMode.Additive);
            }
            List<Scene> previous = new List<Scene>();

            List<GameObject> toMove = Selection.transforms.Convert(_ => _.gameObject).ToList();

            foreach (var gameObject in toMove)
            {
                previous.Add(gameObject.scene);

                gameObject.transform.parent = null;
                SceneManager.MoveGameObjectToScene(gameObject, scene);
            }

            if (unloadWhenDone)
            {
                foreach (Scene pScene in previous)
                {
                    if (pScene.IsValid())
                        EditorSceneManager.SaveScene(pScene);
                }

                EditorSceneManager.SaveScene(scene);
                EditorSceneManager.CloseScene(scene, true);
            }
            EditorUtility.ClearProgressBar();
        }



        [Command("Duplicate to Scene", "Duplicates the selected objects in another scene", QuickName = "DS",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM,
            Category = "Scene")]
        public static void DuplicateToScene(
            [CommandParameter(Help = "The name of the scene to duplicate the objects to",
                AutoCompleteMethodName = "SceneNameAutoComplete",ForceAutoCompleteUsage = true,
                OverrideName = "Scene Name",PreventDefaultValueUsage = true)]
            string sceneName)
        {
            EditorUtility.DisplayProgressBar("Moving Objects to Scene...",
                "MonKey is duplicating your objects to another scene, please wait!", .5f);
            Scene scene = SceneManager.GetSceneByPath(sceneName);
            bool unloadWhenDone;

            if (scene.IsValid())
            {
                unloadWhenDone = false;
            }
            else
            {
                unloadWhenDone = true;
                scene = EditorSceneManager.OpenScene(sceneName, OpenSceneMode.Additive);
            }
            Scene previous = new Scene();

            foreach (var gameObject in Selection.gameObjects.Where(_ => _.scene.IsValid()))
            {
                if (!previous.IsValid())
                    previous = gameObject.scene;

                GameObject prefabParent = (GameObject)PrefabUtilities.GetPrefabParentPlatformIndependant(gameObject);

                if (prefabParent)
                {
                    PrefabUtility.InstantiatePrefab(prefabParent, scene);
                }
                else
                {
                    GameObject newGameObject = Object.Instantiate(gameObject);
                    SceneManager.MoveGameObjectToScene(newGameObject, scene);
                }
            }

            if (unloadWhenDone && previous.IsValid())
            {
                EditorSceneManager.SaveScene(previous);
                EditorSceneManager.SaveScene(scene);
                EditorSceneManager.CloseScene(scene, true);
            }
            EditorUtility.ClearProgressBar();
        }
    }
}
