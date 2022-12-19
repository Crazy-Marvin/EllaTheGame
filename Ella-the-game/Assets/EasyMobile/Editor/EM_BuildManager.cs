#if UNITY_ANDROID || UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace EasyMobile.Editor
{
#if UNITY_2018_1_OR_NEWER
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;

    public class EM_PreBuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            EM_BuildProcessorUtil.PreBuildProcessing(report.summary.platform, report.summary.outputPath);
        }
    }

    public class EM_PostBuildProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 9999; } }

        public void OnPostprocessBuild(BuildReport report)
        {
            EM_BuildProcessorUtil.PostBuildProcessing(report.summary.platform, report.summary.outputPath);
        }
    }

#elif UNITY_5_6_OR_NEWER
    using UnityEditor.Build;

    public class EM_PreBuildProcessor : IPreprocessBuild
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            EM_BuildProcessorUtil.PreBuildProcessing(target, path);
        }
    }

    public class EM_PostBuildProcessor : IPostprocessBuild
    {
        public int callbackOrder { get { return 9999; } }

        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            EM_BuildProcessorUtil.PostBuildProcessing(target, path);
        }
    }

#else
    using UnityEditor.Callbacks;

    public class EM_LegacyBuildProcessor
    {
        [PostProcessBuildAttribute(9999)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            EM_BuildProcessorUtil.PostBuildProcessing(target, path);
        }
    }
#endif


    public class EM_BuildProcessorUtil
    {
        public static void PreBuildProcessing(BuildTarget target, string path)
        {
            // Auto initialization check warning
            if (!EM_Settings.IsRuntimeAutoInitializationEnabled)
            {
                Debug.LogWarning(
                    "Easy Mobile's Auto Runtime Initialization feature is turned off. " +
                    "Make sure you call RuntimeManager.Init() before using Easy Mobile API in your code. " +
                    "You can re-enable the feature in menu Window > Easy Mobile > Build tab > Auto Initialization.");
            }

            if (target == BuildTarget.Android)
            {
                // Force regenerating manifest at every build to avoid issues due to
                // the generated file being removed unintentionally.
                string jdkPath = EM_EditorUtil.GetJdkPath();

                if (string.IsNullOrEmpty(jdkPath))
                    throw new BuildFailedException(
                        "A JDK path needs to be specified for the Android build. Go to Preferences > External Tools > JDK to set it.");

                EM_AndroidManifestBuilder.GenerateManifest(jdkPath, forceGenerate: true);
            }
        }

        public static void PostBuildProcessing(BuildTarget target, string path)
        {
#if UNITY_IOS
            if (target == BuildTarget.iOS)
            {
                // Read PBX project.
                string pbxPath = PBXProject.GetPBXProjectPath(path);
                PBXProject project = new PBXProject();
                project.ReadFromFile(pbxPath);

#if UNITY_2019_3_OR_NEWER
                string targetGUID = project.GetUnityMainTargetGuid();
#else
                string targetName = PBXProject.GetUnityTargetName();
                string targetGUID = project.TargetGuidByName(targetName);
#endif

                // Add frameworks here if needed.

                // Add required flags.
                project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");

                // Write PBX project.
                project.WriteToFile(pbxPath);

                // Update Plist.
                var plistPath = GetPlistPath(path);
                var plist = ReadPlist(plistPath);

                if (plist == null || plist.root == null)
                {
                    Debug.LogWarning("Info.plist or its root dictionary is null!");
                    return;
                }

                // Add necessary keys.
                AddIOSInfoPlistItemsRequiredByModules(plist);

                // Close Plist.
                WritePlist(plistPath, plist);
            }
#endif
        }

#if UNITY_IOS

        private static PlistDocument ReadPlist(string plistPath)
        {
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            return plist;
        }

        private static void WritePlist(string plistPath, PlistDocument plist)
        {
            if (plist != null)
                plist.WriteToFile(plistPath);
        }

        /// <summary>
        /// Adds the IOS Info.plist items required by EM modules.
        /// </summary>
        /// <returns>The IOS info plist items required by modules.</returns>
        /// <param name="plist">Plist.</param>
        private static PlistDocument AddIOSInfoPlistItemsRequiredByModules(PlistDocument plist)
        {
            if (plist == null || plist.root == null)
                return plist;

            var requiredInfoPlistItems = EM_PluginManager.GetAllIOSInfoItemsRequired();

            // Add usage descriptions.
            if (requiredInfoPlistItems != null)
            {
                foreach (var moduleItems in requiredInfoPlistItems)
                    foreach (var item in moduleItems.Value)
                        plist.root.AddIOSInfoPlistItem(item.Key, item.Value);
            }

            return plist;
        }

        internal static PlistElementDict AddIOSInfoPlistItemIf(bool shoudAdd, PlistElementDict root, string key, string value)
        {
            if (!shoudAdd)
                return root;

            return AddIOSInfoPlistItem(root, key, value);
        }

        internal static PlistElementDict AddIOSInfoPlistItems(PlistElementDict plistRoot, Dictionary<string, string> source)
        {
            if (source == null || source.Count < 1)
                return plistRoot;

            foreach (var pair in source)
                AddIOSInfoPlistItem(plistRoot, pair.Key, pair.Value);

            return plistRoot;
        }

        internal static PlistElementDict AddIOSInfoPlistItem(PlistElementDict plistRoot, string key, string value)
        {
            if (plistRoot == null)
                return plistRoot;

            if (string.IsNullOrEmpty(key))
                return plistRoot;

            if (string.IsNullOrEmpty(value))
                Debug.LogWarning("Dectected an empty Info.plist item value with the key: " + key);

            plistRoot.SetString(key, value);
            return plistRoot;
        }

        internal static string GetPlistPath(string path)
        {
            return Path.Combine(path, "Info.plist");
        }

#endif
    }

#if UNITY_IOS
    public static class EM_BuildProcessorUtilExtension
    {
        public static PlistElementDict AddIOSInfoPlistItemIf(this PlistElementDict root, bool shoudAdd, string key, string description)
        {
            return EM_BuildProcessorUtil.AddIOSInfoPlistItemIf(shoudAdd, root, key, description);
        }

        public static PlistElementDict AddIOSInfoPlistItems(this PlistElementDict plistRoot, Dictionary<string, string> source)
        {
            return EM_BuildProcessorUtil.AddIOSInfoPlistItems(plistRoot, source);
        }

        public static PlistElementDict AddIOSInfoPlistItem(this PlistElementDict plistRoot, string key, string description)
        {
            return EM_BuildProcessorUtil.AddIOSInfoPlistItem(plistRoot, key, description);
        }
    }
#endif
}
#endif
