using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    public class Racing3DGameView : MonoBehaviour
    {
        [SerializeField]
        private Racing3DGameControl control = null;

        [SerializeField]
        private DemoUtils utilities = null;

        [SerializeField]
        private GameObject settingUIRootObject = null,
            ingameRootObject = null;

        [SerializeField]
        private Image nitroImage = null;

        [SerializeField]
        private Text ingamePrepareText = null;

        [SerializeField]
        private Button acceptFromInboxButton = null;

        [SerializeField]
        private GameObject nitroRoot = null,
            gameoverPanel = null,
            requestResponseRoot = null;

        [SerializeField]
        private Text gameoverTitle = null,
            gameoverSubtitle = null;

        [SerializeField]
        private Button gameoverDoneButton = null,
            rematchButton = null,
            acceptRematchButton = null,
            denyRematchButton = null;

        [SerializeField]
        private GameObject header = null;

        [Space]
        [SerializeField]
        private int maxCountDown = 3;

        [SerializeField]
        private float countDownTime = 5f;

        public bool IsWaitingUIShowing { get; private set; }

        public bool IsSettingUIShowing { get; private set; }

        public bool IsIngameShowing { get; private set; }

        private const string wonText = "<color=blue>YOU WON!!!</color>";
        private const string loseText = "<color=red>YOU LOSE!!!</color>";

        protected virtual void Start()
        {
            RegisterButtonsEvent();
            IsSettingUIShowing = true;
            gameoverPanel.SetActive(false);
            requestResponseRoot.SetActive(false);
            gameoverDoneButton.onClick.AddListener(control.LeaveRoom);
            rematchButton.onClick.AddListener(SendRematchRequest);
            acceptRematchButton.onClick.AddListener(() => SendRematchResponse(true));
            denyRematchButton.onClick.AddListener(() => SendRematchResponse(false));
        }

        protected virtual void Update()
        {
            if (control.ControllableCar != null)
                nitroImage.fillAmount = control.ControllableCar.Nitro / 50f;
        }

        public void ShowSettingUI()
        {
            header.SetActive(true);
            SetActiveSettingUI(true);
            SetActiveInGame(false);
        }

        public void ShowInGameUI()
        {
            nitroRoot.SetActive(false);
            gameoverPanel.SetActive(false);
            rematchButton.interactable = true;
            requestResponseRoot.SetActive(false);
            SetActiveSettingUI(false);
            SetActiveInGame(true);
            header.SetActive(true);
        }

        public void ShowPrepareText()
        {
            ingamePrepareText.gameObject.SetActive(true);
            ingamePrepareText.text = "Ready";
        }

        public void StartCounting(float delay = 0f, Action onFinish = null)
        {
            ingamePrepareText.gameObject.SetActive(true);
            StartCoroutine(CountingDownCoroutine(delay, onFinish));
        }

        public void SetActiveSettingUI(bool showFlag)
        {
            if (showFlag == IsSettingUIShowing)
                return;

            settingUIRootObject.SetActive(showFlag);
            IsSettingUIShowing = showFlag;
        }

        public void SetActiveInGame(bool showFlag)
        {
            if (showFlag == IsIngameShowing)
                return;

            ingameRootObject.SetActive(showFlag);
            IsIngameShowing = showFlag;
        }

        public void ShowGameOverUI(bool wonFlag, Racing3DGameModel.GameoverReason gameoverReason)
        {
            gameoverTitle.text = wonFlag ? wonText : loseText;
            gameoverSubtitle.text = GetGameoverSubtitle(gameoverReason);
            gameoverPanel.SetActive(true);
            header.SetActive(false);
            nitroRoot.SetActive(false);
        }

        public void SetActiveNitro(bool active)
        {
            if (active)
            {
                nitroRoot.SetActive(true);
            }
            else
            {
                nitroRoot.SetActive(false);
            }
        }

        /// <summary>
        /// Back button in the header.
        /// </summary>
        public void BackButton()
        {
            if (IsWaitingUIShowing || IsIngameShowing)
            {
                var popup = NativeUI.ShowTwoButtonAlert("Leave room", "Do you want to leave the room?", "Yes", "No");
                popup.OnComplete += button =>
                {
                    if (button == 0)
                    {
                        ShowSettingUI();
                        control.LeaveRoom();
                        control.ControllableCar.StopMoving();
                        control.OpponentCar.StopMoving();
                    }
                };
                return;
            }

            utilities.GameServiceDemo_Multiplayer();
        }

        /// <summary>
        /// Show UI to accept or deny rematch request.
        /// </summary>
        public void ShowRematchRequestedUI()
        {
            gameoverPanel.SetActive(false);
            rematchButton.interactable = false;
            requestResponseRoot.SetActive(true);
        }

        /// <summary>
        /// Show UI to let players know if their request accepted or denied.
        /// </summary>
        public void ShowRematchResponsedUI(bool accepted)
        {
            if (!accepted)
            {
                NativeUI.Alert("Request denied", "Your rematch request has been denied!!!");
                return;
            }

            Debug.Log("Starting rematch game...");
        }

        private void SendRematchRequest()
        {
            rematchButton.interactable = false;

            if (control.IsOpponentDisconnected)
                NativeUI.Alert("Rematch Failed", "Your opponent has left the game.");
            else
                control.SendRematchRequestMessage();
        }

        private void SendRematchResponse(bool accepted)
        {
            control.SendRematchResponseMessage(accepted);
            gameoverPanel.SetActive(true);
            rematchButton.interactable = false;
            requestResponseRoot.SetActive(false);

            if (accepted)
                control.StartGame();
        }

        private IEnumerator CountingDownCoroutine(float delay = 0f, Action onFinish = null)
        {
            if (delay >= countDownTime)
            {
                if (onFinish != null)
                    onFinish();

                ingamePrepareText.gameObject.SetActive(false);
                yield break;
            }

            float totalTime = countDownTime - delay;
            float waitTime = totalTime / (maxCountDown + 1);
            for (int i = maxCountDown; i > 0; i--)
            {
                ingamePrepareText.text = i.ToString();
                yield return new WaitForSeconds(waitTime);
            }

            ingamePrepareText.text = "Start";
            yield return new WaitForSeconds(waitTime);

            ingamePrepareText.gameObject.SetActive(false);
            if (onFinish != null)
                onFinish();
        }

        private void RegisterButtonsEvent()
        {
            acceptFromInboxButton.onClick.AddListener(control.AcceptFromInbox);
        }

        private string GetGameoverSubtitle(Racing3DGameModel.GameoverReason gameoverReason)
        {
            switch (gameoverReason)
            {
                case Racing3DGameModel.GameoverReason.FinishRace:
                    return "You've finished the race";

                case Racing3DGameModel.GameoverReason.OpponentFinishRace:
                    return "Your opponent has finished the race sooner than you";

                case Racing3DGameModel.GameoverReason.HitPowerUp:
                    return "You hit an power up";

                case Racing3DGameModel.GameoverReason.OpponentHitPowerUp:
                    return "Your opponent hit an power up";

                case Racing3DGameModel.GameoverReason.OpponentLeftRoom:
                    return "Your opponent had left the room";

                case Racing3DGameModel.GameoverReason.None:
                default:
                    return "Unknown";
            }
        }
    }
}
