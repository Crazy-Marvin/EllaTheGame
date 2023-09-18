using MonKey.Commands;
using MonKey.Editor.Internal;
using MonKey.Extensions;
using MonKey.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameObject = UnityEngine.GameObject;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace MonKey.Editor.Commands
{
    public static class CreationUtilities
    {

        [Command("New Siblings",
             Help = "Creates new siblings for the selected game objects",
            QuickName = "NS", DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            Category = "GameObject")]
        public static void CreateNewSibling(
            [CommandParameter(Help="The amount of siblings to create")]
            int siblingAmount = 1)
        {
            if (!Selection.activeTransform)
                return;

            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Create Siblings");
            int group = Undo.GetCurrentGroup();

            foreach (var gameObject in Selection.gameObjects)
            {
                for (int i = 0; i < siblingAmount; i++)
                {
                    GameObject go = new GameObject(gameObject.name);
                    go.transform.SetParent(gameObject.transform.parent);
                    Undo.RegisterCreatedObjectUndo(go, "sibling created");
                    go.transform.position = gameObject.transform.position;
                }
            }

            Undo.CollapseUndoOperations(group);
        }

        [Command("New Children",
            Help = "Creates new children for the selected game objects",
            QuickName = "NC", DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            Category = "GameObject")]
        public static void CreateNewChildren(
            [CommandParameter(Help="The amount of children to create")]
            int amount = 1)
        {
            if (!Selection.activeTransform)
                return;

            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Create Children");
            int group = Undo.GetCurrentGroup();

            List<Object> childrenCreated = new List<Object>();
            foreach (var gameObject in Selection.gameObjects)
            {
                for (int i = 0; i < amount; i++)
                {
                    GameObject go = new GameObject(gameObject.name);
                    go.transform.SetParent(gameObject.transform);
                    Undo.RegisterCreatedObjectUndo(go, "Children created");
                    go.transform.position = gameObject.transform.position;
                    childrenCreated.Add(go);
                }
            }

            Selection.objects = childrenCreated.ToArray();

            Undo.CollapseUndoOperations(group);
        }


        [Command("New Parent", Help = "Creates a parent object for the selection",
             AlwaysShow = true, QuickName = "NP", Order = -6,
             DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            MenuItemLink = "CreateParentForSelection",
            Category = "GameObject")]
        public static void ParentSelection(
            [CommandParameter("The name of the new parent object")]
            string parentName="New Parent", bool centerParent = false)
        {
            if (Selection.gameObjects.Length == 0)
                return;

            int group = MonkeyEditorUtils.CreateUndoGroup("Create Parent");

            Vector3 center = Vector3.zero;
            int i = 0;

            GameObject[] selected =
                Selection.gameObjects.Where(_ => _.scene.IsValid() && _.hideFlags == HideFlags.None)
                    .ToArray();

            foreach (GameObject o in selected)
            {
                Undo.RecordObject(o.transform, "Recording transform");
                center += o.transform.position;
                i++;
            }

            center /= i;

            GameObject parentObject = new GameObject(parentName);
            SceneManager.MoveGameObjectToScene(parentObject, Selection.gameObjects[0].scene);
            Undo.RegisterCreatedObjectUndo(parentObject, "Parent Object Creation");

            foreach (GameObject obj in selected)
            {
                if (obj.GetComponent<RectTransform>())
                {
                    parentObject.AddComponent<RectTransform>();
                    break;
                }
            }

            Transform common = ParentingUtilities.FindEarliestCommonParent(selected);
            Transform commonParent = common != null &&
                                     selected.Contains(common.gameObject) ? common.parent : common;
            int siblingIndex = 0;

            if (common == null)
            {
                GameObject first = Selection.gameObjects[0];
                if (!MonkeyEditorUtils.OrderedSelectedGameObjects.Any())
                    first = MonkeyEditorUtils.OrderedSelectedGameObjects.First();

                int j = 0;
                foreach (GameObject o in first.scene.GetRootGameObjects())
                {
                    if (selected.Contains(o))
                    {
                        siblingIndex = j;
                        break;
                    }

                    j++;
                }
            }
            else if (selected.Contains(common.gameObject))
                siblingIndex = common.GetSiblingIndex();

            if (centerParent)
                parentObject.transform.SetParent(commonParent);
            else
                parentObject.transform.SetParent(commonParent, false);
            parentObject.transform.SetSiblingIndex(siblingIndex);

            if (centerParent)
                parentObject.transform.position = center;

            List<int> siblingIndexes = new List<int>();

            foreach (GameObject obj in selected)
            {
                siblingIndexes.Add(obj.transform.GetSiblingIndex());
                Undo.SetTransformParent(obj.transform, parentObject.transform, "Re - Parenting");
            }

            for (var index = 0; index < selected.Length; index++)
            {
                GameObject obj = selected[index];
                obj.transform.SetSiblingIndex(siblingIndexes[index]);
            }

            Selection.objects = new Object[] { parentObject };

            Selection.activeObject = parentObject;

            parentObject.transform.SetSiblingIndex(siblingIndexes.OrderBy(_ => _).First());

            Undo.CollapseUndoOperations(group);
        }

        [Command("New Parent For Each", "Creates a new parent for each of the selected objects",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, QuickName = "NPE",
            MenuItemLink = "NewParentForEach", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "GameObject")]
        public static void CreateParentForEach()
        {
            int undoId = MonkeyEditorUtils.CreateUndoGroup("Parent For Each");
            List<GameObject> createdParents = new List<GameObject>();
            foreach (var gameObject in Selection.gameObjects.Where(_ => _.scene.IsValid()))
            {
                GameObject go = new GameObject(gameObject.name + " Parent");
                createdParents.Add(go);
                go.transform.SetParent(gameObject.transform.parent);
                go.transform.Reset();

                Undo.RegisterCreatedObjectUndo(go, "created new parent");
                Undo.SetTransformParent(gameObject.transform, go.transform, "ReParenting");
                go.transform.CopyLocal(gameObject.transform);
                Undo.RecordObject(gameObject.transform, "Resetting transform");
                gameObject.transform.Reset();
            }

            Selection.objects = createdParents.Convert(_ => (Object)_).ToArray();
            Undo.CollapseUndoOperations(undoId);
        }

        [Command("New Instances",
            "Instantiates the specified prefabs under the selected objects " +
            "(by default one of each prefab selected)", QuickName = "NI", MenuItemLink = "NewInstance",
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "GameObject")]
        public static void InstantiateAsChildren(
            [CommandParameter(ForceAutoCompleteUsage = true,PreventDefaultValueUsage = true,
                DefaultValueNameOverride = "No Prefab Selected",
                Help = "the prefabs you want to instantiate",
                AutoCompleteMethodName = "PrefabAssets")]
            string[] prefabs,
            [CommandParameter(Help = "The amount of instances of each prefabs to create")]
            int amount=1)
        {

            GameObject[] parents = Selection.gameObjects.Where(_ => _.scene.IsValid()).ToArray();

            int undoId = MonkeyEditorUtils.CreateUndoGroup("Instantiate Prefabs");

            if (parents.Length == 0)
            {
#if UNITY_2019
                var stage = PrefabStageUtility.GetCurrentPrefabStage();
                if (stage != null)
                {
                    parents = new[] { stage.prefabContentsRoot };
                }
                else
                {
                    parents = new GameObject[] { null };
                }
#else
                parents = new GameObject[] { null };
#endif
            }
            if (prefabs == null || prefabs.Length == 0)
            {
                Debug.LogWarning("Monkey Warning: the prefab could not be " +
                                 "instantiated because none were selected or could be recognized");
                return;
            }

            List<Object> createdObjects = new List<Object>();
            int j = 0;
            int k = 0;
            Object[] selected = prefabs.Convert(_ => AssetDatabase.LoadAssetAtPath<Object>(_)).ToArray();
            foreach (Object o in selected)
            {
                foreach (var parent in parents)
                {
                    if (o != parent && PrefabUtility.GetPrefabAssetType(o) !=PrefabAssetType.NotAPrefab  ||
                        PrefabUtility.GetPrefabInstanceStatus(o)!= PrefabInstanceStatus.NotAPrefab)
                    {
                        for (int i = 0; i < amount; i++)
                        {
                            if (EditorUtility.DisplayCancelableProgressBar("Instantiating Prefabs",
                                "Instantiating " + o.name,
                                ((float)i + i * j + i * j * k)
                                / (amount * parents.Length * selected.Count())))
                                break;

                            Object newObject = PrefabUtility.InstantiatePrefab(o,
                                (!parent) ? SceneManager.GetActiveScene() : parent.scene);
                            Undo.RegisterCreatedObjectUndo(newObject, "prefab creation");
                            createdObjects.Add(newObject);
                            if (parent)
                            {
                                GameObject obj = newObject as GameObject;
                                if (obj != null)
                                {
                                    Undo.SetTransformParent(obj.transform,
                                        parent.transform, "re-parenting");
                                    obj.transform.Reset();
                                }
                            }
                        }
                    }

                    k++;
                }

                j++;
            }

            EditorUtility.ClearProgressBar();
            Selection.objects = createdObjects.ToArray();
            Undo.CollapseUndoOperations(undoId);
        }

        public static AssetNameAutoComplete PrefabAssets()
        {
            return new AssetNameAutoComplete() { IncludeDirectories = false, CustomType = "GameObject" };
        }

        [Command("New Scriptable Object",
            "Creates a new ScriptableObject of the type specified in the first folder selected",
            AlwaysShow = true, QuickName = "NSO", MenuItemLink = "NewScriptableObject", Order = -5,
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Assets")]
        public static void CreateNewScriptableObject(
            [CommandParameter(Help="The type of the ScriptableObject to create",
                ForceAutoCompleteUsage = true, PreventDefaultValueUsage = true,
                AutoCompleteMethodName = "ScriptableObjectAutoComplete")]
            Type type)
        {
            ScriptableObject obj = ScriptableObject.CreateInstance(type);

            NameAndCreateNewAsset(obj, "New" + type.Name, type.Name);

        }


        [Command("Attach New Material",
            "Creates and attaches a new material in the selected folder to the selected objects",
        ValidationMethodName = "ObjectsWithRenderer", QuickName = "AM",
            Category = "Assets")]
        public static void CreateAndAttachMaterial(
            [CommandParameter(Help = "the name of the shader to use for the new material",
                AutoCompleteMethodName ="ShaderAutoComplete",ForceAutoCompleteUsage = true,
                DefaultValueNameOverride = "Standard Shader")]
            string shaderName="")
        {
            int undoGroup = MonkeyEditorUtils.CreateUndoGroup("New Material");

            Shader shader = (shaderName.IsNullOrEmpty()) ? Shader.Find("Standard") :
                AssetDatabase.LoadAssetAtPath<Shader>(shaderName);
            Material mat = new Material(shader);

            Object obj = (Object)mat;

            IEnumerable<GameObject> gosWithRenderer =
                Selection.gameObjects.Where(_ => _.GetComponentInChildren<Renderer>() != null);

            if (!NameAndCreateNewAsset(obj, "New Material", "Material")) return;

            foreach (GameObject o in gosWithRenderer)
            {
                Renderer rend = o.GetComponentInChildren<Renderer>();
                Undo.RecordObject(rend, "Setting Material");
                rend.sharedMaterial = mat;
            }

            Undo.CollapseUndoOperations(undoGroup);
        }

        [Command("Replace Material", "Replaces the first material on all the meshrenderers of the objects selected",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT, QuickName = "RM",
            Category = "Assets")]
        public static void ReplaceMaterial(
            [CommandParameter("The material to replace the old one with",OverrideTypeName = "Material",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true,AutoCompleteMethodName = "MaterialAutoComplete")]
            string materialReplacement)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(materialReplacement);
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Replace Material");
            foreach (MeshRenderer renderer in
                Selection.gameObjects.Convert(_ => _.GetComponentInChildren<MeshRenderer>()))
            {
                Undo.RecordObject(renderer, "change of material");
                if (renderer)
                    renderer.sharedMaterial = mat;
            }
            Undo.CollapseUndoOperations(undoID);
        }

        public static GenericCommandParameterAutoComplete MaterialAutoComplete()
        {
            return new AssetNameAutoComplete() { CustomType = "Material" };
        }


        public static bool NameAndCreateNewAsset(Object obj, string name, string type)
        {
            obj.name = name;

            string savePath;
            if (!Selection.objects.Any(_ => _.IsDirectory()))
            {
                savePath = MonkeyEditorUtils.GetProjectWindowFocusedFolder();
                if (savePath.IsNullOrEmpty())
                {
                    Debug.Log("Monkey Create New " + type + ":" +
                              "No Folder Selected, manual folder selection mode");
                    savePath = EditorUtility.SaveFilePanelInProject(type + " Creation",
                        "New " + type, "", "Select the object's folder", "Assets");
                }
            }
            else
                savePath = AssetDatabase.GetAssetPath(Selection.objects.First(_ => _.IsDirectory()));

            if (savePath.Length <= 0)
                return false;

            savePath += "/" + obj.name + ".asset";
            EditorUtility.DisplayProgressBar("Object creation", "In Progress!", 1);
            savePath = AssetDatabase.GenerateUniqueAssetPath(savePath);
            AssetDatabase.CreateAsset(obj, savePath);
            EditorUtility.ClearProgressBar();
            Selection.objects = new Object[] { obj };
            EditorGUIUtility.PingObject(obj);
            return true;
        }

        [CommandValidation("Select some GameObjects with renderers")]
        private static bool ObjectsWithRenderer()
        {
            return Selection.gameObjects.Any(_ => _.GetComponentInChildren<Renderer>());
        }

        private static AssetNameAutoComplete ShaderAutoComplete()
        {
            return new AssetNameAutoComplete() { IncludeDirectories = false, CustomType = "Shader" };
        }

        [Command("New Scene",
            "Creates a new scene in the first selected folder, or the focused one in the project window",
            MenuItemLink = "NewScene", MenuItemLinkTypeOwner = "MonkeyMenuItems", QuickName = "NSC",
            Category = "Scene")]
        public static void CreateNewScene()
        {
            ProjectWindowUtil.CreateScene();
        }

        [Command("New Folder",
            "Creates a new folder in the first selected folder, or the focused one in the project window",
            QuickName = "NF",
            Category = "Assets")]
        public static void CreateNewFolder()
        {
            ProjectWindowUtil.CreateFolder();
        }

        [Command("Add Components",
            "Creates new components on the selected GameObjects, with the types specified",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            AlwaysShow = true, QuickName = "AC", MenuItemLink = "AddComponents",
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "GameObject/Component")]
        public static void AddComponents(
            [CommandParameter(Help = "The types of components to add",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true,
                AutoCompleteMethodName = "ComponentAutoComplete")]
            Type[] componentTypes)
        {
            if (componentTypes == null || componentTypes.Length == 0)
            {
                Debug.LogWarning("Monkey Warning: Error parsing the component types");
                return;
            }

            int undoID = MonkeyEditorUtils.CreateUndoGroup("Adding Components");

            foreach (var gameObject in Selection.gameObjects)
            {
                foreach (var type in componentTypes)
                {
                    if (type == null)
                        continue;

                    Component comp = gameObject.AddComponent(type);
                    if (comp)
                        Undo.RegisterCreatedObjectUndo(comp, "component attached");
                }
            }
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Replace Components",
            "Replaces the first component of the type specified " +
            "by another one of another type on all objects",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            AlwaysShow = true, QuickName = "RC",
            MenuItemLink = "ReplaceComponents", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "GameObject/Component")]
        public static void ReplaceComponents(
            [CommandParameter(Help = "The type of component to replace",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true,
                AutoCompleteMethodName = "TypeAuto")]
            Type componentType,
            [CommandParameter(Help = "The types of components to add",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true,
                AutoCompleteMethodName = "ComponentAutoComplete")]
            Type newType)
        {
            if (componentType == null || newType == null)
            {
                Debug.LogWarning("Monkey Warning: Error parsing the component types");
                return;
            }

            int undoID = MonkeyEditorUtils.CreateUndoGroup("Replacing Components");

            foreach (var gameObject in Selection.gameObjects)
            {

                Component oldcomp = gameObject.GetComponent(componentType);
                if (oldcomp)
                    Undo.DestroyObjectImmediate(oldcomp);
                Component comp = gameObject.AddComponent(newType);
                if (comp)
                    Undo.RegisterCreatedObjectUndo(comp, "component attached");

            }
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Remove Component",
            "Removes the first components of the type specified " +
            "on all objects",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            QuickName = "REC",
            Category = "GameObject/Component")]
        public static void RemoveComponents(
            [CommandParameter(Help = "The type of component to remove",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true,
                AutoCompleteMethodName = "TypeAuto")]
            Type componentType
           )
        {
            if (componentType == null)
            {
                Debug.LogWarning("Monkey Warning: Error parsing the component types");
                return;
            }

            int undoID = MonkeyEditorUtils.CreateUndoGroup("Removing Components");

            foreach (var gameObject in Selection.gameObjects)
            {
                Component[] oldcomps = gameObject.GetComponents(componentType);

                foreach (var oldcomp in oldcomps)
                {
                    if (oldcomp)
                        Undo.DestroyObjectImmediate(oldcomp);
                }

            }
            Undo.CollapseUndoOperations(undoID);
        }


        public static TypeAutoComplete ScriptableObjectAutoComplete()
        {
            return new TypeAutoComplete(false, false, false);
        }

        public static TypeAutoComplete ObjectAutoComplete()
        {
            return new TypeAutoComplete(true, false, false, true, false);
        }


        public static TypeAutoComplete ComponentAutoComplete()
        {
            return new TypeAutoComplete(false, true, true, false);
        }

        [Command("Duplicate", "Duplicates the selected objects, by default once",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT,
            MenuItemLink = "Duplicate", MenuItemLinkTypeOwner = "MonkeyMenuItems", QuickName = "D",
            Category = "GameObject")]
        public static void Duplicate(
            [CommandParameter(Help = "The amount of duplicates that will be created")]
            int amount=1)
        {
            List<Object> createdObjects = new List<Object>();
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Duplicate");
            foreach (GameObject toDup in Selection.gameObjects)
            {
                for (int i = 0; i < amount; i++)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Duplicating Objects",
                        "Duplicating " + toDup.name, (float)i / amount))
                    {
                        Undo.CollapseUndoOperations(undoID);
                        Selection.objects = createdObjects.ToArray();
                        EditorUtility.ClearProgressBar();
                        return;
                    }

                    Object newObj;
                    GameObject go = null;
#if UNITY_2019

                    if (PrefabUtility.IsPartOfPrefabInstance(toDup))
                    {
                        GameObject prefabParent = (GameObject)
                            PrefabUtilities.GetPrefabParentPlatformIndependant(toDup);

                        newObj = PrefabUtility.
                            InstantiatePrefab(prefabParent,
                                toDup.scene);
                        go = newObj as GameObject;

                    }
                    else if (/*!PrefabUtility.IsPartOfPrefabAsset(toDup) || */!toDup.scene.IsValid())
                    {
                        continue;
                    }
                    else
                    {

                        newObj = Object.Instantiate(toDup);
                        go = newObj as GameObject;

                    }

                    if (toDup.scene.IsValid())
#else

                    if (PrefabUtility.GetPrefabType(toDup) == PrefabType.PrefabInstance)
                    {
                        GameObject prefabParent = (GameObject)
                            PrefabUtilities.GetPrefabParentPlatformIndependant(toDup);

                        newObj = PrefabUtility.
                            InstantiatePrefab(prefabParent,
                                toDup.scene);
                        go = newObj as GameObject;
                    }
                    else if (!toDup.scene.IsValid())
                    {
                        continue;
                    }
                    else
                    {
#if UNITY_2017_1_OR_NEWER
                        newObj = Object.Instantiate(toDup, toDup.transform.parent);
                        go = newObj as GameObject;
#else
                        newObj = Object.Instantiate(toDup);
                      go = newObj as GameObject;
#endif
                    }
#endif
                        if (go)
                        {
                            go.transform.SetSiblingIndex(toDup.transform.GetSiblingIndex() + 1 + i);
                            go.transform.SetParent(toDup.transform.parent);
                            go.transform.CopyLocalPositionRotation(toDup.transform);
                            go.transform.localScale = toDup.transform.localScale;
                        }

                    createdObjects.Add(newObj);

                    Undo.RegisterCreatedObjectUndo(newObj, "new Duplicate");
                    EditorApplication.RepaintHierarchyWindow();
                }
            }

            foreach (Object o in Selection.objects)
            {
                GameObject go = o as GameObject;
#if UNITY_2019
                if (go && !PrefabUtility.IsPartOfPrefabAsset(go))
                    continue;
#else
                if (go && Selection.transforms.Contains(go.transform))
                    continue;
#endif
                for (int i = 0; i < amount; i++)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Duplicating Objects",
                        "Duplicating " + o.name, (float)i / amount))
                    {
                        Undo.CollapseUndoOperations(undoID);
                        Selection.objects = createdObjects.ToArray();
                        EditorUtility.ClearProgressBar();
                        return;
                    }

                    string path = AssetDatabase.GetAssetPath(o);
                    string newPath = AssetDatabase.GenerateUniqueAssetPath(path);
                    AssetDatabase.CopyAsset(path, newPath);
                    Object newlyCreated = AssetDatabase.LoadAssetAtPath(newPath, typeof(Object));
                    //Undo.RegisterCreatedObjectUndo(newlyCreated, "new Duplicate");
                    createdObjects.Add(newlyCreated);
                }
            }
            Undo.CollapseUndoOperations(undoID);
            Selection.objects = createdObjects.Append(Selection.gameObjects).ToArray();
            EditorUtility.ClearProgressBar();
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.DirtyHierarchyWindowSorting();
            EditorApplication.RepaintProjectWindow();

        }

        private static GameObject FirstSelected()
        {
            return MonkeyEditorUtils.OrderedSelectedGameObjects.FirstOrDefault();
        }

        [Command("Duplicate And Mirror", "Duplicates an objects following a point symmetry"
            , QuickName = "DMP",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            Category = "GameObject")]
        public static void DuplicateMirrorPoint()
        {
            MonkeyEditorUtils.AddSceneCommand(new MirrorCreationCommand(Selection.activeGameObject));
        }

        public class MirrorCreationCommand : ConfirmedCommand
        {
            public GameObject MirrorCenter;
            public GameObject ObjectToMirror;

            public bool UseRotation;

            public MirrorCreationCommand(GameObject toMirror)
            {
                SceneCommandName = "Mirror Creation";
                ObjectToMirror = toMirror;
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayObjectOption("Mirror Center", ref MirrorCenter);
                DisplayBoolOption("Use Rotation", ref UseRotation);
                DisplayObjectOption("Object To Mirror", ref ObjectToMirror);
                DisplayButton("Create Mirror", CreateNewMirror);
            }

            public void CreateNewMirror()
            {
                var go = ObjectToMirror.InstantiateObjectAsInstance();

                go.transform.SetParent(ObjectToMirror.transform.parent);
                go.transform.SetSiblingIndex(ObjectToMirror.transform.GetSiblingIndex() + 1);

                Vector3 direction = MirrorCenter.transform.position - ObjectToMirror.transform.position;

                go.transform.position = MirrorCenter.transform.position + direction;

                if (UseRotation)
                {
                    go.transform.rotation = Quaternion.Inverse(go.transform.rotation);
                }
            }

        }

        [Command("New Instances Under Mouse", QuickName = "NIM", Order = -6,
            Help = "Instantiates prefabs interactively at the mouse position, " +
                   "randomly from a list of selected prefabs", AlwaysShow = true,
            MenuItemLink = "InstantiateMousePrefab",
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "GameObject")]
        public static void InstantiatePrefabUnderMouse(
            [CommandParameter(Help="The list of prefabs you want to instantiate," +
                                   " they will be randomly chosen",
                AutoCompleteMethodName = "PrefabAssets",
                ForceAutoCompleteUsage = true,DefaultValueMethod = "DefaultAssets",
                DefaultValueNameOverride = "The Selected Prefabs",
                OverrideTypeName = "Prefab Names")]
            string[] prefabs)

        {
            List<GameObject> prefabsObjects = new List<GameObject>();
            foreach (string prefab in prefabs)
            {
                prefabsObjects.Add(AssetDatabase.LoadAssetAtPath<GameObject>(prefab));
            }
            MonkeyEditorUtils.AddSceneCommand(
                new RaycastCreationSceneCommand(prefabsObjects, 0, DirectionAxis.UP,
                    Vector2.zero, true));
        }


        [Command("New Instances Under Mouse 2D", QuickName = "NIM2", Order = -6,
            Help = "Instantiates prefabs interactively at the mouse position in 2D, " +
                   "randomly from a list of selected prefabs", AlwaysShow = true,
            Category = "2D/Creation")]
        public static void InstantiatePrefabUnderMouse2D(
            [CommandParameter(Help="The list of prefabs you want to instantiate," +
                                   " they will be randomly chosen",
                AutoCompleteMethodName = "PrefabAssets",
                ForceAutoCompleteUsage = true,DefaultValueMethod = "DefaultAssets",
                DefaultValueNameOverride = "The Selected Prefabs",
                OverrideTypeName = "Prefab Names")]
            string[] prefabs)

        {
            List<GameObject> prefabsObjects = new List<GameObject>();
            foreach (string prefab in prefabs)
            {
                prefabsObjects.Add(AssetDatabase.LoadAssetAtPath<GameObject>(prefab));
            }
            MonkeyEditorUtils.AddSceneCommand(new Raycast2DCreationSceneCommand(prefabsObjects, Vector2.one, true));
        }


        [Command("Duplicate Under Mouse", QuickName = "DM", AlwaysShow = true, Order = -6,
            Help = "Duplicates the selected objects randomly under the raycasted position of the mouse " +
                   "(new instance for prefabs)",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            MenuItemLink = "DuplicateMouseRay", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "GameObject")]
        public static void DuplicateObjectUnderMouse()

        {
            MonkeyEditorUtils.AddSceneCommand(
                new RaycastCreationSceneCommand(Selection.gameObjects.ToList(), 0, DirectionAxis.UP,
                    Vector2.zero, true));
        }

        [Command("Duplicate Under Mouse 2D", QuickName = "DM2", AlwaysShow = true, Order = -6,
            Help = "Duplicates the selected objects randomly under the 2D position of the mouse " +
                   "(new instance for prefabs)",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            Category = "2D/Creation")]
        public static void DuplicateObjectUnderMouse2D()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new Raycast2DCreationSceneCommand(Selection.gameObjects.ToList(), Vector2.zero, true));
        }

        [Command("New GameObjects Under Mouse", QuickName = "NG",
            Help = "Creates new objects under the raycasted position of the mouse",
            Category = "GameObject")]
        public static void CreateObjectUnderMouse()

        {
            MonkeyEditorUtils.AddSceneCommand(
                new RaycastCreationSceneCommand(null, 0, DirectionAxis.UP,
                Vector2.zero, false));
        }


        [Command("New GameObjects Under Mouse 2D", QuickName = "NG2",
            Help = "Creates new objects under the 2D position of the mouse",
            Category = "2D/Creation")]
        public static void CreateObjectUnderMouse2D()
        {
            MonkeyEditorUtils.AddSceneCommand(
                new Raycast2DCreationSceneCommand(null, Vector2.one, false));
        }

        public class RaycastCreationSceneCommand : ConfirmedCommand
        {
            public GameObject ParentObject;

            public float Offset = 0;

            private Vector3 lastProjectionPosition;
            private Vector3 lastProjectionNormal;

            private readonly List<GameObject> prefabs;
            private Vector2 scaleRandomRange;

            private List<GameObject> createdObjects = new List<GameObject>();
            private bool collision;

            private DirectionAxis axisToNormal;
            private DirectionAxis forwardAxis;

            private bool followCreationCurve;

            private Vector3? lastPositionCreated = null;

            private GameObject debugDrawObject;
            private GameObject lastCreated;

            public float angleOffset;

            public Vector2 RandomRotationRange;

            private bool IgnoreInvisibleColliders;

            public RaycastCreationSceneCommand(List<GameObject> prefabs, float offset,
                DirectionAxis axisToNormal, Vector2 scaleRandomRange, bool followCreationCurve)
            {
                ConfirmationMode = ActionConfirmationMode.ESCAPE;
                SceneCommandName = "RayCast Mouse Creation";
                IgnoreInvisibleColliders = true;
                forwardAxis = DirectionAxis.FORWARD;
                scaleRandomRange = Vector2.one;
                this.prefabs = prefabs;
                Offset = offset;
                this.axisToNormal = axisToNormal;
                this.scaleRandomRange = scaleRandomRange;
                RandomRotationRange = Vector2.zero;
            }

            public override string ConfirmationMessage()
            {
                return "Press SPACE or Click to instantiate, ESCAPE to stop";
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayFloatOption("Surface Offset", ref Offset);
                DisplayFloatOption("Angle Offset", ref angleOffset);
                Enum axis = axisToNormal;
                DisplayEnumOption("Up Axis", ref axis);
                axisToNormal = (DirectionAxis)axis;
                axis = forwardAxis;
                DisplayEnumOption("Forward Axis", ref axis);
                forwardAxis = (DirectionAxis)axis;

                DisplayBoolOption("Orient Along Spawn", ref followCreationCurve);
                DisplayVector2Option("Random Rotation Range", ref RandomRotationRange);

                DisplayBoolOption("Ignore Invisible Colliders", ref IgnoreInvisibleColliders);
                DisplayVector2Option("Scale Range", ref scaleRandomRange);
                DisplayObjectOption("Parent Object", ref ParentObject);
                if (prefabs != null)
                    DisplayObjectListOption("Objects To Instantiate", prefabs);
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();

                if (stopping)
                    return;

                if (!lastCreated ||
                    Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space ||
                    Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    Event.current.Use();

                    if (lastCreated && RandomRotationRange != Vector2.zero)
                        lastCreated.transform.rotation =
                            Quaternion.AngleAxis(Random.Range(RandomRotationRange.x, RandomRotationRange.y),
                                lastProjectionNormal) * lastCreated.transform.rotation;

                    UpdateCreatedObject();
                    Selection.objects = new Object[0];

                    if (lastCreated)
                    {
                        lastCreated.SetActive(false);
                        Undo.RegisterCreatedObjectUndo(lastCreated, "Mouse Raycast Object Creation");
                    }
                }

                if (lastCreated)
                    lastCreated.transform.
                            AlignTransformToCollision(lastProjectionPosition, lastProjectionNormal, true, axisToNormal, forwardAxis);

                if (followCreationCurve && lastCreated)
                {
                    if (lastPositionCreated != null)
                    {
                        Quaternion rotation = Quaternion.LookRotation(
                            (Vector3)(lastPositionCreated - lastCreated.transform.position),
                            lastCreated.transform.AxisToVector(axisToNormal, true));
                        lastCreated.transform.rotation = lastCreated.transform.rotation * rotation;
                    }

                    lastPositionCreated = lastCreated.transform.position;

                }

                if (lastCreated)
                    lastCreated.transform.rotation =
                        Quaternion.AngleAxis(angleOffset,
                            lastProjectionNormal) * lastCreated.transform.rotation;

                if (collision)
                {

#if UNITY_2017_1_OR_NEWER
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
#endif
                    Handles.Label(lastProjectionPosition + lastProjectionNormal, "MonKey Creation",
                        MonkeyStyle.Instance.CommandNameStyle);

#if UNITY_2017_1_OR_NEWER
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
#endif
                    Handles.color = MonkeyStyle.Instance.WindowColor.Alphaed(.1f);
                    Handles.DrawSolidDisc(lastProjectionPosition + lastProjectionNormal * 0.05f,
                        lastProjectionNormal, .5f);
                    Handles.color = MonkeyStyle.Instance.WindowColor.Alphaed(.9f);
                    Handles.DrawSolidDisc(lastProjectionPosition + lastProjectionNormal * 0.05f,
                        lastProjectionNormal, .1f);
                    Handles.DrawLine(lastProjectionPosition,
                        lastProjectionPosition + lastProjectionNormal * 1);

                    Handles.color = MonkeyStyle.Instance.WindowColor.Alphaed(.5f).Inverted();
#if UNITY_2017_1_OR_NEWER
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
#endif
                    Handles.DrawWireDisc(lastProjectionPosition + lastProjectionNormal * 0.05f,
                        lastProjectionNormal, .5f);
                    Handles.DrawDottedLine(lastProjectionPosition,
                        lastProjectionPosition + lastProjectionNormal * 1, 1);

                    Handles.color = Color.white;

                }
            }

            private GameObject UpdateCreatedObject()
            {
                if (lastCreated)
                {
                    lastCreated.SetActive(true);
                    if (debugDrawObject)
                        Object.DestroyImmediate(debugDrawObject);
                    createdObjects.Add(lastCreated);
                    if (ParentObject)
                    {
                        lastCreated.transform.SetParent(ParentObject.transform, true);
                    }
                    if (createdObjects.Count > 0)
                        createdObjects = createdObjects.Where(_ => _ != null).ToList();
                }

                GameObject go;
                if (prefabs == null || prefabs.Count == 0)
                    go = new GameObject("Mouse Raycast Object " + createdObjects.Count);
                else
                {
                    GameObject pref = prefabs.GetRandom();
                    if (!pref.IsPrefab())
                    {
                        go = pref.InstantiateObjectAsInstance();

                        if (pref.scene.IsValid())
                        {
                            go.transform.SetParent(pref.transform.parent);
                            go.transform.SetSiblingIndex(pref.transform.GetSiblingIndex() + 1);
                        }
                    }
                    else
                    {
                        go = (GameObject)
                            PrefabUtility.InstantiatePrefab(pref, SceneManager.GetActiveScene());
                    }
                }
                debugDrawObject = new GameObject("MonKey Debug Draw");
                MonKeyDebugMeshDrawer drawer = debugDrawObject.AddComponent<MonKeyDebugMeshDrawer>();
                drawer.reference = go;
                drawer.MeshColor = Color.white.Alphaed(.6f);
                drawer.OutlineColor = MonkeyStyle.Instance.WarningColor.Alphaed(.9f);

                lastCreated = go;
                lastCreated.transform.localScale = go.transform.localScale * Random.Range(scaleRandomRange.x, scaleRandomRange.y);


                return go;
            }

            public override void Update()
            {
                base.Update();

                lastProjectionPosition = MonkeyEditorUtils.GetMouseRayCastedPosition(null, Offset,
                    out collision, out lastProjectionNormal, IgnoreInvisibleColliders);

            }

            private bool stopping = false;

            public override void Stop()
            {
                if (lastCreated)
                {
                    Object.DestroyImmediate(lastCreated);
                }

                if (debugDrawObject)
                {
                    Object.DestroyImmediate(debugDrawObject);
                }

                base.Stop();
                Selection.objects = createdObjects.ToArray();
                stopping = true;
            }
        }

        public class Raycast2DCreationSceneCommand : ConfirmedCommand
        {

            private Vector2 lastProjectionPosition;
            private Vector2 lastProjectionNormal;

            private readonly List<GameObject> prefabs;
            private Vector2 scaleRandomRange;

            private List<GameObject> createdObjects = new List<GameObject>();
            private bool collision;

            private DirectionAxis axisToNormal;

            private bool followCreationCurve;

            private Vector2? lastPositionCreated = null;

            public Raycast2DCreationSceneCommand(List<GameObject> prefabs, Vector2 scaleRandomRange, bool followCreationCurve)
            {
                ConfirmationMode = ActionConfirmationMode.ESCAPE;
                SceneCommandName = "RayCast Mouse Creation";

                this.prefabs = prefabs;
                this.scaleRandomRange = scaleRandomRange;
            }

            public override string ConfirmationMessage()
            {
                return "Press SPACE to Instantiate a new object, ESCAPE to stop";
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();
                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
                {
                    Event.current.Use();
                    GameObject go;
                    if (prefabs == null || prefabs.Count == 0)
                        go = new GameObject("Mouse Raycast Object " + createdObjects.Count);
                    else
                    {
                        GameObject pref = prefabs.GetRandom();
                        if (!pref.IsPrefab())
                        {
                            go = pref.InstantiateObjectAsInstance();

                            if (pref.scene.IsValid())
                            {
                                go.transform.SetParent(pref.transform.parent);
                                go.transform.SetSiblingIndex(pref.transform.GetSiblingIndex() + 1);
                            }
                        }
                        else
                        {
                            go = (GameObject)
                                PrefabUtility.InstantiatePrefab(pref, SceneManager.GetActiveScene());
                        }
                    }

                    createdObjects.Add(go);
                    go.transform.
                        AlignTransformToCollision(lastProjectionPosition, lastProjectionNormal, true, axisToNormal);


                    if (followCreationCurve)
                    {
                        if (lastPositionCreated != null)
                        {
                            Quaternion rotation = Quaternion.LookRotation(
                                (Vector3)(lastPositionCreated - go.transform.position),
                                go.transform.AxisToVector(axisToNormal, true));
                            go.transform.rotation = go.transform.rotation * rotation;
                        }

                        lastPositionCreated = go.transform.position;

                    }

                    if (scaleRandomRange != Vector2.zero)
                        go.transform.localScale = Vector3.one *
                                                  Random.Range(scaleRandomRange.x, scaleRandomRange.y);
                    Undo.RegisterCreatedObjectUndo(go, "Mouse Raycast Object Creation");
                }

                createdObjects = createdObjects.Where(_ => _ != null).ToList();

                if (collision)
                {
#if UNITY_2017_1_OR_NEWER
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
#endif
                    Handles.Label(lastProjectionPosition + lastProjectionNormal, "MonKey Creation",
                        MonkeyStyle.Instance.CommandNameStyle);

#if UNITY_2017_1_OR_NEWER
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
#endif
                    Handles.color = MonkeyStyle.Instance.WindowColor.Alphaed(.1f);
                    Handles.DrawSolidDisc(lastProjectionPosition + lastProjectionNormal * 0.05f,
                        lastProjectionNormal, .5f);
                    Handles.color = MonkeyStyle.Instance.WindowColor.Alphaed(.9f);
                    Handles.DrawSolidDisc(lastProjectionPosition + lastProjectionNormal * 0.05f,
                        lastProjectionNormal, .1f);
                    Handles.DrawLine(lastProjectionPosition,
                        lastProjectionPosition + lastProjectionNormal * 1);

                    Handles.color = MonkeyStyle.Instance.WindowColor.Alphaed(.5f).Inverted();
#if UNITY_2017_1_OR_NEWER
                    Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
#endif
                    Handles.DrawWireDisc(lastProjectionPosition + lastProjectionNormal * 0.05f,
                        lastProjectionNormal, .5f);
                    Handles.DrawDottedLine(lastProjectionPosition,
                        lastProjectionPosition + lastProjectionNormal * 1, 1);

                    Handles.color = Color.white;
                }
            }

            public override void Update()
            {
                base.Update();

                if (!MonkeyEditorUtils.CurrentSceneView)
                    MonkeyEditorUtils.CurrentSceneView = SceneView.currentDrawingSceneView;

                lastProjectionPosition = MonkeyEditorUtils.CurrentSceneView.
                    camera.ScreenToWorldPoint(MonkeyEditorUtils.SceneViewMousePosition);

            }

            public override void Stop()
            {
                base.Stop();
                Selection.objects = createdObjects.ToArray();
            }
        }

        [Command("Replace Selected GameObjects", "Replaces the selection with objects specified",
            QuickName = "RS",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            Category = "GameObject")]
        public static void ReplaceSelection(
            [CommandParameter(Help = "The objects that will replace the selected ones." +
                                     " If less objects than selected ones, will loop",
                ForceAutoCompleteUsage = true,PreventDefaultValueUsage = true,
                AutoCompleteMethodName = "AssetObjectsAuto")]
            string[] toReplaceWith,
            [CommandParameter(Help = "Should the scale of the replaced objects be kept")]
            bool keepScale=false,
            [CommandParameter(Help = "Should the names of the replaced objects be kept")]
            bool keepNames=true,
            [CommandParameter(Help = "Should the children of the replaced objects be kept")]
            bool keepChildren=false)
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Replace Game Objects");

            List<GameObject> gos =
                MonkeyEditorUtils.OrderedSelectedGameObjects.Where(_ => _.scene.IsValid()).ToList();
            List<GameObject> newGos = new List<GameObject>(gos.Count);

            int i = 0;
            foreach (var go in gos)
            {
                if (i >= toReplaceWith.Length)
                    i = 0;

                GameObject newGo;
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(toReplaceWith[i]);
#if UNITY_2019
                if (PrefabUtility.GetPrefabAssetType(prefab) != PrefabAssetType.NotAPrefab)
                {
                    newGo = (GameObject)PrefabUtility.InstantiatePrefab(prefab, go.scene);
                }
                else
                {
                    newGo = Object.Instantiate(prefab);
                }
#else

                if (PrefabUtility.GetPrefabType(prefab) == PrefabType.Prefab)
                {
                    newGo = (GameObject)PrefabUtility.InstantiatePrefab(prefab, go.scene);
                }
                else
                {
                    newGo = Object.Instantiate(prefab);
                }
#endif
                newGo.name = keepNames ? go.name : prefab.name;

                Undo.RegisterCreatedObjectUndo(newGo, "New Object Created");

                newGo.transform.SetParent(go.transform.parent);
                if (keepScale)
                    newGo.transform.CopyLocal(go.transform);
                else
                    newGo.transform.CopyLocalPositionRotation(go.transform);

                newGo.SetActive(go.activeSelf);

                if (keepChildren)
                {
                    foreach (Transform subTransform in go.transform)
                    {
                        Undo.SetTransformParent(subTransform, newGo.transform, "ReParent Children");
                    }
                }

                if (go)
                    Undo.DestroyObjectImmediate(go);

                i++;
            }

            Selection.objects = newGos.Convert(_ => _ as Object).ToArray();

            Undo.CollapseUndoOperations(undoID);
        }

        public static AssetNameAutoComplete AssetObjectsAuto()
        {
            return new AssetNameAutoComplete() { IncludeDirectories = false, CustomType = "GameObject" };
        }

        private static Type componentCopiedType;

        [Command("Copy Component",
            "Copies the first component of the first selected object " +
            "of the type specified to use later", QuickName = "CC",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            MenuItemLink = "CopyComponent", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "GameObject/Component")]
        public static void CopyComponent(
            [CommandParameter(Help="The type of component to copy",AutoCompleteMethodName = "TypeAuto",
                ForceAutoCompleteUsage = true)]
            Type type)
        {
            componentCopiedType = type;
            if (!MonkeyEditorUtils.OrderedSelectedGameObjects.Any())
            {
                Debug.LogWarning("Monkey Copy Component:".Bold() +
                                 " GameObjects were deselected before the method could complete");
                return;
            }

            GameObject first = MonkeyEditorUtils.OrderedSelectedGameObjects.ElementAt(0);
            Component comp = first.GetComponent(type);
            UnityEditorInternal.ComponentUtility.CopyComponent(comp);
        }

        /// <summary>
        /// creates an auto complete that returns all the component types attached to a gameobject
        /// </summary>
        /// <returns></returns>
        public static TypeAutoComplete TypeAuto()
        {
            TypeAutoComplete auto = new TypeAutoComplete(false, false, false, false);
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                foreach (var component in gameObject.GetComponents<Component>())
                {
                    if (!component)
                        continue;
                    Type type = component.GetType();

                    while (type != typeof(Component))
                    {
                        if (!auto.ContainsKey(type.Name))
                            auto.AddNewType(type.Name, type);

                        type = type.BaseType;

                        if (type == null || type == typeof(Object))
                        {
                            //never too sure
                            break;
                        }
                    }
                }
            }
            return auto;
        }

        private static CommandParameterObjectAutoComplete<Rigidbody> RigidBodyAutoComplete()
        {
            CommandParameterObjectAutoComplete<Rigidbody> autoComplete = new CommandParameterObjectAutoComplete<Rigidbody>();

            foreach (var gameObject in Selection.gameObjects)
            {
                Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                if (rb)
                {
                    autoComplete.AddValue(gameObject.name, rb);
                }
            }

            return autoComplete;
        }


        [Command("Paste Component As New", QuickName = "PN",
            Help = "Paste the previously copied component as a new one on the selected gameObjects ",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            MenuItemLink = "PasteComponentAsNew", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "GameObject/Component")]
        public static void PasteComponentAsNew()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(o);
            }
        }

        [Command("Paste Component Value",
            "Paste the previously copied component values " +
            "in place of the first one of its type in the selected object", QuickName = "PV",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            MenuItemLink = "PasteComponentValues", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "GameObject/Component")]
        public static void ReplaceComponent()
        {
            if (componentCopiedType == null)
                return;

            foreach (GameObject o in Selection.gameObjects)
            {

                Component comp = o.GetComponent(componentCopiedType);
                if (comp)
                    UnityEditorInternal.ComponentUtility.PasteComponentValues(comp);
            }
        }

        [Command("Paste Component Value All",
            "Paste the previously copied component values " +
            "in place of all of its type in the selected object",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT, QuickName = "PVA",
            Category = "GameObject/Component")]
        public static void ReplaceComponents()
        {
            if (componentCopiedType == null)
                return;

            foreach (GameObject o in Selection.gameObjects)
            {

                Component[] comp = o.GetComponents(componentCopiedType);
                foreach (var component in comp)
                {
                    UnityEditorInternal.ComponentUtility.PasteComponentValues(component);
                }
            }
        }


        [Command("Duplicate On Axis", QuickName = "DA",
            Help = "Duplicates the selected object along the axis indicated, separated by a given distance",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_TRANSFORM, MenuItemLink = "DuplicateOnAxis",
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "GameObject")]
        public static void DuplicateOnAxis()
        {
            MonkeyEditorUtils.AddSceneCommand(new DuplicateAxisSceneCommand(Selection.gameObjects, 3, 1));
        }

        public class DuplicateAxisSceneCommand : TimedSceneCommand
        {
            public List<GameObject> ToDup = new List<GameObject>();
            public Vector3 Offset;

            public bool LocalAxis;
            public DirectionAxis Axis;
            public int DuplicateAmount;
            public float DuplicateDistance;

            private readonly Dictionary<GameObject, List<GameObject>> duplicated = new Dictionary<GameObject, List<GameObject>>();

            private bool applyDuplicate = false;

            public DuplicateAxisSceneCommand(GameObject[] toDuplicate, int duplicateAmount, float duplicateDistance) : base(0)
            {
                SceneCommandName = "Duplicate On Axis";

                ToDup.AddRange(toDuplicate);

                DuplicateAmount = duplicateAmount;
                DuplicateDistance = duplicateDistance;

                foreach (var o in ToDup)
                {
                    duplicated.Add(o, new List<GameObject>());
                }

                Duplicate();
            }

            private List<GameObject> toRemove = new List<GameObject>();

            public override void DisplayParameters()
            {
                int preAmount = DuplicateAmount;
                DisplayIntOption("Duplicate Amount", ref DuplicateAmount);

                if (DuplicateAmount < 1)
                    DuplicateAmount = 1;

                float prevDistance = DuplicateDistance;

                DisplayFloatOption("Duplicate Distance", ref DuplicateDistance);
                DisplayVectorOption("Offset", ref Offset);

                if (DuplicateDistance < 0)
                    DuplicateDistance = 0;

                DisplayBoolOption("Use Local Axis", ref LocalAxis);

                DirectionAxis prevAxis = Axis;
                Enum ax = Axis;
                DisplayEnumOption("Axis", ref ax);
                Axis = (DirectionAxis)ax;

                bool changed = DisplayObjectListOption("Objects To Duplicate", ToDup) != -1;

                if (changed)
                    ResolveNewObjects();

                if (preAmount != DuplicateAmount || changed || !Mathf.Approximately(prevDistance, DuplicateDistance)
                                                 || ToDup.Any(_ => _.transform.hasChanged) || prevAxis != Axis)
                    Duplicate();

                DisplayButton("Refresh Duplicate", Duplicate);

                DisplayButton("Apply Duplicate", ApplyDuplicate);
            }

            private void ResolveNewObjects()
            {
                toRemove.Clear();

                foreach (var pair in duplicated)
                {
                    if (ToDup.Contains(pair.Key))
                        continue;

                    foreach (var o in pair.Value)
                    {
                        Object.DestroyImmediate(o);
                    }

                    toRemove.Add(pair.Key);
                }

                foreach (var gameObject in toRemove)
                {
                    duplicated.Remove(gameObject);
                }

                foreach (var gameObject in ToDup)
                {
                    if (!duplicated.ContainsKey(gameObject))
                        duplicated.Add(gameObject, new List<GameObject>());
                }
            }

            private void ApplyDuplicate()
            {
                applyDuplicate = true;
                Stop();
            }

            public void Duplicate()
            {

                if (stopped)
                    return;

                if (ToDup.Count == 0)
                {
                    DestroyAll();

                    return;
                }

                if (duplicated[ToDup[0]].Count > DuplicateAmount)
                {
                    foreach (var pair in duplicated)
                    {
                        for (int i = 0; i < pair.Value.Count - DuplicateAmount; i++)
                        {
                            Object.DestroyImmediate(pair.Value[i]);
                        }

                        pair.Value.RemoveAll(_ => !_);
                    }
                }
                else if (duplicated[ToDup[0]].Count < DuplicateAmount)
                {
                    int count = duplicated[ToDup[0]].Count;

                    foreach (var pair in duplicated)
                    {
                        for (int i = 0; i < DuplicateAmount - count; i++)
                        {
                            var dup = pair.Key.InstantiateObjectAsInstance();
                            dup.transform.localScale = dup.transform.localScale;
                            dup.transform.SetParent(pair.Key.transform.parent, true);
                            pair.Value.Add(dup);
                        }
                    }
                }

                foreach (var pair in duplicated)
                {
                    Vector3 direction = pair.Key.transform.AxisToVector(Axis, LocalAxis);
                    Vector3 currentOff = LocalAxis ? pair.Key.transform.TransformDirection(Offset) : Offset;
                    float distance = DuplicateDistance;

                    for (int i = 0; i < DuplicateAmount; i++)
                    {
                        var newGo = pair.Value[i];
                        newGo.transform.position = pair.Key.transform.position
                                                   + distance * direction + currentOff * (i + 1);
                        newGo.transform.rotation = pair.Key.transform.rotation;

                        distance += DuplicateDistance;
                    }
                }



            }

            private void DestroyAll()
            {
                foreach (var pair in duplicated)
                {
                    foreach (var gameObject in pair.Value)
                    {
                        Object.DestroyImmediate(gameObject);
                    }

                    pair.Value.Clear();
                }
            }

            private bool stopped = false;

            public override void Stop()
            {
                if (!applyDuplicate)
                {
                    DestroyAll();
                    stopped = true;
                    base.Stop();
                    return;
                }

                int undoId = MonkeyEditorUtils.CreateUndoGroup("MonKey Object Duplicate");

                foreach (var pair in duplicated)
                {
                    foreach (var gameObject in pair.Value)
                    {
                        Undo.RegisterCreatedObjectUndo(gameObject, gameObject.name);
                    }
                    Selection.objects = Selection.objects.Append(pair.Value.Convert(_ => _ as Object)).ToArray();
                }

                Undo.CollapseUndoOperations(undoId);


                base.Stop();
            }
        }

        [Command("New GameObject With Components",
            "Creates a new object with the selected component types", QuickName = "NGC",
            Category = "GameObject/Component")]
        public static void CreateObjectWithComponents(
            [CommandParameter("The types of components to add",AutoCompleteMethodName = "ComponentAutoComplete")]
            Type[] componentTypes)
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Object with Components");
            GameObject go = new GameObject();

            Undo.RegisterCreatedObjectUndo(go, "New Object created");

            foreach (Type componentType in componentTypes)
            {
                Component comp = go.AddComponent(componentType);
                Undo.RegisterCreatedObjectUndo(comp, "New component created");
            }

            Selection.objects = new Object[] { go };
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("New Primitive Under Mouse",
            "Creates a new primitive object under the mouse", QuickName = "NPM",
            Category = "GameObject")]
        public static void CreatePrimitiveMouseRaycast(
           [CommandParameter("The primitive type to create")]
            PrimitiveType primitiveType=PrimitiveType.Cube)
        {
            MonkeyEditorUtils.AddSceneCommand(
                 new MoveUtilities.RaycastSceneCommand(
                     new[] { GameObject.CreatePrimitive(primitiveType) }, .5f, DirectionAxis.UP));
        }

        [Command("New Instances Between", QuickName = "NB",
            Help = "Creates new prefab instances between the two objects",
            MenuItemLink = "CreateInstancesBetween", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "GameObject")]
        public static void InstantiateBetweenTwo(
            [CommandParameter(Help="The list of prefabs you want to instantiate",
                AutoCompleteMethodName = "PrefabAssets",
                ForceAutoCompleteUsage = true,DefaultValueMethod = "DefaultAssets",
                DefaultValueNameOverride = "The Selected Prefabs",
                OverrideTypeName = "Prefab Names")]
            string[] prefabNames)
        {

            GameObject[] selected = prefabNames.Convert(AssetDatabase.LoadAssetAtPath<GameObject>).ToArray();

            MonkeyEditorUtils.AddSceneCommand(new NewInstancesBetweenSceneCommand(selected));
        }

        public class NewInstancesBetweenSceneCommand : TimedSceneCommand
        {

            public GameObject StartObject;
            public GameObject EndObject;

            public List<GameObject> ObjectsToCreate;
            private List<GameObject> prevObjectsToCreate;

            public int Amount = 1;

            private List<GameObject> createdObjects = new List<GameObject>();

            private bool commitCreated = false;

            private bool stopped = false;

            public NewInstancesBetweenSceneCommand(GameObject[] objs) : base(0)
            {
                SceneCommandName = "New Instance Between";
                ObjectsToCreate = objs.ToList();
                StartObject = MonKeySelectionUtils.OrderedGameObjects.FirstOrDefault();
                EndObject = MonKeySelectionUtils.OrderedGameObjects.ElementAtOrDefault(1);
                prevObjectsToCreate = new List<GameObject>(ObjectsToCreate);

            }

            public override void DisplayParameters()
            {

                base.DisplayParameters();
                var prevStart = StartObject;
                var prevEnd = EndObject;
                var prevAmount = Amount;

                DisplayObjectOption("Start Object", ref StartObject);
                DisplayObjectOption("End Object", ref EndObject);

                DisplayObjectListOption("Objects To Create", ObjectsToCreate);
                DisplayIntOption("Amount To Create", ref Amount);

                bool shouldRegenerate = false;

                if (ObjectsToCreate.Count != prevObjectsToCreate.Count)
                {
                    shouldRegenerate = true;
                }
                else
                {
                    for (int i = 0; i < ObjectsToCreate.Count; i++)
                    {
                        if (prevObjectsToCreate[i] != ObjectsToCreate[i])
                        {
                            shouldRegenerate = true;
                            break;
                        }
                    }
                }

                if (shouldRegenerate)
                {
                    foreach (var gameObject in createdObjects)
                    {
                        Object.DestroyImmediate(gameObject);
                    }
                    createdObjects.Clear();
                    prevObjectsToCreate = ObjectsToCreate;
                }

                if (StartObject && EndObject && (prevAmount != Amount || shouldRegenerate
                                                                      || prevStart != StartObject || prevEnd != EndObject
                                                                      || StartObject.transform.hasChanged
                                                                      || EndObject.transform.hasChanged))
                {
                    Create();
                }


                DisplayButton("Apply New Instances", Apply);

            }

            private void Apply()
            {
                commitCreated = true;
                Stop();
            }

            public override void Stop()
            {
                if (!commitCreated)
                {
                    foreach (var gameObject in createdObjects)
                    {
                        Object.DestroyImmediate(gameObject);
                    }
                    createdObjects.Clear();
                    stopped = true;

                    base.Stop();
                    return;
                }

                int undoId = MonkeyEditorUtils.CreateUndoGroup("MonKey Object Creation");
                foreach (var gameObject in createdObjects)
                {
                    Undo.RegisterCreatedObjectUndo(gameObject, gameObject.name);
                }
                Undo.CollapseUndoOperations(undoId);

                base.Stop();
            }

            public void Create()
            {

                if (stopped || !StartObject || !EndObject)
                    return;

                if (createdObjects.Count > Amount)
                {
                    for (int i = Amount; i < createdObjects.Count; i++)
                    {
                        if (!createdObjects[i])
                            continue;

                        Object.DestroyImmediate(createdObjects[i]);
                    }

                    createdObjects.RemoveAll(_ => !_);

                }
                else if (createdObjects.Count < Amount)
                {
                    int count = createdObjects.Count;
                    int k = 0;
                    for (int i = 0; i < Amount - count; i++)
                    {
                        var o = ObjectsToCreate[k];
                        // Debug.Log(o);
                        k++;
                        if (k >= ObjectsToCreate.Count)
                            k = 0;
                        /*  if (EditorUtility.DisplayCancelableProgressBar("Instantiating Prefabs",
                              "Instantiating " + o.name,
                              (float)i / (Amount)))
                              break;*/

                        var dup = o.InstantiateObjectAsInstance();
                        createdObjects.Add(dup);
                    }

                    // EditorUtility.ClearProgressBar();
                }


                Vector3 direction = EndObject.transform.position - StartObject.transform.position;
                float distance = direction.magnitude / (Amount + 1);
                Vector3 currentPosition = StartObject.transform.position + distance * direction.normalized;
                
                for (int i = 0; i < Amount; i++)
                {
                    GameObject go = createdObjects[i];

                    if (go)
                    {
                        go.transform.position = currentPosition;
                        currentPosition += distance * direction.normalized;
                    }

                }
            }

        }



        public static string[] DefaultAssets()
        {
            List<string> selectedAssets = new List<string>();

            foreach (GameObject o in Selection.gameObjects)
            {
                if (o.scene.IsValid())
                    continue;
                selectedAssets.Add(AssetDatabase.GetAssetPath(o));
            }

            return selectedAssets.ToArray();
        }

#if UNITY_2019

        [Command("New Material", "Creates a new Material with a default shader", QuickName = "NM")]
        public static void NewMaterial(
            [CommandParameter("The shader to use", AutoCompleteMethodName = "ShaderTypes", ForceAutoCompleteUsage = false,
                DefaultValueNameOverride = "Default Pipeline Shader",DefaultValueMethod = "DefaultShader")]
            ShaderInfo shaderInfo)
        {
            Shader source = Shader.Find(shaderInfo.name);
            Material mat = new Material(source);
            AssetDatabase.CreateAsset(mat, MonkeyEditorUtils.GetProjectWindowFocusedFolder() + "/New Material.mat");
            Selection.objects = new Object[] { mat };
            ProjectWindowUtil.ShowCreatedAsset(mat);
        }

        public static ShaderInfo DefaultShader()
        {
            switch (MonkeyEditorUtils.GetPipelineType())
            {
                case MonkeyEditorUtils.PipelineType.HDRP:
                    return ShaderUtil.GetAllShaderInfo().First(_ => _.name == "HDRP/Lit");
                case MonkeyEditorUtils.PipelineType.URP:
                    return ShaderUtil.GetAllShaderInfo().First(_ => _.name == "Universal Render Pipeline/Lit");
                default:
                    return ShaderUtil.GetAllShaderInfo().First(_ => _.name == "Standard");
            }
        }

        public static StaticCommandParameterAutoComplete ShaderTypes()
        {
            StaticCommandParameterAutoComplete autoComp = new StaticCommandParameterAutoComplete(typeof(ShaderInfo));
            var infos = ShaderUtil.GetAllShaderInfo();

            var dic = new Dictionary<string, object>();
            foreach (var shaderInfo in infos)
            {
                if (shaderInfo.supported && !shaderInfo.hasErrors && !shaderInfo.name.Contains("Hidden") && !shaderInfo.name.Contains("Legacy"))
                    dic.Add(shaderInfo.name, shaderInfo);
            }

            autoComp.ObjectsPerName = dic;
            return autoComp;
        }

#endif

    }
}
