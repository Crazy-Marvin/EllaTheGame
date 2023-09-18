using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EasyMobile.Internal
{
    /// <summary>
    /// Generic factory for creating interop objects.
    /// </summary>
    internal static class InteropObjectFactory<T> where T : InteropObject
    {
        private static StrongToWeakDictionary<IntPtr, T> mBinders = new StrongToWeakDictionary<IntPtr, T>();

        /// <summary>
        /// Creates a managed binding object for the unmanaged object referenced by the given pointer.
        /// If such binding object exists, this method returns it instead of creating a new one.
        /// Therefore using this method guarantees that the same managed binder is always returned
        /// for the same native pointer.
        /// </summary>
        /// <returns>The binder for the given pointer, or null if the pointer is invalid.</returns>
        /// <param name="pointer">Pointer.</param>
        /// <param name="constructor">Constructor.</param>
        public static T FromPointer(IntPtr pointer, Func<IntPtr, T> constructor)
        {
            if (PInvokeUtil.IsNull(pointer))
            {
                return default(T);
            }

            // Check if an binder exists for this IntPtr and return it.
            T binder = FindExistingBinder(pointer);
            if (binder != default(T))
                return binder;

            // Otherwise create a new binder, add it to the binders map and return it.
            T newBinder = constructor(pointer);
            RegisterNewBinder(pointer, newBinder);
            return newBinder;
        }

        /// <summary>
        /// Creates a managed binder object. This should be preferred over calling
        /// the object constructor directly, because it will register the binder
        /// so that it can be reused later via <see cref="InteropObjectFactory.FromPointer"/>.
        /// </summary>
        /// <returns>The created binder.</returns>
        /// <param name="constructor">Constructor.</param>
        /// <param name="ToPointer">To pointer.</param>
        public static T Create(Func<T> constructor, Func<T, IntPtr> ToPointer)
        {
            T newBinder = constructor();
            IntPtr pointer = ToPointer(newBinder);

            // Check if there's any existing binder for this pointer.
            // If yes, warn. Otherwise register the new binder.
            T existingBinder = FindExistingBinder(pointer);

            if (existingBinder == default(T))
                RegisterNewBinder(pointer, newBinder);
            else
                Debug.LogWarning("A binder for pointer " + pointer.ToString() + " already exists. Consider using FromPointer() instead.");
        
            return newBinder;
        }

        /// <summary>
        /// Returns the existing binder for the given pointer if one exists, otherwise returns default(T).
        /// Note that this will return default(T) even if a binder exists but is disposed.
        /// </summary>
        /// <returns>The existing binder.</returns>
        /// <param name="pointer">Pointer.</param>
        private static T FindExistingBinder(IntPtr pointer)
        {
            T binder;
            if (mBinders.TryGetValue(pointer, out binder) && !binder.IsDisposed())
                return binder;
            else
                return default(T);
        }

        /// <summary>
        /// Registers the new binder into the binders dictionary using the given pointer as the key.
        /// If such key exists, the associated binder will be replaced by the new one.
        /// </summary>
        /// <param name="pointer">Pointer.</param>
        /// <param name="newBinder">New binder.</param>
        private static void RegisterNewBinder(IntPtr pointer, T newBinder)
        {
            mBinders.Put(pointer, newBinder); 
        }
    }
}

