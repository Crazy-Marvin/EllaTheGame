
using MonKey.Extensions;
using MonKey.Internal;
using MonKey.Settings.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEditor;

using UnityEngine;
using Component = UnityEngine.Component;
using ThreadPriority = System.Threading.ThreadPriority;


namespace MonKey.Editor.Internal
{
    [InitializeOnLoad]
    public class CommandManager : EditorSingleton<CommandManager>, IMonKeySingleton
    {
        static CommandManager()
        {
            SessionID = "MC_CommandManager";
        }

        public static bool ReadyForPlayMode;

        public static string CommandHistory = "Command History";


        #region SINGLETON


        private static CommandManager instance;

        #endregion

        #region REGISTERING

        public bool IsLoading { get; private set; }

        private readonly List<CommandInfo> infoToAdd = new List<CommandInfo>();

        public string CurrentAssemblyLoading;
        public string CurrentClassLoading;
        public int AssemblyAnalyzed;
        public int TotalAssemblies;
        private readonly int amountSleepThreshold = 10;

        public bool onlyScanSpecified;
        private string[] assembliesToExclude;

        private string[] nameSpacesToExclude;

        public event Action OnCommandLoadingDone;
        #endregion

        public const int MaxCommandShown = 15;
        private const int CommandFoundCountForAliases = 15;

        private readonly CommandInfo defaultInfo = new CommandInfo()
        {
            Action = null,
            CommandHelp = "No Command Found",
            CommandName = "No Command Found",
            CommandOrder = 0
        };

        internal readonly Dictionary<string, CommandCategory> CategoriesByName
            = new Dictionary<string, CommandCategory>();

        internal readonly List<string> BaseCategories = new List<string>();

        internal readonly Dictionary<string, CommandInfo> CommandsByName
            = new Dictionary<string, CommandInfo>();

        public Dictionary<string, string> WordAliases
            = new Dictionary<string, string>();

        public int CommandCount => CommandsByName.Count;

        public void PostInstanceCreation()
        {
            MonKeyInternalSettings.FindInstance();

            MonkeyStyle.FindInstance();

            MonkeyLocalizationFile.FindInstance();

            Instance.OnEnable();
            /*f (CommandConsoleWindow.CurrentPanel)
                 CommandConsoleWindow.CurrentPanel.CloseOrSetInactive();*/
        }

        public void OnEnable()
        {
            InitAliases();
            AutoCompleteManager.InitializeManager();

            MonkeyEditorUtils.EditorUpdatePlug(this);
            MonKeySelectionUtils.PluginImporter();
            RetrieveAllCommands();
        }


        private void InitAliases()
        {
            WordAliases = new Dictionary<string, string>
            {
                {"Select", "Find"},
                {"Find", "Select"},
                {"Create", "New"},
                {"New", "Create"},
                {"Instantiate", "New"},
                {"Prefab", "Instance"},
                {"Place", "Move"},
                {"Raycast", "Collision"},
                {"Search", "Find"},
                {"Copy", "Duplicate"},
                {"Duplicate", "Copy"},
                {"Clone", "Duplicate"},
                {"UnSelect", "Clear"},
                {"Deselect", "Clear"}
            };
        }

        public static void RetrieveAll()
        {
            if (!instance)
                FindInstance();

            if (!EditorApplication.isPlayingOrWillChangePlaymode && instance)
            {
                if (DebugLog)
                    Debug.Log("Retrieving on did reload");
                instance.OnEnable();
            }
        }


        private Thread commandsRetrieving;

        public void RetrieveAllCommands(bool force = false)
        {
            /*  if (!instance)
                  FindInstance();*/

            try
            {
                if (DebugLog)
                    Debug.Log("Retrieving");

                if (!force)
                {
                    if (CommandCount > 0)
                    {
                        if (DebugLog)
                            Debug.Log("Canceled because some commands already found");
                        return;
                    }

                    if (commandsRetrieving != null)
                    {
                        if (commandsRetrieving.IsAlive)
                        {
                            if (DebugLog)
                                Debug.Log("Canceled because thread is alive");
                            return;

                        }
                    }

                }

                if (IsLoading)
                {
                    commandsRetrieving?.Abort();
                }

                if (DebugLog)
                    Debug.Log("NOT CANCELED");

                EditorApplication.update -= AddAllFoundCommand;
                EditorApplication.update += AddAllFoundCommand;

                HotKeysManager.Reset();

                lock (infoToAdd)
                {
                    infoToAdd.Clear();
                }
                onlyScanSpecified = MonKeyInternalSettings.Instance.OnlyScanSpecified;
                assembliesToExclude = MonKeyInternalSettings.Instance.ExcludedAssemblies.Split(';');
                nameSpacesToExclude = MonKeyInternalSettings.Instance.ExcludedNameSpaces.Split(';');

                CommandsByName.Clear();
                TypeManager.Clear();
                IsLoading = true;

                commandsRetrieving?.Abort();

                commandsRetrieving = new Thread(CheckCommandsForAllAssemblies)
                {
                    IsBackground = true,
                    Priority = ThreadPriority.Normal,
                };

                commandsRetrieving.Start();
            }
            catch (Exception)
            {
                // Unity is not ready yet for instance creation
                return;
            }
        }

        #region REGISTERING

        private void AddAllFoundCommand()
        {
            if (!IsLoading)
            {
                lock (infoToAdd)
                {
                    if (infoToAdd.Count > 0)
                    {
                        foreach (var info in infoToAdd)
                        {

                            AddCommand(info.CommandName, info);
                        }
                    }

                    infoToAdd.Clear();
                    OnCommandLoadingDone?.Invoke();

                    BaseCategories.Sort();
                    CommandCategory history = new CommandCategory();
                    history.CategoryName = CommandHistory;

                    if(!BaseCategories.Contains(history.CategoryName))
                        BaseCategories.Insert(0, history.CategoryName);

                    if (!CategoriesByName.ContainsKey(history.CategoryName))
                        CategoriesByName.Add(history.CategoryName, history);

                    EditorApplication.update -= AddAllFoundCommand;
                }
            }
            else
            {
                lock (infoToAdd)
                {
                    foreach (var info in infoToAdd)
                    {
                        AddCommand(info.CommandName, info);
                    }
                    infoToAdd.Clear();
                }
            }


        }

        private void AddCommand(string name, CommandInfo action)
        {
            if (!CommandsByName.ContainsKey(name))
            {
                if (DebugLog)
                {
                    Debug.Log("Adding new command: " + name);
                }

                CommandsByName.Add(name, action);

                if (action.HasQuickName)
                {
                    if (!CommandsByName.ContainsKey(action.CommandQuickName))
                    {
                        CommandsByName.Add(action.CommandQuickName, action);
                    }
                    else if (CommandsByName[action.CommandQuickName].CommandOrder > action.CommandOrder)
                    {
                        CommandsByName[action.CommandQuickName] = action;
                    }
                }

                if (action.HasHotKeys)
                {
                    HotKeysManager.RegisterCommandHotKey(action);
                }
            }
        }

        private void CheckCommandsForAllAssemblies()
        {

            try
            {
                IEnumerable<Assembly> selectedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(_ =>
                    {
                        int i = 0;
                        foreach (var assemblyPrefix in assembliesToExclude)
                        {
                            if (assemblyPrefix.IsNullOrEmpty())
                            {
                                continue;
                            }

                            if (_.GetName().Name.Contains(assemblyPrefix))
                            {
                                if (onlyScanSpecified)
                                {

                                    return true;
                                }
                                else
                                {
                                    if (DebugLog)
                                    {
                                        Debug.Log("Assembly Excluded: " + _.FullName);
                                    }

                                    return false;
                                }

                            }

                            if (i > amountSleepThreshold)
                            {
                                Thread.Sleep(1);
                                i = 0;
                            }
                            i++;
                        }
                        if (DebugLog)
                        {
                            Debug.Log("Assembly Added: " + _.FullName);
                        }

                        if (onlyScanSpecified)
                        {

                            return false;
                        }
                        else
                            return true;

                    }).OrderByDescending(_ => _ == Assembly.GetExecutingAssembly());

                var assemblies = selectedAssemblies.ToArray();

                assemblies = assemblies.OrderByDescending(_ => _.FullName == "Assembly-CSharp").ToArray();
                TotalAssemblies = assemblies.Length;
                AssemblyAnalyzed = 0;

                CheckCommandsForAssemblies(assemblies);
            }
            catch (Exception e)
            {
                if (DebugLog)
                {
                    Debug.LogError("An Exception (" + e + ") \n \n was raised when reading commands ");
                }
                RetrieveAllCommands(true);
            }
        }

        private void CheckCommandsForAssemblies(IEnumerable<Assembly> selectedAssemblies)
        {
            foreach (var assembly in selectedAssemblies)
            {

                CurrentAssemblyLoading = assembly.GetName().Name;

                if (DebugLog)
                {
                    Debug.Log("Checking Assembly ".Bold() + CurrentAssemblyLoading);
                }

                try
                {
                    CheckCommandsFromAssembly(assembly);
                }
                catch (Exception e)
                {
                    if (DebugLog)
                    {
                        Debug.LogError("An Exception (" + e + ") \n \n was raised when reading commands " +
                                       "from assembly '" + assembly + "'");
                    }
                    RetrieveAllCommands(true);
                }

                lock (this)
                {
                    AssemblyAnalyzed++;
                }
            }

            if (DebugLog)
            {
                Debug.Log("Done!");
            }

            IsLoading = false;

        }

        private void CheckCommandsFromAssembly(Assembly assembly)
        {
            int i = 0;
            IEnumerable<Type> types = assembly.GetTypes().Where(_ =>
               {
                   foreach (var nameSpace in nameSpacesToExclude)
                   {
                       if (nameSpace.IsNullOrEmpty())
                       {
                           continue;
                       }

                       if (_.Namespace.Contains(nameSpace))
                       {
                           return false;
                       }
                   }
                   return true;
               });

            foreach (var type in types)
            {
                CurrentClassLoading = type.Name;

                if (DebugLog)
                {
                    Debug.Log("Checking Type :" + CurrentClassLoading);
                }

                try
                {
                    RegisterUnityType(type);
                    RegisterCommandsFromType(type);
                }
                catch (Exception e)
                {
                    if (DebugLog)
                    {
                        Debug.LogError("An Exception \n(" + e + ") \n \n was raised when reading commands " +
                                       "from type '" + type + "'");
                    }
                    RetrieveAllCommands(true);
                }

                i++;
                if (i > amountSleepThreshold)
                {
                    i = 0;
                    Thread.Sleep(1);
                }
            }

        }

        private static void RegisterUnityType(Type type)
        {
            lock (TypeManager.AllEditorTypes)
            {
                if (type.FullName == null)
                {
                    return;
                }

                if (type.FullName.Contains("UnityEditor")
                    || type.IsSubclassOf(typeof(UnityEditor.Editor))
                    || type.IsSubclassOf(typeof(EditorWindow)))
                {
                    if (!TypeManager.AllEditorTypes.ContainsKey(type.FullName))
                    {
                        TypeManager.AllEditorTypes.Add(type.FullName, type);
                    }

                    return;
                }
            }

            lock (TypeManager.AllMonoBehaviorObjectTypes)
            {
                if (type.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    if (type.FullName != null
                        && !TypeManager.AllMonoBehaviorObjectTypes.ContainsKey(type.FullName))
                    {
                        TypeManager.AllMonoBehaviorObjectTypes.Add(type.FullName, type);
                    }

                    return;
                }
            }

            lock (TypeManager.AllScriptableObjectTypes)
            {

                if (type.IsSubclassOf(typeof(ScriptableObject)))
                {
                    if (type.FullName != null &&
                        !TypeManager.AllScriptableObjectTypes.ContainsKey(type.FullName))
                    {
                        TypeManager.AllScriptableObjectTypes.Add(type.FullName, type);
                    }

                    return;

                }
            }

            lock (TypeManager.AllComponentObjectTypes)
            {
                if (type.IsSubclassOf(typeof(Component)))
                {
                    if (type.FullName != null &&
                        !TypeManager.AllComponentObjectTypes.ContainsKey(type.FullName))
                    {
                        TypeManager.AllComponentObjectTypes.Add(type.FullName, type);
                    }

                    return;

                }
            }
            lock (TypeManager.AllObjectsTypes)
            {
                if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    if (type.FullName != null && !TypeManager.AllObjectsTypes.ContainsKey(type.FullName))
                    {
                        TypeManager.AllObjectsTypes.Add(type.FullName, type);
                    }
                }
            }
        }

        public void RegisterCommandsFromType(Type type)
        {
            foreach (var methodInfo in
                type.GetMethods(BindingFlags.Public | BindingFlags.Default
                | BindingFlags.NonPublic | BindingFlags.Static))
            {
                try
                {
                    var quickCommandAttributes = methodInfo.GetCustomAttributes(typeof(Command), true);

                    if (quickCommandAttributes.Length > 0)
                    {
                        foreach (var commandAttribute in
                            (methodInfo.GetCustomAttributes(typeof(Command), true)))
                        {
                            if (DebugLog)
                            {
                                Debug.Log("Console Command Found! :".Bold().Colored(Color.green)
                                    + methodInfo.Name);
                            }

                            CheckCommandStatuses(type, commandAttribute, methodInfo, false);
                        }
                    }
                    else
                    {
                        foreach (var commandAttribute in
                            methodInfo.GetCustomAttributes(typeof(MenuItem), true))
                        {
                            if (DebugLog)
                            {
                                Debug.Log("Menu Item Found! :".Bold().Colored(Color.green) + methodInfo.Name);
                            }

                            MenuItem item = (MenuItem)commandAttribute;

                            if (item.validate)
                            {
                                continue;
                            }

                            if (methodInfo.GetCustomAttributes(typeof(MenuItemCommandLink), true).Length > 0)
                            {
                                continue;
                            }

                            CheckCommandStatuses(type, commandAttribute, methodInfo, true);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (DebugLog)
                    {
                        Debug.LogWarningFormat("An Exception was raised when " +
                                               "trying to retrieve a command " +
                                               "from {0} : {1}".Bold().Colored(Color.red),
                                               methodInfo.Name, e);
                    }
                    RetrieveAllCommands(true);

                }
            }
        }

        private void CheckCommandStatuses(Type actionProvider, object commandAttribute,
            MethodInfo methodInfo, bool menuItem)
        {

            Command command = commandAttribute as Command;

            var validationName = CheckValidationMethod(actionProvider
                , methodInfo, command, out var validationDelegate);

            var hotKey = FindHotKey(methodInfo, command);

            var commandParamsInfos = FindParameters(methodInfo);

            if (command == null && commandParamsInfos != null && commandParamsInfos.Count > 0)
            {
                return;
            }

            AddCommandInfo(methodInfo, validationDelegate, command,
                validationName, hotKey, commandParamsInfos);
        }

        private void AddCommandInfo(MethodInfo methodInfo, MethodInfo validationDelegate,
            Command command, string validationMessage, KeyCombination hotKey,
            List<CommandParameterInfo> paramInfos)
        {

            lock (infoToAdd)
            {
                if (DebugLog)
                {
                    Debug.Log("Adding Command HotKeyInfo");
                }

                string menuItemName = methodInfo.Name.NicifyVariableName();
                if (command == null)
                {
                    object[] items = methodInfo.GetCustomAttributes(typeof(MenuItem), false);
                    if (items.Length > 0)
                    {
                        MenuItem item = (MenuItem)items[0];
                        int startOfName = item.menuItem.LastIndexOf("/", StringComparison.Ordinal);
                        int endOfName = hotKey != null ? item.menuItem.LastIndexOf(" ", StringComparison.Ordinal) - 1 : item.menuItem.Length - 1;
                        menuItemName = item.menuItem.Substring(startOfName + 1, endOfName - startOfName);
                    }
                }

                CommandInfo info = new CommandInfo()
                {
                    Action = methodInfo,
                    ValidationMethod = validationDelegate,
                    AlwaysShow = command != null && command.AlwaysShow,
                    CommandHelp = command?.Help,
                    CommandName = (command != null) ? command.Name : menuItemName,
                    CommandQuickName = (command != null) ? command.QuickName : null,
                    CommandOrder = (command != null) ? command.Order : Int32.MaxValue,
                    CommandValidationMessage = validationMessage,
                    HotKey = hotKey,
                    IgnoreHotKeyConflict = command != null && command.IgnoreHotKeyConflict,
                    CommandParameterInfo = paramInfos,
                    IsMenuItem = command == null,
                    Category = command == null ? "Unity Menus" : command.Category
                };

                infoToAdd.Add(info);

                AddCategories(info);
            }
        }

        private void AddCategories(CommandInfo info)
        {
            string[] categories = info.Category.Split('/');
            string path = "";
            for (int i = 0; i < categories.Length; i++)
            {
                path += categories[i];
                if (!CategoriesByName.ContainsKey(path))
                {
                    CommandCategory cat = new CommandCategory();
                    cat.CategoryName = categories[i];
                    if (i > 0)
                    {
                        cat.ParentCategoryName = path.Replace("/" + categories[i], "");
                    }
                    else
                    {
                        BaseCategories.Add(path);
                    }

                    CategoriesByName.Add(path, cat);
                }
                else if (categories.Length > 1 && i < categories.Length - 1)
                {
                    CommandCategory main = CategoriesByName[path];
                    main.AddSubCategory(path + "/" + categories[i + 1]);
                }

                path += "/";
            }

            CategoriesByName[info.Category].AddCommandName(info.CommandName);
        }

        private KeyCombination FindHotKey(MethodInfo methodInfo, Command command)
        {

            if (DebugLog)
            {
                Debug.Log("Finding Hot Keys");
            }

            var menuItemAttributes = methodInfo.GetCustomAttributes(typeof(MenuItem), true);

            if (command != null && !command.MenuItemLink.IsNullOrEmpty())
            {
                MethodInfo overrideInfo = null;
                if (command.MenuItemLinkTypeOwner == null)
                {
                    if (methodInfo.DeclaringType != null)
                    {
                        overrideInfo =
                        methodInfo.DeclaringType.GetMethod(
                            command.MenuItemLink,
                            BindingFlags.Static | BindingFlags.Default
                            | BindingFlags.NonPublic | BindingFlags.Public);
                    }
                }
                else
                {
                    Type linkOwner = null;
                    int thresh = 0;
                    foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (Type t in a.GetTypes())
                        {
                            if (t.Name == command.MenuItemLinkTypeOwner)
                            {
                                linkOwner = t;
                            }
                        }

                        if (thresh > amountSleepThreshold)
                        {
                            thresh = 0;
                            Thread.Sleep(1);
                        }
                        thresh++;
                    }

                    if (linkOwner != null)
                    {
                        overrideInfo = linkOwner.GetMethod(
                            command.MenuItemLink,
                            BindingFlags.Static | BindingFlags.Default
                            | BindingFlags.NonPublic | BindingFlags.Public);
                    }

                }

                if (overrideInfo != null)
                {
                    menuItemAttributes =
                        overrideInfo.GetCustomAttributes(typeof(MenuItem), true);
                }
            }

            if (menuItemAttributes.Length > 0)
            {
                if (DebugLog)
                {
                    Debug.Log("Finding Menu Item Hot Keys");
                }

                var item = menuItemAttributes[0] as MenuItem;

                if (item != null)
                {

                    int lastIndex = item.menuItem.LastIndexOf(' ');

                    if (lastIndex == item.menuItem.Length - 1)
                    {
                        lastIndex = item.menuItem.Substring(0, item.menuItem.Length - 1)
                            .LastIndexOf(' ');
                    }

                    if (lastIndex == -1)
                    {
                        return null;
                    }

                    var hotKey = item.menuItem
                        .Substring(item.menuItem.LastIndexOf(' '));

                    hotKey = hotKey.Replace(" ", "");

                    bool startsWithSpecialCharacter = hotKey.
                        StartsWith(KeyCombination.KeySymbol.ToString());

                    if (!startsWithSpecialCharacter)
                    {
                        foreach (var modifierKeysAlias in KeyCombination.ModifierKeysAliases.Keys)
                        {
                            if (hotKey.StartsWith(modifierKeysAlias))
                            {
                                startsWithSpecialCharacter = true;
                                break;
                            }
                        }
                    }

                    if (startsWithSpecialCharacter)
                    {
                        if (DebugLog)
                        {
                            Debug.Log("Making Menu Hot Key Readable");
                        }

                        return new KeyCombination(hotKey);
                    }

                    return null;
                }
            }
            return null;
        }

        private static string CheckValidationMethod(Type actionProvider,
            MethodInfo methodInfo, Command command, out MethodInfo validationDelegate)
        {
            if (DebugLog)
            {
                Debug.Log("Finding Validation Method");
            }

            string validationHelp = "";
            MethodInfo validation = null;

            if (command != null && (!command.ValidationMethodName.IsNullOrEmpty()
                                    || command.DefaultValidation != DefaultValidation.NONE))
            {
                if (command.ValidationMethodName.IsNullOrEmpty())
                {
                    validation =
                        ValidationUtilities.GetValidationMethod(command.DefaultValidation);
                }
                else
                {
                    string validationMethodName = command.ValidationMethodName;
                    validation =
                        actionProvider.GetMethod(validationMethodName,
                            BindingFlags.Public | BindingFlags.Default
                            | BindingFlags.NonPublic | BindingFlags.Static);

                    if (validation == null)
                    {
                        Debug.LogWarningFormat("The command '{0}' was associated with " +
                                               "a validation method named '{1}', " +
                                               "but no static method of such name could be found",
                            methodInfo.Name, command.ValidationMethodName);
                    }
                }
            }
            else
            {

                var menuItemAttributes = methodInfo.GetCustomAttributes(typeof(MenuItem), true);
                if (menuItemAttributes.Length > 0)
                {
                    MenuItem item = (MenuItem)menuItemAttributes[0];
                    string menuItemName = item.menuItem;
                    foreach (var validationMethod in
                        actionProvider.GetMethods(BindingFlags.Public | BindingFlags.Default
                        | BindingFlags.NonPublic | BindingFlags.Static))
                    {

                        var attrbs = validationMethod.GetCustomAttributes(typeof(MenuItem), true);
                        if (attrbs.Length > 0)
                        {
                            MenuItem potentialValidation = (MenuItem)attrbs[0];
                            if (potentialValidation.validate && potentialValidation.menuItem == menuItemName)
                            {
                                validation = validationMethod;
                                break;
                            }
                        }
                    }
                }

            }

            validationDelegate = validation;

            if (validation != null)
            {
                var validations =
                    validation.GetCustomAttributes(typeof(CommandValidation)
                        , true);
                if (validations.Length > 0)
                {
                    var quickCommandValidation = validations[0]
                        as CommandValidation;
                    if (quickCommandValidation != null)
                    {
                        validationHelp = quickCommandValidation
                            .InvalidCommandMessage;
                    }
                }
            }
            return validationHelp;
        }

        private static List<CommandParameterInfo> FindParameters(MethodInfo methodInfo)
        {

            if (DebugLog)
            {
                Debug.LogFormat("Checking parameters for method {0}", methodInfo.Name);
            }

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            CommandParameter[] parameterAttributes =
                (CommandParameter[])methodInfo.GetCustomAttributes(typeof(CommandParameter),
                false);

            List<CommandParameterInfo> commandInfos
                = new List<CommandParameterInfo>(parameterInfos.Length);
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                commandInfos.Add(null);
            }

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                CommandParameter[] parameterCommandAttributes =
                    (CommandParameter[])parameterInfos[i].
                        GetCustomAttributes(typeof(CommandParameter), false);
                if (parameterCommandAttributes.Length > 0)
                {
                    parameterCommandAttributes[0].Order = i;
                    AddParameterInfo(methodInfo, parameterCommandAttributes[0],
                        parameterInfos, commandInfos);
                }
            }

            foreach (var attribute in parameterAttributes)
            {
                AddParameterInfo(methodInfo, attribute, parameterInfos, commandInfos);
            }

            for (int i = 0; i < commandInfos.Count; i++)
            {
                if (commandInfos[i] == null)
                {
                    commandInfos[i] = new CommandParameterInfo(parameterInfos[i], i);
                }
            }
            return commandInfos;
        }

        private static void AddParameterInfo(MethodInfo methodInfo, CommandParameter attribute,
            ParameterInfo[] parameterInfos, List<CommandParameterInfo> commandInfos)
        {

            if (DebugLog)
            {
                Debug.LogFormat("Checking parameter info for method {0}", methodInfo.Name);
            }

            if (attribute == null || methodInfo == null)
            {
                Debug.LogWarning("MonKey | Error when parsing a command : no attribute or method info found");
                return;
            }

            if (parameterInfos == null || attribute.Order >= parameterInfos.Length)
            {
                Debug.LogWarningFormat("Monkey Commander Warning:" +
                                       " A command Parameter for the Command '{0}'" +
                                       " was associated with the order '{1}', " +
                                       "but the method does not have that many" +
                                       " parameters", methodInfo.Name, attribute.Order);
                return;
            }

            MethodInfo autoCompleteMethod = null;

            if (attribute.HasAutoCompleteMethod)
            {
                if (methodInfo.DeclaringType != null)
                {
                    autoCompleteMethod =
                        methodInfo.DeclaringType.GetMethod(attribute.AutoCompleteMethodName,
                            BindingFlags.Public | BindingFlags.Default
                            | BindingFlags.NonPublic | BindingFlags.Static);
                }

                if (autoCompleteMethod != null &&
                    (!autoCompleteMethod.ReturnType.IsSubclassOf(
                        typeof(GenericCommandParameterAutoComplete))
                        && autoCompleteMethod.ReturnType != typeof(GenericCommandParameterAutoComplete)
                        || autoCompleteMethod.GetParameters().Length != 0))
                {
                    autoCompleteMethod = null;
                }

                if (autoCompleteMethod == null)
                {
                    Debug.LogWarningFormat("Monkey Commander Warning:A parameter for the " +
                                           "command '{0}' was linked to an auto complete method named '{1}'," +
                                           " but no static method of such name could be found, " +
                                           "or the method does not return a CommandParameterAutoComplete",
                        methodInfo.Name, attribute.AutoCompleteMethodName);
                }
            }

            MethodInfo defaultValueMethod = null;

            if (attribute.HasDefaultValueMethod)
            {
                if (methodInfo.DeclaringType != null)
                {
                    defaultValueMethod = methodInfo.DeclaringType.GetMethod(attribute.DefaultValueMethod,
                        BindingFlags.Public | BindingFlags.Default
                        | BindingFlags.NonPublic | BindingFlags.Static);
                }

                if (defaultValueMethod != null)
                {
                    //default values for array members must be done differently
                    /*if (parameterInfos[attribute.Order].ParameterType.IsArray)
                    {
                        if(defaultValueMethod.ReturnType
                           != parameterInfos[attribute.Order].ParameterType.GetElementType())
                            defaultValueMethod = null;

                    }else*/
                    if (defaultValueMethod.ReturnType !=
                               parameterInfos[attribute.Order].ParameterType
                                     || defaultValueMethod.GetParameters().Length != 0)
                    {
                        defaultValueMethod = null;
                    }
                }

                if (defaultValueMethod == null)
                {
                    Debug.LogWarningFormat("Monkey Commander Warning:A parameter for the " +
                                           "command '{0}' was linked to a default value method named '{1}'," +
                                           " but no static method of such name could be found, " +
                                           "or the method does not return the same type than the specified parameter",
                        methodInfo.Name, attribute.DefaultValueMethod);
                }
            }

            CommandParameterInfo info =
                new CommandParameterInfo(
                    attribute, parameterInfos[attribute.Order], autoCompleteMethod, defaultValueMethod);

            commandInfos[attribute.Order] = info;
        }

        #endregion

        #region COMMAND SEARCH

        public CommandInfo GetCommand(string name)
        {
            return CommandsByName[name];
        }

        public IEnumerable<string> CommandNames
        {
            get { return CommandsByName.Keys; }
        }

        public CommandInfo GetCommandInfo(string name)
        {
            if (CommandsByName.ContainsKey(name))
            {
                return CommandsByName[name];
            }

            return null;
        }

        public bool IsCommandAuthorized(CommandInfo info)
        {
            if (!MonKeyInternalSettings.Instance.IncludeMenuItems && info.IsMenuItem)
            {
                return false;
            }

            if (info.IsMenuItem
                && MonKeyInternalSettings.Instance.IncludeOnlyMenuItemsWithHotKeys && !info.HasHotKeys)
            {
                return false;
            }

            return true;
        }

        public CommandInfo BestMatchingAction(params string[] searchTerms)
        {
            if (searchTerms.Length > 0 && searchTerms.Any(_ => !_.IsNullOrEmpty()))
            {
                return CommandsByName[StringExt.
                    OrderStringsBySearchScore(CommandsByName.Keys, false, searchTerms)
                    .First(_ => IsCommandAuthorized(CommandsByName[_]))];
            }

            return defaultInfo;
        }

        public IEnumerable<CommandInfo> ActionByMatch(params string[] searchTerms)
        {
            if (DebugLog)
            {
                Debug.Log("Searching for actions.. " + EditorApplication.timeSinceStartup);
            }

            if (searchTerms.Length > 0 && searchTerms.Any(_ => !_.IsNullOrEmpty()))
            {
                List<CommandInfo> foundCommands = new List<CommandInfo>(CommandsByName.Count);

                IEnumerable<string> firstSearch;

                //not dependent on the option right now (performance satisfying)
                /*    if (MonKeyInternalSettings.Instance.UseAdvancedFuzzySearch)
                    {*/
                firstSearch = CommandNames.Where(_ =>
                {
                    if (!_.ToLower().Contains(searchTerms[0]))
                    {
                        var x = StringExt.MatchResultSet(new List<string> { _.ToLower() }, searchTerms[0]);
                        return x.Count > 0;
                    }

                    return true;
                });

                firstSearch = CommandNames;

                firstSearch = StringExt.OrderStringsBySearchScore(firstSearch, true, searchTerms)
                    .ThenBy(_ => !CommandsByName[_].HasQuickName)
                    .ThenBy(_ => CommandsByName[_].CommandOrder);
                /*  }
                  else
                  {
                      firstSearch = StringExt.OrderStringsBySearchScore(CommandsByName.Keys, false, searchTerms)
                          .ThenBy(_ => !CommandsByName[_].HasQuickName)
                          .ThenBy(_ => CommandsByName[_].CommandOrder);
                  }*/


                foreach (var name in firstSearch)
                {

                    if (!foundCommands.Contains(CommandsByName[name]) &&
                        IsCommandAuthorized(CommandsByName[name]))
                    {
                        foundCommands.Add(CommandsByName[name]);
                    }
                }

                if (DebugLog)
                {
                    Debug.Log("Search done! " + EditorApplication.timeSinceStartup);
                }

                FindByAliases(searchTerms, foundCommands);

                if (foundCommands.Count > 0)
                {
                    if (MonKeyInternalSettings.Instance.PutInvalidCommandAtEndOfSearch)
                    {
                        return foundCommands.OrderByDescending(_ => !_.HasValidation
                                                                    || _.IsValid);
                    }

                    return foundCommands.Take(MaxCommandShown);
                }


            }
            return new CommandInfo[0];
        }

        private void FindByAliases(string[] searchTerms, List<CommandInfo> foundCommands)
        {
            if (foundCommands.Count < CommandFoundCountForAliases)
            {
                List<string> orderedAliases =
                    StringExt.OrderStringsBySearchScore(WordAliases.Keys, false, searchTerms).ToList();
                if (orderedAliases.Any())
                {
                    foreach (var name in StringExt
                        .OrderStringsBySearchScore(CommandsByName.Keys, false, WordAliases[orderedAliases.ElementAt(0)])
                        .ThenBy(_ => CommandsByName[_].CommandOrder))
                    {
                        if (!foundCommands.Contains(CommandsByName[name]) &&
                            IsCommandAuthorized(CommandsByName[name]))
                        {
                            foundCommands.Add(CommandsByName[name]);
                        }
                    }
                }
            }

        }

        public IEnumerable<CommandInfo> AlwaysShownCommands
        {
            get
            {
                return CommandsByName.Values
                  .Where(_ => _.AlwaysShow)
                  .Distinct()
                  .OrderBy(_ => _.CommandOrder)
                  .ThenByDescending(_ => !_.HasValidation || _.IsValid);
            }
        }

        #endregion
    }


    /*private class Test
    {
        public  void GenerateAndSortAutoComplete(string searchTerms)
        {
            typesOrdered = objectsPerName.Where(type =>
                {
                    if (!type.Value.Name.ToLower().Contains(searchTerms))
                    {
                        //Use Fuzzy Match instead default one
                        var x = StringExt.MatchResultSet(new List<string> { type.Value.Name.NicifyVariableName().ToLower() }, searchTerms);
                        return x.Count > 0;
                    }
                    return true;
                })
                .Convert(_ => _.Value)
                .OrderByDescending(type => type.Name.NicifyVariableName().ToLower().WordSearchScore(false, searchTerms))
                .Take(MaxPick)
                .ToArray();
        }
    }*/


}