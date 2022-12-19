#if UNITY_IOS
using System;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.CoreFoundation;

namespace EasyMobile.iOS.Foundation
{
    using C = InternalNSArray;

    /// <summary>
    /// This class is intended for working with iOS NSArray that
    /// contains interop objects. For native arrays of other types, consider
    /// using <see cref="PInvokeUtil.GetNativeArray"/>.
    /// </summary>
    internal class NSArray<T> : iOSObjectProxy
    {
        internal NSArray(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Creates a NSArray object from a pointer and release the pointer if required.
        /// </summary>
        /// <returns>The NSArray object.</returns>
        /// <param name="pointer">Pointer.</param>
        /// <param name="releasePointer">If set to <c>true</c> release pointer.</param>
        internal static NSArray<T> FromPointer(IntPtr pointer, bool releasePointer)
        {
            if (PInvokeUtil.IsNotNull(pointer))
            {
                var obj = new NSArray<T>(pointer);
                if (releasePointer)
                    CFFunctions.CFRelease(pointer);
                return obj;
            }
            else
            {
                return null;
            }
        }

        #region NSArray API

        /// <summary>
        /// The number of objects in the array.
        /// </summary>
        /// <value>The count.</value>
        public uint Count
        {
            get { return C.NSArray_count(SelfPtr()); }
        }

        /// <summary>
        /// Returns the object located at the specified index.
        /// </summary>
        /// <returns>The at index.</returns>
        /// <param name="index">Index.</param>
        /// <param name="constructor">Constructor.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T ObjectAtIndex(uint index, Func<IntPtr, T> constructor)
        {
            IntPtr ptr = C.NSArray_objectAtIndex(SelfPtr(), index);
            T obj = constructor(ptr);
            CFFunctions.CFRelease(ptr);   // release pointer returned by native method to balance ref count
            return obj;
        }

        #endregion

        #region C# Utils

        public virtual T[] ToArray(Func<IntPtr, T> constructor)
        {
            return PInvokeUtil.ToEnumerable<T>(
                (int)Count,
                index => ObjectAtIndex(
                    (uint)index,
                    constructor
                )).ToArray();
        }

        #endregion
    }

    internal static class InternalNSArray
    {
        [DllImport("__Internal")]
        internal static extern uint NSArray_count(HandleRef self);

        [DllImport("__Internal")]
        internal static extern IntPtr NSArray_objectAtIndex(HandleRef self, uint index);
    }
}
#endif