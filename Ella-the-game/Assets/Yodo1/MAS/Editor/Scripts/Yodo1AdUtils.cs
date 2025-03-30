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
#if UNITY_ANDROID
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
#endif
            return isGooglePlayVersion;
        }

        public static bool IsAppLovinValid(Yodo1PlatfromTarget target)
        {
            return IsValidWithNetwork(target, "Applovin");
        }

        public static bool IsAdMobValid(Yodo1PlatfromTarget target)
        {
            return IsValidWithNetwork(target, "AdMob");
        }

        public static bool IsValidWithNetwork(Yodo1PlatfromTarget target, string network)
        {
            bool ret = false;
            string dependencyFilePath = string.Empty;
            if (target == Yodo1PlatfromTarget.Android)
            {
                dependencyFilePath = DEPENDENCIES_PATH_ANDROID;
            }
            else if (target == Yodo1PlatfromTarget.iOS)
            {
                dependencyFilePath = DEPENDENCIES_PATH_IOS;
            }

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
            if (target == Yodo1PlatfromTarget.Android)
            {
                XmlNode androidPackagesRead = dependenciesRead.SelectSingleNode("androidPackages");
                nodeList = androidPackagesRead.SelectNodes("androidPackage");
            }
            else if (target == Yodo1PlatfromTarget.iOS)
            {
                XmlNode iosPodsRead = dependenciesRead.SelectSingleNode("iosPods");
                nodeList = iosPodsRead.SelectNodes("iosPod");
            }

            if (nodeList != null && nodeList.Count > 0)
            {
                try
                {
                    foreach (XmlNode node in nodeList)
                    {
                        if (target == Yodo1PlatfromTarget.Android)
                        {
                            string name = ((XmlElement)node).GetAttribute("spec").ToString();
                            if (string.IsNullOrEmpty(name))
                            {
                                continue;
                            }
                            string networkName = string.Format("com.yodo1.mas.mediation:{0}", network);
                            if (name.ToLower().Contains(networkName.ToLower()) || name.Contains("com.yodo1.mas:full") || name.Contains("com.yodo1.mas:lite"))
                            {
                                ret = true;
                                break;
                            }

                        }
                        else if (target == Yodo1PlatfromTarget.iOS)
                        {
                            string name = ((XmlElement)node).GetAttribute("name").ToString();
                            if (string.IsNullOrEmpty(name))
                            {
                                continue;
                            }
                            string networkName = string.Format("Yodo1MasMediation{0}", network);
                            if (name.ToLower().Contains(networkName.ToLower()) || name.Contains("Yodo1MasFull") || name.Contains("Yodo1MasLite"))
                            {
                                ret = true;
                                break;
                            }
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