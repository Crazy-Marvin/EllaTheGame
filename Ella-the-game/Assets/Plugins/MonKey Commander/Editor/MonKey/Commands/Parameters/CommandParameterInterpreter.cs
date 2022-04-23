using MonKey.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonKey.Editor.Internal
{
    public abstract class CommandParameterInterpreter
    {
        private static readonly Dictionary<Type, CommandParameterInterpreter> InterpretersByType =
            new Dictionary<Type, CommandParameterInterpreter>();

        public static char ArrayVariableSeparator = ',';

        public static void AddInterpreter(CommandParameterInterpreter interpreter)
        {
            if (!InterpretersByType.ContainsKey(interpreter.Type))
            {
                InterpretersByType.Add(interpreter.Type, interpreter);
            }
        }

        public static CommandParameterInterpreter GetInterpreter(Type type)
        {
            if (type == null)
                return null;

            if (InterpretersByType.ContainsKey(type))
            {
                return InterpretersByType[type];
            }
            else if (type.IsArray)
                return GetInterpreter(type.GetElementType());
            else
            {
                while (type.BaseType != null)
                {
                    if (InterpretersByType.ContainsKey(type))
                    {
                        return InterpretersByType[type];
                    }
                    type = type.BaseType;
                }
            }

            return null;
        }

        public Type Type { get; private set; }

        public string VisualPrefix { get; protected set; }
        public string VisualSuffix { get; protected set; }

        public bool HasPrefix { get { return VisualPrefix.IsNullOrEmpty(); } }
        public bool HasSuffix { get { return VisualSuffix.IsNullOrEmpty(); } }

        protected CommandParameterInterpreter(Type type)
        {
            Type = type;
        }

        public abstract bool TryParse(string text, out object obj, Type subType = null);

        public bool TryParseArray(string text, out object[] objs)
        {
            string[] terms = text.Split(new[] { ArrayVariableSeparator });
            objs = new object[terms.Length];
            for (int i = 0; i < terms.Length; i++)
            {
                object o;
                if (TryParse(terms[i], out o))
                {
                    objs[i] = o;
                }
                else
                {
                    objs = null;
                    return false;
                }
            }
            return true;
        }

        class StringInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new StringInterpreter());
            }

            private StringInterpreter() : base(typeof(string))
            {

            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                obj = text;
                return true;
            }
        }

        class IntInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new IntInterpreter());
            }

            private IntInterpreter() : base(typeof(int))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                int outInt;
                if (int.TryParse(text, out outInt))
                {
                    obj = outInt;
                    return true;
                }
                obj = null;
                return true;
            }
        }

        class FloatInterpreter : CommandParameterInterpreter
        {

            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new FloatInterpreter());
            }

            private FloatInterpreter() : base(typeof(float))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                float outFloat;
                if (float.TryParse(text, out outFloat))
                {
                    obj = outFloat;
                    return true;
                }
                obj = null;
                return true;
            }
        }

        class DoubleInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new DoubleInterpreter());
            }

            private DoubleInterpreter() : base(typeof(double))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                double outDouble;
                if (double.TryParse(text, out outDouble))
                {
                    obj = outDouble;
                    return true;
                }
                obj = null;
                return true;
            }
        }

        class ByteInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new ByteInterpreter());
            }

            private ByteInterpreter() : base(typeof(byte))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                byte outByte;
                if (byte.TryParse(text, out outByte))
                {
                    obj = outByte;
                    return true;
                }
                obj = null;
                return true;
            }
        }

        class BoolInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new BoolInterpreter());
            }

            protected BoolInterpreter() : base(typeof(bool))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                bool outBool;

                if (text == "0")
                    text = "false";
                else if (text == "1")
                    text = "true";
                else if (text == "t")
                    text = "true";
                else if (text == "f")
                    text = "false";

                if (bool.TryParse(text, out outBool))
                {
                    obj = outBool;
                    return true;
                }
                obj = null;
                return true;
            }
        }

        class CharInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new CharInterpreter());
            }

            protected CharInterpreter() : base(typeof(char))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                char outChar;

                if (char.TryParse(text, out outChar))
                {
                    obj = outChar;
                    return true;
                }
                obj = null;
                return true;
            }
        }

        class LongInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new LongInterpreter());
            }

            protected LongInterpreter() : base(typeof(long))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                long outLong;

                if (long.TryParse(text, out outLong))
                {
                    obj = outLong;
                    return true;
                }
                obj = null;
                return true;
            }
        }

        class ShortInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new ShortInterpreter());
            }

            protected ShortInterpreter() : base(typeof(short))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                short outShort;

                if (short.TryParse(text, out outShort))
                {
                    obj = outShort;
                    return true;
                }
                obj = null;
                return true;
            }

        }

        class Vector2Interpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new Vector2Interpreter());
            }

            protected Vector2Interpreter() : base(typeof(Vector2))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                string[] numberSplit = text.Split(new[] { ArrayVariableSeparator });

                if (numberSplit.Length > 2)
                {
                    obj = null;
                    return false;
                }
                float[] values = new float[2];

                for (int i = 0; i < numberSplit.Length; i++)
                {
                    float value;
                    if (float.TryParse(numberSplit[i], out value))
                    {
                        values[i] = value;
                    }
                    else
                    {
                        obj = null;
                        return false;
                    }
                }
                obj = new Vector2(values[0], values[1]);
                return true;
            }

        }

        class Vector3Interpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new Vector3Interpreter());
            }

            protected Vector3Interpreter() : base(typeof(Vector3))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                string[] numberSplit = text.Split(new[] { ArrayVariableSeparator });

                if (numberSplit.Length > 3)
                {
                    obj = null;
                    return false;
                }
                float[] values = new float[3];

                for (int i = 0; i < numberSplit.Length; i++)
                {
                    float value;
                    if (float.TryParse(numberSplit[i], out value))
                    {
                        values[i] = value;
                    }
                    else
                    {
                        obj = null;
                        return false;
                    }
                }
                obj = new Vector3(values[0], values[1], values[2]);
                return true;
            }

        }

        class Vector4Interpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new Vector4Interpreter());
            }

            protected Vector4Interpreter() : base(typeof(Vector4))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                string[] numberSplit = text.Split(new[] { ArrayVariableSeparator });

                if (numberSplit.Length > 4)
                {
                    obj = null;
                    return false;
                }
                float[] values = new float[4];

                for (int i = 0; i < numberSplit.Length; i++)
                {
                    float value;
                    if (float.TryParse(numberSplit[i], out value))
                    {
                        values[i] = value;
                    }
                    else
                    {
                        obj = null;
                        return false;
                    }
                }
                obj = new Vector4(values[0], values[1], values[2], values[3]);
                return true;
            }

        }

        class QuaternionInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new QuaternionInterpreter());
            }

            protected QuaternionInterpreter() : base(typeof(Quaternion))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                string[] numberSplit = text.Split(new[] { ArrayVariableSeparator });

                if (numberSplit.Length > 4)
                {
                    obj = null;
                    return false;
                }
                float[] values = new float[4];

                for (int i = 0; i < numberSplit.Length; i++)
                {
                    float value;
                    if (float.TryParse(numberSplit[i], out value))
                    {
                        values[i] = value;
                    }
                    else
                    {
                        obj = null;
                        return false;
                    }
                }
                obj = new Quaternion(values[0], values[1], values[2], values[3]);
                return true;
            }

        }

        class ColorInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new ColorInterpreter());
            }

            protected ColorInterpreter() : base(typeof(Color))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                string[] numberSplit = text.Split(new[] { ArrayVariableSeparator });

                if (numberSplit.Length > 4)
                {
                    obj = null;
                    return false;
                }
                float[] values = new float[4];

                //by default alpha is 1
                values[3] = 1;

                for (int i = 0; i < numberSplit.Length; i++)
                {
                    float value;
                    if (float.TryParse(numberSplit[i], out value))
                    {
                        values[i] = value;
                    }
                    else
                    {
                        obj = null;
                        return false;
                    }
                }
                obj = new Color(values[0], values[1], values[2], values[3]);
                return true;
            }

        }

        class ObjectInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new ObjectInterpreter());
            }

            protected ObjectInterpreter() : base(typeof(UnityEngine.Object))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {

                if (subType == null)
                {
                    obj = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)).
                        Where(_ => _.name == text);
                    return obj != null;
                }
                else
                {
                    obj = Resources.FindObjectsOfTypeAll(subType).
                      Where(_ => _.name == text);
                    return obj != null;
                }
            }
        }

        class ComponentInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new ComponentInterpreter());
            }

            protected ComponentInterpreter() : base(typeof(Component))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    foreach (var item in SceneManager.GetSceneAt(i).GetRootGameObjects())
                    {
                        if (item.name == text)
                        {
                            obj = item.GetComponent(subType);
                            if (obj != null)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            for (int j = 0; j < item.transform.childCount; j++)
                            {
                                if (item.transform.GetChild(j).name == text)
                                {
                                    obj = item.transform.GetChild(j).GetComponent(subType);
                                    if (obj != null)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }

                obj = null;
                return false;
            }
        }

        class GameObjectInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new GameObjectInterpreter());
            }

            protected GameObjectInterpreter() : base(typeof(GameObject))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    foreach (var item in SceneManager.GetSceneAt(i).GetRootGameObjects())
                    {
                        if (item.name == text)
                        {
                            obj = item;
                            return true;
                        }
                        else
                        {
                            for (int j = 0; j < item.transform.childCount; j++)
                            {
                                if (item.transform.GetChild(j).name == text)
                                {
                                    obj = item.transform.GetChild(j).gameObject;
                                    return true;
                                }
                            }
                        }
                    }
                }

                obj = null;
                return false;
            }
        }

        class LayerMaskInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new LayerMaskInterpreter());
            }

            private LayerMaskInterpreter() : base(typeof(LayerMask))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                string[] layers = text.Split(',');

                obj = null;

                foreach (string layer in layers)
                {
                    int id = LayerMask.NameToLayer(layer);
                    if (id == -1)
                        return false;
                }

                LayerMask mask = LayerMask.GetMask(layers);
                obj = mask;
                return true;
            }
        }

        class EnumInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new EnumInterpreter());
            }

            private EnumInterpreter() : base(typeof(Enum))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {

                obj = null;

                if (subType == null || !subType.IsEnum)
                    return false;
                try
                {
                    obj = Enum.Parse(subType, text, true);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        class SceneInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new SceneInterpreter());
            }

            private SceneInterpreter() : base(typeof(Scene))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                obj = null;
                Scene scene = SceneManager.GetSceneByName(text);
                obj = scene;
                return scene.IsValid();
            }
        }

        class TypeInterpreter : CommandParameterInterpreter
        {
            [InitializeOnLoadMethod]
            static void AddInterpreter()
            {
                AddInterpreter(new TypeInterpreter());
            }

            private TypeInterpreter() : base(typeof(Type))
            {
            }

            public override bool TryParse(string text, out object obj, Type subType = null)
            {
                obj = TypeManager.GetType(text);
                return obj == null;
            }
        }
    }
}
