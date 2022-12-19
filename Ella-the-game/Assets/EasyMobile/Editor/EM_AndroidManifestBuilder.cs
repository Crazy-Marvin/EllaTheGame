using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EasyMobile.Editor
{
    public static class EM_AndroidManifestBuilder
    {
        private static string sLastTimeManifestLibsString = "";
        public static bool sMainManifestGeneratedSucessfully = false;

        public static event Action<string> ManifMergerConsoleErrorsDataReceived;
        public static event Action<string> ManifMergerConsoleDataDataReceived;

        #region Sub-Manifest Paths

        public static string sCustomManifestPath
        {
            get
            {
                return FileIO.ToAbsolutePath("EasyMobile/Editor/Templates/Manifests/Manifest_Custom.xml");
            }
        }

        #endregion

        #region Methods

        internal static List<string> GetAndroidManifestTemplatePathsForModule(Module mod)
        {
            var paths = new List<string>();
            var manager = EM_PluginManager.GetModuleManager(mod);

            if (manager == null)
                return paths;

            // Is this composite module?
            var compManager = manager as CompositeModuleManager;

            if (compManager != null)    // Is a composite module
            {
                foreach (Submodule submod in compManager.SelfSubmodules)
                {
                    if (EM_Settings.IsSubmoduleEnable(submod))
                    {
                        var submodPaths = compManager.AndroidManifestTemplatePathsForSubmodule(submod);
                        if (submodPaths != null && submodPaths.Count > 0)
                            paths.AddRange(submodPaths);
                    }
                }
            }
            else    // Is a normal module
            {
                if (EM_Settings.IsModuleEnable(mod))
                {
                    paths = manager.AndroidManifestTemplatePaths;
                }
            }

            return paths;
        }

        internal static void GenerateManifest(string jdkPath, bool forceGenerate = false, bool verbose = false)
        {
            // First check if JDK is available.
            if (string.IsNullOrEmpty(jdkPath))
            {
                UnityEngine.Debug.LogError("JDK path is invalid. Aborting EM AndroidManifest generation...");
                return;
            }

            // Gather module manifests and merge them into one main manifest for EM.
            List<string> libPaths = new List<string>();

            //--------------------------------------Check for enable modules/ features---------------------------------------------------------//
            //Add more coditions to be checked and included when EasyMobile generates the main AndroidManifest.xml

            // Add the AndroidManifest template paths of all active modules and submodules.
            foreach (Module mod in Enum.GetValues(typeof(Module)))
            {
                var modulePaths = GetAndroidManifestTemplatePathsForModule(mod);
                if (modulePaths != null && modulePaths.Count > 0)
                    libPaths.AddRange(modulePaths);
            }

#if EASY_MOBILE_PRO
            // Add custom Android Manifest
            if (File.Exists(sCustomManifestPath))
                libPaths.Add(sCustomManifestPath);
#endif

            //---------------------------------------------------------------------------------------------------------------------------------//

            string libsPathsStr = "";
            foreach (var lib in libPaths)
            {
                // Fix issue if the path contains spaces
#if UNITY_EDITOR_WIN
                string actualPath = '"' + lib + '"';  
#else
                string actualPath = "'" + lib + "'";
#endif
                libsPathsStr += " --libs " + actualPath;
            }

            // Stop if we have generated a manifest before and there's nothing new to added & not force generating.
            if (libsPathsStr == sLastTimeManifestLibsString && forceGenerate == false)
                return;

            var config = new EM_AndroidLibBuilder.AndroidLibConfig
            {
                packageName = EM_Constants.AndroidNativePackageName,
                targetLibFolderName = EM_Constants.EasyMobileAndroidResFolderName,
                targetContentFolderName = "res"
            };

            EM_AndroidLibBuilder.BuildAndroidLibFromFolder("", config, null, null, null,
                (manifestPath) =>
                {
                    string templatePath = FileIO.ToAbsolutePath("EasyMobile/Editor/Templates/Manifests/Manifest_Main.xml");
                    string outputPath = FileIO.ToAbsolutePath(manifestPath.Remove(0, 7));   // Remove "Assets/" from manifestPath
                    string mexcPath = FileIO.ToAbsolutePath("EasyMobile/Editor");

                    sLastTimeManifestLibsString = libsPathsStr;
                    sMainManifestGeneratedSucessfully = true;

                    if (verbose)
                        UnityEngine.Debug.Log("Generating an Android manifest at\n" + outputPath + " \n");

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WorkingDirectory = mexcPath,
                    };

                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                    startInfo.WorkingDirectory = mexcPath;

                    string javaPath = FileIO.SlashesToPlatformSeparator(Path.Combine(jdkPath, "bin/java"));
#if UNITY_EDITOR_WIN
                    // Fix issue if paths contain spaces.
                    string actualTemplatePath = '"' + templatePath + '"';
                    string actualOutputPath = '"' + outputPath + '"';
                    string actualJavaPath = '"' + javaPath + '"';

                    startInfo.FileName = javaPath;
                    string command = String.Format(" -cp .\\ManifMerger\\* com.android.manifmerger.Merger --main {1}{2} --out {3}", 
                        actualJavaPath, actualTemplatePath, libsPathsStr, actualOutputPath);
                    startInfo.Arguments = command;
#else
                    // Fix issue if paths contain spaces.
                    string actualTemplatePath = "'" + templatePath + "'";
                    string actualOutputPath = "'" + outputPath + "'";
                    string actualJavaPath = "'" + javaPath + "'";

                    string baseCommand = new StringBuilder().Append(actualJavaPath).
                                                                Append(" -cp ./ManifMerger/annotations-12.0.jar:").
                                                                Append("./ManifMerger/annotations-25.3.0.jar:").
                                                                Append("./ManifMerger/bcpkix-jdk15on-1.48.jar:").
                                                                Append("./ManifMerger/bcprov-jdk15on-1.48.jar:").
                                                                Append("./ManifMerger/builder-model-2.3.0.jar:").
                                                                Append("./ManifMerger/builder-test-api-2.3.0.jar:").
                                                                Append("./ManifMerger/common-25.3.0.jar:").
                                                                Append("./ManifMerger/commons-codec-1.4.jar:").
                                                                Append("./ManifMerger/commons-compress-1.8.1.jar:").
                                                                Append("./ManifMerger/commons-logging-1.1.1.jar:").
                                                                Append("./ManifMerger/ddmlib-25.3.0.jar:").
                                                                Append("./ManifMerger/dvlib-25.3.0.jar:").
                                                                Append("./ManifMerger/gson-2.2.4.jar:").
                                                                Append("./ManifMerger/guava-18.0.jar:").
                                                                Append("./ManifMerger/httpclient-4.1.1.jar:").
                                                                Append("./ManifMerger/httpcore-4.1.jar:").
                                                                Append("./ManifMerger/httpmime-4.1.jar:").
                                                                Append("./ManifMerger/jimfs-1.1.jar:").
                                                                Append("./ManifMerger/kxml2-2.3.0.jar:").
                                                                Append("./ManifMerger/layoutlib-api-25.3.0.jar:").
                                                                Append("./ManifMerger/repository-25.3.0.jar:").
                                                                Append("./ManifMerger/sdk-common-25.3.0.jar:").
                                                                Append("./ManifMerger/sdklib-25.3.0.jar:").
                                                                Append("./ManifMerger/manifest-merger-25.3.0.jar ").
                                                                Append("com.android.manifmerger.Merger --main {0}{1} --out {2}").ToString();

                    startInfo.FileName = "/bin/bash";
                    string command = String.Format(baseCommand, actualTemplatePath, libsPathsStr, actualOutputPath);
                    startInfo.Arguments = "-c \"" + command + "\"";
#endif

                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.OutputDataReceived += OnConsoleDataReceived;
                    process.ErrorDataReceived += OnConsoleErrorReceived;
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }
            );
        }

        internal static void OnConsoleErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            sMainManifestGeneratedSucessfully = false;
            UnityEngine.Debug.LogError(e.Data);

            if (ManifMergerConsoleErrorsDataReceived != null)
                ManifMergerConsoleErrorsDataReceived(e.Data);
        }

        internal static void OnConsoleDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            UnityEngine.Debug.Log(e.Data);

            if (ManifMergerConsoleDataDataReceived != null)
                ManifMergerConsoleDataDataReceived(e.Data);
        }

        #endregion
    }
}

