using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

namespace EasyMobile.Editor
{
    public static class GlobalDefineManager
    {
        private const string CSharpRsp = "Assets/smcs.rsp";
        private const string CSharpEditorRsp = "Assets/gmcs.rsp";

        #region Common API

        /// <summary>
        /// Defines the symbol globally for all build target groups if they don't exist.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        public static void AddDefine(string symbol)
        {
#if USE_RSP
            RSP_AddDefine(symbol);
#else
            SDS_AddDefineOnAllPlatforms(symbol);
#endif
        }

        /// <summary>
        /// Defines the symbols globally for all build target groups if they don't exist.
        /// </summary>
        /// <param name="symbols">Symbols.</param>
        public static void AddDefines(string[] symbols)
        {
#if USE_RSP
            RSP_AddDefines(symbols);
#else
            SDS_AddDefinesOnAllPlatforms(symbols);
#endif
        }

        /// <summary>
        /// Undefines the symbol on all build target groups where they exist.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        public static void RemoveDefine(string symbol)
        {
#if USE_RSP
            RSP_RemoveDefine(symbol);
#else
            SDS_RemoveDefineOnAllPlatforms(symbol);
#endif
        }

        /// <summary>
        /// Undefines the symbols on all build target groups where they exist.
        /// </summary>
        /// <param name="symbols">Symbols.</param>
        public static void RemoveDefines(string[] symbols)
        {
#if USE_RSP
            RSP_RemoveDefines(symbols);
#else
            SDS_RemoveDefinesOnAllPlatforms(symbols);
#endif
        }

        #endregion

        #region Global define using Scripting Define Symbols (SDS) in Player Settings

        public static bool SDS_IsDefined(string symbol, BuildTargetGroup platform)
        {
            string symbolStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
            List<string> symbols = new List<string>(symbolStr.Split(';'));

            return symbols.Contains(symbol);
        }

        /// <summary>
        /// Adds the scripting define symbol to all platforms where it doesn't exist.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        public static void SDS_AddDefineOnAllPlatforms(string symbol)
        {
            foreach (BuildTargetGroup target in EM_EditorUtil.GetWorkingBuildTargetGroups())
            {
                SDS_AddDefine(symbol, target);
            }
        }

        /// <summary>
        /// Adds the scripting define symbols in the given array to all platforms where they don't exist. 
        /// </summary>
        /// <param name="symbols">Symbols.</param>
        public static void SDS_AddDefinesOnAllPlatforms(string[] symbols)
        {
            foreach (BuildTargetGroup target in EM_EditorUtil.GetWorkingBuildTargetGroups())
            {
                SDS_AddDefines(symbols, target);
            }
        }

        /// <summary>
        /// Adds the scripting define symbols in given array to the target platforms if they don't exist.
        /// </summary>
        /// <param name="symbols">Symbols.</param>
        /// <param name="platform">Platform.</param>
        public static void SDS_AddDefines(string[] symbols, BuildTargetGroup platform)
        {
            string symbolStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
            List<string> currentSymbols = new List<string>(symbolStr.Split(';'));
            int added = 0;

            foreach (string symbol in symbols)
            {
                if (!currentSymbols.Contains(symbol))
                {
                    currentSymbols.Add(symbol);
                    added++;
                }
            }

            if (added > 0)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < currentSymbols.Count; i++)
                {
                    sb.Append(currentSymbols[i]);
                    if (i < currentSymbols.Count - 1)
                        sb.Append(";");
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, sb.ToString());
            }
        }

        /// <summary>
        /// Adds the scripting define symbols on the platform if it doesn't exist.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        /// <param name="Platform">Platform.</param>
        public static void SDS_AddDefine(string symbol, BuildTargetGroup platform)
        {
            string symbolStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
            List<string> symbols = new List<string>(symbolStr.Split(';'));

            if (!symbols.Contains(symbol))
            {
                symbols.Add(symbol);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < symbols.Count; i++)
                {
                    sb.Append(symbols[i]);
                    if (i < symbols.Count - 1)
                        sb.Append(";");
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, sb.ToString());
            }
        }

        /// <summary>
        /// Removes the scripting define symbols in the given array on all platforms where they exist.
        /// </summary>
        /// <param name="symbols">Symbols.</param>
        public static void SDS_RemoveDefinesOnAllPlatforms(string[] symbols)
        {
            foreach (BuildTargetGroup target in EM_EditorUtil.GetWorkingBuildTargetGroups())
            {
                SDS_RemoveDefines(symbols, target);
            }
        }

        /// <summary>
        /// Removes the scripting define symbol on all platforms where it exists.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        public static void SDS_RemoveDefineOnAllPlatforms(string symbol)
        {
            foreach (BuildTargetGroup target in EM_EditorUtil.GetWorkingBuildTargetGroups())
            {
                SDS_RemoveDefine(symbol, target);
            }
        }

        /// <summary>
        /// Removes the scripting define symbols in the given array on the target platform if they exists.
        /// </summary>
        /// <param name="symbols">Symbols.</param>
        /// <param name="platform">Platform.</param>
        public static void SDS_RemoveDefines(string[] symbols, BuildTargetGroup platform)
        {
            string symbolStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
            List<string> currentSymbols = new List<string>(symbolStr.Split(';'));
            int removed = 0;

            foreach (string symbol in symbols)
            {
                if (currentSymbols.Contains(symbol))
                {
                    currentSymbols.Remove(symbol);
                    removed++;
                }
            }

            if (removed > 0)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < currentSymbols.Count; i++)
                {
                    sb.Append(currentSymbols[i]);
                    if (i < currentSymbols.Count - 1)
                        sb.Append(";");
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, sb.ToString());
            }
        }

        /// <summary>
        /// Removes the scripting define symbol on the platform if it exists.
        /// </summary>
        /// <param name="symbol">Symbol.</param>
        /// <param name="Platform">Platform.</param>
        public static void SDS_RemoveDefine(string symbol, BuildTargetGroup platform)
        {
            string symbolStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
            List<string> symbols = new List<string>(symbolStr.Split(';'));

            if (symbols.Contains(symbol))
            {
                symbols.Remove(symbol);

                StringBuilder settings = new StringBuilder();

                for (int i = 0; i < symbols.Count; i++)
                {
                    settings.Append(symbols[i]);
                    if (i < symbols.Count - 1)
                        settings.Append(";");
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, settings.ToString());
            }
        }

        #endregion

        #region Global define using .rsp files

#if USE_RSP
        /// <summary>
        /// Determines if the symbol is defined globally.
        /// </summary>
        /// <returns><c>true</c> if is defined; otherwise, <c>false</c>.</returns>
        /// <param name="def">The symbol to check.</param>
        public static bool RSP_IsDefined(string symbol)
        {
            List<string> defines = RSP_GetCurrentDefines();
            return defines.Contains(symbol);
        }

        /// <summary>
        /// Adds the specified global define.
        /// </summary>
        /// <param name="def">Def.</param>
        public static void RSP_AddDefine(string symbol)
        {
            List<string> defines = RSP_GetCurrentDefines();

            if (!defines.Contains(symbol))
            {
                defines.Add(symbol);
                RSP_WriteDefines(defines);
            }
        }

        /// <summary>
        /// Adds the global defines in the given array.
        /// </summary>
        /// <param name="defs">Defs.</param>
        public static void RSP_AddDefines(string[] symbols)
        {
            List<string> defines = RSP_GetCurrentDefines();
            int added = 0;

            foreach (string def in symbols)
            {
                if (!defines.Contains(def))
                {
                    defines.Add(def);
                    added++;
                }
            }

            if (added > 0)
            {
                RSP_WriteDefines(defines);
            }
        }

        /// <summary>
        /// Removes the specified global define.
        /// </summary>
        /// <param name="def">Def.</param>
        public static void RSP_RemoveDefine(string symbol)
        {
            List<string> defines = RSP_GetCurrentDefines();

            if (defines.Contains(symbol))
            {
                defines.Remove(symbol);
                RSP_WriteDefines(defines);
            }
        }

        /// <summary>
        /// Removes the global defines in the given array.
        /// </summary>
        /// <param name="defs">Defs.</param>
        public static void RSP_RemoveDefines(string[] symbols)
        {
            List<string> defines = RSP_GetCurrentDefines();
            int removed = 0;

            foreach (string def in symbols)
            {
                if (defines.Contains(def))
                {
                    defines.Remove(def);
                    removed++;
                }
            }

            if (removed > 0)
            {
                RSP_WriteDefines(defines);
            }
        }

        /// <summary>
        /// Get a list of current global defines.
        /// </summary>
        /// <returns>The current defines.</returns>
        public static List<string> RSP_GetCurrentDefines()
        {
            List<string> defines = new List<string>();
            string[] lines = FileIO.ReadAllLines(CSharpRsp);

            if (lines.Length > 0)
            {
                foreach (string l in lines)
                {
                    if (l.StartsWith("-define:"))
                    {
                        defines.AddRange(l.Replace("-define:", "").Replace(" ", "").Split(';'));
                    }
                }
            }

            return defines;
        }

        /// <summary>
        /// Update the rsp files with new defines.
        /// </summary>
        /// <param name="defines">Defines.</param>
        static void RSP_WriteDefines(List<string> defines)
        {
            if (defines.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("-define:");

                for (int i = 0; i < defines.Count; i++)
                {
                    sb.Append(defines[i]);
                    if (i < defines.Count - 1)
                        sb.Append(";");
                }

                string newDefines = sb.ToString();

                RSP_UpdateDefineLine(CSharpRsp, newDefines);
                RSP_UpdateDefineLine(CSharpEditorRsp, newDefines);
            }
            else
            {
                // No symbol defined, remove the -define line in rsp files
                RSP_RemoveDefineLine(CSharpRsp);
                RSP_RemoveDefineLine(CSharpEditorRsp);
            } 

            // Force recompile for the changes to take effect
            ForceRecompile(); 
        }

        /// <summary>
        /// Updates the define line in rsp file.
        /// - If the file exists and has some content: if there's an existing "-define:" line,
        /// it will be replaced with the new content; otherwise, the new line will be appended to the end of the file.
        /// - If the file doesn't exist or is empty, just write in the new line.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="newDefineLine">New define line.</param>
        static void RSP_UpdateDefineLine(string path, string newDefineLine)
        {
            string[] lines = FileIO.ReadAllLines(path);

            if (lines.Length > 0)   // file has some content
            {
                int defineLineIndex = -1;   
                for (int i = 0; i < lines.Length; i++)
                {
                    string l = lines[i];
                    if (l.StartsWith("-define:"))
                    {
                        defineLineIndex = i;
                        break;
                    }
                }

                if (defineLineIndex >= 0)   // there's an existing -define: line
                {
                    lines[defineLineIndex] = newDefineLine;
                    FileIO.WriteAllLines(path, lines);
                }
                else // there's currently no -define: line, just append a new one at the end of the file
                {
                    List<string> newLines = new List<string>(lines.Length + 1);
                    newLines.AddRange(lines);
                    newLines.Add(newDefineLine);
                    FileIO.WriteAllLines(path, newLines.ToArray());      
                }
            }
            else // file not exist or is empty
            {
                FileIO.WriteAllLines(path, new string[]{ newDefineLine });
            }
        }

        /// <summary>
        /// Removes the "-define:" line from the rsp file. Also deletes the file
        /// if it becomes empty after the removal.
        /// </summary>
        /// <param name="path">Path.</param>
        static void RSP_RemoveDefineLine(string path)
        {
            // Read the current file and remove the line starting with "-define:"
            List<string> lines = new List<string>(FileIO.ReadAllLines(path));

            if (lines.Count > 1) // file has more than 1 line
            {
                int defineLineIndex = -1;   
                for (int i = 0; i < lines.Count; i++)
                {
                    string l = lines[i];
                    if (l.StartsWith("-define:"))
                    {
                        defineLineIndex = i;
                        break;
                    }
                }

                if (defineLineIndex > 0)    // there's an existing "-define:" line
                {
                    lines.RemoveAt(defineLineIndex);
                    FileIO.WriteAllLines(path, lines.ToArray());
                }
            }
            else if (lines.Count == 1 && lines[0].StartsWith("-define:")) // file contains only the -define line
            {
                // Remove the file altogether
                RSP_DeleteRspFile(path);
            }
            else if (lines.Count == 0)
            {
                // Remove the file if it exists and is empty
                RSP_DeleteRspFile(path);
            }
        }

        /// <summary>
        /// Deletes smcs.cs and gmcs.cs files, effectively removes all global defines within these files. Use with care.
        /// </summary>
        static void RSP_DeleteRspFile(string path)
        {
            if (FileIO.FileExists(path))
            {
                AssetDatabase.DeleteAsset(path);
            }
        }

        /// <summary>
        /// Forces a recompilation of both editor code and runtime code by resetting
        /// the script execution order of a random script, which actually makes no changes
        /// because we set back the old value.
        /// All credits go to Darbotron http://answers.unity3d.com/answers/1210416/view.html
        /// </summary>
        static void ForceRecompile()
        {
            MonoScript cMonoScript = MonoImporter.GetAllRuntimeMonoScripts()[0];
            MonoImporter.SetExecutionOrder(cMonoScript, MonoImporter.GetExecutionOrder(cMonoScript));
        }
#endif

        #endregion
    }
}
// Force recompile for the changes to take effect
// Force recompile for the changes to take effect