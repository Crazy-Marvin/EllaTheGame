using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    public class MediaDemo : MonoBehaviour
    {
        [SerializeField]
        private GameObject curtain = null;

        [SerializeField]
        private uint randomTextureWidth = 512, randomTextureHeight = 256;

        [SerializeField]
        private Color[] randomTextureColors = null;

        [SerializeField]
        private Color videoBackground = Color.white;

        [SerializeField]
        private float imagePadding = 5;

#if UNITY_IOS || UNITY_ANDROID

        [SerializeField]
        private FullScreenMovieControlMode videoControlMode = FullScreenMovieControlMode.Full;

        [SerializeField]
        private FullScreenMovieScalingMode videoScalingMode = FullScreenMovieScalingMode.None;

#endif

        [SerializeField]
        private MediaResultView viewPrefab = null;

        [SerializeField]
        private Transform viewRoot = null;

        [SerializeField]
        private RawImage displayImage = null;

        [SerializeField]
        private RectTransform displayImageTransform = null, parentTransform = null;

        [SerializeField]
        private InputField imageNameInputField = null;

        [SerializeField]
        private Dropdown imageFormatDropdown = null;

        [SerializeField]
        private Dropdown cameraTypeDropdown = null;

        [SerializeField]
        private DemoUtils demoUtils = null;

        [SerializeField]
        private GameObject frontCameraSupportedToggle = null,
            backCameraSupportedToggle = null;

        [SerializeField]
        private Button takePictureButton = null,
            recordVideoButton = null,
            pickButton = null,
            saveImageButton = null,
            clearViewsButton = null,
            randomTextureButton = null,
            clearTexutreButton = null;

        private List<MediaResultView> displayedViews = new List<MediaResultView>();
        private CameraType currentCameraType = CameraType.Rear;

        protected virtual void Start()
        {
            if (!EM_Settings.IsSubmoduleEnable(Submodule.Media))
            {
                curtain.SetActive(true);
                return;
            }
            else
            {
                curtain.SetActive(false);
            }


            takePictureButton.onClick.AddListener(TakePicture);
            recordVideoButton.onClick.AddListener(RecordVideo);
            pickButton.onClick.AddListener(PickFromGallery);
            saveImageButton.onClick.AddListener(SaveImage);
            clearViewsButton.onClick.AddListener(ClearDisplayedViews);
            randomTextureButton.onClick.AddListener(RandomDisplayImage);
            clearTexutreButton.onClick.AddListener(ClearDisplayImage);
            InitDropdownWithEnum(imageFormatDropdown, typeof(ImageFormat));

            InitDropdownWithEnum(cameraTypeDropdown, typeof(CameraType));
            cameraTypeDropdown.onValueChanged.AddListener(index => currentCameraType = (CameraType)index);

            bool hasFrontCamera = Media.Camera.IsCameraAvailable(CameraType.Front);
            bool hasRearCamera = Media.Camera.IsCameraAvailable(CameraType.Rear);

            demoUtils.DisplayBool(frontCameraSupportedToggle, hasFrontCamera, hasFrontCamera ? "Front Camera Available" : "Front Camera Unavailable");
            demoUtils.DisplayBool(backCameraSupportedToggle, hasRearCamera, hasRearCamera ? "Rear Camera Available" : "Rear Camera Unavailable");
        }

        public void TakePicture()
        {
            Media.Camera.TakePicture(currentCameraType, CameraCallback);
        }

        public void RecordVideo()
        {
            Media.Camera.RecordVideo(currentCameraType, CameraCallback);
        }

        public void PickFromGallery()
        {
            Media.Gallery.Pick(GalleryPickCallback);
        }

        public void SaveImage()
        {
            if (displayImage == null || displayImage.texture == null)
            {
                NativeUI.Alert("Error", "Please pick an image first.");
                return;
            }

            if (string.IsNullOrEmpty(imageNameInputField.text))
            {
                NativeUI.Alert("Error", "Image name can't be empty.");
                return;
            }

            Media.Gallery.SaveImage(
                (Texture2D)displayImage.texture,
                imageNameInputField.text,
                (ImageFormat)imageFormatDropdown.value,
                SaveImageCallback);
        }

        public void UpdateDisplayImage(Texture2D texture)
        {
            if (texture == null)
                return;

            displayImage.texture = texture;
            SizeToParent(parentTransform, displayImageTransform, displayImage, imagePadding);
        }

        public void CameraCallback(string error, MediaResult result)
        {
            if (result != null)
            {
                AddView(result);
                return;
            }

            AddViewWithError(error ?? "Unknown error.");
        }

        public void GalleryPickCallback(string error, MediaResult[] result)
        {
            if (error != null)
            {
                AddViewWithError(error ?? "Unknown error.");
                return;
            }

            if (result != null)
            {
                foreach (var item in result)
                    AddView(item);
                return;
            }
        }

        public void SaveImageCallback(string error)
        {
            NativeUI.Alert("Save image", error == null ? "The image has been saved successfully." : "Failed to save image. Error: " + error);
        }

        public void AddViewWithError(string error)
        {
            var view = Instantiate(viewPrefab, viewRoot);
            view.gameObject.SetActive(true);
            view.UpdateWithError(error);
            displayedViews.Add(view);
        }

        public void AddView(MediaResult model)
        {
            var view = Instantiate(viewPrefab, viewRoot);
            view.VideoBackground = videoBackground;
#if UNITY_IOS || UNITY_ANDROID
            view.VideoControlMode = videoControlMode;
            view.VideoScalingMode = videoScalingMode;
#endif
            view.gameObject.SetActive(true);
            view.UpdateWithModel(model);
            displayedViews.Add(view);
        }

        public void RemoveDisplayedView(MediaResultView view)
        {
            if (displayedViews == null || view == null)
                return;

            Destroy(view.gameObject);
            displayedViews.Remove(view);
        }

        public void RandomDisplayImage()
        {
            UpdateDisplayImage(GenerateRandomTexture2D());
        }

        public void ClearDisplayImage()
        {
            displayImage.texture = null;
        }

        private void ClearDisplayedViews()
        {
            foreach (var view in displayedViews)
                Destroy(view.gameObject);
            displayedViews.Clear();
        }

        private void InitDropdownWithEnum(Dropdown dropdown, Type enumType)
        {
            dropdown.ClearOptions();
            var options = new Dropdown.OptionDataList();

            foreach (var value in Enum.GetValues(enumType))
                options.options.Add(new Dropdown.OptionData(value.ToString()));

            dropdown.AddOptions(options.options);
            dropdown.value = 0;
        }

        private Texture2D GenerateRandomTexture2D()
        {
            Texture2D texture = new Texture2D((int)randomTextureWidth, (int)randomTextureHeight);
            Color[] colors = texture.GetPixels();

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    int index = x + y * texture.width;
                    Color color = randomTextureColors[UnityEngine.Random.Range(0, randomTextureColors.Length)];
                    colors[index] = color;
                }
            }

            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }

        private Vector2 SizeToParent(RectTransform parent, RectTransform imageTransform, RawImage image, float padding = 0)
        {
            float width = 0, height = 0;

            if (image.texture != null)
            {
                if (!parent)
                    return imageTransform.sizeDelta;

                padding = 1 - padding;
                float ratio = image.texture.width / (float)image.texture.height;
                var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);

                if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
                    bounds.size = new Vector2(bounds.height, bounds.width);

                height = bounds.height * padding;
                width = height * ratio;
                if (width > bounds.width * padding)
                {
                    width = bounds.width * padding;
                    height = width / ratio;
                }
            }

            imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            return imageTransform.sizeDelta;
        }
    }
}
