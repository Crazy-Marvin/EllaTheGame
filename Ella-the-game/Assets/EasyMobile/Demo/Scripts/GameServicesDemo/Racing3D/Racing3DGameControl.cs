using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

namespace EasyMobile.Demo
{
    public class Racing3DGameControl : GameServicesDemo_Multiplayer_BaseControl, IRealTimeMultiplayerListener
    {
        #region Fields & Properties 

        [SerializeField]
        private Racing3DGameView view = null;

        [SerializeField]
        private Racing3DCameraControl cameraControl = null;

        [SerializeField]
        private Racing3DCarControl hostCar = null, guestCar = null;

        [SerializeField]
        private Transform roadTransform = null;

        [SerializeField]
        private GameObject powerUpPrefab = null;

        [SerializeField]
        private GameObject[] sideObjects = null;

        [SerializeField]
        private Texture2D anonymousAvatar = null;

        #if UNITY_ANDROID
        [SerializeField]
        private float readyMessageDelay = -0.5f;
        #else
        private float readyMessageDelay = 0f;
        #endif

        [Header("Send Start Message")]
        [SerializeField]
        private float startMessageInterval = 10f;

        [SerializeField]
        private int startMessageLimit = 5;

        [Header("PowerUps")]
        [SerializeField]
        private string powerUpName = "Nitro";

        [SerializeField]
        [Range(0f, 1f)]
        private float powerUpsFrequency = 0.5f;

        [SerializeField]
        private float powerUpsZ = -0.5f;

        [SerializeField]
        private Vector2 powerUpsYRange = Vector2.one, powerUpsDistanceRange = Vector2.one;

        [SerializeField]
        private List<float> powerUpsX = new List<float>();

        [Header("Side Objects")]
        [SerializeField]
        private Vector2 sideObjectsYRange = Vector2.one, sideObjectsDistanceRange = Vector2.one;

        [SerializeField]
        private float sideObjectMaxDeltaX = 0.5f;

        [SerializeField]
        private float sideObjectMaxDeltaY = 1f;

        [SerializeField]
        private List<float> sideObjectsX = new List<float>();

        [Header("Reliable Settings")]
        [SerializeField]
        private bool move = true;

        [SerializeField]
        private bool hitPowerUp = true,
            useNitro = true,
            finish = true, 
            start = true, 
            ready = true, 
            rematchRequest = true,
            rematchResponse = true;

        public Racing3DCarControl ControllableCar
        {
            get
            {
                if (model == null)
                    return null;

                return model.IsHost ? hostCar : guestCar;
            }
        }

        public Racing3DCarControl OpponentCar
        {
            get
            {
                if (model == null)
                    return null;

                return model.IsHost ? guestCar : hostCar;
            }
        }

        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                if (isPlaying == value)
                    return;

                isPlaying = value;
                ShouldShowInvitationPanel = !isPlaying;
            }
        }

        public bool IsRoomConnected
        {
            get { return GameServices.RealTime.IsRoomConnected(); }
        }

        public bool IsOpponentDisconnected { get; private set; }
        protected override MatchType MatchType { get { return MatchType.RealTime; } }

        private MatchRequest matchRequest = new MatchRequest() { MinPlayers = 2, MaxPlayers = 2 };
        private Racing3DGameModel model;
        private string selfId, opponentId;
        private List<GameObject> createdPowerUps = new List<GameObject>();
        private List<GameObject> createdSideObjects = new List<GameObject>();
        private bool isPlaying = false, isStartMessageReceived = false;
        private Stopwatch stopwatch = new Stopwatch();

        #endregion

        #region GameServices.Realtime APIs

        protected override void CreateQuickMatch()
        {
            GameServices.RealTime.CreateQuickMatch(matchRequest, this);
            createQuickMatchRequest++;
            StartCreateQuickMatchSpinningUI();
        }

        protected override void CreateWithMatchmaker()
        {
            GameServices.RealTime.CreateWithMatchmakerUI(matchRequest, this);
        }

        protected override void AcceptInvitation(Invitation invitation)
        {
            GameServices.RealTime.AcceptInvitation(invitation, true, this);
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

        public void LeaveRoom()
        {
            GameServices.RealTime.LeaveRoom();
        }

        #endregion

        #region Send Messages

        public void SendMoveMessage(Racing3DGameModel.MoveDirections direction)
        {
            if (model == null || opponentId == null)
                return;

            GameServices.RealTime.SendMessage(move, opponentId, model.CreateMoveMessage(direction));
        }

        public void SendHitPowerUpMessage()
        {
            if (model == null || opponentId == null)
                return;

            GameServices.RealTime.SendMessage(hitPowerUp, opponentId, model.CreateHitPowerUpMessage());
        }

        public void SendUseNitroMessage()
        {
            if (model == null || opponentId == null)
                return;

            GameServices.RealTime.SendMessage(useNitro, opponentId, model.CreateUseNitroMessage());
        }

        public void SendFinishMessage()
        {
            if (model == null || opponentId == null)
                return;

            GameServices.RealTime.SendMessage(finish, opponentId, model.CreateFinishRaceMessage());
        }

        private void SendStartGameMessage()
        {
            if (model == null || opponentId == null || model.PowerUpsPosition == null || model.SideObjectsPosition == null)
                return;

            Debug.Log("[SendStartGameMessage]");
            GameServices.RealTime.SendMessage(start, opponentId, model.CreateStartGameMessage());
        }

        public void SendReadyMessage()
        {
            if (model == null || opponentId == null)
                return;

            Debug.Log("[SendReadyMessage]");
            GameServices.RealTime.SendMessage(ready, opponentId, model.CreateReadyMessage());
        }

        public void SendRematchRequestMessage()
        {
            if (model == null || opponentId == null)
                return;

            GameServices.RealTime.SendMessage(rematchRequest, opponentId, model.CreateRematchRequestMessage());
        }

        public void SendRematchResponseMessage(bool accepted)
        {
            if (model == null || opponentId == null)
                return;

            GameServices.RealTime.SendMessage(rematchResponse, opponentId, model.CreateRematchReponseMessage(accepted));
        }

        #endregion

        #region Core

        public void StartGame()
        {
            Application.targetFrameRate = 60;
            var self = GameServices.RealTime.GetSelf();
            selfId = self.ParticipantId;
            var opponent = GameServices.RealTime.GetConnectedParticipants().Where(p => !p.ParticipantId.Equals(selfId)).FirstOrDefault();
            opponentId = opponent.ParticipantId;
            model = new Racing3DGameModel(selfId, opponentId);
            IsPlaying = true;

            ClearOldPowerUps();
            ClearSideObjects();

            if (model.IsHost)
            {
                var powerUpsPosition = GenerateRandomPositions();
                InstantiatePowerUps(powerUpsPosition);
                model.PowerUpsPosition = powerUpsPosition;

                var sideObjectsPosition = GenerateRandomSidePositions();
                InstantiateSideObjects(sideObjectsPosition);
                model.SideObjectsPosition = sideObjectsPosition;

                isStartMessageReceived = false;
                Internal.RuntimeHelper.RunOnMainThread(() => StartCoroutine(SendStartMessageCoroutine()));
            }

            ControllableCar.ResetValues();
            ControllableCar.Controllable = true;
            OpponentCar.ResetValues();
            OpponentCar.Controllable = false;

            cameraControl.transform.position = ControllableCar.PrepareCameraPosition;
            view.ShowPrepareText();
            view.ShowInGameUI();
            LoadInfos(self, opponent);
        }

        private void OnDataReceived(string senderId, byte[] data)
        {
            var message = model.FromByteArray(data);
            if (message == null)
            {
                Debug.LogError("Failed to parse data!!!");
                return;
            }

            Debug.Log("[OnDataReceived] Type: " + message.Type);

            /// Opponent sent ready message, this also means you're a host...
            if (message.Type == Racing3DGameModel.MessageTypes.Ready)
            {
                isStartMessageReceived = true;

                if (!model.IsHost)
                    return;
                    
                stopwatch.Stop();
                var delay = stopwatch.Elapsed.TotalSeconds;
                view.StartCounting((float)delay + readyMessageDelay, StartMoving);
                Debug.Log("Stopwatch's delay: " + delay);
                stopwatch.Reset();
                return;
            }

            /// Opponent sent start game message, this also means you're a guest...
            if (message.Type == Racing3DGameModel.MessageTypes.StartGame)
            {
                var startMessage = message as Racing3DGameModel.StartGameMessage;

                if (!model.IsHost)
                {
                    InstantiatePowerUps(startMessage.PowerUpsPosition);
                    InstantiateSideObjects(startMessage.SideObjectsPosition);
                    SendReadyMessage();
                }

                view.StartCounting(onFinish: StartMoving);
                return;
            }

            /// Opponent moved...
            if (message.Type == Racing3DGameModel.MessageTypes.Move)
            {
                var moveMessage = message as Racing3DGameModel.MoveMessage;
                OpponentCar.Move(moveMessage.Direction);
                return;
            }

            /// Opponent start using nitro...
            if (message.Type == Racing3DGameModel.MessageTypes.UseNitro)
            {
                OpponentCar.UseNitro();
                return;
            }

            /// Opponent hit an powerUp...
            if (message.Type == Racing3DGameModel.MessageTypes.HitPowerUp)
            {
                OpponentCar.CreateHitPowerUpEffect();
                StopMoving(true, Racing3DGameModel.GameoverReason.OpponentHitPowerUp, IsPlaying);
                IsPlaying = false;
                return;
            }

            /// Opponent has finished the race...
            if (message.Type == Racing3DGameModel.MessageTypes.FinishRace)
            {
                StopMoving(false, Racing3DGameModel.GameoverReason.OpponentFinishRace, IsPlaying);
                IsPlaying = false;
                return;
            }

            /// Opponent request a rematch...
            if (message.Type == Racing3DGameModel.MessageTypes.RematchRequest)
            {
                view.ShowRematchRequestedUI();
                return;
            }

            /// Opponent accepted or denied the rematch...
            if (message.Type == Racing3DGameModel.MessageTypes.RematchResponse)
            {
                var response = message as Racing3DGameModel.RematchResponseMaessage;
                view.ShowRematchResponsedUI(response.Accepted);

                if (response.Accepted)
                {
                    StartGame();
                }

                return;
            }
        }

        #endregion

        #region Others

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (GameServices.IsInitialized())
                LeaveRoom();
        }

        private void LoadInfos(Participant self, Participant opponent)
        {
            StartCoroutine(LoadUserInfosCoroutine(ControllableCar, self));
            StartCoroutine(LoadUserInfosCoroutine(OpponentCar, opponent));
        }

        public void UseNitro()
        {
            if (ControllableCar == null)
                return;

            ControllableCar.UseNitro();
        }

        private IEnumerator LoadUserInfosCoroutine(Racing3DCarControl car, Participant participant)
        {
            if (participant == null)
                yield break;

            car.UpdateName(participant.DisplayName);

            if (participant.Player == null)
            {
                car.UpdateAvatar(anonymousAvatar);
                yield break;
            }

            if (participant.Player.image != null)
            {
                car.UpdateAvatar(participant.Player.image);
                yield break;
            }

            yield return new WaitUntil(() => participant.Player.image != null);
            car.UpdateAvatar(participant.Player.image);
        }

        private void ClearOldPowerUps()
        {
            foreach (var powerUp in createdPowerUps)
                Destroy(powerUp);
            createdPowerUps.Clear();
        }

        private void ClearSideObjects()
        {
            foreach (var sObject in createdSideObjects)
                Destroy(sObject);
            createdSideObjects.Clear();
        }

        private void InstantiatePowerUps(List<Vector3> positions)
        {
            foreach (var position in positions)
            {
                var powerUp = Instantiate(powerUpPrefab, position, Quaternion.identity, roadTransform);
                powerUp.name = powerUpName;
                createdPowerUps.Add(powerUp);
            }
        }

        private void InstantiateSideObjects(List<Vector3> positions)
        {
            foreach (var position in positions)
            {
                float randX = Random.Range(-sideObjectMaxDeltaX, sideObjectMaxDeltaX);
                float randY = Random.Range(-sideObjectMaxDeltaY, sideObjectMaxDeltaY);
                int i = Random.Range(0, sideObjects.Length);
                var sObject = Instantiate(sideObjects[i], position + new Vector3(randX, randY, 0), Quaternion.identity, roadTransform);
                createdSideObjects.Add(sObject);
            }
        }

        private List<Vector3> GenerateRandomPositions()
        {
            List<Vector3> generatedPosition = new List<Vector3>();

            float currentY = powerUpsYRange.x;
            while (currentY < (powerUpsYRange.y - powerUpsDistanceRange.y))
            {
                int newPowerUpsCount = Random.Range(1, powerUpsX.Count);
                List<float> currentXs = powerUpsX.OrderBy(_ => Random.value).Take(newPowerUpsCount).ToList();
                float yDistance = Random.Range(powerUpsDistanceRange.x, powerUpsDistanceRange.y);
                currentY += yDistance;

                foreach (float x in currentXs)
                {
                    float randNum = Random.Range(0, 1f);
                    if (powerUpsFrequency >= randNum)
                    {
                        generatedPosition.Add(new Vector3(x, currentY, powerUpsZ));
                        break;
                    }
                }
            }

            return generatedPosition;
        }

        private List<Vector3> GenerateRandomSidePositions()
        {
            List<Vector3> generatedPosition = new List<Vector3>();

            float currentY = sideObjectsYRange.x;
            while (currentY < (sideObjectsYRange.y - sideObjectsDistanceRange.y))
            {
                int newObjectsCount = Random.Range(1, powerUpsX.Count);
                List<float> currentXs = sideObjectsX.OrderBy(_ => Random.value).Take(newObjectsCount).ToList();
                float yDistance = Random.Range(sideObjectsDistanceRange.x, sideObjectsDistanceRange.y);
                currentY += yDistance;

                currentXs.ForEach(x =>
                    {
                        float rand = Random.Range(0, 1f);
                        if (rand < 0.85f)
                            generatedPosition.Add(new Vector3(x, currentY, powerUpsZ));
                    });
            }

            return generatedPosition;
        }

        private void StopMoving(bool wonFlag, Racing3DGameModel.GameoverReason stopReason, bool showGameOverUI = true)
        {
            OpponentCar.StopMoving();
            ControllableCar.StopMoving();

            if (showGameOverUI)
                view.ShowGameOverUI(wonFlag, stopReason);
        }

        private void StartMoving()
        {
            ControllableCar.StartMoving(true);
            OpponentCar.StartMoving(false);
            cameraControl.StartFollowing(ControllableCar.gameObject);
        }

        private Racing3DCarControl GetCarWithId(string id)
        {
            return id == selfId ? ControllableCar : OpponentCar;
        }

        private IEnumerator SendStartMessageCoroutine()
        {
            int count = 0;
            while(!isStartMessageReceived)
            {
                if (count < startMessageLimit)
                {
                    stopwatch.Reset();
                    stopwatch.Start();

                    SendStartGameMessage();
                    count++;

                    yield return new WaitForSeconds(startMessageInterval);
                }
                else
                {
                    string title = "No Reponse";
                    string message = "There is no reponse from your opponent. You can click the \"Back\" button in the header to leave the match.";
                    NativeUI.Alert(title, message);

                    yield break;
                }
            }
        }

        #endregion

        #region IRealTimeMultiplayerListener implement

        public void OnLeftRoom()
        {
            if (IsDestroyed)
                return;

            ClearOldPowerUps();
            ClearSideObjects();
            view.ShowSettingUI();
            Debug.Log("[OnLeftRoom]");
        }

        public void OnParticipantLeft(Participant participant)
        {
            Debug.Log("[OnParticipantLeft]. Participant: " + participant);
        }

        public void OnPeersConnected(string[] participantIds)
        {
            if (IsDestroyed)
                return;

            if (participantIds == null)
            {
                Debug.Log("[OnPeersConnected] Null ParticipantIds.");
                return;
            }

            foreach (var id in participantIds)
            {
                if (isPlaying)
                {
                    var car = GetCarWithId(id);
                    if (car != null)
                        car.ContinueMoving();
                }

                if (model != null && id == model.OpponentId)
                    IsOpponentDisconnected = false;
            }

            Debug.Log("[OnPeersConnected]. Ids: " + string.Join(",", participantIds));
        }

        public void OnPeersDisconnected(string[] participantIds)
        {
            if (IsDestroyed)
                return;

            if (participantIds == null)
            {
                Debug.Log("[OnPeersDisconnected] Null ParticipantIds.");
                return;
            }

            foreach (var id in participantIds)
            {
                if (isPlaying)
                {
                    var car = GetCarWithId(id);
                    if (car != null)
                        car.StopMoving();
                }

                if (model != null && id == model.OpponentId)
                    IsOpponentDisconnected = true;
            }

            Debug.Log("[OnPeersDisconnected]. Ids: " + string.Join(",", participantIds));
        }

        public void OnRealTimeMessageReceived(string senderId, byte[] data)
        {
            if (IsDestroyed)
                return;

            OnDataReceived(senderId, data);
        }

        public void OnRoomConnected(bool success)
        {
            Debug.Log("[OnRoomConnected]. Success: " + success);

            if (IsDestroyed)
            {
                Debug.Log("[OnRoomConnected] Stop because IsDestroyed is true");
                return;
            }

            createQuickMatchRequest--;
            if (createQuickMatchRequest <= 0)
                StopCreateQuickMatchSpinningUI();

            if (!success)
            {
                NativeUI.Alert("Error", "Room connection failed.");
                LeaveRoom();
                return;
            }

            StartGame();
        }

        public void OnRoomSetupProgress(float percent)
        {
            Debug.Log("[OnRoomSetupProgress]. Percent: " + percent);
        }

        public bool ShouldReinviteDisconnectedPlayer(Participant participant)
        {
            return false;
        }

        #endregion
    }
}
