namespace Yodo1.MAS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Networking;

    public class Yodo1Net
    {
        private static Yodo1Net instance;

        public static Yodo1Net GetInstance()
        {
            if (instance == null)
            {
                instance = new Yodo1Net();
            }
            return instance;
        }

        public Dictionary<string, object> GetAppInfoByAppKey(string appKey)
        {
            string url = "https://sdk-mas.yodo1.com/v1/unity/setup/" + appKey;
            Dictionary<string, object> dict = new Dictionary<string, object>();
#if UNITY_2018_1_OR_NEWER
            string response = HttpGet(url);
            dict = (Dictionary<string, object>)Yodo1JSON.Deserialize(response);
            return dict;
#else
            ApiCallback callback = delegate (string response)
            {
                dict = (Dictionary<string, object>)Yodo1JSON.Deserialize(response);
            };
            EditorCoroutineRunner.StartEditorCoroutine(SendUrl(url, callback));
            return dict;
#endif
        }

        public Dictionary<string, object> GetAppInfoByBundleID(string platform, string bundleId)
        {
            string url = "https://sdk-mas.yodo1.com/v1/unity/get-app-info-by-bundle-id";
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
            Dictionary<string, object> obj = (Dictionary<string, object>)Yodo1JSON.Deserialize(response);
            return obj;
        }

        private string HttpPost(string url, string json)
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
                // Debug.Log(Yodo1U3dMas.TAG + "HttpPost result - " + result);

                reader.Close();
                return result;
            }
            catch (WebException e)
            {
                //Debug.LogWarning(Yodo1U3dMas.TAG + "HttpPost Exception.Status - " + e.Status + ", please check your bundle id or package name.");
                e.StackTrace.ToString();
                return string.Empty;
            }
        }

        private string HttpGet(string url)
        {
            try
            {
                string serviceAddress = url;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string returnXml = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                response.Close();
                return returnXml;

            }
            catch (WebException e)
            {
                e.StackTrace.ToString();
                return e.StackTrace.ToString();
            }
        }

        private string Md5(string strToEncryppt)
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

#if UNITY_2018_1_OR_NEWER
#else
        public delegate void ApiCallback(string response);

        private IEnumerator SendUrl(string url, ApiCallback callback)
        {
            using (UnityWebRequest www = new UnityWebRequest(url))
            {
                yield return www;
                if (www.error != null)
                {
                    yield return null;
                }
                else
                {
                    callback(www.downloadHandler.text);
                }
            }
        }
#endif
    }
}
