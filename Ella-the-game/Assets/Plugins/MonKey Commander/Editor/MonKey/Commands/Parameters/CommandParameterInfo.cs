
using MonKey.Extensions;
using System;
using System.Reflection;
using System.Text;
using Object = UnityEngine.Object;

namespace MonKey.Editor.Internal
{
    internal class CommandParameterInfo
    {

        public ParameterInfo ParamInfo;

        public string ParameterTypeNameOverride;

        public bool HasTypeNameOverride => !ParameterTypeNameOverride.IsNullOrEmpty();

        public Type ParameterType => ParamInfo.ParameterType;
        public bool IsArray => ParamInfo.ParameterType.IsArray;

        public MethodInfo AutoCompleteMethod;

        public MethodInfo DefaultValueMethod;

        public string DefaultValueNameOverride;

        public bool ForceAutoCompleteUsage;

        public bool HasDefaultValueNameOverride => !DefaultValueNameOverride.IsNullOrEmpty();

        public int Order;
        public string Help;

        public string OverrideName;

        public bool PreventDefaultValueUsage;

        public string Name => OverrideName.IsNullOrEmpty() ? ParamInfo.Name : OverrideName;

        public GenericCommandParameterAutoComplete AutoComplete
        {
            get
            {
                if (AutoCompleteMethod == null)
                {
                    return AutoCompleteManager.GetDefaultAutoComplete(ParameterType);
                }
                return (GenericCommandParameterAutoComplete)AutoCompleteMethod.Invoke(null, null);
            }
        }

        public object DefaultValue
        {
            get
            {
                if (HasDefaultValueMethod)
                {
                    return DefaultValueMethod.Invoke(null, null);
                }
                return ParamInfo.DefaultValue;
            }
        }

        public string DefaultValueName
        {
            get
            {
                if (HasDefaultValueNameOverride)
                    return DefaultValueNameOverride;
                if (ParameterType.IsArray)
                {
                    if (DefaultValue is Array arr)
                    {
                        StringBuilder convert = new StringBuilder();
                        int i = 0;
                        convert.Append(MonkeyStyle.ArrayVisualPrefix);
                        foreach (object o in arr)
                        {
                            Object arraUnity = o as Object;
                            if (arraUnity)
                            {
                                convert.Append(arraUnity.name);
                            }
                            else
                            {
                                convert.Append(o);
                            }

                            i++;
                            if (i != arr.Length)
                                convert.Append(CommandParameterInterpreter.ArrayVariableSeparator);
                        }
                        convert.Append(MonkeyStyle.ArrayVisualSuffix);
                        return convert.ToString();
                    }
                }
                Object unityO = DefaultValue as Object;
                if (unityO)
                {
                    return unityO.name;
                }

                return DefaultValue.ToString();
            }
        }

        public bool HasAutoComplete => AutoCompleteMethod != null;

        public bool HasDefaultValueMethod => DefaultValueMethod != null;

        public bool HadDefaultValue => DefaultValue != null && !PreventDefaultValueUsage;

        public CommandParameterInfo(CommandParameter parameter, ParameterInfo info,
            MethodInfo autoCompleteMethod, MethodInfo defaultValueMethod)
        {
            ParamInfo = info;

            OverrideName = parameter.OverrideName;
            AutoCompleteMethod = autoCompleteMethod;

            DefaultValueMethod = defaultValueMethod;
            ForceAutoCompleteUsage = parameter.ForceAutoCompleteUsage;
            PreventDefaultValueUsage = parameter.PreventDefaultValueUsage;

            Order = parameter.Order;
            Help = parameter.Help;

            DefaultValueNameOverride = parameter.DefaultValueNameOverride;
            ParameterTypeNameOverride = parameter.OverrideTypeName;
        }

        public CommandParameterInfo(ParameterInfo info, int order)
        {
            ParamInfo = info;
            AutoCompleteMethod = null;
            DefaultValueMethod = null;
            Order = order;
        }
    }
}
