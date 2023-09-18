using System;
using MonKey.Extensions;

namespace MonKey
{
    /// <summary>
    /// Put the attribute on the parameter 
    /// it represents to customize the way the parameter 
    /// will be handled by MonKey. You can put the attribute 
    /// on the method as well, but then you need to specify 
    /// the index of the parameter in the Order field (starting at 0)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class CommandParameter : Attribute
    {
        /// <summary>
        /// The order of the parameter in the method (for identification).
        /// You do not need to specify the value if the attribute is on the parameter directly
        /// </summary>
        public int Order;

        /// <summary>
        /// Some Help that can help the user identify the use if this parameter
        /// </summary>
        public string Help;
        /// <summary>
        /// If you want the name of the parameter to appear differently than the one specified in c#
        /// </summary>
        public string OverrideName;

        /// <summary>
        /// If you want the type of the parameter to be formatted in a specific way, 
        /// you can change it here
        /// </summary>
        public string OverrideTypeName;

        /// <summary>
        /// The name of the method that will suggest the user values to be used 
        /// </summary>
        public string AutoCompleteMethodName;

        /// <summary>
        /// The name of the method that will provide 
        /// the default value to be used in case the user does not use any input
        /// </summary>
        public string DefaultValueMethod;

        /// <summary>
        /// If you want the default value to appear with a certain name 
        /// inside Monkey Commander, define it here.
        /// </summary>
        public string DefaultValueNameOverride;

        /// <summary>
        /// if true, forces the user to enter a value for the parameter,
        ///  ignoring potential default values
        /// </summary>
        public bool PreventDefaultValueUsage;

        /// <summary>
        /// When this is true, the value of the variable will 
        /// automatically be taken from auto-complete suggestions (the first one being chosen if none was selected)
        /// </summary>
        public bool ForceAutoCompleteUsage;

        public bool HasAutoCompleteMethod => !AutoCompleteMethodName.IsNullOrEmpty();

        public bool HasDefaultValueMethod => !DefaultValueMethod.IsNullOrEmpty();

        public CommandParameter(string help)
        {
            Help = help;
        }

        public CommandParameter()
        {
        }
    }
}

