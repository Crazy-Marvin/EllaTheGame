using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EasyMobile.Internal
{
    /// <summary>
    /// Interface for unmanaged object proxies, which are referred to using IntPtrs.
    /// </summary>
    internal interface IInteropObject : IDisposable
    {
        HandleRef SelfPointer { get; }

        bool IsDisposed();

        bool HasSamePointerWith(IInteropObject other);

        IntPtr ToPointer();
    }
}

