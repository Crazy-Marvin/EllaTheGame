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
    using C = InternalNSMutableArray;

    /// <summary>
    /// A dynamic ordered collection of objects.
    /// </summary>
    internal class NSMutableArray<T> : NSArray<T> where T : iOSObjectProxy
    {
        internal NSMutableArray(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        internal NSMutableArray()
            : base(C.NSMutableArray_new())
        {
            CFFunctions.CFRelease(this.ToPointer());
        }

        #region NSMutableArray API

        /// <summary>
        /// Creates and returns an NSMutableArray object with enough allocated memory 
        /// to initially hold a given number of objects.
        /// </summary>
        /// <returns>The with capacity.</returns>
        /// <param name="numItems">Number items.</param>
        public static NSMutableArray<T> ArrayWithCapacity(uint numItems)
        {
            var ptr = C.NSMutableArray_arrayWithCapacity(numItems);
            var mutArray = new NSMutableArray<T>(ptr);
            CFFunctions.CFRelease(ptr);
            return mutArray;
        }

        /// <summary>
        /// Inserts a given object at the end of the array.
        /// </summary>
        /// <param name="anObject">An object.</param>
        public void AddObject(T anObject)
        {
            C.NSMutableArray_addObject(SelfPtr(), anObject == null ? IntPtr.Zero : anObject.ToPointer());
        }

        /// <summary>
        /// Inserts a given object into the array’s contents at a given index.
        /// </summary>
        /// <param name="anObject">An object.</param>
        /// <param name="index">Index.</param>
        public void InsertObjectAtIndex(T anObject, uint index)
        {
            C.NSMutableArray_insertObjectAtIndex(SelfPtr(), anObject == null ? IntPtr.Zero : anObject.ToPointer(), index);
        }

        /// <summary>
        /// Empties the array of all its elements.
        /// </summary>
        public void RemoveAllObjects()
        {
            C.NSMutableArray_removeAllObjects(SelfPtr());
        }

        /// <summary>
        /// Removes the object with the highest-valued index in the array.
        /// </summary>
        public void RemoveLastObject()
        {
            C.NSMutableArray_removeLastObject(SelfPtr());
        }

        /// <summary>
        /// Removes all occurrences in the array of a given object.
        /// </summary>
        /// <param name="anObject">An object.</param>
        public void RemoveObject(T anObject)
        {
            C.NSMutableArray_removeObject(SelfPtr(), anObject == null ? IntPtr.Zero : anObject.ToPointer());
        }

        /// <summary>
        /// Removes the object at index.
        /// </summary>
        /// <param name="index">Index.</param>
        public void RemoveObjectAdIndex(uint index)
        {
            C.NSMutableArray_removeObjectAtIndex(SelfPtr(), index);
        }

        /// <summary>
        /// Replaces the object at index with anObject.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="anObject">An object.</param>
        public void ReplaceObjectAtIndex(uint index, T anObject)
        {
            C.NSMutableArray_replaceObjectAtIndexWithObject(SelfPtr(), index, anObject == null ? IntPtr.Zero : anObject.ToPointer());
        }

        /// <summary>
        /// Exchanges the objects in the array at given indexes.
        /// </summary>
        /// <param name="idx1">Idx1.</param>
        /// <param name="idx2">Idx2.</param>
        public void ExchangeObjects(uint idx1, uint idx2)
        {
            C.NSMutableArray_exchangeObjectAtIndexWithObjectAtIndex(SelfPtr(), idx1, idx2);
        }

        #endregion


    }

    internal static class InternalNSMutableArray
    {
        // Constructor
        [DllImport("__Internal")]
        internal static extern /* NSMutableArray */IntPtr NSMutableArray_new();

        // Creating and Initializing a Mutable Array
        [DllImport("__Internal")]
        internal static extern /* NSMutableArray */IntPtr NSMutableArray_arrayWithCapacity(uint numItems);

        // Adding Objects
        [DllImport("__Internal")]
        internal static extern void NSMutableArray_addObject(HandleRef selfPointer, /* ObjectType */IntPtr anObject);

        [DllImport("__Internal")]
        internal static extern void NSMutableArray_insertObjectAtIndex(HandleRef selfPointer, /* ObjectType */IntPtr anObject, uint index);

        // Removing Objects
        [DllImport("__Internal")]
        internal static extern void NSMutableArray_removeAllObjects(HandleRef selfPointer);

        [DllImport("__Internal")]
        internal static extern void NSMutableArray_removeLastObject(HandleRef selfPointer);

        [DllImport("__Internal")]
        internal static extern void NSMutableArray_removeObject(HandleRef selfPointer, /* ObjectType */IntPtr anObject);

        [DllImport("__Internal")]
        internal static extern void NSMutableArray_removeObjectAtIndex(HandleRef selfPointer, uint index);

        // Replacing Objects
        [DllImport("__Internal")]
        internal static extern void NSMutableArray_replaceObjectAtIndexWithObject(HandleRef selfPointer, uint index, /* ObjectType */IntPtr anObject);

        // Rearranging Content
        [DllImport("__Internal")]
        internal static extern void NSMutableArray_exchangeObjectAtIndexWithObjectAtIndex(HandleRef selfPointer, uint idx1, uint idx2);
    }
}
#endif