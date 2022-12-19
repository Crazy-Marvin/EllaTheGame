using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    /// <summary>
    /// Base class for all control classes in multiplayer demos.
    /// </summary>
    public abstract class GameServicesDemo_Multiplayer_BaseControl : MonoBehaviour
    {
        [SerializeField]
        private LoadingPanelController loadingPanel = null;

        [SerializeField]
        protected DemoUtils demoUtils = null;

        [SerializeField]
        private Button createQuickMatchButton = null,
            createWithMatchmakerButton = null,
            acceptInvitationButton = null,
            declineInvitationButton = null,
            closeInvitationPanelButton = null,
            showInvitationDetailsButton = null;

        [SerializeField]
        private Text invitationInfoText = null;

        [SerializeField]
        private GameObject invitationPanel = null;

        [SerializeField]
        private Image createQuickMatchSpinningCircle = null;

        public bool IsInvitationDelegateRegistered { get; private set; }
        protected bool ShouldShowInvitationPanel { get; set; }
        protected bool IsDestroyed { get; private set; }

        private Queue<Invitation> unhandledInvitations = new Queue<Invitation>();
        private Invitation currentInvitation = null;
        private bool isInvitationPanelShowing = false;
        private float showNewInvitationDelayTime = 2f;

        protected int createQuickMatchRequest = 0;
        private bool isCreateQuickMatchSpinningUIShowing = false;
        private Coroutine createQuickMatchSpinningCoroutine = null;
        private float createQuickMatchSpinningAngle = 10, createQuickMatchSpinningFillSpeed = 0.002f;

        protected abstract void CreateQuickMatch();
        protected abstract void CreateWithMatchmaker();
        protected abstract void AcceptInvitation(Invitation invitation);
        protected abstract void DeclineInvitation(Invitation invitation);
        protected abstract MatchType MatchType { get; }

        protected virtual void Awake()
        {
            createQuickMatchButton.onClick.AddListener(CreateQuickMatch);
            createWithMatchmakerButton.onClick.AddListener(CreateWithMatchmaker);

            acceptInvitationButton.onClick.AddListener(AcceptCurrentInvitation);
            declineInvitationButton.onClick.AddListener(DeclineCurrentInvitation);
            closeInvitationPanelButton.onClick.AddListener(CloseInvitationPanel);
            showInvitationDetailsButton.onClick.AddListener(ShowCurrentInvitationDetails);

            ShouldShowInvitationPanel = true;
            invitationPanel.SetActive(false);
            createQuickMatchSpinningCircle.gameObject.SetActive(false);
        }

        protected IEnumerator Start()
        {
            RuntimeManager.Init();

            // Wait until user logged in.
            while (!GameServices.IsInitialized() || Application.isEditor)
            {
                if (!loadingPanel.IsShowing)
                {
                    loadingPanel.SetMessageText(Application.isEditor ? "Please test on a real mobile device." : "Wait until authenticated or exit.");
                    loadingPanel.SetButtonLabel("Exit");
                    loadingPanel.RegisterButtonCallback(() => demoUtils.GameServiceDemo_Multiplayer());
                    loadingPanel.Show();
                }
                yield return null;
            }

            loadingPanel.Hide();
            RegisterInvitationDelegate();
            LateStart();
        }

        protected virtual void OnDestroy()
        {
            IsDestroyed = true;
            if (IsInvitationDelegateRegistered && GameServices.IsInitialized())
                GameServices.RegisterInvitationDelegate(null);
        }

        /// <summary>
        /// Will be called after the user has logged in the invitation delegate is registered.
        /// Use this instead of Start.
        /// </summary>
        protected virtual void LateStart() { }

        protected void OnInvitationReceived(Invitation invitation, bool shouldAutoAccept)
        {
            Debug.Log("[OnInvitationReceived].\n" + GetInvitationDisplayString(invitation) + "shouldAutoAccept: " + shouldAutoAccept);

            if (IsDestroyed)
                return;

            if (invitation == null)
            {
                NativeUI.Alert("Error", "Received a null invitation!!!");
                return;
            }

            if (shouldAutoAccept)
            {
                AcceptInvitation(invitation);
                return;
            }

            ShowInvitationPanel(invitation);
        }

        private void ShowInvitationPanel(Invitation invitation)
        {
            if (invitation.InvitationType != MatchType)
                return;

            if (isInvitationPanelShowing)
            {
                unhandledInvitations.Enqueue(invitation);
                return;
            }

            currentInvitation = invitation;
            isInvitationPanelShowing = true;
            StartCoroutine(ShowInvitationCoroutine(invitation));
        }

        protected void CloseInvitationPanel()
        {
            if (isInvitationPanelShowing)
            {
                invitationPanel.SetActive(false);
                isInvitationPanelShowing = false;
                StartCoroutine(ShowNextInvitationCoroutine());
            }
        }

        private void AcceptCurrentInvitation()
        {
            if (currentInvitation == null)
                return;

            AcceptInvitation(currentInvitation);
            CloseInvitationPanel();
        }

        private void DeclineCurrentInvitation()
        {
            if (currentInvitation == null)
                return;

            DeclineInvitation(currentInvitation);
            CloseInvitationPanel();
        }

        private void ShowCurrentInvitationDetails()
        {
            if (currentInvitation == null)
                return;

            NativeUI.Alert("Invitation", GetInvitationDisplayString(currentInvitation));
        }

        private IEnumerator ShowNextInvitationCoroutine()
        {
            yield return new WaitForSeconds(showNewInvitationDelayTime);
            if (unhandledInvitations.Count > 0)
            {
                var newInvitation = unhandledInvitations.Dequeue();
                ShowInvitationPanel(newInvitation);
            }
        }

        private IEnumerator ShowInvitationCoroutine(Invitation invitation)
        {
            yield return new WaitUntil(() => ShouldShowInvitationPanel);
            invitationInfoText.text = "<b>Inviter:</b>\n" + (invitation.Inviter.DisplayName);
            invitationPanel.SetActive(true);
        }

        private void RegisterInvitationDelegate()
        {
            GameServices.RegisterInvitationDelegate(OnInvitationReceived);
            IsInvitationDelegateRegistered = true;
        }

        public string GetInvitationDisplayString(Invitation invitation)
        {
            if (invitation == null)
                return "null\n";

            string result = "[Invitation]\n";
            result += "InvitationType: " + invitation.InvitationType + "\n";
            result += "Variant: " + invitation.Variant + "\n";
            result += "Inviter: " + GetParticipantDisplayString(invitation.Inviter);

            return result;
        }

        public string GetParticipantDisplayString(Participant participant)
        {
            if (participant == null)
                return "null\n";

            string result = "[Participant]\n";
            result += "DisplayName: " + participant.DisplayName + "\n";
            result += "IsConnectedToRoom: " + participant.IsConnectedToRoom + "\n";
            result += "ParticipantId: " + participant.ParticipantId + "\n";
            result += "Status: " + participant.Status + "\n";

            if (participant.Player == null)
            {
                result += "Player: null\n";
            }
            else
            {
                result += "Player.id: " + participant.Player.id + "\n";
                result += "Player.isFriend: " + participant.Player.isFriend + "\n";
                result += "Player.state: " + participant.Player.state + "\n";
                result += "Player.userName: " + participant.Player.userName + "\n";
            }

            return result;
        }

        public string GetTurnbasedMatchDisplayString(TurnBasedMatch match)
        {
            if (match == null)
                return "null\n";

            string result = "[TurnBasedMatch]\n";
            result += "MatchId: " + (match.MatchId ?? "null") + "\n";
            result += "Status: " + match.Status + "\n";
            result += "CurrentParticipantId: " + (match.CurrentParticipantId ?? "null") + "\n";
            result += "SelfParticipantId: " + (match.SelfParticipantId ?? "null") + "\n";
            result += "PlayerCount: " + match.PlayerCount + "\n";
            result += "IsMyTurn: " + match.IsMyTurn + "\n";

            bool hasData = (match.Data != null && match.Data.Length > 0);
            result += "Has Data: " + hasData + "\n";

            if (hasData)
                result += "Data size: " + (match.Data != null ? match.Data.Length : 0) + "\n";

            return result;
        }

        public virtual void StartCreateQuickMatchSpinningUI()
        {
            if (isCreateQuickMatchSpinningUIShowing)
                return;

            createQuickMatchSpinningCircle.gameObject.SetActive(true);
            createQuickMatchSpinningCoroutine = StartCoroutine(CreateQuickMatchSpinningCoroutine());
            isCreateQuickMatchSpinningUIShowing = true;
        }

        public virtual void StopCreateQuickMatchSpinningUI()
        {
            if (!isCreateQuickMatchSpinningUIShowing || createQuickMatchSpinningCoroutine == null)
                return;

            StopCoroutine(createQuickMatchSpinningCoroutine);
            isCreateQuickMatchSpinningUIShowing = false;
            createQuickMatchSpinningCircle.gameObject.SetActive(false);
        }

        protected IEnumerator CreateQuickMatchSpinningCoroutine()
        {
            float fillAmount = 0f;
            while (true)
            {
                fillAmount += createQuickMatchSpinningFillSpeed;
                createQuickMatchSpinningCircle.fillAmount = Mathf.Repeat(fillAmount, 1);
                createQuickMatchSpinningCircle.rectTransform.Rotate(0, 0, createQuickMatchSpinningAngle);
                yield return null;
            }
        }
    }
}
