using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Yodo1.MAS;

[InitializeOnLoad]
public class IntegrationManager : EditorWindow
{
    int platformTabSelected = 0;
    int prevPlatformTabSelected = 0;
    private const float editorWindowMinWidth = 600f;
    private const float editorWindowMinLength = 800f;
    private const float networkFieldMinWidth = 100f;
    private const float versionFieldMinWidth = 190f;
    private const float actionFieldWidth = 60f;
    private static GUILayoutOption networkWidthOption = GUILayout.Width(networkFieldMinWidth);
    private static GUILayoutOption versionWidthOption = GUILayout.Width(versionFieldMinWidth);
    private static readonly GUILayoutOption fieldWidth = GUILayout.Width(actionFieldWidth);
    private GUIStyle headerLabelStyle;
    private GUIStyle contentLabelStyle;

    Yodo1AdNetworkConfig adNetworkConfig;

    List<Yodo1AdNetwork> androidStandard;
    List<Yodo1AdNetwork> iosStandard;

    Yodo1AdNetworkConfigCacheData androidCachedData;
    Yodo1AdNetworkConfigCacheData iosCachedData;
    float SDKSize = 0f;
    private static string PackageName = string.Empty;

    static IntegrationManager()
    {
        AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
    }
    private static void OnImportPackageCompleted(string packagename)
    {
        if (packagename.Contains("Rivendell"))
        {
#if UNITY_ANDROID
            if (!Yodo1AdUtils.IsGooglePlayVersion())
            {
                return;
            }
#endif
            Yodo1AdNetworkManager.GetInstance().InitAdNetworkConfig();
            Yodo1AdNetworkManager.GetInstance().CheckDependenciesFileByCachedAdNetworks();
        }
    }

    [MenuItem("Yodo1/MAS/Integration Manager", false, 100)]
    static void Init()
    {
        IntegrationManager window = (IntegrationManager)EditorWindow.GetWindow(typeof(IntegrationManager), true, "Yodo1 Integration Manager");
        window.minSize = new Vector2(editorWindowMinWidth, editorWindowMinLength);
        window.maxSize = window.minSize;
        window.Show();
    }
    [MenuItem("Yodo1/MAS/Integration Manager", true, 100)]
    static bool ValidateInit()
    {
#if UNITY_ANDROID
        return Yodo1AdUtils.IsGooglePlayVersion();
#else
        return true;
#endif
    }
    private void Awake()
    {
        headerLabelStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            fixedHeight = 18

        };
        contentLabelStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            fixedHeight = 18,
            alignment = TextAnchor.MiddleCenter
        };
        EditorCoroutineRunner.StartEditorCoroutine(LoadPluginData(result =>
        {
            if (result)
            {
                Repaint();
            }
        }));
    }
    IEnumerator LoadPluginData(Action<bool> callback)
    {
        Yodo1AdNetworkManager.GetInstance().InitAdNetworkConfig();
        adNetworkConfig = Yodo1AdNetworkManager.GetInstance().GetAdNetworkConfig();

        // get the dispalyed networks list
        androidStandard = adNetworkConfig.androidStandard;
        iosStandard = adNetworkConfig.iosStandard;
        // get the cached networks the developer selected before

        androidCachedData = Yodo1AdNetworkManager.GetInstance().GetCachedAndroidAdNetworksInfo();
        iosCachedData = Yodo1AdNetworkManager.GetInstance().GetCachedIOSAdNetworksInfo();
        GetSDKSize();
        callback(true);
        yield return null;

    }
    private string CurrentAdNetworkVersion()
    {
        return Yodo1AdNetworkManager.GetInstance().GetCurMakSdkVersion();
    }
    private string LatestAdNetworkVersion()
    {
        if (adNetworkConfig == null)
        {
            return string.Empty;
        }
        else
        {
            return adNetworkConfig.latestSdkversion;
        }
    }
    public string GetUpgradeDownloadUrl()
    {
        if (adNetworkConfig == null)
        {
            return string.Empty;
        }
        else
        {
            return adNetworkConfig.sdkDownloadUrl;
        }
    }
    private void UpgradeButtonClicked()
    {
        var PackageComponents = GetUpgradeDownloadUrl().Split(new[] { ".unitypackage" }, StringSplitOptions.None);
        PackageName = PackageComponents[0].Substring(PackageComponents[0].LastIndexOf("/") + 1);


        if (PackageName.Contains("-"))
        {
            var components = PackageName.Split(new[] { "-beta" }, StringSplitOptions.None);
            PackageName = components[0];
        }
        EditorCoroutineRunner.StartEditorCoroutine(DownloadPlugin(GetUpgradeDownloadUrl(), PackageName));
    }
    private UnityWebRequest webRequest;
    public IEnumerator DownloadPlugin(string downloadUrl, string Version)
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

    private bool CheckIfNetworkIsInstalled(Yodo1AdNetwork adNetwork)
    {
        bool returnVal = false;
        if (platformTabSelected == 0)
        {
            if (androidCachedData != null)
            {
                if (androidCachedData.networks.Count >= 1)
                {
                    foreach (string network in androidCachedData.networks)
                    {
                        if (string.Equals(network, adNetwork.name))
                        {
                            returnVal = true;
                        }
                    }
                }
                else
                {
                    returnVal = true;
                }
            }
        }
        else
        {
            if (iosCachedData != null)
            {
                if (iosCachedData.networks.Count >= 1)
                {
                    foreach (string network in iosCachedData.networks)
                    {
                        if (string.Equals(network, adNetwork.name))
                        {
                            returnVal = true;
                        }
                    }
                }
                else
                {
                    returnVal = true;
                }
            }
        }
        return returnVal;
    }
    private void RemoveAdNetwork(Yodo1AdNetwork adNetwork)
    {

        if (platformTabSelected == 0)
        {
            if (androidCachedData.networks.Count >= 1)
            {
                androidCachedData.networks.Remove(adNetwork.name);
                Yodo1AdNetworkManager.GetInstance().UpdateAdNetworksInfo(androidCachedData);
                androidCachedData = Yodo1AdNetworkManager.GetInstance().GetCachedAndroidAdNetworksInfo();
            }
            else
            {
                List<string> installedList = new List<string>();
                foreach (Yodo1AdNetwork network in androidStandard)
                {
                    if (!string.Equals(network.name, adNetwork.name))
                    {
                        installedList.Add(network.name);
                    }
                }

                Yodo1AdNetworkConfigCacheData data = new Yodo1AdNetworkConfigCacheData();
                data.sdkType = SDKGroupType.AndroidStandard;
                data.sdkVersion = adNetworkConfig.sdkVersion;
                data.latestSdkVersion = adNetworkConfig.latestSdkversion;
                data.networks = installedList;
                Yodo1AdNetworkManager.GetInstance().UpdateAdNetworksInfo(data);
                androidCachedData = Yodo1AdNetworkManager.GetInstance().GetCachedAndroidAdNetworksInfo();
            }
        }
        else
        {
            if (iosCachedData.networks.Count >= 1)
            {
                iosCachedData.networks.Remove(adNetwork.name);
                Yodo1AdNetworkManager.GetInstance().UpdateAdNetworksInfo(iosCachedData);
                iosCachedData = Yodo1AdNetworkManager.GetInstance().GetCachedIOSAdNetworksInfo();
            }
            else
            {
                List<string> installedList = new List<string>();
                foreach (Yodo1AdNetwork network in iosStandard)
                {
                    if (!string.Equals(network.name, adNetwork.name))
                    {
                        installedList.Add(network.name);
                    }
                }

                Yodo1AdNetworkConfigCacheData data = new Yodo1AdNetworkConfigCacheData();
                data.sdkType = SDKGroupType.IOSStandard;
                data.sdkVersion = adNetworkConfig.sdkVersion;
                data.latestSdkVersion = adNetworkConfig.latestSdkversion;
                data.networks = installedList;
                Yodo1AdNetworkManager.GetInstance().UpdateAdNetworksInfo(data);
                iosCachedData = Yodo1AdNetworkManager.GetInstance().GetCachedIOSAdNetworksInfo();
            }
        }
        GetSDKSize();
        Repaint();
    }
    private void InstallAdNetwork(Yodo1AdNetwork adNetwork)
    {

        if (platformTabSelected == 0)
        {
            androidCachedData.networks.Add(adNetwork.name);
            Yodo1AdNetworkManager.GetInstance().UpdateAdNetworksInfo(androidCachedData);
            androidCachedData = Yodo1AdNetworkManager.GetInstance().GetCachedAndroidAdNetworksInfo();
        }
        else
        {
            iosCachedData.networks.Add(adNetwork.name);
            Yodo1AdNetworkManager.GetInstance().UpdateAdNetworksInfo(iosCachedData);
            iosCachedData = Yodo1AdNetworkManager.GetInstance().GetCachedIOSAdNetworksInfo();
        }
        GetSDKSize();
        Repaint();
    }
    private float GetSDKSize()
    {
        SDKSize = 0f;
        if (platformTabSelected == 0)
        {
            if (androidStandard != null)
            {
                for (int i = 0; i < androidStandard.Count; i++)
                {
                    if (CheckIfNetworkIsInstalled(androidStandard[i]))
                    {
                        SDKSize += androidStandard[i].size;
                    }
                }
                SDKSize += 0.3f;//Amazon Android SDK size
            }
        }
        return SDKSize;
    }
    void OnGUI()
    {

        GUILayout.Space(10);
        DrawPluginDetails();
        GUIUtility.ExitGUI();
    }
    private void DrawPluginDetails()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        using (new EditorGUILayout.VerticalScope("box"))
        {
            DrawHeaders();
            DrawPluginDetailRow("Standard", CurrentAdNetworkVersion(), LatestAdNetworkVersion());
        }


        GUILayout.Space(5);
        GUILayout.EndHorizontal();

        platformTabSelected = GUILayout.Toolbar(platformTabSelected, new string[] { "Android", "iOS" });
        if (platformTabSelected != prevPlatformTabSelected)
        {
            GetSDKSize();
            prevPlatformTabSelected = platformTabSelected;
        }

        GUILayout.Space(10);
        GUILayout.Label("Mediation network details", headerLabelStyle);
        GUILayout.Space(10);
        using (new EditorGUILayout.VerticalScope("box"))
        {
            DrawHeaderNetworks();
            GUILayout.Space(5);
            if (adNetworkConfig != null)
            {
                if (platformTabSelected == 0)
                {
                    if (adNetworkConfig.android != null)
                    {
                        for (int i = 0; i < adNetworkConfig.android.Length; i++)
                        {
                            DrawNetworkDetailRow(adNetworkConfig.android[i]);
                        }
                        Yodo1AdNetwork adNetwork = new Yodo1AdNetwork();
                        adNetwork.name = "Amazon";
                        adNetwork.version = "9.8.4";
                        DrawNetworkDetailRow(adNetwork);
                    }
                }
                else
                {
                    if (adNetworkConfig.ios != null)
                    {
                        for (int i = 0; i < adNetworkConfig.ios.Length; i++)
                        {
                            DrawNetworkDetailRow(adNetworkConfig.ios[i]);
                        }
                        Yodo1AdNetwork adNetwork = new Yodo1AdNetwork();
                        adNetwork.name = "Amazon";
                        adNetwork.version = "4.7.5";
                        DrawNetworkDetailRow(adNetwork);
                    }
                }

            }
        }

        GUILayout.Space(40);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            if (platformTabSelected == 0)
            {
                EditorGUILayout.LabelField(new GUIContent("Current Size : " + SDKSize + " MB"), contentLabelStyle);
            }
            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();
    }

    private void DrawPluginDetailRow(string platform, string currentVersion, string latestVersion)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(5);
            EditorGUILayout.LabelField(new GUIContent(platform), networkWidthOption);
            EditorGUILayout.LabelField(new GUIContent(currentVersion), versionWidthOption);
            GUILayout.Space(3);
            if (currentVersion.Contains("beta") || currentVersion.Contains("alpha"))
            {
                return;
            }
            EditorGUILayout.LabelField(new GUIContent(latestVersion), versionWidthOption);
            GUILayout.Space(3);
            if (CompareVersions(currentVersion, latestVersion) == -1)
            {
                if (GUILayout.Button(new GUIContent("Upgrade"), fieldWidth))
                {
                    UpgradeButtonClicked();
                }
            }
            else
            {
                GUI.enabled = false;
                if (GUILayout.Button(new GUIContent("Upgrade"), fieldWidth))
                {
                }
                GUI.enabled = true;
            }

        }

        GUILayout.Space(4);
    }
    private void DrawNetworkDetailRow(Yodo1AdNetwork adNetwork)
    {
        using (new EditorGUILayout.HorizontalScope())
        {

            GUILayout.Space(5);
            EditorGUILayout.LabelField(new GUIContent(GetDisplayName(adNetwork)), versionWidthOption);
            EditorGUILayout.LabelField(new GUIContent(adNetwork.version), versionWidthOption);
            GUILayout.Space(3);
            ChangeButtonStatus(adNetwork);
            GUILayout.Space(3);
        }
        GUILayout.Space(15);
    }
    private void ChangeButtonStatus(Yodo1AdNetwork adNetwork)
    {

        bool contains = adNetwork.name.IndexOf("APPLOVIN", StringComparison.OrdinalIgnoreCase) >= 0 || adNetwork.name.IndexOf("ADMOB", StringComparison.OrdinalIgnoreCase) >= 0 || adNetwork.name.IndexOf("AMAZON", StringComparison.OrdinalIgnoreCase) >= 0;
        if (contains)
        {
            GUI.enabled = false;
            GUILayout.Button(new GUIContent("Remove"), fieldWidth);
            GUI.enabled = true;
            return;
        }
        if (CheckIfNetworkIsInstalled(adNetwork))
        {
            if (GUILayout.Button(new GUIContent("Remove"), fieldWidth))
            {
                string displayName = GetDisplayName(adNetwork);
                bool selection = EditorUtility.DisplayDialog("Remove " + displayName, "Are you sure you want to remove " + displayName + "? This will impact REVENUE.", "Do Not Remove", "Remove");
                if (!selection)
                {
                    RemoveAdNetwork(adNetwork);
                    ChangeButtonStatus(adNetwork);
                }

            }
        }
        else
        {
            var iconPath = "Assets/Yodo1/MAS/Editor/Resources/asset1.png";
            var icon = AssetDatabase.LoadAssetAtPath(iconPath, typeof(Texture)) as Texture;
            if (GUILayout.Button(new GUIContent("Install"), fieldWidth))
            {
                InstallAdNetwork(adNetwork);
                ChangeButtonStatus(adNetwork);

            }
            GUILayout.Label(new GUIContent(icon), contentLabelStyle, GUILayout.Width(25f));
        }
    }
    private string UpperFirst(string text)
    {
        return char.ToUpper(text[0]) + ((text.Length > 1) ? text.Substring(1).ToLower() : string.Empty);
    }
    private string GetDisplayName(Yodo1AdNetwork adNetwork)
    {
        return string.IsNullOrEmpty(adNetwork.displayName) ? UpperFirst(adNetwork.name) : adNetwork.displayName;

    }
    private void DrawHeaders()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Type", headerLabelStyle, networkWidthOption);
            EditorGUILayout.LabelField("Version", headerLabelStyle, versionWidthOption);
            GUILayout.Space(3);
            EditorGUILayout.LabelField("Latest Version", headerLabelStyle, versionWidthOption);
            GUILayout.Space(3);
            GUILayout.Button("Actions", headerLabelStyle, fieldWidth);
            GUILayout.Space(5);
        }

        GUILayout.Space(4);
    }
    private void DrawHeaderNetworks()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Network", headerLabelStyle, versionWidthOption);
            EditorGUILayout.LabelField("Version", headerLabelStyle, versionWidthOption);
            GUILayout.Space(3);
            GUILayout.Button("Actions", headerLabelStyle, fieldWidth);
            GUILayout.Space(5);
        }

        GUILayout.Space(4);
    }

    private int CompareVersions(string versionA, string versionB)
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
