using MonKey.Editor.Console;
using MonKey.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonKey.Editor.Internal
{
    internal class ParametricCommandExecution
    {
        public CommandInfo Info;
        private int currentArrayIDEdited = -1;
        private readonly List<object> valuesParsed;

        private readonly List<List<string>> arrayTextEntered;
        private readonly List<List<object>> arrayValuesParsed;

        public int LastParameterCompleted { get; private set; } = -1;

        public bool HasNextVariable => CurrentParameterID + 1 < Info.CommandParameterInfo.Count;

        private CommandParameterInterpreter currentInterpreter;

        public object GetValueParsed(int index)
        {
            if (valuesParsed.Count > index && index >= 0)
            {
                return valuesParsed[index];
            }

            return null;
        }


        public string GetFormattedCurrentlyChosen()
        {
            if (!IsArray)
            {
                return CurrentTextEntered;
            }

            string[] split = CurrentTextEntered.Split(',');
            if (split.Length > 0)
            {
                return split.Last();
            }

            return "";
        }

        public List<string> TextEntered { get; }

        public CommandParameterInfo CurrentParameterInfo => (CurrentParameterID == -1) ? null :
            Info.CommandParameterInfo[CurrentParameterID];

        public string CurrentTextEntered { get; private set; }

        public int CurrentParameterID { get; private set; } = -1;

        public GenericCommandParameterAutoComplete CurrentAutoComplete { get; private set; }

        internal ParametricCommandExecution(CommandInfo info, CommandConsoleWindow window)
        {
            Info = info;
            TextEntered = new List<string>(info.CommandParameterInfo.Count);
            valuesParsed = new List<object>(info.CommandParameterInfo.Count);

            arrayValuesParsed = new List<List<object>>();
            arrayTextEntered = new List<List<string>>();

            foreach (var t in info.CommandParameterInfo)
            {
                TextEntered.Add("");
                valuesParsed.Add(null);
                arrayValuesParsed.Add(t.IsArray ?
                    new List<object>() : null);
                arrayTextEntered.Add(t.IsArray ?
                    new List<string>() : null);
            }
        }

        public bool IsParameterArray(int id)
        {
            return Info.CommandParameterInfo[id].IsArray;
        }

        public bool IsParameterEdited(int id)
        {
            return CurrentParameterID == id;
        }

        public bool IsParameterEntered(int id)
        {
            return id <= LastParameterCompleted;
        }

        public bool IsParameterDefault(int id)
        {
            return IsParameterEntered(id) && TextEntered[id] == "";
        }

        public bool IsParameterError(int id)
        {
            if (!IsParameterEntered(id))
            {
                return false;
            }

            if (Info.CommandParameterInfo[id].IsArray)
            {
                return arrayTextEntered[id].Count > 0
                       && arrayValuesParsed[id].Contains(null);
            }

            return (TextEntered[id] != "" ||
                    Info.CommandParameterInfo[id].PreventDefaultValueUsage)
                    && valuesParsed[id] == null;
        }

        public bool IsAutoCompleteSuggestions => CurrentAutoComplete != null
                                                 && CurrentAutoComplete.Count > 0;

        public List<string> CurrentArrayTextEntered { get; private set; }

        public int CurrentArrayIDEdited => currentArrayIDEdited;

        public bool IsArray => CurrentParameterID != -1 && CurrentParameterInfo.IsArray;

        public List<object> CurrentArrayValuesParsed { get; private set; }

        public int CurrentAutoCompleteID { get; private set; } = -1;

        internal bool ExecuteCommand(CommandConsoleWindow window)
        {
            ComputeCurrentParameterValue(window);
            CurrentParameterID = -1;
            window.SearchTerms = "";
            CurrentTextEntered = "";

            List<object> finalObjects = new List<object>(valuesParsed.Count);

            for (int i = 0; i < valuesParsed.Count; i++)
            {
                if (IsParameterError(i))
                {
                    CurrentParameterID = i;
                    PrepareParameterEdition();
                    return false;
                }

                if (!IsParameterEntered(i))
                {
                    CurrentTextEntered = "";
                    CurrentParameterID = i;
                    CurrentAutoCompleteID = -1;
                    ComputeCurrentParameterValue(window);
                }

                finalObjects.Add(valuesParsed[i]);

            }
            Info.ExecuteCommand(finalObjects.ToArray());
            return true;
        }

        public void NotifyFewInputs(string[] inputs, CommandConsoleWindow window)
        {
            for (var i = 0; i < inputs.Length; i++)
            {
                var input = inputs[i];
                NotifyNewInput(input, window);
                if (IsArray || i < inputs.Length - 1)
                {
                    NotifyNextVariable(window);
                }

                if (!IsArray)
                {
                    window.SearchTerms = inputs[i];
                }
            }
        }

        public void NotifyNewInput(string newInput, CommandConsoleWindow window)
        {
            if (newInput.IsNullOrEmpty())
            {
                CurrentTextEntered = "";
                CurrentAutoComplete?.InitializeAutoComplete();
            }
            else
            {
                AddNewArraySlotIfNeeded();
            }

            CurrentTextEntered = newInput;

            RegenerateAutoComplete();

            if (CurrentAutoComplete != null
                && (CurrentAutoCompleteID > CurrentAutoComplete.Count ||
                    CurrentAutoCompleteID == -1))
            {
                if (!newInput.IsNullOrEmpty() && CurrentAutoComplete.Count > 0
                    && Info.CommandParameterInfo[CurrentParameterID].ForceAutoCompleteUsage)
                {
                    CurrentAutoCompleteID = 0;
                    window.SelectedIndex = CurrentAutoCompleteID;
                }
                else
                {
                    CurrentAutoCompleteID = -1;
                    window.SelectedIndex = CurrentAutoCompleteID;
                }
            }
            window.Repaint();
        }

        private void AddNewArraySlotIfNeeded()
        {
            if (IsArray && (CurrentArrayIDEdited == -1
                            || CurrentArrayIDEdited >= CurrentArrayTextEntered.Count))
            {
                if (CurrentArrayIDEdited == -1)
                {
                    currentArrayIDEdited = 0;
                }

                CurrentArrayTextEntered.Add("");
                CurrentArrayValuesParsed.Add(null);
            }
        }

        public void NotifyInputFromAutoComplete(int newInputID)
        {
            if (CurrentAutoComplete == null
                || newInputID >= CurrentAutoComplete.Count)
            {
                return;
            }

            CurrentAutoCompleteID = newInputID;          
        }

        public void NotifyNextVariable(CommandConsoleWindow window)
        {
            if (IsArray)
            {
                if (!CurrentTextEntered.IsNullOrEmpty() || CurrentAutoCompleteID != -1)
                {
                    AddNewArraySlotIfNeeded();

                    JumpToArrayID(window, currentArrayIDEdited + 1);
                    return;
                }
            }

            if (CurrentParameterID != -1)
            {
                ComputeCurrentParameterValue(window);
            }

            if (HasNextVariable)
            {
                CurrentParameterID++;
            }
            else
            {
                window.TryExecuteCurrentParametricCommand();
                return;
            }



            PrepareParameterEdition();

            window.SelectedIndex = CurrentAutoCompleteID;

            window.TabFrames = 10;
            window.Repaint();
        }

        public void JumpToArrayID(CommandConsoleWindow window, int id)
        {
            if (IsArray)
            {
                ComputeCurrentArrayVariable(window);

                currentArrayIDEdited = id;

                CurrentTextEntered = CurrentArrayTextEntered.Count > CurrentArrayIDEdited ?
                    CurrentArrayTextEntered[CurrentArrayIDEdited] : "";

                //TODO add this as a option (has pros and cons)
                //       currentAutoComplete.InitializeAutoComplete();
                window.SelectedIndex = -1;
                window.SearchTerms = CurrentTextEntered;
                NotifyNewInput(CurrentTextEntered, window);
                window.Repaint();
                window.Focus();
                window.TabFrames = 10;
            }
        }

        public void NotifyPreviousVariable(CommandConsoleWindow window)
        {
            if (IsArray && CurrentArrayIDEdited > 0)
            {
                JumpToArrayID(window, CurrentArrayIDEdited - 1);
                return;
            }

            if (CurrentParameterID <= 0)
            {
                return;
            }

            ComputeCurrentParameterValue(window);

            LastParameterCompleted = Mathf.Max(CurrentParameterID, LastParameterCompleted);

            CurrentParameterID--;

            PrepareParameterEdition();

            window.SelectedIndex = CurrentAutoCompleteID;

            window.TabFrames = 10;

            window.Repaint();
        }

        public void JumpToFirstError(CommandConsoleWindow window)
        {
            for (int i = 0; i < valuesParsed.Count; i++)
            {
                if (IsParameterError(i))
                {
                    CurrentParameterID = i;
                    PrepareParameterEdition();
                    window.Focus();
                    window.TabFrames = 10;
                    window.Repaint();
                    break;
                }
            }
        }

        public void JumpToID(int id)
        {
            ComputeCurrentParameterValue(CommandConsoleWindow.CurrentPanel);
            CurrentParameterID = id;
            PrepareParameterEdition();
        }

        private void PrepareParameterEdition()
        {

            CommandParameterInfo paramInfo = Info.CommandParameterInfo[CurrentParameterID];

            currentInterpreter = CommandParameterInterpreter.
                GetInterpreter(paramInfo.ParameterType);
            CurrentTextEntered = TextEntered[CurrentParameterID];

            CurrentAutoComplete = paramInfo.HasAutoComplete
                ? paramInfo.AutoComplete
                : AutoCompleteManager.GetDefaultAutoComplete(IsArray ?
                paramInfo.ParameterType.GetElementType() :
                paramInfo.ParameterType);

            CurrentAutoComplete?.InitializeAutoComplete();

            CurrentAutoCompleteID = -1;

            if (!CurrentParameterInfo.PreventDefaultValueUsage && CurrentParameterInfo.HadDefaultValue && CurrentAutoComplete != null)
            {
                if (CurrentAutoComplete.Count == 0)
                {
                    CurrentAutoComplete.GenerateAndSortAutoComplete("");
                }

                for (int i = 0; i < CurrentAutoComplete.Count; i++)
                {
                    if (CurrentAutoComplete.GetValue(i).Equals(CurrentParameterInfo.DefaultValue))
                    //   if (CurrentAutoComplete.GetValue(i).ToString() == CurrentParameterInfo.DefaultValue.ToString())
                    {
                        CurrentAutoCompleteID = i;
                        break;
                    }
                }
            }

            if (IsArray)
            {
                CurrentArrayTextEntered = arrayTextEntered[CurrentParameterID];
                CurrentArrayValuesParsed = arrayValuesParsed[CurrentParameterID];
                currentArrayIDEdited = CurrentArrayTextEntered.Count - 1;
                if (CurrentArrayIDEdited >= 0)
                {
                    CurrentTextEntered = CurrentArrayTextEntered[CurrentArrayIDEdited];
                }
            }
            else
            {
                CurrentTextEntered = TextEntered[CurrentParameterID];
            }

            if (!CurrentTextEntered.IsNullOrEmpty())
            {
                RegenerateAutoComplete();
            }
            else
            {
                CurrentAutoComplete?.InitializeAutoComplete();
            }

            CommandConsoleWindow.CurrentPanel.SearchTerms = CurrentTextEntered;
            CommandConsoleWindow.CurrentPanel.PreviousSearchTerms = CurrentTextEntered;

        }

        private void RegenerateAutoComplete()
        {
            CurrentAutoComplete?.GenerateAndSortAutoComplete(CurrentTextEntered);
        }

        private void ComputeCurrentParameterValue(CommandConsoleWindow window)
        {
            if (CurrentParameterID == -1)
            {
                return;
            }

            LastParameterCompleted = CurrentParameterID;

            if (!IsArray)
            {
                TextEntered[CurrentParameterID] = CurrentTextEntered;

                object parsedObject = null;


                if (!CurrentParameterInfo.PreventDefaultValueUsage
                    && CurrentTextEntered.IsNullOrEmpty())
                {
                    parsedObject = CurrentParameterInfo.DefaultValue;
                    if (CurrentAutoComplete != null)
                    {
                        if (CurrentAutoCompleteID == -1 || CurrentAutoCompleteID >= CurrentAutoComplete.Count)
                        {
                            for (int i = 0; i < CurrentAutoComplete.Count; i++)
                            {

                                if (CurrentAutoComplete.GetValue(i) != null
                                    && CurrentAutoComplete.GetValue(i).ToString() == parsedObject.ToString())
                                {
                                    CurrentAutoCompleteID = i;
                                    break;
                                }
                            }
                        }
                        if (CurrentAutoCompleteID != -1 && CurrentAutoCompleteID < CurrentAutoComplete.Count)
                            parsedObject = CurrentAutoComplete.GetValue(CurrentAutoCompleteID);

                    }

                }
                else
                {
                    if (!Info.CommandParameterInfo[CurrentParameterID].ForceAutoCompleteUsage 
                        && CurrentAutoCompleteID==-1)
                    {
                        currentInterpreter.TryParse(CurrentTextEntered, out parsedObject);
                    }

                    if (IsAutoCompleteSuggestions
                        && parsedObject == null
                            && (Info.CommandParameterInfo[CurrentParameterID].ForceAutoCompleteUsage
                            || CurrentAutoCompleteID != -1))
                    {
                        if (CurrentAutoCompleteID == -1)
                        {
                            CurrentAutoCompleteID = 0;
                        }

                        if (CurrentAutoComplete != null)
                        {

                            if (CurrentAutoComplete.Count <= CurrentAutoCompleteID)
                                CurrentAutoComplete.GenerateAndSortAutoComplete(CurrentTextEntered);

                            parsedObject = CurrentAutoComplete.GetValue(CurrentAutoCompleteID);
                            TextEntered[CurrentParameterID] =
                                CurrentAutoComplete.GetStringValue(CurrentAutoCompleteID);
                        }
                        else
                        {
                            parsedObject = CurrentParameterInfo.DefaultValue;
                        }

                    }
                }



                if (parsedObject == null)
                {
                    valuesParsed[CurrentParameterID] = TextEntered[CurrentParameterID]
                        .IsNullOrEmpty() && !CurrentParameterInfo.PreventDefaultValueUsage
                        ? Info.CommandParameterInfo[CurrentParameterID].DefaultValue
                        : null;
                }
                else
                {
                    valuesParsed[CurrentParameterID] = parsedObject;
                }
            }
            else
            {
                ComputeCurrentArrayVariable(window);
                if (CurrentArrayValuesParsed != null && CurrentArrayValuesParsed.Count > 0)
                {
                    if (!CurrentArrayValuesParsed.Contains(null))
                    {
                        Type elemType = CurrentParameterInfo.ParameterType.GetElementType();
                        if (elemType != null)
                        {
                            var obj = Array.CreateInstance(elemType,
                                CurrentArrayValuesParsed.Count);
                            for (int i = 0; i < CurrentArrayValuesParsed.Count; i++)
                            {
                                obj.SetValue(CurrentArrayValuesParsed[i], i);
                            }
                            valuesParsed[CurrentParameterID] = obj;
                        }
                        else
                        {
                            valuesParsed[CurrentParameterID] = null;
                        }
                    }
                    else
                    {
                        valuesParsed[CurrentParameterID] = null;
                    }
                    arrayValuesParsed[CurrentParameterID] = CurrentArrayValuesParsed;
                }
                else if (!CurrentParameterInfo.PreventDefaultValueUsage
                        && CurrentParameterInfo.DefaultValue != null)
                {
                    valuesParsed[CurrentParameterID] = CurrentParameterInfo.DefaultValue;
                    Array ar = (Array)
                        CurrentParameterInfo.DefaultValue;

                    if (ar != null)
                    {
                        List<object> list = new List<object>();

                        foreach (object o in ar)
                        {
                            list.Add(o);
                        }

                        arrayValuesParsed[CurrentParameterID] = list;
                    }
                }
                else
                {
                    valuesParsed[CurrentParameterID] = null;
                    arrayValuesParsed[CurrentParameterID] = CurrentArrayValuesParsed;
                }
            }
        }

        private void ComputeCurrentArrayVariable(CommandConsoleWindow window)
        {
            if (currentArrayIDEdited < 0 || currentArrayIDEdited >= CurrentArrayTextEntered.Count)
            {
                return;
            }

            //TODO refactor this condition mess

            CurrentArrayTextEntered[CurrentArrayIDEdited] = CurrentTextEntered;

            object outputObj = null;

            if (!Info.CommandParameterInfo[CurrentParameterID].ForceAutoCompleteUsage
                && !CurrentTextEntered.IsNullOrEmpty())
            {
                currentInterpreter.TryParse(CurrentTextEntered, out outputObj);
            }

            if ((!CurrentTextEntered.IsNullOrEmpty() || CurrentAutoCompleteID != -1)
                && IsAutoCompleteSuggestions && outputObj == null
                && (Info.CommandParameterInfo[CurrentParameterID].ForceAutoCompleteUsage || CurrentAutoCompleteID != -1))
            {
                if (CurrentAutoCompleteID == -1)
                {
                    CurrentAutoCompleteID = 0;
                }

                CurrentArrayValuesParsed[CurrentArrayIDEdited] =
                    (CurrentAutoComplete.GetValue(CurrentAutoCompleteID));
                CurrentArrayTextEntered[CurrentArrayIDEdited] =
                    CurrentAutoComplete.GetStringValue(CurrentAutoCompleteID);
                CurrentAutoComplete.InitializeAutoComplete();
            }
            else
            {
                CurrentArrayValuesParsed[CurrentArrayIDEdited] = outputObj;
            }

            CurrentAutoCompleteID = -1;

            CurrentAutoComplete?.InitializeAutoComplete();

            window.SelectedIndex = -1;
            window.SearchTerms = "";
            window.PreviousSearchTerms = "";

            window.Repaint();
            window.Focus();
        }
    }
}
