using MonKey.Editor.Internal;
using MonKey.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonKey.Editor.Commands
{
    public enum ActionConfirmationMode
    {
        CLICK,
        ENTER,
        ESCAPE,
        ENTER_AND_CLICK,
        ENTER_AND_ESCAPE,
        NONE
    }

    /// <summary>
    /// A scene command is an interactive commands on the scene that updates automatically and stops its logic when required.
    /// It can be started and stopped using the MonkeyEditorUtils class.
    /// </summary>
    public abstract class SceneCommand
    {
        internal bool Hidden = false;

        public bool HideGUI = false;

        internal double TimeSinceLastUpdate;

        public double TimeBetweenUpdate = 0;

        public float GUIHeight = 280;

        private int lineNumber;

        public bool ShouldRepaintSceneView = true;

        /// <summary>
        /// Should the scene command stops when the editor enters or exit play
        /// </summary>
        public bool StopOnPlayStateChange = true;

        /// <summary>
        /// The name of the command that will be displayed in the interface on MonKey
        /// </summary>
        public string SceneCommandName = "";

        /// <summary>
        /// An event raised when the Scene Command has just stopped
        /// </summary>
        public event Action<SceneCommand> OnActionDone;

        /// <summary>
        /// Any logic that should happen when the command stops (for instance registering undo) can be done here.
        /// </summary>
        public virtual void Stop()
        {
            OnActionDone?.Invoke(this);
        }

        /// <summary>
        /// Called on every Unity Update in the editor
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Called when the scene is focused and the MonKey GUI is called.
        /// Unity Requires some logic to be called on GUI, and will thorw en error message if you don't.
        /// </summary>
        public virtual void OnSceneGUI() { }

        private Vector2 scrollPosition;
        protected bool DisplayedInWindow = false;

        public virtual void DisplayCommandPanel(float width, float height, bool inWindow = false)
        {
            if (Hidden)
                return;
            this.DisplayedInWindow = inWindow;
            Color normalText = EditorStyles.label.normal.textColor;
            Color activeText = EditorStyles.label.active.textColor;
            Color hoverText = EditorStyles.label.hover.textColor;
            Color focusedText = EditorStyles.label.focused.textColor;

            if (!DisplayedInWindow)
            {
                EditorStyles.label.normal.textColor = MonkeyStyle.Instance.SearchResultTextColor;
                EditorStyles.label.active.textColor = MonkeyStyle.Instance.WarningColor;
                EditorStyles.label.hover.textColor = MonkeyStyle.Instance.WarningColor;
                EditorStyles.label.focused.textColor = MonkeyStyle.Instance.WarningColor;

            }

            if (!DisplayedInWindow)
                GUILayout.BeginArea(new Rect(0, SceneView.currentDrawingSceneView.position.height - height - 20, width * 2, height),
                    new GUIStyle()
                    {
                        normal = { background = MonkeyStyle.Instance.WindowBackgroundTex },
                        margin = new RectOffset(2, 2, 5, 5),
                    });

            if (!DisplayedInWindow)
                GUILayout.Label(SceneCommandName.Colored(MonkeyStyle.Instance.SearchResultTextColor).Bold(), new GUIStyle()
                {
                    richText = true
                });

            if (!DisplayedInWindow)
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUIStyle()
                {
                    normal = { background = MonkeyStyle.Instance.ResultFieldTex },
                    margin = new RectOffset(0, 0, 5, 5),
                });
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            }

            GUILayout.BeginVertical();

            lineNumber = 0;
            DisplayParameters();
            PostDisplayParameters();

            GUILayout.EndVertical();

            GUILayout.EndScrollView();

            if (!DisplayedInWindow)
                GUILayout.EndArea();

            if (!DisplayedInWindow)
            {
                EditorStyles.label.normal.textColor = normalText;
                EditorStyles.label.active.textColor = activeText;
                EditorStyles.label.hover.textColor = hoverText;
                EditorStyles.label.focused.textColor = focusedText;
            }

        }

        public virtual void DisplayParameters()
        {

        }

        public virtual void PostDisplayParameters()
        {

        }

        public void DisplayMessage(string message)
        {
            ParamStartIndex();

            if (!DisplayedInWindow)
                GUILayout.Label(message.Colored(MonkeyStyle.Instance.SearchResultTextColor)
                    , MonkeyStyle.Instance.CommandConfirmationStyle);
            else
            {
                GUILayout.Label(message);
            }

            GUILayout.EndHorizontal();
        }

        private void ParamStartIndex()
        {
            if (DisplayedInWindow)
            {
                GUILayout.BeginHorizontal();
                return;
            }

            if (!MonkeyStyle.Instance)
            {
                MonkeyStyle.FindInstance();
                GUILayout.BeginHorizontal();
                return;
            }

            GUILayout.BeginHorizontal(lineNumber % 2 == 0
                ? MonkeyStyle.Instance.ParameterLayoutForcedHighlightStyle
                : MonkeyStyle.Instance.ParameterLayoutNoHighlightStyle);
            lineNumber++;
        }

        public void DisplayBoolOption(string message, ref bool condition)
        {
            ParamStartIndex();

            if (!DisplayedInWindow)
                GUILayout.Label(message.Colored(MonkeyStyle.Instance.SearchResultTextColor), new GUIStyle()
                {
                    margin = new RectOffset(5, 0, 0, 0),
                    richText = true,
                });
            else
            {
                GUILayout.Label(message);
            }

            GUILayout.FlexibleSpace();
            condition = GUILayout.Toggle(condition, "");
            GUILayout.EndHorizontal();
        }

        private Dictionary<string, bool> foldoutStatuses = new Dictionary<string, bool>();

        public int DisplayObjectListOption<T>(string name, List<T> list) where T : Object
        {
            int changedID = -1;
            Type type = typeof(T);
            ParamStartIndex();

            GUILayout.BeginVertical();
            if (!foldoutStatuses.ContainsKey(name))
                foldoutStatuses.Add(name, false);

            bool foldStatus = foldoutStatuses[name];

            if (!DisplayedInWindow)
                foldStatus = EditorGUILayout.Foldout(foldStatus, name.Colored(MonkeyStyle.Instance.SearchResultTextColor), true, new GUIStyle(EditorStyles.foldout)
                {
                    margin = new RectOffset(5, 0, 0, 0),
                    richText = true,
                });
            else
            {
                foldStatus = EditorGUILayout.Foldout(foldStatus, name, true);
            }
            foldoutStatuses[name] = foldStatus;

            if (foldStatus)
            {
                T[] array = list.ToArray();
                for (int i = 0; i < array.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    var member = array[i];

                    EditorGUILayout.LabelField("   ");

                    list[i] = (T)EditorGUILayout.ObjectField("", array[i], type, true);

                    if (GUILayout.Button("S"))
                    {
                        if (Selection.activeObject is T)
                            list[i] = (T)Selection.activeObject;
                    }

                    if (member != list[i])
                        changedID = i;
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (!DisplayedInWindow)
                    GUILayout.Label("Add".Colored(MonkeyStyle.Instance.SearchResultTextColor), new GUIStyle()
                    {
                        margin = new RectOffset(5, 0, 0, 0),
                        richText = true,
                    });
                else
                {
                    GUILayout.Label("Add");
                }
                T newT = null;

                if (GUILayout.Button("S"))
                {
                    if (Selection.activeObject is T)
                        newT = (T)Selection.activeObject;
                }

                bool wasNull = !newT;

                newT = (T)EditorGUILayout.ObjectField("", newT, type, true);



                GUILayout.EndHorizontal();

                if (newT)
                {
                    if (wasNull)
                    {
                        if (Selection.objects.Contains(newT))
                        {
                            list.AddRange(Selection.objects.Where(_ => _ is T && !list.Contains(_)).Convert(_ => (T)_));
                        }
                        else
                        {
                            list.Add(newT);
                        }
                    }

                    changedID = list.Count - 1;
                }


                list.RemoveAll(_ => !_);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            return changedID;
        }



        public int DisplayVector2ArrayOption(string name, ref Vector2[] array)
        {
            int changedID = -1;

            ParamStartIndex();

            if (!DisplayedInWindow)
                GUILayout.Label(name.Colored(MonkeyStyle.Instance.SearchResultTextColor), new GUIStyle()
                {
                    margin = new RectOffset(5, 0, 0, 0),
                    richText = true,
                });
            else
            {
                GUILayout.Label(name);
            }

            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();

            for (int i = array.Length - 1; i >= 0; i--)
            {
                GUILayout.BeginHorizontal();
                var member = array[i];

                array[i] = EditorGUILayout.Vector2Field("", array[i]);


                if (member != array[i])
                    changedID = i;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            return changedID;
        }

        public void DisplayObjectOption<T>(string name, ref T obj) where T : Object
        {
            Type type = typeof(T);
            ParamStartIndex();

            if (!DisplayedInWindow)
                GUILayout.Label(name.Colored(MonkeyStyle.Instance.SearchResultTextColor), new GUIStyle()
                {
                    margin = new RectOffset(5, 0, 0, 0),
                    richText = true,
                });
            else
            {
                GUILayout.Label(name);
            }

            GUILayout.FlexibleSpace();

            obj = (T)EditorGUILayout.ObjectField("", obj, type, true);

            if (GUILayout.Button("S"))
            {
                if (Selection.activeObject is T)
                    obj = (T)Selection.activeObject;
            }

            GUILayout.EndHorizontal();

        }

        public virtual void DisplayEnumOption(string name, ref Enum currentEnumValue)
        {
            ParamStartIndex();
            if (!DisplayedInWindow)
                GUILayout.Label(name.Colored(MonkeyStyle.Instance.SearchResultTextColor), new GUIStyle()
                {
                    margin = new RectOffset(5, 0, 0, 0),
                    richText = true,
                });
            else
            {
                GUILayout.Label(name);
            }

            GUILayout.FlexibleSpace();
            currentEnumValue = EditorGUILayout.EnumPopup(currentEnumValue);
            GUILayout.EndHorizontal();
        }

        public void DisplayIntOption(string name, ref int value)
        {
            ParamStartIndex();
            /*  GUILayout.Label(name.Colored(MonkeyStyle.Instance.SearchResultTextColor), new GUIStyle()
              {
                  margin = new RectOffset(5, 0, 0, 0),
                  richText = true,
              });
              GUILayout.FlexibleSpace();*/

            /*  if (GUILayout.Button("-"))
              {
                  value--;
              }*/

            if (!DisplayedInWindow)
                value = EditorGUILayout.IntField(name, value, new GUIStyle(EditorStyles.numberField)
                {
                    margin = new RectOffset(5, 5, 0, 0),
                });
            else
            {
                value = EditorGUILayout.IntField(name, value);
            }
            /*  if (GUILayout.Button("+"))
              {
                  value++;
              }*/

            GUILayout.EndHorizontal();
        }

        public void DisplayDoubleOption(string name, ref double value)
        {
            ParamStartIndex();

            var style = new GUIStyle(EditorStyles.numberField)
            {
                margin = new RectOffset(5, 0, 0, 0),
                richText = true,

            };
            if (!DisplayedInWindow)
                value = EditorGUILayout.DoubleField(name, value, style);
            else
            {
                value = EditorGUILayout.DoubleField(name, value);
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Displays a Float parameter on the scene interface
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns> true if teh value was changed</returns>
        public bool DisplayFloatOption(string name, ref float value)
        {
            float prevValue=value;
            ParamStartIndex();

            var style = new GUIStyle(EditorStyles.numberField)
            {
                margin = new RectOffset(5, 0, 0, 0),
                richText = true,

            };
            if (!DisplayedInWindow)
                value = EditorGUILayout.FloatField(name, value, style);
            else
            {
                value = EditorGUILayout.FloatField(name, value);
            }
            GUILayout.EndHorizontal();
            return !Mathf.Approximately(prevValue, value);
        }

        public void DisplayFloatPercentOption(string name, ref float value)
        {
            ParamStartIndex();

            /* var style = new GUIStyle(EditorStyles.numberField)
             {
                 margin = new RectOffset(5, 0, 0, 0),
                 richText = true,

             };*/

            value = EditorGUILayout.Slider(name, value, 0, 1);
            GUILayout.EndHorizontal();
        }

        public void DisplayVectorOption(string name, ref Vector3 vector)
        {
            ParamStartIndex();

            /*   GUILayout.Label(name.Colored(MonkeyStyle.Instance.SearchResultTextColor), new GUIStyle()
               {
                   margin = new RectOffset(5, 0, 0, 0),
                   richText = true,
               });
               GUILayout.FlexibleSpace();*/

            vector = EditorGUILayout.Vector3Field(name, vector);
            GUILayout.EndHorizontal();
        }

        public void DisplayVector2Option(string name, ref Vector2 vector)
        {
            ParamStartIndex();
            /*GUILayout.Label(name.Colored(MonkeyStyle.Instance.SearchResultTextColor), new GUIStyle()
            {
                margin = new RectOffset(5, 0, 0, 0),
                richText = true,
            });
            GUILayout.FlexibleSpace();*/
            vector = EditorGUILayout.Vector2Field(name, vector);
            GUILayout.EndHorizontal();
        }

        public void DisplayButton(string name, Action action)
        {
            if (GUILayout.Button(name))
            {
                action();
            }
        }
    }

    public abstract class ConfirmedCommand : SceneCommand
    {

        public bool ShowActionButton = true;
        public ActionConfirmationMode ConfirmationMode = ActionConfirmationMode.ENTER_AND_ESCAPE;

        public virtual string ConfirmationMessage()
        {
            StringBuilder builder = new StringBuilder();
            switch (ConfirmationMode)
            {
                case ActionConfirmationMode.CLICK:
                    builder.Append("Click ");
                    break;
                case ActionConfirmationMode.ENTER:
                    builder.Append("Press ENTER ");
                    break;
                case ActionConfirmationMode.ENTER_AND_CLICK:
                    builder.Append("Press ENTER or Click ");
                    break;
                case ActionConfirmationMode.ESCAPE:
                    builder.Append("Press ESCAPE ");
                    break;
                case ActionConfirmationMode.ENTER_AND_ESCAPE:
                    builder.Append("Press ESCAPE or ENTER ");
                    break;
                case ActionConfirmationMode.NONE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            builder.Append("To Execute '");
            builder.Append(SceneCommandName);
            builder.Append("'");

            return builder.ToString();
        }

        public override void DisplayParameters()
        {
            DisplayMessage(ConfirmationMessage());
            base.DisplayParameters();
        }

        public override void PostDisplayParameters()
        {
            base.PostDisplayParameters();
            if (ShowActionButton)
                DisplayButton("Apply", ApplyFunction);
        }

        public virtual void ApplyFunction()
        {
            Stop();
        }

        public override void OnSceneGUI()
        {
            base.OnSceneGUI();
            switch (ConfirmationMode)
            {
                case ActionConfirmationMode.CLICK:
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        Stop();
                    break;
                case ActionConfirmationMode.ENTER:
                    if (MonkeyEditorUtils.IsKeyDown(KeyCode.KeypadEnter) ||
                        MonkeyEditorUtils.IsKeyDown(KeyCode.Return))
                        Stop();
                    break;
                case ActionConfirmationMode.ENTER_AND_CLICK:
                    if ((Event.current.type == EventType.MouseDown && Event.current.button == 0) ||
                        MonkeyEditorUtils.IsKeyDown(KeyCode.KeypadEnter) ||
                        MonkeyEditorUtils.IsKeyDown(KeyCode.Return))
                        Stop();
                    break;
                case ActionConfirmationMode.ESCAPE:
                    if (MonkeyEditorUtils.IsKeyDown(KeyCode.Escape))
                        Stop();
                    break;
                case ActionConfirmationMode.ENTER_AND_ESCAPE:
                    if (MonkeyEditorUtils.IsKeyDown(KeyCode.Escape) || MonkeyEditorUtils.IsKeyDown(KeyCode.KeypadEnter) ||
                        MonkeyEditorUtils.IsKeyDown(KeyCode.Return))
                        Stop();
                    break;
                case ActionConfirmationMode.NONE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public override void Update()
        {
            if (!MonkeyEditorUtils.CurrentSceneView)
                MonkeyEditorUtils.CurrentSceneView = SceneView.currentDrawingSceneView;

            if (EditorWindow.mouseOverWindow != null && MonkeyEditorUtils.CurrentSceneView && EditorWindow.mouseOverWindow == MonkeyEditorUtils.CurrentSceneView)
                MonkeyEditorUtils.CurrentSceneView.Focus();
        }
    }

    public abstract class TimedSceneCommand : SceneCommand
    {
        public float Duration = -1;
        public float CurrentTime = 0;

        protected TimedSceneCommand(float duration)
        {
            Duration = duration;
        }

        public override void Update()
        {
            CurrentTime += Time.deltaTime;
            if (CurrentTime >= Duration && Duration > 0)
                Stop();
        }
    }

    public abstract class InteractiveCommand : ConfirmedCommand
    {
        protected string ActionOnSpace;

        public InteractiveCommand()
        {
            ConfirmationMode = ActionConfirmationMode.ENTER_AND_ESCAPE;
        }

        public override string ConfirmationMessage()
        {
            return "Press SPACE to " + ActionOnSpace + ", ESCAPE to stop";
        }

        public override void OnSceneGUI()
        {
            base.OnSceneGUI();
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Space)
            {
                OnSpaceDownAction();
            }

        }

        protected virtual void OnSpaceDownAction()
        {

        }
    }

    internal class DelegateSceneCommand : SceneCommand
    {

        private Action delegateAction;

        public DelegateSceneCommand(Action delegateAction)
        {
            Hidden = true;
            this.delegateAction = delegateAction;
        }

        public override void Update()
        {
            delegateAction();
        }

    }

    public class TimedMultiSceneCommand : TimedSceneCommand
    {
        private readonly SceneCommand[] commands;

        public TimedMultiSceneCommand(SceneCommand[] commands, float duration) : base(duration)
        {
            this.commands = commands;
        }

        public override void Update()
        {
            base.Update();
            foreach (SceneCommand command in commands)
            {
                command.Update();
            }
        }

        public override void OnSceneGUI()
        {
            base.OnSceneGUI();
            foreach (SceneCommand command in commands)
            {
                command.OnSceneGUI();
            }
        }

        public override void Stop()
        {
            base.Stop();
            foreach (SceneCommand sceneCommand in commands)
            {
                sceneCommand.Stop();
            }
        }
    }

}
