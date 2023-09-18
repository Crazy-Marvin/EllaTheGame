using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EasyMobile.Internal
{
    /// <summary>
    /// An abstract class representing objects that act as proxies for unmanaged objects, which are referred to using IntPtrs.
    /// </summary>
    internal abstract class InteropObject : IInteropObject
    {
        private readonly object locker = new object();

        protected readonly HandleRef mSelfPointer;

        protected bool mIsDisposed;

        protected abstract void AttachHandle(HandleRef selfPointer);

        protected abstract void ReleaseHandle(HandleRef selfPointer);

        public HandleRef SelfPointer
        {
            get
            {
                if (IsDisposed())
                {
                    throw new InvalidOperationException(
                        "Attempted to use object after it was cleaned up");
                }

                return mSelfPointer;
            }
        }

        public bool IsDisposed()
        {
            return mIsDisposed;
        }

        public bool HasSamePointerWith(IInteropObject other)
        {
            return other != null && this.ToPointer().Equals(other.ToPointer());
        }

        protected HandleRef SelfPtr()
        {
            return SelfPointer;
        }

        protected void AttachHandle()
        {
            AttachHandle(SelfPtr());
        }

        protected void ReleaseHandle()
        {
            ReleaseHandle(SelfPtr());
        }

        internal InteropObject(IntPtr pointer)
        {
            mSelfPointer = PInvokeUtil.CheckNonNull(new HandleRef(this, pointer));
            AttachHandle();
        }

        ~InteropObject()
        {
            Cleanup();
        }

        public void Dispose()
        {
            Cleanup();
            GC.SuppressFinalize(this);
        }

        public IntPtr ToPointer()
        {
            return SelfPtr().Handle;
        }

        protected virtual void Cleanup()
        {
            lock (locker)
            {
                if (!mIsDisposed)
                {
                    ReleaseHandle();
                    mIsDisposed = true;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;

            return Equals(obj as InteropObject);
        }

        public virtual bool Equals(InteropObject other)
        {
            if (null == other)
                return false;

            if (PInvokeUtil.IsNull(this.mSelfPointer))
                return PInvokeUtil.IsNull(other.mSelfPointer);

            return mSelfPointer.Handle == other.mSelfPointer.Handle && this.mIsDisposed == other.mIsDisposed;
        }

        public override int GetHashCode()
        {
            return mSelfPointer.Handle.GetHashCode();
        }

        public static bool operator ==(InteropObject objA, InteropObject objB)
        {
            if (ReferenceEquals(objA, null))
                return ReferenceEquals(objB, null);

            return objA.Equals(objB);
        }

        public static bool operator !=(InteropObject objA, InteropObject objB)
        {
            return !(objA == objB);
        }
    }
}

