using UnityEngine;
using System.Collections;
using System;
using EasyMobile.Internal;

namespace EasyMobile
{
    public sealed class AnimatedClip : IDisposable
    {
        /// <summary>
        /// The width of this clip in pixels.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; private set; }

        /// <summary>
        /// The height of this clip in pixels.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; private set; }

        /// <summary>
        /// The FPS of this clip.
        /// </summary>
        /// <value>The frame per second.</value>
        public int FramePerSecond { get; private set; }

        /// <summary>
        /// The length of this clip in seconds.
        /// </summary>
        /// <value>The length.</value>
        public float Length { get; private set; }

        /// <summary>
        /// The frames of this clip.
        /// </summary>
        /// <value>The frames.</value>
        public Texture[] Frames { get; private set; }

        // Whether this object is disposed.
        private bool isDisposed = false;

        public AnimatedClip(int width, int height, int fps, Texture[] frames)
        {
            this.Width = width;
            this.Height = height;
            this.FramePerSecond = fps;
            this.Frames = frames;
            this.Length = (float)frames.Length / fps;
        }

        /// <summary>
        /// Sets filter mode for all texture frames of the clip.
        /// </summary>
        /// <param name="filterMode"></param>
        public void SetFilterMode(FilterMode filterMode)
        {
            foreach (var tex in Frames)
                tex.filterMode = filterMode;
        }

        ~AnimatedClip()
        {
            Action cleanAction = () => Cleanup(Frames);
            RuntimeHelper.RunOnMainThread(cleanAction);
        }

        public bool IsDisposed()
        {
            return isDisposed;
        }

        /// <summary>
        /// Release all the <see cref="RenderTexture"/>(s) in <see cref="Frames"/>.
        /// This method should only be called in main thread.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed)
                return;

            Cleanup(Frames);

            // Obviates the need for the garbage collector to keep this object alive and call the finalizer.
            GC.SuppressFinalize(this);

            isDisposed = true;
        }

        private void Cleanup(Texture[] frames)
        {
            if (frames == null)
                return;

            foreach (var rt in frames)
            {
                if (rt != null)
                {
                    if (rt is RenderTexture)
                        (rt as RenderTexture).Release();
                    UnityEngine.Object.Destroy(rt);
                }
            }
        }
    }
}
