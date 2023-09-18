using UnityEngine;
using System.Collections;
using System;

namespace EasyMobile
{
    internal class GifExportTask
    {
        internal int taskId;
        internal AnimatedClip clip;
        internal Color32[][] imageData;
        internal string filepath;
        internal int loop;
        /// <summary>
        /// The sample factor used in the color quantization algorithm,
        /// determining the color quality of the output GIF.
        /// </summary>
        internal int sampleFac;
        internal bool isExporting;
        internal bool isDone;
        internal float progress;
        internal Action<AnimatedClip, float> exportProgressCallback;
        internal Action<AnimatedClip, string> exportCompletedCallback;
        internal System.Threading.ThreadPriority workerPriority;
    }
}

