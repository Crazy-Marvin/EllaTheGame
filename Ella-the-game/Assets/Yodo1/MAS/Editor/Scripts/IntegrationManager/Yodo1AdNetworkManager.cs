using System;
using System.Collections.Generic;
using Yodo1.MAS;
using UnityEngine;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using System.Xml;

public class Yodo1AdNetworkManager
{
    private static readonly string TARGET_PATH = Path.GetFullPath(".") + "/Assets/Yodo1/MAS/Editor/Dependencies/";
    private static readonly string VERSION_PATH = Path.GetFullPath(".") + "/Assets/Yodo1/MAS/version.xml";
    public Yodo1AdNetworkConfig yodo1AdNetworkConfig;

    public Yodo1AdNetworkManager()
    {
    }

    private static class HelperHolder
    {
        public static Yodo1AdNetworkManager Helper = new Yodo1AdNetworkManager();
    }

    public static Yodo1AdNetworkManager GetInstance()
    {
        return HelperHolder.Helper;
    }

    public void InitAdNetworkConfig()
    {

        string curSDKVersion = Yodo1AdNetworkUtil.TetCurSDKVersion(VERSION_PATH);
        yodo1AdNetworkConfig = GetAdNetworkConfigFromServer(curSDKVersion);

    }

    public Yodo1AdNetworkConfig GetAdNetworkConfig()
    {
        return yodo1AdNetworkConfig;
    }

    public string GetCurMakSdkVersion()
    {
        return Yodo1AdNetworkUtil.TetCurSDKVersion(VERSION_PATH);
    }

    public void CheckDependenciesFileByCachedAdNetworks()
    {
        Yodo1AdNetworkConfigCacheData androidData = GetCachedAndroidAdNetworksInfo();
        Yodo1AdNetworkConfigCacheData iosData = GetCachedIOSAdNetworksInfo();
        if(androidData != null && androidData.networks != null && androidData.networks.Count > 0)
        {
            UpdateAndroidDependenciesFile(androidData.sdkType, androidData.networks);
        }
        if(iosData != null && iosData.networks != null && iosData.networks.Count > 0)
        {
            UpdateIOSDependenciesFile(iosData.sdkType, iosData.networks);
        }
    }

    /**
     * get the previously selected android networks information
     */
    public Yodo1AdNetworkConfigCacheData GetCachedAndroidAdNetworksInfo()
    {
        Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();

        Yodo1AdNetworkConfigCacheData data = new Yodo1AdNetworkConfigCacheData();
        data.sdkType = FormatIntToSDKType(settings.androidDynamicNetworkSettings.sdkType);
        data.sdkVersion = settings.androidDynamicNetworkSettings.sdkVersion;
        data.latestSdkVersion = settings.androidDynamicNetworkSettings.latestSdkVersion;
        data.networks = settings.androidDynamicNetworkSettings.networks;
        return data;
    }

    /**
    * get the previously selected ios networks information
    */
    public Yodo1AdNetworkConfigCacheData GetCachedIOSAdNetworksInfo()
    {
        Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();
        Yodo1AdNetworkConfigCacheData data = new Yodo1AdNetworkConfigCacheData();
        data.sdkType = FormatIntToSDKType(settings.iosDynamicNetworkSettings.sdkType);
        data.sdkVersion = settings.iosDynamicNetworkSettings.sdkVersion;
        data.latestSdkVersion = settings.iosDynamicNetworkSettings.latestSdkVersion;
        data.networks = settings.iosDynamicNetworkSettings.networks;

        return data;
    }

    /**
     * call this method when developer changed the networks in the windows
     */
    public void UpdateAdNetworksInfo(Yodo1AdNetworkConfigCacheData data)
    {
        if (data == null || data.networks == null || data.networks.Count == 0)
        {
            Debug.LogError(Yodo1U3dMas.TAG + ": need selected networks!");
            return;
        }
        UpdateLocalCache(data.sdkType, data.sdkVersion, data.latestSdkVersion, data.networks);
        UpdateDependenciesFile(data.sdkType, data.networks);
    }

    private void UpdateLocalCache(SDKGroupType sdkType, string sdkVersion, string latestSdkVersion, List<string> list)
    {
        Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();

        if(SDKGroupType.AndroidStandard == sdkType || SDKGroupType.AndroidFamily == sdkType)
        {
            settings.androidDynamicNetworkSettings.sdkType = ((int)sdkType);
            settings.androidDynamicNetworkSettings.sdkVersion = sdkVersion;
            settings.androidDynamicNetworkSettings.latestSdkVersion = latestSdkVersion;
            settings.androidDynamicNetworkSettings.networks = list;
        }
        else if(SDKGroupType.IOSStandard == sdkType)
        {
            settings.iosDynamicNetworkSettings.sdkType = ((int)sdkType);
            settings.iosDynamicNetworkSettings.sdkVersion = sdkVersion;
            settings.iosDynamicNetworkSettings.latestSdkVersion = latestSdkVersion;
            settings.iosDynamicNetworkSettings.networks = list;
        }

        Yodo1AdSettingsSave.Save(settings);
    }

    private void UpdateDependenciesFile(SDKGroupType sdkType, List<string> list)
    {
        if(SDKGroupType.AndroidStandard == sdkType || SDKGroupType.AndroidFamily == sdkType)
        {
            UpdateAndroidDependenciesFile(sdkType, list);
        }
        else if(SDKGroupType.IOSStandard == sdkType)
        {
            UpdateIOSDependenciesFile(sdkType, list);
        }
    }

    private void UpdateIOSDependenciesFile(SDKGroupType sdkType, List<string> list)
    {
        //Debug.Log(string.Format("[Yodo1 Mas] Update iOS Dependencies network count : " + (list != null ? list.Count : 0)));
        if(yodo1AdNetworkConfig == null || yodo1AdNetworkConfig.ios == null || yodo1AdNetworkConfig.ios.Length == 0)
        {
            Debug.LogError(Yodo1U3dMas.TAG + ": updateIOSDependenciesFile yodo1AdNetworkConfig is null or ios data is null ");
            return;
        }
        string name = "Yodo1MasiOSDependencies.xml";
        if (!string.IsNullOrEmpty(name))
        {
            string destFile = TARGET_PATH + name;
            if (File.Exists(destFile))
            {
                File.Delete(destFile);
                File.Delete(destFile + ".meta");
            }

            List<Yodo1AdNetwork> yodo1AdNetworks = new List<Yodo1AdNetwork>();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(VERSION_PATH, settings);

            XmlDocument xmlReadDoc = new XmlDocument();
            xmlReadDoc.Load(VERSION_PATH);
            XmlNode xnRead = xmlReadDoc.SelectSingleNode("versions");


            XmlDocument xmlWriteDoc = new XmlDocument();
            XmlDeclaration Declaration = xmlWriteDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            XmlNode dependencies = xmlWriteDoc.CreateElement("dependencies");

            XmlElement iosNode = (XmlElement)xnRead.SelectSingleNode("ios");
            string env = iosNode.GetAttribute("env").ToString();
            string version = iosNode.GetAttribute("version").ToString();
     
            if (SDKGroupType.IOSStandard == sdkType)
            {
                yodo1AdNetworks = yodo1AdNetworkConfig.iosStandard;
            }


            Dictionary<string, Yodo1AdNetwork> networks = new Dictionary<string, Yodo1AdNetwork>();
            foreach (Yodo1AdNetwork adNetwork in yodo1AdNetworks)
            {
                networks.Add(adNetwork.name, adNetwork);
            }

                #region filter data
                bool maxMediation = false, admobMediation = false, ironsourcemediation = false;
            foreach (string networkName in list)
            {
                if (string.Equals("APPLOVIN", networkName))
                {
                    maxMediation = true;
                }
                if (string.Equals("ADMOB", networkName))
                {
                    admobMediation = true;
                }
                if (string.Equals("IRONSOURCE", networkName))
                {
                    ironsourcemediation = true;
                }
            }

            List<string> dependencyList = new List<string>();
            foreach (string networkName in list)
            {
                if (!networks.ContainsKey(networkName)) continue;
                Yodo1AdNetwork adNetwork = networks[networkName];

                dependencyList.Add(adNetwork.dependency);
                if (admobMediation && !string.IsNullOrEmpty(adNetwork.admobAdapterDependency))
                {
                    dependencyList.Add(adNetwork.admobAdapterDependency);
                }
                if (maxMediation && !string.IsNullOrEmpty(adNetwork.applovinAdapterDependency))
                {
                    dependencyList.Add(adNetwork.applovinAdapterDependency);
                }
                if (ironsourcemediation && !string.IsNullOrEmpty(adNetwork.ironsourceAdapterDependency))
                {
                    dependencyList.Add(adNetwork.ironsourceAdapterDependency);
                }
            }
            #endregion

            #region create ios dependencies
            XmlNode iosPods = xmlWriteDoc.CreateElement("iosPods");
            dependencies.AppendChild(iosPods);

            XmlNode sources = xmlWriteDoc.CreateElement("sources");
            iosPods.AppendChild(sources);

            XmlNode masSource = xmlWriteDoc.CreateElement("source");
            if (env.Equals("Dev"))
            {
                masSource.InnerText = "https://github.com/Yodo1Games/MAS-Spec-Dev.git";
            }
            else if (env.Equals("Pre"))
            {
                masSource.InnerText = "https://github.com/Yodo1Games/MAS-Spec-Pre.git";
            }
            else
            {
                masSource.InnerText = "https://github.com/Yodo1Games/MAS-Spec.git";
            }

            sources.AppendChild(masSource);

            foreach(string network in dependencyList)
            {
                XmlNode iosPod = xmlWriteDoc.CreateElement("iosPod");
                iosPods.AppendChild(iosPod);

                XmlAttribute iosPodNameAttribute = xmlWriteDoc.CreateAttribute("name");
                iosPodNameAttribute.Value = network;
                iosPod.Attributes.Append(iosPodNameAttribute);

                XmlAttribute iosPodVersionAttribute = xmlWriteDoc.CreateAttribute("version");
                iosPodVersionAttribute.Value = version;
                iosPod.Attributes.Append(iosPodVersionAttribute);

                XmlAttribute iosPodMinTargetSdkAttribute = xmlWriteDoc.CreateAttribute("minTargetSdk");
                iosPodMinTargetSdkAttribute.Value = "12.0";
                iosPod.Attributes.Append(iosPodMinTargetSdkAttribute);
            }

            XmlNode iosPodYodo1 = xmlWriteDoc.CreateElement("iosPod");
            iosPods.AppendChild(iosPodYodo1);

            XmlAttribute iosPodYodo1NameAttribute = xmlWriteDoc.CreateAttribute("name");
            iosPodYodo1NameAttribute.Value = "Yodo1MasMediationYodo1";
            iosPodYodo1.Attributes.Append(iosPodYodo1NameAttribute);

            XmlAttribute iosPodYodo1VersionAttribute = xmlWriteDoc.CreateAttribute("version");
            iosPodYodo1VersionAttribute.Value = version;
            iosPodYodo1.Attributes.Append(iosPodYodo1VersionAttribute);

            XmlAttribute iosPodYodo1MinTargetSdkAttribute = xmlWriteDoc.CreateAttribute("minTargetSdk");
            iosPodYodo1MinTargetSdkAttribute.Value = "12.0";
            iosPodYodo1.Attributes.Append(iosPodYodo1MinTargetSdkAttribute);

            #endregion

            reader.Close();
            //附加根节点
            xmlWriteDoc.AppendChild(dependencies);
            xmlWriteDoc.InsertBefore(Declaration, xmlWriteDoc.DocumentElement);
            xmlWriteDoc.Save(destFile);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }

    private void UpdateAndroidDependenciesFile(SDKGroupType sdkType, List<string> list)
    {
        //Debug.Log(string.Format("[Yodo1 Mas] Update Android Dependencies network count : " + (list != null ? list.Count : 0)));
        if(yodo1AdNetworkConfig == null || yodo1AdNetworkConfig.android == null || yodo1AdNetworkConfig.android.Length == 0)
        {
            Debug.LogError(Yodo1U3dMas.TAG + ": updateAndroidDependenciesFile yodo1AdNetworkConfig is null or android data is null ");
            return;
        }

        string name = "Yodo1MasAndroidDependencies.xml";

        if (!string.IsNullOrEmpty(name))
        {
            string destFile = TARGET_PATH + name;
            if (File.Exists(destFile))
            {
                File.Delete(destFile);
                File.Delete(destFile + ".meta");
            }

            List<Yodo1AdNetwork> yodo1AdNetworks = new List<Yodo1AdNetwork>();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(VERSION_PATH, settings);

            XmlDocument xmlReadDoc = new XmlDocument();
            xmlReadDoc.Load(VERSION_PATH);
            XmlNode xnRead = xmlReadDoc.SelectSingleNode("versions");


            XmlDocument xmlWriteDoc = new XmlDocument();
            XmlDeclaration Declaration = xmlWriteDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            XmlNode dependencies = xmlWriteDoc.CreateElement("dependencies");

            XmlElement androidNode = (XmlElement)xnRead.SelectSingleNode("android");
            string env = androidNode.GetAttribute("env").ToString();
            string version = androidNode.GetAttribute("version").ToString();
     
            if (!env.Equals("Release"))
            {
                version = version + "-SNAPSHOT";
            }
            if (SDKGroupType.AndroidFamily == sdkType)
            {
                yodo1AdNetworks = yodo1AdNetworkConfig.androidFamily;
            }
            else if (SDKGroupType.AndroidStandard == sdkType)
            {
                yodo1AdNetworks = yodo1AdNetworkConfig.androidStandard;
            }

            #region filter data
            bool maxMediation = false, admobMediation = false, ironsourcemediation = false;
            foreach (string networkName in list)
            {
                if (string.Equals("APPLOVIN", networkName))
                {
                    maxMediation = true;
                }
                if (string.Equals("ADMOB", networkName))
                {
                    admobMediation = true;
                }
                if (string.Equals("IRONSOURCE", networkName))
                {
                    ironsourcemediation = true;
                }
            }

            List<string> repoUrlList = new List<string>();
            List<string> dependencyList = new List<string>();
            foreach (string networkName in list)
            {
                foreach(Yodo1AdNetwork adNetwork in yodo1AdNetworks) {
                    if (string.Equals(networkName, adNetwork.name))
                    {
                        if(!string.IsNullOrEmpty(adNetwork.repoUrl))
                        {
                            repoUrlList.Add(adNetwork.repoUrl);
                        }

                        dependencyList.Add(adNetwork.dependency);
                        if(admobMediation && !string.IsNullOrEmpty(adNetwork.admobAdapterDependency))
                        {
                            dependencyList.Add(adNetwork.admobAdapterDependency);
                        }
                        if(maxMediation && !string.IsNullOrEmpty(adNetwork.applovinAdapterDependency))
                        {
                            dependencyList.Add(adNetwork.applovinAdapterDependency);
                        }
                        if(maxMediation && !string.IsNullOrEmpty(adNetwork.admanagerAdapterDependency))
                        {
                            dependencyList.Add(adNetwork.admanagerAdapterDependency);
                        }
                        if(ironsourcemediation && !string.IsNullOrEmpty(adNetwork.ironsourceAdapterDependency))
                        {
                            dependencyList.Add(adNetwork.ironsourceAdapterDependency);
                        }
                    }
                }
            }
            #endregion

            #region add repository maven url part logic
            XmlNode androidPackages = xmlWriteDoc.CreateElement("androidPackages");
            dependencies.AppendChild(androidPackages);

            XmlNode repositories = xmlWriteDoc.CreateElement("repositories");
            androidPackages.AppendChild(repositories);

            foreach(string repoUrl in repoUrlList)
            {
                XmlNode repositoryNode = xmlWriteDoc.CreateElement("repository");
                repositoryNode.InnerText = repoUrl;
                repositories.AppendChild(repositoryNode);
            }

            if (!env.Equals("Release"))
            {
                XmlNode maven = xmlWriteDoc.CreateElement("repository");
                maven.InnerText = "https://oss.sonatype.org/content/repositories/snapshots/";
                repositories.AppendChild(maven);
            }
            #endregion

            #region anroidPackage implementation part logic
            if(dependencyList.Count > 0)
            {
                dependencyList.Insert(0, "com.yodo1.mas:gplibrary");
                dependencyList.Insert(0, "com.yodo1.mas:core");
                dependencyList.Add("com.yodo1.mas.mediation:yodo1");
            }

            foreach(string dependency in dependencyList)
            {
                XmlNode androidPackage = xmlWriteDoc.CreateElement("androidPackage");
                androidPackages.AppendChild(androidPackage);
                XmlAttribute spec = xmlWriteDoc.CreateAttribute("spec");
                spec.Value = dependency + ":" + version;
                androidPackage.Attributes.Append(spec);
            }
            #endregion

            reader.Close();
            //附加根节点
            xmlWriteDoc.AppendChild(dependencies);
            xmlWriteDoc.InsertBefore(Declaration, xmlWriteDoc.DocumentElement);
            xmlWriteDoc.Save(destFile);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private SDKGroupType FormatIntToSDKType(int value)
    {
        switch(value)
        {
            case (int)SDKGroupType.AndroidStandard:
                return SDKGroupType.AndroidStandard;
            case (int)SDKGroupType.AndroidFamily:
                return SDKGroupType.AndroidFamily;
            case (int)SDKGroupType.IOSStandard:
                return SDKGroupType.IOSStandard;
            default:
                return SDKGroupType.AndroidStandard;
        }
    }

    #region Get Yodo1AdNetworkConfig from server 
    private static Yodo1AdNetworkConfig GetAdNetworkConfigFromServer(String curSDKVersion)
    {
        //Debug.Log(Yodo1U3dMas.TAG + "Yodo1AdNetworkConfig curSDKVersion is " + curSDKVersion);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(VERSION_PATH);
        XmlNode root = xmlDoc.SelectSingleNode("versions");
        XmlElement node = (XmlElement)root.SelectSingleNode("unity");
        string env = node.GetAttribute("env").ToString();

        string url = "Release".Equals(env) ? "https://sdk-mas.yodo1.com/v1/unity/version-mapping" : "https://sdk-mas.yodo1.me/v1/unity/version-mapping";

        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("version", curSDKVersion);

        string response = HttpPost(url, Yodo1JSON.Serialize(dic));
        Yodo1AdNetworkConfig yodo1AdNetworkConfig = new Yodo1AdNetworkConfig();
        yodo1AdNetworkConfig = JsonUtility.FromJson<Yodo1AdNetworkConfig>(response);

        return yodo1AdNetworkConfig;
    }

    private static string HttpPost(string url, string json)
    {
        try
        {
            //Debug.Log(Yodo1U3dMas.TAG + "HttpPost request - " + json);

            // Send Request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            // Get Response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response != null)
            {
                //Debug.Log(Yodo1U3dMas.TAG + "Response.StatusCode: " + response.StatusCode);
            }
            else
            {
                //Debug.Log(Yodo1U3dMas.TAG + "Response is null");
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            //Debug.Log(Yodo1U3dMas.TAG + "HttpPost result - " + result);

            reader.Close();
            return result;
        }
        catch (WebException e)
        {
            Debug.LogWarning(Yodo1U3dMas.TAG + "HttpPost Exception.Status - " + e.Status);
            return string.Empty;
        }
    }
    #endregion


}
