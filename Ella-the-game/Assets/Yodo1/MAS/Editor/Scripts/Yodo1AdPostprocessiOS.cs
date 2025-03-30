#if UNITY_IOS || UNITY_IPHONE
#if UNITY_2019_3_OR_NEWER
using UnityEditor.iOS.Xcode.Extensions;
#endif
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;
using UnityEditor.iOS.Xcode;
using UnityEditor.XCodeEditor;

namespace Yodo1.MAS
{
    public class Yodo1SkAdNetworkData
    {
        [SerializeField] public string[] SkAdNetworkIds;
    }
    public class Yodo1PostProcessiOS
    {
        private const string TargetUnityIphonePodfileLine = "target 'Unity-iPhone' do";

        [PostProcessBuild(int.MaxValue)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();
                if (Yodo1AdSettingsSave.CheckConfiguration_iOS(settings))
                {
                    UpdateIOSPlist(pathToBuiltProject, settings);
                    UpdateIOSProject(pathToBuiltProject);
                    UpdatePodfile(pathToBuiltProject);
                    //if (Yodo1AdUtils.IsAppLovinValid(Yodo1PlatfromTarget.iOS))
                    //{
                    //    EnableAdReview(pathToBuiltProject);
                    //}
                    CheckAndPodInstall(pathToBuiltProject);
                }
            }
        }

        #region SkAdNetworksInfo

        private static Yodo1SkAdNetworkData GetSkAdNetworkData()
        {
            var uriBuilder = new UriBuilder("https://dash.applovin.com/docs/v1/unity_integration_manager/sk_ad_networks_info");
            uriBuilder.Query += "adnetworks=AdColony,Amazon,BidMachine,BigoAds,ByteDance,CSJ,Facebook,Fyber,Google,GoogleAdManager,InMobi,IronSource,Mintegral,Moloco,MyTarget,Tapjoy,TencentGDT,UnityAds,Vungle,Yandex,YSONetwork";
            var unityWebRequest = UnityWebRequest.Get(uriBuilder.ToString());

#if UNITY_2017_2_OR_NEWER
            var operation = unityWebRequest.SendWebRequest();
#else
            var operation = unityWebRequest.Send();
#endif
            // Wait for the download to complete or the request to timeout.
            while (!operation.isDone) { }


#if UNITY_2020_1_OR_NEWER
            if (unityWebRequest.result != UnityWebRequest.Result.Success)
#elif UNITY_2017_2_OR_NEWER
            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
#else
            if (unityWebRequest.isError)
#endif
            {
                Debug.LogError(Yodo1U3dMas.TAG + "Failed to retrieve SKAdNetwork IDs with error: " + unityWebRequest.error);
                return new Yodo1SkAdNetworkData();
            }
            try
            {
                return JsonUtility.FromJson<Yodo1SkAdNetworkData>(unityWebRequest.downloadHandler.text);
            }
            catch (Exception exception)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "Failed to parse data '" + unityWebRequest.downloadHandler.text + "' with exception: " + exception);
                return new Yodo1SkAdNetworkData();
            }
        }

        private static string[] GetSkAdNetworksFromLocal()
        {
            string[] applovinSkIDs = GetSkAdNetworksIDs("sk_ad_networks_info.plist");
            string[] bigoSkIDs = GetSkAdNetworksIDs("sk_ad_networks_info_bigo.plist");
            string[] tobidSkIDs = GetSkAdNetworksIDs("sk_ad_networks_info_tobid.plist");

            string[] skIDs = applovinSkIDs.Union(bigoSkIDs).ToArray<string>(); //Merge arrays and remove duplicates
            string[] allSkIDs = skIDs.Union(tobidSkIDs).ToArray<string>();

            return allSkIDs;
        }

        private static string[] GetSkAdNetworksIDs(string fileName)
        {
            string path = Application.dataPath + "/Yodo1/MAS/Editor/Resources";
            string plistPath = Path.Combine(path, fileName);
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            //Get Root
            PlistElementDict rootDict = plist.root;
            PlistElementArray skItems = (PlistElementArray)rootDict["SKAdNetworkItems"];
            if (skItems == null)
            {
                return null;
            }
            List<PlistElement> list = skItems.values;
            string[] mSKAdNetworkIDs = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                PlistElementDict skItem = (PlistElementDict)list[i];
                PlistElementString skAdNetworkIdentifier = (PlistElementString)skItem["SKAdNetworkIdentifier"];
                mSKAdNetworkIDs[i] = skAdNetworkIdentifier.value;
            }
            return mSKAdNetworkIDs;
        }

        #endregion

        private static void UpdateIOSPlist(string path, Yodo1AdSettings settings)
        {
            string plistPath = Path.Combine(path, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            //Get Root
            PlistElementDict rootDict = plist.root;
            PlistElementDict transportSecurity = rootDict.CreateDict("NSAppTransportSecurity");
            transportSecurity.SetBoolean("NSAllowsArbitraryLoads", true);

            //Set SKAdNetwork
            PlistElementArray skItem = rootDict.CreateArray("SKAdNetworkItems");
            var skAdNetworkData = GetSkAdNetworkData();
            var skAdNetworkLocal = GetSkAdNetworksFromLocal();
            var skAdNetworkIds = skAdNetworkData.SkAdNetworkIds;
            if (skAdNetworkIds == null || skAdNetworkIds.Length < 1)
            {
                skAdNetworkIds = skAdNetworkLocal;
            }
            else
            {
                //Merge arrays and remove duplicates
                skAdNetworkIds = skAdNetworkIds.Union(skAdNetworkLocal).ToArray<string>();
            }
            foreach (string skAdNetworkId in skAdNetworkIds)
            {
                PlistElementDict skDic = skItem.AddDict();
                skDic.SetString("SKAdNetworkIdentifier", skAdNetworkId);
            }

            //Set AppLovinSdkKey
            rootDict.SetString("AppLovinSdkKey", Yodo1AdEditorConstants.DEFAULT_APPLOVIN_SDK_KEY);

            if (settings.iOSSettings.GlobalRegion)
            {
                //Add Google AdMob App ID
                rootDict.SetString("GADApplicationIdentifier", settings.iOSSettings.AdmobAppID);
                //Enable Google Ad Manager
                rootDict.SetBoolean("GADIsAdManagerApp", true);
            }

            string version = Application.unityVersion;
            if (!string.IsNullOrEmpty(version))
            {
                rootDict.SetString("engineType", "Unity");
                rootDict.SetString("engineVersion", version);
            }

            PlistElementString locationAlwaysUsagePrivacy = (PlistElementString)rootDict["NSLocationAlwaysUsageDescription"];
            if (locationAlwaysUsagePrivacy == null)
            {
                rootDict.SetString("NSLocationAlwaysUsageDescription", "Some ad content may require access to the location for an interactive ad experience.");
            }

            PlistElementString locationWhenInUseUsagePrivacy = (PlistElementString)rootDict["NSLocationWhenInUseUsageDescription"];
            if (locationWhenInUseUsagePrivacy == null)
            {
                rootDict.SetString("NSLocationWhenInUseUsageDescription", "Some ad content may require access to the location for an interactive ad experience.");
            }

            PlistElementString locationAlwaysAndWhenInUseUsagePrivacy = (PlistElementString)rootDict["NSLocationAlwaysAndWhenInUseUsageDescription"];
            if (locationAlwaysAndWhenInUseUsagePrivacy == null)
            {
                rootDict.SetString("NSLocationAlwaysAndWhenInUseUsageDescription", "Some ad content may require access to the location for an interactive ad experience.");
            }

            PlistElementString attPrivacy = (PlistElementString)rootDict["NSUserTrackingUsageDescription"];
            if (attPrivacy == null)
            {
                rootDict.SetString("NSUserTrackingUsageDescription", "This identifier will be used to deliver personalized ads to you.");
            }

            PlistElementString bluetoothPrivacy = (PlistElementString)rootDict["NSBluetoothAlwaysUsageDescription"];
            if (bluetoothPrivacy == null)
            {
                rootDict.SetString("NSBluetoothAlwaysUsageDescription", "Some ad content may require access to the location for an interactive ad experience.");
            }

            File.WriteAllText(plistPath, plist.WriteToString());
        }

        private static void UpdateIOSProject(string buildPath)
        {
            UnityEditor.iOS.Xcode.PBXProject proj = new UnityEditor.iOS.Xcode.PBXProject();
            string projPath = UnityEditor.iOS.Xcode.PBXProject.GetPBXProjectPath(buildPath);
            proj.ReadFromFile(projPath);

            string unityMainTargetGuid = string.Empty;
            string unityFrameworkTargetGuid = string.Empty;

#if UNITY_2019_3_OR_NEWER
            unityMainTargetGuid = proj.GetUnityMainTargetGuid();
            unityFrameworkTargetGuid = proj.GetUnityFrameworkTargetGuid();
#else
            unityMainTargetGuid = proj.TargetGuidByName("Unity-iPhone");
            unityFrameworkTargetGuid = unityMainTargetGuid;
#endif

            proj.SetBuildProperty(unityMainTargetGuid, "ENABLE_BITCODE", "NO");
            proj.SetBuildProperty(unityFrameworkTargetGuid, "ENABLE_BITCODE", "NO");
            proj.SetBuildProperty(unityFrameworkTargetGuid, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");

#if UNITY_2019_4_OR_NEWER
            string MARKETING_VERSION = proj.GetBuildPropertyForConfig(unityMainTargetGuid, "MARKETING_VERSION");
            if (string.IsNullOrEmpty(MARKETING_VERSION))
            {
                proj.SetBuildProperty(unityMainTargetGuid, "MARKETING_VERSION", Application.version);
            }
            MARKETING_VERSION = proj.GetBuildPropertyForConfig(unityFrameworkTargetGuid, "MARKETING_VERSION");
            if (string.IsNullOrEmpty(MARKETING_VERSION))
            {
                proj.SetBuildProperty(unityFrameworkTargetGuid, "MARKETING_VERSION", Application.version);
            }
#endif
            /// <summary>
            /// For Swift 5+ code that uses the standard libraries, the Swift Standard Libraries MUST be embedded for iOS < 12.2
            /// Swift 5 introduced ABI stability, which allowed iOS to start bundling the standard libraries in the OS starting with iOS 12.2
            /// Issue Reference: https://github.com/facebook/facebook-sdk-for-unity/issues/506
            /// </summary>
            proj.SetBuildProperty(unityMainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            proj.AddBuildProperty(unityFrameworkTargetGuid, "LD_RUNPATH_SEARCH_PATHS", "/usr/lib/swift");

            EmbedDynamicLibrariesIfNeeded(buildPath, proj, unityMainTargetGuid);
            AddSwiftSupportIfNeeded(buildPath, proj, unityFrameworkTargetGuid);
            // rewrite to file
            File.WriteAllText(projPath, proj.WriteToString());


#if UNITY_2019_3_OR_NEWER
            XCProject xcProject = new XCProject(buildPath);

            string unityFrameworkTargetName = "UnityFramework";
            PBXNativeTarget nativeTarget = xcProject.GetNativeTarget(unityFrameworkTargetName);
            if (nativeTarget == null)
            {
                unityFrameworkTargetName = "Unity-iPhone";
                nativeTarget = xcProject.GetNativeTarget(unityFrameworkTargetName);
            }
            if (nativeTarget != null)
            {
                string headersGuid = proj.GetHeadersBuildPhaseByTarget(unityFrameworkTargetGuid);
                string sourcesGuid = proj.GetSourcesBuildPhaseByTarget(unityFrameworkTargetGuid);

                PBXList buildPhases = (PBXList)nativeTarget.data["buildPhases"];
                if (headersGuid != null)
                {
                    buildPhases.Remove(headersGuid);

                    int index = 1;
                    if (sourcesGuid != null)
                    {
                        index = buildPhases.IndexOf(sourcesGuid);
                    }
                    if (index >= 0 && index < buildPhases.Count)
                    {
                        buildPhases.Insert(index, headersGuid);
                    }

                    nativeTarget.data["buildPhases"] = buildPhases;

                    xcProject.SetNativeTarget(unityFrameworkTargetName, nativeTarget);
                    xcProject.Save();
                }
            }
#endif
        }

        private static void EmbedDynamicLibrariesIfNeeded(string buildPath, UnityEditor.iOS.Xcode.PBXProject project, string targetGuid)
        {
            var dynamicLibraryPathsPresentInProject = DynamicLibraryPathsToEmbed.Where(dynamicLibraryPath => Directory.Exists(Path.Combine(buildPath, dynamicLibraryPath))).ToList();
            if (dynamicLibraryPathsPresentInProject.Count <= 0) return;

#if UNITY_2019_3_OR_NEWER
            // Embed framework only if the podfile does not contain target `Unity-iPhone`.
            //if (!ContainsUnityIphoneTargetInPodfile(buildPath))
            {
                foreach (var dynamicLibraryPath in dynamicLibraryPathsPresentInProject)
                {
                    var fileGuid = project.AddFile(dynamicLibraryPath, dynamicLibraryPath);
                    project.AddFileToEmbedFrameworks(targetGuid, fileGuid);
                }
            }
#else
            string runpathSearchPaths;
#if UNITY_2018_2_OR_NEWER
            runpathSearchPaths = project.GetBuildPropertyForAnyConfig(targetGuid, "LD_RUNPATH_SEARCH_PATHS");
#else
            runpathSearchPaths = "$(inherited)";
#endif
            runpathSearchPaths += string.IsNullOrEmpty(runpathSearchPaths) ? "" : " ";

            // Check if runtime search paths already contains the required search paths for dynamic libraries.
            if (runpathSearchPaths.Contains("@executable_path/Frameworks")) return;

            runpathSearchPaths += "@executable_path/Frameworks";
            project.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", runpathSearchPaths);
#endif
        }

        private static void AddSwiftSupportIfNeeded(string buildPath, UnityEditor.iOS.Xcode.PBXProject project, string targetGuid)
        {
            var swiftFileRelativePath = "Classes/MASSwiftSupport.swift";
            var swiftFilePath = Path.Combine(buildPath, swiftFileRelativePath);

            // Add Swift file
            CreateSwiftFile(swiftFilePath);
            var swiftFileGuid = project.AddFile(swiftFileRelativePath, swiftFileRelativePath, PBXSourceTree.Source);
            project.AddFileToBuild(targetGuid, swiftFileGuid);

            // Enable Swift modules
            project.SetBuildProperty(targetGuid, "SWIFT_VERSION", "5.0");
            project.AddBuildProperty(targetGuid, "CLANG_ENABLE_MODULES", "YES");
        }

        private static void CreateSwiftFile(string swiftFilePath)
        {
            if (File.Exists(swiftFilePath)) return;

            // Create a file to write to.
            using (var writer = File.CreateText(swiftFilePath))
            {
                writer.WriteLine("//\n//  MASSwiftSupport.swift\n//");
                writer.WriteLine("\nimport Foundation\n");
                writer.WriteLine("// This file ensures the project includes Swift support.");
                writer.WriteLine("// It is automatically generated by the MAS Unity Plugin.");
                writer.Close();
            }
        }

        public static void CopyDirectory(string srcPath, string dstPath, string[] excludeExtensions, bool overwrite = true)
        {
            if (!Directory.Exists(dstPath))
                Directory.CreateDirectory(dstPath);

            foreach (var file in Directory.GetFiles(srcPath, "*.*", SearchOption.TopDirectoryOnly).Where(path => excludeExtensions == null || !excludeExtensions.Contains(Path.GetExtension(path))))
            {
                File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)), overwrite);
            }

            foreach (var dir in Directory.GetDirectories(srcPath))
                CopyDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)), excludeExtensions, overwrite);
        }

        public static void StartPodsProcess(string path, string podcommand)
        {
            var proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = Path.Combine(path, podcommand);
            proc.Start();
        }

        public static void EnableAdReview(string buildPath)
        {
            const string OutputFileName = "AppLovinQualityServiceSetup.rb";
            var sdkKey = Yodo1AdEditorConstants.DEFAULT_APPLOVIN_SDK_KEY;
            if (string.IsNullOrEmpty(sdkKey))
            {
                Debug.LogError(Yodo1U3dMas.TAG + "Failed to install AppLovin Quality Service plugin. AppLovin SDK Key is empty");
                return;
            }

            var outputFilePath = Path.Combine(buildPath, OutputFileName);

            // Check if Quality Service is already installed.
            if (File.Exists(outputFilePath) && Directory.Exists(Path.Combine(buildPath, "AppLovinQualityService")))
            {
                // TODO: Check if there is a way to validate if the SDK key matches the script. Else the pub can't use append when/if they change the SDK Key.
                return;
            }

            // Download the ruby script needed to install Quality Service
#if UNITY_2017_2_OR_NEWER
            var downloadHandler = new DownloadHandlerFile(outputFilePath);
#else
            var downloadHandler = new Yodo1AdDownloadHandler(path);
#endif
            var postJson = string.Format("{{\"sdk_key\" : \"{0}\"}}", sdkKey);
            var bodyRaw = Encoding.UTF8.GetBytes(postJson);
            var uploadHandler = new UploadHandlerRaw(bodyRaw);
            uploadHandler.contentType = "application/json";

            var unityWebRequest = new UnityWebRequest("https://api2.safedk.com/v1/build/ios_setup2")
            {
                method = UnityWebRequest.kHttpVerbPOST,
                downloadHandler = downloadHandler,
                uploadHandler = uploadHandler
            };

#if UNITY_2017_2_OR_NEWER
            var operation = unityWebRequest.SendWebRequest();
#else
            var operation = webRequest.Send();
#endif

            // Wait for the download to complete or the request to timeout.
            while (!operation.isDone) { }

#if UNITY_2020_1_OR_NEWER
            if (unityWebRequest.result != UnityWebRequest.Result.Success)
#elif UNITY_2017_2_OR_NEWER
            if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
#else
            if (webRequest.isError)
#endif
            {
                Debug.LogError(Yodo1U3dMas.TAG + "AppLovin Quality Service installation failed. Failed to download script with error: " + unityWebRequest.error);
                return;
            }

            // Check if Ruby is installed
            var rubyVersion = Yodo1AdCommandLine.Run("ruby", "--version", buildPath);
            if (rubyVersion.ExitCode != 0)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "AppLovin Quality Service installation requires Ruby. Please install Ruby, export it to your system PATH and re-export the project.");
                return;
            }

            // Ruby is installed, run `ruby AppLovinQualityServiceSetup.rb`
            var result = Yodo1AdCommandLine.Run("ruby", OutputFileName, buildPath);

            // Check if we have an error.
            if (result.ExitCode != 0) Debug.LogError(Yodo1U3dMas.TAG + "Failed to set up AppLovin Quality Service");

            Debug.Log(Yodo1U3dMas.TAG + result.Message);
        }

#if UNITY_2019_3_OR_NEWER
        private static bool ContainsUnityIphoneTargetInPodfile(string buildPath)
        {
            var podfilePath = Path.Combine(buildPath, "Podfile");
            if (!File.Exists(podfilePath)) return false;

            var lines = File.ReadAllLines(podfilePath);
            return lines.Any(line => line.Contains(TargetUnityIphonePodfileLine));
        }
#endif

        private static List<string> DynamicLibraryPathsToEmbed
        {
            get
            {
                var dynamicLibraryPathsToEmbed = new List<string>();

                if (Yodo1AdUtils.IsAppLovinValid(Yodo1PlatfromTarget.iOS))
                {
                    dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "AppLovinSDK/applovin-ios-sdk-12.6.1/AppLovinSDK.xcframework"));
                    // Amazon
                    dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "AmazonPublisherServicesSDK/APS_iOS_SDK-4.10.0/DTBiOSSDK.xcframework"));
                }

                // BidMachine
                if (Yodo1AdUtils.IsValidWithNetwork(Yodo1PlatfromTarget.iOS, "BidMachine"))
                {
                    dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "OMSDK_Appodeal/OMSDK_Appodeal.xcframework"));
                }

                // Fyber have changed it from dynamic to static, need not handle these lines
                //if (Yodo1AdUtils.IsValidWithNetwork(Yodo1PlatfromTarget.iOS, "Fyber")) 
                //{
                //    dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "Fyber_Marketplace_SDK/IASDKCore/IASDKCore.xcframework"));
                //}

                // InMobi
                if (Yodo1AdUtils.IsValidWithNetwork(Yodo1PlatfromTarget.iOS, "InMobi"))
                {
                    dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "InMobiSDK/InMobiSDK.xcframework"));
                }

                // Moloco
                if (Yodo1AdUtils.IsValidWithNetwork(Yodo1PlatfromTarget.iOS, "Moloco"))
                {
                    dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "MolocoSDKiOS/MolocoSDK.xcframework"));
                }

                // ToBid - KuaiShou
                if (Yodo1AdUtils.IsValidWithNetwork(Yodo1PlatfromTarget.iOS, "ToBid"))
                {
                    dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "ToBid-iOS/tobid-sdk-ios-cn/AdNetworks/kuaishou/KSAdSDK.xcframework"));
                }

                // YSONetwork
                if (Yodo1AdUtils.IsValidWithNetwork(Yodo1PlatfromTarget.iOS, "YSONetwork") || Yodo1AdUtils.IsValidWithNetwork(Yodo1PlatfromTarget.iOS, "YSO"))
                {
                    dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "YsoNetworkSDK/YsoNetwork.framework"));
                }

                return dynamicLibraryPathsToEmbed;
            }
        }

        private static void CheckAndPodInstall(string path)
        {
            var podVersion = Yodo1AdCommandLine.Run("pod", "--version", path);
            if (podVersion.ExitCode == 0)
            {
                var podResult = Yodo1AdCommandLine.Run("pod", "install --repo-update", path);
                if (podResult != null)
                {
                    if (podResult.ExitCode != 0)
                    {
                        Yodo1AdCommandLine.Run("pod", "install", path);
                    }
                }
            }
            else
            {
                //Debug.LogWarning(Yodo1U3dMas.TAG + "Cocoapods is not installed, " + podVersion.StandardOutput + "," + podVersion.StandardError);
            }
        }

        private static void UpdatePodfile(string path)
        {
            string topTag = "target 'UnityFramework' do";
            string installerTag = "post_install do |installer|";

            string installer =
                "\tinstaller.generated_projects.each do |project|\n" +
                "\t\tproject.targets.each do |target|\n" +
                "\t\t\ttarget.build_configurations.each do |config|\n" +
                "\t\t\t\tconfig.build_settings['IPHONEOS_DEPLOYMENT_TARGET'] = '13.0'\n" +
                "\t\t\tend\n" +
                "\t\tend\n" +
                "\tend\n";

            Yodo1AdFileClass app = new Yodo1AdFileClass(path + "/Podfile");
            //Debug.Log("=================== " + path + "/Podfile");
            if (app.IsHaveText(installerTag))
            {
                app.WriteBelow(installerTag, installer);
            }
            else
            {
                app.WriteBelow(topTag, installerTag + "\n" + installer + "end\n");
            }
        }
    }
}
#endif