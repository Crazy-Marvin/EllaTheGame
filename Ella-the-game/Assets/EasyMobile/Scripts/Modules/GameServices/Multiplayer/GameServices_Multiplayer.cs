using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using EasyMobile.Internal;
using EasyMobile.Internal.GameServices;

namespace EasyMobile
{
#if UNITY_IOS
    using EasyMobile.iOS.GameKit;
    using EasyMobile.Internal.GameServices.iOS;
#endif

#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS
    using GooglePlayGames;
    using GooglePlayGames.BasicApi;
    using GPGS_Invitation = GooglePlayGames.BasicApi.Multiplayer.Invitation;
    using GPGS_TurnBasedMatch = GooglePlayGames.BasicApi.Multiplayer.TurnBasedMatch;
#endif

    public partial class GameServices
    {
        #region Public API

        /// <summary>
        /// Delegate that handles an incoming multiplayer match invitation
        /// (for both real-time and turn-base matches).
        /// </summary>
        /// <param name="invitation">The invitation received.</param>
        /// <param name="shouldAutoAccept">If this is true, then the game should immediately
        /// accept the invitation and go to the game screen without prompting the user. If
        /// false, you should prompt the user before accepting the invitation. As an example,
        /// when a user taps on an invitation notification or message to accept it, it is
        /// clear that they want to accept that invitation right away, so the plugin calls this
        /// delegate with shouldAutoAccept = true. However, if we receive an incoming invitation
        /// that the player hasn't specifically indicated they wish to accept (for example,
        /// we received one silently while the app is in foreground), this delegate will be called
        /// with shouldAutoAccept = false to indicate that you should confirm with the user
        /// to see if they wish to accept or decline the invitation.</param>
        public delegate void InvitationReceivedDelegate(Invitation invitation, bool shouldAutoAccept);

        /// <summary>
        /// Turn-Based Multiplayer API entry point.
        /// </summary>
        /// <value>The turn based.</value>
        public static ITurnBasedMultiplayerClient TurnBased
        {
            get
            {
                if (sTurnBasedClient == null)
                    sTurnBasedClient = GetTurnBasedMultiplayerClient();

                return sTurnBasedClient;
            }
        }

        /// <summary>
        /// Real-Time Multiplayer API entry point.
        /// </summary>
        /// <value>The real time.</value>
        public static IRealTimeMultiplayerClient RealTime
        {
            get
            {
                if (sRealTimeClient == null)
                    sRealTimeClient = GetRealTimeMultiplayerClient();

                return sRealTimeClient;
            }
        }

        /// <summary>
        /// Gets the currently registered invitation delegate.
        /// </summary>
        /// <value>The invitation delegate.</value>
        public static InvitationReceivedDelegate InvitationDelegate
        {
            get
            {
                return sInvitationDelegate;
            }
        }

        private static ITurnBasedMultiplayerClient sTurnBasedClient;
        private static IRealTimeMultiplayerClient sRealTimeClient;
        private static InvitationReceivedDelegate sInvitationDelegate;

        /// <summary>
        /// Registers the invitation delegate for both real-time and turn-based matches.
        /// This should be done as soon as authentication completes, 
        /// i.e. in the handler of <see cref="UserLoginSucceeded"/> event.
        /// </summary>
        /// <param name="invitationDelegate">Invitation delegate.</param>
        public static void RegisterInvitationDelegate(InvitationReceivedDelegate invitationDelegate)
        {
            sInvitationDelegate = invitationDelegate;

#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS
            // Invoke delegate for pending invitations if any.
            if (sInvitationDelegate != null && sGPGSPendingInvitations != null)
            {
                while (sGPGSPendingInvitations.Count > 0)
                    sGPGSPendingInvitations.Dequeue()();

                sGPGSPendingInvitations = null;
            }
#endif
        }

        #endregion

        #region Private methods

        private static ITurnBasedMultiplayerClient GetTurnBasedMultiplayerClient()
        {
#if UNITY_EDITOR
            return new EditorTurnBasedMultiplayerClient();
#elif UNITY_IOS
            return new GCTurnBasedMultiplayerClient();
#elif UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS
            var gpgClient = new GPGTurnBasedMultiplayerClient(sGPGSPendingMatchDelegates);
            sGPGSPendingMatchDelegates = null;
            return gpgClient;
#else
            return new UnsupportedTurnBasedMultiplayerClient();
#endif
        }

        private static IRealTimeMultiplayerClient GetRealTimeMultiplayerClient()
        {
#if UNITY_EDITOR
            return new EditorRealTimeMultiplayerClient();
#elif UNITY_IOS
            return new GCRealTimeMultiplayerClient();
#elif UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS
            return new GPGRealTimeMultiplayerClient();
#else
            return new UnsupportedRealTimeMultiplayerClient();
#endif
        }

        #endregion

#if UNITY_ANDROID && EM_GPGS && EM_OBSOLETE_GPGS

        internal class GPGS_PendingTurnBasedMatchDelegate
        {
            public GPGS_TurnBasedMatch Match { get; private set; }

            public bool ShouldAutoLaunch { get; private set; }

            internal GPGS_PendingTurnBasedMatchDelegate(GPGS_TurnBasedMatch match, bool shouldAutoLaunch)
            {
                this.Match = match;
                this.ShouldAutoLaunch = shouldAutoLaunch;
            }
        }

        // Invitations whose delegates are pending to invoke.
        private static Queue<Action> sGPGSPendingInvitations = null;

        // Turn-Based matches whose delegates are pending to invoke.
        private static Queue<GPGS_PendingTurnBasedMatchDelegate> sGPGSPendingMatchDelegates = null;

        /// <summary>
        /// Invokes consumer invitation delegate if one is registered. Otherwise
        /// stores this handler in a queue so that it can be repeated later when a consumer
        /// delegate is registered. When being repeated, it also runs on main thread.
        /// </summary>
        /// <param name="gpgsInv">Gpgs inv.</param>
        /// <param name="shouldAutoAccept">If set to <c>true</c> should auto accept.</param>
        private static void OnGPGSInvitationReceived(GPGS_Invitation gpgsInv, bool shouldAutoAccept)
        {
            if (sInvitationDelegate != null)
            {
                // Invoke consumer delegate immediately.
                sInvitationDelegate(Invitation.FromGPGSInvitation(gpgsInv), shouldAutoAccept);
            }
            else
            {
                // Enqueue this invitation for later invoking when a consumer invitation delegate is registered.
                if (sGPGSPendingInvitations == null)
                    sGPGSPendingInvitations = new Queue<Action>();
                sGPGSPendingInvitations.Enqueue(RuntimeHelper.ToMainThread(() => OnGPGSInvitationReceived(gpgsInv, shouldAutoAccept)));
            }
        }

        private static void OnGPGSTBMatchReceived(GPGS_TurnBasedMatch match, bool shouldAutoLaunch)
        {
            // Enqueue this match for later invoking when a consumer match delegate is registered.
            if (sGPGSPendingMatchDelegates == null)
                sGPGSPendingMatchDelegates = new Queue<GPGS_PendingTurnBasedMatchDelegate>();
            sGPGSPendingMatchDelegates.Enqueue(new GPGS_PendingTurnBasedMatchDelegate(match, shouldAutoLaunch));
        }

#endif

#if UNITY_IOS
        
        private static void RegisterDefaultGKLocalPlayerListener()
        {
            GKLocalPlayer.LocalPlayer.RegisterListener(new InternalGKLocalPlayerListenerImpl());
        }

        // Local player listener for invite events.
        private class InternalGKLocalPlayerListenerImpl : GKLocalPlayerListenerImpl
        {
            public override void PlayerDidAcceptInvite(GKPlayer player, GKInvite invite)
            {
                // On iOS (Game Center), users receive an invitation as a message in the iMessage app.
                // This only runs if the user clicks the message, thus 'shouldAutoAccept' is always true.
                // Also the invitation delegate is actually called for Real-Time matches only.
                // For Turn-Based matches, the MatchDelegate is called instead because a user can join
                // multiple Turn-Based matches.
                RuntimeHelper.RunOnMainThread(() =>
                    {
                        if (sInvitationDelegate != null)
                            sInvitationDelegate(Invitation.FromGKInvite(invite, MatchType.RealTime), true);
                    });
            }
        }

#endif
    }
}

