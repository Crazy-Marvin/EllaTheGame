using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Yodo1.MAS;
[InitializeOnLoad]
public class MASAutoUpdateManager : MonoBehaviour
{
    static Yodo1AdNetworkConfig adNetworkConfig;
    private static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly int SecondsInADay = (int)TimeSpan.FromDays(1).TotalSeconds;
    static MASAutoUpdateManager()
    {
        var now = (int)(DateTime.UtcNow - EpochTime).TotalSeconds;
        if (EditorPrefs.HasKey("MASLastUpdateCheckTime"))
        {
            var elapsedTime = now - EditorPrefs.GetInt("MASLastUpdateCheckTime");

            // Check if we have checked for a new version in the last 24 hrs and skip update if we have.
            if (elapsedTime < SecondsInADay) return;
        }

        // Update last checked time.
        EditorPrefs.SetInt("MASLastUpdateCheckTime", now);
        if (!EditorPrefs.GetBool("MASSDKAutoUpdate", true)) return;

#if UNITY_ANDROID
        if (!Yodo1AdUtils.IsGooglePlayVersion())
        {
            return;
        }
#endif
        Yodo1AdNetworkManager.GetInstance().InitAdNetworkConfig();
        adNetworkConfig = Yodo1AdNetworkManager.GetInstance().GetAdNetworkConfig();
        if (CompareVersions(Yodo1AdNetworkManager.GetInstance().GetCurMakSdkVersion(), adNetworkConfig.latestSdkversion) == -1)
        {
            int option = EditorUtility.DisplayDialogComplex("Yodo1 MAS SDK Update",
             "A new version of MAS SDK is available for download. Update now?",
             "Download",
             "Not Now",
             "Don't Ask Again");
            switch (option)
            {
                case 0:
                    EditorCoroutineRunner.StartEditorCoroutine(DownloadPlugin(adNetworkConfig.sdkDownloadUrl, PackageName));
                    break;
                case 1:

                    break;
                case 2:
                    EditorPrefs.SetBool("MASSDKAutoUpdate", false);
                    break;

                default:
                    break;
            }
        }

    }
    private static string PackageName = string.Empty;
    private static UnityWebRequest webRequest;
    public static IEnumerator DownloadPlugin(string downloadUrl, string Version)
    {
        var path = Path.Combine(Application.temporaryCachePath, Version + ".unitypackage");
        var downloadHandler = new DownloadHandlerFile(path);
        webRequest = new UnityWebRequest(downloadUrl)
        {
            method = UnityWebRequest.kHttpVerbGET,
            downloadHandler = downloadHandler
        };
        var operation = webRequest.SendWebRequest();
        while (!operation.isDone)
        {
            yield return new WaitForSeconds(0.1f); // Just wait till webRequest is completed. Our coroutine is pretty rudimentary.
                                                   //CallDownloadPluginProgressCallback(network.DisplayName, operation.progress, operation.isDone);
        }

#if UNITY_2020_1_OR_NEWER
            if (webRequest.result != UnityWebRequest.Result.Success)
#elif UNITY_2017_2_OR_NEWER
        if (webRequest.isNetworkError || webRequest.isHttpError)
#else
            if (webRequest.isError)
#endif
        {
            //Debug.LogError(webRequest.error);
        }
        else
        {
            AssetDatabase.ImportPackage(path, true);
        }

        webRequest.Dispose();
        webRequest = null;
    }
    private static int CompareVersions(string versionA, string versionB)
    {
        if (versionA.Equals(versionB)) return 0;

        // Check if either of the versions are beta versions. Beta versions could be of format x.y.z-beta or x.y.z-betaX.
        // Split the version string into beta component and the underlying version.
        int piece;
        var isVersionABeta = versionA.Contains("-beta");
        var versionABetaNumber = 0;
        if (isVersionABeta)
        {
            var components = versionA.Split(new[] { "-beta" }, StringSplitOptions.None);
            versionA = components[0];
            if (components[1].Contains("."))
            {
                components[1] = components[1].Replace(".", string.Empty);
            }
            versionABetaNumber = int.TryParse(components[1], out piece) ? piece : 0;
        }
        if (!string.IsNullOrEmpty(versionB))
        {
            var isVersionBBeta = versionB.Contains("-beta");
            var versionBBetaNumber = 0;
            if (isVersionBBeta)
            {
                var components = versionB.Split(new[] { "-beta" }, StringSplitOptions.None);
                versionB = components[0];
                if (components[1].Contains("."))
                {
                    components[1] = components[1].Replace(".", string.Empty);
                }
                versionBBetaNumber = int.TryParse(components[1], out piece) ? piece : 0;
            }

            // Now that we have separated the beta component, check if the underlying versions are the same.
            if (versionA.Equals(versionB))
            {
                // The versions are the same, compare the beta components.
                if (isVersionABeta && isVersionBBeta)
                {
                    if (versionABetaNumber < versionBBetaNumber) return -1;

                    if (versionABetaNumber > versionBBetaNumber) return 1;
                }
                // Only VersionA is beta, so A is older.
                else if (isVersionABeta)
                {
                    return -1;
                }
                // Only VersionB is beta, A is newer.
                else
                {
                    return 1;
                }
            }

            // Compare the non beta component of the version string.
            var versionAComponents = versionA.Split('.').Select(version => int.TryParse(version, out piece) ? piece : 0).ToArray();
            var versionBComponents = versionB.Split('.').Select(version => int.TryParse(version, out piece) ? piece : 0).ToArray();
            var length = Mathf.Max(versionAComponents.Length, versionBComponents.Length);
            for (var i = 0; i < length; i++)
            {
                var aComponent = i < versionAComponents.Length ? versionAComponents[i] : 0;
                var bComponent = i < versionBComponents.Length ? versionBComponents[i] : 0;

                if (aComponent < bComponent) return -1;

                if (aComponent > bComponent) return 1;
            }
        }

        return 0;
    }
}
