using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EasyMobile.Editor
{
    internal static class EM_BuiltinObjectCreator
    {
        // Built-in object name
        public const string ClipPlayerName = "ClipPlayer";
        public const string ClipPlayerUIName = "ClipPlayerUI";
        public const string CanvasName = "Canvas";
        public const string EventSystemName = "EventSystem";

        [System.Obsolete("This method was deprecated since the EasyMobile prefab is no longer used.")]
        internal static GameObject CreateEasyMobilePrefab(bool showAlert = false)
        {
            // Stop if the prefab is already created.
            string prefabPath = EM_Constants.MainPrefabPath;
            GameObject existingPrefab = EM_EditorUtil.GetMainPrefab();

            if (existingPrefab != null)
            {
                if (showAlert)
                {
                    EM_EditorUtil.Alert("Prefab Exists", "EasyMobile.prefab already exists at " + prefabPath);
                }

                return existingPrefab;
            }

            // Make sure the containing folder exists.
            FileIO.EnsureFolderExists(EM_Constants.MainPrefabFolder);

            // Create a temporary gameObject and then create the prefab from it.
            GameObject tmpObj = new GameObject(EM_Constants.MainPrefabName);

            // Generate the prefab from the temporary game object.
            GameObject prefabObj = PrefabUtility.CreatePrefab(prefabPath, tmpObj);
            GameObject.DestroyImmediate(tmpObj);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (showAlert)
            {
                EM_EditorUtil.Alert("Prefab Created", "EasyMobile.prefab was created at " + prefabPath);
            }
            else
            {
                Debug.Log("EasyMobile.prefab was created at " + prefabPath);
            }

            return prefabObj;
        }

        internal static EM_Settings CreateEMSettingsAsset()
        {
            // Stop if the asset is already created.
            EM_Settings instance = EM_Settings.LoadSettingsAsset();
            if (instance != null)
            {
                return instance;
            }

            // Create Resources folder if it doesn't exist.
            FileIO.EnsureFolderExists(EM_Constants.ResourcesFolder);

            // Now create the asset inside the Resources folder.
            instance = EM_Settings.Instance; // this will create a new instance of the EMSettings scriptable.
            AssetDatabase.CreateAsset(instance, EM_Constants.SettingsAssetPath);    
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("EM_Settings.asset was created at " + EM_Constants.SettingsAssetPath);

            return instance;
        }

        //------------------------------------------------------------------
        // Saved Games (EM Pro)
        //------------------------------------------------------------------
        #if EASY_MOBILE_PRO
        internal static GameObject CreateClipPlayer(GameObject parent = null)
        {
            // Calculate object index in the current context
            int nameIndex = CalculateGameObjectIndex(parent, ClipPlayerName);

            // Create a quad
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Quad);

            // Give it a name
            player.name = nameIndex > 0 ? ClipPlayerName + " (" + nameIndex + ")" : ClipPlayerName;

            // Set its material
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(EM_Constants.ClipPlayerMaterialPath);
            player.GetComponent<MeshRenderer>().material = mat;

            // Add the ClipPlayer component
            player.AddComponent<ClipPlayer>();

            // Add it to its parent
            GameObjectUtility.SetParentAndAlign(player, parent);

            return player;
        }

        internal static GameObject CreateClipPlayerUI(GameObject parent = null)
        {
            // Since this is a UI element, we need to make sure it lives inside a canvas
            if (parent == null || (parent != null && parent.transform.root.GetComponent<Canvas>() == null))
            {
                var existingCanvas = GameObject.FindObjectOfType<Canvas>();
                if (existingCanvas != null)
                    parent = existingCanvas.gameObject;
                else
                    parent = CreateCanvas(null);    // create a new canvas at scene root
            }
        
            int nameIndex = CalculateGameObjectIndex(parent, ClipPlayerUIName);
            GameObject player = new GameObject(nameIndex > 0 ? ClipPlayerUIName + " (" + nameIndex + ")" : ClipPlayerUIName);
            player.AddComponent<RawImage>();
            player.AddComponent<ClipPlayerUI>();
            GameObjectUtility.SetParentAndAlign(player, parent);
        
            return player;
        }
        #endif

        internal static GameObject CreateCanvas(GameObject parent = null)
        {
            GameObject canvas = new GameObject(CanvasName);
        
            // Add required components of a canvas
            var canvasComp = canvas.AddComponent<Canvas>();
            canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            GameObjectUtility.SetParentAndAlign(canvas, parent);

            // Create an EventSystem object if needed.
            CreateEventSystem();

            return canvas;
        }

        internal static GameObject CreateEventSystem(GameObject parent = null)
        {
            // Only one EventSystem should exist.
            var existingES = GameObject.FindObjectOfType<EventSystem>();
            if (existingES != null)
                return existingES.gameObject;

            GameObject eventSystem = new GameObject(EventSystemName);
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            GameObjectUtility.SetParentAndAlign(eventSystem, parent);

            return eventSystem;
        }

        /// <summary>
        /// Calculates the index of the game object to avoid duplicate names similar to what Unity does.
        /// </summary>
        /// <returns>The game object index, 0 means the object can use the base name without index.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="baseName">Base name.</param>
        internal static int CalculateGameObjectIndex(GameObject parent, string baseName)
        {
            string pattern = "^" + baseName + " \\([0-9]+\\)$"; // baseName with index in brackets, e.g. baseName (1)
            List<GameObject> objects;

            if (parent == null)
            {
                // Get all root objects.
                Scene scene = EditorSceneManager.GetActiveScene();
                objects = new List<GameObject>(scene.rootCount);
                scene.GetRootGameObjects(objects);
            }
            else
            {
                // Get all childs of parent.
                objects = new List<GameObject>(parent.transform.childCount);
                foreach (Transform childTf in parent.transform)
                    objects.Add(childTf.gameObject);
            }

            bool foundIndexZeroObj = false;
            int indexedObjectsCount = 0;

            foreach (var obj in objects)
            {
                if (obj.name.Equals(baseName))
                    foundIndexZeroObj = true;
                else if (Regex.IsMatch(obj.name, pattern))
                    indexedObjectsCount++;
            }

            if (!foundIndexZeroObj)
                return 0;
            else
                return indexedObjectsCount + 1;
        }

    }
}
