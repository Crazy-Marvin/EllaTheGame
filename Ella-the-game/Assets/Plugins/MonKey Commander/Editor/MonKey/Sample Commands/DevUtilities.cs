
using MonKey.Editor.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace MonKey.Editor.Commands
{
    public class DevUtilities
    {

        [Command("Find Missing Scripts", QuickName = "FMS",
             Help = "Finds all the missing scripts attached to selected game objects",
            DefaultValidation = DefaultValidation.AT_LEAST_ONE_OBJECT,
            Category = "Dev")]
        public static void FindMissingScripts()
        {
            int miss = 0;
            foreach (GameObject g in Selection.gameObjects)
            {
                miss += FindMissingScripts(g, Selection.gameObjects);
            }
            if (miss == 0)
                Debug.Log("Monkey found no missing scripts in the selection :)");
            else
            {
                Debug.LogFormat("Monkey found a total of {0} scripts missing", miss);
            }
        }

        public static int FindMissingScripts(GameObject gameObject, GameObject[] selection)
        {
            int totalMiss = 0;
            foreach (var component in gameObject.GetComponents<Component>())
            {
                if (component == null)
                {
                    Debug.LogWarningFormat(gameObject, "Missing Component Found in '{0}'",
                        gameObject.name);
                    totalMiss++;
                }
            }

            foreach (Transform t in gameObject.transform)
            {
                if (!selection.Contains(t.gameObject))
                    totalMiss += FindMissingScripts(t.gameObject, selection);
            }

            return totalMiss;
        }

        [Command("Set String Value", "Sets the value of a string field of a given name in a given type")]
        public static void SetStringValue(
            [CommandParameter(Help = "The type of script on which to change a value",AutoCompleteMethodName = "TypeAuto",ForceAutoCompleteUsage = true,PreventDefaultValueUsage = true)]
            Type scriptType,
            [CommandParameter(Help = "The name of the field the change the value of"
                ,AutoCompleteMethodName = "StringFieldAuto",ForceAutoCompleteUsage = true,PreventDefaultValueUsage = true)]
            string fieldName, string value)
        {
            List<UnityEngine.Object> objectsToChange = new List<UnityEngine.Object>();

            foreach (var o in Selection.objects)
            {
                if (o.GetType() == scriptType)
                {
                    objectsToChange.Add(o);
                }
                else if (o is GameObject go)
                {
                    objectsToChange.AddRange(go.GetComponentsInChildren(scriptType));
                }
            }

            PropertyInfo propInfo = null;
            var info = scriptType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Default);
            if (info == null)
                propInfo = scriptType.GetProperty(fieldName,
                     BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                     BindingFlags.FlattenHierarchy | BindingFlags.Default);

            if (info == null && propInfo == null)
            {
                Debug.LogWarning("MonKey - Error when attempting to change the value of a field, the value wasn't changed");
                return;
            }

            int undoID = MonkeyEditorUtils.CreateUndoGroup("MonKey - Set Float Value");

            foreach (var o in objectsToChange)
            {
                Undo.RecordObject(o, "modify Value");
                info?.SetValue(o, value);
                propInfo?.SetValue(o, value, null);
                EditorUtility.SetDirty(o);
            }
            Undo.CollapseUndoOperations(undoID);
        }

        public static TypeAutoComplete TypeAuto()
        {
            return AutoCompleteManager.TypeAutoCompleteFromSelection();
        }

        public static StaticCommandParameterAutoComplete StringFieldAuto()
        {
            return AutoCompleteManager.FieldAutoComplete(0, typeof(string));
        }

        [Command("Set Bool Value", "Sets the value of a boolean field of a given name in a given type")]
        public static void SetBoolValue(
         [CommandParameter(Help = "The type of script on which to change a value",AutoCompleteMethodName = "TypeAuto",ForceAutoCompleteUsage = true,PreventDefaultValueUsage = true)]
            Type scriptType,
         [CommandParameter(Help = "The name of the field the change the value of"
                ,AutoCompleteMethodName = "BoolFieldAuto",ForceAutoCompleteUsage = true,PreventDefaultValueUsage = true)]
            string fieldName, bool value)
        {
            List<UnityEngine.Object> objectsToChange = new List<UnityEngine.Object>();

            foreach (var o in Selection.objects)
            {
                if (o.GetType() == scriptType)
                {
                    objectsToChange.Add(o);
                }
                else if (o is GameObject go)
                {
                    objectsToChange.AddRange(go.GetComponentsInChildren(scriptType));
                }
            }

            PropertyInfo propInfo = null;
            var info = scriptType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Default);
            if (info == null)
                propInfo = scriptType.GetProperty(fieldName,
                     BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                     BindingFlags.FlattenHierarchy | BindingFlags.Default);

            if (info == null && propInfo == null)
            {
                Debug.LogWarning("MonKey - Error when attempting to change the value of a field, the value wasn't changed");
                return;
            }

            int undoID = MonkeyEditorUtils.CreateUndoGroup("MonKey - Set bool Value");

            foreach (var o in objectsToChange)
            {
                Undo.RecordObject(o, "modify Value");
                info?.SetValue(o, value);


                propInfo?.SetValue(o, value, null);
                EditorUtility.SetDirty(o);
            }

            Undo.CollapseUndoOperations(undoID);
        }

        public static StaticCommandParameterAutoComplete BoolFieldAuto()
        {
            return AutoCompleteManager.FieldAutoComplete(0, typeof(bool));
        }



        [Command("Set Float Value", "Sets the value of a float field of a given name in a given type")]
        public static void SetFloatValue(
          [CommandParameter(Help = "The type of script on which to change a value",AutoCompleteMethodName = "TypeAuto",ForceAutoCompleteUsage = true,PreventDefaultValueUsage = true)]
            Type scriptType,
          [CommandParameter(Help = "The name of the field the change the value of"
                ,AutoCompleteMethodName = "FloatFieldAuto",ForceAutoCompleteUsage = true,PreventDefaultValueUsage = true)]
            string fieldName, float value)
        {
            List<UnityEngine.Object> objectsToChange = new List<UnityEngine.Object>();

            foreach (var o in Selection.objects)
            {
                if (o.GetType() == scriptType)
                {
                    objectsToChange.Add(o);
                }
                else if (o is GameObject go)
                {
                    objectsToChange.AddRange(go.GetComponentsInChildren(scriptType));
                }
            }

            PropertyInfo propInfo = null;
            var info = scriptType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Default);
            if (info == null)
                propInfo = scriptType.GetProperty(fieldName,
                     BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                     BindingFlags.FlattenHierarchy | BindingFlags.Default);

            if (info == null && propInfo == null)
            {
                Debug.LogWarning("MonKey - Error when attempting to change the value of a field, the value wasn't changed");
                return;
            }

            int undoID = MonkeyEditorUtils.CreateUndoGroup("MonKey - Set Float Value");

            foreach (var o in objectsToChange)
            {
                Undo.RecordObject(o, "modify Value");
                info?.SetValue(o, value);


                propInfo?.SetValue(o, value, null);
                EditorUtility.SetDirty(o);
            }

            Undo.CollapseUndoOperations(undoID);
        }

        public static StaticCommandParameterAutoComplete FloatFieldAuto()
        {
            return AutoCompleteManager.FieldAutoComplete(0, typeof(float));
        }

        public static StaticCommandParameterAutoComplete VectorFieldAuto()
        {
            return AutoCompleteManager.FieldAutoComplete(0, typeof(Vector3));
        }

        [Command("Set Random Float Value", "Sets the value of a float field of a given name in a given type to a random value within a range")]
        public static void SetRandomFloatValue(
      [CommandParameter(Help = "The type of script on which to change a value",AutoCompleteMethodName = "TypeAuto",ForceAutoCompleteUsage = true,PreventDefaultValueUsage = true)]
            Type scriptType,
      [CommandParameter(Help = "The name of the field the change the value of"
                ,AutoCompleteMethodName = "FloatFieldAuto",ForceAutoCompleteUsage = true,PreventDefaultValueUsage = true)]
            string fieldName)
        {
            List<UnityEngine.Object> objectsToChange = new List<UnityEngine.Object>();

            foreach (var o in Selection.objects)
            {
                if (o.GetType() == scriptType)
                {
                    objectsToChange.Add(o);
                }
                else if (o is GameObject go)
                {
                    objectsToChange.AddRange(go.GetComponentsInChildren(scriptType));
                }
            }

            PropertyInfo propInfo = null;
            var info = scriptType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Default);
            if (info == null)
                propInfo = scriptType.GetProperty(fieldName,
                     BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                     BindingFlags.FlattenHierarchy | BindingFlags.Default);

            if (info == null && propInfo == null)
            {
                Debug.LogWarning("MonKey - Error when attempting to change the value of a field, the value wasn't changed");
                return;
            }

            MonkeyEditorUtils.AddSceneCommand(new InteractiveRandomFloatValue(info, propInfo, objectsToChange.ToArray()));
        }

        [Command("Set Random Vector Value", "Sets the value of a Vector field of a given name in a given type to a random value within a range")]
        public static void SetRandomVectorValue(
            [CommandParameter(Help = "The type of script on which to change a value",AutoCompleteMethodName = "TypeAuto",ForceAutoCompleteUsage = true,PreventDefaultValueUsage = true)]
            Type scriptType,
            [CommandParameter(Help = "The name of the field the change the value of"
                ,AutoCompleteMethodName = "VectorFieldAuto",ForceAutoCompleteUsage = true,PreventDefaultValueUsage = true)]
            string fieldName)
        {
            List<UnityEngine.Object> objectsToChange = new List<UnityEngine.Object>();

            foreach (var o in Selection.objects)
            {
                if (o.GetType() == scriptType)
                {
                    objectsToChange.Add(o);
                }
                else if (o is GameObject go)
                {
                    objectsToChange.AddRange(go.GetComponentsInChildren(scriptType));
                }
            }

            PropertyInfo propInfo = null;
            var info = scriptType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Default);
            if (info == null)
                propInfo = scriptType.GetProperty(fieldName,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                    BindingFlags.FlattenHierarchy | BindingFlags.Default);

            if (info == null && propInfo == null)
            {
                Debug.LogWarning("MonKey - Error when attempting to change the value of a field, the value wasn't changed");
                return;
            }

            MonkeyEditorUtils.AddSceneCommand(new InteractiveRandomVectorValue(info, propInfo, objectsToChange.ToArray()));
        }


        public class InteractiveRandomFloatValue : InteractiveCommand
        {

            private float minValue;
            private float maxValue;
            public float Multiplier = 1;

            private List<Object> toRandomize;
            private FieldInfo info;
            private PropertyInfo propInfo;

            public InteractiveRandomFloatValue(FieldInfo info, PropertyInfo propInfo,
                params Object[] toRandomize)
            {
                ConfirmationMode = ActionConfirmationMode.ESCAPE;
                SceneCommandName = "Float Randomization";
                ActionOnSpace = "to randomize Values";
                minValue = 0;
                maxValue = 1;
                this.info = info;
                this.propInfo = propInfo;
                this.toRandomize = toRandomize.ToList();
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();

                DisplayFloatOption("Min Scale", ref minValue);
                DisplayFloatOption("Max Scale", ref maxValue);

                DisplayFloatOption("Multiplier", ref Multiplier);
                DisplayObjectListOption("Objects To Randomize", toRandomize);
            }

            public override void ApplyFunction()
            {
                OnSpaceDownAction();
            }

            protected override void OnSpaceDownAction()
            {
                base.OnSpaceDownAction();


                toRandomize.RemoveAll(_ => !_);

                if (info == null && propInfo == null)
                {
                    Debug.LogWarning("MonKey - Error when attempting to change the value of a field, the value wasn't changed");
                    return;
                }

                int undoID = MonkeyEditorUtils.CreateUndoGroup("MonKey - Set Float Value");

                foreach (var o in toRandomize)
                {
                    Undo.RecordObject(o, "modify Value");
                    info?.SetValue(o, Random.Range(minValue, maxValue) * Multiplier);


                    propInfo?.SetValue(o, Random.Range(minValue, maxValue) * Multiplier, null);
                    EditorUtility.SetDirty(o);
                }

                Undo.CollapseUndoOperations(undoID);
            }
        }


        public class InteractiveRandomVectorValue : InteractiveCommand
        {

            private Vector3 minValue;
            private Vector3 maxValue;
            public float Multiplier = 1;

            private List<Object> toRandomize;
            private FieldInfo info;
            private PropertyInfo propInfo;

            public InteractiveRandomVectorValue(FieldInfo info, PropertyInfo propInfo,
                params Object[] toRandomize)
            {
                ConfirmationMode = ActionConfirmationMode.ESCAPE;
                SceneCommandName = "Value Randomization";
                ActionOnSpace = "to randomize Values";
                minValue = Vector3.zero;
                maxValue = Vector3.one;
                this.info = info;
                this.propInfo = propInfo;
                this.toRandomize = toRandomize.ToList();
            }

            public override void DisplayParameters()
            {
                base.DisplayParameters();

                DisplayVectorOption("Min Scale", ref minValue);
                DisplayVectorOption("Max Scale", ref maxValue);

                DisplayFloatOption("Multiplier", ref Multiplier);
                DisplayObjectListOption("Objects To Randomize", toRandomize);
            }

            public override void ApplyFunction()
            {
                OnSpaceDownAction();
            }

            protected override void OnSpaceDownAction()
            {
                base.OnSpaceDownAction();


                toRandomize.RemoveAll(_ => !_);

                if (info == null && propInfo == null)
                {
                    Debug.LogWarning("MonKey - Error when attempting to change the value of a field, the value wasn't changed");
                    return;
                }

                int undoID = MonkeyEditorUtils.CreateUndoGroup("MonKey - Set Float Value");

                foreach (var o in toRandomize)
                {
                    Undo.RecordObject(o, "modify Value");
                    info?.SetValue(o, Vector3.Lerp(minValue, maxValue,Random.value) * Multiplier);


                    propInfo?.SetValue(o, Vector3.Lerp(minValue, maxValue, Random.value) * Multiplier, null);
                    EditorUtility.SetDirty(o);
                }

                Undo.CollapseUndoOperations(undoID);
            }
        }


    }





}
