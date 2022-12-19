using UnityEngine;
using System.Collections;
using System;
using EasyMobile.Internal;

namespace EasyMobile
{
#if UNITY_2018_3_OR_NEWER
    using UnityEngine.Networking;
#endif

    [AddComponentMenu("")]
    public class Giphy : MonoBehaviour
    {
        public static Giphy Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject ob = new GameObject("Giphy");
                    _instance = ob.AddComponent<Giphy>();
                    DontDestroyOnLoad(ob);
                }

                return _instance;
            }
        }

        private static Giphy _instance;

        public static bool IsUsingAPI { get { return _apiUseCount > 0; } }

        [Obsolete("This public beta key is now obsolete. Now you can register your app on the Giply developers dashboard and get a specific key for it.")]
        public const string GIPHY_PUBLIC_BETA_KEY = "dc6zaTOxFJmzC";
        public const string GIPHY_UPLOAD_PATH = "https://upload.giphy.com/v1/gifs";
        public const string GIPHY_BASE_URL = "http://giphy.com/gifs/";

        static int _apiUseCount = 0;

        #region Child classes

        [System.Serializable]
        private class UploadSuccessResponse
        {
            public UploadSuccessData data = new UploadSuccessData();

            [System.Serializable]
            public class UploadSuccessData
            {
                public string id = "";
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Uploads a GIF image to Giphy using the public beta key. The GIF file can be stored on
        /// the local storage or at a provided URL.
        /// </summary>
        /// <param name="content">Content to upload.</param>
        /// <param name="uploadProgressCallback">Upload progress callback: the parameter indicates upload progress from 0 to 1.</param>
        /// <param name="uploadCompletedCallback">Upload completed callback: the parameter is the URL of the uploaded image.</param>
        /// <param name="uploadFailedCallback">Upload failed callback: the parameter is the error message.</param>
        [Obsolete("This method was deprecated. Now you must upload with a Giphy API key specific to your app.")]
        public static void Upload(GiphyUploadParams content, Action<float> uploadProgressCallback, Action<string> uploadCompletedCallback, Action<string> uploadFailedCallback)
        {
            Upload("", GIPHY_PUBLIC_BETA_KEY, content, uploadProgressCallback, uploadCompletedCallback, uploadFailedCallback);
        }

        /// <summary>
        /// Uploads a GIF image to your channel using your Giphy username and your app's API key.
        /// The GIF file can be stored on the local storage or at a provided URL.
        /// </summary>
        /// <param name="username">Your Giphy username.</param>
        /// <param name="apiKey">The API key for your app.</param>
        /// <param name="content">Content to upload.</param>
        /// <param name="uploadProgressCallback">Upload progress callback: the parameter indicates upload progress from 0 to 1.</param>
        /// <param name="uploadCompletedCallback">Upload completed callback: the parameter is the URL of the uploaded image.</param>
        /// <param name="uploadFailedCallback">Upload failed callback: the parameter is the error message.</param>
        public static void Upload(string username, string apiKey, GiphyUploadParams content, Action<float> uploadProgressCallback, Action<string> uploadCompletedCallback, Action<string> uploadFailedCallback)
        {
#if !UNITY_WEBPLAYER
            if (string.IsNullOrEmpty(content.localImagePath) && string.IsNullOrEmpty(content.sourceImageUrl))
            {
                Debug.LogError("UploadToGiphy FAILED: no image was specified for uploading.");
                return;
            }
            else if (!string.IsNullOrEmpty(content.localImagePath) && !System.IO.File.Exists(content.localImagePath))
            {
                Debug.LogError("UploadToGiphy FAILED: (local) file not found.");
                return;
            }

            // Append the API key to the upload URL itself, as simply adding it to the form doesn't really work.
            string uploadPath = GIPHY_UPLOAD_PATH + (string.IsNullOrEmpty(apiKey) ? string.Empty : "?api_key=" + apiKey);

            // Prepare upload form.
            WWWForm form = new WWWForm();
            form.AddField("api_key", apiKey);
            form.AddField("username", username);

            if (!string.IsNullOrEmpty(content.localImagePath) && System.IO.File.Exists(content.localImagePath))
                form.AddBinaryData("file", FileUtil.ReadAllBytes(content.localImagePath));

            if (!string.IsNullOrEmpty(content.sourceImageUrl))
                form.AddField("source_image_url", content.sourceImageUrl);

            if (!string.IsNullOrEmpty(content.tags))
                form.AddField("tags", content.tags);

            if (!string.IsNullOrEmpty(content.sourcePostUrl))
                form.AddField("source_post_url", content.sourcePostUrl);

            if (content.isHidden)
                form.AddField("is_hidden", "true");

            // Start uploading.
            Instance.StartCoroutine(CRUpload(uploadPath, form, uploadProgressCallback, uploadCompletedCallback, uploadFailedCallback));
#endif
        }

        #endregion

        #region Private Stuff

#if UNITY_2018_3_OR_NEWER
        // WWW was deprecated since Unity 2018.3.0. UnityWebRequest is the replacement.
        static IEnumerator CRUpload(string uploadPath, WWWForm form, Action<float> uploadProgressCB, Action<string> uploadCompletedCB, Action<string> uploadFailedCB)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(uploadPath, form))
            {
                www.SendWebRequest();
                _apiUseCount++;

                while (!www.isDone)
                {
                    if (uploadProgressCB != null)
                        uploadProgressCB(www.uploadProgress);

                    yield return null;
                }

                if (string.IsNullOrEmpty(www.error))
                {
                    if (uploadCompletedCB != null)
                    {
                        // Extract and return the GIF URL from the return response.
                        UploadSuccessResponse json = JsonUtility.FromJson<UploadSuccessResponse>(www.downloadHandler.text);
                        uploadCompletedCB(GIPHY_BASE_URL + json.data.id);
                    }
                }
                else
                {
                    if (uploadFailedCB != null)
                        uploadFailedCB(www.error);
                }

                _apiUseCount--;
            }
        }
#else
        static IEnumerator CRUpload(string uploadPath, WWWForm form, Action<float> uploadProgressCB, Action<string> uploadCompletedCB, Action<string> uploadFailedCB)
        {
            WWW www = new WWW(uploadPath, form);
            _apiUseCount++;

            while (!www.isDone)
            {
                if (uploadProgressCB != null)
                    uploadProgressCB(www.uploadProgress);

                yield return null;
            }

            if (string.IsNullOrEmpty(www.error))
            {
                if (uploadCompletedCB != null)
                {
                    // Extract and return the GIF URL from the return response.
                    UploadSuccessResponse json = JsonUtility.FromJson<UploadSuccessResponse>(www.text);
                    uploadCompletedCB(GIPHY_BASE_URL + json.data.id);
                }
            }
            else
            {
                if (uploadFailedCB != null)
                    uploadFailedCB(www.error);
            }

            _apiUseCount--;
        }
#endif

        #endregion

        #region Unity events

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            if (this == _instance)
                _instance = null;
        }

        #endregion
    }
}
