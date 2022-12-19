using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    public class TicTacToeGameView : MonoBehaviour
    {
        #region Inner classes

        [Serializable]
        private struct PlayerInfosUI
        {
            private bool highLight;

            public static readonly Color highlightColor = new Color(255, 0, 0, 150),
                                         normalColor = new Color(0, 0, 0, 150);

            #pragma warning disable 0414 // suppress value not used warning
            #pragma warning disable 0649 // suppress value never asigned warning
            public RawImage avatar;
            public Image rootPanel;
            public Image mark;
            public Text name;
            #pragma warning disable 0649 // restore value never asigned warning
            #pragma warning disable 0414 // restore value not used warning

            public bool HighLight
            {
                get
                {
                    return highLight;
                }
                set
                {
                    if (highLight == value)
                        return;

                    highLight = value;

                    rootPanel.color = value ? highlightColor : normalColor;
                    name.color = value ? highlightColor : normalColor;
                }
            }

            public void Update(Texture2D avatar, Sprite mark, Color markColor, string name, bool hightLight, bool anonymous = false)
            {
                this.mark.sprite = mark;
                this.avatar.texture = avatar;
                this.mark.color = markColor;
                this.name.text = anonymous ? ("[anonymous] " + name) : name;
                HighLight = hightLight;
            }
        }

        #endregion

        #region Fields & Properties

        [SerializeField]
        private Color xColor = Color.white, oColor = Color.black;

        [SerializeField, Range(0, 1)]
        private float spaceRate = 0.1f;

        [SerializeField, Range(1, 100)]
        private float progressUISpeed = 1f, progressUIDisappearTime = 3f;

        [Header("References")]
        [SerializeField]
        private TicTacToeGameControl control = null;

        [SerializeField]
        private DemoUtils demoUtils = null;

        [SerializeField]
        private AlertPopup popup;

        [SerializeField]
        private GameObject gameSection = null,
            multiplayerSection = null,
            progressTextObject = null,
            resultRootObject = null;

        [SerializeField]
        private Text resultText = null;

        [SerializeField]
        private GridLayoutGroup grid = null;

        [SerializeField]
        private RectTransform gridRect = null;

        [SerializeField]
        private Button showMatchesUIButton = null,
                       hideGameButton = null,
                       leaveMatchButton = null,
                       rematchButton = null;

        [SerializeField]
        private Button tilePrefab = null;

        [SerializeField]
        private Text progressText = null;

        [SerializeField]
        private Dropdown gameSizeSelector = null;

        [SerializeField]
        private Sprite blankImage = null, xImage = null, oImage = null;

        [SerializeField]
        private PlayerInfosUI selfInfos, opponentInfos;

        [SerializeField]
        private Texture2D defaulAvatar = null;

        public float TileSize { get; private set; }

        public float TileSpacing
        {
            get
            {
                return TileSize * spaceRate;
            }
        }

        public int BoardSize
        {
            get
            {
                if (gameSizeSelector == null)
                    return 0;

                return gameSizeSelector.value == 0 ? 3 : 5;
            }
        }

        public bool IsInPlayingMode { get; private set; }

        private Button[][] board;
        private TicTacToeGameModel currentModel;
        private Coroutine progressUIRunningCoroutine = null, progressUIStoppingCoroutine = null;
        private string wonText = "<color=green>You won!!!</color>",
            loseText = "<color=red>You lose!!!</color>",
            tiedText = "<color=blue>Tied!!!</color>";

        #endregion

        #region MonoBehaviours

        protected virtual void Start()
        {
            RegisterButtonsEvents();

            gameSizeSelector.ClearOptions();
            gameSizeSelector.AddOptions(new List<Dropdown.OptionData>()
            {
                new Dropdown.OptionData("3 x 3"),
                new Dropdown.OptionData("5 x 5"),
            });
        }

        protected virtual void Update()
        {
            if (currentModel == null)
                return;

            selfInfos.Update(currentModel.Match.Self.Player.image,
                             GetSpriteForTile(currentModel.LocalPlayerMark.ToTileState()),
                             GetColorForTile(currentModel.LocalPlayerMark.ToTileState()),
                             currentModel.Match.Self.DisplayName,
                             control.CanMove);

            opponentInfos.Update((currentModel.Opponent != null && currentModel.Opponent.Player != null) ? currentModel.Opponent.Player.image : defaulAvatar,
                                  GetSpriteForTile(currentModel.OpponentMark.ToTileState()),
                                  GetColorForTile(currentModel.OpponentMark.ToTileState()),
                                  currentModel.Opponent != null ? currentModel.Opponent.DisplayName : "Unmatched player",
                                  !control.CanMove,
                                  currentModel.Opponent != null && currentModel.Opponent.Player == null);
        }

        #endregion

        #region Public Methods

        public void CreateBoard(TicTacToeGameModel model)
        {
            if (model == null)
                return;

            currentModel = model;
            IsInPlayingMode = true;
            gameSection.SetActive(true);
            multiplayerSection.SetActive(false);
            progressTextObject.SetActive(false);
            rematchButton.gameObject.SetActive(false);
            leaveMatchButton.gameObject.SetActive(true);
            resultRootObject.SetActive(false);

            InstantiateBoard(model);
        }

        public void Move(int x, int y)
        {
            control.Move(x, y, mark =>
            {
                UpdateTileUI(x, y, mark.ToTileState());
            });
        }

        public void EndGame(MatchOutcome.ParticipantResult result)
        {
            ShowGameOverUI(result, true);

            foreach (var array in board)
                foreach (var button in array)
                    button.interactable = false;
        }

        public void LeaveMatch()
        {
            control.LeaveMatch();
        }

        public void ShowGameOverUI(MatchOutcome.ParticipantResult result, bool showRematchButton)
        {
            leaveMatchButton.gameObject.SetActive(false);
            IsInPlayingMode = false;
            resultRootObject.SetActive(true);
            resultText.text = GetResultText(result);
            rematchButton.gameObject.SetActive(showRematchButton);

            var tempModel = currentModel;
            currentModel = null;

            selfInfos.Update(tempModel.Match.Self.Player.image,
                 GetSpriteForTile(tempModel.LocalPlayerMark.ToTileState()),
                 GetColorForTile(tempModel.LocalPlayerMark.ToTileState()),
                 tempModel.Match.Self.DisplayName,
                 false);

            opponentInfos.Update((tempModel.Opponent != null && tempModel.Opponent.Player != null) ? tempModel.Opponent.Player.image : defaulAvatar,
                                  GetSpriteForTile(tempModel.OpponentMark.ToTileState()),
                                  GetColorForTile(tempModel.OpponentMark.ToTileState()),
                                  tempModel.Opponent != null ? tempModel.Opponent.DisplayName : "Unmatched player",
                                  false,
                                  tempModel.Opponent != null && tempModel.Opponent.Player == null);
        }

        public void HideGameSection()
        {
            ClearBoard();
            IsInPlayingMode = false;
            currentModel = null;
            gameSection.SetActive(false);
            rematchButton.gameObject.SetActive(false);
            multiplayerSection.SetActive(true);
        }

        public void ShowAlert(string errorMessage, string title = "Error")
        {
            NativeUI.Alert(title ?? "Error", errorMessage ?? "Unknown error.");
        }

        public void ShowYesNoPopup(string title, string message, Action<bool> callback = null)
        {
            popup.Alert(title, message, "Yes", "No");
            popup.OnClosed.AddListener(buttonName =>
            {
                if (callback == null)
                    return;

                callback(buttonName == AlertPopup.ButtonName.ButtonA);
            });
        }

        public bool CheckShowingMatch(string matchId)
        {
            if (matchId == null || currentModel == null || currentModel.Match == null)
                return false;

            return matchId.Equals(currentModel.Match.MatchId);
        }

        public void StartProgressUI(string message)
        {
            if (progressUIRunningCoroutine != null)
                StopCoroutine(progressUIRunningCoroutine);

            progressUIRunningCoroutine = StartCoroutine(ProgressRunningUICoroutine(message));
        }

        public void StopProgressUI(string message, float appearTime = -1)
        {
            if (progressUIRunningCoroutine == null)
                return;

            StopCoroutine(progressUIRunningCoroutine);

            if (progressUIStoppingCoroutine != null)
                StopCoroutine(progressUIStoppingCoroutine);

            progressUIStoppingCoroutine =
                StartCoroutine(ProgressUIStoppingCoroutine(message, appearTime < 0 ? progressUIDisappearTime : appearTime));
        }

        #endregion

        #region Private Methods

        private IEnumerator ProgressRunningUICoroutine(string message)
        {
            progressTextObject.SetActive(true);
            int loopTimes = 0;

            while (true)
            {
                string newMessage = message + GetDotMessageForProgessUI(loopTimes);
                progressText.text = newMessage;

                loopTimes++;
                yield return new WaitForSeconds(50 / progressUISpeed); /// 100 = <see cref="progressUISpeed"/> max range value / 2.
            }
        }

        private IEnumerator ProgressUIStoppingCoroutine(string message, float appearTime)
        {
            progressText.text = message;
            yield return new WaitForSeconds(appearTime);
            progressText.text = "";
            progressTextObject.SetActive(false);
        }

        private string GetDotMessageForProgessUI(int currentLoopTimes)
        {
            if (currentLoopTimes % 3 == 0)
                return ".";

            if (currentLoopTimes % 3 == 1)
                return "..";

            return "...";
        }

        private void InstantiateBoard(TicTacToeGameModel model)
        {
            /// This mean the board has been instantiated before 
            /// and it's showing the same model. So we'll just update it.
            if (board != null && model.Match.MatchId.Equals(currentModel.Match.MatchId))
            {
                for (int i = 0; i < model.TransferDatas.Board.Length; i++)
                {
                    var innerArray = model.TransferDatas.Board[i];
                    for (int j = 0; j < innerArray.Length; j++)
                    {
                        UpdateTileUI(i, j, innerArray[j]);
                    }
                }
                return;
            }

            /// Otherwise we need to clear the old board and create a new one.
            ClearBoard();

            TileSize = CalculateTileSize(model.TransferDatas.Size);
            grid.cellSize = new Vector2(TileSize, TileSize);
            grid.spacing = new Vector2(TileSpacing, TileSpacing);
            grid.padding.top = (int)TileSpacing;
            grid.padding.bottom = (int)TileSpacing;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = model.TransferDatas.Size;

            /// Init array.
            board = new Button[model.TransferDatas.Size][];
            for (int i = 0; i < board.Length; i++)
                board[i] = new Button[model.TransferDatas.Size];

            /// Instantiate prefabs.
            for (int i = 0; i < board.Length; i++)
            {
                var innerArray = board[i];
                for (int j = 0; j < innerArray.Length; j++)
                {
                    int x = i, y = j;
                    innerArray[j] = Instantiate(tilePrefab, grid.transform);
                    UpdateTileUI(i, j, model.TransferDatas.Board[i][j]);
                    innerArray[j].onClick.AddListener(() =>
                    {
                        Move(x, y);
                        demoUtils.PlayButtonSound();
                    });
                }
            }
        }

        private void ClearBoard()
        {
            if (board == null)
                return;

            foreach (var array in board)
                foreach (var button in array)
                    Destroy(button.gameObject);

            board = null;
        }

        private float CalculateTileSize(int boardSize)
        {
            float gridSize = gridRect.sizeDelta.x;
            float sizeWithoutSpace = gridSize / boardSize;
            return sizeWithoutSpace - (sizeWithoutSpace * spaceRate);
        }

        private void UpdateTileUI(int x, int y, TicTacToeGameModel.TileState state)
        {
            if (board == null)
                return;

            try
            {
                board[x][y].image.sprite = GetSpriteForTile(state);
                board[x][y].image.color = GetColorForTile(state);
                board[x][y].interactable = state == TicTacToeGameModel.TileState.Blank;
            }
            catch (IndexOutOfRangeException e)
            {
                ShowAlert(e.Message);
            }
        }

        private void RegisterButtonsEvents()
        {
            showMatchesUIButton.onClick.AddListener(ShowMatchesUI);
            hideGameButton.onClick.AddListener(() => HideGameSection());
            leaveMatchButton.onClick.AddListener(LeaveMatch);
            rematchButton.onClick.AddListener(control.Rematch);
        }

        private void ShowMatchesUI()
        {
            control.ShowMatchesUI();
        }

        private Sprite GetSpriteForTile(TicTacToeGameModel.TileState state)
        {
            switch (state)
            {
                case TicTacToeGameModel.TileState.Blank: return blankImage;
                case TicTacToeGameModel.TileState.X: return xImage;
                case TicTacToeGameModel.TileState.O: return oImage;
                default: return null;
            }
        }

        private Color GetColorForTile(TicTacToeGameModel.TileState state)
        {
            switch (state)
            {

                case TicTacToeGameModel.TileState.X: return xColor;
                case TicTacToeGameModel.TileState.O: return oColor;

                case TicTacToeGameModel.TileState.Blank:
                default:
                    return Color.white;
            }
        }

        private string GetResultText(MatchOutcome.ParticipantResult result)
        {
            switch (result)
            {
                case MatchOutcome.ParticipantResult.Won: return wonText;

                case MatchOutcome.ParticipantResult.Lost: return loseText;

                case MatchOutcome.ParticipantResult.Tied: return tiedText;

                case MatchOutcome.ParticipantResult.CustomPlacement:
                case MatchOutcome.ParticipantResult.None:
                default:
                    return "Unknown";
            }
        }

        #endregion
    }
}
