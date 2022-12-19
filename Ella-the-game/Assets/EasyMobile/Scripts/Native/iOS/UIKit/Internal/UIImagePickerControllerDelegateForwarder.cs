#if UNITY_IOS && EM_CAMERA_GALLERY
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.iOS.CoreFoundation;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.UIKit;

namespace EasyMobile.Internal.iOS.UIKit
{
    using UIImagePickerControllerDelegate = UIImagePickerController.UIImagePickerControllerDelegate;

    internal class UIImagePickerControllerDelegateForwarder : iOSDelegateForwarder<UIImagePickerControllerDelegate>
    {
        internal UIImagePickerControllerDelegateForwarder(IntPtr selfPtr)
            : base(selfPtr)
        {
        }

        internal static UIImagePickerControllerDelegateForwarder FromPointer(IntPtr pointer)
        {
            return InteropObjectFactory<UIImagePickerControllerDelegateForwarder>.FromPointer(
                pointer,
                ptr => new UIImagePickerControllerDelegateForwarder(ptr));
        }

        internal UIImagePickerControllerDelegateForwarder()
            : this(C.UIImagePickerControllerDelegateForwarder_new(
                    InternalUIDidFinishPickingMediaWithInfoCallback,
                    InternalUIDidCancelCallback))
        {
            // We're using a pointer returned by a native constructor: call CFRelease to balance native ref count
            CFFunctions.CFRelease(this.ToPointer());
        }

        [MonoPInvokeCallback(typeof(C.InternalUIDidFinishPickingMediaWithInfo))]
        private static void InternalUIDidFinishPickingMediaWithInfoCallback(IntPtr delegatePtr, IntPtr pickerPtr, IntPtr infoPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                // Picker.
                var picker = InteropObjectFactory<UIImagePickerController>.FromPointer(pickerPtr, ptr => new UIImagePickerController(ptr));

                // Info.
                var infoDict = InteropObjectFactory<NSDictionary<NSString, iOSObjectProxy>>.FromPointer(infoPtr, ptr => new NSDictionary<NSString, iOSObjectProxy>(ptr));

                // Invoke consumer delegates.
                forwarder.InvokeOnListener(l => l.ImagePickerControllerDidFinishPickingMediaWithInfo(picker, infoDict));
            }
        }

        [MonoPInvokeCallback(typeof(C.InternalUIDidCancel))]
        private static void InternalUIDidCancelCallback(IntPtr delegatePtr, IntPtr pickerPtr)
        {
            var forwarder = FromPointer(delegatePtr);

            if (forwarder != null && forwarder.Listener != null)
            {
                var picker = InteropObjectFactory<UIImagePickerController>.FromPointer(pickerPtr, ptr => new UIImagePickerController(ptr));
                forwarder.InvokeOnListener(l => l.ImagePickerControllerDidCancel(picker));
            }
        }

        #region C wrapper

        private static class C
        {
            internal delegate void InternalUIDidFinishPickingMediaWithInfo(
            /* UIImagePickerControllerDelegateForwarder */ IntPtr delegatePtr,
            /* UIImagePickerController */ IntPtr pickerPtr,
            /* NSDictionary<UIImagePickerControllerInfoKey,id> */ IntPtr infoPtr);

            internal delegate void InternalUIDidCancel(
            /* UIImagePickerControllerDelegateForwarder */ IntPtr delegatePtr,
            /* UIImagePickerController */ IntPtr pickerPtr);

            [DllImport("__Internal")]
            internal static extern /* UIImagePickerControllerDelegateForwarder */ IntPtr
            UIImagePickerControllerDelegateForwarder_new(InternalUIDidFinishPickingMediaWithInfo didFinishPickingMediaWithInfoCallback,
                                                         InternalUIDidCancel didCancelCallback);
        }

        #endregion
    }
}
#endif