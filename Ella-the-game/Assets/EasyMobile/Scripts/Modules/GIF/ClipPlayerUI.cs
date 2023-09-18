using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EasyMobile
{
    [AddComponentMenu("Easy Mobile/Clip Player (UI)"), RequireComponent(typeof(RawImage)), DisallowMultipleComponent]
    public class ClipPlayerUI : MonoBehaviour, IClipPlayer
    {
        /// <summary>
        /// Gets or sets the scale mode.
        /// </summary>
        /// <value>The scale mode.</value>
        public ClipPlayerScaleMode ScaleMode
        {
            get { return _scaleMode; }
            set { _scaleMode = value; }
        }

        [SerializeField]
        ClipPlayerScaleMode _scaleMode = ClipPlayerScaleMode.AutoHeight;

        // Projecting object
        RawImage rawImage;
        RectTransform rt;
        IEnumerator playCoroutine;
        bool isPaused;

        void Awake()
        {
            rawImage = GetComponent<RawImage>();
            rt = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Play the specified clip.
        /// </summary>
        /// <param name="clip">Clip.</param>
        /// <param name="startDelay">Optional delay before the playing starts.</param>
        /// <param name="loop">If set to <c>true</c> loop indefinitely.</param>
        public void Play(AnimatedClip clip, float startDelay = 0, bool loop = true)
        {
            if (clip == null || clip.Frames.Length == 0 || clip.IsDisposed())
            {
                Debug.LogError("Attempted to play an empty or disposed clip.");
                return;
            }

            Stop();
            Resize(clip);
            isPaused = false;

            playCoroutine = CRPlay(clip, startDelay, loop);
            StartCoroutine(playCoroutine);
        }

        /// <summary>
        /// Pauses the player.
        /// </summary>
        public void Pause()
        {
            isPaused = true;
        }

        /// <summary>
        /// Resumes playing.
        /// </summary>
        public void Resume()
        {
            isPaused = false;
        }

        /// <summary>
        /// Stops playing.
        /// </summary>
        public void Stop()
        {
            if (playCoroutine != null)
            {
                StopCoroutine(playCoroutine);
                playCoroutine = null;
            }
        }

        /// <summary>
        /// Resizes this player according to the predefined scale mode and the clip's aspect ratio.
        /// </summary>
        /// <param name="clip">Clip.</param>
        void Resize(AnimatedClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("Could not resize player: clip is null.");
                return;
            }

            if (_scaleMode == ClipPlayerScaleMode.None)
            {
                return;
            }
            else
            {
                float aspectRatio = (float)clip.Width / clip.Height;

                if (_scaleMode == ClipPlayerScaleMode.AutoHeight)
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.x / aspectRatio);
                else if (_scaleMode == ClipPlayerScaleMode.AutoWidth)
                    rt.sizeDelta = new Vector2(rt.sizeDelta.y * aspectRatio, rt.sizeDelta.y);
            }
        }

        IEnumerator CRPlay(AnimatedClip clip, float startDelay, bool loop)
        {
            float timePerFrame = 1f / clip.FramePerSecond;
            bool hasDelayed = false;

            do
            {
                for (int i = 0; i < clip.Frames.Length; i++)
                {
                    rawImage.texture = clip.Frames[i];
                    yield return new WaitForSeconds(timePerFrame);

                    // Wait at the 1st frame if startDelay is required
                    if (startDelay > 0 && !hasDelayed && i == 0)
                    {
                        hasDelayed = true;
                        yield return new WaitForSeconds(startDelay);
                    }

                    // Standby if paused
                    if (isPaused)
                        yield return null;
                }
            } while (loop);
        }
    }
}
