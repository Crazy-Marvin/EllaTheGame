#if UNITY_IOS
using System;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;

namespace EasyMobile.Internal.GameServices.iOS
{
    #region iOSSavedGame Native API
    internal static class iOSSavedGameNative
    {
        //----------------------------------------------------------
        // Saved Games API
        //----------------------------------------------------------

        internal delegate void OpenSavedGameCallback(IntPtr response, IntPtr callbackPtr);

        internal delegate void SaveGameDataCallback(IntPtr response, IntPtr callbackPtr);

        internal delegate void LoadSavedGameDataCallback(IntPtr response, IntPtr callbackPtr);

        internal delegate void FetchSavedGamesCallback(IntPtr response, IntPtr callbackPtr);

        internal delegate void ResolveConflictingSavedGamesCallback(IntPtr response, IntPtr callbackPtr);

        internal delegate void DeleteSavedGameCallback(IntPtr response, IntPtr callbackPtr);

        [DllImport("__Internal")]
        internal static extern void EM_OpenSavedGame(
            string name,
            OpenSavedGameCallback callback,
            IntPtr secondaryCallback
        );

        [DllImport("__Internal")]
        internal static extern void EM_SaveGameData(
            IntPtr gkSavedGamePtr,
            byte[] data,
            int dataLength,
            SaveGameDataCallback callback,
            IntPtr secondaryCallback);

        [DllImport("__Internal")]
        internal static extern void EM_LoadSavedGameData(
            IntPtr gkSavedGamePtr,
            LoadSavedGameDataCallback callback,
            IntPtr secondaryCallback);

        [DllImport("__Internal")]
        internal static extern void EM_FetchSavedGames(
            FetchSavedGamesCallback callback,
            IntPtr secondaryCallback);

        [DllImport("__Internal")]
        internal static extern void EM_ResolveConflictingSavedGames(
            IntPtr[] conflictingSavedGamePtrs,
            int savedGamesCount,
            byte[] data,
            int dataLength,
            ResolveConflictingSavedGamesCallback callback,
            IntPtr secondaryCallback);

        [DllImport("__Internal")]
        internal static extern void EM_DeleteSavedGame(
            string name,
            DeleteSavedGameCallback callback,
            IntPtr secondaryCallback);
    }

    #endregion // iOSSavedGame Native API

    #region iOSGKSavedGame

    internal class iOSGKSavedGame : InteropObject
    {
        [DllImport("__Internal")]
        internal static extern void EM_GKSavedGame_Ref(HandleRef self);

        [DllImport("__Internal")]
        internal static extern void EM_GKSavedGame_Unref(HandleRef self);

        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EM_GKSavedGame_IsOpen(HandleRef self);

        [DllImport("__Internal")]
        internal static extern int EM_GKSavedGame_Name(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        internal static extern int EM_GKSavedGame_DeviceName(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        internal static extern long EM_GKSavedGame_ModificationDate(HandleRef self);

        internal iOSGKSavedGame(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        protected override void AttachHandle(HandleRef selfPointer)
        {
            EM_GKSavedGame_Ref(selfPointer);
        }

        protected override void ReleaseHandle(HandleRef selfPointer)
        {
            EM_GKSavedGame_Unref(selfPointer);
        }

        internal static iOSGKSavedGame FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new iOSGKSavedGame(pointer);
        }

        internal bool IsOpen
        {
            get
            {
                return EM_GKSavedGame_IsOpen(SelfPtr());
            }
        }

        internal string Name
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                    EM_GKSavedGame_Name(SelfPtr(), strBuffer, strLen));
            }
        }

        internal string DeviceName
        {
            get
            {
                return PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                    EM_GKSavedGame_DeviceName(SelfPtr(), strBuffer, strLen));
            }
        }

        internal DateTime ModificationDate
        {
            get
            {
                return Util.FromMillisSinceUnixEpoch(
                    EM_GKSavedGame_ModificationDate(SelfPtr()));
            }
        }
    }

    #endregion // iOSGKSavedGame

    #region OpenSavedGameResponse

    internal class OpenSavedGameResponse : InteropObject
    {
        [DllImport("__Internal")]
        internal static extern void EM_OpenSavedGameResponse_Ref(HandleRef self);

        [DllImport("__Internal")]
        internal static extern void EM_OpenSavedGameResponse_Unref(HandleRef self);

        [DllImport("__Internal")]
        internal static extern int EM_OpenSavedGameResponse_ErrorDescription(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        internal static extern int EM_OpenSavedGameResponse_GetData_Length(HandleRef self);

        [DllImport("__Internal")]
        internal static extern IntPtr EM_OpenSavedGameResponse_GetData_GetElement(HandleRef self, int index);

        internal OpenSavedGameResponse(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        protected override void AttachHandle(HandleRef selfPointer)
        {
            EM_OpenSavedGameResponse_Ref(selfPointer);
        }

        protected override void ReleaseHandle(HandleRef selfPointer)
        {
            EM_OpenSavedGameResponse_Unref(selfPointer);
        }

        internal static OpenSavedGameResponse FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new OpenSavedGameResponse(pointer);
        }

        internal string GetError()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                EM_OpenSavedGameResponse_ErrorDescription(SelfPtr(), strBuffer, strLen));
        }

        internal iOSGKSavedGame[] GetResultSavedGames()
        {
            return PInvokeUtil.ToEnumerable<iOSGKSavedGame>(
                EM_OpenSavedGameResponse_GetData_Length(SelfPtr()),
                index =>
                iOSGKSavedGame.FromPointer(EM_OpenSavedGameResponse_GetData_GetElement(SelfPtr(), index))
            ).Cast<iOSGKSavedGame>().ToArray();
        }
    }

    #endregion  // OpenSavedGameResponse

    #region SaveGameDataResponse

    internal class SaveGameDataResponse : InteropObject
    {
        [DllImport("__Internal")]
        internal static extern void EM_SaveGameDataResponse_Ref(HandleRef self);

        [DllImport("__Internal")]
        internal static extern void EM_SaveGameDataResponse_Unref(HandleRef self);

        [DllImport("__Internal")]
        internal static extern int EM_SaveGameDataResponse_ErrorDescription(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        internal static extern IntPtr EM_SaveGameDataResponse_GetData(HandleRef self);

        internal SaveGameDataResponse(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        protected override void AttachHandle(HandleRef selfPointer)
        {
            EM_SaveGameDataResponse_Ref(selfPointer);
        }

        protected override void ReleaseHandle(HandleRef selfPointer)
        {
            EM_SaveGameDataResponse_Unref(selfPointer);
        }

        internal static SaveGameDataResponse FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new SaveGameDataResponse(pointer);
        }

        internal string GetError()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                EM_SaveGameDataResponse_ErrorDescription(SelfPtr(), strBuffer, strLen));
        }

        internal iOSGKSavedGame GetSavedGame()
        {
            return iOSGKSavedGame.FromPointer(EM_SaveGameDataResponse_GetData(SelfPtr()));
        }
    }

    #endregion  // SavedGameDataResponse

    #region LoadSavedGameDataResponse

    internal class LoadSavedGameDataResponse : InteropObject
    {
        [DllImport("__Internal")]
        internal static extern void EM_LoadSavedGameDataResponse_Ref(HandleRef self);

        [DllImport("__Internal")]
        internal static extern void EM_LoadSavedGameDataResponse_Unref(HandleRef self);

        [DllImport("__Internal")]
        internal static extern int EM_LoadSavedGameDataResponse_ErrorDescription(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        internal static extern int EM_LoadSavedGameDataResponse_GetData(HandleRef self, [In, Out] byte[] buffer, int byteCount);

        internal LoadSavedGameDataResponse(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        protected override void AttachHandle(HandleRef selfPointer)
        {
            EM_LoadSavedGameDataResponse_Ref(selfPointer);
        }

        protected override void ReleaseHandle(HandleRef selfPointer)
        {
            EM_LoadSavedGameDataResponse_Unref(selfPointer);
        }

        internal static LoadSavedGameDataResponse FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new LoadSavedGameDataResponse(pointer);
        }

        internal string GetError()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                EM_LoadSavedGameDataResponse_ErrorDescription(SelfPtr(), strBuffer, strLen));
        }

        internal byte[] GetData()
        {
            return PInvokeUtil.GetNativeArray<byte>((buffer, length) =>
                EM_LoadSavedGameDataResponse_GetData(SelfPtr(), buffer, length));
        }
    }

    #endregion  // LoadSavedGameDataResponse

    #region FetchSavedGamesResponse

    internal class FetchSavedGamesResponse : InteropObject
    {
        [DllImport("__Internal")]
        private static extern void EM_FetchSavedGamesResponse_Ref(HandleRef self);

        [DllImport("__Internal")]
        private static extern void EM_FetchSavedGamesResponse_Unref(HandleRef self);

        [DllImport("__Internal")]
        private static extern int EM_FetchSavedGamesResponse_ErrorDescription(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        private static extern int EM_FetchSavedGamesResponse_GetData_Length(HandleRef self);

        [DllImport("__Internal")]
        private static extern IntPtr EM_FetchSavedGamesResponse_GetData_GetElement(HandleRef self, int index);

        internal FetchSavedGamesResponse(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        protected override void AttachHandle(HandleRef selfPointer)
        {
            EM_FetchSavedGamesResponse_Ref(selfPointer);
        }

        protected override void ReleaseHandle(HandleRef selfPointer)
        {
            EM_FetchSavedGamesResponse_Unref(selfPointer);
        }

        internal static FetchSavedGamesResponse FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new FetchSavedGamesResponse(pointer);
        }

        internal string GetError()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                EM_FetchSavedGamesResponse_ErrorDescription(SelfPtr(), strBuffer, strLen));
        }

        internal iOSGKSavedGame[] GetFetchedSavedGames()
        {
            return PInvokeUtil.ToEnumerable<iOSGKSavedGame>(
                EM_FetchSavedGamesResponse_GetData_Length(SelfPtr()),
                index =>
                iOSGKSavedGame.FromPointer(EM_FetchSavedGamesResponse_GetData_GetElement(SelfPtr(), index))
            ).Cast<iOSGKSavedGame>().ToArray();
        }
    }

    #endregion

    #region ResolveConflictResponse

    internal class ResolveConflictResponse : InteropObject
    {
        [DllImport("__Internal")]
        private static extern void EM_ResolveConflictResponse_Ref(HandleRef self);

        [DllImport("__Internal")]
        private static extern void EM_ResolveConflictResponse_Unref(HandleRef self);

        [DllImport("__Internal")]
        private static extern int EM_ResolveConflictResponse_ErrorDescription(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        [DllImport("__Internal")]
        private static extern int EM_ResolveConflictResponse_GetData_Length(HandleRef self);

        [DllImport("__Internal")]
        private static extern IntPtr EM_ResolveConflictResponse_GetData_GetElement(HandleRef self, int index);

        internal ResolveConflictResponse(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        protected override void AttachHandle(HandleRef selfPointer)
        {
            EM_ResolveConflictResponse_Ref(selfPointer);
        }

        protected override void ReleaseHandle(HandleRef selfPointer)
        {
            EM_ResolveConflictResponse_Unref(selfPointer);
        }

        internal static ResolveConflictResponse FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new ResolveConflictResponse(pointer);
        }

        internal string GetError()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                EM_ResolveConflictResponse_ErrorDescription(SelfPtr(), strBuffer, strLen));
        }

        internal iOSGKSavedGame[] GetSavedGames()
        {
            return PInvokeUtil.ToEnumerable<iOSGKSavedGame>(
                EM_ResolveConflictResponse_GetData_Length(SelfPtr()),
                index =>
                iOSGKSavedGame.FromPointer(EM_ResolveConflictResponse_GetData_GetElement(SelfPtr(), index))
            ).Cast<iOSGKSavedGame>().ToArray();
        }
    }

    #endregion  // ResolveConflictResponse

    #region DeleteSavedGameResponse

    internal class DeleteSavedGameResponse : InteropObject
    {
        [DllImport("__Internal")]
        private static extern void EM_DeleteSavedGameResponse_Ref(HandleRef self);

        [DllImport("__Internal")]
        private static extern void EM_DeleteSavedGameResponse_Unref(HandleRef self);

        [DllImport("__Internal")]
        private static extern int EM_DeleteSavedGameResponse_ErrorDescription(HandleRef self, [In, Out] byte[] strBuffer, int strLen);

        internal DeleteSavedGameResponse(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        protected override void AttachHandle(HandleRef selfPointer)
        {
            EM_DeleteSavedGameResponse_Ref(selfPointer);
        }

        protected override void ReleaseHandle(HandleRef selfPointer)
        {
            EM_DeleteSavedGameResponse_Unref(selfPointer);
        }

        internal static DeleteSavedGameResponse FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new DeleteSavedGameResponse(pointer);
        }

        internal string GetError()
        {
            return PInvokeUtil.GetNativeString((strBuffer, strLen) =>
                EM_DeleteSavedGameResponse_ErrorDescription(SelfPtr(), strBuffer, strLen));
        }
    }

    #endregion  // DeleteSavedGameResponse
}
#endif
