using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    public class GameServicesDemo_Multiplayer_RealtimeKitchenSink : GameServicesDemo_Multiplayer_BaseControl, IRealTimeMultiplayerListener
    {
        #region Inner classes

        [Serializable]
        public class SampleData
        {
            public string Text { get; set; }

            public float Value { get; set; }

            public DateTime TimeStamp { get; set; }

            private byte[] dummyData = new byte[0];

            public int GetSize()
            {
                var bytes = ToByteArray();
                return bytes != null ? bytes.Length : 0;
            }

            public int GetDummySize()
            {
                return dummyData != null ? dummyData.Length : 0;
            }

            public int GetBaseSize()
            {
                return GetSize() - GetDummySize();
            }

            public void UpdateDummySize(int size)
            {
                if (size < 0)
                    return;

                dummyData = new byte[size];
            }

            public byte[] ToByteArray()
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, this);
                    return memoryStream.ToArray();
                }
            }

            public static SampleData FromByteArray(byte[] bytes)
            {
                if (bytes == null)
                    throw new ArgumentNullException();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    memoryStream.Write(bytes, 0, bytes.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    SampleData obj = (SampleData)binaryFormatter.Deserialize(memoryStream);
                    return obj;
                }
            }

            public override string ToString()
            {
                string result = "[SampleData]\n";
                result += "Text(string): " + Text + "\n";
                result += "Value(float): " + Value + "\n";
                result += "Timestamp(DateTime): " + TimeStamp + "\n";

                byte[] bytes = ToByteArray();
                result += "Size: " + (bytes != null ? bytes.Length.ToString() : "0") + " byte(s)";

                return result;
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

        private const string AcceptFromInboxHint = "Creates a real-time game starting with the inbox screen.";

        private const string SendMessageHint = "Send a message to a particular participant, " +
                                               "which can be selected via the \"Target Participant\" dropdown list above.";

        private const string SendMessageToAllHint = "Sends a message to all other participants.";

        private const string LeaveRoomHint = "Leaves the room.";

        private const string UseReliableMessageHint = "Determine if \"SendMessage\" and \"SendMessageToAll\" " +
                                                      "should send reliable or unreliable message. " +
                                                      "Unreliable messages are faster, but are not guaranteed to arrive and may arrive out of order.";

        private const string ShouldReinviteDisconnectedPlayerHint = "[Game Center only] " +
                                                                    "Called when a player in a two-player match was disconnected. " +
                                                                    "Your game should return \"true\" if it wants Game Kit to attempt to reconnect the player, " +
                                                                    "\"false\" if it wants to terminate the match.";

        private const string SampleDataHint = "These value will be sent to other participant(s) " +
                                              "when calling \"SendMessage\" or \"SendMessageToAll\". ";

        private const string ReceivedMessagesHint = "All the messages (or errors) received in " +
                                                    "\"OnRealTimeMessageReceived\" will be displayed here.";

        private const string OnLeftRoomMessage = "The current player has left the room. " +
                                                 "This may have happened because you called LeaveRoom, " +
                                                 "or because an error occurred and the player was dropped from the room. " +
                                                 "You should react by stopping your game and possibly showing an error screen " +
                                                 "(unless leaving the room was the player's request, naturally).";

        private const string OnParticipantLeftMessage = "Raises the participant left event. " +
                                                        "This is called during room setup if a player declines an invitation or leaves. " +
                                                        "The status of the participant can be inspected to determine the reason. " +
                                                        "If all players have left, the room is closed automatically.";

        private const string OnRoomConnectedMessage = "Notifies that room setup is finished. If result is true, " +
                                                      "you should react by starting to play the game; " +
                                                      "otherwise, show an error screen.";

        private const string OnRoomSetupProgressMessage = "Called during room setup to notify of room setup progress.";

        [SerializeField]
        private GameObject matchRequestRoot = null,
            matchCreationRoot = null,
            ingameRoot = null;

        [SerializeField]
        private Button acceptFromInboxButton = null,
            sendMessageButton = null,
            sendMessageToAllButton = null,
            getConnectedParticipantsButton = null,
            showTargetParticipantButton = null,
            getSelfButton = null,
            leaveRoomButton = null,
            clearReceivedMessagesButton = null;

        [SerializeField]
        private InputField variantInputField = null,
            exclusiveBitmaskInputField = null,
            minPlayersInputField = null,
            maxPlayersInputField = null,
            sampleDataTextInputField = null,
            sampleDataValueInputField = null,
            dummySizeInputField = null;

        [SerializeField]
        private Dropdown targetParticipantDropdown = null;

        [SerializeField]
        private Text finalSizeText = null;

        [SerializeField]
        private Scrollbar receivedMessagesVerticalScrollbar = null;

        [SerializeField]
        private Button variantHintButton = null,
            bitmaskHintButton = null,
            minPlayersHintButton = null,
            maxPlayersHintButton = null,
            createQuickMatchHintButton = null,
            createWithMatchmakerUIHintButton = null,
            acceptFromInboxHintButton = null,
            sendMessageHintButton = null,
            sendMessageToAllHintButton = null,
            leaveRoomHintButton = null,
            sampleDataHintButton = null,
            useReliableMessageHintButton = null,
            reiniviteDisconnectedPlayerHintButton = null,
            receivedMessagesHintButton = null;

        [SerializeField]
        private GameObject isRoomConnectedUI = null,
            receivedMessagesRoot = null;

        [SerializeField]
        private Text receivedMessagesTextPrefab = null;

        [Space]
        [SerializeField]
        private Toggle useMinimalDataToggle = null;

        [SerializeField]
        private GameObject sampleDataTextRoot = null,
            sampleDataValueRoot = null,
            sampleDataDummySizeRoot = null;

        [SerializeField, Tooltip("Show waiting room UI when accepting invitation?")]
        private bool showInvitationWaitingRoomUI = true;

        public uint Variant { get; private set; }

        public uint ExclusiveBitmask { get; set; }

        public uint MinPlayers { get; private set; }

        public uint MaxPlayers { get; private set; }

        public uint MaxPlayersAllowed { get; private set; }

        public bool UseReliableMessage { get; set; }

        public bool ShouldReInviteDisconnectedPlayer { get; set; }

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

        public Participant TargetParticipant
        {
            get
            {
                if (Opponents == null)
                    return null;

                int index = targetParticipantDropdown.value;
                if (index < 0 || index >= Opponents.Count)
                    return null;

                return Opponents[index];
            }
        }

        protected override MatchType MatchType { get { return MatchType.RealTime; } }

        private List<Participant> Opponents = new List<Participant>();
        private List<Text> ReceivedMessages = new List<Text>();
        private bool isShowingIngameUI = false, useMinimalData = false;
        private uint maxVariant = 511;

        #endregion

        #region MonoBehaviours

        protected override void Awake()
        {
            base.Awake();

            ShowSetupUI();
            InitButtons();
            InitInputFields();
            ClearTargetParticipantDropdown();
            RefreshFinalSizeText();

            useMinimalDataToggle.onValueChanged.AddListener(OnUseMinimalDataToggleValueChanged);
            UseReliableMessage = true;
            ShouldReInviteDisconnectedPlayer = true;
        }

        protected override void LateStart()
        {
            MaxPlayersAllowed = MatchRequest.GetMaxPlayersAllowed(MatchType.RealTime);
        }

        protected virtual void FixedUpdate()
        {
            if (GameServices.IsInitialized() && !Application.isEditor)
                demoUtils.DisplayBool(isRoomConnectedUI, IsRoomConnected(), "Is Room Connected");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (GameServices.IsInitialized())
                LeaveRoom();
        }

        #endregion

        #region IRealtimeMultiplayer client

        protected override void CreateQuickMatch()
        {
            createQuickMatchRequest++;
            GameServices.RealTime.CreateQuickMatch(MatchRequest, this);
            StartCreateQuickMatchSpinningUI();
        }

        protected override void CreateWithMatchmaker()
        {
            GameServices.RealTime.CreateWithMatchmakerUI(MatchRequest, this);
        }

        protected override void AcceptInvitation(Invitation invitation)
        {
            GameServices.RealTime.AcceptInvitation(invitation, showInvitationWaitingRoomUI, this);
        }

        protected override void DeclineInvitation(Invitation invitation)
        {
            GameServices.RealTime.DeclineInvitation(invitation);
        }

        public void AcceptFromInbox()
        {
            #if UNITY_IOS
            NativeUI.Alert("Unsupported Feature", "\"Accept From Inbox\" feature is not available on Game Center platform");
            #else
            GameServices.RealTime.ShowInvitationsUI(this);
            #endif
        }

        public void SendMessageToAll()
        {
            var data = GetSentData();
            if (data == null)
            {
                NativeUI.Alert("Error", "Error orcurs when creating sample data.");
                return;
            }

            GameServices.RealTime.SendMessageToAll(UseReliableMessage, data);
        }

        public void SendMessage()
        {
            if (TargetParticipant == null)
            {
                NativeUI.Alert("Error", "Please pick a target participant first.");
                return;
            }

            var data = GetSentData();
            if (data == null)
            {
                NativeUI.Alert("Error", "Error orcurs when creating sample data.");
                return;
            }

            GameServices.RealTime.SendMessage(UseReliableMessage, TargetParticipant.ParticipantId, data);
        }

        public void GetConnectedParticipants()
        {
            var connectedParticipants = GameServices.RealTime.GetConnectedParticipants();

            if (connectedParticipants == null || connectedParticipants.Count < 1)
            {
                NativeUI.Alert("Error", "The connected participants list is empty.");
                return;
            }

            var message = string.Join("\n\n", connectedParticipants.Select(p => GetParticipantDisplayString(p)).ToArray());
            NativeUI.Alert("Connected Participants", message);
        }

        public void GetSelf()
        {
            var self = GameServices.RealTime.GetSelf();
            NativeUI.Alert("Self Info", GetParticipantDisplayString(self));
        }

        public void LeaveRoom()
        {
            GameServices.RealTime.LeaveRoom();
        }

        public bool IsRoomConnected()
        {
            return GameServices.RealTime.IsRoomConnected();
        }

        public Participant GetParticipant(string participantId)
        {
            return GameServices.RealTime.GetParticipant(participantId);
        }

        #endregion

        #region IRealtimeMultiplayerListener implement

        public void OnLeftRoom()
        {
            if (IsDestroyed)
                return;

            ShowSetupUI();
            NativeUI.Alert("On Left Room", OnLeftRoomMessage);
            Debug.Log("[OnLeftRoom].");
        }

        public void OnParticipantLeft(Participant participant)
        {
            if (IsDestroyed)
                return;

            string message = OnParticipantLeftMessage + "\n" + GetParticipantDisplayString(participant);
            NativeUI.Alert("On Participant Left", message);
            Debug.Log("[OnParticipantLeft]. Participant: " + GetParticipantDisplayString(participant));
        }

        public void OnPeersConnected(string[] participantIds)
        {
            if (IsDestroyed)
                return;

            if (participantIds == null)
            {
                NativeUI.Alert("On Peers Connected", "Received a null \"participantIds\".");
                return;
            }

            var message = "Connected Participant(s):\n";
            for (int i = 0; i < participantIds.Length; i++)
            {
                var participant = GetParticipant(participantIds[i]);
                message += "\n" + GetParticipantDisplayString(participant);
            }

            NativeUI.Alert("On Peers Connected", message);
            Debug.Log("[OnPeersConnected]. " + message);

            UpdateTargetParticipantDrowdown();
        }

        public void OnPeersDisconnected(string[] participantIds)
        {
            if (IsDestroyed)
                return;

            if (participantIds == null)
            {
                NativeUI.Alert("On Peers Disconnected", "Received a null \"participantIds\".");
                return;
            }

            var message = "Disconnected Participant(s):\n";
            for (int i = 0; i < participantIds.Length; i++)
            {
                var participant = GetParticipant(participantIds[i]);

                if (participant != null)
                    message += "\n" + GetParticipantDisplayString(participant);
                else
                    message += "\nID: " + participantIds[i];
            }

            NativeUI.Alert("On Peers Disconnected", message);
            Debug.Log("[OnPeersDisconnected]. " + message);

            UpdateTargetParticipantDrowdown();
        }

        public void OnRealTimeMessageReceived(string senderId, byte[] data)
        {
            if (IsDestroyed)
                return;

            if (data == null)
            {
                AddReceivedMessage("[Error]. Received a realtime message with null data.");
                return;
            }

            if (IsMinimumSizeData(data))
            {
                AddReceivedMessage("SenderId: " + senderId + "\nData: minimal 4 bytes data [6, 6, 6, 6].");
                return;
            }

            try
            {
                var sampleData = SampleData.FromByteArray(data);
                AddReceivedMessage("SenderId: " + senderId + "\nData: " + (sampleData != null ? "\n" + sampleData.ToString() : "null"));
            }
            catch (Exception e)
            {
                AddReceivedMessage("[Error]. Error orcurs when trying to parse data: \n" + e.Message + "\n" + e.StackTrace);
            }
        }

        public void OnRoomConnected(bool success)
        {
            NativeUI.Alert("On Room Connected", "Result: " + success + "\n" + OnRoomConnectedMessage);
            Debug.Log("[OnRoomConnected]. Success: " + success);

            if (IsDestroyed)
                return;

            createQuickMatchRequest--;

            if (createQuickMatchRequest <= 0)
                StopCreateQuickMatchSpinningUI();

            if (success)
            {
                ShowInGameUI();
                UpdateTargetParticipantDrowdown();
            }
        }

        public void OnRoomSetupProgress(float percent)
        {
            if (IsDestroyed)
                return;

            Debug.Log("[OnRoomSetupProgress]. Percent: " + percent);
        }

        public bool ShouldReinviteDisconnectedPlayer(Participant participant)
        {
            return ShouldReInviteDisconnectedPlayer;
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

        public void ShowAcceptFromInboxHint()
        {
            NativeUI.Alert("Accept From Inbox", AcceptFromInboxHint);
        }

        public void ShowSendMessageHint()
        {
            NativeUI.Alert("Send Message", SendMessageHint);
        }

        public void ShowSendMessageToAllHint()
        {
            NativeUI.Alert("Send Message To All", SendMessageToAllHint);
        }

        public void ShowLeaveRoomHint()
        {
            NativeUI.Alert("Leave Room", LeaveRoomHint);
        }

        public void ShowSampleDataHint()
        {
            NativeUI.Alert("Sample Data", SampleDataHint);
        }

        public void ShowUseReliableMessageHint()
        {
            NativeUI.Alert("Use Reliable Message", UseReliableMessageHint);
        }

        public void ShowShouldReinviteDisconnectedPlayerHint()
        {
            NativeUI.Alert("Should Re-invite Disconnected Player", ShouldReinviteDisconnectedPlayerHint);
        }

        public void ShowReceivedMessagesHint()
        {
            NativeUI.Alert("Received Message", ReceivedMessagesHint);
        }

        #endregion

        #region Others

        private void InitButtons()
        {
            acceptFromInboxButton.onClick.AddListener(AcceptFromInbox);
            sendMessageToAllButton.onClick.AddListener(SendMessageToAll);
            sendMessageButton.onClick.AddListener(SendMessage);
            getConnectedParticipantsButton.onClick.AddListener(GetConnectedParticipants);
            showTargetParticipantButton.onClick.AddListener(ShowTargetParticipant);
            getSelfButton.onClick.AddListener(GetSelf);
            leaveRoomButton.onClick.AddListener(LeaveRoom);
            clearReceivedMessagesButton.onClick.AddListener(ClearReceivedMessagesText);
            receivedMessagesHintButton.onClick.AddListener(ShowReceivedMessagesHint);

            variantHintButton.onClick.AddListener(ShowVariantHint);
            bitmaskHintButton.onClick.AddListener(ShowExclusiveBitmaskHint);
            minPlayersHintButton.onClick.AddListener(ShowMinPlayersHint);
            maxPlayersHintButton.onClick.AddListener(ShowMaxPlayersHint);
            createQuickMatchHintButton.onClick.AddListener(ShowCreateQuickMatchHint);
            createWithMatchmakerUIHintButton.onClick.AddListener(ShowCreateWithmatchmakerHint);
            acceptFromInboxHintButton.onClick.AddListener(ShowAcceptFromInboxHint);
            sendMessageHintButton.onClick.AddListener(ShowSendMessageHint);
            sendMessageToAllHintButton.onClick.AddListener(ShowSendMessageToAllHint);
            leaveRoomHintButton.onClick.AddListener(ShowLeaveRoomHint);
            sampleDataHintButton.onClick.AddListener(ShowSampleDataHint);
            useReliableMessageHintButton.onClick.AddListener(ShowUseReliableMessageHint);
            reiniviteDisconnectedPlayerHintButton.onClick.AddListener(ShowShouldReinviteDisconnectedPlayerHint);
        }

        private void InitInputFields()
        {
            variantInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
            exclusiveBitmaskInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
            minPlayersInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
            maxPlayersInputField.keyboardType = TouchScreenKeyboardType.NumberPad;
            sampleDataValueInputField.keyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
            dummySizeInputField.keyboardType = TouchScreenKeyboardType.NumberPad;

            variantInputField.onEndEdit.AddListener(OnVariantInputChanged);
            exclusiveBitmaskInputField.onEndEdit.AddListener(OnExclusiveBitmaskInputChanged);
            minPlayersInputField.onEndEdit.AddListener(OnMinPlayersInputChanged);
            maxPlayersInputField.onEndEdit.AddListener(OnMaxPlayersInputChanged);

            sampleDataTextInputField.onEndEdit.AddListener(_ => RefreshFinalSizeText());
            sampleDataValueInputField.onEndEdit.AddListener(_ => RefreshFinalSizeText());
            dummySizeInputField.onEndEdit.AddListener(_ => RefreshFinalSizeText());

            MinPlayers = 2;
            MaxPlayers = 2;
            minPlayersInputField.text = "2";
            maxPlayersInputField.text = "2";
            exclusiveBitmaskInputField.text = "0";
            variantInputField.text = "0";
            sampleDataValueInputField.text = "0";
            sampleDataTextInputField.text = "Hello, world.";
            dummySizeInputField.text = "0";
        }

        private void ShowSetupUI()
        {
            if (isShowingIngameUI)
            {
                matchRequestRoot.SetActive(true);
                matchCreationRoot.SetActive(true);
                ingameRoot.SetActive(false);
                isShowingIngameUI = false;
            }
        }

        private void ShowInGameUI()
        {
            if (!isShowingIngameUI)
            {
                matchRequestRoot.SetActive(false);
                matchCreationRoot.SetActive(false);
                ingameRoot.SetActive(true);
                ClearReceivedMessagesText();
                isShowingIngameUI = true;
            }
        }

        private byte[] GetSentData()
        {
            if (useMinimalData)
                return new byte[] { 6, 6, 6, 6 };

            float value = 0;
            if (!float.TryParse(sampleDataValueInputField.text, out value))
                return null;

            var sampleData = new SampleData()
            {
                TimeStamp = DateTime.UtcNow,
                Text = sampleDataTextInputField.text,
                Value = value,
            };

            int dummySize = 0;
            if (int.TryParse(dummySizeInputField.text, out dummySize))
                sampleData.UpdateDummySize(dummySize);

            return sampleData.ToByteArray();
        }

        private void ClearTargetParticipantDropdown()
        {
            targetParticipantDropdown.ClearOptions();
            Opponents.Clear();
        }

        private void UpdateTargetParticipantDrowdown()
        {
            ClearTargetParticipantDropdown();

            var connectedParticipant = GameServices.RealTime.GetConnectedParticipants();
            if (connectedParticipant == null || connectedParticipant.Count < 1)
                return;

            var self = GameServices.RealTime.GetSelf();
            if (self == null)
                return;

            var options = new List<Dropdown.OptionData>();
            foreach (var participant in connectedParticipant)
            {
                if (self.Equals(participant))
                    continue;

                options.Add(new Dropdown.OptionData(participant.DisplayName));
                Opponents.Add(participant);
            }
            targetParticipantDropdown.AddOptions(options);
            targetParticipantDropdown.value = 0;
        }

        private void RefreshFinalSizeText()
        {
            var data = GetSentData();
            var size = data != null ? data.Length : 0;
            finalSizeText.text = "FINAL SIZE: " + size + "byte(s)";
        }

        private void ShowTargetParticipant()
        {
            var target = TargetParticipant;
            if (target == null)
            {
                NativeUI.Alert("Error", "There's no info to show.");
                return;
            }

            NativeUI.Alert("Target Participant", GetParticipantDisplayString(target));
        }

        private void ClearReceivedMessagesText()
        {
            ReceivedMessages.ForEach(rm => Destroy(rm.gameObject));
            ReceivedMessages.Clear();
        }

        private void AddReceivedMessage(string message)
        {
            var newMessage = Instantiate(receivedMessagesTextPrefab, receivedMessagesRoot.transform);
            newMessage.gameObject.SetActive(true);
            newMessage.text = message;
            ReceivedMessages.Add(newMessage);
            receivedMessagesVerticalScrollbar.value = 0f;
        }

        private void OnUseMinimalDataToggleValueChanged(bool value)
        {
            if (useMinimalData == value)
                return;

            useMinimalData = value;
            sampleDataTextRoot.SetActive(!useMinimalData);
            sampleDataValueRoot.SetActive(!useMinimalData);
            sampleDataDummySizeRoot.SetActive(!useMinimalData);

            if (useMinimalData)
                finalSizeText.text = "FINAL SIZE: 4 bytes";
            else
                RefreshFinalSizeText();
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

        private bool IsMinimumSizeData(byte[] bytes)
        {
            if (bytes == null)
                return false;

            return bytes.Length == 4 && bytes[0] == 6 && bytes[1] == 6 && bytes[2] == 6 && bytes[3] == 6;
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
