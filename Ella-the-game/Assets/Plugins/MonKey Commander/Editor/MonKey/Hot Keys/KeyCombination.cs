using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MonKey.Internal
{
    public class KeyCombination
    {

        #region BORING LIST OF SHORTCUTS

        public static readonly char KeySymbol = '_';

        public static readonly char SeparatorSymbol = '+';

        public static readonly string[] SpecialKeys =
        {
        "LEFT", "RIGHT", "UP", "DOWN", "F2", "F3", "F4", "F5", "F6", "F7", "F8",
        "F9", "F10", "F11", "F12","F1", "HOME", "END", "PGUP", "PGDN"
    };

        public static readonly KeyCode[] SpecialKeyCodes =
        {
            KeyCode.LeftControl, KeyCode.LeftAlt, KeyCode.LeftShift, KeyCode.F1,
            KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5, KeyCode.F6, KeyCode.F7, KeyCode.F8, KeyCode.F9, KeyCode.F10,
            KeyCode.F11, KeyCode.F12, KeyCode.F13, KeyCode.F14, KeyCode.F15
        };

        public static readonly Dictionary<string, string> ModifierKeysAliases
            = new Dictionary<string, string>()
            {
            {"%", "CTRL"},
            {"&", "ALT"},
            {"#", "SHIFT"}
            };

        public static readonly Dictionary<string, KeyCode> KeycodeStringAliases
            = new Dictionary<string, KeyCode>()
        {
        {"CTRL",KeyCode.LeftControl },
        {"CONTROL",KeyCode.LeftControl },

        {"COMMAND",KeyCode.LeftCommand },
        {"CMD",KeyCode.LeftCommand },

        {"ALT",KeyCode.LeftAlt },
        {"ALTERNATE",KeyCode.LeftAlt },

        {"SHIFT",KeyCode.LeftShift },
        {"SHFT",KeyCode.LeftShift },

        {"BACKSPACE",KeyCode.Backspace },
        {"BACK",KeyCode.Backspace },
        {"BSPACE",KeyCode.Backspace },

        {"DELETE",KeyCode.Delete },
        {"DEL",KeyCode.Delete },

        {"TAB",KeyCode.Tab },
        {"TABULATION",KeyCode.Tab },

        {"CLEAR",KeyCode.Clear },

        {"ENTER",KeyCode.Return },
        {"RETURN",KeyCode.Return },

        {"PAUSE",KeyCode.Pause },

        {" ",KeyCode.Space },
        {"SPACE",KeyCode.Space },

        {"PERIOD",KeyCode.Period },
        {".",KeyCode.Period },

        {"+",KeyCode.Plus },
        {"PLUS",KeyCode.Plus },

        {"-",KeyCode.Minus },
        {"MINUS",KeyCode.Minus },

        {"=",KeyCode.Equals },
        {"EQUAL",KeyCode.Equals },
        {"EQUALS",KeyCode.Equals },

        {"UP",KeyCode.UpArrow },
        {"UPARROW",KeyCode.UpArrow },

        {"DOWN",KeyCode.DownArrow },
        {"DOWNARROW",KeyCode.DownArrow },

        {"LEFT",KeyCode.LeftArrow },
        {"LEFTARROW",KeyCode.LeftArrow },

        {"RIGHT",KeyCode.RightArrow },
        {"RIGHTARROW",KeyCode.RightArrow },

        {"INSERT",KeyCode.Insert },
        {"INS",KeyCode.Insert },

        { "HOME",KeyCode.Home },
        { "END",KeyCode.End },

        { "PAGEUP",KeyCode.PageUp },
        { "PGUP",KeyCode.PageUp },

        { "PAGEDOWN",KeyCode.PageDown },
        { "PGDOWN",KeyCode.PageDown },
        { "PGDN",KeyCode.PageDown },

        { "F1",KeyCode.F1 },
        { "F2",KeyCode.F2 },
        { "F3",KeyCode.F3 },
        { "F4",KeyCode.F4 },
        { "F5",KeyCode.F5 },
        { "F6",KeyCode.F6 },
        { "F7",KeyCode.F7 },
        { "F8",KeyCode.F8 },
        { "F9",KeyCode.F9 },
        { "F10",KeyCode.F10 },
        { "F11",KeyCode.F11 },
        { "F12",KeyCode.F12 },
        { "F13",KeyCode.F13 },
        { "F14",KeyCode.F14 },
        { "F15",KeyCode.F15 },
        {"0",KeyCode.Alpha0 },
        {"1",KeyCode.Alpha1 },
        {"2",KeyCode.Alpha2 },
        {"3",KeyCode.Alpha3 },
        {"4",KeyCode.Alpha4 },
        {"5",KeyCode.Alpha5 },
        {"6",KeyCode.Alpha6 },
        {"7",KeyCode.Alpha7 },
        {"8",KeyCode.Alpha8 },
        {"9",KeyCode.Alpha9 },

        {"!",KeyCode.Exclaim },
        {"EXCLAIM",KeyCode.Exclaim },
        {"EXCLAMATION",KeyCode.Exclaim },
        {"EXCLAMATIONMARK",KeyCode.Exclaim },

        {"\"",KeyCode.DoubleQuote },
        {"DOUBLEQUOTE",KeyCode.DoubleQuote },

        {"#",KeyCode.Hash },
        {"HASH",KeyCode.Hash },

        {"$",KeyCode.Dollar },
        {"DOLLAR",KeyCode.Dollar },

        {"AMPERSAND",KeyCode.Ampersand },
        {"&",KeyCode.Ampersand },

        {"'",KeyCode.Quote },
        {"QUOTE",KeyCode.Quote },

        {"(",KeyCode.LeftParen },
        {"LEFTPAREN",KeyCode.LeftParen },
        {"LEFTPARENTHESIS",KeyCode.LeftParen },

        {")",KeyCode.RightParen },
        {"RIGHTPAREN",KeyCode.RightParen },
        {"RIGHTPARENTHESIS",KeyCode.RightParen },

        {"*",KeyCode.Asterisk },
        {"ASTERISK",KeyCode.Asterisk },

        {"/",KeyCode.Slash },
        {"SLASH",KeyCode.Slash },

        {":",KeyCode.Colon },
        {"COLON",KeyCode.Colon },

        {";",KeyCode.Semicolon },
        {"SEMICOLON",KeyCode.Semicolon },

        {"<",KeyCode.Less },
        {"LESS",KeyCode.Less },
        {"LESSTHAN",KeyCode.Less },

        {">",KeyCode.Greater },
        {"GREATER",KeyCode.Greater },
        {"GREATERTHAN",KeyCode.Greater },

        {"?",KeyCode.Question },
        {"QUESTION",KeyCode.Question },

        {"@",KeyCode.At },
        {"AT",KeyCode.At },

        {"[",KeyCode.LeftBracket },
        {"LEFTBRACKET",KeyCode.LeftBracket },

        {"\\",KeyCode.Backslash },
        {"BACKSLASH",KeyCode.Backslash },

        {"]",KeyCode.RightBracket },
        {"RIGHTBRACKET",KeyCode.RightBracket },

        {"^",KeyCode.Caret },
        {"CARET",KeyCode.Caret },

        {"_",KeyCode.Underscore },
        {"UNDERSCORE",KeyCode.Underscore },

        {"`",KeyCode.BackQuote },
        {"BACKQUOTE",KeyCode.BackQuote },
        {"A",KeyCode.A },
        {"B",KeyCode.B },
        {"C",KeyCode.C },
        {"D",KeyCode.D },
        {"E",KeyCode.E },
        {"F",KeyCode.F },
        {"G",KeyCode.G },
        {"H",KeyCode.H },
        {"I",KeyCode.I },
        {"J",KeyCode.J },
        {"K",KeyCode.K },
        {"L",KeyCode.L },
        {"M",KeyCode.M },
        {"N",KeyCode.N },
        {"O",KeyCode.O },
        {"P",KeyCode.P },
        {"Q",KeyCode.Q },
        {"R",KeyCode.R },
        {"S",KeyCode.S },
        {"T",KeyCode.T },
        {"U",KeyCode.U },
        {"V",KeyCode.V },
        {"W",KeyCode.W },
        {"X",KeyCode.X },
        {"Y",KeyCode.Y },
        {"Z",KeyCode.Z },
        {"NUMLOCK",KeyCode.Numlock },

        {"CAPSLOCK",KeyCode.CapsLock },
        {"CAPS",KeyCode.CapsLock },

        {"SCROLLLOCK",KeyCode.ScrollLock },
        {"APPLE",KeyCode.LeftApple },
        {"WINDOWS",KeyCode.LeftWindows },
        {"HELP",KeyCode.Help },
        {"PRINT",KeyCode.Print },
        {"SYSREQ",KeyCode.SysReq },
        {"BREAK",KeyCode.Break },
        {"MENU",KeyCode.Menu },
        {"MOUSE0",KeyCode.Mouse0 },
        {"MOUSE1",KeyCode.Mouse1 },
        {"MOUSE2",KeyCode.Mouse2 },
        {"MOUSE3",KeyCode.Mouse3 },
        {"MOUSE4",KeyCode.Mouse4 },
        {"MOUSE5",KeyCode.Mouse5 },
        {"MOUSE6",KeyCode.Mouse6 },
            {"COMMA",KeyCode.Comma },
            {",",KeyCode.Comma },


        };

        public static readonly Dictionary<KeyCode, string> KeycodeStringFormat
           = new Dictionary<KeyCode, string>()
       {
        {KeyCode.LeftControl,"CTRL" },
       // {KeyCode.LeftCommand,"CMD" },
        {KeyCode.LeftAlt ,"ALT"},
        {KeyCode.LeftShift,"SHIFT" },
        {KeyCode.Backspace,"BACKSPACE" },
        {KeyCode.Delete,"DELETE" },
        {KeyCode.Tab,"TAB"},
        {KeyCode.Clear,"CLEAR"},
        {KeyCode.Return,"ENTER"},
        {KeyCode.Pause,"PAUSE"},
        {KeyCode.Space,"SPACE"},
        {KeyCode.Period,"."},
        {KeyCode.Plus,"'+'"},
        {KeyCode.Minus,"-"},
        {KeyCode.Equals,"="},
        {KeyCode.UpArrow,"UP"},
        {KeyCode.DownArrow,"DOWN" },
        {KeyCode.LeftArrow,"LEFT" },
        {KeyCode.RightArrow,"RIGHT" },
        {KeyCode.Insert,"INSERT" },
        {KeyCode.Home, "HOME" },
        {KeyCode.End, "END" },
        {KeyCode.PageUp, "PAGE UP" },
        {KeyCode.PageDown, "PAGE DOWN" },
        {KeyCode.F1, "F1" },
        {KeyCode.F2, "F2" },
        {KeyCode.F3, "F3" },
        {KeyCode.F4,"F4" },
        {KeyCode.F5, "F5" },
        {KeyCode.F6, "F6" },
        {KeyCode.F7, "F7" },
        {KeyCode.F8, "F8" },
        {KeyCode.F9,"F9" },
        {KeyCode.F10, "F10" },
        {KeyCode.F11, "F11" },
        {KeyCode.F12, "F12" },
        {KeyCode.F13, "F13" },
        {KeyCode.F14, "F14" },
        {KeyCode.F15, "F15" },
        {KeyCode.Alpha0,"0" },
        {KeyCode.Alpha1,"1" },
        {KeyCode.Alpha2,"2" },
        {KeyCode.Alpha3,"3" },
        {KeyCode.Alpha4,"4" },
        {KeyCode.Alpha5,"5" },
        {KeyCode.Alpha6,"6" },
        {KeyCode.Alpha7,"7" },
        {KeyCode.Alpha8,"8" },
        {KeyCode.Alpha9,"9" },
        {KeyCode.Exclaim,"!" },
        {KeyCode.DoubleQuote,"\"" },
        {KeyCode.Hash,"#" },
        {KeyCode.Dollar,"$" },
        {KeyCode.Ampersand,"&" },
        {KeyCode.Quote,"'" },
        {KeyCode.LeftParen,"(" },
        {KeyCode.RightParen,")" },
        {KeyCode.Asterisk,"*" },
        {KeyCode.Slash,"/" },
        {KeyCode.Colon,":" },
        {KeyCode.Semicolon,";" },
        {KeyCode.Less,"<" },
        {KeyCode.Greater,">" },
        {KeyCode.Question,"?" },
        {KeyCode.At,"@" },
        {KeyCode.LeftBracket,"[" },
        {KeyCode.Backslash,"\\" },
        {KeyCode.RightBracket,"]" },
        {KeyCode.Caret,"^" },
        {KeyCode.Underscore,"_" },
        {KeyCode.BackQuote,"`" },
        {KeyCode.A,"A" },
        {KeyCode.B,"B" },
        {KeyCode.C,"C" },
        {KeyCode.D,"D" },
        {KeyCode.E,"E" },
        {KeyCode.F,"F" },
        {KeyCode.G,"G" },
        {KeyCode.H,"H" },
        {KeyCode.I,"I" },
        {KeyCode.J,"J" },
        {KeyCode.K,"K" },
        {KeyCode.L,"L" },
        {KeyCode.M,"M" },
        {KeyCode.N,"N" },
        {KeyCode.O,"O" },
        {KeyCode.P,"P" },
        {KeyCode.Q,"Q" },
        {KeyCode.R,"R" },
        {KeyCode.S,"S" },
        {KeyCode.T,"T" },
        {KeyCode.U,"U" },
        {KeyCode.V,"V" },
        {KeyCode.W,"W" },
        {KeyCode.X,"X" },
        {KeyCode.Y,"Y" },
        {KeyCode.Z,"Z" },
        {KeyCode.Numlock,"NUMLOCK" },
        {KeyCode.CapsLock,"CAPSLOCK" },
        {KeyCode.ScrollLock,"SCROLLLOCK" },
        {KeyCode.LeftApple,"APPLE" },
        //{KeyCode.LeftWindows,"WINDOWS" },
        {KeyCode.Help,"HELP" },
        {KeyCode.Print,"PRINT" },
        {KeyCode.SysReq,"SYSREQ" },
        {KeyCode.Break,"BREAK" },
        {KeyCode.Menu,"MENU" },
        {KeyCode.Mouse0,"MOUSE0" },
        {KeyCode.Mouse1,"MOUSE1" },
        {KeyCode.Mouse2,"MOUSE2" },
        {KeyCode.Mouse3,"MOUSE3" },
        {KeyCode.Mouse4,"MOUSE4" },
        {KeyCode.Mouse5,"MOUSE5" },
        {KeyCode.Mouse6,"MOUSE6" },
           {KeyCode.Comma,"," },

       };

        public static readonly Dictionary<KeyCode, KeyCode> KeyCodeRightToLeftKeyCode
         = new Dictionary<KeyCode, KeyCode>()
     {
     { KeyCode.Keypad0,KeyCode.Alpha0},
     { KeyCode.Keypad1,KeyCode.Alpha1},
     { KeyCode.Keypad2,KeyCode.Alpha2},
     { KeyCode.Keypad3,KeyCode.Alpha3},
     { KeyCode.Keypad4,KeyCode.Alpha4},
     { KeyCode.Keypad5,KeyCode.Alpha5},
     { KeyCode.Keypad6,KeyCode.Alpha6},
     { KeyCode.Keypad7,KeyCode.Alpha7},
     { KeyCode.Keypad8,KeyCode.Alpha8},
     { KeyCode.Keypad9,KeyCode.Alpha9},
     { KeyCode.KeypadPeriod,KeyCode.Period},
     { KeyCode.KeypadDivide,KeyCode.Slash},
     { KeyCode.KeypadMultiply,KeyCode.Asterisk},
     { KeyCode.KeypadMinus,KeyCode.Minus},
     { KeyCode.KeypadPlus,KeyCode.Plus},
     { KeyCode.KeypadEnter,KeyCode.Return},
     { KeyCode.KeypadEquals,KeyCode.Equals},
     { KeyCode.RightShift,KeyCode.LeftShift},
     { KeyCode.RightAlt,KeyCode.LeftAlt},
     { KeyCode.RightControl,KeyCode.LeftControl},
   //  { KeyCode.RightCommand,KeyCode.LeftCommand},
     { KeyCode.RightApple,KeyCode.LeftApple},
     { KeyCode.AltGr,KeyCode.LeftAlt},
     };

        #endregion

        private List<KeyCode> keysInOrder=new List<KeyCode>();
        private string formattedKeys;

        public bool IsSceneShortcut => keysInOrder.Count == 1 &&
                                       !SpecialKeyCodes.Contains(keysInOrder[1]);

        public IEnumerable<KeyCode> KeysInOrder => keysInOrder;

        public string FormattedKeys => formattedKeys;

        public KeyCombination(int keyCapacity)
        {
            keysInOrder = new List<KeyCode>(keyCapacity);
        }

        /// <summary>
        /// Create a key combination by specifying every key and special key that need to be hit
        /// See documentation for aliases that can be used.
        /// </summary>
        /// <param name="keys"></param>
        public KeyCombination(params string[] keys)
        {
            SetKeysInOrderFromSymbols(keys);
        }

        private void SetKeysInOrderFromSymbols(string[] keys)
        {
            List<KeyCode> foundCodes = new List<KeyCode>(keys.Length);

            foreach (var key in keys)
            {
                var formattedKey = key.ToUpper().Replace(" ", "");
                if (KeycodeStringAliases.ContainsKey(formattedKey))
                {
                    foundCodes.Add(KeycodeStringAliases[formattedKey]);
                }
                else
                {
                    //TODO add to localization
                    Debug.LogError("MonKey: Unrecognized key '" + key +
                                   "', Check the documentation if you think " +
                                   "the key should be recognized to know " +
                                   "the exact naming for each key");
                    foundCodes.Clear();
                    break;
                }
            }

            keysInOrder = foundCodes.OrderBy(KeyCodeOrderer()).ToList();
            PrepareFormattedKeys();
        }


        /// <summary>
        /// Creates a new Key Combination based on the symbols used by Menu Items
        /// (see https://docs.unity3d.com/ScriptReference/MenuItem.html)
        /// </summary>
        /// <param name="combinedKeys"></param>
        public KeyCombination(string combinedKeys)
        {
            combinedKeys = combinedKeys.Replace(KeySymbol.ToString(), "").Replace(" ", "");
            List<string> brokenDownString = new List<string>();

            foreach (var alias in ModifierKeysAliases)
            {
                combinedKeys = combinedKeys.Replace(alias.Key, alias.Value);
                combinedKeys = CombineAddNewSymbol(alias.Value,
                    combinedKeys, brokenDownString);
            }

            foreach (var specialKey in SpecialKeys)
            {
                combinedKeys = CombineAddNewSymbol(specialKey,
                    combinedKeys, brokenDownString);
            }

            foreach (char c in combinedKeys)
            {
                brokenDownString.Add(c.ToString());
            }

            SetKeysInOrderFromSymbols(brokenDownString.ToArray());
        }

        public KeyCombination(params KeyCode[] keyCodes)
        {
            keysInOrder = new List<KeyCode>(keyCodes.Length);
            AddKeyCodes(true, keyCodes);
        }

        public void ClearKeys()
        {
            keysInOrder.Clear();
            formattedKeys = "";
        }

        public void AddKeyCodes(bool prepareFormat = true, params KeyCode[] codes)
        {
            List<KeyCode> finalCodes = new List<KeyCode>();
            foreach (var code in codes)
            {
                if (KeyCodeRightToLeftKeyCode.ContainsKey(code))
                    finalCodes.Add(KeyCodeRightToLeftKeyCode[code]);
                else
                {
                    finalCodes.Add(code);
                }
            }
            keysInOrder.AddRange(finalCodes.OrderBy(KeyCodeOrderer()));
            if (prepareFormat)
                PrepareFormattedKeys();
        }

        private static string CombineAddNewSymbol(string symbol, string combinedKeys, List<string> brokenDownString)
        {
            if (combinedKeys.Contains(symbol))
            {
                brokenDownString.Add(symbol);
                combinedKeys = combinedKeys.Replace(symbol, "");
            }

            return combinedKeys;
        }

        private static Func<KeyCode, int> KeyCodeOrderer()
        {
            return _ =>
            {
                if (_ == KeyCode.LeftControl)
                    return 0;
                if (_ == KeyCode.LeftAlt)
                    return 1;
                if (_ == KeyCode.LeftShift)
                    return 2;
                return (int)(3 + _);
            };
        }

        private void PrepareFormattedKeys()
        {
            formattedKeys = "";

            foreach (var keyCode in keysInOrder)
            {
                if (formattedKeys.Length > 0)
                    formattedKeys += SeparatorSymbol;
                formattedKeys += AdaptToMac(KeycodeStringFormat[keyCode]);
            }
        }

        private string AdaptToMac(string value)
        {
            if (Application.platform == RuntimePlatform.OSXEditor 
                || Application.platform == RuntimePlatform.OSXPlayer)
            {
                if (value == "CTRL")
                    return "CMD";
            }

            return value;
        }

        public bool IsIdentical(KeyCombination comb)
        {
            for (int i = 0; i < keysInOrder.Count; i++)
            {
                if (keysInOrder[i] != comb.keysInOrder[i])
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsIdenticalNonOrdered(KeyCombination comb)
        {
            if (comb.keysInOrder.Count != keysInOrder.Count)
                return false;

            for (int i = 0; i < keysInOrder.Count; i++)
            {
                if (!comb.keysInOrder.Contains(keysInOrder[i]))
                {
                    return false;
                }
            }


            return true;
        }


        public bool ContainsKey(KeyCode code, bool unformatedCode = false)
        {
            if (unformatedCode && KeyCodeRightToLeftKeyCode.ContainsKey(code))
                code = KeyCodeRightToLeftKeyCode[code];
            return keysInOrder.Contains(code);
        }

        public bool IsContainedIn(KeyCombination combination)
        {
            bool bothControl = (combination.ContainsKey(KeyCode.LeftControl) && ContainsKey(KeyCode.LeftControl) ||
                                !combination.ContainsKey(KeyCode.LeftControl) && !ContainsKey(KeyCode.LeftControl));

            bool bothAlt = (combination.ContainsKey(KeyCode.LeftAlt) && ContainsKey(KeyCode.LeftAlt) ||
                                !combination.ContainsKey(KeyCode.LeftAlt) && !ContainsKey(KeyCode.LeftAlt));

            bool bothShift = (combination.ContainsKey(KeyCode.LeftShift) && ContainsKey(KeyCode.LeftShift) ||
                            !combination.ContainsKey(KeyCode.LeftShift) && !ContainsKey(KeyCode.LeftShift));

            if (!bothShift || !bothAlt || !bothControl)
                return false;

            foreach (KeyCode code in KeysInOrder)
            {
                if (!combination.ContainsKey(code))
                    return false;
            }

            return true;
        }

        public void RemoveKey(KeyCode code, bool unformatedCode)
        {
            if (unformatedCode && KeyCodeRightToLeftKeyCode.ContainsKey(code))
                code = KeyCodeRightToLeftKeyCode[code];
            if (keysInOrder.Contains(code))
                keysInOrder.Remove(code);
        }
    }

}


