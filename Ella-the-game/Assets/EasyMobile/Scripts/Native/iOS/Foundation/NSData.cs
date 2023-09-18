#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;

namespace EasyMobile.iOS.Foundation
{
    internal class NSData : iOSObjectProxy
    {
        internal NSData(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        #region NSData API

        /// <summary>
        /// Creates an empty data object.
        /// </summary>
        public static NSData Data()
        {
            return FromPointer(C.NSData_data());
        }

        /// <summary>
        /// Creates a data object containing a given number of bytes copied from a given buffer.
        /// </summary>
        /// <returns>The with bytes.</returns>
        /// <param name="bytes">Bytes.</param>
        /// <param name="length">Length.</param>
        public static NSData DataWithBytes(byte[] bytes, uint length)
        {
            return bytes == null ? null : FromPointer(C.NSData_dataWithBytes(bytes, length));
        }

        /// <summary>
        /// Creates a data object that holds a given number of bytes from a given buffer.
        /// </summary>
        /// <returns>The with bytes no copy.</returns>
        /// <param name="bytes">Bytes.</param>
        /// <param name="length">Length.</param>
        public static NSData DataWithBytesNoCopy(byte[] bytes, uint length)
        {
            return bytes == null ? null : FromPointer(C.NSData_dataWithBytesNoCopy(bytes, length));
        }

        /// <summary>
        /// Creates a data object by reading every byte from the file at a given path.
        /// </summary>
        /// <returns>The with contents of file.</returns>
        /// <param name="path">Path.</param>
        public static NSData DataWithContentsOfFile(NSString path)
        {
            return path == null ? null : FromPointer(C.NSData_dataWithContentsOfFile(path.ToPointer()));
        }

        /// <summary>
        /// Creates a data object containing the data from the location specified by a given URL.
        /// </summary>
        /// <returns>The with contents of UR.</returns>
        /// <param name="url">URL.</param>
        public static NSData DataWithContentsOfURL(NSURL url)
        {
            return url == null ? null : FromPointer(C.NSData_dataWithContentsOfURL(url.ToPointer()));
        }

        /// <summary>
        /// The number of bytes contained by the data object.
        /// </summary>
        /// <value>The length.</value>
        public uint Length
        {
            get { return C.NSData_length(SelfPtr()); }
        }

        /// <summary>
        /// A string that contains a hexadecimal representation of the data object’s contents in a property list format.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                    C.NSData_description(SelfPtr(), strBuffer, strLen));
            }
        }

        /// <summary>
        /// Copies a number of bytes from the start of the data object into a given buffer.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="length">Length.</param>
        /// <remarks>
        /// The number of bytes copied is the smaller of the length parameter and the length of the data encapsulated in the object.
        /// </remarks>
        public byte[] GetBytes(uint length)
        {
            // The number of bytes copied is the smaller of the length parameter 
            // and the length of the data encapsulated in the object.
            // So we don't want to allocate an array bigger than the actual
            // data that can be read.
            var smallerLen = Math.Min(this.Length, length);
            var buffer = new byte[smallerLen];
            C.NSData_getBytes_length(SelfPtr(), buffer, smallerLen);
            return buffer;
        }

        /// <summary>
        /// Copies a range of bytes from the data object into a given buffer.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="range">Range.</param>
        /// <remarks>
        /// If range isn’t within the receiver’s range of bytes, an NSRangeException is raised.
        /// </remarks>
        public byte[] GetBytes(NSRange range)
        {
            var buffer = new byte[range.length];
            C.NSData_getBytes_range(SelfPtr(), buffer, ref range);
            return buffer;
        }

        #endregion

        #region Private

        /// <summary>
        /// Creates a NSData object from a pointer and release that pointer.
        /// </summary>
        /// <returns>The NSData object.</returns>
        /// <param name="pointer">Pointer.</param>
        private static NSData FromPointer(IntPtr pointer)
        {
            if (PInvokeUtil.IsNotNull(pointer))
            {
                var obj = new NSData(pointer);
                CoreFoundation.CFFunctions.CFRelease(pointer);
                return obj;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region C# Utils

        public byte[] ToBytes()
        {
            return GetBytes(Length);
        }

        #endregion

        #region C Wrapper

        private static class C
        {
            // Creating Data
            [DllImport("__Internal")]
            internal static extern /* NSData */IntPtr NSData_data();

            [DllImport("__Internal")]
            internal static extern /* NSData */IntPtr NSData_dataWithBytes(byte[] bytes, uint length);

            [DllImport("__Internal")]
            internal static extern /* NSData */IntPtr NSData_dataWithBytesNoCopy(byte[] bytes, uint length);

            // Reading Data from a File
            [DllImport("__Internal")]
            internal static extern /* NSData */IntPtr NSData_dataWithContentsOfFile(/* NSString */IntPtr path);

            [DllImport("__Internal")]
            internal static extern /* NSData */IntPtr NSData_dataWithContentsOfURL(/* NSURL */IntPtr url);

            // Accessing Underlying Bytes.
            [DllImport("__Internal")]
            internal static extern IntPtr NSData_bytes(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void NSData_getBytes_length(
                HandleRef selfPtr, 
                [Out] /* from(unsigned char *) */ byte[] buffer, uint length);

            [DllImport("__Internal")]
            internal static extern void NSData_getBytes_range(
                HandleRef selfPtr, 
                [Out] /* from(unsigned char *) */ byte[] buffer, ref NSRange range);

            // Testing Data.
            [DllImport("__Internal")]
            internal static extern uint NSData_length(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern int NSData_description(
                HandleRef selfPtr, 
                [Out] /* from(char *) */ byte[] strBuffer, int charCount);

        }

        #endregion
    }

}
#endif