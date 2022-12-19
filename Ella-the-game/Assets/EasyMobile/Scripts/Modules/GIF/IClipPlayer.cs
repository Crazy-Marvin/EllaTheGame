using UnityEngine;
using System.Collections;

namespace EasyMobile
{
    public interface IClipPlayer
    {
        // Properties
        ClipPlayerScaleMode ScaleMode { get; set; }

        // Methods
        void Play(AnimatedClip clip, float startDelay = 0, bool loop = true);

        void Pause();

        void Resume();

        void Stop();
    }
}
