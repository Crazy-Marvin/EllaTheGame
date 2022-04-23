using MonKey.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonKey.Editor.Commands
{
    public static class VisibilityUtilities
    {
        [Command("Lock / Unlock Inspector",
            Help = "Locks / Unlocks the first selected object's inspector", AlwaysShow = true,
            QuickName = "LI", MenuItemLinkTypeOwner = "MonkeyMenuItems", MenuItemLink = "ToggleLock",
            Category = "Dev")]
        public static void ToggleLock()
        {
            Object[] previousSelection = Selection.objects;
            Object go = DefaultValuesUtilities.DefaultFirstObjectSelected();

            if (go)
            {
#if UNITY_2017_1_OR_NEWER
                Selection.SetActiveObjectWithContext(go, go);
#else
                Selection.activeObject = go;
#endif
            }

            var tracker = ActiveEditorTracker.sharedTracker;
            tracker.isLocked = !tracker.isLocked;
            tracker.ForceRebuild();
            Selection.objects = previousSelection;
            EditorWindow.mouseOverWindow.Repaint();
            EditorWindow.focusedWindow.Repaint();
        }


        [Command("Clear Console", "Clears the editor console", QuickName = "CLC",
            Category = "Dev")]
        public static void ClearConsole()
        {
#if UNITY_2017_1_OR_NEWER
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            if (method != null) method.Invoke(new object(), null);

#elif UNITY_5_3_OR_NEWER
            Assembly assembly = Assembly.GetAssembly (typeof(SceneView));
            Type logEntries = assembly.GetType ("UnityEditorInternal.LogEntries");
            MethodInfo clearConsoleMethod = logEntries.GetMethod ("Clear");
            if (clearConsoleMethod != null)
                clearConsoleMethod.Invoke(new object(), null);
#endif
        }


        [Command("Enable / Disable Hierarchy",
            AlwaysShow = true, Order = -5,
            Help = "Toggles Selection Active/Inactive (by default toggles hierarchy recursively)",
            ValidationMethodName = "ValidationSelection",
            QuickName = "ED", MenuItemLink = "Tenable", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Hierarchy")]
        public static void ToggleEnable()
        {
            ToggleObjects(true, Selection.gameObjects);
        }

        private static void ToggleObjects(bool recursive = true, params GameObject[] objects)
        {
            bool[] previousActivation = objects.Convert(_ => _.activeInHierarchy).ToArray();

            int i = 0;
            int undoID = MonkeyEditorUtils.CreateUndoGroup("Activate / Deactivate");
            foreach (var gameObject in objects)
            {
                Undo.RecordObject(gameObject, "activation");
                if (gameObject.activeInHierarchy && previousActivation[i])
                {
                    gameObject.SetActive(false);
                    continue;
                }

                gameObject.SetActive(true);
                if (recursive && gameObject.transform.parent
                              && !gameObject.transform.parent.gameObject.activeInHierarchy)
                    ToggleObjects(true, gameObject.transform.parent.gameObject);
                i++;
            }

            Undo.CollapseUndoOperations(undoID);
        }

        [CommandValidation(DefaultValidationMessages.DEFAULT_SELECTED_GAMEOBJECTS)]
        private static bool ValidationSelection()
        {
            return (Selection.activeGameObject != null);
        }

        //#if !UNITY_2018_1_OR_NEWER
        [Command("Expand All Children", QuickName = "EA", AlwaysShow = true, Order = -5,
            Help = "Expands all the children of the selected game objects in the hierarchy",
            MenuItemLink = "ExpandAll", MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Hierarchy")]
        public static void ExpandAllChildren()
        {
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            var methodInfo = type.GetMethod("SetExpandedRecursive");

#if UNITY_2018_2_OR_NEWER
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
#else
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
#endif
            EditorUtility.DisplayProgressBar("Expanding All Children", "Please Wait, Monkey Is Expending Some Object",
                .5f);

            GameObject[] selection = Selection.gameObjects;

            if (selection == null || selection.Length == 0)
                selection = TransformExt.GetAllTransformedOrderUpToDown().Convert(_ => _.gameObject).ToArray();

            var window = EditorWindow.focusedWindow;
            if (methodInfo != null)
                foreach (GameObject go in selection)
                {
                    methodInfo.Invoke(window, new object[] {go.GetInstanceID(), true});
                }

            EditorUtility.ClearProgressBar();
        }


        [Command("Collapse All", QuickName = "CA",
            Help = "Collapses all the selected objects and their children",
            MenuItemLink = "CollapseAll",
            MenuItemLinkTypeOwner = "MonkeyMenuItems",
            Category = "Hierarchy")]
        public static void CollapseAll()
        {
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            var methodInfo = type.GetMethod("SetExpandedRecursive");

#if UNITY_2018_2_OR_NEWER
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
#else
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
#endif
            EditorUtility.DisplayProgressBar("Collapsing All Children", "Please Wait, Monkey Is Collapsing Some Object",
                .5f);

            GameObject[] selection = Selection.transforms.Convert(_ => _.gameObject).ToArray();

            if (selection == null || selection.Length == 0)
                selection = TransformExt.GetAllTransformedOrderUpToDown().Convert(_ => _.gameObject).ToArray();

            var window = EditorWindow.focusedWindow;
            if (methodInfo != null)
                foreach (GameObject t in selection)
                {
                    methodInfo.Invoke(window, new object[] {t.GetInstanceID(), false});
                }

            EditorUtility.ClearProgressBar();
        }
        //#endif

        [Command("Show Bounding Boxes", "Shows the bounding boxes of the selected objects",
            Category = "Physics")]
        public static void ShowBoundingBox()
        {
            MonkeyEditorUtils.AddSceneCommand(new ShowBoundingBoxSceneCommand(Selection.gameObjects.ToList(), 0));
        }

        public class ShowBoundingBoxSceneCommand : TimedSceneCommand
        {
            private bool displayMeasures = true;
            private List<GameObject> objects;

            private Bounds[] boundsPerObjects;

            public ShowBoundingBoxSceneCommand(List<GameObject> objects, float duration) : base(duration)
            {
                SceneCommandName = "Show Bounding Box";
                this.objects = objects;

                boundsPerObjects = new Bounds[objects.Count];

                for (int i = 0; i < objects.Count; i++)
                {
                    Bounds bounds = new Bounds();

                    foreach (var renderer in objects[i].GetComponentsInChildren<Renderer>())
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }

                    boundsPerObjects[i] = bounds;
                }
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();
                DisplayBoolOption("Display Measurement", ref displayMeasures);
                DisplayObjectListOption("Object To Bound", objects);
            }

            public override void OnSceneGUI()
            {
                base.OnSceneGUI();
                int i = 0;

                for (int j = 0; j < objects.Count; j++)
                {
                    Bounds bounds = new Bounds();

                    foreach (var renderer in objects[j].GetComponentsInChildren<Renderer>())
                    {
                        if (bounds.extents == Vector3.zero)
                            bounds = renderer.bounds;
                        else
                            bounds.Encapsulate(renderer.bounds);
                    }

                    boundsPerObjects[j] = bounds;
                }

                foreach (var bounds in boundsPerObjects)
                {
                    Vector3 v3Center = bounds.center;
                    Vector3 v3Extents = bounds.extents;

                    var v3FrontTopLeft = new Vector3(v3Center.x - v3Extents.x,
                        v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);
                    var v3FrontTopRight = new Vector3(v3Center.x + v3Extents.x,
                        v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);
                    var v3FrontBottomLeft = new Vector3(v3Center.x - v3Extents.x,
                        v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);
                    var v3FrontBottomRight = new Vector3(v3Center.x + v3Extents.x,
                        v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);
                    var v3BackTopLeft = new Vector3(v3Center.x - v3Extents.x,
                        v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);
                    var v3BackTopRight = new Vector3(v3Center.x + v3Extents.x,
                        v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);
                    var v3BackBottomLeft = new Vector3(v3Center.x - v3Extents.x,
                        v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);
                    var v3BackBottomRight = new Vector3(v3Center.x + v3Extents.x,
                        v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);

                    Handles.color = Color.red;
                    Handles.Disc(Quaternion.identity, v3Center, Vector3.up, 2, false, 0);
                    Handles.color = Color.white;

                    Handles.color = Color.red;

                    Handles.DrawLine(v3FrontTopLeft, v3FrontTopRight);

                    Handles.color = Color.green;

                    Handles.DrawLine(v3FrontTopRight, v3FrontBottomRight);
                    Handles.color = Color.white;

                    Handles.DrawLine(v3FrontBottomRight, v3FrontBottomLeft);
                    Handles.DrawLine(v3FrontBottomLeft, v3FrontTopLeft);

                    Handles.DrawLine(v3BackTopLeft, v3BackTopRight);
                    Handles.DrawLine(v3BackTopRight, v3BackBottomRight);
                    Handles.DrawLine(v3BackBottomRight, v3BackBottomLeft);
                    Handles.DrawLine(v3BackBottomLeft, v3BackTopLeft);

                    Handles.DrawLine(v3FrontTopLeft, v3BackTopLeft);

                    Handles.color = Color.blue;

                    Handles.DrawLine(v3FrontTopRight, v3BackTopRight);
                    Handles.color = Color.white;

                    Handles.DrawLine(v3FrontBottomRight, v3BackBottomRight);
                    Handles.DrawLine(v3FrontBottomLeft, v3BackBottomLeft);

                    Handles.color = Color.black;

                    Handles.color = Color.red;

                    Vector3 pos = (v3FrontTopLeft + v3FrontTopRight) / 2;
                    float value = (v3FrontTopLeft - v3FrontTopRight).magnitude;

                    Handles.Label(pos, value.ToString(CultureInfo.InvariantCulture).Colored(Color.red),new GUIStyle()
                    {
                        richText = true,
                    });

                    Handles.color = Color.green;

                    pos = (v3FrontTopRight + v3FrontBottomRight) / 2;
                    value = (v3FrontTopRight - v3FrontBottomRight).magnitude;


                    Handles.Label(pos, value.ToString(CultureInfo.InvariantCulture).Colored(Color.green),new GUIStyle()
                    {
                        richText = true,
                    });

                    Handles.color = Color.blue;

                    pos = (v3FrontTopRight + v3BackTopRight) / 2;
                    value = (v3FrontTopRight - v3BackTopRight).magnitude;

                    Handles.Label(pos, value.ToString(CultureInfo.InvariantCulture).Colored(Color.blue),new GUIStyle()
                    {
                        richText = true,
                    });

                    i++;
                }
            }
        }
    }
}