﻿using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Yodo1.MAS;
using UnityEditor;
using System.IO;
using System.Net;
using System;
using System.Text;

public class Yodo1IdSync : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    private String AdmobDefaultIDAndroid = "ca-app-pub-5580537606944457~4465578836";
    private String AdmobDefaultIDiOS = "ca-app-pub-5580537606944457~2166718551";
    public void OnPreprocessBuild(BuildReport report)
    {
        Yodo1AdSettings settings = Yodo1AdSettingsSave.Load();

        string bundleId = string.Empty;
#if UNITY_ANDROID
        if (!string.Equals(settings.androidSettings.AdmobAppID, AdmobDefaultIDAndroid))
        {
            bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            Dictionary<string, object> androidData = GetAppInfo("android", bundleId);
            Yodo1AdAssetsImporter.UpdateData(settings, androidData);
        }
#elif UNITY_IOS
        if (!string.Equals(settings.iOSSettings.AdmobAppID, AdmobDefaultIDiOS))
        {
            bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
            Dictionary<string, object> iosData = GetAppInfo("iOS", bundleId);
            Yodo1AdAssetsImporter.UpdateData(settings, iosData);
        }
#endif
    }

    private static Dictionary<string, object> GetAppInfo(string platform, string bundleId)
    {
        string url = "https://sdk.mas.yodo1.com/v1/unity/get-app-info-by-bundle-id";
        string secretKey = "W4OJaaOVAmO2uGaUVCCw24cuHKu4Zc";
        string timestamp = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000) + "";
        string sign = Md5(secretKey + timestamp + bundleId).ToLower();

        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("timestamp", timestamp);
        dic.Add("bundle_id", bundleId);
        dic.Add("plugin_version", Yodo1AdUtils.GetPluginVersion());
        dic.Add("platform", platform);
        dic.Add("sign", sign);

        string response = HttpPost(url, Yodo1JSON.Serialize(dic));
        //Dictionary<string, object> obj = (Dictionary<string, object>)Yodo1JSON.Deserialize(response);
        return (Dictionary<string, object>)Yodo1JSON.Deserialize(response);
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
                throw new BuildFailedException(Yodo1U3dMas.TAG + " Unable to verify AdMob App Id with MAS Server. Please check your internet connectivity and try again.");
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            // Debug.Log(Yodo1U3dMas.TAG + "HttpPost result - " + result);

            reader.Close();
            return result;
        }
        catch (WebException)
        {
            throw new BuildFailedException(Yodo1U3dMas.TAG + " Unable to verify AdMob App Id with MAS Server. Please check your internet connectivity and try again.");
        }
    }
    private static string Md5(string strToEncryppt)
    {
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] bs = UTF8Encoding.UTF8.GetBytes(strToEncryppt);
            byte[] hashBytes = md5.ComputeHash(bs);
            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
