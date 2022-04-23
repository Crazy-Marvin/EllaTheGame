using System;
using System.Linq;
using MonKey.Editor.Internal;
using MonKey.Extensions;
using MonKey.Internal;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MonKey.Editor.Console
{
    internal static class ParametricPanelDisplay
    {
        private static readonly int ArrayIdBreakout = 8;

        internal static void DisplayParametricPanel(CommandConsoleWindow window)
        {
            try
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    window.Repaint();
                    window.Focus();
                    window.TabFrames = 10;
                }

                CommandConsoleWindow.RowHeight = new Rect(0, 0, 0, 0.55f);

                DisplayCommandInfo(window.CurrentExecution);

                Rect selectedRect = new Rect();
                if (window.CurrentExecution.Info.CommandParameterInfo.Count > 1)
                    selectedRect = DisplayParameterTabs(window);

                window.CheckSearch();

                if (window.CurrentExecution.CurrentParameterInfo != null)
                {
                    DisplayParameterPanel(window);
                }

                if (window.CurrentExecution.Info.CommandParameterInfo.Count > 1)
                    DisplayTabLink(selectedRect);

                DisplayBottomHelp(window);
            }
            catch (Exception e)
            {
                if (CommandConsoleWindow.LogExceptions)
                    Debug.Log(e);
                GUIUtility.ExitGUI();
            }
        }

        private static void DisplayTabLink(Rect selectedRect)
        {
            Rect overlayForTab = new Rect(selectedRect.x,
                selectedRect.y + selectedRect.height, selectedRect.width, 3);
            GUI.DrawTexture(overlayForTab, MonkeyStyle.Instance.SelectedVariableTex);
        }

        private static void DisplayBottomHelp(CommandConsoleWindow window)
        {
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideLineStyle);
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideSecondLineStyle);
            EditorGUILayout.BeginHorizontal(MonkeyStyle.Instance.HelpStyle);
            GUILayout.FlexibleSpace();
            CommandSearchPanelDisplay.DisplayParametricCommandTip(window);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        internal static void DisplayCommandInfo(ParametricCommandExecution execution)
        {
            GUILayout.BeginHorizontal(MonkeyStyle.Instance.TopSearchParametricPanelStyle);

            GUILayout.BeginVertical(MonkeyStyle.Instance.MonkeyLogoParametricGroupStyle);
            GUILayout.Label("", MonkeyStyle.Instance.MonkeyLogoStyleHappy);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            DisplayLogoTop();
            DisplayCommandDetails(execution);

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideSecondLineStyle);
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSideLineStyle);

        }

        private static void DisplayCommandDetails(ParametricCommandExecution execution)
        {
            GUILayout.BeginVertical(MonkeyStyle.Instance.ParametricMethodMethodGroup,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            CommandDisplay.DisplayCommandTitle(false, execution.Info, null);
            CommandDisplay.DisplayCommandHelp(execution.Info, false, true);

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        private static void DisplayLogoTop()
        {
            EditorGUILayout.BeginHorizontal(MonkeyStyle.Instance.NameLogoStyleGroup);
            GUILayout.FlexibleSpace();
            GUILayout.Label("", MonkeyStyle.Instance.ParametricNameLogoStyle);
            EditorGUILayout.EndHorizontal();
        }

        internal static Rect DisplayParameterTabs(CommandConsoleWindow window)
        {
            MonkeyStyle monkeyStyle = MonkeyStyle.Instance;

            GUILayout.BeginVertical(monkeyStyle.ParametricWindowBackgroundStyle);

            int i = 0;

            GUILayout.BeginHorizontal();
            int current = window.CurrentExecution.CurrentParameterID;

            Rect selectedRect = new Rect(0, 0, 0, 0);

            foreach (var info in window.CurrentExecution.Info.CommandParameterInfo)
            {

                GUIStyle panelStyle = current == i
                    ? monkeyStyle.VariableSelectedGroupStyle
                    : monkeyStyle.VariableNonSelectedGroupStyle;

                GUILayout.BeginHorizontal(monkeyStyle.VariableGroupStyle);

                GUILayout.Label("", monkeyStyle.ParametricTabShadow2Style);
                GUILayout.Label("", monkeyStyle.ParametricTabShadow1Style);

                GUILayout.BeginVertical(panelStyle, GUILayout.ExpandHeight(true));

                if (current == i)
                {
                    GUILayout.Label("", monkeyStyle.ParametricTabOutline1HorizontalStyle);
                    GUILayout.Label("", monkeyStyle.ParametricTabOutline2HorizontalStyle);
                }
                else
                {
                    GUILayout.Label("", monkeyStyle.ParametricTabOutlineUnfocusedHorizontalStyle);
                }

                GUIStyle variableStyle;
                if (window.CurrentExecution.IsParameterError(i))
                {
                    variableStyle = monkeyStyle.VariableNameErrorTextStyle;
                }
                else
                {
                    variableStyle = current == i
                        ? monkeyStyle.VariableNameSelectedTextStyle
                        : monkeyStyle.VariableNameNonSelectedTextStyle;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label(ObjectNames.NicifyVariableName(info.Name).Bold()
                    , variableStyle);

                if (window.CurrentExecution.IsParameterError(i))
                    GUILayout.Label("", monkeyStyle.ParameterWarningIconStyle);

                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                if (current == i)
                {
                    GUILayout.Label("", monkeyStyle.ParametricTabOutline2VerticalStyle);
                    GUILayout.Label("", monkeyStyle.ParametricTabOutline1VerticalStyle);
                }
                else
                {
                    GUILayout.Label("", monkeyStyle.ParametricTabOutlineUnfocusedVerticalStyle);
                }

                GUILayout.EndHorizontal();

                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) &&
                    Event.current.type == EventType.MouseDown)
                {
                    window.CurrentExecution.JumpToID(i);
                    window.Repaint();
                }

                if (current == i)
                {
                    selectedRect = GUILayoutUtility.GetLastRect();
                }

                i++;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            return selectedRect;
        }

        internal static void DisplayParameterPanel(CommandConsoleWindow window)
        {

            const string controlName = "searchParameter";

            GUILayout.BeginVertical(MonkeyStyle.Instance.VariablePanelStyle,
                GUILayout.ExpandWidth(true));
            GUILayout.Label("", MonkeyStyle.Instance.ParametricTabOutline1HorizontalStyle);
            GUILayout.Label("", MonkeyStyle.Instance.ParametricTabOutline2HorizontalStyle);

            GUILayout.BeginHorizontal(new GUIStyle { margin = new RectOffset(10, 10, 20, 10) });

            DisplayHelpPanel(window);

            DisplayAutoCompleteSection(window, controlName);

            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();

            window.Repaint();

            if (window.TabFrames > 0 && window.TabFrames < 5)
            {
                CommandConsoleWindow.ForceEditSearchAtEnd(window.SearchTerms);
                GUI.FocusControl(controlName);
            }

        }

        private static void DisplayAutoCompleteSection(CommandConsoleWindow window,
            string controlName)
        {
            ParametricCommandExecution exec = window.CurrentExecution;

            GUILayout.BeginVertical(MonkeyStyle.Instance.SearchLabelGroupStyle);


            if (exec.IsArray && exec.CurrentTextEntered.IsNullOrEmpty()
                             && exec.CurrentAutoCompleteID == -1)
            {
                if (exec.HasNextVariable)
                {
                    GUILayout.Label("Press TAB or ENTER to go to the next variable",
                        MonkeyStyle.Instance.VariableTypeTextStyle, GUILayout.ExpandWidth(true));
                }
                else
                {
                    GUILayout.Label("Press TAB or ENTER to go to execute the command",
                        MonkeyStyle.Instance.VariableTypeTextStyle, GUILayout.ExpandWidth(true));
                }
            }

            GUILayout.BeginHorizontal(MonkeyStyle.Instance.SearchLabelStyle);
            GUILayout.BeginHorizontal(MonkeyStyle.Instance.AutoCompleteSearchLabelGroupStyle);

            DisplayInstructionOrDefault(window, exec);

            GUI.SetNextControlName(controlName);
            window.SearchTerms = EditorGUIExt.TextField(window.SearchTerms, controlName,
                MonkeyStyle.Instance.SearchLabelStyle);

            if (!window.IsDocked || window.JustOpenedActiveMode ||
                window.PreventSearchMovement || window.Focused)
            {
                if (window.PreventSearchMovement)
                    CommandConsoleWindow.ForceEditSearchAtEnd(window.SearchTerms);
                window.Focus();

                GUI.FocusControl(controlName);
            }

            //    GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();

            ComputeDragAndDrop(window);

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            DisplayAutoCompleteOptions(window);

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
        }

        private static bool startedDrag = false;

        private static void ComputeDragAndDrop(CommandConsoleWindow window)
        {
            Type currentParameterType = window.CurrentExecution.CurrentParameterInfo.ParameterType;
            if (window.CurrentExecution.CurrentParameterInfo.IsArray)
                currentParameterType = currentParameterType.GetElementType();

            if (!currentParameterType.IsSubclassOf(typeof(UnityEngine.Object)) &&
                currentParameterType != typeof(string))
                return;

            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetLastRect();

            bool isDrag = EventType.DragUpdated == evt.type || evt.type == EventType.DragPerform;

            if (isDrag && !startedDrag)
            {
                startedDrag = true;
            }

            bool stringParam = currentParameterType == typeof(string);
            string message = stringParam ? "Drop Your Asset Here!"
                : "Drop Your Scene Object Here!";

            GUI.Box(dropArea, startedDrag ? message : "", new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    background = MonkeyStyle.ColorTexture(1,1,startedDrag?
                            Color.white.Alphaed(1f) : Color.white.Alphaed(0) )
                }
            });

            if (isDrag)
            {
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                bool validDragAndDrop = false;

                if (stringParam)
                {
                    validDragAndDrop = !DragAndDrop.objectReferences.Where(_ => _ is GameObject)
                        .Any(_ => ((GameObject)_).scene.IsValid());

                }
                else
                {
                    validDragAndDrop =
                        DragAndDrop.objectReferences
                            .All(_ => _ is GameObject && ((GameObject)_).scene.IsValid());
                }

                DragAndDrop.visualMode = validDragAndDrop ? DragAndDropVisualMode.Copy
                    : DragAndDropVisualMode.Rejected;

                if (evt.type == EventType.DragPerform && validDragAndDrop)
                {
                    if (window.CurrentExecution.CurrentParameterInfo.IsArray)
                        window.CurrentExecution.NotifyFewInputs(DragAndDrop.objectReferences.Convert(_ => _.name).ToArray(), window);
                    else
                    {
                        window.SearchTerms = DragAndDrop.objectReferences[0].name;
                        window.CurrentExecution.NotifyNewInput(DragAndDrop.objectReferences[0].name, window);
                    }
                    DragAndDrop.AcceptDrag();

                }
            }
            else if (evt.type == EventType.DragExited)
            {
                startedDrag = false;
            }
        }

        private static void DisplayInstructionOrDefault(CommandConsoleWindow window,
            ParametricCommandExecution exec)
        {

            bool customInstruction = exec.CurrentAutoComplete != null
                                     && !exec.CurrentAutoComplete.SearchInstruction.IsNullOrEmpty()
                                     && exec.CurrentParameterInfo.PreventDefaultValueUsage;
            string textEntered = null;
            if (exec.CurrentAutoCompleteID == -1)
            {
                if (!customInstruction && exec.CurrentTextEntered.IsNullOrEmpty())
                {
                    if (exec.CurrentParameterInfo.PreventDefaultValueUsage
                        || exec.CurrentParameterInfo.DefaultValueName.IsNullOrEmpty())
                    {
                        textEntered = MonKeyLocManager.CurrentLoc.NoValue;
                    }
                    else
                    {
                        textEntered = exec.CurrentParameterInfo.DefaultValueName;
                        textEntered += " ( ";
                        textEntered += MonKeyLocManager.CurrentLoc.Default;
                        textEntered += " )";
                    }
                }

                textEntered = customInstruction ? exec.CurrentAutoComplete.SearchInstruction : textEntered;
            }
            else if (exec.CurrentAutoComplete != null && exec.CurrentAutoCompleteID < exec.CurrentAutoComplete.Count)
                textEntered = exec.CurrentAutoComplete.GetStringValue(exec.CurrentAutoCompleteID);

            bool noValue = customInstruction &&
                           textEntered == exec.CurrentAutoComplete.SearchInstruction;

            if (!textEntered.IsNullOrEmpty())
                GUILayout.Label(textEntered + " | ",
                    noValue ? MonkeyStyle.Instance.SearchLabelHelpStyle : MonkeyStyle.Instance.SearchLabelStyle,
                    GUILayout.ExpandWidth(false));
        }

        private static void DisplayArrayIds(CommandConsoleWindow window, ParametricCommandExecution exec)
        {
            GUILayout.Label("", MonkeyStyle.Instance.ParametricTabUnderline1Style);
            GUILayout.Label("", MonkeyStyle.Instance.ParametricTabUnderline2Style);

            string textEntered = "";

            if (exec.IsArray)
                textEntered = MonKeyLocManager.CurrentLoc.CurrentArrayValues;
            else
            {
                if (exec.IsParameterError(exec.CurrentParameterID))
                {
                    textEntered = (MonKeyLocManager.CurrentLoc.Error)
                        .Colored(MonkeyStyle.Instance.VariableValueErrorTextColor);
                }
            }

            GUILayout.Label(textEntered, MonkeyStyle.Instance.VariableValueSelectedTextStyle);

            GUILayout.BeginHorizontal();

            if (exec.IsArray)
            {
                GUILayout.BeginHorizontal(new GUIStyle()
                {
                    margin = new RectOffset(2, 2, 2, 2),
                    normal = { background = MonkeyStyle.Instance.HelpVariableTex }
                });

                int startValue = exec.CurrentArrayIDEdited == -1 ? -1 : 0;

                int columnCount = 0;
                for (int i = startValue;
                    i < Mathf.Max(exec.CurrentArrayTextEntered.Count,
                        exec.CurrentArrayIDEdited + 1);
                    i++)
                {
                    GUIStyle style;
                    if (i == exec.CurrentArrayIDEdited && i != -1)
                        style = MonkeyStyle.Instance.ArrayVariableSelectedGroupStyle;
                    else
                    {
                        if (i >= exec.CurrentArrayTextEntered.Count || i == -1)
                        {
                            style = MonkeyStyle.Instance.ArrayVariableNewGroupStyle;
                        }
                        else
                        {
                            style = MonkeyStyle.Instance.ArrayVariableNonSelectedGroupStyle;
                        }
                    }

                    GUILayout.BeginHorizontal(style);

                    GUILayout.Label((i >= exec.CurrentArrayTextEntered.Count || i == -1 ? " (New)" : i.ToString())
                        , MonkeyStyle.Instance.VariableTypeTextStyle);

                    if (i >= 0 && exec.CurrentArrayTextEntered.Count > i &&
                        !exec.CurrentArrayTextEntered[i].IsNullOrEmpty()
                        && exec.CurrentArrayValuesParsed[i] == null)
                    {
                        GUILayout.Label("", MonkeyStyle.Instance.ArrayWarningIconStyle);
                    }

                    GUILayout.EndHorizontal();

                    if (Event.current.type == EventType.MouseDown &&
                        GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        exec.JumpToArrayID(window, i);
                    }

                    if (i - columnCount * ArrayIdBreakout >= ArrayIdBreakout)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.BeginHorizontal(new GUIStyle()
                        {
                            margin = new RectOffset(2, 2, 2, 2),
                            normal = { background = MonkeyStyle.Instance.HelpVariableTex }
                        });
                        columnCount++;
                    }
                }

                if (exec.CurrentArrayTextEntered.Count == 0 && exec.CurrentArrayIDEdited != -1)
                    GUILayout.Label("None", MonkeyStyle.Instance.VariableTypeTextStyle);

                GUILayout.EndHorizontal();
            }

            GUILayout.EndHorizontal();
        }

        private static void DisplayHelpPanel(CommandConsoleWindow window)
        {
            ParametricCommandExecution exec = window.CurrentExecution;
            GUILayout.BeginVertical(GUILayout.MinWidth(70),
                GUILayout.MaxWidth(200), GUILayout.ExpandHeight(true));

            GUILayout.BeginVertical(MonkeyStyle.Instance.VariableHelpGroupStyle);

            GUILayout.Label(exec.CurrentParameterInfo.
                    Name.NicifyVariableName().Bold()
                , MonkeyStyle.Instance.VariableTypeTextStyle);

            GUILayout.Label("", MonkeyStyle.Instance.ParametricTabUnderline1Style);
            GUILayout.Label("", MonkeyStyle.Instance.ParametricTabUnderline2Style);

            CommandParameterInfo info = exec.CurrentParameterInfo;
            string parameterTypeName = info.HasTypeNameOverride ?
                info.ParameterTypeNameOverride :
                MonkeyStyle.PrettifyTypeName(exec.CurrentParameterInfo.ParameterType);

            GUILayout.Label(MonKeyLocManager.CurrentLoc.Type + parameterTypeName
                , MonkeyStyle.Instance.VariableTypeTextStyle);

            GUILayout.Label("", MonkeyStyle.Instance.ParametricTabUnderline1Style);
            GUILayout.Label("", MonkeyStyle.Instance.ParametricTabUnderline2Style);

            if (!info.Help.IsNullOrEmpty())
            {
                GUILayout.Label(info.Help, MonkeyStyle.Instance.VariableHelpTextStyle);
            }

            GUILayout.EndVertical();

            DisplayArrayIds(window, exec);

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
        }

        private static void DisplayAutoCompleteOptions(CommandConsoleWindow window)
        {
            if (!window.CurrentExecution.IsAutoCompleteSuggestions)
                return;

            if (window.MouseActivity)
                window.MouseOverField = false;

            GUI.skin = MonkeyStyle.Instance.MonkeyScrollBarStyle;

            window.ScrollIndex = EditorGUILayout.BeginScrollView(window.ScrollIndex, new GUIStyle()
            {
                normal = { background = MonkeyStyle.Instance.ResultFieldTex }
            });

            for (int i = 0; i < window.CurrentExecution.CurrentAutoComplete.Count; i++)
            {
                bool selected = InitResultSelectionStyle(i, window.SelectedIndex,
                    window.MouseActivity);

                GUILayout.Label("", MonkeyStyle.Instance.AutoCompleteLine1Style);

                if (!selected)
                    GUILayout.Label("", MonkeyStyle.Instance.AutoCompleteLine2Style);
                else
                {
                    GUILayout.Label("", MonkeyStyle.Instance.AutoCompleteLine1Style);
                }

                GUILayout.BeginVertical(new GUIStyle { margin = new RectOffset(2, 2, 5, 5) });

                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                DisplayAutoCompleteField(window.CurrentExecution.CurrentAutoComplete,
                    window.CurrentExecution.GetFormattedCurrentlyChosen(), selected, i);
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.EndVertical();

                if ((window.MouseActivity || Event.current.type == EventType.MouseDown) &&
                    GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {

                    if (Event.current.type == EventType.MouseDown)
                    {
                        window.SelectedIndex = i;
                        window.CurrentExecution.
                            NotifyInputFromAutoComplete(window.SelectedIndex);
                        window.CurrentExecution.NotifyNextVariable(window);
                    }
                    window.MouseOverField = true;
                    window.SelectedIndex = i;
                    window.Repaint();
                }

                if (selected)
                {
                    window.Repaint();
                }
            }
            EditorGUILayout.EndScrollView();

            GUI.skin = MonkeyStyle.Instance.DefaultStyle;
        }

        private static bool InitResultSelectionStyle(int index, int selectedIndex,
            bool mouseActivity)
        {
            bool selected = false;
            if (mouseActivity)
            {
                GUILayout.BeginVertical(MonkeyStyle.Instance.AutoCompleteResultLayoutStyle);
                selected = true;
            }
            else if (index == selectedIndex)
            {
                GUILayout.BeginVertical(MonkeyStyle.Instance.AutoCompleteResultLayoutForcedHighlightStyle);
                selected = true;
            }
            else
                GUILayout.BeginVertical(MonkeyStyle.Instance.AutoCompleteResultLayoutNoHighlightStyle);
            return selected;
        }

        private static void DisplayAutoCompleteField(
            GenericCommandParameterAutoComplete autoComplete,
            string searchTerms, bool selected, int id)
        {

            autoComplete.DrawAutoCompleteMember(id, searchTerms, selected);
        }


    }
}

