using MonKey.Commands;
using MonKey.Editor.Console;
using MonKey.Editor.Internal;
using MonKey.Extensions;
using MonKey.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using Assembly = UnityEditor.Compilation.Assembly;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace MonKey.Editor.Commands
{
    public static class SelectionUtilities
    {
        public static bool IsAssetAFolder(Object obj)
        {
            string path = "";

            if (obj == null)
            {
                return false;
            }

            path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            if (path.Length > 0)
            {
                if (Directory.Exists(path))
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        internal static Type HierarchyWindowType = Type.GetType("UnityEditorInternal.SceneHierarchyWindow");
        internal static EditorWindow hierarchyWindow;

        internal static void SetSelectionActiveVersionIndependent(Object obj)
        {
            Object[] objs = Selection.objects;
#if UNITY_2017_1_OR_NEWER
            Selection.SetActiveObjectWithContext(obj, obj);
#else
            Selection.activeObject = obj;
#endif
            EditorGUIUtility.PingObject(obj);
            Selection.objects = objs;

            EditorApplication.RepaintHierarchyWindow();
            FocusHierarchyWindow();
        }

        private static void FocusHierarchyWindow()
        {
            if (hierarchyWindow == null)
            {
                Object[] potentialWindows = Resources.FindObjectsOfTypeAll(typeof(EditorWindow));
                foreach (var potentialWindow in potentialWindows)
                {
                    EditorWindow window = potentialWindow as EditorWindow;

                    if (window && window.titleContent.text == "Hierarchy")
                    {
                        hierarchyWindow = potentialWindow as EditorWindow;
                    }
                }
            }

            if (hierarchyWindow != null)
                hierarchyWindow.Focus();
        }

        public enum SelectionCombineRestrictedCombine
        {
            REPLACE,
            UNION,
            INTERSECTION,
            SEARCH_IN_CHILDREN,
            SEARCH_EXCLUDING_SELECTION,
            SELECTION_EXCLUDING_SEARCH
        }

        public static void SelectionCombine(SelectionCombineRestrictedCombine comb, params Object[] newSelection)
        {
            var prev = Selection.objects;
            switch (comb)
            {
                case SelectionCombineRestrictedCombine.REPLACE:
                    Selection.objects = newSelection;
                    return;
                case SelectionCombineRestrictedCombine.INTERSECTION:
                    Selection.objects = prev.Intersect(newSelection).ToArray();
                    return;
                case SelectionCombineRestrictedCombine.UNION:
                    Selection.objects = prev.Union(newSelection).ToArray();
                    return;
                case SelectionCombineRestrictedCombine.SEARCH_EXCLUDING_SELECTION:
                    Selection.objects = newSelection.Except(prev).ToArray();
                    return;
                case SelectionCombineRestrictedCombine.SELECTION_EXCLUDING_SEARCH:
                    Selection.objects = prev.Except(newSelection).ToArray();
                    return;
                case SelectionCombineRestrictedCombine.SEARCH_IN_CHILDREN:
                    Selection.objects = newSelection.Where(_ => Selection.objects.Any(yo => IsInChildren(yo, _)))
                        .ToArray();
                    return;
            }
        }

        private static bool IsInChildren(Object parent, Object toCheck)
        {
            if (parent is GameObject goParent)
            {
                if (toCheck is GameObject go)
                {
                    return go.transform.GetAllParentTransforms().Contains(goParent.transform);
                }

                if (toCheck is Component comp)
                {
                    return goParent.GetComponentsInChildren(comp.GetType()).Contains(comp);
                }

                return false;
            }

            return false;
        }


        public enum SelectionCombineType
        {
            UNION,
            INTERSECTION,
            SAVED_MINUS_CURRENT,
            CURRENT_MINUS_SAVED,
        }

        [Command("Save Selection",
            "Saves the currently selected objects to be reused later," +
            " or combined using the Combine Saved Selection Command")]
        public static void SaveSelection()
        {
            MonKeySelectionUtils.SelectionSave.Clear();
            MonKeySelectionUtils.SelectionSave.AddRange(Selection.objects);
        }

        [Command("Deselect Assets", "Deselects assets present in the selection", QuickName = "DA")]
        public static void UnSelectAssets()
        {
            var list = Selection.objects.ToList();

            list.RemoveAll(AssetDatabase.Contains);
            Selection.objects = list.ToArray();
        }

        [Command("Deselect Scene Objects", "Deselects assets present in the selection", QuickName = "DSO")]
        public static void UnSelectSceneObjects()
        {
            var list = Selection.objects.ToList();

            list.RemoveAll(_ => !AssetDatabase.Contains(_));
            Selection.objects = list.ToArray();
        }


        [Command("Select Saved Selection",
            "Selects the current saved selected objects")]
        public static void SelectSavedSelection()
        {
            Selection.objects = MonKeySelectionUtils.SelectionSave.ToArray();
        }

        [Command("Combine Saved Selection",
            "Combines the current selection with the previously saved selection (with teh save selection command) with boolean logic",
            Category = "Selection", QuickName = "CS")]
        public static void SelectionCombineWthSaved(SelectionCombineType combination)
        {
            List<Object> currentSelection = new List<Object>(Selection.objects);

            switch (combination)
            {
                case SelectionCombineType.UNION:
                    currentSelection.AddRange(MonKeySelectionUtils.SelectionSave);
                    break;
                case SelectionCombineType.CURRENT_MINUS_SAVED:
                    currentSelection.RemoveAll(_ => MonKeySelectionUtils.SelectionSave.Contains(_));
                    break;
                case SelectionCombineType.INTERSECTION:
                    currentSelection.RemoveAll(_ => !MonKeySelectionUtils.SelectionSave.Contains(_));
                    break;
                case SelectionCombineType.SAVED_MINUS_CURRENT:
                    var previous = MonKeySelectionUtils.SelectionSave;
                    previous.RemoveAll(_ => currentSelection.Contains(_));
                    currentSelection = previous;
                    break;
                default:
                    break;
            }

            Selection.objects = currentSelection.ToArray();
        }

        [Command("Select Common Parent",
            Help = "Selects the common parent of all the selected objects",
            QuickName = "SCP",
            ValidationMethodName = "ValidationAnyGameObject",
            MenuItemLink = "SelectParent", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Selection")]
        public static void SelectParent()
        {
            Transform trans = ParentingUtilities.FindEarliestCommonParent(Selection.gameObjects);

            if (trans)
            {
                Selection.objects = new Object[] {trans.gameObject};
                SetSelectionActiveVersionIndependent(trans.gameObject);
            }
            else
            {
                Debug.Log("Monkey Select Parent: No Common Parent Found");
            }
        }

        [Command("Select Parents",
            Help = "Selects the parents of all the selected objects",
            QuickName = "SPA",
            ValidationMethodName = "ValidationAnyGameObject",
            Category = "Selection")]
        public static void SelectParents()
        {
            Selection.objects =
                Selection.gameObjects.Convert(_ => _.transform.parent)
                    .Where(_ => _).Convert(_ => _.gameObject as Object).ToArray();

            if (Selection.objects.Length > 0)
                SetSelectionActiveVersionIndependent(Selection.objects[0]);
            else
            {
                Debug.LogWarning("MonKey Warning: No Objects were selected");
            }
        }


        [Command("Select GameObjects With Name Terms", QuickName = "SGT",
            Help = "Selects all objects that contain the specified terms in their name",
            MenuItemLink = "SelectWithTerms", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Selection")]
        public static void SelectSceneObjectsWithTerms(
            [CommandParameter(Help = "The terms to look for")]
            string[] termsToLookFor,
            [CommandParameter(Help = "How to combine the search with the current selection",
                OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            IEnumerable<Object> newObjs = Resources.FindObjectsOfTypeAll<Transform>()
                .Where(_ => SceneObjectWithTermSearch(_, termsToLookFor))
                .Convert(_ => (Object) _.gameObject);

            SelectionCombine(comb, newObjs.ToArray());

            if (Selection.objects.Length > 0)
                SetSelectionActiveVersionIndependent(Selection.objects[0]);
            else
            {
                Debug.LogWarning("MonKey Warning: No Objects were selected");
            }
        }

        private static bool SceneObjectWithTermSearch(Transform t, string[] termsToLookFor)
        {
            bool anyTerm = false;

            if (!t.gameObject.scene.IsValid())
                return false;

            foreach (var s in termsToLookFor)
            {
                anyTerm = t.name.ToLowerInvariant().Contains(s.ToLowerInvariant());
                if (anyTerm)
                    break;
            }

            return anyTerm;
        }

        [Command("Select Assets With Terms", QuickName = "SAT",
            Help = "Selects all assets that contain the specified terms in their name",
            MenuItemLink = "SelectWithTermsAssets", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Selection")]
        public static void SelectAssetsWithTerm(
            [CommandParameter(Help = "The terms to look for")]
            string[] termsToLookFor,
            [CommandParameter(Help = "How to combine the search with the current selection",
                OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            List<string> paths = new List<string>();
            foreach (string s in termsToLookFor)
            {
                paths.AddRange(
                    AssetDatabase.FindAssets(s.ToLowerInvariant()).Where(_ => !paths.Contains(_)));
            }

            IEnumerable<Object> objs =
                paths.Convert(_ =>
                    AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(_)));

            SelectionCombine(comb, objs.ToArray());

            if (Selection.objects.Length > 0)
                SetSelectionActiveVersionIndependent(Selection.objects[0]);
            else
            {
                Debug.LogWarning("MonKey Warning: No Objects were selected");
            }
        }

        [Command("Select Siblings",
            Help = "Selects the siblings of all the selected objects",
            QuickName = "SS",
            ValidationMethodName = "ValidationAnyGameObject",
            MenuItemLink = "SelectSiblings", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Selection")]
        public static void SelectSiblings()
        {
            List<Object> objectsToSelect = new List<Object>();

            foreach (var gameObject in Selection.gameObjects.Where(_ => _.scene.IsValid()))
            {
                if (gameObject.transform.parent)
                    foreach (Transform t in gameObject.transform.parent)
                    {
                        objectsToSelect.Add(t.gameObject);
                    }
                else
                {
                    objectsToSelect.AddRange(gameObject.scene.GetRootGameObjects());
                }
            }

            Selection.objects = Selection.objects.Append(objectsToSelect.Where(_ => !Selection.objects.Contains(_)))
                .ToArray();

            if (Selection.objects.Length > 0)
                SetSelectionActiveVersionIndependent(Selection.objects[0]);
            else
            {
                Debug.LogWarning("MonKey Warning: No Objects were selected");
            }
        }

        [Command("Find Asset", "Selects an asset and pings it in the project folder",
            AlwaysShow = true, QuickName = "FA", Order = -5,
            MenuItemLink = "SelectAssetHotKeyOverride", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Selection")]
        public static void FindAsset(
            [CommandParameter(Help = "The name of the asset you wish to select",
                AutoCompleteMethodName = "AssetNamesSuggestions",
                ForceAutoCompleteUsage = true, OverrideTypeName = "Asset Name",
                PreventDefaultValueUsage = true)]
            string assetName)
        {
            Object selected =
                AssetDatabase.LoadAssetAtPath<Object>(assetName);
            Selection.objects = new[] {selected};
            EditorUtility.FocusProjectWindow();
            SetSelectionActiveVersionIndependent(selected);
        }


        [Command("Find Asset Of Type", "Selects an asset of a type and pings it in the project folder",
            QuickName = "FAT",
            Category = "Selection")]
        public static void FindAssetOfType(
            [CommandParameter(AutoCompleteMethodName = "AssetTypeAuto",
                Help = "The type of asset to look for",
                ForceAutoCompleteUsage = true, PreventDefaultValueUsage = true)]
            Type assetType,
            [CommandParameter(Help = "The name of the asset you wish to select",
                AutoCompleteMethodName = "AssetNamesOfTypeSuggestions",
                ForceAutoCompleteUsage = true, OverrideTypeName = "Asset Name",
                PreventDefaultValueUsage = true)]
            string assetName)
        {
            Object selected =
                AssetDatabase.LoadAssetAtPath<Object>(assetName);
            Selection.objects = new[] {selected};
            EditorUtility.FocusProjectWindow();
            SetSelectionActiveVersionIndependent(selected);
        }

        public static TypeAutoComplete AssetTypeAuto()
        {
            return new TypeAutoComplete();
        }

        public static AssetNameAutoComplete AssetNamesOfTypeSuggestions()
        {
            Type type = (Type) MonkeyEditorUtils.GetParsedParameter(0);
            Debug.Log(type.ToString());
            Debug.Log(MonkeyEditorUtils.GetParsedParameter(0));
            return new AssetNameAutoComplete() {CustomType = type.Name, IncludeDirectories = false};
        }

        [Command("Find Asset With Preview", "Selects an asset and ping it in the project folder. " +
                                            "Slower than Find Asset, as it needs to fetch previews.",
            AlwaysShow = true, QuickName = "FAP", Order = -5,
            Category = "Selection")]
        public static void FindAssetPrev(
            [CommandParameter(Help = "The name of the asset you wish to select",
                AutoCompleteMethodName = "AssetNamesSuggestionsPrev",
                ForceAutoCompleteUsage = true, OverrideTypeName = "Asset Name",
                PreventDefaultValueUsage = true)]
            string assetName)
        {
            Object selected =
                AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetName);
            Selection.objects = new[] {selected};
            EditorUtility.FocusProjectWindow();
            SetSelectionActiveVersionIndependent(selected);
        }

        [Command("Find Folder", "Selects a folder and ping it in the project folder",
            QuickName = "FF", Order = 0,
            Category = "Selection")]
        public static void FindFolder(
            [CommandParameter(Help = "The name of the folder you wish to select",
                AutoCompleteMethodName = "FolderNamesSuggestions", ForceAutoCompleteUsage = true)]
            string assetName)
        {
            Object selected =
                AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetName);
            Selection.objects = new[] {selected};
            EditorUtility.FocusProjectWindow();
            SetSelectionActiveVersionIndependent(selected);
        }


        [Command("Find GameObject", "Selects a GameObject in one of the opened scene",
            AlwaysShow = true, QuickName = "FG", Order = -5,
            MenuItemLink = "SelectGameObjectHotKeyOverride",
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Selection")]
        public static void FindGameObject(
            [CommandParameter(Help = "The name of the GameObject you wish to select",
                ForceAutoCompleteUsage = true)]
            GameObject gameObject)
        {
            Selection.objects = new Object[] {gameObject};
            SetSelectionActiveVersionIndependent(gameObject);
        }

        [Command("Find GameObject In Children",
            "Finds a GameObject under the currently selected GameObjects",
            QuickName = "FGC", Order = -1,
            MenuItemLink = "SelectWithTermsChildren", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Selection")]
        public static void SelectGameObjectInChildren(
            [CommandParameter(Help = "The name of the GameObject you wish to select",
                ForceAutoCompleteUsage = true, AutoCompleteMethodName = "GameObjectInChildren")]
            GameObject gameObject)
        {
            Selection.objects = new Object[] {gameObject};

            SetSelectionActiveVersionIndependent(gameObject);
        }


        private static CommandParameterAutoComplete<GameObject> GameObjectInChildren()
        {
            CommandParameterObjectAutoComplete<GameObject> autoComplete =
                new CommandParameterObjectAutoComplete<GameObject>();
            foreach (var transform in Selection.transforms)
            {
                foreach (Transform subTransform in transform.GetAllSubTransforms())
                {
                    autoComplete.AddValue(subTransform.gameObject.name, subTransform.gameObject);
                }
            }

            return autoComplete;
        }

        private static AssetNameAutoComplete FolderNamesSuggestions()
        {
            return new AssetNameAutoComplete() {DirectoryMode = true};
        }

        private static AssetNameAutoComplete AssetNamesSuggestions()
        {
            return AutoCompleteManager.AssetNameAutoComplete;
        }

        private static AssetNameAutoComplete AssetNamesSuggestionsPrev()
        {
            return new AssetNameAutoComplete() {showAssetPreview = true};
        }


        [CommandValidation(DefaultValidationMessages.DEFAULT_SELECTED_GAMEOBJECTS)]
        private static bool ValidationAnyGameObject()
        {
            return Selection.gameObjects.Length > 0;
        }

        [Command("Select Children", "Select the direct children of the selected objects",
            QuickName = "SC", ValidationMethodName = "ValidationAnyGameObject",
            Category = "Selection")]
        public static void SelectChildren(SelectionCombineRestrictedCombine SelectionCombine = SelectionCombineRestrictedCombine.REPLACE)
        {
            List<Object> objects = new List<Object>();

            foreach (var o in Selection.gameObjects)
            {
                foreach (Transform t in o.transform)
                {
                    if (!objects.Contains(t))
                        objects.Add(t.gameObject);
                }
            }

            if (Selection.objects.Length == 0)
            {
                Debug.LogWarning("MonKey Warning: No Objects were selected");
            }

            SelectionUtilities.SelectionCombine(SelectionCombine, objects.ToArray());
        }

        [Command("Select All Children", "Select all the children of the selected objects",
            QuickName = "SC", ValidationMethodName = "ValidationAnyGameObject",
            Category = "Selection")]
        public static void SelectAllChildren(
            SelectionCombineRestrictedCombine SelectionCombine = SelectionCombineRestrictedCombine.REPLACE)
        {
            List<Object> objects = new List<Object>();

            foreach (var o in Selection.gameObjects)
            {
                objects.AddRange(o.GetComponentsInChildren<Transform>().Where(_ => _.gameObject != o)
                    .Convert(_ => _.gameObject));
            }

            if (Selection.objects.Length == 0)
            {
                Debug.LogWarning("MonKey Warning: No Objects were selected");
            }

            SelectionUtilities.SelectionCombine(SelectionCombine, objects.ToArray());
        }

        public enum RandomSelectionRange
        {
            IN_CURRENT_SELECTION,
            IN_CHILDREN,
        }

        [Command("Select Random", "Select Random objects within current selection, children, or all objects",
            QuickName = "SC", ValidationMethodName = "ValidationAnyGameObject",
            Category = "Selection")]
        public static void SelectRandomChildren(float percent = 0.5f,
            [CommandParameter(Help = "Where to select random objects",
                OverrideName = "Selection Combining")]
            RandomSelectionRange comb = RandomSelectionRange.IN_CURRENT_SELECTION)
        {
            List<Object> pool = new List<Object>();

            switch (comb)
            {
                case RandomSelectionRange.IN_CURRENT_SELECTION:
                    pool.AddRange(Selection.gameObjects);
                    break;
                case RandomSelectionRange.IN_CHILDREN:
                    pool.AddRange(Selection.activeGameObject.transform.GetChildren().Convert(_ => _.gameObject));
                    break;
            }

            percent = Mathf.Clamp01(percent);

            List<Object> selectedObjects = new List<Object>();
            pool.Shuffle();
            selectedObjects.AddRange(pool.GetRange(0, Mathf.FloorToInt(pool.Count * percent)));


            Selection.objects = selectedObjects.ToArray();

            if (Selection.objects.Length > 0)
                SetSelectionActiveVersionIndependent(Selection.objects[0]);
            else
            {
                Debug.LogWarning("MonKey Warning: No Objects were selected");
            }
        }


        [Command("Add GameObjects To Selection",
            "Adds some objects to the currently selected ones", QuickName = "AG",
            Category = "Selection")]
        public static void AddToSelection(
            [CommandParameter(Help = "The objects to add", PreventDefaultValueUsage = true,
                ForceAutoCompleteUsage = true)]
            GameObject[] objects)
        {
            Selection.objects = Selection.objects.Append(objects).ToArray();

            if (Selection.objects.Length > 0)
                SetSelectionActiveVersionIndependent(Selection.objects[0]);
            else
            {
                Debug.LogWarning("MonKey Warning: No Objects were selected");
            }
        }

        [Command("Select Scene Objects of Type",
            "Selects all the objects of the specified type", QuickName = "ST",
            Category = "Selection")]
        public static void SelectOfType(
            [CommandParameter(Help = "The Type to Select", PreventDefaultValueUsage = true,
                ForceAutoCompleteUsage = true, AutoCompleteMethodName = "SceneTypes")]
            Type type,
            [CommandParameter(Help = "How to combine the search with the current selection",
                OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            EditorUtility.DisplayProgressBar("Finding " + type.Name + " in the scene",
                "MonKey is still looking for all the object!", .5f);

            Object[] objs;
            
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                objs = stage.scene.GetRootGameObjects()[0].GetComponentsInChildren(type)
                    .Convert(_ => _.gameObject).ToArray();
            }
            else
            {
                objs = TransformExt.GetAllTransformedOrderUpToDown(_ => _.GetComponent(type) != null)
                    .Convert(_ => (Object) _.gameObject).ToArray();
            }

            SelectionCombine(comb, objs.ToArray());

            EditorUtility.ClearProgressBar();
            if (Selection.objects.Length > 0)
                SetSelectionActiveVersionIndependent(Selection.objects[0]);
            else
            {
                Debug.Log("MonKey found no objects of the corresponding type");
            }
        }

        [Command("Select Children of Type",
            "Selects all the children objects of the specified type", QuickName = "SCT",
            Category = "Selection")]
        public static void SelectChildrenOfType(
            [CommandParameter(Help = "The Type to Select", PreventDefaultValueUsage = true,
                ForceAutoCompleteUsage = true, AutoCompleteMethodName = "SceneTypes")]
            Type type,
            [CommandParameter(Help = "How to combine the search with the current selection",
                OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            EditorUtility.DisplayProgressBar("Finding " + type.Name + " in the children",
                "MonKey is still looking for all the object!", .5f);

            List<Object> selection = new List<Object>();
            foreach (GameObject o in Selection.gameObjects)
            {
                selection.AddRange(o.GetComponentsInChildren(type)
                    .Convert(_ => _.gameObject as Object));
            }

            EditorUtility.ClearProgressBar();

            SelectionCombine(comb, selection.ToArray());

            if (Selection.objects.Length > 0)
                SetSelectionActiveVersionIndependent(Selection.objects[0]);
            else
            {
                Debug.Log("MonKey found no objects of the corresponding type");
            }
        }
        

        
        public static TypeAutoComplete AssetTypes()
        {
            return new TypeAutoComplete(true, false, true, true);
        }

        public static TypeAutoComplete SceneTypes()
        {
            return new TypeAutoComplete(false, true, true, false);
        }


        [Command("Add Asset To Selection", "Adds some assets to the currently selected ones",
            QuickName = "AA",
            Category = "Selection")]
        public static void AddToSelection(
            [CommandParameter(Help = "The assets to add", PreventDefaultValueUsage = true,
                ForceAutoCompleteUsage = true, AutoCompleteMethodName = "AssetNamesAuto")]
            string[] assets)
        {
            List<Object> objs = new List<Object>();
            foreach (var asset in assets)
            {
                objs.Add(AssetDatabase.LoadAssetAtPath<Object>(asset));
            }

            Selection.objects = Selection.objects.Append(objs).ToArray();
            EditorUtility.FocusProjectWindow();
        }

        [Command("UnSelect All", "Clears all the selected objects", QuickName = "UNA",
            MenuItemLink = "UnSelectAll", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Selection")]
        public static void ClearSelection()
        {
            Selection.objects = new Object[0];
        }

        /*   [Command("Order Selection By Hierarchy",
               "Orders the selection based on the hierarchy as seen in the editor", QuickName = "OSH",
               Category = "Selection")]
           public static void OrderSelectionByHierarchy(
               [CommandParameter(Help=
                   "if true, sorts from top to bottom, with scene objects first, and assets then")]
               bool descending=true)
           {
               MonkeyEditorUtils.OrderSelectionByHierarchy(descending);
           }*/

        [Command("UnSelect Children",
            "UnSelects all the children objects if their parent is selected",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT, QuickName = "UNC",
            Category = "Selection")]
        public static void UnSelectChildren()
        {
            Selection.objects = Selection.transforms.Convert(_ => (Object) _.gameObject).ToArray();
        }

        [Command("UnSelect Parent", "UnSelects the up most parents of the selection :" +
                                    " intermediate parents will still be selected",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT, QuickName = "UNP",
            Category = "Selection")]
        public static void UnSelectParents()
        {
            Selection.objects = Selection.gameObjects.Where(_ => !Selection.transforms.Contains(_.transform) ||
                                                                 _.transform.childCount == 0).Convert(_ => (Object) _)
                .ToArray();
        }

        public static AssetNameAutoComplete AssetNamesAuto()
        {
            return new AssetNameAutoComplete();
        }

        [Command("Select Folders of Objects", "Selects the containing folders of the selected assets",
            QuickName = "SF", DefaultValidation = DefaultValidation.AT_LEAST_ONE_ASSET,
            Category = "Selection")]
        public static void SelectFoldersOfObjects()
        {
            List<Object> folderObjects = new List<Object>();

            foreach (Object o in Selection.objects.Where(AssetDatabase.Contains))
            {
                if (!o.IsDirectory())
                    folderObjects.Add(AssetDatabase.LoadAssetAtPath<Object>(
                        ProjectWindowUtil.GetContainingFolder(AssetDatabase.GetAssetPath(o))));
            }

            Selection.objects = folderObjects.ToArray();
            EditorUtility.FocusProjectWindow();
        }

        [Command("Select Meshes In Children",
            "Selects all the meshes under the selected objects (included)",
            QuickName = "SME", DefaultValidation = DefaultValidation.AT_LEAST_ONE_GAME_OBJECT,
            Category = "Selection")]
        public static void SelectMeshesInChildren()
        {
            List<Object> selection = new List<Object>();

            foreach (var o in MonkeyEditorUtils.OrderedSelectedGameObjects)
            {
                selection.AddRange(o.GetComponentsInChildren<MeshRenderer>()
                    .Convert(_ => _.gameObject as Object));
            }

            Selection.objects = selection.ToArray();
            if (Selection.objects.Length > 0)
                SetSelectionActiveVersionIndependent(Selection.objects[0]);
            else
            {
                Debug.LogWarning("MonKey Warning: No Objects were selected");
            }
        }


        [Command("Select Previous Selection", QuickName = "SPS",
            Help = "Selects the previous selection since the last empty selection ",
            MenuItemLink = "SelectPreviousSelection",
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Selection")]
        private static void SelectPreviousSelection()
        {
            /* List<Object> oldList = MonkeyEditorUtils.OrderedSelectedObjectsInt;
             List<Transform> oldListT = MonkeyEditorUtils.OrderedSelectedTransformInt;

             MonkeyEditorUtils.OrderedSelectedObjectsInt = new List<Object>(MonkeyEditorUtils.PreviousOrderedSelectedObjects);
             MonkeyEditorUtils.OrderedSelectedTransformInt = new List<Transform>(MonkeyEditorUtils.PreviousOrderedSelectedTransform);
             Selection.objects = MonkeyEditorUtils.OrderedSelectedObjectsInt.ToArray();

             MonkeyEditorUtils.PreviousOrderedSelectedObjects = oldList;
             MonkeyEditorUtils.PreviousOrderedSelectedTransform = oldListT;*/

            Selection.objects = MonKeySelectionUtils.PreviousSelectionStack.ToArray();
        }

        /// <summary>
        /// Test of ordered Selection
        /// </summary>
        [Command("Print Ordered Selection",
            "Prints the names of teh objects in the order they were selected", QuickName = "POS",
            Category = "Selection")]
        public static void PrintOrderedSelection()
        {
            Debug.Log("---------TRANSFORMS---------");
            foreach (var transform in MonkeyEditorUtils.OrderedSelectedTransform)
            {
                Debug.Log(transform);
            }


            Debug.Log("---------Game Objects---------");
            foreach (var go in MonkeyEditorUtils.OrderedSelectedGameObjects)
            {
                Debug.Log(go.name);
            }

            Debug.Log("---------Objects---------");
            foreach (var o in MonkeyEditorUtils.OrderedSelectedObjects)
            {
                Debug.Log(o.name);
            }
        }

        [Command("Select Objects With Tag",
            "Selects all the objects in the opened scenes that have the given tag",
            QuickName = "SOT",
            Category = "Selection")]
        public static void SelectObjectsWithTag(
            [CommandParameter("The tag you wish to look for",
                AutoCompleteMethodName = "TagAutoComplete",
                ForceAutoCompleteUsage = true,
                OverrideTypeName = "Tag Name", PreventDefaultValueUsage = true)]
            string tag,
            [CommandParameter(Help = "How to combine the search with the current selection",
                OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            Object[] newObjs;
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                newObjs = stage.scene.GetRootGameObjects()[0].GetComponentsInChildren<Transform>()
                    .Where(_ => _.CompareTag(tag)).Convert(_ => _.gameObject).ToArray();
            }
            else
            {
                newObjs = GameObject.FindGameObjectsWithTag(tag)
                    .Convert(_ => _ as Object).ToArray();
            }
            
            SelectionCombine(comb, newObjs.ToArray());
        }

        public static TagAutoComplete TagAutoComplete()
        {
            return new TagAutoComplete();
        }

        public static TypeAutoComplete SelectStringType()
        {
            return new TypeAutoComplete();
        }

        public static StaticCommandParameterAutoComplete SelectBoolField()
        {
            return AutoCompleteManager.FieldAutoComplete(1, typeof(bool));
        }

        public enum ObjectTargeted
        {
            SCENE,
            ASSET,
            BOTH
        }

        [Command("Select Objects By Bool Value",
            "Selects all the object with a defined bool field having defined value", QuickName = "SOBV",
            Category = "Selection")]
        public static void SelectObjectWithBoolValue(
            [CommandParameter(Help = "Should the command included only scene objects or more",
                ForceAutoCompleteUsage = true,
                DefaultValueMethod = "DefaultTargetObject")]
            ObjectTargeted targetedObjects,
            [CommandParameter(Help = "The type of object to look for",
                AutoCompleteMethodName = "SelectStringType",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            Type objectType,
            [CommandParameter(Help = "The name of the field to look for",
                AutoCompleteMethodName = "SelectBoolField",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            string fieldName, bool value,
            [CommandParameter(Help = "How to combine the search with the current selection",
                OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            SelectObjectWithMembersWithValue(objectType, fieldName, value, targetedObjects, null, comb);
        }


        [Command("Select Objects By Object Reference",
            "Selects all the object with a defined Unity Object field having a defined value", QuickName = "SOOV",
            Category = "Selection")]
        public static void SelectObjectWithObjectValue(
            [CommandParameter(Help = "Should the command included only scene objects or more",
                ForceAutoCompleteUsage = true,
                DefaultValueMethod = "DefaultTargetObject")]
            ObjectTargeted targetedObjects,
            [CommandParameter(Help = "The type of object to look for",
                AutoCompleteMethodName = "SelectStringType",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            Type objectType,
            [CommandParameter(Help = "The type of field to look for",
                AutoCompleteMethodName = "SelectStringTypeSecond",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            Type fieldType,
            [CommandParameter(Help = "The name of the field to look for",
                AutoCompleteMethodName = "SelectObjectField",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            string fieldName,
            [CommandParameter(Help = "The object to check as a reference",
                PreventDefaultValueUsage = true, ForceAutoCompleteUsage = true,
                AutoCompleteMethodName = "ObjectByTypeAuto")]
            Object value, [CommandParameter(Help = "How to combine the search with the current selection",
                OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            SelectObjectWithMembersWithValue(objectType, fieldName, value, targetedObjects, null, comb);
        }

        public static ObjectTypeAutoComplete<Object> ObjectByTypeAuto()
        {
            ObjectTypeAutoComplete<Object> auto = new ObjectTypeAutoComplete<Object>();
            auto.SceneObjectsOnly = false;
            auto.ParameterType = (Type) MonkeyEditorUtils.GetParsedParameter(2);

            return auto;
        }

        public static TypeAutoComplete SelectStringTypeSecond()
        {
            var typeFound = (Type) MonkeyEditorUtils.GetParsedParameter(1);

            var fields = typeFound.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                             BindingFlags.FlattenHierarchy | BindingFlags.Default);
            var properties = typeFound.GetProperties(BindingFlags.NonPublic | BindingFlags.Public |
                                                     BindingFlags.Instance | BindingFlags.FlattenHierarchy |
                                                     BindingFlags.Default & BindingFlags.SetProperty);

            var complete = new TypeAutoComplete(false, false, false, false);

            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType.IsSubclassOf(typeof(Object)) || fieldInfo.FieldType == typeof(Object))
                    complete.AddNewType(fieldInfo.FieldType.Name, fieldInfo.FieldType);
            }

            foreach (var fieldInfo in properties)
            {
                if (fieldInfo.PropertyType.IsSubclassOf(typeof(Object)) || fieldInfo.PropertyType == typeof(Object))
                    complete.AddNewType(fieldInfo.PropertyType.Name, fieldInfo.PropertyType);
            }

            return complete;
        }

        public static StaticCommandParameterAutoComplete SelectObjectField()
        {
            Type type = (Type) MonkeyEditorUtils.GetParsedParameter(2);
            return AutoCompleteManager.FieldAutoComplete(1, type);
        }


        public static StaticCommandParameterAutoComplete SelectStringField()
        {
            return AutoCompleteManager.FieldAutoComplete(1, typeof(string));
        }

        [Command("Select Objects By String Value",
            "Selects all the object with a defined string field having defined value", QuickName = "SOSV",
            Category = "Selection")]
        public static void SelectObjectWithStringValue(
            [CommandParameter(Help = "Should the command included only scene objects or more",
                ForceAutoCompleteUsage = true,
                DefaultValueMethod = "DefaultTargetObject")]
            ObjectTargeted targetedObjects,
            [CommandParameter(Help = "The type of object to look for",
                AutoCompleteMethodName = "SelectStringType",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            Type objectType,
            [CommandParameter(Help = "The name of the field to look for",
                AutoCompleteMethodName = "SelectStringField",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            string fieldName, string value,
            [CommandParameter(Help = "How to combine the search with the current selection",
                OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            SelectObjectWithMembersWithValue(objectType, fieldName, value, targetedObjects, null, comb);
        }

        [Command("Select Objects Containing String Value",
            "Selects all the object with a defined string field containing a defined value", QuickName = "SOSC",
            Category = "Selection")]
        public static void SelectObjectsContainingStringValue(
            [CommandParameter(Help = "Should the command included only scene objects or more",
                ForceAutoCompleteUsage = true,
                DefaultValueMethod = "DefaultTargetObject")]
            ObjectTargeted targetedObjects,
            [CommandParameter(Help = "The type of object to look for",
                AutoCompleteMethodName = "SelectStringType",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            Type objectType,
            [CommandParameter(Help = "The name of the field to look for",
                AutoCompleteMethodName = "SelectStringField",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            string fieldName, string value,
            [CommandParameter(Help = "How to combine the search with the current selection",
                OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            bool Contains(object found, object objectvalueWanted)
            {
                string foundString = (string) found;
                return (foundString.Contains((string) objectvalueWanted));
            }

            SelectObjectWithMembersWithValue(objectType, fieldName, value, targetedObjects, Contains, comb);
        }

        public enum CompareMethod
        {
            EQUAL,
            LESSER,
            GREATER,
            GREATER_OR_EQUAL,
            LESSER_OR_EQUAL,
        }

        [Command("Select Objects By Float Value",
            "Selects all the object with a defined float field compared to a float value", QuickName = "SOFV",
            Category = "Selection")]
        public static void SelectObjectsWithFloatValue(
            [CommandParameter(Help = "Should the command included only scene objects or more",
                ForceAutoCompleteUsage = true,
                DefaultValueMethod = "DefaultTargetObject")]
            ObjectTargeted targetedObjects,
            [CommandParameter(Help = "The type of object to look for",
                AutoCompleteMethodName = "SelectStringType",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            Type objectType,
            [CommandParameter(Help = "The name of the field to look for",
                AutoCompleteMethodName = "SelectFloatField",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            string fieldName, CompareMethod compare = CompareMethod.EQUAL,
            float value = 0,
            [CommandParameter(Help = "How to combine the search with the current selection",
                OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            bool Contains(object found, object objectvalueWanted)
            {
                float foundFloat = (float) found;
                float valueWanted = (float) objectvalueWanted;

                switch (compare)
                {
                    case CompareMethod.EQUAL:
                        return Mathf.Approximately(foundFloat, valueWanted);
                    case CompareMethod.GREATER:
                        return foundFloat > valueWanted;
                    case CompareMethod.GREATER_OR_EQUAL:
                        return foundFloat >= valueWanted;
                    case CompareMethod.LESSER:
                        return foundFloat < valueWanted;
                    case CompareMethod.LESSER_OR_EQUAL:
                        return foundFloat <= valueWanted;
                    default:
                        return false;
                }
            }

            SelectObjectWithMembersWithValue(objectType, fieldName, value, targetedObjects, Contains, comb);
        }

        [Command("Select Objects By Vector Magnitude Value",
            "Selects all the object with a defined vector field compared to a magnitude value", QuickName = "SOFV",
            Category = "Selection")]
        public static void SelectObjectsWithVectorMagnitudeValue(
            [CommandParameter(Help = "Should the command included only scene objects or more",
                ForceAutoCompleteUsage = true,
                DefaultValueMethod = "DefaultTargetObject")]
            ObjectTargeted targetedObjects,
            [CommandParameter(Help = "The type of object to look for",
                AutoCompleteMethodName = "SelectStringType",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            Type objectType,
            [CommandParameter(Help = "The name of the field to look for",
                AutoCompleteMethodName = "SelectVectorField",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            string fieldName, CompareMethod compare = CompareMethod.EQUAL,
            float value = 0, [CommandParameter(Help = "How to combine the search with the current selection",
                OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            bool Contains(object found, object objectvalueWanted)
            {
                float foundFloat = ((Vector3) found).magnitude;
                float valueWanted = (float) objectvalueWanted;

                switch (compare)
                {
                    case CompareMethod.EQUAL:
                        return Mathf.Approximately(foundFloat, valueWanted);
                    case CompareMethod.GREATER:
                        return foundFloat > valueWanted;
                    case CompareMethod.GREATER_OR_EQUAL:
                        return foundFloat >= valueWanted;
                    case CompareMethod.LESSER:
                        return foundFloat < valueWanted;
                    case CompareMethod.LESSER_OR_EQUAL:
                        return foundFloat <= valueWanted;
                    default:
                        return false;
                }
            }

            SelectObjectWithMembersWithValue(objectType, fieldName, value, targetedObjects, Contains, comb);
        }

        [Command("Select Objects By Vector Axis Value",
            "Selects all the object with a defined vector field with x, y or z matching a value", QuickName = "SOFV",
            Category = "Selection")]
        public static void SelectObjectsWithVectorAxisValue(
            [CommandParameter(Help = "Should the command included only scene objects or more",
                ForceAutoCompleteUsage = true,
                DefaultValueMethod = "DefaultTargetObject")]
            ObjectTargeted targetedObjects,
            [CommandParameter(Help = "The type of object to look for",
                AutoCompleteMethodName = "SelectStringType",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            Type objectType,
            [CommandParameter(Help = "The name of the field to look for",
                AutoCompleteMethodName = "SelectVectorField",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            string fieldName, CompareMethod compare = CompareMethod.EQUAL,
            float value = 0, [CommandParameter(Help = "How to combine the search with the current selection",
                OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            bool Contains(object found, object objectvalueWanted)
            {
                Vector3 foundFloat = ((Vector3) found);
                float valueWanted = (float) objectvalueWanted;

                switch (compare)
                {
                    case CompareMethod.EQUAL:
                        return Mathf.Approximately(foundFloat.x, valueWanted)
                               || Mathf.Approximately(foundFloat.y, valueWanted)
                               || Mathf.Approximately(foundFloat.z, valueWanted);
                    case CompareMethod.GREATER:
                        return foundFloat.x > valueWanted || foundFloat.y > valueWanted || foundFloat.y > valueWanted;
                    case CompareMethod.GREATER_OR_EQUAL:
                        return foundFloat.x >= valueWanted || foundFloat.y >= valueWanted ||
                               foundFloat.z >= valueWanted;
                    case CompareMethod.LESSER:
                        return foundFloat.x < valueWanted || foundFloat.y < valueWanted || foundFloat.z < valueWanted;
                    case CompareMethod.LESSER_OR_EQUAL:
                        return foundFloat.x <= valueWanted || foundFloat.y <= valueWanted ||
                               foundFloat.z <= valueWanted;
                    default:
                        return false;
                }
            }

            SelectObjectWithMembersWithValue(objectType, fieldName, value, targetedObjects, Contains, comb);
        }

        [Command("Select Objects By Vector Angle Value",
            "Selects all the object with a defined vector field's angle to a vector value", QuickName = "SOFV",
            Category = "Selection")]
        public static void SelectObjectsWithVectorAngleValue(
            [CommandParameter(Help = "Should the command included only scene objects or more",
                ForceAutoCompleteUsage = true,
                DefaultValueMethod = "DefaultTargetObject")]
            ObjectTargeted targetedObjects,
            [CommandParameter(Help = "The type of object to look for",
                AutoCompleteMethodName = "SelectStringType",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            Type objectType,
            [CommandParameter(Help = "The name of the field to look for",
                AutoCompleteMethodName = "SelectVectorField",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            string fieldName, CompareMethod compare = CompareMethod.EQUAL,
            Vector3 value = default(Vector3), float angle = 0.1f, [CommandParameter(
                Help = "How to combine the search with the current selection", OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            bool Contains(object found, object objectvalueWanted)
            {
                Vector3 foundVec = (Vector3) found;
                Vector3 otherVec = (Vector3) objectvalueWanted;

                float a = Mathf.Abs(Vector3.Angle(foundVec, otherVec));

                switch (compare)
                {
                    case CompareMethod.EQUAL:
                        return Mathf.Approximately(a, angle);
                    case CompareMethod.GREATER:
                        return a > angle;
                    case CompareMethod.GREATER_OR_EQUAL:
                        return a >= angle;
                    case CompareMethod.LESSER:
                        return a < angle;
                    case CompareMethod.LESSER_OR_EQUAL:
                        return a <= angle;
                    default:
                        return false;
                }
            }

            SelectObjectWithMembersWithValue(objectType, fieldName, value, targetedObjects, Contains, comb);
        }

        public static StaticCommandParameterAutoComplete SelectVectorField()
        {
            return AutoCompleteManager.FieldAutoComplete(1, typeof(Vector3));
        }

        public static StaticCommandParameterAutoComplete SelectFloatField()
        {
            return AutoCompleteManager.FieldAutoComplete(1, typeof(float));
        }

        [Command("Select Objects By Int Value",
            "Selects all the object with a defined int field compared to an int value", QuickName = "SOIV",
            Category = "Selection")]
        public static void SelectObjectsWithIntValue(
            [CommandParameter(Help = "Should the command included only scene objects or more",
                ForceAutoCompleteUsage = true,
                DefaultValueMethod = "DefaultTargetObject")]
            ObjectTargeted targetedObjects,
            [CommandParameter(Help = "The type of object to look for",
                AutoCompleteMethodName = "SelectStringType",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            Type objectType,
            [CommandParameter(Help = "The name of the field to look for",
                AutoCompleteMethodName = "SelectIntField",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            string fieldName,
            CompareMethod compare = CompareMethod.EQUAL, int value = 0, [CommandParameter(
                Help = "How to combine the search with the current selection", OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            bool Contains(object found, object objectvalueWanted)
            {
                int foundFloat = (int) found;
                int valueWanted = (int) objectvalueWanted;

                switch (compare)
                {
                    case CompareMethod.EQUAL:
                        return Mathf.Approximately(foundFloat, valueWanted);
                    case CompareMethod.GREATER:
                        return foundFloat > valueWanted;
                    case CompareMethod.GREATER_OR_EQUAL:
                        return foundFloat >= valueWanted;
                    case CompareMethod.LESSER:
                        return foundFloat < valueWanted;
                    case CompareMethod.LESSER_OR_EQUAL:
                        return foundFloat <= valueWanted;
                    default:
                        return false;
                }
            }

            SelectObjectWithMembersWithValue(objectType, fieldName, value, targetedObjects, Contains, comb);
        }

        public static StaticCommandParameterAutoComplete SelectIntField()
        {
            return AutoCompleteManager.FieldAutoComplete(1, typeof(int));
        }


        [Command("Select Objects By Double Value",
            "Selects all the object with a defined double field compared to a double value", QuickName = "SODV",
            Category = "Selection")]
        public static void SelectObjectsWithDoubleValue(
            [CommandParameter(Help = "Should the command included only scene objects or more",
                ForceAutoCompleteUsage = true,
                DefaultValueMethod = "DefaultTargetObject")]
            ObjectTargeted targetedObjects,
            [CommandParameter(Help = "The type of object to look for",
                AutoCompleteMethodName = "SelectStringType",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            Type objectType,
            [CommandParameter(Help = "The name of the field to look for",
                AutoCompleteMethodName = "SelectDoubleField",
                ForceAutoCompleteUsage = true,
                PreventDefaultValueUsage = true)]
            string fieldName, CompareMethod compare = CompareMethod.EQUAL,
            double value = 0, [CommandParameter(Help = "How to combine the search with the current selection",
                OverrideName = "Selection Combining")]
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            bool Contains(object found, object objectvalueWanted)
            {
                double foundFloat = (double) found;
                double valueWanted = (double) objectvalueWanted;

                switch (compare)
                {
                    case CompareMethod.EQUAL:
                        return Mathf.Approximately((float) foundFloat, (float) valueWanted);
                    case CompareMethod.GREATER:
                        return foundFloat > valueWanted;
                    case CompareMethod.GREATER_OR_EQUAL:
                        return foundFloat >= valueWanted;
                    case CompareMethod.LESSER:
                        return foundFloat < valueWanted;
                    case CompareMethod.LESSER_OR_EQUAL:
                        return foundFloat <= valueWanted;
                    default:
                        return false;
                }
            }

            SelectObjectWithMembersWithValue(objectType, fieldName, value, targetedObjects, Contains, comb);
        }

        public static StaticCommandParameterAutoComplete SelectDoubleField()
        {
            return AutoCompleteManager.FieldAutoComplete(1, typeof(double));
        }

        public static ObjectTargeted DefaultTargetObject()
        {
            return ObjectTargeted.SCENE;
        }


        public static void SelectObjectWithMembersWithValue(Type objectType, string fieldName,
            object value, ObjectTargeted targetedObjects = ObjectTargeted.SCENE,
            Func<object, object, bool> validityTest = null,
            SelectionCombineRestrictedCombine comb = SelectionCombineRestrictedCombine.REPLACE)
        {
            var objs = Resources.FindObjectsOfTypeAll(objectType);

            PropertyInfo propInfo = null;
            var info = objectType.GetField(fieldName, TypeManager.AllInstanceFlags);
            if (info == null)
                propInfo = objectType.GetProperty(fieldName,
                    TypeManager.AllInstanceFlags);

            bool IsEqual(object a, object b)
            {
                if (a != null)
                    return a.Equals(b);
                return b == null;
            }

            if (validityTest == null)
            {
                validityTest = IsEqual;
            }

            List<Object> selection = new List<Object>();

            foreach (var obj in objs)
            {
                if (info != null)
                {
                    if (validityTest(info.GetValue(obj), value))
                    {
                        if (obj is Component comp)
                            selection.Add(comp.gameObject);
                        else
                            selection.Add(obj);
                    }
                }
                else if (propInfo != null)
                {
                    if (validityTest(propInfo.GetValue(obj, null), value))
                    {
                        if (obj is Component comp)
                            selection.Add(comp.gameObject);
                        else
                            selection.Add(obj);
                    }
                }
            }

            var prev = Selection.objects;
            Selection.objects = selection.ToArray();

            switch (targetedObjects)
            {
                case ObjectTargeted.SCENE:
                    UnSelectAssets();
                    break;
                case ObjectTargeted.ASSET:
                    UnSelectSceneObjects();
                    break;
            }

            var news = Selection.objects;
            Selection.objects = prev;
            SelectionCombine(comb, news);
        }
    }
}