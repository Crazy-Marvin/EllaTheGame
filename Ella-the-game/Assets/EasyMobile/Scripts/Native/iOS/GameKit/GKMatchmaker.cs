#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.UIKit;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.GameKit
{
    /// <summary>
    /// An object that programmatically creates matches with other players 
    /// and receives match invitations sent by other players.
    /// </summary>
    internal class GKMatchmaker : iOSObjectProxy
    {
        private static GKMatchmaker sSharedMatchmaker;

        internal GKMatchmaker(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Returns the singleton matchmaker instance.
        /// </summary>
        /// <returns>The matchmaker.</returns>
        public static GKMatchmaker SharedMatchmaker()
        {
            if (sSharedMatchmaker == null)
            {
                var ptr = C.GKMatchmaker_sharedMatchmaker();
                sSharedMatchmaker = InteropObjectFactory<GKMatchmaker>.FromPointer(ptr, p => new GKMatchmaker(p));
                CoreFoundation.CFFunctions.CFRelease(ptr);
            }
            return sSharedMatchmaker;
        }

        /// <summary>
        /// Creates a match from an accepted invitation.
        /// </summary>
        /// <param name="invite">Invite.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public void MatchForInvite(GKInvite invite, Action<GKMatch, NSError> completionHandler)
        {
            Util.NullArgumentTest(invite);

            C.GKMatchmaker_matchForInvite(
                SelfPtr(),
                invite.ToPointer(),
                MatchErrorCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Adds players to an existing match.
        /// </summary>
        /// <param name="match">Match.</param>
        /// <param name="matchRequest">Match request.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public void AddPlayersToMatch(GKMatch match, GKMatchRequest matchRequest, Action<NSError> completionHandler)
        {
            Util.NullArgumentTest(match);
            Util.NullArgumentTest(matchRequest);

            C.GKMatchmaker_addPlayersToMatch(
                SelfPtr(),
                match.ToPointer(),
                matchRequest.ToPointer(),
                ErrorCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Cancels a pending matchmaking request.
        /// </summary>
        /// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
        public void Cancel()
        {
            C.GKMatchmaker_cancel(SelfPtr());
        }

        /// <summary>
        /// Cancels a pending invitation to another player.
        /// </summary>
        /// <returns><c>true</c> if this instance cancel pending invite the specified player; otherwise, <c>false</c>.</returns>
        /// <param name="player">Player.</param>
        public void CancelPendingInvite(GKPlayer player)
        {
            Util.NullArgumentTest(player);

            C.GKMatchmaker_cancelPendingInviteToPlayer(SelfPtr(), player.ToPointer());
        }

        /// <summary>
        /// Initiates a request to find players for a peer-to-peer match.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public void FindMatchForRequest(GKMatchRequest request, Action<GKMatch, NSError> completionHandler)
        {
            Util.NullArgumentTest(request);

            C.GKMatchmaker_findMatchForRequest(
                SelfPtr(),
                request.ToPointer(),
                MatchErrorCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Initiates a request to find players for a hosted match.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public void FindPlayersForHostedRequest(GKMatchRequest request, Action<NSArray<GKPlayer>, NSError> completionHandler)
        {
            Util.NullArgumentTest(request);

            C.GKMatchmaker_findPlayersForHostedRequest(
                SelfPtr(),
                request.ToPointer(),
                PlayersErrorCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Informs Game Center that programmatic matchmaking has finished.
        /// </summary>
        /// <param name="match">Match.</param>
        public void FinishMatchmakingForMatch(GKMatch match)
        {
            Util.NullArgumentTest(match);

            C.GKMatchmaker_finishMatchmakingForMatch(SelfPtr(), match.ToPointer());
        }

        /// <summary>
        /// Initiates a search for activity in all player groups.
        /// </summary>
        /// <param name="completionHandler">Completion handler.</param>
        public void QueryActivity(Action<int, NSError> completionHandler)
        {
            C.GKMatchmaker_queryActivity(
                SelfPtr(),
                IntErrorCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Queries Game Center for the activity in a player group.
        /// </summary>
        /// <param name="playerGroup">Player group.</param>
        /// <param name="completionHandler">Completion handler.</param>
        public void QueryPlayerGroupActivity(uint playerGroup, Action<int, NSError> completionHandler)
        {
            C.GKMatchmaker_queryPlayerGroupActivity(
                SelfPtr(),
                playerGroup,
                IntErrorCallback,
                PInvokeCallbackUtil.ToIntPtr(completionHandler));
        }

        /// <summary>
        /// Enables the matchmaking process to find nearby players through Bluetooth or WiFi (same subnet only).
        /// </summary>
        /// <param name="reachableHandler">Reachable handler.</param>
        public void StartBrowsingForNearbyPlayers(Action<GKPlayer, bool> reachableHandler)
        {
            C.GKMatchmaker_startBrowsingForNearbyPlayers(
                SelfPtr(),
                PlayerBoolCallback,
                PInvokeCallbackUtil.ToIntPtr(reachableHandler));
        }

        /// <summary>
        /// Ends the search for nearby players.
        /// </summary>
        public void StopBrowsingForNearbyPlayers()
        {
            C.GKMatchmaker_stopBrowsingForNearbyPlayers(SelfPtr());
        }

        #region Internal Callbacks

        [MonoPInvokeCallback(typeof(C.MatchErrorCallback))]
        private static void MatchErrorCallback(
            /* InteropGKMatch */ IntPtr matchPtr,/* InteropNSError */IntPtr errorPtr, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            var match = InteropObjectFactory<GKMatch>.FromPointer(matchPtr, p => new GKMatch(p));
            var error = PInvokeUtil.IsNotNull(errorPtr) ? new NSError(errorPtr) : null;

            PInvokeCallbackUtil.PerformInternalCallback(
                "GKMatchmaker#MatchErrorCallback",
                PInvokeCallbackUtil.Type.Temporary,
                match, error, secondaryCallback);
        }

        [MonoPInvokeCallback(typeof(C.ErrorCallback))]
        private static void ErrorCallback(
            /* InteropNSError */ IntPtr errorPtr, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            var error = PInvokeUtil.IsNotNull(errorPtr) ? new NSError(errorPtr) : null;

            PInvokeCallbackUtil.PerformInternalCallback(
                "GKMatchmaker#ErrorCallback",
                PInvokeCallbackUtil.Type.Temporary,
                error, secondaryCallback);
        }

        [MonoPInvokeCallback(typeof(C.PlayersErrorCallback))]
        private static void PlayersErrorCallback(
            /* InteropNSArray<GKPlayer> */ IntPtr playerArrayPtr, /* InteropNSError */IntPtr errorPtr, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            var array = InteropObjectFactory<NSArray<GKPlayer>>.FromPointer(playerArrayPtr, p => new NSArray<GKPlayer>(p));
            var error = PInvokeUtil.IsNotNull(errorPtr) ? new NSError(errorPtr) : null;

            PInvokeCallbackUtil.PerformInternalCallback(
                "GKMatchmaker#PlayersErrorCallback",
                PInvokeCallbackUtil.Type.Temporary,
                array, error, secondaryCallback);
        }

        [MonoPInvokeCallback(typeof(C.IntErrorCallback))]
        private static void IntErrorCallback(
            int intArg, /* InteropNSError */IntPtr errorPtr, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            var error = PInvokeUtil.IsNotNull(errorPtr) ? new NSError(errorPtr) : null;

            PInvokeCallbackUtil.PerformInternalCallback(
                "GKMatchmaker#IntErrorCallback",
                PInvokeCallbackUtil.Type.Temporary,
                intArg, error, secondaryCallback);
        }

        [MonoPInvokeCallback(typeof(C.PlayerBoolCallback))]
        private static void PlayerBoolCallback(
            /* InteropGKPlayer */ IntPtr playerPtr, bool boolArg, IntPtr secondaryCallback)
        {
            if (PInvokeUtil.IsNull(secondaryCallback))
                return;

            var player = InteropObjectFactory<GKPlayer>.FromPointer(playerPtr, p => new GKPlayer(p));

            PInvokeCallbackUtil.PerformInternalCallback(
                "GKMatchmaker#PlayerBoolCallback",
                PInvokeCallbackUtil.Type.Temporary,
                player, boolArg, secondaryCallback);
        }

        #endregion

        #region C wrapper

        private static class C
        {
            internal delegate void MatchErrorCallback(
            /* InteropGKMatch */ IntPtr matchPtr,/* InteropNSError */ IntPtr errorPtr,IntPtr secondaryCallback);

            internal delegate void ErrorCallback(
            /* InteropNSError */ IntPtr errorPtr,IntPtr secondaryCallback);

            internal delegate void PlayersErrorCallback(
            /* InteropNSArray<GKPlayer> */ IntPtr playerArrayPtr,/* InteropNSError */ IntPtr errorPtr,IntPtr secondaryCallback);

            internal delegate void IntErrorCallback(
                int intArg,/* InteropNSError */ IntPtr errorPtr,IntPtr secondaryCallback);

            internal delegate void PlayerBoolCallback(
            /* InteropGKPlayer */ IntPtr playerPtr,bool boolArg,IntPtr secondaryCallback);

            // Retrieving the Shared Matchmaker

            [DllImport("__Internal")]
            internal static extern /* InteropGKMatchmaker */IntPtr GKMatchmaker_sharedMatchmaker();

            // Receiving Invitations from Other Players
            [DllImport("__Internal")]
            internal static extern void GKMatchmaker_matchForInvite(HandleRef selfPtr,
                /* InteropGKInvite */IntPtr invitePointer, MatchErrorCallback callback, IntPtr secondaryCallback);

            // Matching Players

            [DllImport("__Internal")]
            internal static extern void GKMatchmaker_addPlayersToMatch(
                HandleRef selfPtr,
                /* InteropGKMatch */IntPtr matchPointer,
                /* InteropGKMatchRequest */IntPtr matchRequestPointer,
                ErrorCallback callback, IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void GKMatchmaker_cancel(HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void GKMatchmaker_cancelPendingInviteToPlayer(
                HandleRef selfPtr, /* InteropGKPlayer */IntPtr playerPointer);

            [DllImport("__Internal")]
            internal static extern void GKMatchmaker_findMatchForRequest(
                HandleRef selfPtr,
                /* InteropGKMatchRequest */IntPtr requestPointer,
                MatchErrorCallback callback, IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void GKMatchmaker_findPlayersForHostedRequest(
                HandleRef selfPtr,
                /* InteropGKMatchRequest */IntPtr requestPointer,
                PlayersErrorCallback callback, IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void GKMatchmaker_finishMatchmakingForMatch(
                HandleRef selfPtr, /* InteropGKMatch */IntPtr matchPointer);

            [DllImport("__Internal")]
            internal static extern void GKMatchmaker_queryActivity(HandleRef selfPtr,
                                                                   IntErrorCallback callback, IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void GKMatchmaker_queryPlayerGroupActivity(HandleRef selfPtr,
                                                                              uint playerGroup,
                                                                              IntErrorCallback callback, IntPtr secondaryCallback);

            // Looking for Nearby Players

            [DllImport("__Internal")]
            internal static extern void GKMatchmaker_startBrowsingForNearbyPlayers(
                HandleRef selfPtr,
                PlayerBoolCallback callback, IntPtr secondaryCallback);

            [DllImport("__Internal")]
            internal static extern void GKMatchmaker_stopBrowsingForNearbyPlayers(HandleRef selfPtr);
        }

        #endregion
    }
}
#endif
