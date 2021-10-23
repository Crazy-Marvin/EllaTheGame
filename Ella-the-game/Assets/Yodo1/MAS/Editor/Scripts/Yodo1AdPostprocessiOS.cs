using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Yodo1.MAS
{
    public class Yodo1SkAdNetworkData
    {
        [SerializeField] public string[] SkAdNetworkIds;
    }
    public class Yodo1PostProcessiOS
    {
        [PostProcessBuild(int.MaxValue)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.iOS)
            {
#if UNITY_IOS
                Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();
                if (CheckConfiguration_iOS(settings))
                {
                    UpdateIOSPlist(pathToBuiltProject, settings);
                    UpdateIOSProject(pathToBuiltProject);
                    // CopyDirectory("Assets/Yodo1/MAS/Command/", pathToBuiltProject, null);
                    // StartPodsProcess(pathToBuiltProject, "open_repo_update.command");

                    var podVersion = Yodo1AdCommandLine.Run("pod", "--version", pathToBuiltProject);
                    if (podVersion.ExitCode != 0)
                    {
                        Debug.LogError("Cocoapods is not installed, " + podVersion.StandardOutput + "," + podVersion.StandardError);
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
#endif
            }
        }

        public static bool CheckConfiguration_iOS(Yodo1AdSettings settings)
        {
            if (settings == null)
            {
                string message = "MAS iOS settings is null, please check the configuration.";
                Debug.LogError("[Yodo1 Mas] " + message);
                Yodo1AdUtils.ShowAlert("Error", message, "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(settings.iOSSettings.AppKey.Trim()))
            {
                string message = "MAS iOS AppKey is null, please check the configuration.";
                Debug.LogError("[Yodo1 Mas] " + message);
                Yodo1AdUtils.ShowAlert("Error", message, "Ok");
                return false;
            }

            if (settings.iOSSettings.GlobalRegion && string.IsNullOrEmpty(settings.iOSSettings.AdmobAppID.Trim()))
            {
                string message = "MAS iOS AdMob App ID is null, please check the configuration.";
                Debug.LogError("[Yodo1 Mas] " + message);
                Yodo1AdUtils.ShowAlert("Error", message, "Ok");
                return false;
            }
            return true;
        }

#if UNITY_IOS
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
                Debug.LogError("Failed to retrieve SKAdNetwork IDs with error: " + unityWebRequest.error);
                return new Yodo1SkAdNetworkData();
            }
            try
            {
                return JsonUtility.FromJson<Yodo1SkAdNetworkData>(unityWebRequest.downloadHandler.text);
            }
            catch (Exception exception)
            {
                Debug.LogError("Failed to parse data '" + unityWebRequest.downloadHandler.text + "' with exception: " + exception);
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

            //Set AdMob APP Id
            if (settings.iOSSettings.GlobalRegion)
            {
                rootDict.SetString("GADApplicationIdentifier", settings.iOSSettings.AdmobAppID);
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

        private static void UpdateIOSProject(string path)
        {
            PBXProject proj = new PBXProject();
            string projPath = PBXProject.GetPBXProjectPath(path);
            proj.ReadFromFile(projPath);

            string mainTargetGuid = string.Empty;
            string unityFrameworkTargetGuid = string.Empty;

#if UNITY_2019_3_OR_NEWER
            mainTargetGuid = proj.GetUnityMainTargetGuid();
            unityFrameworkTargetGuid = proj.GetUnityFrameworkTargetGuid();
            //string frameworksPath = path + "/Frameworks/Plugins/iOS/Yodo1Ads/";
            //string[] directories = Directory.GetDirectories(frameworksPath, "*.bundle", SearchOption.AllDirectories);
            //for (int i = 0; i < directories.Length; i++)
            //{
            //    var dirPath = directories[i];
            //    var suffixPath = dirPath.Substring(path.Length + 1);
            //    var fileGuid = proj.AddFile(suffixPath, suffixPath);
            //    proj.AddFileToBuild(mainTargetGuid, fileGuid);

            //    fileGuid = proj.FindFileGuidByProjectPath(suffixPath);
            //    if (fileGuid != null)
            //    {
            //        proj.RemoveFileFromBuild(unityFrameworkTargetGuid, fileGuid);
            //    }
            //}
#else
            mainTargetGuid = proj.TargetGuidByName("Unity-iPhone");
            unityFrameworkTargetGuid = mainTargetGuid;
#endif

#if UNITY_2019_3_OR_NEWER
            var unityFrameworkGuid = proj.FindFileGuidByProjectPath("UnityFramework.framework");
            if (unityFrameworkGuid == null)
            {
                unityFrameworkGuid = proj.AddFile("UnityFramework.framework", "UnityFramework.framework");
                proj.AddFileToBuild(mainTargetGuid, unityFrameworkGuid);
            }
            proj.AddFrameworkToProject(mainTargetGuid, "UnityFramework.framework", false);
            proj.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");
#endif

            proj.SetBuildProperty(unityFrameworkTargetGuid, "ENABLE_BITCODE", "NO");
            proj.SetBuildProperty(unityFrameworkTargetGuid, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
            // rewrite to file
            File.WriteAllText(projPath, proj.WriteToString());
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
                Debug.LogError("Failed to install AppLovin Quality Service plugin. AppLovin SDK Key is empty");
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
                Debug.LogError("AppLovin Quality Service installation failed. Failed to download script with error: " + unityWebRequest.error);
                return;
            }

            // Check if Ruby is installed
            var rubyVersion = Yodo1AdCommandLine.Run("ruby", "--version", buildPath);
            if (rubyVersion.ExitCode != 0)
            {
                Debug.LogError("AppLovin Quality Service installation requires Ruby. Please install Ruby, export it to your system PATH and re-export the project.");
                return;
            }

            // Ruby is installed, run `ruby AppLovinQualityServiceSetup.rb`
            var result = Yodo1AdCommandLine.Run("ruby", OutputFileName, buildPath);

            // Check if we have an error.
            if (result.ExitCode != 0) Debug.LogError("Failed to set up AppLovin Quality Service");

            Debug.Log(result.Message);
        }
#endif
    }
}