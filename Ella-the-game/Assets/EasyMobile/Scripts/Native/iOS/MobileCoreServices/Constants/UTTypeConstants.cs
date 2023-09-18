#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EasyMobile.iOS.Foundation;
using EasyMobile.Internal.iOS;
using EasyMobile.Internal;

namespace EasyMobile.iOS.MobileCoreServices
{
    internal static partial class UTTypeConstants
    {
        #region UTI Image Content Types

        /// <summary>
        /// The abstract type identifier for image data.
        /// </summary>
        public static NSString kUTTypeImage
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeImage); }
        }

        /// <summary>
        /// The type identifier for a JPEG image.
        /// </summary>
        public static NSString kUTTypeJPEG
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeJPEG); }
        }

        /// <summary>
        /// The type identifier for a JPEG-2000 image.
        /// </summary>
        public static NSString kUTTypeJPEG2000
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeJPEG2000); }
        }

        /// <summary>
        /// The type identifier for a TIFF image.
        /// </summary>
        public static NSString kUTTypeTIFF
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeTIFF); }
        }

        /// <summary>
        /// The type identifier for a Quickdraw PICT.
        /// </summary>
        public static NSString kUTTypePICT
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypePICT); }
        }

        /// <summary>
        /// The type identifier for a GIF image.
        /// </summary>
        public static NSString kUTTypeGIF
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeGIF); }
        }

        /// <summary>
        /// The type identifier for a PNG image.
        /// </summary>
        public static NSString kUTTypePNG
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypePNG); }
        }

        /// <summary>
        /// The type identifier for a QuickTime image. Corresponds to the 'qtif' OSType.
        /// </summary>
        public static NSString kUTTypeQuickTimeImage
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeQuickTimeImage); }
        }

        /// <summary>
        /// The type identifier for Apple icon data.
        /// </summary>
        public static NSString kUTTypeAppleICNS
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeAppleICNS); }
        }

        /// <summary>
        /// The type identifier for a Windows bitmap.
        /// </summary>
        public static NSString kUTTypeBMP
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeBMP); }
        }

        /// <summary>
        /// The type identifier for Windows icon data.
        /// </summary>
        public static NSString kUTTypeICO
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeICO); }
        }

        #endregion

        #region UTI Audio Visual Content Types

        /// <summary>
        /// An abstract type identifier for audio and/or video content.
        /// </summary>
        public static NSString kUTTypeAudiovisualContent
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeAudiovisualContent); }
        }

        /// <summary>
        /// An abstract type identifier for a media format which may contain both video and audio.
        /// Corresponds to what users would label a "movie"
        /// </summary>
        public static NSString kUTTypeMovie
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeMovie); }
        }

        /// <summary>
        /// An abstract type identifier for pure video data(no audio).
        /// </summary>
        public static NSString kUTTypeVideo
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeVideo); }
        }

        /// <summary>
        /// An abstract type identifier for pure audio data (no video).
        /// </summary>
        public static NSString kUTTypeAudio
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeAudio); }
        }

        /// <summary>
        /// The type identifier for a QuickTime movie.
        /// </summary>
        public static NSString kUTTypeQuickTimeMovie
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeQuickTimeMovie); }
        }

        /// <summary>
        /// The type identifier for a MPEG-1 or MPEG-2 movie.
        /// </summary>
        public static NSString kUTTypeMPEG
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeMPEG); }
        }

        /// <summary>
        /// The type identifier for a MPEG-4 movie.
        /// </summary>
        public static NSString kUTTypeMPEG4
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeMPEG4); }
        }

        /// <summary>
        /// The type identifier for MP3 audio.
        /// </summary>
        public static NSString kUTTypeMP3
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeMP3); }
        }

        /// <summary>
        /// The type identifier for a MPEG-4 audio layer (.m4a, or the MIME type audio/MP4).
        /// </summary>
        public static NSString kUTTypeMPEG4Audio
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeMPEG4Audio); }
        }

        /// <summary>
        /// The type identifier for Apple protected MPEG4 format (.m4p, iTunes music store format).
        /// </summary>
        public static NSString kUTTypeAppleProtectedMPEG4Audio
        {
            get { return GetConstantValue(() => UTTypeConstants.kUTTypeAppleProtectedMPEG4Audio); }
        }

        #endregion

        #region Private

        private const string FrameworkName = "MobileCoreServices";

        private static NSString GetConstantValue(Expression<Func<NSString>> constantExp)
        {
            return iOSInteropUtil.LookupStringConstant(constantExp, FrameworkName);
        }

        #endregion
    }
}
#endif