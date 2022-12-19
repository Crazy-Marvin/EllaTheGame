using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace EasyMobile.Internal
{
    internal static class PInvokeCallbackUtil
    {
        private static readonly bool VERBOSE_DEBUG = false;

        internal static IntPtr ToIntPtr<T>(Action<T> callback, Func<IntPtr, T> conversionFunction)
        where T : InteropObject
        {
            Action<IntPtr> pointerReceiver = result =>
            {
                using (T converted = conversionFunction(result))
                {
                    if (callback != null)
                    {
                        callback(converted);
                    }
                }
            };

            return ToIntPtr(pointerReceiver);
        }

        internal static IntPtr ToIntPtr<T, P>(Action<T, P> callback, Func<IntPtr, T> conversionFunction)
        where T : InteropObject
        {
            Action<IntPtr, P> pointerReceiver = (param1, param2) =>
            {
                using (T converted = conversionFunction(param1))
                {
                    if (callback != null)
                    {
                        callback(converted, param2);
                    }
                }
            };

            return ToIntPtr(pointerReceiver);
        }

        internal static IntPtr ToIntPtr<T, P>(Action<T, P> callback, 
                                              Func<IntPtr, T> conversionFunctionT, Func<IntPtr, P> conversionFunctionP) 
            where T : InteropObject
            where P : InteropObject
        {
            Action<IntPtr, IntPtr> pointerReceiver = (t, p) =>
            {
                using (T convertedT = conversionFunctionT(t))
                {
                    using (P convertedP = conversionFunctionP(p))
                    {
                        if (callback != null)
                        {
                            callback(convertedT, convertedP);
                        }
                    }
                }
            };

            return ToIntPtr(pointerReceiver);
        }

        internal static IntPtr ToIntPtr<T,P>(Action<T, P> callback)
        {
            return ToIntPtr((Delegate)callback);
        }

        internal static IntPtr ToIntPtr<T>(Action<T> callback)
        {
            return ToIntPtr((Delegate)callback);
        }

        internal static IntPtr ToIntPtr(Action callback)
        {
            return ToIntPtr((Delegate)callback);
        }

        internal static IntPtr ToIntPtr<T, P>(Func<T,P> function)
        {
            return ToIntPtr((Delegate)function);
        }

        internal static IntPtr ToIntPtr(Delegate callback)
        {
            if (callback == null)
            {
                return IntPtr.Zero;
            }

            // Use a GCHandle to retain the callback, it will be freed when the callback returns the and
            // handle is converted back to callback via IntPtrToCallback.
            var handle = GCHandle.Alloc(callback);
            return GCHandle.ToIntPtr(handle);
        }

        internal static T IntPtrToTempCallback<T>(IntPtr handle) where T : class
        {
            return IntPtrToCallback<T>(handle, true);
        }

        internal static T IntPtrToPermanentCallback<T>(IntPtr handle) where T : class
        {
            return IntPtrToCallback<T>(handle, false);
        }

        internal static void UnpinCallbackHandle(IntPtr handle)
        {
            if (PInvokeUtil.IsNotNull(handle))
            {
                var gcHandle = GCHandle.FromIntPtr(handle);
                gcHandle.Free();
            }
        }

        private static T IntPtrToCallback<T>(IntPtr handle, bool unpinHandle) where T : class
        {
            if (PInvokeUtil.IsNull(handle))
            {
                return null;
            }

            var gcHandle = GCHandle.FromIntPtr(handle);
            try
            {
                return (T)gcHandle.Target;
            }
            catch (System.InvalidCastException e)
            {
                Debug.LogError("GC Handle pointed to unexpected type: " + gcHandle.Target.ToString() +
                    ". Expected " + typeof(T));
                throw e;
            }
            finally
            {
                if (unpinHandle)
                {
                    gcHandle.Free();
                }
            }
        }

        internal enum Type
        {
            Permanent,
            Temporary}

        ;

        internal static void PerformInternalCallback(string callbackName, Type callbackType, IntPtr callbackPtr)
        {
            if (VERBOSE_DEBUG)
                Debug.Log("Entering internal callback for " + callbackName);

            Action callback = null;
            try
            {
                callback = callbackType == Type.Permanent ? 
                    IntPtrToPermanentCallback<Action>(callbackPtr)
                    : IntPtrToTempCallback<Action>(callbackPtr);
            }
            catch (Exception e)
            {
                Debug.LogError("Error encountered converting " + callbackName + ". " +
                    "Smothering to avoid passing exception into Native: " + e);
                return;
            }

            if (VERBOSE_DEBUG)
                Debug.Log("Internal Callback converted to action");

            InvokeConvertedCallback(callbackName, callback);
        }

        internal static void PerformInternalCallback<T>(string callbackName, Type callbackType,
                                                        T param, IntPtr callbackPtr)
        {
            if (VERBOSE_DEBUG)
                Debug.Log("Entering internal callback for " + callbackName);

            Action<T> callback = null;
            try
            {
                callback = callbackType == Type.Permanent ?
                    IntPtrToPermanentCallback<Action<T>>(callbackPtr)
                    : IntPtrToTempCallback<Action<T>>(callbackPtr);
            }
            catch (Exception e)
            {
                Debug.LogError("Error encountered converting " + callbackName + ". " +
                    "Smothering to avoid passing exception into Native: " + e);
                return;
            }

            if (VERBOSE_DEBUG)
                Debug.Log("Internal Callback converted to action");

            InvokeConvertedCallback(callbackName, callback, param);
        }

        internal static void PerformInternalCallback<T, P>(string callbackName, Type callbackType,
                                                           T param1, P param2, IntPtr callbackPtr)
        {
            if (VERBOSE_DEBUG)
                Debug.Log("Entering internal callback for " + callbackName);
            
            Action<T, P> callback = null;
            try
            {
                callback = callbackType == Type.Permanent ?
                    IntPtrToPermanentCallback<Action<T, P>>(callbackPtr)
                    : IntPtrToTempCallback<Action<T, P>>(callbackPtr);
            }
            catch (Exception e)
            {
                Debug.LogError("Error encountered converting " + callbackName + ". " +
                    "Smothering to avoid passing exception into Native: " + e);
                return;
            }

            if (VERBOSE_DEBUG)
                Debug.Log("Internal Callback converted to action");

            InvokeConvertedCallback(callbackName, callback, param1, param2);
        }

        internal static P PerformInternalFunction<T, P>(string funcName, Type callbackType,
                                                        T param, IntPtr funcPtr)
        {
            if (VERBOSE_DEBUG)
                Debug.Log("Entering internal callback for " + funcName);

            Func<T, P> function = null;
            try
            {
                function = callbackType == Type.Permanent ?
                    IntPtrToPermanentCallback<Func<T, P>>(funcPtr)
                    : IntPtrToTempCallback<Func<T, P>>(funcPtr);
            }
            catch (Exception e)
            {
                Debug.LogError("Error encountered converting " + funcName + ". " +
                    "Smothering to avoid passing exception into Native: " + e);
                return default(P);
            }

            if (VERBOSE_DEBUG)
                Debug.Log("Internal Function converted to action");

            return InvokeConvertedFunction(funcName, function, param);
        }

        private static void InvokeConvertedCallback(string callbackName, Action callback)
        {
            if (callback == null)
            {
                return;
            }

            try
            {
                callback();
            }
            catch (Exception e)
            {
                Debug.LogError("Error encountered executing " + callbackName + ". " +
                    "Smothering to avoid passing exception into Native: " + e);
            }
        }

        private static void InvokeConvertedCallback<T>(string callbackName, Action<T> callback, T param)
        {
            if (callback == null)
            {
                return;
            }

            try
            {
                callback(param);
            }
            catch (Exception e)
            {
                Debug.LogError("Error encountered executing " + callbackName + ". " +
                    "Smothering to avoid passing exception into Native: " + e);
            }
        }

        private static void InvokeConvertedCallback<T,P>(string callbackName, Action<T,P> callback, T param1, P param2)
        {
            if (callback == null)
            {
                return;
            }

            try
            {
                callback(param1, param2);
            }
            catch (Exception e)
            {
                Debug.LogError("Error encountered executing " + callbackName + ". " +
                    "Smothering to avoid passing exception into Native: " + e);
            }
        }

        private static P InvokeConvertedFunction<T,P>(string funcName, Func<T,P> function, T param)
        {
            if (function == null)
            {
                return default(P);
            }

            try
            {
                return function(param);
            }
            catch (Exception e)
            {
                Debug.LogError("Error encountered executing " + funcName + ". " +
                    "Smothering to avoid passing exception into Native: " + e);
                return default(P);
            }
        }
    }
}

