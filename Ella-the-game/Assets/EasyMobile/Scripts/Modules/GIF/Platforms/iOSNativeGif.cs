#if UNITY_IOS
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;

namespace EasyMobile.Internal.Gif.iOS
{
    internal static class iOSNativeGif
    {
        #region Gif Encoding

        internal static event Action<int, float> GifExportProgress;
        internal static event Action<int, string> GifExportCompleted;

        [MonoPInvokeCallback(typeof(C.NativeExportProgressDelegate))]
        private static void GifExportProgressCallback(int taskId, float progress)
        {
            if (GifExportProgress != null)
                GifExportProgress(taskId, progress);
        }

        [MonoPInvokeCallback(typeof(C.NativeExportCompletedDelegate))]
        private static void GifExportCompletedCallback(int taskId, string filepath)
        {
            if (GifExportCompleted != null)
                GifExportCompleted(taskId, filepath);

            // Free the GC handles and IntPtr
            foreach (var gch in gcHandles[taskId])
            {
                gch.Free();
            }

            gcHandles.Remove(taskId);
        }

        // Store the array of GCHandles for each exporting task
        private static Dictionary<int, GCHandle[]> gcHandles;

        internal static void ExportGif(GifExportTask task)
        {
            var taskId = task.taskId;
            var filepath = task.filepath;
            var width = task.clip.Width;
            var height = task.clip.Height;
            var loop = task.loop;
            var fps = task.clip.FramePerSecond;
            var sampleFac = task.sampleFac;
            var frameCount = task.clip.Frames.Length;
            var imageData = task.imageData;
            var threadPriority = EncodeThreadPriority(task.workerPriority);

            var gcHandleArray = new GCHandle[imageData.Length];
            var ptrArray = new IntPtr[imageData.Length];

            for (int i = 0; i < imageData.Length; i++)
            {
                gcHandleArray[i] = GCHandle.Alloc(imageData[i], GCHandleType.Pinned);
                ptrArray[i] = gcHandleArray[i].AddrOfPinnedObject();
            }

            if (gcHandles == null)
                gcHandles = new Dictionary<int, GCHandle[]>();

            gcHandles.Add(taskId, gcHandleArray);

            C.EM_ExportGif(taskId,
                filepath,
                width,
                height,
                loop,
                fps,
                sampleFac,
                frameCount,
                ptrArray,
                threadPriority,
                GifExportProgressCallback,
                GifExportCompletedCallback);
        }

        #endregion

        #region Gif Decoding

        // Store the GCHandles of all buffers created for each decoding task
        private static Dictionary<int, GifDecodeResources> sDecodeTasks;

        private static Dictionary<int, GifDecodeResources> DecodeTasks
        {
            get
            {
                if (sDecodeTasks == null)
                    sDecodeTasks = new Dictionary<int, GifDecodeResources>();
                return sDecodeTasks;
            }
        }

        [MonoPInvokeCallback(typeof(C.NativeGetFrameMetadataHolderDelegate))]
        private static void GetFrameMetadataHolderFunc(int taskId, int frameCount, IntPtr pointerHolder)
        {
            if (frameCount <= 0 || PInvokeUtil.IsNull(pointerHolder))
            {
                Debug.LogError("GetFrameMetadataHolderFunc Error: Invalid buffer pointers from native side.");
                return;
            }

            var gcHandleArray = new GCHandle[frameCount];
            var ptrArray = new IntPtr[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                gcHandleArray[i] = GCHandle.Alloc(new GifFrameMetadata(), GCHandleType.Pinned);
                ptrArray[i] = gcHandleArray[i].AddrOfPinnedObject();   // fill the pointers sent by native code with the object pointer we've just pinned.
            }

            // Copy pointers to unmanaged holder.
            C.EM_CopyPointerArray(pointerHolder, ptrArray, frameCount);

            // Cache GCHandles.
            DecodeTasks[taskId].frameMetadataHandles = gcHandleArray;
        }

        [MonoPInvokeCallback(typeof(C.NativeGetImageDataHolderDelegate))]
        private static void GetImageDataHolderFunc(int taskId, int frameCount, int frameWidth, int frameHeight, IntPtr pointerHolder)
        {
            if (frameCount <= 0 || PInvokeUtil.IsNull(pointerHolder))
            {
                Debug.LogError("GetImageDataHolderFunc Error: Invalid buffer pointers from native side.");
                return;
            }

            // Create managed buffer for unmanaged code to fill data in.
            int frameSize = frameWidth * frameHeight;
            var buff = new Color32[frameCount][];

            for (int i = 0; i < frameCount; i++)
                buff[i] = new Color32[frameSize];

            var gcHandleArray = new GCHandle[frameCount];
            var ptrArray = new IntPtr[frameCount];

            for (int i = 0; i < buff.Length; i++)
            {
                gcHandleArray[i] = GCHandle.Alloc(buff[i], GCHandleType.Pinned);
                ptrArray[i] = gcHandleArray[i].AddrOfPinnedObject();   // fill the pointers sent by native code with the object pointer we've just pinned.
            }

            // Copy pointers to unmanaged holder.
            C.EM_CopyPointerArray(pointerHolder, ptrArray, frameCount);

            // Cache GCHandles.
            DecodeTasks[taskId].imageDataHandles = gcHandleArray;
        }

        [MonoPInvokeCallback(typeof(C.NativeGifDecodingCompletedDelegate))]
        private static void GifDecodingCompleteCallback(int taskId)
        {
            if (!DecodeTasks.ContainsKey(taskId) || DecodeTasks[taskId] == null)
            {
                Debug.LogErrorFormat("Complete decoding GIF image with invalid task ID {0}. Please check!", taskId);
                return;
            }

            var taskResources = DecodeTasks[taskId];
            GifMetadata gifMetadata;
            GifFrameMetadata[] gifFrameMetadata = null;
            Color32[][] imageData = null;

            try
            {
                gifMetadata = (GifMetadata)taskResources.gifMetadataHandle.Target;

                if (taskResources.frameMetadataHandles != null || taskResources.imageDataHandles != null)
                {
                    imageData = new Color32[taskResources.imageDataHandles.Length][];
                    gifFrameMetadata = new GifFrameMetadata[taskResources.frameMetadataHandles.Length];

                    for (int i = 0; i < taskResources.frameMetadataHandles.Length; i++)
                        gifFrameMetadata[i] = (GifFrameMetadata)taskResources.frameMetadataHandles[i].Target;

                    for (int j = 0; j < taskResources.imageDataHandles.Length; j++)
                        imageData[j] = (Color32[])taskResources.imageDataHandles[j].Target;
                }

                if (taskResources.completeCallback != null)
                {
                    var callback = taskResources.completeCallback;  // cache the callback reference as we'll reset taskResources soon.
                    RuntimeHelper.RunOnMainThread(() =>
                    {
                        callback(taskId, gifMetadata, gifFrameMetadata, imageData);
                    });
                }
            }
            catch (System.InvalidCastException e)
            {
                Debug.LogError("Error casting GCHandle back to decoding buffer:" + e);
                throw e;
            }
            finally
            {
                taskResources.gifMetadataHandle.Free();

                if (taskResources.frameMetadataHandles != null)
                    foreach (var handle in taskResources.frameMetadataHandles)
                        handle.Free();

                if (taskResources.imageDataHandles != null)
                    foreach (var handle in taskResources.imageDataHandles)
                        handle.Free();

                taskResources = null;
                DecodeTasks.Remove(taskId);
            }
        }

        internal static void DecodeGif(int taskId, string filepath, System.Threading.ThreadPriority workerPriority, DecodeCompleteCallback completeCallback)
        {
            DecodeGif(taskId, filepath, -1, workerPriority, completeCallback); // framesToRead == -1: read whole GIF
        }

        internal static void DecodeGif(int taskId, string filepath, int framesToRead, System.Threading.ThreadPriority workerPriority, DecodeCompleteCallback completeCallback)
        {
            var gifMetaHolderHandle = GCHandle.Alloc(new GifMetadata(), GCHandleType.Pinned);

            DecodeTasks[taskId] = new GifDecodeResources()
            {
                gifMetadataHandle = gifMetaHolderHandle,
                completeCallback = completeCallback
            };

            C.EM_DecodeGif(taskId,
            filepath,
            framesToRead,
            EncodeThreadPriority(workerPriority),
            gifMetaHolderHandle.AddrOfPinnedObject(),
            GetFrameMetadataHolderFunc,
            GetImageDataHolderFunc,
            GifDecodingCompleteCallback);
        }

        #endregion

        #region Helpers

        private static int EncodeThreadPriority(System.Threading.ThreadPriority priority)
        {
            switch (priority)
            {
                case System.Threading.ThreadPriority.Lowest:
                    return -2;
                case System.Threading.ThreadPriority.BelowNormal:
                    return -1;
                case System.Threading.ThreadPriority.Normal:
                    return 0;
                case System.Threading.ThreadPriority.AboveNormal:
                    return 1;
                case System.Threading.ThreadPriority.Highest:
                    return 2;
                default:
                    return 0;
            }
        }

        #endregion

        #region C Wrapper

        private static class C
        {
            internal delegate void NativeExportProgressDelegate(int taskId, float progress);
            internal delegate void NativeExportCompletedDelegate(int taskId, string filepath);
            internal delegate void NativeGetFrameMetadataHolderDelegate(int taskId, int frameCount, IntPtr pointerHolder);
            internal delegate void NativeGetImageDataHolderDelegate(int taskId, int frameCount, int frameWidth, int frameHeight, IntPtr pointerHolder);
            internal delegate void NativeGifDecodingCompletedDelegate(int taskId);

            [DllImport("__Internal")]
            internal static extern void EM_ExportGif(int taskId,
                                              string filepath,
                                              int width,
                                              int height,
                                              int loop,
                                              int fps,
                                              int sampleFac,
                                              int frameCount,
                                              IntPtr[] imageData,
                                              int threadPriority,
                                              NativeExportProgressDelegate exportingCallback,
                                              NativeExportCompletedDelegate exportCompletedCallback);

            [DllImport("__Internal")]
            internal static extern void EM_DecodeGif(int taskId,
               string filepath,
               int framesToRead,    // <= 0: read whole GIF
               int queuePriority,
               [In, Out]/* GifMetadata* */IntPtr gifMetadataBuff,
               NativeGetFrameMetadataHolderDelegate getMetadataHolder,
               NativeGetImageDataHolderDelegate getImageDataHolder,
               NativeGifDecodingCompletedDelegate completeCallback);

            [DllImport("__Internal")]
            internal static extern void EM_CopyPointerArray(IntPtr destPtrArray, IntPtr[] srcPtrArray, int length);
        }

        #endregion
    }
}
#endif
