using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EasyMobile
{
    [AddComponentMenu("Easy Mobile/Clip Player"), RequireComponent(typeof(MeshRenderer)), DisallowMultipleComponent]
    public class ClipPlayer : MonoBehaviour, IClipPlayer
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
        Material mat;
        IEnumerator playCoroutine;
        bool isPaused;

        void Awake()
        {
            mat = GetComponent<MeshRenderer>().material;
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
            if (_scaleMode == ClipPlayerScaleMode.None)
            {
                return;
            }
            else
            {
                float aspectRatio = (float)clip.Width / clip.Height;
                var scale = transform.localScale;

                if (_scaleMode == ClipPlayerScaleMode.AutoHeight)
                    scale.y = scale.x / aspectRatio;
                else if (_scaleMode == ClipPlayerScaleMode.AutoWidth)
                    scale.x = scale.y * aspectRatio;

                transform.localScale = scale;
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
                    mat.mainTexture = clip.Frames[i];
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
