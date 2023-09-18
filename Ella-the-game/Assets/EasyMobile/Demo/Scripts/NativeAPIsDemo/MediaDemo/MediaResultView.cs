using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    public class MediaResultView : MonoBehaviour
    {
        [SerializeField]
        private Text infoText = null;

        [SerializeField]
        private InputField maxImageSizeInput = null;

        [SerializeField]
        private GameObject maxImageSizeRoot = null;

        [SerializeField]
        private Button loadButton = null,
            removeButton = null;

        [SerializeField]
        private MediaDemo mediaDemo = null;

        private MediaResult model = null;
        private string error = null;

        public Color VideoBackground { get; set; }

#if UNITY_IOS || UNITY_ANDROID
        public FullScreenMovieControlMode VideoControlMode { get; set; }
        public FullScreenMovieScalingMode VideoScalingMode { get; set; }
#endif

        public int MaxImageSize
        {
            get
            {
                if (string.IsNullOrEmpty(maxImageSizeInput.text))
                    return -1;

                int result = -1;
                int.TryParse(maxImageSizeInput.text, out result);
                return result;
            }
        }

        private void Start()
        {
            loadButton.onClick.AddListener(Load);
            maxImageSizeInput.keyboardType = TouchScreenKeyboardType.NumberPad;
            removeButton.onClick.AddListener(() => mediaDemo.RemoveDisplayedView(this));
        }

        public MediaResult GetMedia()
        {
            return this.model;
        }

        public void UpdateWithError(string error)
        {
            this.error = error;
            model = null;
            maxImageSizeRoot.SetActive(false);
            loadButton.gameObject.SetActive(false);
            infoText.text = error != null ? "<b>Error:</b><i>" + error + "</i>" : "Null error.";
        }

        public void UpdateWithModel(MediaResult newModel)
        {
            model = newModel;
            error = null;
            maxImageSizeRoot.SetActive(model != null && error == null && model.Type == MediaType.Image && (model.contentUri != null || model.absoluteUri != null));
            loadButton.gameObject.SetActive(model != null && error == null && model.Uri != null);
            infoText.text = string.Format(
                "<b>Type:</b> <i>{0}</i> \n<b>Uri:</b> <i>{1}</i>",
                model.Type, model.Uri ?? "null"); ;
        }

        public void Load()
        {
            if (model == null || model.Type == MediaType.None)
                return;

            if (model.Type == MediaType.Image)
            {
                LoadImage();
                return;
            }

            PlayVideo();
        }

        private void LoadImage()
        {
            model.LoadImage(LoadImageCallback, MaxImageSize);
        }

        private void PlayVideo()
        {
#if UNITY_IOS || UNITY_ANDROID
            Handheld.PlayFullScreenMovie(model.Uri, VideoBackground, VideoControlMode, VideoScalingMode);
#endif
        }

        private void LoadImageCallback(string error, Texture2D image)
        {
            if (error != null)
            {
                NativeUI.Alert("Load Image", "Error: " + error);
                return;
            }

            mediaDemo.UpdateDisplayImage(image);
        }
    }
}
