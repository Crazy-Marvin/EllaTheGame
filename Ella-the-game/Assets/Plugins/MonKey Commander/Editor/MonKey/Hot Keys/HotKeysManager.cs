using System.Collections.Generic;
using MonKey.Editor.Console;
using MonKey.Internal;
using MonKey.Settings.Internal;
using UnityEditor;
using UnityEngine;

namespace MonKey.Editor.Internal
{

    public static class HotKeysManager
    {
        internal static bool DebugKeyPressed = true;

        internal static Dictionary<HotKeyInfo, List<HotKeyInfo>> OverlappingHotKeys =
            new Dictionary<HotKeyInfo, List<HotKeyInfo>>();

        internal static Dictionary<KeyCombination, HotKeyInfo> HotKeyInfos
            = new Dictionary<KeyCombination, HotKeyInfo>();

        //   private static KeyboardHook keyboardHook;

        internal static void Reset()
        {
            OverlappingHotKeys.Clear();
            HotKeyInfos.Clear();
        }

        public static void RegisterCommandHotKey(CommandInfo info)
        {
            if (!info.HasHotKeys)
                return;

            foreach (var keyComb in HotKeyInfos.Keys)
            {
                if (keyComb.IsIdentical(info.HotKey))
                {
                    info.HotKey = keyComb;
                    if (!HotKeyInfos[keyComb].AssociatedCommands.Contains(info))
                        HotKeyInfos[keyComb].AssociatedCommands.Add(info);
                    info.HotKeyInfo = HotKeyInfos[keyComb];
                    return;
                }
            }


            HotKeyInfo hkInfo = new HotKeyInfo(info.HotKey, info);
            info.HotKeyInfo = hkInfo;

            foreach (var comb in HotKeyInfos.Keys)
            {
                if (info.HotKey.IsContainedIn(comb))
                {
                    if (!OverlappingHotKeys.ContainsKey(hkInfo))
                        OverlappingHotKeys.Add(hkInfo, new List<HotKeyInfo>());
                    OverlappingHotKeys[hkInfo].Add(HotKeyInfos[comb]);
                }
            }

            HotKeyInfos.Add(info.HotKey, hkInfo);
        }

        /*
          
         // Custom Hotkeys is disabled until a proper reliable framework is found
         // (gainput is not reliable, and Unity events are a mess)
        
        private static bool isCurrentCombinationWithShift;
        private static readonly KeyCombination CurrentCombination=new KeyCombination(3);

        private static List<KeyCombination> infoCandidates = new List<KeyCombination>();

        public static Vector2 MousePosition;*/

        [InitializeOnLoadMethod]
        static void EditorInit()
        {
            System.Reflection.FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            EditorApplication.CallbackFunction value =
                (EditorApplication.CallbackFunction)info.GetValue(null);

            value -= EditorGlobalKeyPress;
            value += EditorGlobalKeyPress;

            info.SetValue(null, value);

            // keyboardHook=new KeyboardHook();

            EditorApplication.modifierKeysChanged -= CheckIfShift;
            EditorApplication.modifierKeysChanged += CheckIfShift;

            FillReservedUnityHotKeys();

        }

        private static void CheckIfShift()
        {
            MightBeShift = !MightBeShift;
        }

        public static bool MightBeShift { get; private set; }

        private static void FillReservedUnityHotKeys()
        {

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Pan",
                HotKey = (new KeyCombination("Q"))

            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Move",
                HotKey = (new KeyCombination("W"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Rotate",
                HotKey = (new KeyCombination("E"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Scale",
                HotKey = (new KeyCombination("R"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Rect Tool",
                HotKey = (new KeyCombination("T"))
            }); RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Pivot Mode toggle",
                HotKey = (new KeyCombination("Z"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Pivot Rotation Toggle",
                HotKey = (new KeyCombination("X"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Vertex Snap",
                HotKey = (new KeyCombination("V"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity New empty game object",
                HotKey = (new KeyCombination("CTRL", "SHIFT", "N"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity New empty child to selected game object",
                HotKey = (new KeyCombination("ALT", "SHIFT", "N"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Move to view",
                HotKey = (new KeyCombination("CTRL", "ALT", "F"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Align with view",
                HotKey = (new KeyCombination("Ctrl", "Shift", "F"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Locks the scene view camera to the selected GameObject",
                HotKey = (new KeyCombination("Shift", "F"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Scene",
                HotKey = (new KeyCombination("CTRL", "1"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Game",
                HotKey = (new KeyCombination("CTRL", "2"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Inspector",
                HotKey = (new KeyCombination("CTRL", "3"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Hierarchy",
                HotKey = (new KeyCombination("CTRL", "4"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Project",
                HotKey = (new KeyCombination("CTRL", "5"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Animation",
                HotKey = (new KeyCombination("CTRL", "6"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Profiler",
                HotKey = (new KeyCombination("CTRL", "7"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Asset store",
                HotKey = (new KeyCombination("CTRL", "8"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity 	Version Control",
                HotKey = (new KeyCombination("CTRL", "0"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Undo",
                HotKey = (new KeyCombination("CTRL", "Z"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Redo",
                HotKey = (new KeyCombination("CTRL", "Y"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Redo",
                HotKey = (new KeyCombination("CMD", "SHIFT", "Z"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Cut",
                HotKey = (new KeyCombination("CTRL", "X"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Copy",
                HotKey = (new KeyCombination("CTRL", "C"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Paste",
                HotKey = (new KeyCombination("CTRL", "V"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Duplicate",
                HotKey = (new KeyCombination("CTRL", "D"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Delete",
                HotKey = (new KeyCombination("SHIFT", "DELETE"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Frame",
                HotKey = (new KeyCombination("F"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Find",
                HotKey = (new KeyCombination("CTRL", "F"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Select All",
                HotKey = (new KeyCombination("CTRL", "A"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Play",
                HotKey = (new KeyCombination("CTRL", "P"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Pause",
                HotKey = (new KeyCombination("CTRL", "SHIFT", "P"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Step",
                HotKey = (new KeyCombination("CTRL", "ALT", "P"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Load Selection 1",
                HotKey = (new KeyCombination("CTRL", "SHIFT", "1"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Load Selection 2",
                HotKey = (new KeyCombination("CTRL", "SHIFT", "2"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Load Selection 3",
                HotKey = (new KeyCombination("CTRL", "SHIFT", "3"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Load Selection 4",
                HotKey = (new KeyCombination("CTRL", "SHIFT", "4"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Load Selection 5",
                HotKey = (new KeyCombination("CTRL", "SHIFT", "5"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Load Selection 6",
                HotKey = (new KeyCombination("CTRL", "SHIFT", "6"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Load Selection 7",
                HotKey = (new KeyCombination("CTRL", "SHIFT", "7"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Load Selection 8",
                HotKey = (new KeyCombination("CTRL", "SHIFT", "8"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Load Selection 9",
                HotKey = (new KeyCombination("CTRL", "SHIFT", "9"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Save Selection 1",
                HotKey = (new KeyCombination("CTRL", "ALT", "1"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Save Selection 2",
                HotKey = (new KeyCombination("CTRL", "ALT", "2"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Save Selection 3",
                HotKey = (new KeyCombination("CTRL", "ALT", "3"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Save Selection 4",
                HotKey = (new KeyCombination("CTRL", "ALT", "4"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Save Selection 5",
                HotKey = (new KeyCombination("CTRL", "ALT", "5"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Save Selection 6",
                HotKey = (new KeyCombination("CTRL", "ALT", "6"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Save Selection 7",
                HotKey = (new KeyCombination("CTRL", "ALT", "7"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Save Selection 8",
                HotKey = (new KeyCombination("CTRL", "ALT", "8"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Save Selection 9",
                HotKey = (new KeyCombination("CTRL", "ALT", "9"))
            });
            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Refresh",
                HotKey = (new KeyCombination("CTRL", "R"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity First KeyFrame",
                HotKey = (new KeyCombination("SHIFT", ","))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Key Modified",
                HotKey = (new KeyCombination("SHIFT", "K"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Key Selected",
                HotKey = (new KeyCombination("K"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Last KeyFrame",
                HotKey = (new KeyCombination("Shift", "."))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Next KeyFrame",
                HotKey = (new KeyCombination("Alt", "."))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Play Animation",
                HotKey = (new KeyCombination("Space"))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Previous Frame",
                HotKey = (new KeyCombination(","))
            });

            RegisterCommandHotKey(new CommandInfo()
            {
                CommandName = "Unity Previous KeyFrame",
                HotKey = (new KeyCombination("ALT", ","))
            });
        }

        internal static KeyCombination CustomMonkeyConsoleCall;

        static void EditorGlobalKeyPress()
        {
            CheckCustomHotKey();
        }

        internal static void CheckCustomHotKey()
        {
            if (!MonKeyInternalSettings.Instance)
                return;

            if (MonKeyInternalSettings.Instance.UseCustomConsoleKey)
            {
                if (Event.current.type == EventType.KeyDown)
                {
                    if(CustomMonkeyConsoleCall==null)
                        CustomMonkeyConsoleCall = new KeyCombination(
                            MonKeyInternalSettings.Instance.MonkeyConsoleOverrideHotKey);

                    if (CustomMonkeyConsoleCall == null)
                        return;

                    foreach (var key in CustomMonkeyConsoleCall.KeysInOrder)
                    {
                        if (Event.current.keyCode == key)
                        {
                            Event.current.Use();                           
                            CommandConsoleWindow.TogglePanelCustom();
                        }
                    }
                }
            }
        }

        /*
                     if (Event.current.type == EventType.KeyDown)
                     {
                         if (Event.current.shift )
                         {
                             if (!isCurrentCombinationWithShift)
                             {
                                 CurrentCombination.AddKeyCodes(false, KeyCode.LeftShift);
                                 isCurrentCombinationWithShift = true;
                                 AddNewCodeToCommandCandidate(KeyCode.LeftShift);
                             }

                         }else if (isCurrentCombinationWithShift)
                         {
                             isCurrentCombinationWithShift = false;
                             CurrentCombination.RemoveKey(KeyCode.LeftShift,false);
                             RemoveCodeFromCommandCandidate(KeyCode.LeftShift);
                         }


                         if (Event.current.keyCode!=KeyCode.None &&
                             !CurrentCombination.ContainsKey(Event.current.keyCode, true))
                         {
                             CurrentCombination.AddKeyCodes(false, Event.current.keyCode);
                             AddNewCodeToCommandCandidate(Event.current.keyCode);

                         }

                         Event.current.Use();
                     }
                     else if (Event.current.type == EventType.KeyUp)
                     {
                         if (CurrentCombination.ContainsKey(Event.current.keyCode, true))
                         {
                             CurrentCombination.RemoveKey(Event.current.keyCode, true);
                             RemoveCodeFromCommandCandidate(Event.current.keyCode);
                         }
                     }
        }*/

        /* private static void AddNewCodeToCommandCandidate(KeyCode addedCode)
         {
            if (!infoCandidates.Any())
             {
                 infoCandidates = HotKeyInfos.Keys
                     .Where(_ => _.ContainsKey(addedCode, true)).ToList();
             }
             else
             {
                 infoCandidates = infoCandidates
                     .Where(_ => _.ContainsKey(addedCode, true)).ToList();
             }

             if (infoCandidates.Count == 1 && CurrentCombination.IsIdenticalNonOrdered(infoCandidates[0]))
             {
                 ExecuteAllCandidates();
             }
         }


         private static void RemoveCodeFromCommandCandidate(KeyCode removedCode)
         {
             if (infoCandidates.Count > 0)
                 infoCandidates = infoCandidates
                     .Where(_ => !_.ContainsKey(removedCode, true)).ToList();

             if (infoCandidates.Count == 1 
                 && CurrentCombination.IsIdenticalNonOrdered(infoCandidates[0]))
             {
                 ExecuteAllCandidates();
             }
         }

         private static void ExecuteAllCandidates()
         {
             HotKeyInfos[infoCandidates[0]]
                 .AssociatedCommands.ForEach(_ => _.Action.Invoke());
             infoCandidates.Clear();
             CurrentCombination.ClearKeys();
             isCurrentCombinationWithShift = false;
         }
         */
    }
}