#if UNITY_IOS
using System;
using System.Linq;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.Internal.iOS.GameKit;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.CoreFoundation;

namespace EasyMobile.iOS.GameKit
{
    /// <summary>
    /// A peer-to-peer network between a group of devices that are connected to Game Center.
    /// </summary>
    internal class GKMatch : iOSObjectProxy
    {
        #region GKMatchDelegate

        /// <summary>
        /// The state of another player in the match.
        /// </summary>
        public enum GKPlayerConnectionState
        {
            Unknown = 0,
            Connected,
            Disconnected
        }

        /// <summary>
        /// The mechanism used to transmit data to other players.
        /// </summary>
        public enum GKMatchSendDataMode
        {
            /// <summary>
            /// The data is sent continuously until it is successfully received 
            /// by the intended recipients or the connection times out. Use this 
            /// when you need to guarantee delivery and speed is not critical.
            /// </summary>
            Reliable = 0,

            /// <summary>
            /// The data is sent once and is not sent again if a transmission error occurs. 
            /// Use this for small packets of data that must arrive quickly to be useful to the recipient.
            /// </summary>
            Unreliable
        }

        /// <summary>
        /// The delegate is called when status updates and network data is received from players.
        /// </summary>
        public interface GKMatchDelegate
        {
            /// <summary>
            /// Called when data is received by a player from another player.
            /// </summary>
            /// <param name="match">Match.</param>
            /// <param name="data">Data.</param>
            /// <param name="recipient">Recipient.</param>
            /// <param name="remotePlayer">Remote player.</param>
            void MatchDidReceiveDataForRecipient(GKMatch match, NSData data, GKPlayer recipient, GKPlayer remotePlayer);

            /// <summary>
            /// Called when data is received from a player.
            /// </summary>
            /// <param name="match">Match.</param>
            /// <param name="data">Data.</param>
            /// <param name="remotePlayer">Remote player.</param>
            void MatchDidReceiveData(GKMatch match, NSData data, GKPlayer remotePlayer);

            /// <summary>
            /// Called when a player connects to or disconnects from the match.
            /// </summary>
            /// <param name="match">Match.</param>
            /// <param name="player">Player.</param>
            /// <param name="state">State.</param>
            void PlayerDidChangeConnectionState(GKMatch match, GKPlayer player, GKPlayerConnectionState state);

            /// <summary>
            /// Called when the match cannot connect to any other players.
            /// </summary>
            /// <param name="match">Match.</param>
            /// <param name="error">Error.</param>
            void MatchDidFailWithError(GKMatch match, NSError error);

            /// <summary>
            /// Called when a player in a two-player match was disconnected.
            /// </summary>
            /// <returns>Your game should return <c>true</c> if it wants Game Kit 
            /// to attempt to reconnect the player, <c>false</c> if it wants to terminate the match.</returns>
            /// <param name="match">Match.</param>
            /// <param name="player">Player.</param>
            bool ShouldReinviteDisconnectedPlayer(GKMatch match, GKPlayer player);
        }

        #endregion

        private GKMatchDelegate mMatchDelegate;
        private GKMatchDelegateForwarder mMatchDelegateForwarder;

        internal GKMatch(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// The delegate for the match.
        /// </summary>
        /// <value>The delegate.</value>
        public GKMatchDelegate Delegate
        {
            get
            {
                return mMatchDelegate;
            }
            set
            {
                mMatchDelegate = value;

                if (mMatchDelegate == null)
                {
                    // Nil out the native delegate.
                    mMatchDelegateForwarder = null;
                    C.GKMatch_setDelegate(SelfPtr(), IntPtr.Zero);
                }
                else
                {
                    // Create a delegate forwarder if needed.
                    if (mMatchDelegateForwarder == null)
                    {
                        mMatchDelegateForwarder = InteropObjectFactory<GKMatchDelegateForwarder>.Create(
                            () => new GKMatchDelegateForwarder(),
                            fwd => fwd.ToPointer());

                        // Assign on native side.
                        C.GKMatch_setDelegate(SelfPtr(), mMatchDelegateForwarder.ToPointer());
                    }

                    // Set delegate.
                    mMatchDelegateForwarder.Listener = mMatchDelegate;
                }
            }
        }

        /// <summary>
        /// The remaining number of players who have not yet connected to the match.
        /// </summary>
        /// <value>The expected player count.</value>
        public uint ExpectedPlayerCount
        {
            get
            {
                return C.GKMatch_expectedPlayerCount(SelfPtr());
            }
        }

        /// <summary>
        /// An array of <see cref="GKPlayer"/> objects that represent the players in the match.
        /// </summary>
        /// <value>The players.</value>
        public NSArray<GKPlayer> Players
        {
            get
            {
                var ptr = C.GKMatch_players(SelfPtr());
                var nsArray = InteropObjectFactory<NSArray<GKPlayer>>.FromPointer(ptr, p => new NSArray<GKPlayer>(p));
                CFFunctions.CFRelease(ptr);  // release pointer returned by the native method.
                return nsArray;
            }
        }

        /// <summary>
        /// Determines the best player in the game to act as the server for a client-server match.
        /// </summary>
        /// <param name="completionHandler">Completion handler.</param>
        public void ChooseBestHostingPlayer(Action<GKPlayer> completionHandler)
        {
            C.GKMatch_chooseBestHostingPlayer(
                SelfPtr(),
                InternalChooseBestHostingPlayerCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Transmits data to a list of connected players.
        /// </summary>
        /// <returns><c>true</c>, if data was sent, <c>false</c> otherwise.</returns>
        /// <param name="data">Data.</param>
        /// <param name="players">Players.</param>
        /// <param name="mode">Mode.</param>
        /// <param name="error">Error.</param>
        public bool SendData(byte[] data, GKPlayer[] players, GKMatchSendDataMode mode, out NSError error)
        {
            Util.NullArgumentTest(data);
            Util.NullArgumentTest(players);

            IntPtr errorPtr = new IntPtr();
            bool success = C.GKMatch_sendData(
                               SelfPtr(),
                               data, 
                               data.Length,
                               players.Select(p => p != null ? p.ToPointer() : IntPtr.Zero).ToArray(), 
                               players.Length,
                               mode,
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);     // balance out ref count of a pointer returned directly from native side.
            }

            return success;
        }

        /// <summary>
        /// Transmits data to all players connected to the match.
        /// </summary>
        /// <returns><c>true</c>, if data to all players was sent, <c>false</c> otherwise.</returns>
        /// <param name="data">Data.</param>
        /// <param name="mode">Mode.</param>
        /// <param name="error">Error.</param>
        public bool SendDataToAllPlayers(byte[] data, GKMatchSendDataMode mode, out NSError error)
        {
            Util.NullArgumentTest(data);

            IntPtr errorPtr = new IntPtr();
            bool success = C.GKMatch_sendDataToAllPlayers(
                               SelfPtr(),
                               data,
                               data.Length,
                               mode,
                               ref errorPtr);

            error = null;
            if (PInvokeUtil.IsNotNull(errorPtr))
            {
                error = new NSError(errorPtr);
                CFFunctions.CFRelease(errorPtr);     // balance out ref count of a pointer returned directly from native side.
            }
            
            return success;
        }

        /// <summary>
        /// Disconnects the local player from the match.
        /// </summary>
        public void Disconnect()
        {
            C.GKMatch_disconnect(SelfPtr());
        }

        /// <summary>
        /// Create a new match with the list of players from an existing match.
        /// </summary>
        /// <param name="completionHandler">Completion handler.</param>
        public void Rematch(Action<GKMatch, NSError> completionHandler)
        {
            C.GKMatch_rematch(
                SelfPtr(),
                InternalRematchCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        #region Private Stuff

        [MonoPInvokeCallback(typeof(C.ChooseBestHostingPlayerCallback))]
        private static void InternalChooseBestHostingPlayerCallback(IntPtr player, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            var gkPlayer = InteropObjectFactory<GKPlayer>.FromPointer(player, p => new GKPlayer(p));

            PInvokeCallbackUtil.PerformInternalCallback(
                "GKMatch#InternalChooseBestHostingPlayerCallback",
                PInvokeCallbackUtil.Type.Temporary,
                gkPlayer, secondaryCallback);
        }

        [MonoPInvokeCallback(typeof(C.RematchCallback))]
        private static void InternalRematchCallback(IntPtr match, IntPtr error, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            var gkMatch = InteropObjectFactory<GKMatch>.FromPointer(match, p => new GKMatch(p));
            var nsError = PInvokeUtil.IsNotNull(error) ? new NSError(error) : null;

            PInvokeCallbackUtil.PerformInternalCallback(
                "GKMatch#InternalRematchCallback",
                PInvokeCallbackUtil.Type.Temporary,
                gkMatch, nsError, secondaryCallback);
        }

        #endregion

        #region C wrapper

        private static class C
        {
            internal delegate void ChooseBestHostingPlayerCallback(
            /* InteropGKPlayer */ IntPtr player,
                IntPtr secondaryCallbackPtr);

            internal delegate void RematchCallback(
            /* InteropGKMatch */ IntPtr match,
            /* InteropNSError */ IntPtr error,
                IntPtr secondaryCallbackPtr);

            [DllImport("__Internal")]
            internal static extern /* InteropGKMatchDelegate */ IntPtr GKMatch_delegate(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void GKMatch_setDelegate(HandleRef selfPtr, /* InteropGKMatchDelegate */IntPtr matchDelegate);

            [DllImport("__Internal")]
            internal static extern uint GKMatch_expectedPlayerCount(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern /* InteropNSArray<InteropGKPlayer *> */ IntPtr GKMatch_players(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void GKMatch_chooseBestHostingPlayer(
                HandleRef selfPtr,
                ChooseBestHostingPlayerCallback callback,
                IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern bool GKMatch_sendData(
                HandleRef selfPtr,
                byte[] matchData, int matchDataLength,
                /* InteropGKPlayer[] */IntPtr[] players, int playersCount,
                GKMatchSendDataMode mode,
                [In, Out]/* InteropNSError */ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern bool GKMatch_sendDataToAllPlayers(
                HandleRef selfPtr,
                byte[] matchData, int matchDataLength,
                GKMatchSendDataMode mode,
                [In, Out]/* InteropNSError */ref IntPtr error);

            [DllImport("__Internal")]
            internal static extern void GKMatch_disconnect(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void GKMatch_rematch(
                HandleRef selfPtr,
                RematchCallback callback,
                IntPtr secondaryCallback);
        }

        #endregion
    }
}
#endif
