namespace Yodo1.MAS
{
    using UnityEditor;
    using System.IO;
    using System.Xml;
    using System;
    using UnityEngine;

    public class Yodo1AdUtils
    {
        /// <summary>
        /// Show Alert
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="positiveButton"></param>
        public static void ShowAlert(string title, string message, string positiveButton)
        {
            if (!string.IsNullOrEmpty(positiveButton))
            {
                int index = EditorUtility.DisplayDialogComplex(title, message, positiveButton, "", "");

            }
            return;
        }

        private static readonly string VERSION_PATH = Path.GetFullPath(".") + "/Assets/Yodo1/MAS/version.xml";

        public static string GetPluginVersion()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(VERSION_PATH, settings);

            XmlDocument xmlReadDoc = new XmlDocument();
            xmlReadDoc.Load(VERSION_PATH);
            XmlNode xnRead = xmlReadDoc.SelectSingleNode("versions");
            XmlElement unityNode = (XmlElement)xnRead.SelectSingleNode("unity");
            string version = unityNode.GetAttribute("version").ToString();

            reader.Close();
            return version;
        }

        private static readonly string DEPENDENCIES_PATH = "Assets/Yodo1/MAS/Editor/Dependencies";
        private static readonly string DEPENDENCIES_PATH_ANDROID = Path.Combine(DEPENDENCIES_PATH, "Yodo1MasAndroidDependencies.xml");
        private static readonly string DEPENDENCIES_PATH_IOS = Path.Combine(DEPENDENCIES_PATH, "Yodo1MasiOSDependencies.xml");

        public static bool IsGooglePlayVersion()
        {
            bool isGooglePlayVersion = false;
#if UNITY_IOS || UNITY_IPHONE
            return isGooglePlayVersion;
#endif

            string dependencyFilePath = DEPENDENCIES_PATH_ANDROID;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(dependencyFilePath, settings);

            XmlDocument xmlReadDoc = new XmlDocument();
            xmlReadDoc.Load(dependencyFilePath);
            XmlNode dependenciesRead = xmlReadDoc.SelectSingleNode("dependencies");
            XmlNode androidPackagesRead = dependenciesRead.SelectSingleNode("androidPackages");
            XmlNodeList nodeList = androidPackagesRead.SelectNodes("androidPackage");
            if (nodeList != null && nodeList.Count > 0)
            {
                try
                {
                    foreach (XmlNode node in nodeList)
                    {
                        string specString = ((XmlElement)node).GetAttribute("spec").ToString();
                        if (string.IsNullOrEmpty(specString))
                        {
                            continue;
                        }
                        if (specString.Contains("com.yodo1.mas:gplibrary") ||
                            specString.Contains("com.yodo1.mas:full") ||
                            specString.Contains("com.yodo1.mas:lite") ||
                            specString.Contains("com.yodo1.mas:google"))
                        {
                            isGooglePlayVersion = true;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            reader.Close();

            return isGooglePlayVersion;
        }

        public static bool IsAppLovinValid()
        {
            bool ret = false;
            string dependencyFilePath = string.Empty;
#if UNITY_ANDROID
            dependencyFilePath = DEPENDENCIES_PATH_ANDROID;
#elif UNITY_IOS || UNITY_IPHONE
            dependencyFilePath = DEPENDENCIES_PATH_IOS;
#endif
            if (string.IsNullOrEmpty(dependencyFilePath))
            {
                return ret;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(dependencyFilePath, settings);

            XmlDocument xmlReadDoc = new XmlDocument();
            xmlReadDoc.Load(dependencyFilePath);
            XmlNode dependenciesRead = xmlReadDoc.SelectSingleNode("dependencies");

            XmlNodeList nodeList = null;
#if UNITY_ANDROID
            XmlNode androidPackagesRead = dependenciesRead.SelectSingleNode("androidPackages");
            nodeList = androidPackagesRead.SelectNodes("androidPackage");
#endif
#if UNITY_IOS || UNITY_IPHONE
            XmlNode iosPodsRead = dependenciesRead.SelectSingleNode("iosPods");
            nodeList = iosPodsRead.SelectNodes("iosPod");
#endif
            if (nodeList != null && nodeList.Count > 0)
            {
                try
                {
                    foreach (XmlNode node in nodeList)
                    {
#if UNITY_ANDROID
                        string name = ((XmlElement)node).GetAttribute("spec").ToString();
                        if (string.IsNullOrEmpty(name))
                        {
                            continue;
                        }
                        if (name.Contains("com.yodo1.mas.mediation:applovin") || name.Contains("com.yodo1.mas:full") || name.Contains("com.yodo1.mas:lite"))
#endif
#if UNITY_IOS || UNITY_IPHONE
                        string name = ((XmlElement)node).GetAttribute("name").ToString();
                        if (string.IsNullOrEmpty(name))
                        {
                            continue;
                        }
                        if (name.Contains("Yodo1MasMediationApplovin") || name.Contains("Yodo1MasFull") || name.Contains("Yodo1MasLite"))
#endif
                        {
                            ret = true;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            reader.Close();

            return ret;
        }

        public static bool IsAdMobValid()
        {
            bool ret = false;
            string dependencyFilePath = string.Empty;
#if UNITY_ANDROID
            dependencyFilePath = DEPENDENCIES_PATH_ANDROID;
#elif UNITY_IOS || UNITY_IPHONE
            dependencyFilePath = DEPENDENCIES_PATH_IOS;
#endif
            if (string.IsNullOrEmpty(dependencyFilePath))
            {
                return ret;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(dependencyFilePath, settings);

            XmlDocument xmlReadDoc = new XmlDocument();
            xmlReadDoc.Load(dependencyFilePath);
            XmlNode dependenciesRead = xmlReadDoc.SelectSingleNode("dependencies");

            XmlNodeList nodeList = null;
#if UNITY_ANDROID
            XmlNode androidPackagesRead = dependenciesRead.SelectSingleNode("androidPackages");
            nodeList = androidPackagesRead.SelectNodes("androidPackage");
#endif
#if UNITY_IOS || UNITY_IPHONE
            XmlNode iosPodsRead = dependenciesRead.SelectSingleNode("iosPods");
            nodeList = iosPodsRead.SelectNodes("iosPod");
#endif
            if (nodeList != null && nodeList.Count > 0)
            {
                try
                {
                    foreach (XmlNode node in nodeList)
                    {
#if UNITY_ANDROID
                        string name = ((XmlElement)node).GetAttribute("spec").ToString();
                        if (string.IsNullOrEmpty(name))
                        {
                            continue;
                        }
                        if (name.Contains("com.yodo1.mas.mediation:admob") || name.Contains("com.yodo1.mas:full") || name.Contains("com.yodo1.mas:lite"))
#endif
#if UNITY_IOS || UNITY_IPHONE
                        string name = ((XmlElement)node).GetAttribute("name").ToString();
                        if (string.IsNullOrEmpty(name))
                        {
                            continue;
                        }
                        if (name.Contains("Yodo1MasMediationAdMob") || name.Contains("Yodo1MasFull") || name.Contains("Yodo1MasLite"))
#endif
                        {
                            ret = true;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            reader.Close();

            return ret;
        }


    }


}