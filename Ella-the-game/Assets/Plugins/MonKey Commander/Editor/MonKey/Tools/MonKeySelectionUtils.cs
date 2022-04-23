using MonKey.Editor.Commands;
using MonKey.Extensions;
using MonKey.Settings.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

public class MonKeySelectionUtils
{
    public static readonly List<Object> SelectionSave = new List<Object>();
    public static readonly List<Object> PreviousSelectionStack = new List<Object>();
    
    
    public static readonly List<Object> SelectionStack = new List<Object>();
    public static GameObject LastGOSelected;
    
    
    public static int CurrentSelectionCount = 0;

    public static List<Object> OrderedObjects => SelectionStack;

    public static List<GameObject> OrderedGameObjects
    {
        get
        {
            return SelectionStack.Where(_ => _ is GameObject)
                .Convert(_ => _ as GameObject).ToList();
        }
    }

    public static List<Transform> OrderedTransforms
    {
        get
        {
            return SelectionStack.Where(_ => _ is GameObject)
                .Convert(_ => _ as GameObject).Where(_ => _.scene.IsValid()).Convert(_ => _.transform).ToList();
        }
    }

    [InitializeOnLoadMethod]
    public static void PluginImporter()
    {
        SelectionStack.AddRange(Selection.objects);
        PreviousSelectionStack.Clear();
        PreviousSelectionStack.AddRange(Selection.objects);
        Selection.selectionChanged -= UpdateOrderedSelection2;
        Selection.selectionChanged += UpdateOrderedSelection2;
    }

    public static void UpdateOrderedSelection()
    {
        if (!InitializeSelectionUpdate()) return;

        if (Selection.objects.Length < OrderedObjects.Count)
        {
            //then one objects wha removed using ctrl click
            HandleObjectRemoval();
        }
        else if (Selection.objects.Length == OrderedObjects.Count + 1)
        {
            //then one object was added with click
            HandleOneObjectAdded();
        }
        else
        {
            //then several objects were added with shift+click
            HandleSeveralObjectsAdded();
        }
    }

    private static void HandleSeveralObjectsAdded()
    {
      /*  var addedOjects = FindNewObjects();
        var addedGameObject = addedOjects.Where(_ =>!IsAsset(_)).Cast<GameObject>();
        var addedAsset = addedOjects.Where(IsAsset);
        var orderedGameObjects = addedGameObject.OrderBy(CalculateOrder);
        if()*/
    }

    private static float CalculateOrder(GameObject obj)
    {
        float order = obj.transform.GetSiblingIndex();

        int digit = 1;

        Transform parent = obj.transform.parent;

        while (parent)
        {
            order += parent.GetSiblingIndex() * digit;
            digit++;
            parent = parent.parent;
        }

        order /= (digit);
        return order;
    }

    private static void HandleOneObjectAdded()
    {
        var addedOjects = FindNewObjects();

        Debug.Assert(addedOjects.Count == 1,
            "MonKey Error: something went wrong with what Unity did with the selection");
        var obj = addedOjects.First();

        SelectionStack.Add(obj);
    }

    private static bool IsAsset(Object obj)
    {
        var go = obj as GameObject;
        bool isPrefabStage = PrefabStageUtility.GetCurrentPrefabStage() != null;
        if (go && (go.scene.IsValid()
                   || (isPrefabStage && PrefabStageUtility.GetCurrentPrefabStage().scene == go.scene)))
        {
            return false;
        }

        return true;
    }

    private static List<Object> FindNewObjects()
    {
        var newObjs = new List<Object>();
        foreach (Object o in Selection.objects)
        {
            if (SelectionStack.Contains(o))
                continue;
            newObjs.Add(o);
        }

        return newObjs;
    }

    private static bool InitializeSelectionUpdate()
    {
        EditorUtility.ClearProgressBar();

        if (!MonKeyInternalSettings.Instance || !MonKeyInternalSettings.Instance.UseSortedSelection)
            return false;

        if (Selection.objects.Length > MonKeyInternalSettings.Instance.MaxSortedSelectionSize &&
            MonKeyInternalSettings.Instance.ShowSortedSelectionWarning)
        {
            Debug.Log("MonKey Info: " +
                      "You selected more objects than the limit set in settings for the maximum amount of ordered objects:" +
                      " the selection's order won't be guaranteed. ");
            Debug.Log("...You can change that amount in the settings, but keep in mind that trying to " +
                      "used ordered selection on large amount of objects will require some " +
                      "calculation time.");
            Debug.Log("You can disable that warning in the settings as well.");

            SelectionStack.Clear();
            SelectionStack.AddRange(Selection.objects);

            return false;
        }

        if (Selection.objects.Length > 50)
        {
            EditorUtility.DisplayProgressBar("MonKey Is Updating Ordered Selection, Please Wait",
                "Making sure the selection is properly ordered. If this is getting too long, you can deactivate that  ",
                0.5f);
        }

        return true;
    }

    private static void HandleObjectRemoval()
    {
        throw new System.NotImplementedException();
    }

    public static void UpdateOrderedSelection2()
    {
        /* PreviousSelectionStack.Clear();
         PreviousSelectionStack.AddRange(Selection.objects);*/

        if (Selection.objects.Length > 0)
        {
            PreviousSelectionStack.Clear();
            PreviousSelectionStack.AddRange(Selection.objects);
        }

        if (Selection.objects == null)
        {
            SelectionStack.Clear();
            return;
        }

        EditorUtility.ClearProgressBar();

        if (!MonKeyInternalSettings.Instance || !MonKeyInternalSettings.Instance.UseSortedSelection)
            return;

        if (Selection.objects.Length > MonKeyInternalSettings.Instance.MaxSortedSelectionSize &&
            MonKeyInternalSettings.Instance.ShowSortedSelectionWarning)
        {
            Debug.Log("MonKey Info: " +
                      "You selected more objects than the limit set in settings for the maximum amount of ordered objects:" +
                      " the selection's order won't be guaranteed. ");
            Debug.Log("...You can change that amount in the settings, but keep in mind that trying to " +
                      "used ordered selection on large amount of objects will require some " +
                      "calculation time.");
            Debug.Log("You can disable that warning in the settings as well.");

            SelectionStack.Clear();
            SelectionStack.AddRange(Selection.objects);

            return;
        }

        if (Selection.objects.Length > 50)
        {
            EditorUtility.DisplayProgressBar("MonKey Is Updating Ordered Selection, Please Wait",
                "Making sure the selection is properly ordered. If this is getting too long, you can deactivate that  ",
                0.5f);
        }

        //if there is a current selection and it has only one object now or none,
        //then the selection can be considered changed and we can store the previous selection
        if (Selection.objects.Length <= 1)
        {
            if (SelectionStack.Count >= 1)
            {
                SelectionStack.Clear();
            }

            if (Selection.objects.IsEmpty())
            {
                CurrentSelectionCount = 0;
                LastGOSelected = null;
            }
            else
            {
                SelectionStack.AddRange(Selection.objects);
                CurrentSelectionCount = 1;
                if (Selection.objects[0] is GameObject)
                {
                    LastGOSelected = (GameObject) Selection.objects[0];
                }
            }
        }
        else
        {
            //in that case we need to check the differences and adjust the ordered selection accordingly

            if (Selection.objects.Length <= CurrentSelectionCount)
            {
                //if so, it means that some objects were removed,
                //we need to check which ones and remove them

                for (int j = SelectionStack.Count - 1; j >= 0; j--)
                {
                    if (!Selection.objects.Contains(SelectionStack[j]))
                    {
                        SelectionStack.RemoveAt(j);
                        CurrentSelectionCount--;
                    }
                }
            }
            else
            {
                //then some objects were added, and must be sorted and added.
                //We first must find the objects that were added
                List<GameObject> newGOs = new List<GameObject>();
                List<Object> newAssets = new List<Object>();

                for (int j = 0; j < Selection.objects.Length; j++)
                {
                    var obj = Selection.objects[j];
                    if (!SelectionStack.Contains(obj))
                    {
                        var go = obj as GameObject;
                        bool isPrefabStage = PrefabStageUtility.GetCurrentPrefabStage() != null;
                        if (go && (go.scene.IsValid()
                                   || (isPrefabStage && PrefabStageUtility.GetCurrentPrefabStage().scene == go.scene)))
                        {
                            newGOs.Add(go);
                        }
                        else
                        {
                            newAssets.Add(obj);
                        }
                    }
                }


                CurrentSelectionCount += newGOs.Count + newAssets.Count;
                newAssets.Sort(new NamingUtilities.ObjectComparer());

                List<GameObject> orderedObjects = new List<GameObject>();
                Dictionary<GameObject, List<int>> orderIDs = new
                    Dictionary<GameObject, List<int>>(orderedObjects.Count);
                List<int> idsOfLastSelected = new List<int>();
                //now we need to determine the order
                for (int i = 0; i < newGOs.Count; i++)
                {
                    var obj = newGOs[i];
                    orderIDs.Add(obj, new List<int>());
                    var parentTransform = obj.transform.parent;
                    orderIDs[obj].Add(obj.transform.GetSiblingIndex());
                    if (parentTransform)
                    {
                        while (parentTransform)
                        {
                            if (orderIDs[obj].Count > 0)
                                orderIDs[obj].Insert(0, parentTransform.GetSiblingIndex());
                            else
                            {
                                orderIDs[obj].Add(parentTransform.GetSiblingIndex());
                            }

                            parentTransform = parentTransform.parent;
                        }
                    }
                }


                if (LastGOSelected)
                {
                    var lastParentTransform = LastGOSelected.transform.parent;
                    if (!lastParentTransform)
                    {
                        idsOfLastSelected.Add(LastGOSelected.transform.GetSiblingIndex());
                    }
                    else
                    {
                        while (lastParentTransform)
                        {
                            idsOfLastSelected.Add(lastParentTransform.GetSiblingIndex());
                            lastParentTransform = lastParentTransform.parent;
                        }
                    }
                }

                //now that we have all the orders computed, we can order the objects
                for (int i = 0; i < newGOs.Count; i++)
                {
                    if (orderedObjects.Count == 0)
                    {
                        orderedObjects.Add(newGOs[i]);
                    }
                    else
                    {
                        int insertID = -1;
                        var newGo = newGOs[i];
                        var newIds = orderIDs[newGo];

                        for (int j = 0; j < orderedObjects.Count; j++)
                        {
                            var ids = orderIDs[orderedObjects[j]];

                            for (int k = 0; k < ids.Count; k++)
                            {
                                if (newIds.Count <= k)
                                    break;

                                int newID = newIds[k];
                                if (ids[k] > newID)
                                {
                                    //then we can insert before
                                    insertID = j;
                                    break;
                                }

                                if (ids[k] == newID)
                                {
                                    //then we need to check further, or if there isn't left just add right after
                                    if (k == ids.Count - 1)
                                    {
                                        break;
                                    }

                                    if (newIds.Count == k + 1)
                                    {
                                        //this means it's the parent of the object
                                        insertID = j;
                                        break;
                                    }
                                }
                                else
                                {
                                    // then we are after, and can break to compare to the next one
                                    break;
                                }
                            }

                            if (insertID != -1)
                            {
                                break;
                            }
                        }

                        if (insertID != -1 && insertID < orderedObjects.Count)
                        {
                            orderedObjects.Insert(insertID, newGOs[i]);
                        }
                        else
                        {
                            orderedObjects.Add(newGOs[i]);
                        }
                    }
                }

                bool top = false;
                for (int i = 0; i < idsOfLastSelected.Count; i++)
                {
                    if (orderedObjects.Count == 0)
                        break;

                    var list = orderIDs[orderedObjects[0]];

                    if (list.Count <= i)
                        break;

                    if (idsOfLastSelected[i] < list[i])
                    {
                        top = true;
                        break;
                    }

                    if (idsOfLastSelected[i] > list[i])
                    {
                        top = false;
                        break;
                    }
                }

                if (!top)
                {
                    orderedObjects.Reverse();
                }

                //now that we have our ordered game objects, we can get the last one , and add the assets, and be done!
                if (orderedObjects.Count > 0)
                {
                    LastGOSelected = orderedObjects.Last();
                }

                SelectionStack.AddRange(orderedObjects.Convert(_ => _ as Object));
                SelectionStack.AddRange(newAssets);
            }
        }

        EditorUtility.ClearProgressBar();
    }
}