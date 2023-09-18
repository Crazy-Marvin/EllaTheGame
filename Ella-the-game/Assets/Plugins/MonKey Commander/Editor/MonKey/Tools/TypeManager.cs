using System;
using System.Collections.Generic;
using System.Reflection;


namespace MonKey
{
    public static class TypeManager
    {
        public static BindingFlags AllInstanceFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                              BindingFlags.FlattenHierarchy | BindingFlags.Default;

        public static Dictionary<string, Type> AllObjectsTypes
            = new Dictionary<string, Type>();
        public static Dictionary<string, Type> AllScriptableObjectTypes
            = new Dictionary<string, Type>();
        public static Dictionary<string, Type> AllMonoBehaviorObjectTypes
            = new Dictionary<string, Type>();
        public static Dictionary<string, Type> AllComponentObjectTypes
            = new Dictionary<string, Type>();
        public static Dictionary<string, Type> AllEditorTypes
            = new Dictionary<string, Type>();

        public static void Clear()
        {
            AllObjectsTypes.Clear();
            AllScriptableObjectTypes.Clear();
            AllMonoBehaviorObjectTypes.Clear();
            AllComponentObjectTypes.Clear();
            AllEditorTypes.Clear();
        }

        public static Type GetType(string fullName)
        {
            if (AllObjectsTypes.ContainsKey(fullName))
                return AllObjectsTypes[fullName];
            if (AllScriptableObjectTypes.ContainsKey(fullName))
                return AllScriptableObjectTypes[fullName];
            if (AllMonoBehaviorObjectTypes.ContainsKey(fullName))
                return AllMonoBehaviorObjectTypes[fullName];
            if (AllComponentObjectTypes.ContainsKey(fullName))
                return AllComponentObjectTypes[fullName];
            if (AllEditorTypes.ContainsKey(fullName))
                return AllEditorTypes[fullName];
            return null;
        }
    }
}
