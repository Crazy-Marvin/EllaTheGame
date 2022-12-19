using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyMobile;
using System.IO;
#if UNITY_2018_3_OR_NEWER
using UnityEngine.Networking;
#endif

namespace EasyMobile.Demo
{
    [AddComponentMenu("")]
    public class GifDemo : MonoBehaviour
    {
        [Header("GIF Playing Settings")]
        // In this demo we'll download and play a GIF file we uploaded to Giphy with the link below.
        // In a real project, you can store your GIF files in the StreamingAssets
        // folder so you can get their filepath directly (except on Android).
        public const string DemoGifFileName = "demoGifPlaying.gif";
        public string demoGifLink = "https://media.giphy.com/media/f8c0hyn3leSAXhAK9k/giphy.gif";
        public System.Threading.ThreadPriority decodeThreadPriority;

        [Header("GIF Playing UI Stuff")]
        public GameObject gifPlayingPanel;
        public Text demoGifDownloadingText;
        public GameObject retryDownloadButton;
        public ClipPlayerUI decodedClipPlayer;
        public Text displayGifFilePath;

        [Header("GIF Generation Settings")]
        public Recorder recorder;
        public string gifFilename = "easy_mobile_demo";
        public int loop = 0;
        [Range(1, 100)]
        public int quality = 90;
        public System.Threading.ThreadPriority exportThreadPriority;

        [Header("Giphy Upload Key")]
        public string giphyUsername;
        public string giphyApiKey;

        [Header("GIF Generation UI Stuff")]
        public GameObject recordingMark;
        public GameObject startRecordingButton;
        public GameObject stopRecordingButton;
        public GameObject playbackPanel;
        public ClipPlayerUI clipPlayer;
        public GameObject giphyLogo;
        public GameObject activityProgress;
        public Text activityText;
        public Text progressText;

        AnimatedClip decodedClip;
        AnimatedClip recordedClip;
        bool isExportingGif;
        bool isUploadingGif;
        string exportedGifPath;
        string uploadedGifUrl;
        string demoGifFilePath;

        void OnDestroy()
        {
            // Dispose the used clip if needed
            if (recordedClip != null)
                recordedClip.Dispose();
        }

        void Awake()
        {
            // Init EM runtime if needed (useful in case only this scene is built).
            if (!RuntimeManager.IsInitialized())
                RuntimeManager.Init();
        }

        void Start()
        {
            gifPlayingPanel.SetActive(true);
            decodedClipPlayer.gameObject.SetActive(false);
            demoGifDownloadingText.gameObject.SetActive(false);
            retryDownloadButton.SetActive(false);
            displayGifFilePath.gameObject.SetActive(false);
            startRecordingButton.SetActive(true);
            stopRecordingButton.SetActive(false);
            recordingMark.SetActive(false);
            playbackPanel.SetActive(false);
            activityProgress.SetActive(false);

            DownloadDemoGif();
        }

        void Update()
        {
            giphyLogo.SetActive(Giphy.IsUsingAPI);
            activityProgress.SetActive(isExportingGif || isUploadingGif);
        }

        public void DownloadDemoGif()
        {
            StartCoroutine(CRDownloadDemoGif());
        }

        IEnumerator CRDownloadDemoGif()
        {
            yield return null;
            // Download the demo GIF from the internet & store it in Application.persistentDataPath.
            // This is only done once.
            demoGifFilePath = Path.Combine(Application.persistentDataPath, DemoGifFileName);
            if (!File.Exists(demoGifFilePath))
            {
                // Load the file from the internet.
                displayGifFilePath.gameObject.SetActive(false);
                retryDownloadButton.SetActive(false);
                decodedClipPlayer.gameObject.SetActive(false);
                demoGifDownloadingText.gameObject.SetActive(true);
                demoGifDownloadingText.text = "Downloading demo GIF from\n" + demoGifLink;
#if UNITY_2018_3_OR_NEWER
                var www = new UnityWebRequest(demoGifLink);
                www.downloadHandler = new DownloadHandlerBuffer();
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    demoGifDownloadingText.text = "Failed to download demo GIF:\n" + www.error;
                    retryDownloadButton.SetActive(true);
                    yield break;
                }
                else
                {
                    // File loaded. Now store it to persistentDataPath.
                    File.WriteAllBytes(demoGifFilePath, www.downloadHandler.data);
                }
#else
                var www = new WWW(demoGifLink);
                while (!www.isDone) { yield return null; }

                if (!string.IsNullOrEmpty(www.error))
                {
                    demoGifDownloadingText.text = "Failed to download demo GIF:\n" + www.error;
                    retryDownloadButton.SetActive(true);
                    yield break;
                }
                else
                {
                    // File loaded. Now store it to persistentDataPath.
                    File.WriteAllBytes(demoGifFilePath, www.bytes);
                }
#endif
            }

            // Display GIF filepath.
            demoGifDownloadingText.gameObject.SetActive(false);
            displayGifFilePath.gameObject.SetActive(true);
            displayGifFilePath.text = demoGifFilePath;

            // Now decode and play the demo GIF file at the demoGifFilePath.
            PlayDemoGif();
        }

        public void PlayDemoGif()
        {
            decodedClipPlayer.gameObject.SetActive(true);

            if (string.IsNullOrEmpty(demoGifFilePath))
            {
                Debug.LogError("Couldn't file demo GIF file in persistentDataPath. Filepath: " + demoGifFilePath);
                return;
            }

            Gif.DecodeGif(demoGifFilePath, decodeThreadPriority, clip =>
             {
                 if (clip != null)
                 {
                     decodedClip = clip;
                     clip.SetFilterMode(FilterMode.Point);
                     Gif.PlayClip(decodedClipPlayer, decodedClip);
                 }
                 else
                 {
                     Debug.LogError("Error decoding GIF: received null AnimatedClip object.");
                 }
             });
        }

        public void ShowDemoGifFirstFrame()
        {
            decodedClipPlayer.gameObject.SetActive(true);

            if (string.IsNullOrEmpty(demoGifFilePath))
            {
                Debug.LogError("Couldn't file demo GIF file in persistentDataPath. Filepath: " + demoGifFilePath);
                return;
            }

            Gif.DecodeGif(demoGifFilePath, 1, decodeThreadPriority, clip =>
            {
                if (clip != null)
                {
                    decodedClip = clip;
                    clip.SetFilterMode(FilterMode.Point);
                    Gif.PlayClip(decodedClipPlayer, decodedClip);
                }
                else
                {
                    Debug.LogError("Error decoding GIF: received null AnimatedClip object.");
                }
            });
        }

        public void OpenGifGenerationDemo()
        {
            decodedClipPlayer.Stop();
            gifPlayingPanel.SetActive(false);
        }

        public void OpenGifPlayingDemo()
        {
            gifPlayingPanel.SetActive(true);
            PlayDemoGif();
        }

        public void StartRecording()
        {
            // Dispose the old clip
            if (recordedClip != null)
                recordedClip.Dispose();

            Gif.StartRecording(recorder);
            startRecordingButton.SetActive(false);
            stopRecordingButton.SetActive(true);
            recordingMark.SetActive(true);
        }

        public void StopRecording()
        {
            recordedClip = Gif.StopRecording(recorder);
            startRecordingButton.SetActive(true);
            stopRecordingButton.SetActive(false);
            recordingMark.SetActive(false);
        }

        public void OpenPlaybackPanel()
        {
            if (recordedClip != null)
            {
                playbackPanel.SetActive(true);
                Gif.PlayClip(clipPlayer, recordedClip, 1f);
            }
            else
            {
                NativeUI.Alert("Nothing Recorded", "Please finish recording first.");
            }
        }

        public void ClosePlaybackPanel()
        {
            clipPlayer.Stop();
            playbackPanel.SetActive(false);
        }

        public void ExportGIF()
        {
            if (isExportingGif)
            {
                NativeUI.Alert("Exporting In Progress", "Please wait until the current GIF exporting is completed.");
                return;
            }
            else if (isUploadingGif)
            {
                NativeUI.Alert("Uploading In Progress", "Please wait until the GIF uploading is completed.");
                return;
            }

            isExportingGif = true;
            Gif.ExportGif(recordedClip, gifFilename, loop, quality, exportThreadPriority, OnGifExportProgress, OnGifExportCompleted);
        }

        public void UploadGIFToGiphy()
        {
            if (isExportingGif)
            {
                NativeUI.Alert("Exporting In Progress", "Please wait until the GIF exporting is completed.");
                return;
            }
            else if (string.IsNullOrEmpty(exportedGifPath))
            {
                NativeUI.Alert("No Exported GIF", "Please export a GIF file first.");
                return;
            }

            isUploadingGif = true;

            var content = new GiphyUploadParams();
            content.localImagePath = exportedGifPath;
            content.tags = "demo, easy mobile, sglib games, unity3d";

            if (!string.IsNullOrEmpty(giphyUsername) && !string.IsNullOrEmpty(giphyApiKey))
                Giphy.Upload(giphyUsername, giphyApiKey, content, OnGiphyUploadProgress, OnGiphyUploadCompleted, OnGiphyUploadFailed);
            else
#if UNITY_EDITOR
                Debug.LogError("Upload failed: please provide valid Giphy username and API key in the GifDemo game object.");
#else
                NativeUI.Alert("Upload Failed", "Please provide valid Giphy username and API key in the GifDemo game object.");
#endif
        }

        public void ShareGiphyURL()
        {
            if (string.IsNullOrEmpty(uploadedGifUrl))
            {
                NativeUI.Alert("Invalid URL", "No valid Giphy URL found. Did the upload succeed?");
                return;
            }

            Sharing.ShareURL(uploadedGifUrl);
        }

        // This callback is called repeatedly during the GIF exporting process.
        // It receives a progress value ranging from 0 to 1.
        void OnGifExportProgress(AnimatedClip clip, float progress)
        {
            activityText.text = "GENERATING GIF...";
            progressText.text = string.Format("{0:P0}", progress);
        }

        // This callback is called once the GIF exporting has completed.
        // It receives the filepath of the generated image.
        void OnGifExportCompleted(AnimatedClip clip, string path)
        {
            progressText.text = "DONE";

            isExportingGif = false;
            exportedGifPath = path;

#if UNITY_EDITOR
            bool shouldUpload = UnityEditor.EditorUtility.DisplayDialog("Export Completed", "A GIF file has been created. Do you want to upload it to Giphy?", "Yes", "No");
            if (shouldUpload)
                UploadGIFToGiphy();
#else
			NativeUI.AlertPopup popup = NativeUI.ShowTwoButtonAlert("Export Completed", "A GIF file has been created. Do you want to upload it to Giphy?", "Yes", "No");
			if (popup != null)
			{
				popup.OnComplete += (int buttonId) =>
				{
					if (buttonId == 0)
						UploadGIFToGiphy();
				};
			}
#endif
        }

        // This callback is called repeatedly during the uploading process.
        // It receives a progress value ranging from 0 to 1.
        void OnGiphyUploadProgress(float progress)
        {
            activityText.text = "UPLOADING TO GIPHY...";
            progressText.text = string.Format("{0:P0}", progress);
        }

        // This callback is called once the uploading has completed.
        // It receives the URL of the uploaded image.
        void OnGiphyUploadCompleted(string url)
        {
            progressText.text = "DONE";

            isUploadingGif = false;
            uploadedGifUrl = url;
#if UNITY_EDITOR
            bool shouldOpen = UnityEditor.EditorUtility.DisplayDialog("Upload Completed", "The GIF image has been uploaded to Giphy at " + url + ". Open it in the browser?", "Yes", "No");
            if (shouldOpen)
                Application.OpenURL(uploadedGifUrl);
#else
			NativeUI.AlertPopup alert = NativeUI.ShowTwoButtonAlert("Upload Completed", "The GIF image has been uploaded to Giphy at " + url + ". Open it in the browser?", "Yes", "No");

			if (alert != null)
			{
				alert.OnComplete += (int buttonId) =>
				{
					if (buttonId == 0)
						Application.OpenURL(uploadedGifUrl);
				};
			}
#endif
        }

        // This callback is called if the upload has failed.
        // It receives the error message.
        void OnGiphyUploadFailed(string error)
        {
            isUploadingGif = false;
            NativeUI.Alert("Upload Failed", "Uploading to Giphy has failed with error " + error);
        }
    }
}

