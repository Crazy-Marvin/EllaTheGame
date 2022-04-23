using MonKey;
using MonKey.Editor.Commands;
using MonKey.Editor.Console;
using MonKey.Editor.Internal;
using MonKey.Extensions;
using MonKey.Internal;
using MonKey.Settings.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace MonKey.Editor
{
    public static class MonkeyEditorUtils
    {
        /*  internal static List<Object> PreviousOrderedSelectedObjects = new List<Object>();
          internal static List<Transform> PreviousOrderedSelectedTransform = new List<Transform>();

          internal static List<Object> OrderedSelectedObjectsInt = new List<Object>();
          internal static List<Transform> OrderedSelectedTransformInt = new List<Transform>();*/

        /// <summary>
        /// While Unity does not keep track of the order with which you selected objects, MonKey does.
        /// This Enumerator can help you retrieve the first object, the last, or anything in between, which can be very useful for custom commands.
        /// </summary>
        public static IEnumerable<GameObject> OrderedSelectedGameObjects
        {
            get
            {
                if (!MonKeyInternalSettings.Instance.UseSortedSelection)
                {
                    if (MonKeyInternalSettings.Instance.ShowSortedSelectionWarning)
                        Debug.LogWarning(" Monkey Warning: A command is attempting to use a sorted selection," +
                                         " but it was disabled in the monkey preferences:" +
                                         " this can lead to unexpected results");
                    return Selection.gameObjects;
                }


                return OrderedSelectedObjects.Where(_ => _ is GameObject)
                    .Convert(_ => _ as GameObject).ToList();
            }
        }

        /// <summary>
        /// The ordered list of objects.
        /// </summary>
        public static IEnumerable<Object> OrderedSelectedObjects
        {
            get
            {
                if (!MonKeyInternalSettings.Instance.UseSortedSelection)
                {
                    if (MonKeyInternalSettings.Instance.ShowSortedSelectionWarning)
                        Debug.LogWarning(" Monkey Warning: A command is attempting to use a sorted selection," +
                                         " but it was disabled in the monkey preferences:" +
                                         " this can lead to unexpected results");
                    return Selection.objects;
                }

                return MonKeySelectionUtils.OrderedObjects;
            }
        }


        public static object GetParsedParameter(int parameterID)
        {
            return CommandConsoleWindow.CurrentPanel.CurrentExecution.GetValueParsed(parameterID);
        }


        /// <summary>
        /// The ordered list of transforms.
        /// Keep in mind that this follows the same logic as Unity's:
        /// the transforms contained are only the uppermost selected: if you select a transform and some of its children,
        /// on the parent will appear in that list.
        /// </summary>
        public static IEnumerable<Transform> OrderedSelectedTransform
        {
            get
            {
                if (!MonKeyInternalSettings.Instance.UseSortedSelection)
                {
                    if (MonKeyInternalSettings.Instance.ShowSortedSelectionWarning)
                        Debug.LogWarning(" Monkey Warning: A command is attempting to use a sorted selection," +
                                         " but it was disabled in the monkey preferences:" +
                                         " this can lead to unexpected results");
                    return Selection.transforms;
                }

                return MonKeySelectionUtils.OrderedTransforms;
            }
        }

        /// <summary>
        /// Keeps a reference to the mouse position so that it may be accessed outside of a GUI call: this can be useful when creating scene commands.
        /// </summary>
        public static Vector2 MousePosition { get; internal set; }

        /// <summary>
        /// Returns a ray that goes from the mouse position onto the scene.
        /// </summary>
        public static Ray MouseSceneRay
        {
            get;
            private set;
            /*   {
                   Vector3 inv = new Vector3(MousePosition.x,
                       +CurrentSceneView.position.height + weirdConst
                       - MousePosition.y);
   
                   return HandleUtility.GUIPointToWorldRay(MousePosition);
                   // return HandleUtility.GUIPointToWorldRay(MousePosition);
                   //  return CurrentSceneView.camera.ScreenPointToRay(inv);
               }*/
        }

        public static Vector2 SceneViewMousePosition { get; private set; }/* new Vector3(MousePosition.x,
            +CurrentSceneView.position.height + weirdConst
            - MousePosition.y)*/ //Input.mousePosition;

        /// <summary>
        /// /Returns the raycasted position of the mouse on the scene,
        /// with the possibility to ignore some objects and to apply an offset.
        /// This method outputs the normal of the collision point as well.
        /// </summary>
        /// <param name="transformsToIgnore"> The objects to ignore when raycasting</param>
        /// <param name="hitOffset">the offset relative to the hit point</param>
        /// <param name="collision"> returns true if the ray found a collision</param>
        /// <param name="normal"> returns the normal vector on the hit point</param>
        /// <returns></returns>
        public static Vector3 GetMouseRayCastedPosition(Transform[] transformsToIgnore, float hitOffset,
            out bool collision, out Vector3 normal, bool ignoreInvisible = false)
        {
            if (!CurrentSceneView)
            {
                CurrentSceneView = SceneView.currentDrawingSceneView;
                if (!CurrentSceneView)
                    CurrentSceneView = SceneView.lastActiveSceneView;
                if (!CurrentSceneView)
                {
                    SceneView.duringSceneGui -= OnSceneViewGUI;
                    SceneView.duringSceneGui += OnSceneViewGUI;
                    collision = false;
                    normal = Vector3.zero;
                    return Vector3.zero;
                }
            }


            Vector3 position = CurrentSceneView.camera.ScreenToWorldPoint(MousePosition);
            normal = Vector3.up;

            float minDistanceToCam = float.MaxValue;
            collision = false;
            foreach (var hit in Physics.RaycastAll(MouseSceneRay))
            {
                if (!ConstrainUtilities.GloballyLockedTransforms.Contains(hit.transform) &&
                    transformsToIgnore != null && transformsToIgnore.Any(_ => _ == hit.transform))
                    continue;

                if (ignoreInvisible && !hit.transform.gameObject.GetComponent<Renderer>() && !hit.transform.gameObject.GetComponent<TerrainCollider>())
                    continue;

                collision = true;
                float distanceToCam = (hit.point - CurrentSceneView.camera.transform.position).magnitude;
                if (minDistanceToCam > distanceToCam)
                {
                    minDistanceToCam = distanceToCam;
                    position = hit.point + hitOffset * hit.normal;
                    normal = hit.normal;
                }
            }

            return position;
        }


        /// <summary>
        /// Keeps a reference to the last Event that was called on GUI, so that you may use it outside of GUI logics.
        /// </summary>
        public static Event LastGlobalEvent { get; private set; }

        /// <summary>
        /// Returns the currently focused scene view.
        /// </summary>
        public static SceneView CurrentSceneView { get; internal set; }

        /// <summary>
        /// Returns the folder that is currently focused in the project window. Keep in mind that this is not the same as the selected folder.
        /// </summary>
        /// <returns></returns>
        public static string GetProjectWindowFocusedFolder()
        {
            MethodInfo info = typeof(ProjectWindowUtil).GetMethod("GetActiveFolderPath",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (info != null)
                return info.Invoke(null, null) as string;
            return "";
        }

        public static double EditorDeltaTime => EditorApplication.timeSinceStartup - lastTimeSinceStartup;
        private static double lastTimeSinceStartup;

        private static int SceneGUIMinWidth = 250;
        private static int SceneGUIMaxWidth = 400;

        private static int SceneGUIMinHeight = 130;
        private static int SceneGUIMaxHeight = 300;

        private static Vector2 scrollPosition = Vector2.zero;

        public static readonly List<SceneCommand> CurrentSceneCommands
            = new List<SceneCommand>();

        private static readonly Dictionary<Action, SceneCommand> DelegateCommands
            = new Dictionary<Action, SceneCommand>();

        private static readonly List<SceneCommand> ToRemove = new List<SceneCommand>();

        private static bool ShouldShowSceneGUI
        {
            get { return CurrentSceneCommands.Any(_ => !_.Hidden); }
        }

        /// <summary>
        ///  Returns the data structure of the command of the specified name.
        /// CommandInfo contains all the information used by MonKey to display and execute the command.
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public static CommandInfo GetCommandInfo(string commandName)
        {
            return CommandManager.Instance.GetCommandInfo(commandName);
        }

        //[InitializeOnLoadMethod]
        internal static void EditorUpdatePlug(CommandManager manager)
        {
#if UNITY_2017_1_OR_NEWER
            EditorApplication.playModeStateChanged -= PlayStateChangeCallBack;
            EditorApplication.playModeStateChanged += PlayStateChangeCallBack;
#else
            EditorApplication.playmodeStateChanged -= PlayStateChangeCallBack;
            EditorApplication.playmodeStateChanged += PlayStateChangeCallBack;
#endif
            EditorApplication.update -= MonKeyEditorUpdate;
            EditorApplication.update += MonKeyEditorUpdate;

#if UNITY_2019
            SceneView.duringSceneGui -= OnSceneViewGUI;
            SceneView.duringSceneGui += OnSceneViewGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneViewGUI;
            SceneView.onSceneGUIDelegate += OnSceneViewGUI;
#endif

            System.Reflection.FieldInfo info = typeof(EditorApplication)
                .GetField("globalEventHandler", BindingFlags.Static
                                                | BindingFlags.NonPublic);

            EditorApplication.CallbackFunction value = (EditorApplication.CallbackFunction)
                info.GetValue(null);

            value -= EditorGlobalEvent;
            value += EditorGlobalEvent;

            info.SetValue(null, value);

            manager.OnCommandLoadingDone += InvokeOnCommandLoadingDone;
            lastTimeSinceStartup = EditorApplication.timeSinceStartup;

            ResetSelectionToFirstSelected();
        }

        private static void ResetSelectionToFirstSelected()
        {
            if (Selection.transforms.Length > 0 || Selection.objects.Length > 0)
            {
                if (Selection.activeContext)
                    Selection.objects = new[] { Selection.activeContext };
                else
                    Selection.objects = new[] { Selection.objects[0] };
            }
        }

        /// <summary>
        ///  If you want to associate reserved hotkeys that are used for something external to MonKey,
        /// you can specify it here, and it will appear visible in the console when a conflict happens.
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="hotKeys"></param>
        public static void AddCustomReservedHotKeys(string commandName, params string[] hotKeys)
        {
            HotKeysManager.RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = commandName,
                HotKey = (new KeyCombination(hotKeys))
            });
        }


#if UNITY_2017_1_OR_NEWER
        private static void PlayStateChangeCallBack(PlayModeStateChange obj)
        {
            foreach (SceneCommand command in CurrentSceneCommands)
            {
                if (command.StopOnPlayStateChange)
                    ToRemove.Add(command);
            }

            RemoveCommandsToBeRemoved();

        }
#else

        private static void PlayStateChangeCallBack()
        {
            foreach (SceneCommand command in CurrentSceneCommands)
            {
                if (command.StopOnPlayStateChange)
                    ToRemove.Add(command);
            }

            RemoveCommandsToBeRemoved();
        }

#endif


        private static void EditorGlobalEvent()
        {
            LastGlobalEvent = Event.current;
        }

        private static void MonKeyEditorUpdate()
        {
            CommandUpdate();
            lastTimeSinceStartup = EditorApplication.timeSinceStartup;
        }

        private static void CommandUpdate()
        {
            RemoveCommandsToBeRemoved();

            if (CurrentSceneCommands == null)
                return;

            foreach (var command in CurrentSceneCommands)
            {
                if (command == null)
                    continue;

                if (command.TimeSinceLastUpdate >= command.TimeBetweenUpdate)
                {
                    command.Update();
                    command.TimeSinceLastUpdate = 0;
                }
                else
                {
                    command.TimeSinceLastUpdate += EditorDeltaTime;
                }
            }
        }

        private static void RemoveCommandsToBeRemoved()
        {
            if (ToRemove == null)
                return;

            foreach (var command in ToRemove)
            {
                CurrentSceneCommands.Remove(command);
            }

            if (ToRemove.Count > 0)
                ToRemove.Clear();
        }

        public static void AddEditorDelegate(Action delegateAction, float timeBetweenUpdate = 0)
        {
            if (DelegateCommands.ContainsKey(delegateAction))
                RemoveEditorDelegate(delegateAction);

            DelegateSceneCommand del = new DelegateSceneCommand(delegateAction)
            { TimeBetweenUpdate = timeBetweenUpdate };
            DelegateCommands.Add(delegateAction, del);
            AddSceneCommand(del);
        }

        public static void RemoveEditorDelegate(Action delegateAction)
        {
            if (!DelegateCommands.ContainsKey(delegateAction))
                return;
            DelegateCommands[delegateAction].Stop();
            DelegateCommands.Remove(delegateAction);
        }

        /// <summary>
        ///  Adds a scene command to the active ones on the scene.
        /// </summary>
        /// <param name="command"></param>
        public static void AddSceneCommand(SceneCommand command)
        {

#if UNITY_2017_1_OR_NEWER
            EditorApplication.playModeStateChanged -= PlayStateChangeCallBack;
            EditorApplication.playModeStateChanged += PlayStateChangeCallBack;
#else
            EditorApplication.playmodeStateChanged -= PlayStateChangeCallBack;
            EditorApplication.playmodeStateChanged += PlayStateChangeCallBack;
#endif
            EditorApplication.update -= MonKeyEditorUpdate;
            EditorApplication.update += MonKeyEditorUpdate;

#if UNITY_2019
            SceneView.duringSceneGui -= OnSceneViewGUI;
            SceneView.duringSceneGui += OnSceneViewGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneViewGUI;
            SceneView.onSceneGUIDelegate += OnSceneViewGUI;
#endif
            command.OnActionDone += NotifyActionRemoved;
            int i = 1;
            while (CurrentSceneCommands.Count(_ => _.SceneCommandName == command.SceneCommandName) > 0)
            {
                if (command.SceneCommandName.Contains(i.ToString()))
                    command.SceneCommandName = command.SceneCommandName.Replace((i - 1).ToString(), i.ToString());
                else
                {
                    command.SceneCommandName = command.SceneCommandName + " " + i;
                }

                i++;
            }

            CurrentSceneCommands.Add(command);

            if (!CurrentSceneView)
                CurrentSceneView = SceneView.currentDrawingSceneView;

            if (CurrentSceneView)
                CurrentSceneView.Focus();

            SelectedSceneCommand = command;

            if (MonKeyInternalSettings.Instance.UseSceneCommandAsEditorWindow)
            {
                SceneCommandsWindow.ShowSceneCommandWindow();
            }
        }

        private static void NotifyActionRemoved(SceneCommand command)
        {
            ToRemove.Add(command);
        }

        private static bool overCross;
        public static SceneCommand SelectedSceneCommand;

        private static void OnSceneViewGUI(SceneView sceneView)
        {

            MousePosition = Event.current.mousePosition;
            MouseSceneRay = HandleUtility.GUIPointToWorldRay(MousePosition);

            SceneViewMousePosition = HandleUtility.GUIPointToScreenPixelCoordinate(MousePosition);
          //  sceneView.Repaint();

            if (!CurrentSceneView)
                CurrentSceneView = sceneView;

            SceneCommandSceneViewInterface();
            ExecuteCommandOnSceneGUI();
        }

        private static void SceneCommandSceneViewInterface()
        {
            if (!ShouldShowSceneGUI)
                return;

            if (MonKeyInternalSettings.Instance.UseSceneCommandAsEditorWindow)
                return;

            if (!MonkeyStyle.Instance.SearchFieldBackgroundTex)
                MonkeyStyle.Instance.PostInstanceCreation();

            float width = Mathf.Clamp(SceneView.currentDrawingSceneView.position.width * 0.2f,
                SceneGUIMinWidth, SceneGUIMaxWidth);
            float height = Mathf.Clamp(SceneView.currentDrawingSceneView.position.height * 0.2f,
                SceneGUIMinHeight, SceneGUIMaxHeight);

            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(SceneView.currentDrawingSceneView.position.width - width,
                    SceneView.currentDrawingSceneView.position.height - height, width, height),
                new GUIStyle()
                {
                    normal = { background = MonkeyStyle.Instance.WindowBackgroundTex }
                });
            GUILayout.BeginVertical(new GUIStyle() { margin = new RectOffset(0, 4, 0, 5) });

            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideLineStyle);
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideSecondLineStyle);

            GUILayout.BeginVertical(new GUIStyle() { margin = new RectOffset(0, 2, 0, 5) });

            GUILayout.BeginHorizontal();
            GUILayout.Label("", MonkeyStyle.Instance.SmallMonkey);

            GUILayout.BeginHorizontal(new GUIStyle() { margin = new RectOffset(5, 5, 0, 0) });
            GUILayout.Label("Active Scene Commands", MonkeyStyle.Instance.VariableHelpTextStyle);
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();

            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideLineStyle);
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideSecondLineStyle);

            GUI.skin = MonkeyStyle.Instance.MonkeyScrollBarStyle;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUIStyle()
            {
                margin = new RectOffset(4, 2, 0, 2),
                normal = { background = MonkeyStyle.Instance.ResultFieldTex },
            }, GUILayout.Height(60));

            foreach (var command in CurrentSceneCommands)
            {
                try
                {
                    if (command.Hidden)
                        continue;

                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal(new GUIStyle()
                    {
                        padding = new RectOffset(5, 5, 2, 2),
                        normal = { background = MonkeyStyle.Instance.ResultFieldTex }
                    }, GUILayout.Height(25));

                    GUILayout.BeginVertical(
                        new GUIStyle()
                        {
                            margin = new RectOffset(0, 0, 2, 0)
                        },
                        GUILayout.ExpandHeight(true));
                    string commandTitle = command.SceneCommandName
                        .Colored(MonkeyStyle.Instance.SearchResultTextColor).Bold();
                    if (command == SelectedSceneCommand)
                        commandTitle = commandTitle.Insert(0, "|".Colored(MonkeyStyle.Instance.WarningColor).Bold());

                    GUILayout.Label(commandTitle,
                        MonkeyStyle.Instance.SceneCommandHelpStyle);
                    GUILayout.EndVertical();

                    GUILayout.FlexibleSpace();

                    GUILayout.BeginVertical(new GUIStyle()
                    {
                        padding = new RectOffset(5, 5, 2, 2),
                        normal =
                        {
                            background = overCross
                                ? MonkeyStyle.Instance.BlackTex
                                : MonkeyStyle.Instance.HelpVariableTex
                        },
                        hover = { background = MonkeyStyle.Instance.BlackTex }
                    });

                    GUILayout.FlexibleSpace();

                    bool pressed = GUILayout.Button(" X "
                            .Colored(MonkeyStyle.Instance.WarningColor).Bold(),
                        MonkeyStyle.Instance.SceneCommandCrossStyle, GUILayout.ExpandWidth(true));
                    GUILayout.FlexibleSpace();

                    GUILayout.EndVertical();
                    overCross = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);

                    if (pressed)
                    {
                        command.Stop();
                        Event.current.Use();
                    }

                    /* if (command is ConfirmedCommand)
                     confirmedAction = true;*/

                    GUILayout.EndHorizontal();

                    GUILayout.Label("", MonkeyStyle.Instance.HorizontalSearchResultLine1Style);
                    GUILayout.Label("", MonkeyStyle.Instance.HorizontalSearchResultLine2Style);
                    GUILayout.Label("", MonkeyStyle.Instance.HorizontalSearchResultLine3Style);

                    GUILayout.EndVertical();

                    if (Event.current.type == EventType.MouseDown &&
                        GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        SelectedSceneCommand = command == SelectedSceneCommand ? null : command;
                        Event.current.Use();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("An Error was caught when using a command, sorry about that :( " +
                                   "Please report the exception under on MonKey's Discord channel");
                    Debug.LogException(e);
                    //stop the command if it encountered an exception
                    command.Stop();
                }
            }

            GUILayout.EndScrollView();

            GUI.skin = MonkeyStyle.Instance.DefaultStyle;
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideLineStyle);
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideSecondLineStyle);
            GUILayout.EndVertical();

            GUILayout.EndVertical();

            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideLineStyle);
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideSecondLineStyle);
            GUILayout.EndArea();

            if (SelectedSceneCommand != null && !SelectedSceneCommand.HideGUI)
            {
                SelectedSceneCommand.DisplayCommandPanel(width, height);
            }

            Handles.EndGUI();
            return;
        }

        private static void ExecuteCommandOnSceneGUI()
        {
            foreach (var command in CurrentSceneCommands)
            {
                try
                {
                    command.OnSceneGUI();
                    if (command.ShouldRepaintSceneView)
                        SceneView.RepaintAll();
                }
                catch (Exception e)
                {
                    Debug.LogError("An Error was caught when using a command, sorry about that :( " +
                                   "Please report the exception under on MonKey's Discord channel");
                    Debug.LogException(e);
                    command.Stop();
                }
            }
        }

        /// <summary>
        /// Calls a command by its name as if it would be selected in the MonKey console.
        /// </summary>
        /// <param name="commandName"></param>
        public static void CallCommand(string commandName)
        {
            CommandConsoleWindow.ExecuteCommand(commandName);
        }

        /// <summary>
        /// Makes it easier to collapse few undo operations at once. Returns an undoID that you must use together with Undo.CollapseOperation.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int CreateUndoGroup(string name)
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName(name);
            return Undo.GetCurrentGroup();
        }

        /// <summary>
        /// A Utility to know if a key is pressed by the user, to be used within a GUI block.
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static bool IsKeyDown(KeyCode keyCode)
        {
            return Event.current.type == EventType.KeyDown
                   && Event.current.keyCode == keyCode;
        }

        [Command("Print All Commands", "Prints information " +
                                       "about all the commands usable in Monkey in the console",
            QuickName = "PAL", Category = "Help")]
        public static void PrintAllCommand(
            [CommandParameter(Help = "Should the menu items be included in the information?" +
                                     " (useful to identify HotKey conflicts)")]
            bool includeMenuItems = true,
            [CommandParameter(Help = "Should the commands be printed in one print log" +
                                     " (to be all pasted in a file for instance)")]
            bool printInOneGo = false,
            [CommandParameter(Help = "Only prints the names of the commands")]
            bool printOnlyNames = false)
        {
            Debug.Log("MonKey List Of Commands:".Bold());
            List<CommandInfo> infoPrinted = new List<CommandInfo>();
            StringBuilder builder = new StringBuilder();
            foreach (var name in CommandManager.Instance.CommandNames.OrderBy(_ => _))
            {
                CommandInfo info = CommandManager.Instance.GetCommand(name);
                if (!includeMenuItems && info.IsMenuItem)
                    continue;

                if (infoPrinted.Contains(info))
                    continue;
                infoPrinted.Add(info);
                bool conflict = false;
                if (!printInOneGo)
                    builder = new StringBuilder();
                else
                {
                    builder.Append("\n");
                }

                builder.Append(info.CommandName.Bold());
                if (!printOnlyNames)
                {
                    if (!info.CommandHelp.IsNullOrEmpty())
                    {
                        builder.Append("\n");
                    }

                    builder.Append(info.CommandHelp);
                    if (info.IsMenuItem)
                    {
                        builder.Append(" ( ");
                        builder.Append(MonKeyLocManager.CurrentLoc.MenuItemAddedByMonkey);
                        builder.Append(" ) ");
                    }

                    builder.Append("\n");
                    if (info.HasQuickName)
                    {
                        builder.Append(" Quick Name: ");
                        builder.Append(info.CommandQuickName);
                    }

                    if (info.HasHotKeys)
                    {
                        builder.Append("\n");
                        builder.Append(" Hot Keys: ");
                        builder.Append(info.HotKey.FormattedKeys);
                        if (info.HotKeyInfo.IsConflictual)
                        {
                            conflict = true;
                            builder.Append(" Conflict with: ");
                            foreach (var commandInfo in info.HotKeyInfo.AssociatedCommands)
                            {
                                if (commandInfo != info)
                                {
                                    builder.Append(commandInfo.CommandName);
                                    builder.Append(" ");
                                }
                            }
                        }
                    }
                }

                if (!printInOneGo)
                {
                    if (conflict)
                        Debug.Log(builder.ToString().Bold()
                            .Colored(Color.red));
                    else
                    {
                        Debug.Log(builder.ToString());
                    }
                }
            }

            if (printInOneGo)
                Debug.Log(builder.ToString());
        }

        /// <summary>
        ///  An event you can register to, it will be called once all the MonKey commands are loaded.
        /// </summary>
        public static event Action OnCommandLoadingDone;

        private static void InvokeOnCommandLoadingDone()
        {
            OnCommandLoadingDone?.Invoke();
        }

        /// <summary>
        /// Removes the quick name specified from MonKey
        /// </summary>
        /// <param name="quickName"></param>
        public static void UnRegisterQuickName(string quickName)
        {
            CommandManager.Instance.CommandsByName.Remove(quickName);
        }

        public static void AddWordAliasesForCommandSearch(string word, string alias)
        {
            if (CommandManager.Instance.WordAliases.ContainsKey(word))
            {
                Debug.LogWarningFormat("MonKey Commander already has the alias '{0}' " +
                                       "for the word '{1}': it will be replaced by '{2}' ",
                    CommandManager.Instance.WordAliases[word], word, alias);

                CommandManager.Instance.WordAliases[word] = alias;
            }
            else
                CommandManager.Instance.WordAliases.Add(word, alias);
        }

        /// <summary>
        /// Removes the specified command from MonKey
        /// </summary>
        /// <param name="commandName"></param>
        public static void UnRegisterCommand(string commandName)
        {
            CommandManager.Instance.CommandsByName.Remove(commandName);
        }

        [Command("Stop All Scene Commands",
            "Stops the execution of all the scene commands at once", QuickName = "SAC", Category = "Dev")]
        public static void StopAllSceneCommands()
        {
            foreach (SceneCommand command in CurrentSceneCommands)
            {
                command.Stop();
            }
        }

        public enum PipelineType
        {
            HDRP,
            URP,
            CUSTOM,
            LEGACY,
        }


        public static PipelineType GetPipelineType()
        {
            string assetTypeHDRP = "HDRenderPipelineAsset";
            string assetTypeURP = "UniversalRenderPipelineAsset";

            if (!GraphicsSettings.renderPipelineAsset)
                return PipelineType.LEGACY;

            if (GraphicsSettings.renderPipelineAsset.GetType().Name.Contains(assetTypeHDRP))
            {
                return PipelineType.HDRP;
            }

            if (GraphicsSettings.renderPipelineAsset.GetType().Name.Contains(assetTypeURP))
            {
                return PipelineType.URP;
            }

            return PipelineType.LEGACY;
        }
    }
}