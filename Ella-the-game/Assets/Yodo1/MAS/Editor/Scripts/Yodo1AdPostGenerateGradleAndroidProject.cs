#if UNITY_2018_2_OR_NEWER && UNITY_ANDROID

using UnityEngine;
using UnityEditor;
using UnityEditor.Android;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System;

namespace Yodo1.MAS
{
    public class Yodo1PostGenerateGradleAndroidProject : Yodo1ProcessGradleBuildFile, IPostGenerateGradleAndroidProject
    {
        public int callbackOrder
        {
            get { return 100; }
        }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            UpdateBuildGradle(path);
            UpdateAndroidManifest(path);
            UpdateGradleProperties(path);

            //if (Yodo1AdUtils.IsAppLovinValid())
            //{
            //    AddAdReview(path);
            //}

#if UNITY_2022_2_OR_NEWER
            var rootSettingsGradleFilePath = Path.Combine(path, "../settings.gradle");
            AddMasRepository(rootSettingsGradleFilePath);
#endif
        }

        /// <summary>
        /// Adds Quality Service plugin to the Gradle project once the project has been exported. See <see cref="Yodo1ProcessGradleBuildFile"/> for more details.
        /// </summary>
        private void AddAdReview(string path)
        {
#if UNITY_2019_3_OR_NEWER
            // On Unity 2019.3+, the path returned is the path to the unityLibrary's module.
            // The AppLovin Quality Service buildscript closure related lines need to be added to the root build.gradle file.
            var rootGradleBuildFilePath = Path.Combine(path, "../build.gradle");
#if UNITY_2022_2_OR_NEWER
            if (!AddPluginToRootGradleBuildFile(rootGradleBuildFilePath)) return;

            var rootSettingsGradleFilePath = Path.Combine(path, "../settings.gradle");
            if (!AddAppLovinRepository(rootSettingsGradleFilePath)) return;
#else
            var buildScriptChangesAdded = AddQualityServiceBuildScriptLines(rootGradleBuildFilePath);
            if (!buildScriptChangesAdded) return;
#endif

            // The plugin needs to be added to the application module (named launcher)
            var applicationGradleBuildFilePath = Path.Combine(path, "../launcher/build.gradle");
#else
            // If Gradle template is enabled, we would have already updated the plugin.
            if (AppLovinIntegrationManager.GradleTemplateEnabled) return;

            var applicationGradleBuildFilePath = Path.Combine(path, "build.gradle");
#endif

            if (!File.Exists(applicationGradleBuildFilePath))
            {
                Debug.LogError(Yodo1U3dMas.TAG + "Couldn't find build.gradle file. Failed to add AppLovin Quality Service plugin to the gradle project.");
                return;
            }

            AddAppLovinQualityServicePlugin(applicationGradleBuildFilePath);
        }

#region Android Manifest

        static void UpdateAndroidManifest(string path)
        {
            // unityLibrary
            var manifestPath = Path.Combine(path, "src/main/AndroidManifest.xml");
            CheckAndAddExportedForAndroidApiLevel31(manifestPath, GetLauncherGradlePath(path));

            // launcher
            if (manifestPath.Contains("unityLibrary"))
            {
                manifestPath = manifestPath.Replace("unityLibrary", "launcher");
            }
            if (File.Exists(manifestPath))
            {
                Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();
                if (Yodo1PostProcessAndroid.CheckConfiguration_Android(settings))
                {
                    UpdateManifestWithSettings(manifestPath, settings);
                }
            }
        }

        private static void CheckAndAddExportedForAndroidApiLevel31(string manifestFile, string projectGradlePath)
        {
            // Check AndroidSdkVersions.AndroidApiLevel30(Android 11)
            int targetSdkVersion = GetTargetSdkVersion(projectGradlePath);
            if (targetSdkVersion <= 30)
            {
                return;
            }

            // Trying to add the exported attribute for AndroidApiLevel31(Android 12)
            XmlDocument doc = new XmlDocument();
            doc.Load(manifestFile);

            if (doc == null)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "Couldn't load " + manifestFile);
                return;
            }

            XmlNode manNode = Yodo1PostProcessAndroid.FindChildNode(doc, "manifest");
            string ns = manNode.GetNamespaceOfPrefix("android");

            XmlNode app = Yodo1PostProcessAndroid.FindChildNode(manNode, "application");
            if (app == null)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "Error parsing " + manifestFile + ", tag for application not found.");
                return;
            }

            XmlNode launcherActivityNode = Yodo1PostProcessAndroid.FindLauncherActivityNode(app);
            if (launcherActivityNode != null)
            {
                XmlElement actvitiyElement = (XmlElement)launcherActivityNode;
                actvitiyElement.SetAttribute("exported", ns, "true");
            }

            doc.Save(manifestFile);
        }

        static void UpdateManifestWithSettings(string manifestFile, Yodo1AdSettings settings)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(manifestFile);

            if (doc == null)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "Couldn't load " + manifestFile);
                return;
            }

            XmlNode manNode = Yodo1PostProcessAndroid.FindChildNode(doc, "manifest");
            string ns = manNode.GetNamespaceOfPrefix("android");

            XmlNode app = Yodo1PostProcessAndroid.FindChildNode(manNode, "application");
            if (app == null)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "Error parsing " + manifestFile + ", tag for application not found.");
                return;
            }

            //Add AdMob App ID
            string admobAppIdValue = settings.androidSettings.AdmobAppID.Trim();
            if (Yodo1AdUtils.IsAdMobValid())
            {
                if (string.IsNullOrEmpty(admobAppIdValue))
                {
                    Debug.LogError(Yodo1U3dMas.TAG + "MAS Android AdMob App ID is null, please check the configuration.");
                    return;
                }
                string admobAppIdName = "com.google.android.gms.ads.APPLICATION_ID";
                AddMetaData(app, doc, ns, admobAppIdName, admobAppIdValue);
            }

            //Add Channel
            string channelValue = string.Empty;
            if (settings.androidSettings.ChineseAndroidStores)
            {
                channelValue = settings.androidSettings.Channel.Trim();
                if (string.IsNullOrEmpty(channelValue))
                {
                    Debug.LogError(Yodo1U3dMas.TAG + "MAS Android Channel is null, please check the configuration.");
                    return;
                }
            }
            if (Yodo1AdUtils.IsGooglePlayVersion())
            {
                channelValue = "GooglePlay";
            }
            string channelName = "Yodo1ChannelCode";
            XmlElement metaElementChannel = AddMetaData(app, doc, ns, channelName, channelValue);
            string ns2 = manNode.GetNamespaceOfPrefix("tools");
            metaElementChannel.SetAttribute("replace", ns2, "android:value");

            //Add Engine type and version
            string version = Application.unityVersion;
            if (!string.IsNullOrEmpty(version))
            {
                string engineType = "engineType";
                AddMetaData(app, doc, ns, engineType, "Unity");

                string engineVersion = "engineVersion";
                AddMetaData(app, doc, ns, engineVersion, version);
            }

            doc.Save(manifestFile);
        }

        private static XmlElement AddMetaData(XmlNode app, XmlDocument doc, string ns, string metaName, string metaValue)
        {
            XmlNode metaNode = Yodo1PostProcessAndroid.FindChildNodeWithAttribute(app, "meta-data", "android:name", metaName);
            if (metaNode == null)
            {
                metaNode = (XmlElement)doc.CreateNode(XmlNodeType.Element, "meta-data", null);
                app.AppendChild(metaNode);
            }

            XmlElement metaElement = (XmlElement)metaNode;
            metaElement.SetAttribute("name", ns, metaName);
            metaElement.SetAttribute("value", ns, metaValue);
            metaElement.GetNamespaceOfPrefix("android");

            return metaElement;
        }

#endregion

#region Gradle Properties

        private static string GetProjectPropertiesPath(string path)
        {
            var projectPropertiesPath = string.Empty;
            if (path.Contains("unityLibrary"))
            {
                var projectPath = path.Replace("unityLibrary", "");
                projectPropertiesPath = Path.Combine(projectPath, "gradle.properties");
            }
            else
            {
                projectPropertiesPath = Path.Combine(path, "gradle.properties");
            }
            return projectPropertiesPath;
        }

        static void UpdateGradleProperties(string path)
        {
            var gradlePropertiesPath = GetProjectPropertiesPath(path);

            StreamReader streamReader = new StreamReader(gradlePropertiesPath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            string enableDexingArtifactTransform = string.Empty;
            string[] array = text_all.Split(System.Environment.NewLine.ToCharArray());
            for (int i = 0; i < array.Length; i++)
            {
                string lineString = array[i];
                if (lineString.Contains("android.enableDexingArtifactTransform"))
                {
                    enableDexingArtifactTransform = lineString;
                    break;
                }
            }

            bool needToChange = false;
            if (string.IsNullOrEmpty(enableDexingArtifactTransform))
            {
                needToChange = true;
                text_all = text_all + "\n" + "android.enableDexingArtifactTransform=true";
            }
            else
            {
                if (enableDexingArtifactTransform.Contains("false"))
                {
                    needToChange = true;
                    string temp = enableDexingArtifactTransform.Replace("false", "true");
                    text_all = text_all.Replace(enableDexingArtifactTransform, temp);
                }
            }

            if (needToChange)
            {
                StreamWriter streamWriter = new StreamWriter(gradlePropertiesPath);
                streamWriter.Write(text_all);
                streamWriter.Close();
            }
        }

#endregion

#region Gradle

        private static string GetProjectGradlePath(string path)
        {
            var gradlePath = string.Empty;
            if (path.Contains("unityLibrary"))
            {
                var projectPath = path.Replace("unityLibrary", "");
                gradlePath = Path.Combine(projectPath, "build.gradle");
            }
            else
            {
                gradlePath = Path.Combine(path, "build.gradle");
            }
            return gradlePath;
        }

        private static string GetLauncherGradlePath(string path)
        {
            var gradlePath = string.Empty;
            if (path.Contains("unityLibrary"))
            {
                var projectPath = path.Replace("unityLibrary", "launcher");
                gradlePath = Path.Combine(projectPath, "build.gradle");
            }
            else
            {
                gradlePath = Path.Combine(path, "build.gradle");
            }
            return gradlePath;
        }

        static void UpdateBuildGradle(string path)
        {
            CheckAndUpateGradlePluginVersion(GetProjectGradlePath(path));
            CheckAndUpateCompileSdkVersion(GetLauncherGradlePath(path));

            // Add multiDexEnabled
            WriteBelow(GetLauncherGradlePath(path), "defaultConfig {", "\t\tmultiDexEnabled true");
        }

        private static void CheckAndUpateCompileSdkVersion(string launcherGradlePath)
        {
            StreamReader streamReader = new StreamReader(launcherGradlePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            string oldLineStr = string.Empty;
            string newLineStr = "\tcompileSdkVersion 33";

            string[] array = text_all.Split(System.Environment.NewLine.ToCharArray());
            for (int i = 0; i < array.Length; i++)
            {
                string str = array[i];
                if (str.Contains("compileSdkVersion"))
                {
                    oldLineStr = str;
                    break;
                }
            }

            if (string.IsNullOrEmpty(oldLineStr))
            {
                return;
            }

            string tempStr = oldLineStr.Trim();
            string[] tempArray = Regex.Split(tempStr, @"\s+");
            if (tempArray.Length > 1)
            {
                string versionStr = tempArray[1];
                int version;
                bool parsed = int.TryParse(versionStr, out version);
                if (parsed && version >= 33)
                {
                    return;
                }
            }

            text_all = text_all.Replace(oldLineStr, newLineStr);
            StreamWriter streamWriter = new StreamWriter(launcherGradlePath);
            streamWriter.Write(text_all);
            streamWriter.Close();
        }

        private static void CheckAndUpateGradlePluginVersion(string projectGradlePath)
        {
            StreamReader streamReader = new StreamReader(projectGradlePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            bool changed = false;
            string oldLineStr = string.Empty;
            string newLineStr = string.Empty;

            string[] array = text_all.Split(System.Environment.NewLine.ToCharArray());
            for (int i = 0; i < array.Length; i++)
            {
                string str = array[i];
                if (str.Contains("com.android.tools.build:gradle"))
                {
                    oldLineStr = str;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(oldLineStr) && oldLineStr.Contains(":") && oldLineStr.Contains("'"))
            {
                var temp = oldLineStr.Replace("'", "");
                string[] tempArray = temp.Split(":".ToCharArray());
                if (tempArray != null && tempArray.Length > 0)
                {
                    var oldPluginVersion = tempArray[tempArray.Length - 1]; // such as 3.4.0
                    Version minVersion = new Version("4.2.0");
                    Version currentVersion = new Version(oldPluginVersion);
                    if (currentVersion < minVersion)
                    {
                        newLineStr = oldLineStr.Replace(oldPluginVersion, minVersion.ToString());
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                text_all = text_all.Replace(oldLineStr, newLineStr);
                StreamWriter streamWriter = new StreamWriter(projectGradlePath);
                streamWriter.Write(text_all);
                streamWriter.Close();
                Debug.Log(Yodo1U3dMas.TAG + "Gradle plugin version changed from " + oldLineStr.Trim() + " to " + newLineStr.Trim());
            }
        }

#endregion

        private static int GetTargetSdkVersion(string projectGradlePath)
        {
            int targetSdkVersion = (int)PlayerSettings.Android.targetSdkVersion;
            Debug.Log(Yodo1U3dMas.TAG + "PlayerSettings.Android.targetSdkVersion: " + targetSdkVersion);
            if (targetSdkVersion > 0)
            {
                return targetSdkVersion;
            }

            StreamReader streamReader = new StreamReader(projectGradlePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            string targetLineStr = string.Empty;

            string[] array = text_all.Split(System.Environment.NewLine.ToCharArray());
            for (int i = 0; i < array.Length; i++)
            {
                string str = array[i];
                if (str.Contains("targetSdkVersion"))
                {
                    targetLineStr = str.Trim().Replace(" ", "");
                    break;
                }
            }

            if (!string.IsNullOrEmpty(targetLineStr))
            {
                string tempTargetSdkVersion = targetLineStr.Replace("targetSdkVersion", "");
                if (!string.IsNullOrEmpty(tempTargetSdkVersion))
                {
                    try
                    {
                        Debug.Log(Yodo1U3dMas.TAG + "Got targetSdkVersion from gradle is : " + tempTargetSdkVersion);
                        return int.Parse(tempTargetSdkVersion);
                    }
                    catch
                    {
                        return 0;
                    }
                }

            }
            return 0;
        }

        static bool WriteBelow(string filePath, string below, string text)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            int beginIndex = text_all.LastIndexOf(below);
            if (beginIndex == -1)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "Error parsing " + filePath + ", tag for " + below + " not found.");
                return false;
            }

            if (text_all.IndexOf(text) == -1)
            {
                int endIndex = beginIndex + below.Length;

                text_all = text_all.Substring(0, endIndex) + "\n" + text + /*"\n" +*/ text_all.Substring(endIndex);

                StreamWriter streamWriter = new StreamWriter(filePath);
                streamWriter.Write(text_all);
                streamWriter.Close();
                return true;
            }
            return false;
        }
    }
}
#endif