using MonKey.Editor.Internal;
using MonKey.Extensions;
using MonKey.Settings.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace MonKey.Editor.Console
{
    
    internal class CommandConsoleWindow : EditorWindow
    {
        internal static readonly bool DrawDebugAssemblies = false;

        internal static readonly bool LogExceptions = false;

        internal static Rect RowHeight = new Rect(0, 0, 0, 40);

        internal static event Action OnConsoleClosed;

        private static bool pausedByMonkey = false;

        private const float WindowMinWidth = 600;
        private const float WindowMaxWidth = 1000;
        private const float WindowMinHeight = 360;
        private const float WindowMaxHeight = 600;
        private const int ScrollIndexThreshold = 8;

        private MethodInfo isDockedInfo;

        internal bool IsDocked
        {
            get { return (bool)isDockedInfo.Invoke(this, null); }
        }

        private bool isActiveMode;
        private bool validationKeyPressed;
        private bool mousePressed;
        internal bool IsParametricMethodCompletion;

        internal string PreviousSearchTerms;

        internal ParametricCommandExecution CurrentExecution;

        internal int SelectedIndex;
        internal Vector2 ScrollIndex;

        internal string SearchTerms = "";
        internal string CurrentCategory = "";
        internal bool BackCategory;
        internal string HoveredCategory = "";


        internal IEnumerable<CommandInfo> CommandResult;
        internal bool DisplayNoResult;

        internal bool MouseActivity;
        internal bool ForceValidationKeyPressed;
        private bool isShiftPressed;

        internal bool MouseOverField;

        internal bool JustOpenedActiveMode;

        internal GUIStyle MonkeyFocusLogoStyle;

        internal bool Focused;

        internal bool PreventSearchMovement;

        internal int TabFrames = 10;

        [MenuItem("Tools/MonKey Commander/🐵 Open Command Console _`", false, 9999)]
#if !UNITY_2019
        [MenuItem("Tools/MonKey Commander/Open Console Alt/🐵 Open Command Console 1 _~", false, 9999)]
        [MenuItem("Tools/MonKey Commander/Open Console Alt/🐵 Open Command Console 2 _§", false, 9999)]
        [MenuItem("Tools/MonKey Commander/Open Console Alt/🐵 Open Command Console 3 _±", false, 9999)]
        [MenuItem("Tools/MonKey Commander/Open Console Alt/🐵 Open Command Console 4 _²", false, 9999)]
        [MenuItem("Tools/MonKey Commander/Open Console Alt/🐵 Open Command Console 5 _^", false, 9999)]
        [MenuItem("Tools/MonKey Commander/Open Console Alt/🐵 Open Command Console 6 _º", false, 9999)]
        [MenuItem("Tools/MonKey Commander/Open Console Alt/🐵 Open Command Console 7 _¬", false, 9999)]
        [MenuItem("Tools/MonKey Commander/Open Console Alt/🐵 Open Command Console 8 _Ё", false, 9999)]
#endif
        [MenuItemCommandLink]
        static void ToggleMonkeyPanel()
        {
            if (!MonKeyInternalSettings.Instance.UseCustomConsoleKey)
                TogglePanelCustom();
        }

        public static void TogglePanelCustom()
        {

            if (MonkeyStyle.Instance.IsFakeTextureInUse)
            {
                MonkeyStyle.Instance.PostInstanceCreation();
            }

            if (CommandManager.Instance.CommandCount == 0)
                CommandManager.Instance.OnEnable();

            if (!CurrentPanel)
                ShowNewPanel();
            else
            {
                if (CurrentPanel.isDockedInfo == null)
                    GetIsDockedInfo();

                if (CurrentPanel.IsDocked)
                {
                    if (CurrentPanel.isActiveMode)
                    {
                        if (!CurrentPanel.Focused)
                        {
                            CurrentPanel.Focus();
                            CurrentPanel.Focused = true;
                        }
                        else
                        {
                            ResetSearchTerms();
                            CurrentPanel.CloseOrSetInactive();
                        }
                    }
                    else
                    {
                        ActivatePanel();
                    }
                }
                else
                {
                    CurrentPanel.CloseOrSetInactive();
                }
            }
        }

        private static void ResetSearchTerms()
        {
            CurrentPanel.SearchTerms = "";
            CurrentPanel.CurrentCategory = "";
            CurrentPanel.PreviousSearchTerms = "";
            SetFirstHistoryCommandSelected();
        }

        private static void SetFirstHistoryCommandSelected()
        {
            if (!CurrentPanel || CommandManager.Instance == null
                || CommandManager.Instance.CategoriesByName == null ||
                CommandManager.CommandHistory == null)
                return;

            CommandManager.Instance.CategoriesByName.TryGetValue(CommandManager.CommandHistory, out var cat);
            if (cat != null && cat.CommandNames != null && cat.CommandNames.Count > 0)
            {
                var list = new List<CommandInfo> { CommandManager.Instance.GetCommand(cat.CommandNames.First()) };
                CurrentPanel.CommandResult = list;
                CurrentPanel.SelectedIndex = 0;
                // Debug.Log(cat.CommandNames.First());
            }
        }

        private static void ActivatePanel()
        {
            if (Application.isPlaying && !EditorApplication.isPaused
                                      && MonKeyInternalSettings.Instance.PauseGameOnConsoleOpen)
            {
                pausedByMonkey = true;
                EditorApplication.isPaused = true;
            }

            CurrentPanel.isActiveMode = !CurrentPanel.isActiveMode;

            if (CurrentPanel.isActiveMode)
                CurrentPanel.JustOpenedActiveMode = true;

            ResetSearchTerms();
            CurrentPanel.ScrollIndex = Vector2.zero;
            CurrentPanel.IsParametricMethodCompletion = false;
            CurrentPanel.DisplayNoResult = false;
            CurrentPanel.Focus();
            CurrentPanel.Repaint();

            if (CommandManager.Instance.CommandsByName.Count == 0)
                CommandManager.Instance.OnEnable();
        }

        [DidReloadScripts]
        private static void ResetWindow()
        {
            EditorWindow window = focusedWindow;
            try
            {
                CurrentPanel = FindObjectOfType<CommandConsoleWindow>();
                if (CurrentPanel)
                {
                    ShowNewPanel();
                    CurrentPanel.CloseOrSetInactive();
                }

            /*    if (window)
                    window.Focus();*/
            }
            catch (NullReferenceException)
            {
                //ignore, due to change of version
            }

        }

        private void OnFocus()
        {
            if (!MonkeyStyle.Instance)
                return;

            MonkeyFocusLogoStyle = SearchTerms.IsNullOrEmpty()
                ? MonkeyStyle.Instance.MonkeyLogoStyleSmiling
                : MonkeyStyle.Instance.MonkeyLogoStyleHappy;
            Focused = true;
        }

        private void OnLostFocus()
        {
            if (!MonkeyStyle.Instance)
                return;

            MonkeyFocusLogoStyle = MonkeyStyle.Instance.MonkeyLogoStyle;
            Repaint();
            if (!MonKeyInternalSettings.Instance.ForceFocusOnDocked)
                Focused = false;
        }

        private void OnGUI()
        {
            if (isDockedInfo == null)
                return;

            if (SearchTerms == "`" ||
                (MonKeyInternalSettings.Instance.UseCustomConsoleKey &&
                 SearchTerms == MonKeyInternalSettings.Instance.MonkeyConsoleOverrideHotKey))
            {
                ResetSearchTerms();
                /*if (!JustOpenedActiveMode)
                    CloseOrSetInactive();*/
            }

            HotKeysManager.CheckCustomHotKey();

            HandleMouse();

            MonkeyStyle.Instance.InitDefaultStyle();
            GUI.skin = MonkeyStyle.Instance.DefaultStyle;

            if (TabFrames > 0)
            {
                TabFrames--;
                Focus();
            }

            if (!CurrentPanel)
            {
                //   ToggleMonkeyPanel();
                //not sure why needed now, tbc
                return;
            }

            if (!Focused)
            {
                if ((!IsDocked && !MonKeyInternalSettings.Instance.PreventFocusOnPopup)
                    || (isActiveMode && MonKeyInternalSettings.Instance.ForceFocusOnDocked))
                    Focus();
            }

            if (!isActiveMode)
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    Event.current.Use();
                    ToggleMonkeyPanel();
                    Repaint();
                    return;
                }

                try
                {
                    SleepingMonkeyPanelDisplay.DisplaySleepingMonkey();
                    // OnGUIActiveMode();
                    if (CommandManager.Instance.IsLoading)
                        LoadingNoticeDisplay.DisplayLoadingNotice(this);
                }
                catch (Exception e)
                {
                    if (LogExceptions)
                        Debug.Log(e);
                    GUIUtility.ExitGUI();
                }

                return;
            }

            OnGUIActiveMode();
        }

        private void OnGUIActiveMode()
        {
            HandleInput();
            HandleCommandChoice();

            if ((!IsDocked && focusedWindow != this &&
                 !MonKeyInternalSettings.Instance.PreventFocusOnPopup)
                || JustOpenedActiveMode || MonKeyInternalSettings.Instance.ForceFocusOnDocked)
            {
                Focus();
            }

            InitDisplayWindow();

            if (IsParametricMethodCompletion)
            {
                ParametricPanelDisplay.DisplayParametricPanel(this);
            }
            else
            {
                CommandSearchPanelDisplay.DisplayCommandPanel(this);
            }

            if (JustOpenedActiveMode)
            {
                JustOpenedActiveMode = false;
            }

            HandleCommandChoice();

            if (MonkeyEditorUtils.IsKeyDown(KeyCode.Tab))
            {
                TabFrames = 5;
            }
        }

        #region OPENCLOSE

        private static void ShowNewPanel()
        {
            if (CurrentPanel != null)
            {
                CurrentPanel.CloseOrSetInactive();
            }

            if (Application.isPlaying && !EditorApplication.isPaused
                                      && MonKeyInternalSettings.Instance.PauseGameOnConsoleOpen)
            {
                pausedByMonkey = true;
                EditorApplication.isPaused = true;
            }

            var monkeyPanel = GetWindow<CommandConsoleWindow>();

            monkeyPanel.wantsMouseMove = true;

            monkeyPanel.Show();
            monkeyPanel.titleContent = new GUIContent(" MonKey", MonkeyStyle.Instance.MonkeyHead, "Uuuh! Uuuh!");
            monkeyPanel.maxSize = new Vector2(WindowMaxWidth, WindowMaxHeight);
            monkeyPanel.minSize = new Vector2(WindowMinWidth, WindowMinHeight);
            CurrentPanel = monkeyPanel;
            SetFirstHistoryCommandSelected();

            if (CurrentPanel != null)
            {
                CurrentPanel.isActiveMode = true;
                CurrentPanel.JustOpenedActiveMode = true;
                GetIsDockedInfo();
            }
        }

        private static void GetIsDockedInfo()
        {
            BindingFlags fullBinding = BindingFlags.Public
                                       | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            PropertyInfo info = typeof(EditorWindow).GetProperty("docked", fullBinding);
            if (info != null)
            {
                CurrentPanel.isDockedInfo = info.GetGetMethod(true);
            }

        }


        public void CloseOrSetInactive()
        {
            if (Application.isPlaying && EditorApplication.isPaused && pausedByMonkey)
                EditorApplication.isPaused = false;

            if (isDockedInfo == null)
                GetIsDockedInfo();

            if (IsDocked)
            {
                isActiveMode = false;
                ResetSearchTerms();
                Repaint();
                GUI.UnfocusWindow();
                //TODO replace with a better alternative
                //    EditorUtility.FocusProjectWindow();
            }
            else
            {
                Close();
                CurrentPanel = null;
                OnConsoleClosed?.Invoke();
            }

        }

        #endregion

        #region INPUT

        public static void ForceEditSearchAtEnd(string searchTerm)
        {
            var te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor),
                GUIUtility.keyboardControl);
#if UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1
            te.pos = itemName.Length;
            te.selectPos = itemName.Length;
#else
            te.cursorIndex = searchTerm.Length;
            te.selectIndex = searchTerm.Length;
#endif
        }

        private void HandleInput()
        {
            CurrentPanel.PreventSearchMovement = false;

            // HandleMouse();

            if (MonkeyEditorUtils.IsKeyDown(KeyCode.Escape))
            {
                if (IsParametricMethodCompletion)
                {
                    IsParametricMethodCompletion = false;
                    CurrentExecution = null;
                    ResetSearchTerms();
                    JustOpenedActiveMode = true;
                    DisplayNoResult = false;
                    ScrollIndex = Vector2.zero;
                    CheckSearch();
                    Repaint();
                }
                else
                {
                    CloseOrSetInactive();
                }
            }
            else if (MonkeyEditorUtils.IsKeyDown(KeyCode.DownArrow))
            {
                PreventSearchMovement = true;
                SelectedIndex++;
                if (!IsParametricMethodCompletion)
                    SelectedIndex = Mathf.Min(SelectedIndex, CommandResult?.Count() - 1 ?? 0);
                else
                {
                    SelectedIndex = Mathf.Min(SelectedIndex,
                        CurrentExecution.CurrentAutoComplete?.Count - 1 ?? 0);
                    CurrentExecution.NotifyInputFromAutoComplete(SelectedIndex);
                }

                if (SelectedIndex > ScrollIndexThreshold)
                    ScrollIndex.y += RowHeight.height * 60;
            }
            else if (MonkeyEditorUtils.IsKeyDown(KeyCode.UpArrow))
            {
                PreventSearchMovement = true;

                SelectedIndex--;

                if (IsParametricMethodCompletion)
                {
                    SelectedIndex = Mathf.Max(-1, SelectedIndex);

                    CurrentExecution.NotifyInputFromAutoComplete(SelectedIndex);
                }
                else
                {
                    SelectedIndex = Mathf.Max(0, SelectedIndex);
                }

                ScrollIndex.y -= RowHeight.height * 60;
            }
            else if (MonkeyEditorUtils.IsKeyDown(KeyCode.Backspace) && SearchTerms.IsNullOrEmpty())
            {

            }

            validationKeyPressed = MonkeyEditorUtils.IsKeyDown(KeyCode.Return) || MonkeyEditorUtils.IsKeyDown(KeyCode.Tab) || MonkeyEditorUtils.IsKeyDown(KeyCode.KeypadEnter);
            bool previousForce = ForceValidationKeyPressed;
            ForceValidationKeyPressed = Event.current.command || Event.current.control;

            if (previousForce != ForceValidationKeyPressed)
                Repaint();

            isShiftPressed = Event.current.shift;

        }

        private bool HasCommandResult
        {
            get
            {
                return CommandResult != null
                       && CommandResult.Count() > SelectedIndex;
            }
        }

        public static CommandConsoleWindow CurrentPanel { get; private set; }

        public bool MousePressed => mousePressed;

        private void HandleCommandChoice()
        {
            if (!Focused)
                return;
            /*
                        if (CommandResult != null && CommandResult.Any())
                            Debug.Log(CommandResult.ElementAt(SelectedIndex).CommandName);
                        else
                        {
                            Debug.Log("Nothing?");
                        }*/

            if (IsParametricMethodCompletion)
            {
                HandleParametricCommandChoice();
            }
            else if (validationKeyPressed || (MousePressed && MouseOverField)
                     && HasCommandResult)
            {
                HandleSimpleCommand();
            }
            else if (MousePressed && (!HoveredCategory.IsNullOrEmpty() || BackCategory))
            {
                CurrentCategory = HoveredCategory;
                HoveredCategory = "";
                BackCategory = false;

                if (!CurrentCategory.IsNullOrEmpty())
                {
                    CommandResult = CommandManager.Instance.CategoriesByName[CurrentCategory]
                        .CommandNames.ConvertAll(_ => CommandManager.Instance.GetCommand(_));
                }

                Repaint();
            }

            validationKeyPressed = false;
        }

        private void HandleSimpleCommand()
        {
            if (CommandResult == null)
                return;

            if (CommandResult.Count() > SelectedIndex && SelectedIndex >= 0)
            {
                if (CommandResult.ElementAt(SelectedIndex) == null)
                    return;

                if (CommandResult.ElementAt(SelectedIndex).IsParametric)
                {
                    ParametricCommandExecution exec =
                        new ParametricCommandExecution(CommandResult.ElementAt(SelectedIndex), this);
                    if (ForceValidationKeyPressed
                        && CommandResult.ElementAt(SelectedIndex).CanUseQuickDefaultCall)
                    {
                        exec.ExecuteCommand(this);
                        CloseOrSetInactive();
                    }
                    else if (CommandResult.ElementAt(SelectedIndex).IsValid)
                    {
                        ActivateParametricMode(exec);
                    }
                }
                else
                {
                    if (ExecuteCommand(new[] { CommandResult.ElementAt(SelectedIndex) }))
                    {
                        if (!Event.current.control)
                        {
                            CloseOrSetInactive();
                        }
                    }
                }
            }
        }

        public static void ExecuteCommand(string commandName, bool emitWarning = true)
        {
            CommandInfo comInfo = CommandManager.Instance.GetCommandInfo(commandName);

            if (emitWarning && comInfo == null)
            {
                Debug.LogWarningFormat("Monkey Warning: {0} " +
                                       "is not a valid command name: the command " +
                                       "couldn't be executed", commandName);
                return;
            }

            if (!comInfo.IsParametric)
            {
                comInfo.ExecuteCommand();
                return;
            }

            if (!CurrentPanel)
            {
                ShowNewPanel();
            }
            else
            {
                if (!CurrentPanel.isActiveMode)
                {
                    ActivatePanel();
                }
                else
                {
                    CurrentPanel.Focus();
                }

            }

            ParametricCommandExecution exec =
                new ParametricCommandExecution(comInfo, CurrentPanel);

            CurrentPanel.ActivateParametricMode(exec);
        }

        private void ActivateParametricMode(ParametricCommandExecution exec)
        {
            CurrentExecution = exec;
            IsParametricMethodCompletion = true;
            JustOpenedActiveMode = true;
            ResetSearchTerms();
            ScrollIndex = Vector2.zero;
            DisplayNoResult = CurrentExecution.CurrentAutoComplete == null;
            mousePressed = false;
            exec.NotifyNextVariable(this);
            Repaint();
        }

        private void HandleParametricCommandChoice()
        {
            if (validationKeyPressed)
            {
                if (ForceValidationKeyPressed || !CurrentExecution.HasNextVariable
                    && !isShiftPressed && (!CurrentExecution.CurrentParameterInfo.IsArray ||
                                           CurrentExecution.CurrentTextEntered.IsNullOrEmpty() &&
                                           CurrentExecution.CurrentAutoCompleteID == -1))
                {
                    if (TryExecuteCurrentParametricCommand())
                        return;
                    return;
                }

                if (isShiftPressed)
                {
                    CurrentExecution.NotifyPreviousVariable(this);
                    SearchTerms = CurrentExecution.CurrentTextEntered;
                    Repaint();
                    PreventSearchMovement = true;
                    Focus();
                }
                else
                {
                    CurrentExecution.NotifyNextVariable(this);
                    SearchTerms = CurrentExecution.CurrentTextEntered;
                    Repaint();
                    PreventSearchMovement = true;
                    Focus();
                }
            }
        }

        internal bool TryExecuteCurrentParametricCommand()
        {
            if (CurrentExecution.ExecuteCommand(this))
            {
                CloseOrSetInactive();
                Repaint();
                return true;
            }

            CurrentExecution.JumpToFirstError(this);
            SearchTerms = CurrentExecution.CurrentTextEntered;
            Repaint();
            return false;
        }

        private void HandleMouse()
        {
            MouseActivity = Event.current.type == EventType.MouseMove;
            mousePressed = Event.current.type == EventType.MouseDown;
        }

        #endregion

        #region COMMANDS

        public void CheckSearch()
        {
            RefreshResult();

            PreviousSearchTerms = SearchTerms;

            if (IsParametricMethodCompletion)
            {
                if (CurrentExecution.CurrentParameterInfo == null)
                {
                    SelectedIndex = -1;
                    return;
                }

                SelectedIndex = CurrentExecution.CurrentAutoComplete != null &&
                                CurrentExecution.CurrentAutoComplete.Count > 0
                    ? Mathf.Clamp(SelectedIndex, -1, CurrentExecution.CurrentAutoComplete.Count - 1)
                    : -1;
            }
            else
            {
                SelectedIndex = CommandResult != null && CommandResult.Any()
                    ? Mathf.Clamp(SelectedIndex, 0, CommandResult.Count() - 1)
                    : -1;
            }
        }

        private void RefreshResult()
        {
            if (MonKeyInternalSettings.Instance.UseCustomConsoleKey)
                SearchTerms = SearchTerms.Replace(MonKeyInternalSettings.Instance.MonkeyConsoleOverrideHotKey, "");

            if (IsParametricMethodCompletion)
            {
                if (PreviousSearchTerms != SearchTerms)
                {
                    CurrentExecution.NotifyNewInput(SearchTerms, this);
                    DisplayNoResult = CurrentExecution.CurrentAutoComplete == null ||
                        CurrentExecution.CurrentAutoComplete.Count == 0;
                    if (CurrentExecution.CurrentParameterInfo.ForceAutoCompleteUsage)
                        SelectedIndex = 0;
                    else
                        SelectedIndex = -1;
                    ScrollIndex = new Vector2();
                }
            }
            else
            {
                if (SearchTerms.IsNullOrEmpty() && CurrentCategory.IsNullOrEmpty())
                {
                    SetFirstHistoryCommandSelected();
                }
                else if (PreviousSearchTerms != SearchTerms)
                {
                    CommandResult = CommandManager.Instance.ActionByMatch(SearchTerms.Split(' '));

                    if (CommandResult == null)
                        CommandResult = CommandManager.Instance.AlwaysShownCommands;

                    DisplayNoResult = !CommandResult.Any();
                    SelectedIndex = 0;
                    ScrollIndex = new Vector2();

                }
            }
        }

        private bool ExecuteCommand(IEnumerable<CommandInfo> commands)
        {
            bool allExecuted = true;

            foreach (var command in commands)
            {
                if (command.IsValid)
                {
                    command.ExecuteCommand();
                }
                else
                {
                    allExecuted = false;
                }
            }

            return allExecuted;
        }

        #endregion

        #region DISPLAY

        private void InitDisplayWindow()
        {
            GUI.contentColor = Color.white;
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;

            if (MonkeyStyle.Instance.WindowBackgroundTex == null)
            {
                MonkeyStyle.Instance.PostInstanceCreation();
            }

            GUI.DrawTexture(new Rect(0, 0, 2000, 2000), MonkeyStyle.Instance.WindowBackgroundTex);
        }

        #endregion
    }
}