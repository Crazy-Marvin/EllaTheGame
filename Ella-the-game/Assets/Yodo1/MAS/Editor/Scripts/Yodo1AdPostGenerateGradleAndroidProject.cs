#if UNITY_2018_2_OR_NEWER && UNITY_ANDROID

using UnityEngine;
using UnityEditor;
using UnityEditor.Android;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

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
            Yodo1ValidateGradle(path);
            Yodo1ValidateManifest(path);
            Yodo1ValidateGradleProperties(path);

            if (isAdReviewFuntionEnable())
            {
                AddAdReview(path);
            }

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

        static void Yodo1ValidateManifest(string path)
        {
            var mainfestPath = Path.Combine(path, "src/main/AndroidManifest.xml");
            ValidateManifestForSupportAndroid12(mainfestPath, getProjectGradlePath(path));

            if (mainfestPath.Contains("unityLibrary"))
            {
                mainfestPath = mainfestPath.Replace("unityLibrary", "launcher");
            }

            if (File.Exists(mainfestPath))
            {
                Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();
                if (Yodo1PostProcessAndroid.CheckConfiguration_Android(settings))
                {
                    ValidateManifest(mainfestPath, settings);
                }
            }
        }

        private static void ValidateManifestForSupportAndroid12(string manifestFile, string projectGradlePath)
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

            // supprted android 12
            int targetSDKVersion = getTargetSDKVersion(projectGradlePath);
            if (targetSDKVersion <= 30) // support android 12
            {
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

        static void ValidateManifest(string manifestFile, Yodo1AdSettings settings)
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

            ////Enable hardware acceleration for video play
            //XmlElement elem = (XmlElement)app;

            //Add AdMob App ID
            if (settings.androidSettings.GooglePlayStore)
            {
                string admobAppIdValue = settings.androidSettings.AdmobAppID.Trim();
                if (string.IsNullOrEmpty(admobAppIdValue))
                {
                    Debug.LogError(Yodo1U3dMas.TAG + "MAS Android AdMob App ID is null, please check the configuration.");
                    return;
                }
                string admobAppIdName = "com.google.android.gms.ads.APPLICATION_ID";
                ValidateMetaData(app, doc, ns, admobAppIdName, admobAppIdValue);
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
            if (settings.androidSettings.GooglePlayStore)
            {
                channelValue = "GooglePlay";
            }
            string channelName = "Yodo1ChannelCode";
            XmlElement metaElementChannel = ValidateMetaData(app, doc, ns, channelName, channelValue);
            string ns2 = manNode.GetNamespaceOfPrefix("tools");
            metaElementChannel.SetAttribute("replace", ns2, "android:value");

            //Add Engine type and version
            string version = Application.unityVersion;
            if (!string.IsNullOrEmpty(version))
            {
                string engineType = "engineType";
                ValidateMetaData(app, doc, ns, engineType, "Unity");

                string engineVersion = "engineVersion";
                ValidateMetaData(app, doc, ns, engineVersion, version);
            }

            doc.Save(manifestFile);
        }

        private static XmlElement ValidateMetaData(XmlNode app, XmlDocument doc, string ns, string metaName, string metaValue)
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

        static void Yodo1ValidateGradleProperties(string path)
        {
            var gradlePropertiesPath = Path.Combine(path.Replace("unityLibrary", ""), "gradle.properties");
            StreamReader streamReader1 = new StreamReader(gradlePropertiesPath);
            string text_all = streamReader1.ReadToEnd();
            streamReader1.Close();

            //bool changed = false;
            string oldLineStr = string.Empty;
            string newLineStr = string.Empty;

            string[] array = text_all.Split(System.Environment.NewLine.ToCharArray());
            for (int i = 0; i < array.Length; i++)
            {
                string str = array[i];
                if (str.Contains("android.enableDexingArtifactTransform"))
                {
                    oldLineStr = str;
                    break;
                }
            }

            if (string.IsNullOrEmpty(oldLineStr) || oldLineStr.Contains("false"))
            {
                if (string.IsNullOrEmpty(oldLineStr))
                {
                    newLineStr = "android.enableDexingArtifactTransform=false";
                    text_all = text_all + "\n" + newLineStr;
                }
                else
                {
                    newLineStr = oldLineStr.Replace("false", "true");
                    text_all = text_all.Replace(oldLineStr, newLineStr);
                }
                StreamWriter streamWriter = new StreamWriter(gradlePropertiesPath);
                streamWriter.Write(text_all);
                streamWriter.Close();
            }
        }

        static void Yodo1ValidateGradle(string path)
        {
            var gradlePath = Path.Combine(path, "build.gradle");
            ValidateGradlePluginVersion(path);
            if (gradlePath.Contains("unityLibrary"))
            {
                gradlePath = gradlePath.Replace("unityLibrary", "launcher");
            }
            Debug.LogFormat(Yodo1U3dMas.TAG + "Updating gradle for Play Instant: {0}", gradlePath);
            WriteBelow(gradlePath, "defaultConfig {", "\t\tmultiDexEnabled true");
            validateCompileSDKLevel(gradlePath);
        }

        private static void validateCompileSDKLevel(string launcherGradlePath)
        {
            StreamReader streamReader1 = new StreamReader(launcherGradlePath);
            string text_all = streamReader1.ReadToEnd();
            streamReader1.Close();

            string oldLineStr = string.Empty;
            string newLineStr = "compileSdkVersion 31";

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
                if (parsed && version >= 31)
                {
                    return;
                }
            }

            text_all = text_all.Replace(oldLineStr, newLineStr);
            StreamWriter streamWriter = new StreamWriter(launcherGradlePath);
            streamWriter.Write(text_all);
            streamWriter.Close();
            //Debug.Log(Yodo1U3dMas.TAG + "changed compileSdkVersion version from " + oldLineStr + " to " + newLineStr);
        }

        static void ValidateGradlePluginVersion(string path)
        {
            string projectGradlePath = getProjectGradlePath(path);
            ValidateGradlePluginVersion_(projectGradlePath);
        }

        private static string getProjectGradlePath(string path)
        {
            var projectGradlePath = "";
            if (path.Contains("unityLibrary"))
            {
                var projectPath = path.Replace("unityLibrary", "");
                projectGradlePath = Path.Combine(projectPath, "build.gradle");
            }
            else
            {
                projectGradlePath = Path.Combine(path, "build.gradle");
            }
            return projectGradlePath;
        }

        private static void ValidateGradlePluginVersion_(string projectGradlePath)
        {
            StreamReader streamReader1 = new StreamReader(projectGradlePath);
            string text_all = streamReader1.ReadToEnd();
            streamReader1.Close();

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
                    string resultPluginVersion = ValidatePluginVersionStr_(oldPluginVersion, projectGradlePath);
                    if (resultPluginVersion != null && !oldPluginVersion.Equals(resultPluginVersion))
                    {
                        newLineStr = oldLineStr.Replace(oldPluginVersion, resultPluginVersion);
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
                Debug.Log(Yodo1U3dMas.TAG + "changed gradle plugin version from " + oldLineStr + " to " + newLineStr);
            }
        }

        private static string ValidatePluginVersionStr_(string oldPluginVersion, string projectGradlePath)
        {
            if (!oldPluginVersion.Contains("."))
            {
                return null;
            }

            var versionNumStr = oldPluginVersion.Replace(".", "");
            int oldVerionNum = int.Parse(versionNumStr);
            int minVersionNum = 401;

            if (oldVerionNum >= minVersionNum)
            {
                return null;
            }
            else
            {
                return "4.0.1";
            }
        }

        private static int getTargetSDKVersion(string projectGradlePath)
        {
            int targetSDKVersion = (int)PlayerSettings.Android.targetSdkVersion;
            Debug.Log(Yodo1U3dMas.TAG + "current targetSDKVersion is : " + targetSDKVersion);
            if (targetSDKVersion > 0)
            {
                return targetSDKVersion;
            }

            StreamReader streamReader1 = new StreamReader(projectGradlePath);
            string text_all = streamReader1.ReadToEnd();
            streamReader1.Close();

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
                string temp = targetLineStr.Replace("targetSdkVersion", "");
                if (!string.IsNullOrEmpty(temp))
                {
                    try
                    {
                        Debug.Log(Yodo1U3dMas.TAG + "get targetSDKVersion from gradle is : " + temp);
                        return int.Parse(temp);
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