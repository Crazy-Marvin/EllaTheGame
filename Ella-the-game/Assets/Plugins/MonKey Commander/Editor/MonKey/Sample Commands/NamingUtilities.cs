using MonKey.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonKey.Editor.Commands
{
    public static class NamingUtilities
    {
        [Command("Sort By Name", "Sort all the selected GameObjects by name in the hierarchy",
            DefaultValidation = DefaultValidation.AT_LEAST_TWO_GAME_OBJECTS, QuickName = "SN",
            Category = "Naming")]
        public static void SortByName()
        {
            List<GameObject> freeObjects = new List<GameObject>();
            Dictionary<Transform, List<GameObject>> selectedObjectsByParent
                = new Dictionary<Transform, List<GameObject>>();

            int undoID = MonkeyEditorUtils.CreateUndoGroup("Order Siblings");
            foreach (GameObject gameObject in Selection.gameObjects)
            {
                Transform parent = gameObject.transform.parent;
                if (parent == null)
                    freeObjects.Add(gameObject);
                else if (selectedObjectsByParent.ContainsKey(parent))
                {
                    selectedObjectsByParent[parent].Add(gameObject);
                }
                else
                {
                    selectedObjectsByParent.Add(parent, new List<GameObject>());
                    selectedObjectsByParent[parent].Add(gameObject);
                }
            }

            SortSiblings(freeObjects);
            foreach (List<GameObject> gameObjects in selectedObjectsByParent.Values)
            {
                SortSiblings(gameObjects);
            }

            Undo.CollapseUndoOperations(undoID);
        }

        private static void SortSiblings(List<GameObject> siblings)
        {
            siblings.Sort(new GameObjectComparer());
            for (int i = 0; i < siblings.Count; i++)
            {
                if (siblings[i].transform.parent)
                    Undo.RegisterFullObjectHierarchyUndo(siblings[i].transform.parent,
                        "Sibling Index");
                else
                {
                    Undo.RegisterFullObjectHierarchyUndo(siblings[i], "Sibling Index");
                }

                siblings[i].transform.SetSiblingIndex(i);
            }
        }

        [Command("Nicify Name",
            "Changes a c# name type to a name containing spaces and easier to read",
            QuickName = "NN", DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT,
            Category = "Naming")]
        public static void NicifyName()
        {
            List<string> newNames = new List<string>();

            foreach (var o in MonkeyEditorUtils.OrderedSelectedObjects)
            {
                newNames.Add(o.name.NicifyVariableName());
            }

            RenameSelection(newNames.ToArray());
        }

        [Command("Remove Unity Duplicate Terms",
            "Removes the terms created in the names of the selected objects when you duplicate an object",
            QuickName = "RUD", DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT,
            Category = "Naming")]
        public static void RenameRemoveUnityDuplicateTerms()
        {
            List<string> newNames = new List<string>();

            foreach (var o in MonkeyEditorUtils.OrderedSelectedObjects)
            {
                newNames.Add(StringExt.RemoveUnityCloneTerms(o.name));
            }

            RenameSelection(newNames.ToArray());
        }

        [Command("Rename Replace Term", "Replaces a term by another one in the names of the objects selected",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT, QuickName = "RRT",
            MenuItemLink = "RenameReplace", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Naming")]
        public static void ReplaceNameTerm(
            [CommandParameter(Help = "The term to look for in the name")]
            string termToLookFor,
            [CommandParameter(Help = "The text that will replace the " +
                                     "term you are looking for (by default will just remove it)")]
            string toReplaceWith = "",
            [CommandParameter(Help = "if true, will consider capital letters as normal letters")]
            bool ignoreCase = true)
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Replace in Names");
            int i = 0;
            foreach (Object o in MonkeyEditorUtils.OrderedSelectedObjects)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Replacing Terms in name...",
                    "MonKey is renaming the objects, please wait!",
                    (float) i / MonkeyEditorUtils.OrderedSelectedObjects.Count()))
                    break;

                Undo.RecordObject(o, "renaming");

                string finalReplacement = toReplaceWith;

                if (AssetDatabase.Contains(o))
                {
                    finalReplacement = finalReplacement.GetSafeForFileName();
                }

                o.RenameObjectAndAsset(o.name.Replace(termToLookFor, finalReplacement, ignoreCase));
                i++;
            }

            EditorUtility.ClearProgressBar();

            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Rename", "Renames all the selected objects with new names specified",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT, QuickName = "R",
            MenuItemLink = "RenameSelection", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Naming")]
        public static void RenameSelection(
            [CommandParameter(Help = "The Names to use (if less names than select objects, will loop)")]
            string[] newNames)
        {
            int i = 0;
            int j = 0;
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Selection Rename");
            foreach (var o in MonkeyEditorUtils.OrderedSelectedObjects)
            {
                if (!o)
                    continue;

                if (i >= newNames.Length)
                    i = 0;

                if (EditorUtility.DisplayCancelableProgressBar("Renaming Selection...",
                    "MonKey is renaming the objects, please wait!",
                    (float) j / MonkeyEditorUtils.OrderedSelectedObjects.Count()))
                    break;

                Undo.RecordObject(o, "renaming");

                o.RenameObjectAndAsset(newNames[i]);

                i++;
                j++;
            }

            EditorUtility.ClearProgressBar();

            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Fix Asset Names", QuickName = "FAN",
            Help = "Renames the assets selected so that the file name is the same as the object name",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT,
            Category = "Naming")]
        public static void FixAssetNames()
        {
            int i = 0;
            foreach (var o in Selection.objects)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Fixing Asset Names...",
                    "MonKey is renaming the objects, please wait!",
                    (float) i / Selection.objects.Count()))
                    break;

                if (AssetDatabase.Contains(o))
                {
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(o), o.name);
                }

                i++;
            }

            EditorUtility.ClearProgressBar();
        }

        [Command("Rename Replace Terms Progressive",
            "Replaces a term by a different one for each selected objects names", QuickName = "RPT",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT,
            Category = "Naming")]
        public static void ReplaceNameTerm(
            [CommandParameter(Help = "The term to look for in the name")]
            string termToLookFor,
            [CommandParameter(Help = "The texts that will replace the " +
                                     "term, one by one (and then looped to the start if less terms than objects ")]
            string[] toReplaceWith,
            [CommandParameter(Help = "if true, will consider capital letters as normal letters")]
            bool ignoreCase = true)
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Replace in Names");
            int i = 0;
            int j = 0;

            foreach (Object o in MonkeyEditorUtils.OrderedSelectedObjects)
            {
                Undo.RecordObject(o, "renaming");

                if (i >= toReplaceWith.Length)
                    i -= toReplaceWith.Length;

                if (EditorUtility.DisplayCancelableProgressBar("Renaming Selection...",
                    "MonKey is renaming the objects, please wait!",
                    (float) j / MonkeyEditorUtils.OrderedSelectedObjects.Count()))
                    break;

                string finalReplacement = toReplaceWith[i];

                if (AssetDatabase.Contains(o))
                    finalReplacement = finalReplacement.GetSafeForFileName();

                o.RenameObjectAndAsset(o.name.Replace(termToLookFor, finalReplacement, ignoreCase));

                i++;
                j++;
            }

            EditorUtility.ClearProgressBar();
            Undo.CollapseUndoOperations(undoID);
        }


        [Command("Rename From Last", "Replaces text from the last occurrence of a specified term",
            QuickName = "RL", DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT,
            Category = "Naming")]
        public static void ReplaceNameFromLast(
            [CommandParameter(Help = "The last tern from which the replacement will apply (term included)")]
            string lastTermSeparator = " ",
            [CommandParameter(Help = "The text to replace the end of the name with")]
            string replacement = "")
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Replace after in Names");
            int i = 0;
            foreach (Object o in MonkeyEditorUtils.OrderedSelectedObjects)
            {
                Undo.RecordObject(o, "renaming");

                if (EditorUtility.DisplayCancelableProgressBar("Renaming Selection...",
                    "MonKey is renaming the objects, please wait!",
                    (float) i / MonkeyEditorUtils.OrderedSelectedObjects.Count()))
                    break;

                string finalReplacement = o.name.ReplaceFromLast(lastTermSeparator, replacement);

                if (AssetDatabase.Contains(o))
                    finalReplacement = finalReplacement.GetSafeForFileName();

                o.RenameObjectAndAsset(finalReplacement);
                i++;
            }

            EditorUtility.ClearProgressBar();
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Rename After Last", "Replaces text after the last occurrence of a specified term",
            QuickName = "RAL", DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT,
            Category = "Naming")]
        public static void ReplaceNameAfterLast(
            [CommandParameter(Help = "The last term after which the replacement will apply (term included)")]
            string lastTermSeparator = " ",
            [CommandParameter(Help = "The text to replace the end of the name with")]
            string replacement = "")
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Replace after in Names");
            int i = 0;
            foreach (Object o in MonkeyEditorUtils.OrderedSelectedObjects)
            {
                Undo.RecordObject(o, "renaming");

                if (EditorUtility.DisplayCancelableProgressBar("Renaming Selection...",
                    "MonKey is renaming the objects, please wait!",
                    (float) i / MonkeyEditorUtils.OrderedSelectedObjects.Count()))
                    break;

                string finalReplacement = o.name.ReplaceAfterLast(lastTermSeparator, replacement);

                if (AssetDatabase.Contains(o))
                    finalReplacement = finalReplacement.GetSafeForFileName();

                o.RenameObjectAndAsset(finalReplacement);
                i++;
            }

            EditorUtility.ClearProgressBar();

            Undo.CollapseUndoOperations(undoID);
        }


        [Command("Rename From First", "Replaces text from the first occurrence of a specified term",
            QuickName = "RF", DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT,
            Category = "Naming")]
        public static void ReplaceNameFromFirst(
            [CommandParameter(Help = "The first term from which the replacement will apply (term included)")]
            string lastTermSeparator = " ",
            [CommandParameter(Help = "The text to replace the end of the name with")]
            string replacement = "")
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Replace after in Names");
            int i = 0;
            foreach (Object o in MonkeyEditorUtils.OrderedSelectedObjects)
            {
                Undo.RecordObject(o, "renaming");

                if (EditorUtility.DisplayCancelableProgressBar("Renaming Selection...",
                    "MonKey is renaming the objects, please wait!",
                    (float) i / MonkeyEditorUtils.OrderedSelectedObjects.Count()))
                    break;

                string finalReplacement = o.name.ReplaceFromFirst(lastTermSeparator, replacement);

                if (AssetDatabase.Contains(o))
                    finalReplacement = finalReplacement.GetSafeForFileName();

                o.RenameObjectAndAsset(finalReplacement);
                i++;
            }

            EditorUtility.ClearProgressBar();

            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Rename Until", QuickName = "RE",
            Help = "Replaces text until the first occurrence of a specified term",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT,
            Category = "Naming")]
        public static void ReplaceNameUntilFirst(
            [CommandParameter(Help = "The first term until which the replacement will apply")]
            string term = " ",
            [CommandParameter(Help = "The text to replace the start of the name with")]
            string replacement = "")
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Replace before in Names");
            int i = 0;
            foreach (Object o in MonkeyEditorUtils.OrderedSelectedObjects)
            {
                Undo.RecordObject(o, "renaming");

                if (EditorUtility.DisplayCancelableProgressBar("Renaming Selection...",
                    "MonKey is renaming the objects, please wait!",
                    (float) i / MonkeyEditorUtils.OrderedSelectedObjects.Count()))
                    break;

                string finalReplacement = o.name.ReplaceUntil(term, replacement);

                if (AssetDatabase.Contains(o))
                    finalReplacement = finalReplacement.GetSafeForFileName();

                o.RenameObjectAndAsset(finalReplacement);
                i++;
            }

            EditorUtility.ClearProgressBar();
            Undo.CollapseUndoOperations(undoID);
        }


        [Command("Rename Add Prefixes", "Adds prefixes to the names of the selected objects.", QuickName = "RPR",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT,
            Category = "Naming")]
        public static void AddNamePrefixes(
            [CommandParameter(Help = "The prefixes to add to the selected objects." +
                                     " The prefixes will be apply one after the other to the objects" +
                                     " (and loop back to the start)", PreventDefaultValueUsage = true)]
            string[] prefixes)
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Replace after in Names");
            int i = 0;
            int j = 0;
            foreach (Object o in MonkeyEditorUtils.OrderedSelectedObjects)
            {
                Undo.RecordObject(o, "renaming");

                if (i >= prefixes.Length)
                    i -= prefixes.Length;

                if (EditorUtility.DisplayCancelableProgressBar("Renaming Selection...",
                    "MonKey is renaming the objects, please wait!",
                    (float) j / MonkeyEditorUtils.OrderedSelectedObjects.Count()))
                    break;

                string finalReplacement = prefixes[i] + o.name;

                if (AssetDatabase.Contains(o))
                    finalReplacement = finalReplacement.GetSafeForFileName();

                o.RenameObjectAndAsset(finalReplacement);
                i++;
                j++;
            }

            EditorUtility.ClearProgressBar();

            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Rename Add Name Suffixes", "Adds suffixes to the names of the selected objects.",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT, QuickName = "RNS",
            Category = "Naming")]
        public static void AddNameSuffixes(
            [CommandParameter(Help = "The suffixes to add to the selected objects." +
                                     " The suffixes will be apply one after the other to the objects" +
                                     " (and loop back to the start)", PreventDefaultValueUsage = true)]
            string[] suffixes)
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Replace after in Names");
            int i = 0;
            int j = 0;

            foreach (Object o in MonkeyEditorUtils.OrderedSelectedObjects)
            {
                Undo.RecordObject(o, "renaming");

                if (EditorUtility.DisplayCancelableProgressBar("Renaming Selection...",
                    "MonKey is renaming the objects, please wait!",
                    (float) j / MonkeyEditorUtils.OrderedSelectedObjects.Count()))
                    break;

                if (i >= suffixes.Length)
                    i -= suffixes.Length;

                string finalReplacement = o.name + suffixes[i];

                if (AssetDatabase.Contains(o))
                    finalReplacement = finalReplacement.GetSafeForFileName();

                o.RenameObjectAndAsset(finalReplacement);
                i++;
            }

            EditorUtility.ClearProgressBar();
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Rename Add Order Number",
            "Adds a number at the end of the selected objects depending on the order of selection",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT, QuickName = "RON",
            MenuItemLink = "RenameAddOrderNumber", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Naming")]
        public static void RenameAddNumberInOrder(
            [CommandParameter(Help = "The number to start the counting with")]
            int startingNumber = 0,
            [CommandParameter(Help = "the text to include before the number")]
            string beforeNumber = " ",
            [CommandParameter(Help = "the text to include after the number")]
            string afterNumber = "")
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Replace after in Names");
            int i = startingNumber;
            int j = 0;
            foreach (Object o in new List<Object>(MonkeyEditorUtils.OrderedSelectedObjects))
            {
                Undo.RecordObject(o, "renaming");

                if (EditorUtility.DisplayCancelableProgressBar("Renaming Selection...",
                    "MonKey is renaming the objects, please wait!",
                    (float) j / MonkeyEditorUtils.OrderedSelectedObjects.Count()))
                    break;

                o.RenameObjectAndAsset(o.name + beforeNumber + i + afterNumber);

                j++;
                i++;
            }

            EditorUtility.ClearProgressBar();
            Undo.CollapseUndoOperations(undoID);
        }

        [Command("Rename Update Order Number",
            "Updates the numbers in the selected objects' names to reflect the order",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT, QuickName = "RUO",
            MenuItemLink = "RenameUpdateOrderNumber", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Naming")]
        public static void RenameUpdateNumberInOrder(
            [CommandParameter(Help = "The number to start the counting with")]
            int startingNumber = 0)
        {
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Update Numbers in Names");
            int i = startingNumber;
            int j = 0;

            foreach (Object o in MonkeyEditorUtils.OrderedSelectedObjects)
            {
                Undo.RecordObject(o, "renaming");

                if (EditorUtility.DisplayCancelableProgressBar("Renaming Selection...",
                    "MonKey is renaming the objects, please wait!",
                    (float) j / MonkeyEditorUtils.OrderedSelectedObjects.Count()))
                    break;

                string newName = Regex.Replace(o.name, "[0-9]{1,}", i.ToString());

                if (newName == o.name)
                {
                    if (o.name.Contains(i.ToString()))
                        continue;

                    newName = newName + " " + i;
                }

                o.RenameObjectAndAsset(newName);
                i++;
                j++;
            }

            EditorUtility.ClearProgressBar();
            Undo.CollapseUndoOperations(undoID);
        }


        public class ObjectComparer : IComparer<UnityEngine.Object>
        {
            public int Compare(Object x, Object y)
            {
                if (x != null && y != null)
                {
                    bool xIsFolder = !AssetDatabase.GetAssetPath(x).Contains(".");
                    bool yIsFolder = !AssetDatabase.GetAssetPath(y).Contains(".");

                    if ((xIsFolder && yIsFolder) || (!xIsFolder && !yIsFolder))
                    {
                        return EditorUtility.NaturalCompare(x.name, y.name);
                    }

                    return xIsFolder ? -1 : 1;
                }

                return 0;
            }
        }

        public class GameObjectComparer : IComparer<GameObject>
        {
            public int Compare(GameObject x, GameObject y)
            {
                if (x != null && y != null)
                    return EditorUtility.NaturalCompare(x.name, y.name);
                return 0;
            }
        }
    }
}