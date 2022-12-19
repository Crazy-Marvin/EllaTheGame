/*
 * 
 * This Recorder is inspired by the Recorder class of the Moments plugin by Chman (Thomas Hourdel) at https://github.com/Chman/Moments.
 * 
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using EM_Moments;

namespace EasyMobile
{
    using Min = EM_Moments.MinAttribute;

    [AddComponentMenu("Easy Mobile/Recorder"), RequireComponent(typeof(Camera)), DisallowMultipleComponent]
    public sealed class Recorder : MonoBehaviour
    {
        public enum RecorderState
        {
            Stopped = 1,
            Recording = 2
        }

        /// <summary>
        /// Determines whether the height should be computed automatically from the current width and aspect ratio.
        /// </summary>
        /// <value><c>true</c> if auto aspect; otherwise, <c>false</c>.</value>
        public bool AutoHeight { get { return _autoHeight; } }

        /// <summary>
        /// Image width in pixels.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get { return _width; } }

        /// <summary>
        /// Image height in pixels.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get { return _height; } }

        /// <summary>
        /// Gets the frame per second.
        /// </summary>
        /// <value>The frame per second.</value>
        public int FramePerSecond { get { return _framePerSecond; } }

        /// <summary>
        /// The length of the GIF in seconds.
        /// </summary>
        /// <value>The length.</value>
        public float Length { get { return _length; } }

        /// <summary>
        /// Gets the current state of the recorder.
        /// </summary>
        /// <value>The state.</value>
        public RecorderState State { get { return _state; } }

        /// <summary>
        /// Gets the camera that this recorder operates on.
        /// </summary>
        /// <value>The target camera.</value>
        public Camera TargetCamera
        {
            get
            {
                if (_targetCamera == null)
                    _targetCamera = GetComponent<Camera>();

                return _targetCamera;
            }
        }

        [SerializeField]
        bool _autoHeight = true;

        [SerializeField, Min(8)]
        int _width = 320;

        [SerializeField, Min(8)]
        int _height = 480;

        [SerializeField, Range(1, 30)]
        int _framePerSecond = 15;

        [SerializeField, Range(0.1f, 30f)]
        float _length = 3f;

        [SerializeField]
        RecorderState _state = RecorderState.Stopped;

        Camera _targetCamera;
        int maxFrameCount;
        float pastTime;
        float timePerFrame;
        Queue<RenderTexture> recordedFrames;
        ReflectionUtils<Recorder> reflectionUtils;

        #region Static methods

        /// <summary>
        /// Calculates height based on the specified width and the aspect ratio of the specified camera.
        /// </summary>
        /// <returns>The auto height.</returns>
        /// <param name="width">Width.</param>
        /// <param name="targetCam">Target camera.</param>
        public static int CalculateAutoHeight(int width, Camera targetCam)
        {
            return Mathf.RoundToInt((float)width / targetCam.aspect);
        }

        /// <summary>
        /// Estimates the memory use by the recorder.
        /// </summary>
        /// <returns>The memory use.</returns>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="fps">Fps.</param>
        /// <param name="length">Length.</param>
        public static float EstimateMemoryUse(int width, int height, int fps, float length)
        {
            float mem = fps * length;
            mem *= width * height * 4;
            mem /= 1024 * 1024;
            return mem;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Configures the recorder with the specified parameters.
        /// Use this to change the recorder settings in a script.
        /// This can't be done while recording is in progress.
        /// </summary>
        /// <param name="autoHeight">If set to <c>true</c>, the height will be computed automatically.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height, will be ignored if autoHeight is enable.</param>
        /// <param name="fps">Frame per second.</param>
        /// <param name="length">Length in seconds.</param>
        public void Setup(bool autoHeight, int width, int height, int fps, float length)
        {
            if (_state == RecorderState.Recording)
            {
                Debug.LogWarning("Attempting to init the recorder while a recording is in process.");
                return;
            }

            // Start fresh
            FlushMemory();

            // Validate and set values
            _autoHeight = autoHeight;
            reflectionUtils.ConstrainMin(x => x._width, width);
            if (!_autoHeight)
                reflectionUtils.ConstrainMin(x => x._height, height);
            reflectionUtils.ConstrainRange(x => x._framePerSecond, fps);
            reflectionUtils.ConstrainRange(x => x._length, length);

            // Recalculate stuff
            Init();
        }

        /// <summary>
        /// Starts recording.
        /// </summary>
        public void Record()
        {
            _state = RecorderState.Recording;
#if EM_URP
            EM_GIFRecorderFeature.SetBlitRT(TargetCamera, null);
#endif
        }

        /// <summary>
        /// Stops recording.
        /// </summary>
        /// <returns>The recorded clip, which could be empty if the recording was not started.</returns>
        public AnimatedClip Stop()
        {
            _state = RecorderState.Stopped;

            if (recordedFrames.Count == 0)
                Debug.LogWarning("Nothing recorded, an empty clip will be returned.");

            var clip = new AnimatedClip(_width, _height, _framePerSecond, recordedFrames.ToArray());

            // Since we return the clip, we also hand over the control of frame data to the holder of that clip
            // (e.g. the frames can be destroyed without our knowledge), therefore we should not hold any references to this data.
            recordedFrames.Clear();

            return clip;
        }

        /// <summary>
        /// Determines whether a recording is in process.
        /// </summary>
        /// <returns><c>true</c> if this instance is recording; otherwise, <c>false</c>.</returns>
        public bool IsRecording()
        {
            return _state == RecorderState.Recording;
        }

        #endregion

        #region Unity events

        void Awake()
        {
            reflectionUtils = new ReflectionUtils<Recorder>(this);
            recordedFrames = new Queue<RenderTexture>();

            Init();
        }

        void OnDestroy()
        {
            _state = RecorderState.Stopped;
            FlushMemory();
#if EM_URP
            EM_GIFRecorderFeature.SetBlitRT(TargetCamera, null);
#endif
        }
#if EM_URP
        private void LateUpdate()
        {
            if (_state != RecorderState.Recording)
                return;
            pastTime += Time.unscaledDeltaTime;

            if (pastTime >= timePerFrame)
            {
                pastTime -= timePerFrame;
                RenderTexture targetBlitRT = EM_GIFRecorderFeature.GetBlitRT(TargetCamera);

                if (targetBlitRT != null)
                {
                    //Enqueue last frame blit result
                    recordedFrames.Enqueue(targetBlitRT);
                }

                RenderTexture tempRT = null;

                // Recycle old frames -> discard old content
                if (recordedFrames.Count >= maxFrameCount)
                    tempRT = recordedFrames.Dequeue();

                if (tempRT == null)
                {
                    tempRT = new RenderTexture(_width, _height, 0, RenderTextureFormat.ARGB32);
                    tempRT.wrapMode = TextureWrapMode.Clamp;
                    tempRT.filterMode = FilterMode.Bilinear;
                    tempRT.anisoLevel = 0;
                }
                else
                {
                    // Discard the reused RT's content before rendering new content into it
                    // to avoid a "restore" operation, which is costly on many mobile GPUs and
                    // multi-GPU systems and will cause Unity to issue a warning.
                    tempRT.DiscardContents();
                }
                EM_GIFRecorderFeature.SetBlitRT(TargetCamera, tempRT);
            }
        }
#else
        // OnRenderImage is sent to all scripts attached to a camera.
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_state != RecorderState.Recording)
            {
                Graphics.Blit(source, destination);
                return;
            }

            pastTime += Time.unscaledDeltaTime;

            if (pastTime >= timePerFrame)
            {
                pastTime -= timePerFrame;

                RenderTexture tempRT = null;

                // Recycle old frames -> discard old content
                if (recordedFrames.Count >= maxFrameCount)
                    tempRT = recordedFrames.Dequeue();

                if (tempRT == null)
                {
                    tempRT = new RenderTexture(_width, _height, 0, RenderTextureFormat.ARGB32);
                    tempRT.wrapMode = TextureWrapMode.Clamp;
                    tempRT.filterMode = FilterMode.Bilinear;
                    tempRT.anisoLevel = 0;
                }
                else
                {
                    // Discard the reused RT's content before rendering new content into it
                    // to avoid a "restore" operation, which is costly on many mobile GPUs and
                    // multi-GPU systems and will cause Unity to issue a warning.
                    tempRT.DiscardContents();
                }

                Graphics.Blit(source, tempRT);
                recordedFrames.Enqueue(tempRT);
            }

            Graphics.Blit(source, destination);
        }
#endif
        #endregion

        #region Methods

        void Init()
        {
            maxFrameCount = Mathf.RoundToInt(_length * _framePerSecond);
            timePerFrame = 1f / _framePerSecond;
            pastTime = 0f;

            // Calculate automatic height if needed.
            if (_autoHeight)
                reflectionUtils.ConstrainMin(x => x._height, CalculateAutoHeight(_width, TargetCamera));
        }

        void FlushMemory()
        {
            if (recordedFrames != null)
            {
                foreach (var rt in recordedFrames)
                {
                    rt.Release();
                    Flush(rt);
                }
                recordedFrames.Clear();
            }
        }

        void Flush(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                Destroy(obj);
            else
                DestroyImmediate(obj);
#else
            Destroy(obj);
#endif
        }

        #endregion
    }
}
