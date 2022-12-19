#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.iOS.UIKit;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.iOS.GameKit
{
    using DelegateForwarder = EasyMobile.Internal.iOS.GameKit.GKMatchmakerViewControllerDelegateForwarder;

    /// <summary>
    /// A user interface for inviting friends to a match or for allowing 
    /// Game Center to fill the remaining players needed for a match.
    /// </summary>
    internal class GKMatchmakerViewController : UIViewController /* GKMatchmakerViewController actually directly inherits UINavigationController
    // rather than UIViewController. */
    {
        #region GKMatchmakerViewControllerDelegate

        /// <summary>
        /// A class implements the GKMatchmakerViewControllerDelegate protocol to receive 
        /// notifications from a GKMatchmakerViewController object. The delegate is called 
        /// if a new match has been successfully created, if the user cancels matchmaking, 
        /// and if an error occurs. In all three cases, the delegate should dismiss the view controller.
        /// </summary>
        public interface GKMatchmakerViewControllerDelegate
        {
            /// <summary>
            /// Called when a peer-to-peer match is found.
            /// </summary>
            /// <param name="viewController">View controller.</param>
            /// <param name="match">Match.</param>
            void MatchmakerViewControllerDidFindMatch(GKMatchmakerViewController viewController, GKMatch match);

            /// <summary>
            /// Called when a hosted match is found.
            /// </summary>
            /// <param name="viewController">View controller.</param>
            /// <param name="players">Players.</param>
            void MatchmakerViewControllerDidFindHostedPlayers(GKMatchmakerViewController viewController, NSArray<GKPlayer> players);

            /// <summary>
            /// Matchmakers the view controller was cancelled.
            /// </summary>
            /// <param name="viewController">View controller.</param>
            void MatchmakerViewControllerWasCancelled(GKMatchmakerViewController viewController);

            /// <summary>
            /// Called when the view controller encounters an unrecoverable error.
            /// </summary>
            /// <param name="viewController">View controller.</param>
            /// <param name="error">Error.</param>
            void MatchmakerViewControllerDidFailWithError(GKMatchmakerViewController viewController, NSError error);

            /// <summary>
            /// Called when a player in a hosted match accepts the invitation.
            /// </summary>
            /// <param name="viewController">View controller.</param>
            /// <param name="player">Player.</param>
            void MatchmakerViewControllerHostedPlayerDidAccept(GKMatchmakerViewController viewController, GKPlayer player);
        }

        #endregion

        private DelegateForwarder mDelegateForwarder;
        private GKMatchmakerViewControllerDelegate mDelegate;
        private GKMatchRequest mMatchRequest;

        internal GKMatchmakerViewController(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Initializes a matchmaker view controller to respond to an invitation received from another player.
        /// </summary>
        /// <param name="invite">Invite.</param>
        public GKMatchmakerViewController(GKInvite invite)
            : this(C.GKMatchmakerViewController_initWithInvite(invite != null ? invite.ToPointer() : IntPtr.Zero))
        {
            // We're using a pointer returned by a native constructor: must call CFRelease().
            CoreFoundation.CFFunctions.CFRelease(this.ToPointer());
        }

        /// <summary>
        /// Initializes a matchmaker view controller to create a new match.
        /// </summary>
        /// <param name="request">Request.</param>
        public GKMatchmakerViewController(GKMatchRequest request)
            : this(C.GKMatchmakerViewController_initWithMatchRequest(request != null ? request.ToPointer() : IntPtr.Zero))
        {
            // We're using a pointer returned by a native constructor: must call CFRelease().
            CoreFoundation.CFFunctions.CFRelease(this.ToPointer());
        }

        /// <summary>
        /// The delegate for the matchmaker view controller.
        /// </summary>
        /// <value>The matchmaker delegate.</value>
        public GKMatchmakerViewControllerDelegate MatchmakerDelegate
        {
            get
            {
                return mDelegate;
            }
            set
            {
                mDelegate = value;

                if (mDelegate == null)
                {
                    // Nil out the native delegate.
                    mDelegateForwarder = null;
                    C.GKMatchmakerViewController_setMatchmakerDelegate(SelfPtr(), IntPtr.Zero);
                }
                else
                {
                    // Create a delegate forwarder if needed.
                    if (mDelegateForwarder == null)
                    {
                        mDelegateForwarder = InteropObjectFactory<DelegateForwarder>.Create(
                            () => new DelegateForwarder(),
                            fwd => fwd.ToPointer());

                        // Assign on native side.
                        C.GKMatchmakerViewController_setMatchmakerDelegate(SelfPtr(), mDelegateForwarder.ToPointer());
                    }

                    // Set delegate.
                    mDelegateForwarder.Listener = mDelegate;
                }
            }
        }

        /// <summary>
        /// A Boolean value that indicates whether the match is hosted or peer-to-peer.
        /// </summary>
        /// <value><c>true</c> if this instance is hosted; otherwise, <c>false</c>.</value>
        public bool IsHosted
        {
            get
            {
                return C.GKMatchmakerViewController_isHosted(SelfPtr());
            }
            set
            {
                C.GKMatchmakerViewController_hosted(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The configuration for the desired match.
        /// </summary>
        /// <value>The match request.</value>
        public GKMatchRequest MatchRequest
        {
            get
            {
                if (mMatchRequest == null)
                {
                    var reqPtr = C.GKMatchmakerViewController_matchRequest(SelfPtr());
                    mMatchRequest = InteropObjectFactory<GKMatchRequest>.FromPointer(reqPtr, p => new GKMatchRequest(p));
                    CoreFoundation.CFFunctions.CFRelease(reqPtr);    // balance ref count.
                }
                return mMatchRequest;
            }
        }

        /// <summary>
        /// Adds new players to an existing match instead of starting a new match.
        /// </summary>
        /// <param name="match">Match.</param>
        public void AddPlayersToMatch(GKMatch match)
        {
            if (match != null)
                C.GKMatchmakerViewController_addPlayersToMatch(SelfPtr(), match.ToPointer());
        }

        /// <summary>
        /// Updates a player’s status on the view to show that the player 
        /// has connected or disconnected from your server.
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="connected">If set to <c>true</c> connected.</param>
        public void SetHostedPlayerDidConnect(GKPlayer player, bool connected)
        {
            if (player != null)
                C.GKMatchmakerViewController_setHostedPlayerDidConnect(SelfPtr(), player.ToPointer(), connected);
        }

        #region C wrapper

        private static class C
        {
            // Initializing a Matchmaker View Controller

            [DllImport("__Internal")]
            internal static extern /* InteropGKMatchmakerViewController */ IntPtr GKMatchmakerViewController_initWithInvite(
                /* InteropGKInvite */ IntPtr invitePointer);

            [DllImport("__Internal")]
            internal static extern /* InteropGKMatchmakerViewController */
            IntPtr GKMatchmakerViewController_initWithMatchRequest(/* InteropGKMatchRequest */ IntPtr requestPointer);

            // Getting and Setting the Delegate

            [DllImport("__Internal")]
            internal static extern /* InteropGKMatchmakerViewControllerDelegate */
            IntPtr GKMatchmakerViewController_matchmakerDelegate(/* InteropGKMatchmakerViewController */ HandleRef selfPtr);

            [DllImport("__Internal")]
            internal static extern void GKMatchmakerViewController_setMatchmakerDelegate(
                /* InteropGKMatchmakerViewController */ HandleRef selfPtr,
                /* InteropGKMatchmakerViewControllerDelegate */IntPtr matchmakerDelegatePointer);

            // Matchmaker View Controller Properties

            [DllImport("__Internal")]
            internal static extern bool GKMatchmakerViewController_isHosted(HandleRef pointer);

            [DllImport("__Internal")]
            internal static extern void GKMatchmakerViewController_hosted(HandleRef pointer, bool value);

            [DllImport("__Internal")]
            internal static extern /* InteropGKMatchRequest */ IntPtr GKMatchmakerViewController_matchRequest(HandleRef selfPtr);

            // Add players to existed match

            [DllImport("__Internal")]
            internal static extern void GKMatchmakerViewController_addPlayersToMatch(
                HandleRef selfPtr, /* InteropGKMatch */IntPtr matchPointer);

            // Implementing Hosted Matches

            [DllImport("__Internal")]
            internal static extern void GKMatchmakerViewController_setHostedPlayerDidConnect(
                HandleRef selfPtr, /* InteropGKPlayer */IntPtr playerPointer, bool connected);
        }

        #endregion
    }
}
#endif
