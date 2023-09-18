using JetBrains.Annotations;
using MonKey.Extensions;
using MonKey.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MonKey.Editor.Internal
{
    public class CommandInfo
    {
        public bool AlwaysShow;
        public string CommandHelp;
        public string CommandName;
        public string CommandQuickName;

        public int CommandOrder;

        public bool IgnoreHotKeyConflict;
        public KeyCombination HotKey;
        internal HotKeyInfo HotKeyInfo;

        public MethodInfo Action;

        public MethodInfo ValidationMethod;
        public string CommandValidationMessage;

        internal List<CommandParameterInfo> CommandParameterInfo;

        public bool IsMenuItem;

        public int ParameterCount { get { return CommandParameterInfo.Count; } }

        public string Category;

        public bool HasValidation
        {
            get
            {
                return ValidationMethod != null;
            }
        }

        public bool IsValid
        {
            get
            {
                return !HasValidation ||
                       ValidationMethod.GetParameters().Length == 0 &&
                       (bool)ValidationMethod.Invoke(null, null);
            }
        }

        public bool HasQuickName
        {
            get { return !CommandQuickName.IsNullOrEmpty(); }
        }

        public bool HasHotKeys
        {
            get { return HotKey != null && HotKey.KeysInOrder.Any(); }
        }

        public bool IsParametric { get { return Action.GetParameters().Length > 0; } }

        public bool CanUseQuickDefaultCall
        {
            get { return CommandParameterInfo.All(_ => _.HadDefaultValue); }
        }

        public bool IsConflictual
        {
            get
            {

                if (IgnoreHotKeyConflict || !HasHotKeys || HotKeyInfo == null || IsMenuItem)
                    return false;

                return HotKeyInfo.IsConflictual ||
                    (HotKeysManager.OverlappingHotKeys.ContainsKey(HotKeyInfo)
                    && HotKeysManager.OverlappingHotKeys[HotKeyInfo].Count(_ => _.IsSomeCommandsValid) > 1);
            }
        }

        public List<CommandInfo> ConflictualCommands
        {
            get
            {
                if (!IsConflictual)
                    return null;
                CommandInfo info = this;
                List<CommandInfo> conflicts = HotKeyInfo.AssociatedCommands
                    .Where(_ => _.CommandName != info.CommandName).ToList();
                if (HotKeysManager.OverlappingHotKeys.ContainsKey(HotKeyInfo))
                {
                    foreach (var hkInfo in HotKeysManager.OverlappingHotKeys[HotKeyInfo])
                    {
                        conflicts.AddRange(hkInfo.AssociatedCommands
                            .Where(_ => _.CommandName != info.CommandName));
                    }
                }
                return conflicts;
            }
        }

        public void ExecuteCommand(params object[] parameters)
        {
            Action.Invoke(null, parameters);

            if (!CommandManager.Instance.CategoriesByName.ContainsKey("Command History"))
                return;
            var cat = CommandManager.Instance.CategoriesByName["Command History"];
            cat.CommandNames.Insert(0, CommandName);
            if (cat.CommandNames.Count > 10)
                cat.CommandNames.RemoveAt(cat.CommandNames.Count - 1);

        }
    }
}