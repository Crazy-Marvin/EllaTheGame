using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EasyMobile.Demo
{
    public class GameServicesDemo_Multiplayer_TurnbasedKitchenSink : GameServicesDemo_Multiplayer_BaseControl
    {
        #region Inner classes

        [Serializable]
        public class MatchData
        {
            public int TurnCount { get; set; }

            public string WinnerName { get; set; }

            public byte[] ToByteArray()
            {
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(memoryStream, this);
                        return memoryStream.ToArray();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static MatchData FromByteArray(byte[] bytes)
            {
                try
                {
                    if (bytes == null)
                        throw new ArgumentNullException();

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        memoryStream.Write(bytes, 0, bytes.Length);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        object obj = binaryFormatter.Deserialize(memoryStream);
                        return obj is MatchData ? obj as MatchData : null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        #endregion

        #region Fields & Properties

        private const string VariantHint = "The match variant. The meaning of this parameter is defined by the game. " +
                                           "It usually indicates a particular game type or mode(for example \"capture the flag\", \"first to 10 points\", etc). " +
                                           "Setting this value to 0 (default) allows the player to be matched into any waiting match. " +
                                           "Setting it to a nonzero number to match the player only with players whose match request shares the same variant number. " +
                                           "This value must be between 1 and 1023 (inclusive).";

        private const string ExclusiveBitmaskHint = "If your game has multiple player roles (such as farmer, archer, and wizard) " +
                                                    "and you want to restrict auto-matched games to one player of each role, " +
                                                    "add an exclusive bitmask to your match request.When auto-matching with this option, " +
                                                    "players will only be considered for a match when the logical AND of their exclusive bitmasks is equal to 0. " +
                                                    "In other words, this value represents the exclusive role the player making this request wants to play in the created match. " +
                                                    "If this value is 0 (default) it will be ignored. " +
                                                    "If you're creating a match with the standard matchmaker UI, this value will also be ignored.";

        private const string MinPlayersHint = "The minimum number of players that may join the match, " +
                                              "including the player who is making the match request. Must be at least 2 (default).";

        private const string MaxPlayersHint = "The maximum number of players that may join the match, " +
                                              "including the player who is making the match request. " +
                                              "Must be equal or greater than \"minPlayers\" and may be no more than the maximum number of players " +
                                              "allowed for the turnbased type. Default value is 2.";

        private const string CreateQuickMatchHint = "Start a game with randomly selected opponent(s).";

        private const string CreateWithMatchmakerUIHint = "Start a game with built-in invitation screen.";

        private const string TakeTurnHint = "Take a turn.\n " +
                                            "Choose the next participant to play in \"Next Participant\" above. " +
                                            "If you choose \"Auto-match slot\" and there are automatch slots open, the turn will be passed to one of the automatch players. " +
                                            "Choosing \"Auto-match slot\" when there are no open automatch slots is an error.";

        private const string FinishHint = "Notify all the participants that the match is over.";

        private const string LeaveMatchHint = "Call this method to leave the match.";

        private const string AcknowledgeMatchHint = "Acknowledges that a match was finished. " +
                                                    "Call this on a finished match that you have just shown to the user, " +
                                                    "to acknowledge that the user has seen the results of the finished match. " +
                                                    "This will remove the match from the user's inbox.";

        private const string RematchHint = "Request a rematch. " +
                                           "This can be used on a finished match in order to start a new match with the same opponents.";

        private const string ShowMatchesUIHint = "Show the standard UI where player can pick a match or accept an invitations.";

        private const string GetAllMatchesHint = "Return all matches' data.";

        private const string RegisterMatchDelegateHint = "Register a match delegate to be called when a match arrives. " +
                                                         "Matches may arrive as notifications on the device when it's the player's turn. " +
                                                         "If the match arrived via notification (this can be determined from the delegate's parameters), " +
                                                         "the recommended implementation is to take the player directly to the game screen so they can play their turn.";

        private const string NextParticipantHint = "Pick next participant to pass the turn when calling \"Take Turn\" " +
                                                   "or \"Leave During Turn\".";

        private const string MatchFinishedMessage = "Click the \"Acknowledge Finished Match\" button to call \"AcknowledgeFinished\" method " +
                                                    "to acknowledge that the user has seen the results of the finished match. " +
                                                    "This will remove the match from the user's inbox.";

        private const string NullMatchMessage = "Please create a match first.";

        private const string ExpiredMatchMessage = "[Google Play Games only]\n" +
                                                   "The match expired. A match expires when a user does not respond to an invitation or turn notification for two weeks. " +
                                                   "A match also expires in one day if there is an empty auto-match slot available " +
                                                   "but Google Play games services cannot find a user to auto-match.";

        private const string DeletedMatchMessage = " [Google Play Games only]\nThe match has been deleted.";

        private const string CancelledMatchMessage = "[Google Play Games only]\n" +
                                                     "The match was cancelled by one of the participants. " +
                                                     "This might occur, for example, " +
                                                     "if a user who was invited to the match declined the invitation or if a participant explicitly cancels the match. " +
                                                     "Google Play games services allows participants to cancel the match at any point after joining a match " +
                                                     "(if you game interface supports this action).";

        private const string SelectedParticipantLeftMessage = "The selected participant has left the match, please choose a different one.";

        private const string AllOpponentsLeftMessage = "All your opponent(s) had left the match.";

        [SerializeField]
        private Button getAllMatchesButton = null,
            showMatchesUIButton = null,
            takeTurnButton = null,
            finishButton = null,
            acknowledgeMatchButton = null,
            leaveMatchButton = null,
            rematchButton = null,
            showMatchInfosButton = null,
            showMatchDataButton = null,
            showSelfInfosButton = null,
            showOpponentsInfosButton = null,
            showNextParticipantDetailsButton = null,
            registerMatchDelegateButton = null;

        [SerializeField]
        private InputField variantInputField = null,
            exclusiveBitmaskInputField = null,
            minPlayersInputField = null,
            maxPlayersInputField = null;

        [SerializeField]
        private Dropdown participantsDropdown = null;

        [SerializeField]
        private Text maxDataSizeText = null;

        [SerializeField]
        private GameObject matchDelegateRegisteredUI = null,
            isMyTurnUI = null;

        [SerializeField]
        private string matchDelegateRegisteredText = "Is match delegate registered";

        [SerializeField]
        private Button variantHintButton = null,
            bitmaskHintButton = null,
            minPlayersHintButton = null,
            maxPlayersHintButton = null,
            createQuickMatchHintButton = null,
            createWithMatchmakerUIHintButton = null,
            takeTurnHintButton = null,
            finishHintButton = null,
            acknowledgeMatchHintButton = null,
            leaveMatchHintButton = null,
            rematchHintButton = null,
            showMatchesUIHintButton = null,
            getAllMatchesHintButton = null,
            registerMatchDelegateHintButton = null,
            nextParticipantHintButton = null;

        [Space]
        [SerializeField]
        private GameObject allMatchesRootPanel = null;

        [SerializeField]
        private Transform allMatchesContent = null;

        [SerializeField]
        private Button matchButton = null,
            hideAllMatchesPanelButton = null;

        public uint Variant { get; private set; }

        public uint ExclusiveBitmask { get; private set; }

        public uint MinPlayers { get; private set; }

        public uint MaxPlayers { get; private set; }

        public uint MaxPlayersAllowed { get; private set; }

        public bool IsMatchDelegateRegistered { get; private set; }

        public TurnBasedMatch CurrentMatch { get; private set; }

        public MatchData CurrentMatchData { get; private set; }

        public Participant[] CurrentOpponents { get; private set; }

        public bool IsMyTurn
        {
            get
            {
                if (CurrentMatch == null)
                    return false;

                return CurrentMatch.IsMyTurn && canTakeTurn;
            }
        }

        public Participant SelectedParticipant
        {
            get
            {
                if (CurrentOpponents == null)
                    return null;

                var index = participantsDropdown.value;
                if (index < 0 || index >= CurrentOpponents.Length)
                    return null;

                return CurrentOpponents[index];
            }
        }

        public bool AllOpponensLeft
        {
            get
            {
                if (CurrentOpponents == null || CurrentOpponents.Length < 1 || CurrentMatch == null || CurrentMatch.HasVacantSlot)
                    return false;

                return !CurrentOpponents.Any(participant => participant.Status != Participant.ParticipantStatus.Left &&
                    participant.Status != Participant.ParticipantStatus.Done);
            }
        }

        public bool SelectedParticipantLeftMatch
        {
            get
            {
                var participant = SelectedParticipant;
                if (participant == null)
                    return false;

                return participant.Status == Participant.ParticipantStatus.Left ||
                participant.Status == Participant.ParticipantStatus.Done;
            }
        }

        public MatchRequest MatchRequest
        {
            get
            {
                var matchRequest = new MatchRequest()
                {
                    ExclusiveBitmask = ExclusiveBitmask,
                    MinPlayers = MinPlayers,
                    MaxPlayers = MaxPlayers
                };

                if (Variant > 0 && Variant <= 1023)
                    matchRequest.Variant = Variant;

                return matchRequest;
            }
        }

        public bool CanRematch
        {
            get
            {
                return CurrentMatch != null && CurrentMatch.Status == TurnBasedMatch.MatchStatus.Ended;
            }
        }

        protected override MatchType MatchType { get { return MatchType.TurnBased; } }

        private bool canTakeTurn = false;
        private List<Button> createdMatchButtons = new List<Button>();
        private uint maxVariant = 511;

        #endregion

        #region MonoBehaviours

        protected override void Awake()
        {
            base.Awake();

            RefreshParticipantsDropDown();
            
            InitButtons();
            InitInputFields();
        }

        protected override void LateStart()
        {

            MaxPlayersAllowed = MatchRequest.GetMaxPlayersAllowed(MatchType.TurnBased);
            maxDataSizeText.text = "Max data size: " + GameServices.TurnBased.GetMaxMatchDataSize() + " byte(s)";
            RegisterMatchDelegate();
        }

        protected virtual void FixedUpdate()
        {
            demoUtils.DisplayBool(isMyTurnUI, IsMyTurn, "Your turn");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (IsMatchDelegateRegistered)
                GameServices.TurnBased.RegisterMatchDelegate((_, __, ___) => { });
        }

        #endregion

        #region Main APIs

        protected override void CreateQuickMatch()
        {
            StartCreateQuickMatchSpinningUI();
            GameServices.TurnBased.CreateQuickMatch( MatchRequest, OnCreateQuickMatch);
        }

        protected override void CreateWithMatchmaker()
        {
            GameServices.TurnBased.CreateWithMatchmakerUI(
                MatchRequest,
                () => { },
                error => NativeUI.Alert("Error", "Failed to create a match with MatchMakerUI. Error: " + error));
        }

        protected override void AcceptInvitation(Invitation invitation)
        {
            GameServices.TurnBased.AcceptInvitation(invitation, (flag, match) =>
            {
                if (!flag)
                {
                    NativeUI.Alert("Error", "Failed to accept an invitation!!!");
                    return;
                }

                OnMatchReceived(match, true, false);
            });
        }

        protected override void DeclineInvitation(Invitation invitation)
        {
            GameServices.TurnBased.DeclineInvitation(invitation);
        }

        public void GetAllMatches()
        {
            GameServices.TurnBased.GetAllMatches(OnGetAllMatches);
        }

        public void ShowMatchesUI()
        {
            GameServices.TurnBased.ShowMatchesUI();
        }

        public void TakeTurn()
        {
            if (CurrentMatch == null)
            {
                NativeUI.Alert("Error", NullMatchMessage);
                return;
            }

            if (!IsMyTurn)
            {
                NativeUI.Alert("Warning", "Not your turn.");
                return;
            }

            if (CurrentMatchData == null)
            {
                NativeUI.Alert("Error", "Couldn't find any match data.");
                return;
            }

            if (AllOpponensLeft)
            {
                NativeUI.Alert("Error", AllOpponentsLeftMessage);
                return;
            }

            if (SelectedParticipantLeftMatch)
            {
                NativeUI.Alert("Error", SelectedParticipantLeftMessage);
                return;
            }

            GameServices.TurnBased.TakeTurn(CurrentMatch, CurrentMatchData.ToByteArray(), SelectedParticipant,
                success =>
                {
                    if (success)
                    {
                        canTakeTurn = false;
                        NativeUI.Alert("Success", "Take turn successfully.");
                        return;
                    }

                    NativeUI.Alert("Error", "Failed to take turn.");
                });
        }

        public void Finish()
        {
            if (CurrentMatch == null)
            {
                NativeUI.Alert("Error", NullMatchMessage);
                return;
            }

            if (!IsMyTurn)
            {
                NativeUI.Alert("Warning", "Not your turn.");
                return;
            }

            if (CurrentMatchData == null)
            {
                NativeUI.Alert("Error", "Couldn't find any match data.");
                return;
            }

            MatchOutcome outcome = new MatchOutcome();
            outcome.SetParticipantResult(CurrentMatch.SelfParticipantId, MatchOutcome.ParticipantResult.Won);
            foreach (var id in CurrentOpponents.Select(p => p.ParticipantId))
                outcome.SetParticipantResult(id, MatchOutcome.ParticipantResult.Lost);

            CurrentMatchData.WinnerName = CurrentMatch.Self.DisplayName;
            var callback = GetAlertCallbackAction("Finished the match successfully.", "Failed to finish the match.");
            callback += success =>
            {
                if (success)
                    canTakeTurn = false;
            };
            GameServices.TurnBased.Finish(CurrentMatch, CurrentMatchData.ToByteArray(), outcome, callback);
        }

        public void LeaveMatch()
        {
            if (CurrentMatch == null)
            {
                NativeUI.Alert("Error", NullMatchMessage);
                return;
            }

            if (AllOpponensLeft)
            {
                NativeUI.Alert("Game Over", AllOpponentsLeftMessage);
                return;
            }

            if (SelectedParticipantLeftMatch)
            {
                NativeUI.Alert("Error", SelectedParticipantLeftMessage);
                return;
            }

            var id = SelectedParticipant != null ? SelectedParticipant.ParticipantId : null;

            var message = IsMyTurn ? "Leave during your turn?" : "It's not your turn, want to leave?";
            var alert = NativeUI.ShowTwoButtonAlert("Leave match", message, "Yes", "No");
            if (alert != null)
            {
                alert.OnComplete += button =>
                {
                    if (button != 0)
                        return;

                    var action = GetAlertCallbackAction("Leave the match successfully.", "Failed to leave the match");
                    action += success =>
                    {
                        if (success)
                            canTakeTurn = false;
                    };

                    if (IsMyTurn)
                    {
                        GameServices.TurnBased.LeaveMatchInTurn(CurrentMatch, id, action);
                        return;
                    }

                    GameServices.TurnBased.LeaveMatch(CurrentMatch, action);
                };
            }
        }

        public void Rematch()
        {
            if (!CanRematch)
            {
                NativeUI.Alert("Error", "You can't rematch now.");
                return;
            }

            GameServices.TurnBased.Rematch(CurrentMatch, (success, match) =>
                {
                    if (success)
                    {
                        NativeUI.Alert("Rematch success", "Rematch successfully, a new match will be started right now...");
                        OnMatchReceived(match, true, false);
                    }
                    else
                    {
                        NativeUI.Alert("Rematch failed", "Failed to rematch");
                    }
                });
        }

        public void AcknowledgeFinishedMatch()
        {
            if (CurrentMatch == null)
            {
                NativeUI.Alert("Error", NullMatchMessage);
                return;
            }

            GameServices.TurnBased.AcknowledgeFinished(CurrentMatch, GetAlertCallbackAction(
                    "Acknowledged finished match successfully.",
                    "Failed to acknowledge finished match"));
        }

        public void RegisterMatchDelegate()
        {
            IsMatchDelegateRegistered = true;
            GameServices.TurnBased.RegisterMatchDelegate(OnMatchReceived);
            demoUtils.DisplayBool(matchDelegateRegisteredUI, true, matchDelegateRegisteredText);
        }

        #endregion

        #region Show hint methods

        public void ShowVariantHint()
        {
            NativeUI.Alert("Variant", VariantHint);
        }

        public void ShowExclusiveBitmaskHint()
        {
            NativeUI.Alert("Exclusive Bitmask", ExclusiveBitmaskHint);
        }

        public void ShowMinPlayersHint()
        {
            NativeUI.Alert("Min players", MinPlayersHint);
        }

        public void ShowMaxPlayersHint()
        {
            NativeUI.Alert("Max players", MaxPlayersHint);
        }

        public void ShowCreateQuickMatchHint()
        {
            NativeUI.Alert("Create Quick Match", CreateQuickMatchHint);
        }

        public void ShowCreateWithmatchmakerHint()
        {
            NativeUI.Alert("Create With Matchmaker UI", CreateWithMatchmakerUIHint);
        }

        public void ShowTakeTurnHint()
        {
            NativeUI.Alert("Take Turn", TakeTurnHint);
        }

        public void ShowFinishHint()
        {
            NativeUI.Alert("Finish", FinishHint);
        }

        public void ShowAcknowledgeMatchHint()
        {
            NativeUI.Alert("Acknowledge Finished", AcknowledgeMatchHint);
        }

        public void ShowLeaveHint()
        {
            NativeUI.Alert("Leave Match", LeaveMatchHint);
        }

        public void ShowRematchHint()
        {
            NativeUI.Alert("Rematch", RematchHint);
        }

        public void ShowShowMatchesUIHint()
        {
            NativeUI.Alert("Show Matches UI", ShowMatchesUIHint);
        }

        public void ShowGetAllMatchesHint()
        {
            NativeUI.Alert("Get All Matches", GetAllMatchesHint);
        }

        public void ShowRegisterMatchDelegateHint()
        {
            NativeUI.Alert("Register Match Delegate", RegisterMatchDelegateHint);
        }

        public void ShowNextParticipantHint()
        {
            NativeUI.Alert("Next Participant", NextParticipantHint);
        }

        #endregion

        #region Callbacks

        private void OnMatchReceived(TurnBasedMatch match, bool autoLaunch, bool playerWantsToQuit)
        {
            Debug.Log(string.Format("[OnMatchReceived]. match: {0}, autoLaunch: {1}, playerWantsToQuit: {2}",
                match, autoLaunch, playerWantsToQuit));

            if (match == null)
            {
                NativeUI.Alert("Error", "Received a null match.");
                return;
            }

            // This only happens on Game Center platform when the local player
            // removes the match in the matches UI while being the turn holder.
            // We'll end the local player's turn and pass the match to a next
            // participant. If there's no active participant left we'll end the match.
            if (playerWantsToQuit)
            {
                if (match.HasVacantSlot)
                {
                    GameServices.TurnBased.LeaveMatchInTurn(match, "", null);
                    return;
                }

                var nextParticipant = match.Participants.FirstOrDefault(
                                          p => p.ParticipantId != match.SelfParticipantId &&
                                          (p.Status == Participant.ParticipantStatus.Joined ||
                                          p.Status == Participant.ParticipantStatus.Invited ||
                                          p.Status == Participant.ParticipantStatus.Matching));

                if (nextParticipant != default(Participant))
                {
                    GameServices.TurnBased.LeaveMatchInTurn(
                        match,
                        nextParticipant.ParticipantId,
                        null
                    );
                }
                else
                {
                    // No valid next participant, match ends here.
                    // In this case we'll set the outcome for all players as Tied for demo purpose.
                    // In a real game you may determine the outcome based on the game data and your game logic.
                    MatchOutcome outcome = new MatchOutcome();
                    foreach (var id in match.Participants.Select(p => p.ParticipantId))
                    {
                        var result = MatchOutcome.ParticipantResult.Tied;
                        outcome.SetParticipantResult(id, result);
                    }
                    GameServices.TurnBased.Finish(match, match.Data, outcome, null);
                }

                return;
            }

            if (CurrentMatch != null && CurrentMatch.MatchId != match.MatchId)
            {
                var alert = NativeUI.ShowTwoButtonAlert("Received Different Match",
                                "A different match has been arrived, do you want to replace it with the current one?", "Yes", "No");

                if (alert != null)
                {
                    alert.OnComplete += button =>
                    {
                        if (button == 0)
                            CheckAndPlayMatch(match, playerWantsToQuit);
                    };
                    return;
                }

                CheckAndPlayMatch(match, playerWantsToQuit);
                return;
            }

            CheckAndPlayMatch(match, playerWantsToQuit);
        }

        private void OnCreateQuickMatch(bool success, TurnBasedMatch match)
        {
            if (IsDestroyed)
                return;

            StopCreateQuickMatchSpinningUI();
            if (!success)
            {
                NativeUI.Alert("Error", "Failed to create a quick match.");
                return;
            }

            if (match == null)
            {
                NativeUI.Alert("Error", "Received a null \"TurnBasedMatch\" .");
                return;
            }

            OnMatchReceived(match, true, false);
        }

        private void OnGetAllMatches(TurnBasedMatch[] matches)
        {
            if (IsDestroyed)
                return;

            if (matches == null)
            {
                NativeUI.Alert("All Matches", "There's no match to show.");
                return;
            }

            ShowAllMatchesPanel(matches);
        }

        #endregion

        #region Others

        private void CheckAndPlayMatch(TurnBasedMatch match, bool playerWantsToQuit)
        {
            if (match.Data != null && match.Data.Length > 0 && MatchData.FromByteArray(match.Data) == null)
            {
                NativeUI.Alert("Error", "The arrived match can't be opened in this scene. You might want to open it in the TicTacToe demo instead.");
                return;
            }

            CurrentMatch = match;
            CurrentOpponents = CurrentMatch.Participants.Where(p => p.ParticipantId != CurrentMatch.SelfParticipantId).ToArray();
            RefreshParticipantsDropDown();
            canTakeTurn = true;

            if (CurrentMatch.Data == null || CurrentMatch.Data.Length < 1) /// New game detected...
                CurrentMatchData = new MatchData() { TurnCount = 0 };
            else
                CurrentMatchData = MatchData.FromByteArray(CurrentMatch.Data);

            if (CurrentMatch.Status == TurnBasedMatch.MatchStatus.Ended)
            {
                canTakeTurn = false;
                var result = string.Format("Winner: {0}\nTurnCount: {1}\n\n", CurrentMatchData.WinnerName ?? "null", CurrentMatchData.TurnCount);
                NativeUI.Alert("Finished Match Arrived", result + MatchFinishedMessage + "\n\nMatch info:\n" + GetTurnbasedMatchDisplayString(CurrentMatch));
                return;
            }
            else if (CurrentMatch.Status == TurnBasedMatch.MatchStatus.Cancelled)
            {
                NativeUI.Alert("Cancelled Match Arrived", CancelledMatchMessage);
                return;
            }
            else if (CurrentMatch.Status == TurnBasedMatch.MatchStatus.Deleted)
            {
                NativeUI.Alert("Deleted Match Arrived", DeletedMatchMessage);
                return;
            }
            else if (CurrentMatch.Status == TurnBasedMatch.MatchStatus.Expired)
            {
                NativeUI.Alert("Expired Match Arrived", ExpiredMatchMessage);
                return;
            }

            if (AllOpponensLeft)
            {
                NativeUI.Alert("Game Over", AllOpponentsLeftMessage);
                return;
            }

            CurrentMatchData.TurnCount++;
            NativeUI.Alert("Match Arrived", "New match data has been arrived:\n" + GetTurnbasedMatchDisplayString(match));
        }

        private void RefreshParticipantsDropDown()
        {
            participantsDropdown.ClearOptions();

            if (CurrentOpponents == null)
                return;

            var options = new List<Dropdown.OptionData>();
            foreach (var participant in CurrentOpponents)
            {
                if (participant.ParticipantId != CurrentMatch.SelfParticipantId)
                {
                    var displayName = string.Format("<b><i>[{0}]</i></b>{1}", participant.Status, participant.DisplayName);
                    options.Add(new Dropdown.OptionData(displayName));
                }
            }

            var openSlots = CurrentMatch.PlayerCount - CurrentOpponents.Length - 1;
            for (int i = 0; i < openSlots; i++)
                options.Add(new Dropdown.OptionData("Auto-match slot"));

            participantsDropdown.AddOptions(options);
        }

        private void ShowMatchInfo()
        {
            if (CurrentMatch == null)
            {
                NativeUI.Alert("Error", NullMatchMessage);
                return;
            }

            NativeUI.Alert("Current Match Info", GetTurnbasedMatchDisplayString(CurrentMatch));
        }

        private void ShowMatchData()
        {
            if (CurrentMatch == null)
            {
                NativeUI.Alert("Error", NullMatchMessage);
                return;
            }

            if (CurrentMatchData == null)
            {
                NativeUI.Alert("Error", "The current match doesn't have any data to show.");
                return;
            }

            var bytes = CurrentMatchData.ToByteArray();
            string dataSize = bytes != null ? (bytes.Length.ToString() + " byte(s)") : "Error";
            string message = string.Format("Turn Count: {0}\nWinner Name: {1}\nSize: {2}",
                                 CurrentMatchData.TurnCount, CurrentMatchData.WinnerName, dataSize);
            NativeUI.Alert("Match Data", message);
        }

        private void ShowSelfInfo()
        {
            if (CurrentMatch == null)
            {
                NativeUI.Alert("Error", NullMatchMessage);
                return;
            }

            NativeUI.Alert("Self infomations", GetParticipantDisplayString(CurrentMatch.Self));
        }

        private void ShowOpponentsInfo()
        {
            if (CurrentMatch == null)
            {
                NativeUI.Alert("Error", NullMatchMessage);
                return;
            }

            var opponents = CurrentMatch.Participants.Where(p => p.ParticipantId != CurrentMatch.SelfParticipantId);
            if (opponents.Count() < 1)
            {
                NativeUI.Alert("No Opponent", "Noone has joined your match yet. Auto-match players only appear after they joined the game.");
                return;
            }

            var info = string.Join("\n", opponents.Select(p => GetParticipantDisplayString(p)).ToArray());
            NativeUI.Alert("Opponents", info);
        }

        private void ShowNextParticipantDetails()
        {
            var nextParticipant = SelectedParticipant;
            if (nextParticipant == null)
            {
                NativeUI.Alert("Next Participant Info", "There's no infomations to show.");
                return;
            }

            NativeUI.Alert("Next Participant Info", GetParticipantDisplayString(nextParticipant));
        }

        private void ShowAllMatchesPanel(TurnBasedMatch[] matches)
        {
            foreach(var match in matches)
            {
                if (match != null)
                {
                    Button button = Instantiate(matchButton, allMatchesContent);
                    Text text = button.GetComponentInChildren<Text>();
                    text.text = "<b>Match id:</b> " + match.MatchId + "\n";
                    text.text += "<b>Status:</b> " + match.Status + ", ";
                    text.text += "<b>Is your turn:</b> " + match.IsMyTurn;
                    button.gameObject.SetActive(true);
                    button.onClick.AddListener(() =>
                    {
                        demoUtils.PlayButtonSound();
                        NativeUI.Alert("Turnbased Match", GetTurnbasedMatchDisplayString(match));
                    });
                    createdMatchButtons.Add(button);
                }
            }
            allMatchesRootPanel.SetActive(true);
        }

        private void HideAllMatchesPanel()
        {
            foreach (var button in createdMatchButtons)
                Destroy(button.gameObject);
            createdMatchButtons.Clear();
            allMatchesRootPanel.SetActive(false);
        }

        private void InitButtons()
        {
            getAllMatchesButton.onClick.AddListener(GetAllMatches);
            showMatchesUIButton.onClick.AddListener(ShowMatchesUI);
            takeTurnButton.onClick.AddListener(TakeTurn);
            finishButton.onClick.AddListener(Finish);
            acknowledgeMatchButton.onClick.AddListener(AcknowledgeFinishedMatch);
            leaveMatchButton.onClick.AddListener(LeaveMatch);
            rematchButton.onClick.AddListener(Rematch);
            registerMatchDelegateButton.onClick.AddListener(RegisterMatchDelegate);
            showMatchInfosButton.onClick.AddListener(ShowMatchInfo);
            showMatchDataButton.onClick.AddListener(ShowMatchData);
            showSelfInfosButton.onClick.AddListener(ShowSelfInfo);
            showOpponentsInfosButton.onClick.AddListener(ShowOpponentsInfo);
            showNextParticipantDetailsButton.onClick.AddListener(ShowNextParticipantDetails);
            hideAllMatchesPanelButton.onClick.AddListener(HideAllMatchesPanel);

            variantHintButton.onClick.AddListener(ShowVariantHint);
            bitmaskHintButton.onClick.AddListener(ShowExclusiveBitmaskHint);
            minPlayersHintButton.onClick.AddListener(ShowMinPlayersHint);
            maxPlayersHintButton.onClick.AddListener(ShowMaxPlayersHint);
            createQuickMatchHintButton.onClick.AddListener(ShowCreateQuickMatchHint);
            createWithMatchmakerUIHintButton.onClick.AddListener(ShowCreateWithmatchmakerHint);
            takeTurnHintButton.onClick.AddListener(ShowTakeTurnHint);
            finishHintButton.onClick.AddListener(ShowFinishHint);
            acknowledgeMatchHintButton.onClick.AddListener(ShowAcknowledgeMatchHint);
            leaveMatchHintButton.onClick.AddListener(ShowLeaveHint);
            rematchHintButton.onClick.AddListener(ShowRematchHint);
            showMatchesUIHintButton.onClick.AddListener(ShowShowMatchesUIHint);
            getAllMatchesHintButton.onClick.AddListener(ShowGetAllMatchesHint);
            registerMatchDelegateHintButton.onClick.AddListener(ShowRegisterMatchDelegateHint);
            nextParticipantHintButton.onClick.AddListener(ShowNextParticipantHint);
        }

        private void InitInputFields()
        {
            variantInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
            exclusiveBitmaskInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
            minPlayersInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
            maxPlayersInputField.keyboardType = TouchScreenKeyboardType.NumberPad;

            variantInputField.onEndEdit.AddListener(OnVariantInputChanged);
            exclusiveBitmaskInputField.onEndEdit.AddListener(OnExclusiveBitmaskInputChanged);
            minPlayersInputField.onEndEdit.AddListener(OnMinPlayersInputChanged);
            maxPlayersInputField.onEndEdit.AddListener(OnMaxPlayersInputChanged);

            MinPlayers = 2;
            MaxPlayers = 2;
            minPlayersInputField.text = "2";
            maxPlayersInputField.text = "2";
            exclusiveBitmaskInputField.text = "0";
            variantInputField.text = "0";
        }

        private void OnVariantInputChanged(string value)
        {
            uint variant = 0;
            if (!uint.TryParse(value, out variant))
            {
                Variant = 0;
                variantInputField.text = "0";
                return;
            }

            if (variant > maxVariant)
            {
                Variant = maxVariant;
                variantInputField.text = maxVariant.ToString();
                return;
            }

            Variant = variant;
        }

        private void OnExclusiveBitmaskInputChanged(string value)
        {
            uint bitmask = 0;
            if (!uint.TryParse(value, out bitmask))
            {
                ExclusiveBitmask = 0;
                exclusiveBitmaskInputField.text = "0";
                return;
            }

            ExclusiveBitmask = bitmask;
        }

        private void OnMinPlayersInputChanged(string value)
        {
            uint minPlayers = 0;
            if (!uint.TryParse(value, out minPlayers))
            {
                MinPlayers = 2;
                minPlayersInputField.text = "2";
                return;
            }

            if (minPlayers < 2)
            {
                MinPlayers = 2;
                minPlayersInputField.text = "2";
                return;
            }

            if (MaxPlayers < minPlayers)
            {
                MaxPlayers = (uint)Mathf.Clamp(minPlayers, 2, MaxPlayersAllowed);
                maxPlayersInputField.text = MaxPlayers.ToString();
            }

            if (minPlayers > MaxPlayersAllowed)
            {
                MinPlayers = MaxPlayersAllowed;
                minPlayersInputField.text = MaxPlayersAllowed.ToString();
                return;
            }

            MinPlayers = minPlayers;
        }

        private void OnMaxPlayersInputChanged(string value)
        {
            uint maxPlayers = 0;
            if (!uint.TryParse(value, out maxPlayers))
            {
                MaxPlayers = 2;
                maxPlayersInputField.text = "2";
                return;
            }

            if (maxPlayers > MaxPlayersAllowed)
            {
                MaxPlayers = MaxPlayersAllowed;
                maxPlayersInputField.text = MaxPlayersAllowed.ToString();
                return;
            }

            if (maxPlayers < MinPlayers)
            {
                MaxPlayers = MinPlayers;
                maxPlayersInputField.text = MinPlayers.ToString();
                return;
            }

            MaxPlayers = maxPlayers;
        }

        private Action<bool> GetAlertCallbackAction(string successMessage, string failedMessage)
        {
            return success =>
            {
                var title = success ? "Success" : "Fail";
                NativeUI.Alert(title, success ? successMessage : failedMessage);
            };
        }

        public override void StartCreateQuickMatchSpinningUI()
        {
            base.StartCreateQuickMatchSpinningUI();
            createQuickMatchHintButton.gameObject.SetActive(false);
        }

        public override void StopCreateQuickMatchSpinningUI()
        {
            base.StopCreateQuickMatchSpinningUI();
            createQuickMatchHintButton.gameObject.SetActive(true);
        }

        #endregion
    }
}
