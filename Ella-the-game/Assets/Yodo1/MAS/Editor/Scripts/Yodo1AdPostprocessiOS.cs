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

                    var podVersion = Yodo1AdCommandLine.Run("pod", "--version", pathToBuiltProject);
                    if (podVersion.ExitCode != 0)
                    {
                        Debug.LogError(Yodo1U3dMas.TAG + "Cocoapods is not installed, " + podVersion.StandardOutput + "," + podVersion.StandardError);
                        return;
                    }

                    var podResult = Yodo1AdCommandLine.Run("pod", "install --repo-update", pathToBuiltProject);
                    if (podResult != null)
                    {
                        EnableAdReview(pathToBuiltProject);

                        if (podResult.ExitCode != 0)
                        {
                            Yodo1AdCommandLine.Run("pod", "install", pathToBuiltProject);
                        }
                    }
                }
            }
        }

        private static Yodo1SkAdNetworkData GetSkAdNetworkData()
        {
            var uriBuilder = new UriBuilder("https://dash.applovin.com/docs/v1/unity_integration_manager/sk_ad_networks_info");
            uriBuilder.Query += "adnetworks=AdColony,ByteDance,Facebook,Fyber,Google,GoogleAdManager,InMobi,IronSource,Mintegral,MyTarget,Tapjoy,TencentGDT,UnityAds,Vungle,Yandex";
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

        private static void UpdateIOSPlist(string path, Yodo1AdSettings settings)
        {
            string[] mSKAdNetworkId = new string[] {
                "275upjj5gd.skadnetwork",
                "294l99pt4k.skadnetwork",
                "2u9pt9hc89.skadnetwork",
                "3rd42ekr43.skadnetwork",
                "4468km3ulz.skadnetwork",
                "44jx6755aq.skadnetwork",
                "4fzdc2evr5.skadnetwork",
                "4pfyvq9l8r.skadnetwork",
                "5lm9lj6jb7.skadnetwork",
                "6g9af3uyq4.skadnetwork",
                "7rz58n8ntl.skadnetwork",
                "7ug5zh24hu.skadnetwork",
                "8s468mfl3y.skadnetwork",
                "9nlqeag3gk.skadnetwork",
                "9rd848q2bz.skadnetwork",
                "9t245vhmpl.skadnetwork",
                "c6k4g5qg8m.skadnetwork",
                "cg4yq2srnc.skadnetwork",
                "ejvt5qm6ak.skadnetwork",
                "g28c52eehv.skadnetwork",
                "hs6bdukanm.skadnetwork",
                "kbmxgpxpgc.skadnetwork",
                "klf5c3l5u5.skadnetwork",
                "m8dbw4sv7c.skadnetwork",
                "mlmmfzh3r3.skadnetwork",
                "mtkv5xtk9e.skadnetwork",
                "ppxm28t8ap.skadnetwork",
                "prcb7njmu6.skadnetwork",
                "qqp299437r.skadnetwork",
                "rx5hdcabgc.skadnetwork",
                "t38b2kh725.skadnetwork",
                "tl55sbb4fm.skadnetwork",
                "u679fj5vs4.skadnetwork",
                "uw77j35x4d.skadnetwork",
                "v72qych5uu.skadnetwork",
                "wg4vff78zm.skadnetwork",
                "yclnxrl5pm.skadnetwork",
                "2fnua5tdw4.skadnetwork",
                "3qcr597p9d.skadnetwork",
                "3qy4746246.skadnetwork",
                "3sh42y64q3.skadnetwork",
                "424m5254lk.skadnetwork",
                "4dzt52r2t5.skadnetwork",
                "578prtvx9j.skadnetwork",
                "5a6flpkh64.skadnetwork",
                "8c4e2ghe7u.skadnetwork",
                "av6w8kgt66.skadnetwork",
                "cstr6suwn9.skadnetwork",
                "e5fvkxwrpn.skadnetwork",
                "f38h382jlk.skadnetwork",
                "kbd757ywx3.skadnetwork",
                "n6fk4nfna4.skadnetwork",
                "p78axxw29g.skadnetwork",
                "s39g8k73mm.skadnetwork",
                "v4nxqhlyqp.skadnetwork",
                "wzmmz9fp6w.skadnetwork",
                "ydx93a7ass.skadnetwork",
                "zq492l623r.skadnetwork",
                "24t9a8vw3c.skadnetwork",
                "32z4fx6l9h.skadnetwork",
                "523jb4fst2.skadnetwork",
                "54nzkqm89y.skadnetwork",
                "5l3tpt7t6e.skadnetwork",
                "6xzpu9s2p8.skadnetwork",
                "79pbpufp6p.skadnetwork",
                "9b89h5y424.skadnetwork",
                "cj5566h2ga.skadnetwork",
                "feyaarzu9v.skadnetwork",
                "ggvn48r87g.skadnetwork",
                "glqzh8vgby.skadnetwork",
                "gta9lk7p23.skadnetwork",
                "k674qkevps.skadnetwork",
                "ludvb6z3bs.skadnetwork",
                "n9x2a789qt.skadnetwork",
                "pwa73g5rt2.skadnetwork",
                "r45fhb6rf7.skadnetwork",
                "rvh3l7un93.skadnetwork",
                "x8jxxk4ff5.skadnetwork",
                "xy9t38ct57.skadnetwork",
                "zmvfpc5aq8.skadnetwork",
                "n38lu8286q.skadnetwork",
                "v9wttpbfk9.skadnetwork",
                "22mmun2rn5.skadnetwork",
                "252b5q8x7y.skadnetwork",
                "9g2aggbj52.skadnetwork",
                "dzg6xy7pwj.skadnetwork",
                "f73kdq92p3.skadnetwork",
                "hdw39hrw9y.skadnetwork",
                "x8uqf25wch.skadnetwork",
                "y45688jllp.skadnetwork",
                "74b6s63p6l.skadnetwork",
                "97r2b46745.skadnetwork",
                "b9bk5wbcq9.skadnetwork",
                "mls7yz5dvl.skadnetwork",
                "w9q455wk68.skadnetwork",
                "su67r6k2v3.skadnetwork",
                "r26jy69rpl.skadnetwork",
                "238da6jt44.skadnetwork",
                "44n7hlldy6.skadnetwork",
                "488r3q3dtq.skadnetwork",
                "52fl2v3hgk.skadnetwork",
                "5tjdwbrq8w.skadnetwork",
                "737z793b9f.skadnetwork",
                "9yg77x724h.skadnetwork",
                "ecpz2srf59.skadnetwork",
                "gvmwg8q7h5.skadnetwork",
                "lr83yxwka7.skadnetwork",
                "n66cz3y3bx.skadnetwork",
                "nzq8sh4pbs.skadnetwork",
                "pu4na253f3.skadnetwork",
                "v79kvwwj4g.skadnetwork",
                "yrqqpx2mcb.skadnetwork",
                "z4gj7hsk7h.skadnetwork",
                "f7s53z58qe.skadnetwork",
                "mp6xlyr22a.skadnetwork",
                "x44k69ngh6.skadnetwork",
                "7953jerfzd.skadnetwork",
            };
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
            var skAdNetworkIds = skAdNetworkData.SkAdNetworkIds;
            if (skAdNetworkIds == null || skAdNetworkIds.Length < 1)
            {
                skAdNetworkIds = mSKAdNetworkId;
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

           PlistElementString privacy = (PlistElementString)rootDict["NSLocationAlwaysUsageDescription"];
            if (privacy == null)
            {
                rootDict.SetString("NSLocationAlwaysUsageDescription", "Some ad content may require access to the location for an interactive ad experience.");
            }

            PlistElementString privacy1 = (PlistElementString)rootDict["NSLocationWhenInUseUsageDescription"];
            if (privacy1 == null)
            {
                rootDict.SetString("NSLocationWhenInUseUsageDescription", "Some ad content may require access to the location for an interactive ad experience.");
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
            if (!ContainsUnityIphoneTargetInPodfile(buildPath))
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
                dynamicLibraryPathsToEmbed.Add(Path.Combine("Pods/", "FBSDKCoreKit_Basics/XCFrameworks/FBSDKCoreKit_Basics.xcframework"));
                return dynamicLibraryPathsToEmbed;
            }
        }
    }
}
#endif