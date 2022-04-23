using MonKey.Editor.Internal;
using MonKey.Extensions;
using MonKey.Internal;
using MonKey.Settings.Internal;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MonKey.Editor.Console
{
    internal static class CommandDisplay
    {
        internal static void DisplayCommandInSearch(CommandInfo x, int index,
            int selectedIndex, bool mouseActivity, bool control, string searchTerms)
        {

            int height = x.CommandHelp.IsNullOrEmpty()
                ? MonkeyStyle.Instance.CommandGroupNoHelpHeight
                : MonkeyStyle.Instance.CommandGroupHeight;

            bool selected = InitResultSelectionStyle(index, selectedIndex, mouseActivity, height);

            EditorGUILayout.BeginVertical(MonkeyStyle.Instance.CommandResultInsideLayoutStyle,
                GUILayout.ExpandHeight(true));

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(MonkeyStyle.Instance.CommandResultGroupStyle);

            EditorGUILayout.BeginVertical(MonkeyStyle.Instance.ParamIconGroupStyle,
                GUILayout.Height(height), GUILayout.ExpandHeight(true));
            GUILayout.FlexibleSpace();

            if (x.IsMenuItem)
            {
                GUILayout.Label("",
                    new GUIStyle()
                    {
                        fixedWidth = 18,
                        fixedHeight = 16,
                        alignment = TextAnchor.MiddleCenter,
                        normal = {background =
                            MonkeyStyle.GetIconForFile("fake.unity")}
                    });
            }
            else
            {
                if (x.IsParametric && (!control || !selected || !x.CanUseQuickDefaultCall))
                    GUILayout.Label("", (selected) ? MonkeyStyle.Instance.ParamIconSelectedStyle :
                        MonkeyStyle.Instance.ParamIconStyle);
                else
                {
                    GUILayout.Label("", (selected) ? MonkeyStyle.Instance.NonParamIconSelectedStyle :
                        MonkeyStyle.Instance.NonParamIconStyle);
                }
            }


            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            GUILayout.FlexibleSpace();

            DisplayCommandTitle(selected, x, searchTerms);

            DisplayCommandHelp(x, selected);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(MonkeyStyle.Instance.CommandHotKeyStyle,
                GUILayout.ExpandHeight(true));

            if (DisplayCommandValidationMessage(x, selected, control) || !selected)
            {
                if (Event.current.type != EventType.MouseMove)
                {
                    GUILayout.FlexibleSpace();
                    DisplayCommandHotKey(x, selected);
                    GUILayout.FlexibleSpace();
                }
            }

            EditorGUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSearchResultLine1Style);
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSearchResultLine2Style);
            GUILayout.Label("", MonkeyStyle.Instance.HorizontalSearchResultLine3Style);

            EditorGUILayout.EndVertical();

            if (selected)
            {
                CommandConsoleWindow.RowHeight = GUILayoutUtility.GetLastRect();
            }
        }

        internal static bool InitResultSelectionStyle(int index, int selectedIndex,
            bool mouseActivity, float minHeight)
        {

            bool selected = false;
            if (mouseActivity)
            {
                EditorGUILayout.BeginVertical(MonkeyStyle.Instance.CommandResultLayoutStyle, GUILayout.ExpandHeight(true)
                    , GUILayout.MinHeight(minHeight));
                selected = true;
            }
            else if (index == selectedIndex)
            {
                EditorGUILayout.BeginVertical(MonkeyStyle.Instance.CommandResultLayoutForcedHighlightStyle,
                    GUILayout.ExpandHeight(true), GUILayout.MinHeight(minHeight));
                selected = true;
            }
            else
                EditorGUILayout.BeginVertical(MonkeyStyle.Instance.CommandResultLayoutNoHighlightStyle,
                    GUILayout.ExpandHeight(true), GUILayout.MinHeight(minHeight));
            return selected;
        }

        internal static void DisplayCommandHelp(CommandInfo x, bool selected, bool parametric = false)
        {

            if (MonKeyInternalSettings.Instance.ShowHelpOnlyOnActiveCommand && !selected)
            {
                GUILayout.Label("",MonkeyStyle.Instance.CommandHelpStyle);
            }
            else
            {

                if (!x.CommandHelp.IsNullOrEmpty() || x.IsMenuItem)
                {
                    if (x.IsMenuItem)
                    {
                        GUILayout.Label(MonKeyLocManager.CurrentLoc.MenuItemAddedByMonkey,
                            selected
                                ? MonkeyStyle.Instance.CommandHelpStyleSelected
                                : MonkeyStyle.Instance.CommandHelpStyle);
                    }
                    else if (!parametric)
                        GUILayout.Label(x.CommandHelp,
                            selected
                                ? MonkeyStyle.Instance.CommandHelpStyleSelected
                                : MonkeyStyle.Instance.CommandHelpStyle);
                    else
                    {
                        GUILayout.Label(x.CommandHelp, MonkeyStyle.Instance.CommandHelpParametricStyle);
                    }
                }
            }
        }

        internal static bool DisplayCommandHotKey(CommandInfo x, bool selected)
        {
            if (x.HotKey != null)
            {
                bool conflict = (x.IsConflictual);

                int height = x.CommandHelp.IsNullOrEmpty()
                    ? MonkeyStyle.Instance.CommandGroupNoHelpHeight
                    : MonkeyStyle.Instance.CommandGroupHeight;

                GUILayout.BeginHorizontal(GUILayout.Height(height));

                GUILayout.Label(x.HotKey.FormattedKeys,
                    conflict ? MonkeyStyle.Instance.CommandConflictHotKeyStyle :
                        selected ? MonkeyStyle.Instance.CommandHotKeySelectedStyle :
                        MonkeyStyle.Instance.CommandHotKeyStyle);
                if (selected && conflict)
                {
                    StringBuilder conflicts = new StringBuilder();
                    conflicts.Append(MonKeyLocManager.CurrentLoc.ConflictWith);
                    foreach (var com in x.ConflictualCommands)
                    {
                        conflicts.Append('"');
                        conflicts.Append(com.CommandName);
                        conflicts.Append('"');
                        conflicts.Append(",");
                    }
                    conflicts.Remove(conflicts.Length - 1, 1);

                    GUILayout.Label(conflicts.ToString(),
                        MonkeyStyle.Instance.CommandConflictHotKeyStyle);

                    GUILayout.BeginHorizontal(MonkeyStyle.Instance.WarningIconGroupStyle);
                    GUILayout.Label("", MonkeyStyle.Instance.WarningIconStyle);
                    GUILayout.EndHorizontal();
                    GUILayout.EndHorizontal();

                    return true;
                }

                GUILayout.EndHorizontal();

                return false;
            }

            GUILayout.Label(MonKeyLocManager.CurrentLoc.NoHotKey,
                MonkeyStyle.Instance.CommandHotKeyStyle);
            return false;

        }

        internal static bool DisplayCommandValidationMessage(CommandInfo x, bool selected, bool forced)
        {
            if (x.HasValidation)
            {
                if (!x.IsValid && selected)
                {
                    int height = x.CommandHelp.IsNullOrEmpty()
                        ? MonkeyStyle.Instance.CommandGroupNoHelpHeight
                        : MonkeyStyle.Instance.CommandGroupHeight;

                    if (Event.current.type == EventType.MouseMove)
                        return false;

                    GUILayout.BeginHorizontal(GUILayout.Height(height));

                    GUILayout.Label(
                        !x.CommandValidationMessage.IsNullOrEmpty()
                            ? x.CommandValidationMessage
                            : MonKeyLocManager.CurrentLoc.CommandValidationFailed,
                        MonkeyStyle.Instance.ValidationStyle, GUILayout.Height(height));

                    GUILayout.BeginVertical(MonkeyStyle.Instance.WarningIconGroupStyle, GUILayout.Height(height));
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("", MonkeyStyle.Instance.WarningIconStyle);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                    return false;
                }
            }

            return true;
        }

        internal static void DisplayCommandTitle(bool selected, CommandInfo x, string searchTerms)
        {
            string commandTitle = x.CommandName;

            if (searchTerms == null)
                commandTitle = commandTitle.Colored(MonkeyStyle.Instance.HighlightOnSelectedTextColor);
            else
            {
                commandTitle = commandTitle.Highlight(searchTerms, true,
                    StringExt.ColorTag(MonkeyStyle.Instance.SearchResultTextColor), StringExt.ColorTagClosing,
                    StringExt.ColorTag(MonkeyStyle.Instance.HighlightOnSelectedTextColor),
                    StringExt.ColorTagClosing);
            }

            commandTitle = AppendQuickName(x, commandTitle, selected, searchTerms);

            GUILayout.Label(commandTitle.Bold(),
                MonkeyStyle.Instance.CommandNameStyle);
        }

        internal static string AppendQuickName(CommandInfo x, string commandTitle, bool selected, string searchTerms)
        {
            if (x.HasQuickName)
            {
                string quick = (x.CommandQuickName).ToUpper();
                if (searchTerms != null)
                    quick = quick.Highlight(searchTerms, true,
                        StringExt.ColorTag(MonkeyStyle.Instance.QuickNameTextColor),
                        StringExt.ColorTagClosing,
                         StringExt.ColorTag(MonkeyStyle.Instance.HighlightOnSelectedTextColor),
                           StringExt.ColorTagClosing);
                else
                {
                    quick = quick.Colored(MonkeyStyle.Instance.QuickNameTextColor);
                }
                commandTitle = commandTitle.Insert(0, quick + " | ".Colored(MonkeyStyle.Instance.CommandHelpTextColor));
            }
            return commandTitle;
        }
    }
}

