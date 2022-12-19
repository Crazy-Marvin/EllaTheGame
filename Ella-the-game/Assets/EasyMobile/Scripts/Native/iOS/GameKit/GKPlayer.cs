#if UNITY_IOS
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.UIKit;

namespace EasyMobile.iOS.GameKit
{
    /// <summary>
    /// An object that provides information about a player on Game Center.
    /// </summary>
    internal class GKPlayer : iOSObjectProxy
    {
        /// <summary>
        /// The size of a photo loaded by Game Center.
        /// </summary>
        internal enum GKPhotoSize
        {
            Small = 0,
            Normal
        }

        private string mPlayerId = null;
        private string mAlias = null;
        private string mDisplayName = null;

        internal GKPlayer(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Loads information from Game Center about a list of players.
        /// </summary>
        /// <param name="identifiers">Identifiers.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public static void LoadPlayersForIdentifiers(string[] identifiers, Action<GKPlayer[], NSError> completionHandler)
        {
            C.GKPlayer_loadPlayersForIdentifiers(
                identifiers, identifiers.Length,
                InternalLoadPlayersForIdentifiersCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// A unique identifier associated with a player.
        /// </summary>
        /// <value>The player I.</value>
        public string PlayerID
        {
            get
            {
                if (string.IsNullOrEmpty(mPlayerId))
                    mPlayerId = PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                        C.GKPlayer_playerId(SelfPtr(), strBuffer, strLen));
                return mPlayerId;
            }
        }

        /// <summary>
        /// A string chosen by the player to identify themselves to other players.
        /// </summary>
        /// <value>The alias.</value>
        public string Alias
        {
            get
            {
                if (string.IsNullOrEmpty(mAlias))
                    mAlias = PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                        C.GKPlayer_alias(SelfPtr(), strBuffer, strLen));
                return mAlias;
            }
        }

        /// <summary>
        /// A string to display for the player.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(mDisplayName))
                    mDisplayName = PInvokeUtil.GetNativeString((strBuffer, strLen) => 
                        C.GKPlayer_displayName(SelfPtr(), strBuffer, strLen));
                return mDisplayName;
            }
        }

        /// <summary>
        /// A Boolean value that indicates whether this player is a friend of the local player.
        /// </summary>
        /// <value><c>true</c> if this instance is friend; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// This was deprecated after iOS 8.0.
        /// </remarks>
        public bool IsFriend
        {
            get
            {
                return true;    
            }
        }

        /// <summary>
        /// Loads a photo of this player from Game Center.
        /// </summary>
        /// <param name="photoSize">Photo size.</param>
        /// <param name="callback">Callback.</param>
        public void LoadPhotoForSize(GKPhotoSize photoSize, Action<UIImage, NSError> completionHandler)
        {
            C.GKPlayer_loadPhotoForSize(
                SelfPtr(), 
                photoSize,
                InternalLoadPhotoForSizeCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        public bool Equals(GKPlayer obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return IsEqual(obj);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (!(obj is GKPlayer))
            {
                return false;
            }

            return IsEqual((GKPlayer)obj);
        }

        public override int GetHashCode()
        {
            return mPlayerId != null ? mPlayerId.GetHashCode() : 0;
        }

        private bool IsEqual(GKPlayer other)
        {
            return PlayerID.Equals(other.PlayerID);
        }

        /// <summary>
        /// Converts native pointers into corresponding managed types and invokes the consumer callback.
        /// </summary>
        /// <param name="players">Players.</param>
        /// <param name="error">Error.</param>
        /// <param name="secondaryCallback">Secondary callback.</param>
        [MonoPInvokeCallback(typeof(C.LoadPlayersCallback))]
        private static void InternalLoadPlayersForIdentifiersCallback(IntPtr players, IntPtr error, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            GKPlayer[] gkPlayers = null;

            if (PInvokeUtil.IsNotNull(players))
            {
                // Creating a one-time usage NSArray binder, so need to use the factory.
                using (var nsArray = new NSArray<GKPlayer>(players))
                {
                    gkPlayers = nsArray.ToArray(ptr => InteropObjectFactory<GKPlayer>.FromPointer(ptr, p => new GKPlayer(p)));
                }
            }

            // A new NSError object is always created on native side, so no need
            // to check the binder pool for reusing an existing one.
            NSError nsError = PInvokeUtil.IsNotNull(error) ? new NSError(error) : null;

            // Invoke consumer callback.
            PInvokeCallbackUtil.PerformInternalCallback(
                "GKPlayer#InternalLoadPlayersForIdentifiersCallback",
                PInvokeCallbackUtil.Type.Temporary,
                gkPlayers, nsError, secondaryCallback);
        }

        [MonoPInvokeCallback(typeof(C.LoadPhotoCallback))]
        private static void InternalLoadPhotoForSizeCallback(IntPtr photo, IntPtr error, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            // Create the UIImage from the pointer.
            UIImage uiImage = InteropObjectFactory<UIImage>.FromPointer(photo, ptr => new UIImage(ptr));
            
            // A new NSError object is always created on native side, so no need
            // to check the binder pool for reusing an existing one.
            NSError nsError = PInvokeUtil.IsNotNull(error) ? new NSError(error) : null;

            PInvokeCallbackUtil.PerformInternalCallback(
                "GKPlayer#InternalLoadPhotoForSizeCallback",
                PInvokeCallbackUtil.Type.Temporary,
                uiImage, nsError, secondaryCallback);
        }

        #region C wrapper

        private static class C
        {
            internal delegate void LoadPlayersCallback(
            /* InteropNSArray<InteropGKPlayer> */IntPtr players,
            /* InteropNSError */IntPtr error,
                IntPtr secondaryCallbackPtr);

            internal delegate void LoadPhotoCallback(
            /* InteropUIImage */IntPtr photo,
            /* InteropNSError */IntPtr error,
                IntPtr secondaryCallbackPtr);

            [DllImport("__Internal")]
            internal static extern void GKPlayer_loadPlayersForIdentifiers(
                string[] identifiers, int identifiersCount, 
                LoadPlayersCallback callback, IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern int GKPlayer_playerId(
                HandleRef self, [In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int GKPlayer_alias(
                HandleRef self, [In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern int GKPlayer_displayName(
                HandleRef self, [In,Out] /* from(char *) */ byte[] strBuffer, int strLen);

            [DllImport("__Internal")]
            internal static extern void GKPlayer_loadPhotoForSize(
                HandleRef self, 
                GKPhotoSize size,
                LoadPhotoCallback callback, IntPtr secondaryCallback);
        }

        #endregion
    }
}
#endif
