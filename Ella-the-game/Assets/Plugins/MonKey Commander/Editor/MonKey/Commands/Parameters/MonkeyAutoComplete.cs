using MonKey;
using MonKey.Editor.Console;
using MonKey.Editor.Internal;
using MonKey.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace MonKey.Editor.Internal
{

    public static class AutoCompleteManager
    {
        private static bool initialized = false;
        private static readonly Dictionary<Type, GenericCommandParameterAutoComplete>
            DefaultAutoCompletes = new Dictionary<Type, GenericCommandParameterAutoComplete>();

        public static AssetNameAutoComplete AssetNameAutoComplete = new AssetNameAutoComplete();
        public static LayerAutoComplete LayerAutoComplete = new LayerAutoComplete();
        public static TagAutoComplete TagAutoComplete = new TagAutoComplete();

        public static void InitializeManager()
        {
            if (initialized)
                return;
            initialized = true;

            DefaultAutoCompletes.Add(typeof(GameObject),
                new ObjectTypeAutoComplete<GameObject>());

            DefaultAutoCompletes.Add(typeof(float),
                new CommandParameterAutoComplete<float>()
                    .AddValue("0.1", 0.1f).AddValue("0.5", 0.5f)
                    .AddValue("1", 1f).AddValue("10", 10f)
                    .AddValue("50", 50).AddValue("100", 100f));

            DefaultAutoCompletes.Add(typeof(int),
                new CommandParameterAutoComplete<int>()
                    .AddValue("1", 1).AddValue("5", 5)
                    .AddValue("10", 10).AddValue("50", 50)
                    .AddValue("100", 100).AddValue("1000", 1000));

            DefaultAutoCompletes.Add(typeof(double),
                new CommandParameterAutoComplete<double>()
                    .AddValue("0.1", 0.1).AddValue("0.5", 0.5)
                    .AddValue("1", 1).AddValue("10", 10)
                    .AddValue("50", 50).AddValue("100", 100));

            DefaultAutoCompletes.Add(typeof(Color), new ColorAutoComplete(true));

            DefaultAutoCompletes.Add(typeof(Vector3),
                new CommandParameterAutoComplete<Vector3>()
                    .AddValue("Zero", Vector3.zero).AddValue("One", Vector3.one)
                    .AddValue("Right", Vector3.right).AddValue("Left", Vector3.left)
                    .AddValue("Up", Vector3.up).AddValue("Down", Vector3.down)
                .AddValue("Forward", Vector3.forward).AddValue("Back", Vector3.back));

            DefaultAutoCompletes.Add(typeof(bool),
                new CommandParameterAutoComplete<bool>()
                    .AddValue("True", true).AddValue("False", false));
            DefaultAutoCompletes.Add(typeof(Scene),
                new SceneAutoComplete());
        }

        public static GenericCommandParameterAutoComplete GetDefaultAutoComplete(Type type)
        {
            if (type.IsEnum)
                return new EnumAutoComplete(type);

            if (DefaultAutoCompletes.ContainsKey(type))
            {
                return DefaultAutoCompletes[type];
            }

            while (type.BaseType != null)
            {
                if (DefaultAutoCompletes.ContainsKey(type))
                {
                    return DefaultAutoCompletes[type];
                }
                type = type.BaseType;
            }

            return null;
        }

        public static StaticCommandParameterAutoComplete FieldAutoComplete(int typeFieldID, Type fieldType)
        {
            var typeFound = (Type)MonkeyEditorUtils.GetParsedParameter(typeFieldID);

            var fields = typeFound.
                GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Default);
            var properties = typeFound.GetProperties(BindingFlags.NonPublic | BindingFlags.Public |
                                                     BindingFlags.Instance | BindingFlags.FlattenHierarchy |
                                                     BindingFlags.Default & BindingFlags.SetProperty);

            var complete = new StaticCommandParameterAutoComplete(typeof(string), new Dictionary<string, object>());

            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType == fieldType)
                    complete.ObjectsPerName.Add(fieldInfo.Name, fieldInfo.Name);
            }

            foreach (var fieldInfo in properties)
            {
                if (fieldInfo.PropertyType == fieldType)
                    complete.ObjectsPerName.Add(fieldInfo.Name, fieldInfo.Name);
            }

            return complete;
        }

        public static TypeAutoComplete TypeAutoCompleteFromSelection()
        {
            var typeAuto = new TypeAutoComplete(false, false, false, false, false);

            foreach (var o in Selection.objects)
            {
                if (o is GameObject go)
                {
                    typeAuto.AddNewType("Game Object", typeof(GameObject));
                    var types = go.GetComponents<Component>();
                    foreach (var component in types)
                    {
                        var type = component.GetType();
                        typeAuto.AddNewType(type.Name, type);
                    }
                }
                else
                {
                    typeAuto.AddNewType(o.GetType().Name, o.GetType());
                }
            }
            return typeAuto;
        }
    }

    /// <summary>
    /// The base class understood by MonKey as a provider of value for a text entered in the interface.
    /// Extend this class if you want a specific behavior for your autocomplete.
    /// Chances are that what you want is already implemented by one of the available classes in MonKeyAutoComplete though,
    /// so I recommend you check it out first!
    /// </summary>
    public abstract class GenericCommandParameterAutoComplete
    {
        /// <summary>
        /// The type of the value that will be returned by the autocomplete
        /// </summary>
        public Type ParameterType;

        /// <summary>
        /// You must specify the type in the constructor so that MonKey can recognize it
        /// </summary>
        /// <param name="parameterType"></param>
        protected GenericCommandParameterAutoComplete(Type parameterType)
        {
            ParameterType = parameterType;
        }

        /// <summary>
        /// Called when the tab of teh variable is opened:
        /// init here values to fetch even when no search terms are entered
        /// </summary>
        public abstract void InitializeAutoComplete();

        /// <summary>
        /// Called when the autoComplete is no longer needed
        /// </summary>
        public abstract void DropAutoComplete();

        /// <summary>
        /// Called every time the user enters a search term : 
        /// try to limit performance heavy operations, or thread them when possible
        /// </summary>
        /// <param name="searchTerms"></param>
        public abstract void GenerateAndSortAutoComplete(string searchTerms);

        /// <summary>
        /// The current amount of values suggested to the user
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Retreives the value suggested from the id selected by the user in the list of available values.
        /// </summary>
        /// <param name="sortId"></param>
        /// <returns></returns>
        public abstract object GetValue(int sortId);

        /// <summary>
        /// The text representation of the value to display in the interface
        /// </summary>
        /// <param name="sortID"></param>
        /// <returns></returns>
        public abstract string GetStringValue(int sortID);

        /// <summary>
        /// The instruction text that will appear in the search bar
        /// </summary>
        public abstract string SearchInstruction { get; }

        /// <summary>
        /// Override to change the look of each auto complete member drawn
        /// </summary>
        /// <param name="id"></param>
        /// <param name="searchTerms"> the terms entered by the user (as one string)</param>
        /// <param name="selected"> true if the currently selected id is this one </param>
        public abstract void DrawAutoCompleteMember(int id, string searchTerms,
            bool selected);
    }

    public class AssetNameAutoComplete : GenericCommandParameterAutoComplete
    {
        private const int MaxShownAssets = 20;
        public const int MaxPathLength = 50;
        public string CustomType = "Object";
        public bool IncludeDirectories = false;
        public bool DirectoryMode;
        public bool PopulateOnInit = false;
        public Func<string, bool> SpecialAssetFilter;
        public bool showAssetPreview = false;

        private int resultIDToUpdate;
        protected readonly Dictionary<int, Texture2D> PreviewTextures = new Dictionary<int, Texture2D>();

        public AssetNameAutoComplete() : base(typeof(string))
        {
        }

        protected string[] AssetsInfos;


        public override void InitializeAutoComplete()
        {
            if (PopulateOnInit)
                GenerateAndSortAutoComplete("");
            if (showAssetPreview)
                MonkeyEditorUtils.AddEditorDelegate(UpdatePreviewTextures, 0.25f);
        }

        public override void DropAutoComplete()
        {
            Resources.UnloadUnusedAssets();
            PreviewTextures.Clear();
        }

        public override void GenerateAndSortAutoComplete(string searchTerms)
        {
            string toSearch = searchTerms.ToLower();

            if (!CustomType.IsNullOrEmpty())
            {
                toSearch += " t:" + CustomType;
            }

            IEnumerable<string> initialSearch = InitialSearch(toSearch, searchTerms);

            if (DirectoryMode)
            {
                initialSearch = initialSearch.Where(_ => !AssetDatabase.GUIDToAssetPath(_)
                    .Contains("."));
            }
            else if (!IncludeDirectories)
            {
                initialSearch = initialSearch.Where(_ => AssetDatabase.GUIDToAssetPath(_)
                    .Contains("."));
            }

            if (SpecialAssetFilter != null)
            {
                initialSearch = initialSearch.Where(_ => SpecialAssetFilter(_));
            }

            AssetsInfos = initialSearch.Take(MaxShownAssets).ToArray();
            resultIDToUpdate = 0;
        }

        private void UpdatePreviewTextures()
        {
            if (AssetsInfos == null || resultIDToUpdate >= AssetsInfos.Length)
                return;

            if (!PreviewTextures.ContainsKey(resultIDToUpdate))
            {
                PreviewTextures.Add(resultIDToUpdate, null);
            }

            Object loaded = AssetDatabase.LoadAssetAtPath<Object>((string)GetValue(resultIDToUpdate));
            if (loaded is Texture2D d)
                PreviewTextures[resultIDToUpdate] = d;
            else
            {
                PreviewTextures[resultIDToUpdate] = AssetPreview.GetAssetPreview(
                    AssetDatabase.LoadAssetAtPath<Object>((string)GetValue(resultIDToUpdate)));
                //   Resources.UnloadAsset(loaded);
            }


            resultIDToUpdate++;
        }

        private bool IsDirectory(string assetPath)
        {
            return !assetPath.Contains(".");
        }

        private IEnumerable<string> InitialSearch(string toSearch, string searchTerms)
        {
            return AssetDatabase.FindAssets(toSearch)
                .OrderByDescending(_ => AssetDatabase.GUIDToAssetPath(_)
                .GetAssetNameFromPath().ToLower()
                    .SentenceSearchScore(searchTerms.ToLower().Split(new[] { " " },
                        StringSplitOptions.RemoveEmptyEntries)));
        }

        public override int Count
        {
            get { return AssetsInfos == null ? 0 : AssetsInfos.Length; }
        }

        public override object GetValue(int sortId)
        {
            return AssetDatabase.GUIDToAssetPath(AssetsInfos[sortId]);
        }

        public override string GetStringValue(int sortID)
        {
            string path = AssetDatabase.GUIDToAssetPath(AssetsInfos[sortID]);
            if (!IsDirectory(path))
                return path.GetAssetNameFromPath();

            return path.GetDirectoryNameFromPath();

        }

        public override string SearchInstruction
        {
            get { return "Enter an asset name"; }
        }

        public override void DrawAutoCompleteMember(int id, string searchTerms, bool selected)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            Texture2D icon = (Texture2D)(IsDirectory((string)GetValue(id))
                ? EditorGUIUtility.IconContent("Folder Icon").image
                : MonkeyStyle.GetIconForFile((string)GetValue(id)));

            if (showAssetPreview)
            {

                Texture2D prev = PreviewTextures.ContainsKey(id) ? PreviewTextures[id] : null;
                if (prev)
                {
                    GUILayout.Label("",
                        new GUIStyle()
                        {
                            fixedHeight = 50,
                            fixedWidth = 50,
                            margin = new RectOffset(5, 5, 0, 0),
                        });
                    GUI.DrawTexture(GUILayoutUtility.GetLastRect(), prev, ScaleMode.ScaleToFit);
                }
                else
                {
                    GUILayout.Label("",
                        new GUIStyle()
                        {
                            fixedWidth = 20,
                            fixedHeight = 20,
                            margin = new RectOffset(0, 2, 0, 0),
                            normal = { background = icon }
                        });
                }
            }
            else
            {
                GUILayout.Label("",
                    new GUIStyle()
                    {
                        fixedWidth = 20,
                        fixedHeight = 20,
                        margin = new RectOffset(0, 2, 0, 0),
                        normal = { background = icon }
                    });
            }

            GUILayout.BeginHorizontal(new GUIStyle() { margin = new RectOffset(0, 2, 0, 0) });
            GUILayout.Label(GetStringValue(id).Highlight(searchTerms, true,
                StringExt.ColorTag(MonkeyStyle.Instance.SearchResultTextColor),
                StringExt.ColorTagClosing,
                StringExt.ColorTag(MonkeyStyle.Instance.HighlightOnSelectedTextColor),
                StringExt.ColorTagClosing), MonkeyStyle.Instance.AssetNameStyle);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

            string path = (string)GetValue(id);
            path = path.Replace("Assets/", "");

            int idName = path.LastIndexOf(path.GetAssetNameFromPath(), StringComparison.Ordinal);
            if (idName > 0)
                path = path.Substring(0, idName - 1);

            if (path.Length > MaxPathLength)
                path = path.Substring(0, Mathf.Min(MaxPathLength, path.Length)) + "...";
            GUILayout.Label(path, MonkeyStyle.Instance.PathStyle/*, GUILayout.MaxWidth(150)*/);

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        public static AssetNameAutoComplete GetAssetAutoCompleteForTypeFromCurrentCommand(int typeParameterIndex)
        {
            //assumes the type is on the first parameter
            string type = (string)CommandConsoleWindow.CurrentPanel.CurrentExecution.GetValueParsed(typeParameterIndex);
            return new AssetNameAutoComplete() { CustomType = type };
        }
    }

    public class ObjectTypeAutoComplete<T> : GenericCommandParameterAutoComplete where T : Object
    {
        private const int MaxShownObjects = 20;
        private List<Object> indexedObjects;

        public bool SceneObjectsOnly = true;

        public ObjectTypeAutoComplete() : base(typeof(T))
        {
        }

        public override void InitializeAutoComplete()
        {
        }

        public override void DropAutoComplete()
        {

        }

        private bool IsSceneObject(Object obj)
        {
            GameObject go = obj as GameObject;
            return go && go.scene.IsValid();
        }

        public override void GenerateAndSortAutoComplete(string searchTerms)
        {

            IEnumerable<Object> objs = Resources.FindObjectsOfTypeAll(ParameterType)
                 .Where(_ => (_.hideFlags & HideFlags.NotEditable) == 0)
                 .Where(_ => !SceneObjectsOnly || IsSceneObject(_));

            string[] separatedTerms = searchTerms.Split(' ');
            indexedObjects = objs.OrderByDescending(_ =>
            _.name.ToLower().SentenceSearchScore(separatedTerms)).ThenBy(_ => _.name.Length)
                  .Take(MaxShownObjects).ToList();
        }

        public override int Count => indexedObjects?.Count ?? 0;

        public override object GetValue(int sortId)
        {
            return indexedObjects[sortId];
        }

        public override string GetStringValue(int sortID)
        {
            return indexedObjects[sortID].name;
        }

        public override string SearchInstruction
        {
            get { return "Enter the name of the object"; }
        }


        public override void DrawAutoCompleteMember(int id, string searchTerms, bool selected)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("",
                new GUIStyle()
                {
                    fixedWidth = 20,
                    fixedHeight = 20,
                    margin = new RectOffset(0, 2, 0, 0),
                    normal = {background =
                    EditorGUIUtility.FindTexture("PrefabNormal Icon")}
                });

            GUILayout.Label(GetStringValue(id).Highlight(searchTerms, true,
                StringExt.ColorTag(MonkeyStyle.Instance.SearchResultTextColor),
                StringExt.ColorTagClosing,
                     StringExt.ColorTag(MonkeyStyle.Instance.HighlightOnSelectedTextColor),
                 StringExt.ColorTagClosing),
                 MonkeyStyle.Instance.AssetNameStyle, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Generates an auto-complete interface based on a static set of values from a dictionary
    /// </summary>
    public class StaticCommandParameterAutoComplete : GenericCommandParameterAutoComplete
    {

        public Dictionary<string, object> ObjectsPerName = new Dictionary<string, object>();
        protected List<string> ObjectNamesOrdered = new List<string>();

        public StaticCommandParameterAutoComplete(Type parameterType) : base(parameterType)
        {
        }

        public StaticCommandParameterAutoComplete(Type parameterType, Dictionary<string, object> objectsPerName) : base(parameterType)
        {
            ObjectsPerName = objectsPerName;
        }

        public override void InitializeAutoComplete()
        {
            ObjectNamesOrdered.Clear();
            ObjectNamesOrdered.AddRange(ObjectsPerName.Keys);
        }

        public override void DropAutoComplete()
        {

        }

        public override void GenerateAndSortAutoComplete(string searchTerms)
        {
            ObjectNamesOrdered = ObjectNamesOrdered.
                OrderByDescending(_ => _.SentenceSearchScore(searchTerms.Split(' '))).ToList();
        }

        public override int Count
        {
            get { return ObjectNamesOrdered.Count; }
        }

        public override object GetValue(int sortId)
        {
            return ObjectsPerName[ObjectNamesOrdered[sortId]];
        }

        public override string GetStringValue(int sortID)
        {
            return ObjectNamesOrdered[sortID];
        }

        public override string SearchInstruction
        {
            get { return "Enter a value"; }
        }

        public override void DrawAutoCompleteMember(int id, string searchTerms,
            bool selected)
        {
            GUILayout.BeginHorizontal(new GUIStyle() { margin = new RectOffset(2, 2, 0, 0) });
            GUILayout.Label(ObjectNamesOrdered[id].Highlight(searchTerms, true,
                    StringExt.ColorTag(MonkeyStyle.Instance.SearchResultTextColor),
                    StringExt.ColorTagClosing,
                    StringExt.ColorTag(MonkeyStyle.Instance.HighlightOnSelectedTextColor),
                    StringExt.ColorTagClosing),
                MonkeyStyle.Instance.CommandNameStyle, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
        }


    }

    public class CommandParameterAutoComplete<T> : StaticCommandParameterAutoComplete
    {
        public CommandParameterAutoComplete() : base(typeof(T))
        {
            ParameterType = typeof(T);
        }

        public virtual CommandParameterAutoComplete<T> AddValue(string alias, T value)
        {
            if (!ObjectsPerName.ContainsKey(alias))
                ObjectsPerName.Add(alias, value);
            else
            {
                ObjectsPerName[alias] = value;
            }

            return this;
        }
    }

    public class CommandParameterObjectAutoComplete<T> : CommandParameterAutoComplete<T> where T : Object
    {
        public override void DrawAutoCompleteMember(int id, string searchTerms, bool selected)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("",
                new GUIStyle()
                {
                    fixedWidth = 20,
                    fixedHeight = 20,
                    margin = new RectOffset(0, 2, 0, 0),
                    normal = {background =
                        EditorGUIUtility.FindTexture("PrefabNormal Icon")}
                });

            Object obj = (Object)GetValue(id);
            GUILayout.Label(obj.name.Highlight(searchTerms, true,
                    StringExt.ColorTag(MonkeyStyle.Instance.SearchResultTextColor),
                    StringExt.ColorTagClosing,
                    StringExt.ColorTag(MonkeyStyle.Instance.HighlightOnSelectedTextColor),
                    StringExt.ColorTagClosing),
                MonkeyStyle.Instance.AssetNameStyle, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
        }
    }

    public class TagAutoComplete : CommandParameterAutoComplete<string>
    {
        public override void InitializeAutoComplete()
        {
            ObjectsPerName.Clear();
            foreach (string tag in UnityEditorInternal.InternalEditorUtility.tags)
            {
                ObjectsPerName.Add(tag, tag);
            }
            base.InitializeAutoComplete();
        }
    }

    public class LayerAutoComplete : CommandParameterAutoComplete<int>
    {
        public override void InitializeAutoComplete()
        {
            ObjectsPerName.Clear();
            for (int i = 0; i <= 31; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName.Length > 0)
                    ObjectsPerName.Add(layerName, i);
            }
            base.InitializeAutoComplete();
        }
    }

    public class EnumAutoComplete : StaticCommandParameterAutoComplete
    {

        public EnumAutoComplete(Type type) : base(type)
        {
            if (!type.IsEnum)
                Debug.LogErrorFormat("Monkey Error: the type {0} is not an enum: " +
                                     "cannot create an enum auto complete out of it!",
                    ParameterType);
        }

        public override void InitializeAutoComplete()
        {
            if (!ParameterType.IsEnum)
            {
                return;
            }

            ObjectsPerName.Clear();

            foreach (var en in Enum.GetValues(ParameterType))
            {
                string name = Enum.GetName(ParameterType, en);

                if (name != null)
                    ObjectsPerName.Add(name.NicifyVariableName(), en);
            }

            base.InitializeAutoComplete();
        }
    }

    public class SceneAutoComplete : StaticCommandParameterAutoComplete
    {

        public SceneAutoComplete() : base(typeof(Scene))
        {
        }

        public override void InitializeAutoComplete()
        {
            ObjectsPerName.Clear();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                ObjectsPerName.Add(SceneManager.GetSceneAt(i).name, SceneManager.GetSceneAt(i));
            }
            base.InitializeAutoComplete();
        }

        public override void DrawAutoCompleteMember(int id, string searchTerms, bool selected)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("",
                new GUIStyle()
                {
                    fixedWidth = 20,
                    fixedHeight = 20,
                    normal = {background =
                        MonkeyStyle.GetIconForFile("fake.unity")}
                });

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label(GetStringValue(id).Highlight(searchTerms, true,
                    StringExt.ColorTag(MonkeyStyle.Instance.SearchResultTextColor),
                    StringExt.ColorTagClosing,
                    StringExt.ColorTag(MonkeyStyle.Instance.HighlightOnSelectedTextColor),
                    StringExt.ColorTagClosing), MonkeyStyle.Instance.CommandNameStyle,
                GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }

    public sealed class ColorAutoComplete : CommandParameterAutoComplete<Color>
    {
        private readonly Dictionary<string, Texture2D> colorTextures = new Dictionary<string, Texture2D>();

        public ColorAutoComplete(bool includeDefaultColors)
        {
            Resources.UnloadUnusedAssets();
            if (includeDefaultColors)
            {
                AddValue("Black", Color.black);
                AddValue("White", Color.white);
                AddValue("Cadet Blue", ColorExt.HTMLColor("#5F9EA0"));
                AddValue("Saddle Brown", ColorExt.HTMLColor("#8B4513"));
                AddValue("Teal Cyan", ColorExt.HTMLColor("#008080"));
                AddValue("Spring Green", ColorExt.HTMLColor("#00FF7F"));
                AddValue("Indian Red", ColorExt.HTMLColor("#CD5C5C"));
                AddValue("Coral Orange", ColorExt.HTMLColor("#FF7F50"));
                AddValue("Plum Purple", ColorExt.HTMLColor("#DDA0DD"));
                AddValue("Dim Gray", ColorExt.HTMLColor("#696969"));
                AddValue("Dark Gray", ColorExt.HTMLColor("#A9A9A9"));
                AddValue("Silver Gray", ColorExt.HTMLColor("#C0C0C0"));
                AddValue("Beige", ColorExt.HTMLColor("#F5F5DC"));
                AddValue("Ivory White", ColorExt.HTMLColor("#FFFFF0"));
                AddValue("Azure White", ColorExt.HTMLColor("#F0FFFF"));
                AddValue("Blue", Color.blue);
                AddValue("Light Blue", ColorExt.HTMLColor("#ADD8E6"));
                AddValue("Navy Blue", ColorExt.HTMLColor("#000080"));
                AddValue("Indigo Purple", ColorExt.HTMLColor("#4B0082"));
                AddValue("Royal Blue", ColorExt.HTMLColor("#4169E1"));
                AddValue("Maroon Brown", ColorExt.HTMLColor("#800000"));
                AddValue("Chocolate Brown", ColorExt.HTMLColor("#D2691E"));
                AddValue("Clear", Color.clear);
                AddValue("Cyan", Color.cyan);
                AddValue("Aquamarine Cyan", ColorExt.HTMLColor("#7FFFD4"));
                AddValue("Turquoise Cyan", ColorExt.HTMLColor("#40E0D0"));
                AddValue("Gray", Color.gray);
                AddValue("Green", Color.green);
                AddValue("Forest Green", ColorExt.HTMLColor("#228B22"));
                AddValue("Light Green", ColorExt.HTMLColor("#90EE90"));
                AddValue("Sea Green", ColorExt.HTMLColor("#2E8B57"));
                AddValue("Olive Green", ColorExt.HTMLColor("#808000"));
                AddValue("Grey", Color.grey);
                AddValue("Magenta", Color.magenta);
                AddValue("Red", Color.red);
                AddValue("Salmon Red", ColorExt.HTMLColor("#FA8072"));
                AddValue("Crimson Red", ColorExt.HTMLColor("#DC143C"));
                AddValue("Brick Red", ColorExt.HTMLColor("#B22222"));
                AddValue("Yellow", Color.yellow);
                AddValue("Khaki Yellow", ColorExt.HTMLColor("#F0E68C"));
                AddValue("Light Yellow", ColorExt.HTMLColor("#FFFF99"));
                AddValue("Dark Yellow", ColorExt.HTMLColor("#666600"));
                AddValue("Orange", ColorExt.HTMLColor("#FFA500"));
                AddValue("Tomato Orange", ColorExt.HTMLColor("#FF6347"));
                AddValue("Gold Yellow", ColorExt.HTMLColor("#FFD700"));
                AddValue("Dark Orange", ColorExt.HTMLColor("#FF8C00"));
                AddValue("Pink", ColorExt.HTMLColor("#FFC0CB"));
                AddValue("Hot Pink", ColorExt.HTMLColor("#FF69B4"));
                AddValue("Deep Pink", ColorExt.HTMLColor("#FF1493"));
                AddValue("Violet Purple", ColorExt.HTMLColor("#EE82EE"));
                AddValue("Medium Purple", ColorExt.HTMLColor("#9370DB"));
            }
        }

        public override void DropAutoComplete()
        {
            base.DropAutoComplete();
            foreach (var colorTexture in colorTextures)
            {
                Resources.UnloadAsset(colorTexture.Value);
            }
            colorTextures.Clear();
        }

        public override CommandParameterAutoComplete<Color> AddValue(string alias, Color value)
        {
            base.AddValue(alias, value);

            if (!colorTextures.ContainsKey(alias))
                colorTextures.Add(alias, MonkeyStyle.ColorTexture(1, 1, value));

            return this;
        }

        public override void DrawAutoCompleteMember(int id, string searchTerms, bool selected)
        {
            GUILayout.BeginHorizontal(new GUIStyle() { margin = new RectOffset(2, 2, 0, 0) });

            GUILayout.Label("", new GUIStyle()
            {
                fixedHeight = 20,
                fixedWidth = 40,
                margin = new RectOffset(5, 5, 0, 0),
                normal = { background = colorTextures[ObjectNamesOrdered[id]] }
            });

            GUILayout.Label(ObjectNamesOrdered[id].Highlight(searchTerms, true,
                    StringExt.ColorTag(MonkeyStyle.Instance.SearchResultTextColor),
                    StringExt.ColorTagClosing,
                    StringExt.ColorTag(MonkeyStyle.Instance.HighlightOnSelectedTextColor),
                    StringExt.ColorTagClosing),
                MonkeyStyle.Instance.CommandNameStyle, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

        }
    }

    public class TypeAutoComplete : GenericCommandParameterAutoComplete
    {
        private readonly bool includeObjectType;
        private readonly bool includeComponentType;
        private readonly bool includeMonoBehaviorType;
        private readonly bool includeScriptableObjectType;
        private readonly bool includeEditorObjectType;

        public int MaxPick = 20;

        private readonly Dictionary<string, Type> objectsPerName = new Dictionary<string, Type>();
        private Type[] typesOrdered;

        public TypeAutoComplete(bool includeObjectType = true,
            bool includeComponentType = true, bool includeMonoBehaviorType = true,
            bool includeScriptableObjectType = true, bool includeEditorObjectType = false) : base(typeof(Type))
        {
            this.includeObjectType = includeObjectType;
            this.includeComponentType = includeComponentType;
            this.includeMonoBehaviorType = includeMonoBehaviorType;
            this.includeScriptableObjectType = includeScriptableObjectType;
            this.includeEditorObjectType = includeEditorObjectType;
            AddAllTypes();
        }

        public bool ContainsKey(string name)
        {
            return objectsPerName.ContainsKey(name);
        }

        private void AddAllTypes()
        {
            if (includeComponentType)
                objectsPerName.AddRange(TypeManager.AllComponentObjectTypes);
            if (includeMonoBehaviorType)
                objectsPerName.AddRange(TypeManager.AllMonoBehaviorObjectTypes);
            if (includeObjectType)
                objectsPerName.AddRange(TypeManager.AllObjectsTypes);
            if (includeScriptableObjectType)
                objectsPerName.AddRange(TypeManager.AllScriptableObjectTypes);
            if (includeEditorObjectType)
                objectsPerName.AddRange(TypeManager.AllEditorTypes);

        }

        public TypeAutoComplete AddNewType(string name, Type type)
        {
            if (objectsPerName.ContainsKey(name))
                return this;

            objectsPerName.Add(name, type);
            return this;
        }

        public override void InitializeAutoComplete()
        {
            GenerateAndSortAutoComplete("");
        }

        public override void DropAutoComplete()
        {
            objectsPerName.Clear();
            typesOrdered = null;
        }

        public override void GenerateAndSortAutoComplete(string searchTerms)
        {
            IEnumerable<Type> tempTypes;
            tempTypes = objectsPerName
                 /* .Where(_ =>
                      _.Value.Name.ToLower().Contains(searchTerms) ||
                      _.Value.Name.NicifyVariableName().ToLower().Contains(searchTerms)
                  )*/.Convert(_ => _.Value);

            typesOrdered = tempTypes.OrderByDescending(_ => _.Name.
                    NicifyVariableName().ToLower().
                    WordSearchScore(false, searchTerms.Split(' ')))
                .Take(MaxPick).ToArray();
        }

        public override int Count
        {
            get
            {
                if (typesOrdered == null)
                    return 0;
                return typesOrdered.Length;
            }
        }

        public override object GetValue(int sortId)
        {
            return typesOrdered[sortId];
        }

        public override string GetStringValue(int sortID)
        {
            return typesOrdered[sortID].Name.
                NicifyVariableName();
        }

        public override string SearchInstruction
        {
            get { return "Enter a Type Name"; }
        }

        public override void DrawAutoCompleteMember(int id, string searchTerms, bool selected)
        {
            GUILayout.BeginHorizontal(new GUIStyle() { margin = new RectOffset(2, 2, 0, 0) });
            GUILayout.Label(GetStringValue(id).Highlight(searchTerms, true,
                    StringExt.ColorTag(MonkeyStyle.Instance.SearchResultTextColor),
                    StringExt.ColorTagClosing,
                    StringExt.ColorTag(MonkeyStyle.Instance.HighlightOnSelectedTextColor),
                    StringExt.ColorTagClosing),
                MonkeyStyle.Instance.CommandNameStyle, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
        }
    }


}
