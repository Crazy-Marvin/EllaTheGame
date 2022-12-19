using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyMobile.Demo
{
    public class TicTacToeGameControl : GameServicesDemo_Multiplayer_BaseControl
    {
        [SerializeField]
        private TicTacToeGameView view = null;

        private TicTacToeGameModel model;
        private bool canMove;

        public bool CanMove
        {
            get
            {
                if (model == null || model.Match == null)
                    return false;

                return canMove && model.Match.IsMyTurn;
            }
            set { canMove = value; }
        }

        public bool IsMatchDelegateRegistered { get; private set; }

        protected override MatchType MatchType { get { return MatchType.TurnBased; } }

        protected override void LateStart()
        {
            RegisterMatchDelegate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (IsMatchDelegateRegistered)
                GameServices.TurnBased.RegisterMatchDelegate((_, __, ___) => { });
        }

        protected override void CreateQuickMatch()
        {
            CreateQuickMatch(view.BoardSize);
        }

        protected override void CreateWithMatchmaker()
        {
            CreateWithMatchmakerUI(view.BoardSize);
        }

        protected override void AcceptInvitation(Invitation invitation)
        {
            GameServices.TurnBased.AcceptInvitation(invitation, (flag, match) =>
            {
                if (!flag)
                {
                    NativeUI.Alert("Error", "Failed to accept an invitation.");
                    return;
                }

                OnMatchReceived(match, true, false);
            });
        }

        protected override void DeclineInvitation(Invitation invitation)
        {
            GameServices.TurnBased.DeclineInvitation(invitation);
        }

        public void CreateQuickMatch(int size)
        {
            StartCreateQuickMatchSpinningUI();
            GameServices.TurnBased.CreateQuickMatch(GetMatchRequest(size), OnCreateQuickMatch);
        }

        public void CreateWithMatchmakerUI(int size)
        {
            GameServices.TurnBased.CreateWithMatchmakerUI(
                GetMatchRequest(size),
                () => { },
                error => view.ShowAlert(error));
        }

        public void ShowMatchesUI()
        {
            GameServices.TurnBased.ShowMatchesUI();
        }

        public void RegisterMatchDelegate()
        {
            GameServices.TurnBased.RegisterMatchDelegate(OnMatchReceived);
            IsMatchDelegateRegistered = true;
        }

        public void Move(int x, int y, Action<TicTacToeGameModel.Mark> successCallback)
        {
            if (model == null)
                return;

            if (model.TransferDatas.IsGameOver)
                return;

            if (!CanMove)
            {
                view.ShowAlert("Not your turn!!", "Warning");
                return;
            }

            if (model.TransferDatas.Board[x][y] == TicTacToeGameModel.TileState.Blank)
                model.TransferDatas.Board[x][y] = model.TransferDatas.CurrentTurn.ToTileState();

            /// Check end conditions...
            /// Check column...
            for (int i = 0; i < model.TransferDatas.Size; i++)
            {
                if (model.TransferDatas.Board[x][i] != model.TransferDatas.CurrentTurn.ToTileState())
                    break;

                if (i == model.TransferDatas.Size - 1)
                {
                    EndGame(MatchOutcome.ParticipantResult.Won, MatchOutcome.ParticipantResult.Lost, x, y, successCallback);
                    return;
                }
            }

            /// Check row...
            for (int i = 0; i < model.TransferDatas.Size; i++)
            {
                if (model.TransferDatas.Board[i][y] != model.TransferDatas.CurrentTurn.ToTileState())
                    break;

                if (i == model.TransferDatas.Size - 1)
                {
                    EndGame(MatchOutcome.ParticipantResult.Won, MatchOutcome.ParticipantResult.Lost, x, y, successCallback);
                    return;
                }
            }

            /// Check diag...
            if (x == y)
            {
                /// On a diagonal...
                for (int i = 0; i < model.TransferDatas.Size; i++)
                {
                    if (model.TransferDatas.Board[i][i] != model.TransferDatas.CurrentTurn.ToTileState())
                        break;

                    if (i == model.TransferDatas.Size - 1)
                    {
                        EndGame(MatchOutcome.ParticipantResult.Won, MatchOutcome.ParticipantResult.Lost, x, y, successCallback);
                        return;
                    }
                }
            }

            /// Check anti diag...
            if (x + y == model.TransferDatas.Size - 1)
            {
                for (int i = 0; i < model.TransferDatas.Size; i++)
                {
                    if (model.TransferDatas.Board[i][(model.TransferDatas.Size - 1) - i] != model.TransferDatas.CurrentTurn.ToTileState())
                        break;

                    if (i == model.TransferDatas.Size - 1)
                    {
                        EndGame(MatchOutcome.ParticipantResult.Won, MatchOutcome.ParticipantResult.Lost, x, y, successCallback);
                        return;
                    }
                }
            }

            /// Check draw...
            if (model.TransferDatas.MoveCount == (Math.Pow(model.TransferDatas.Size, 2) - 1))
            {
                EndGame(MatchOutcome.ParticipantResult.Tied, MatchOutcome.ParticipantResult.Tied, x, y, successCallback);
                return;
            }

            SwitchTurn(x, y, successCallback);
        }

        public void SwitchTurn(int x, int y, Action<TicTacToeGameModel.Mark> successCallback)
        {
            if (model == null)
                return;

            model.TransferDatas.CurrentTurn = GetOppositeMark(model.TransferDatas.CurrentTurn);
            model.TransferDatas.MoveCount++;
            CanMove = false;

            view.StartProgressUI("Switching turn");
            GameServices.TurnBased.TakeTurn(model.Match, model.TransferDatas.ToByteArray(), model.Opponent,
                success =>
                {
                    view.StopProgressUI("Switching turn: " + GetResultMessage(success));

                    if (!success)
                    {
                        model.TransferDatas.CurrentTurn = GetOppositeMark(model.TransferDatas.CurrentTurn);
                        model.TransferDatas.MoveCount--;
                        CanMove = true;
                        model.TransferDatas.Board[x][y] = TicTacToeGameModel.TileState.Blank;
                    }
                    else
                    {
                        if (successCallback != null)
                            successCallback(GetOppositeMark(model.TransferDatas.CurrentTurn));
                    }
                });
        }

        public void LeaveMatch()
        {
            if (model == null || model.Match == null)
                return;

            if (CanMove)
            {
                view.StartProgressUI("Leaving match (during your turn) ");
                GameServices.TurnBased.LeaveMatchInTurn(model.Match, model.OpponentId, flag =>
                    {
                        if (flag)
                            view.HideGameSection();

                        view.StopProgressUI("");
                        view.ShowAlert("Leave match (during your turn). Result: " + (flag ? "success. " : "failed. "), "Leave match");
                    });
            }
            else
            {
                view.StartProgressUI("Leaving match (not your turn) ");
                GameServices.TurnBased.LeaveMatch(model.Match, flag =>
                    {
                        if (flag)
                            view.HideGameSection();

                        view.StopProgressUI("");
                        view.ShowAlert("Leave match (not your turn). Result: " + (flag ? "success. " : "failed. ") +
                            (!flag ? "\nYou can only leave when you received a match and haven't made any change to its data." +
                            "Wait for your next turn or click \"HIDE GAME\" button and open it again via \"SHOW MATCHES UI\". " : ""),
                            "Leave match");
                    });
            }
        }

        public void Rematch()
        {
            if (model == null || model.Match == null)
                return;

            GameServices.TurnBased.Rematch(model.Match, (flag, match) =>
                {
                    if (!flag)
                    {
                        view.ShowAlert("Your rematch request has been failed!!!", "Rematch failed");
                        return;
                    }

                    OnMatchReceived(match, true, false);
                });
        }

        private void OnMatchReceived(TurnBasedMatch match, bool autoLaunch, bool playerWantsToQuit)
        {
            if (match == null)
            {
                view.ShowAlert("Received an invalid match.");
                return;
            }

            Debug.Log("A match has been arrived, matchId: " + match.MatchId);

            int boardSize = 0;
            if (match.Data == null || match.Data.Length < 1)
            {
                boardSize = view.BoardSize;
            }
            else
            {
                var data = match.Data.ToTicTacToeGameTranferDatas();
                if (data == null)
                {
                    view.ShowAlert("The arrived match can't be opened in this scene. You might want to open it in the kitchen sink demo instead.");
                    return;
                }
                boardSize = data.Size;
            }

            if (autoLaunch || view.CheckShowingMatch(match.MatchId))
            {
                Debug.Log("Auto launch or update match...");
                CheckAndPlayMatch(match, playerWantsToQuit, boardSize);
            }
            else
            {
                view.ShowYesNoPopup("Accept match", "A match has been arrived. Do you want to play it now?", accept =>
                    {
                        if (accept)
                            CheckAndPlayMatch(match, playerWantsToQuit, boardSize);
                    });
            }
        }

        private void CheckAndPlayMatch(TurnBasedMatch match, bool playerWantsToQuit, int boardSize)
        {
            if (playerWantsToQuit)
            {
                // This only happens on Game Center platform when the local player
                // removes the match in the matches UI while being the turn holder.
                // We'll end the local player's turn and pass the match to a next
                // participant. If there's no active participant left we'll end the match.
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

            model = new TicTacToeGameModel(match, boardSize);

            if (match.Status == TurnBasedMatch.MatchStatus.Ended)
            {
                CanMove = false;
                view.StartProgressUI("Acknowledging finished match");
                GameServices.TurnBased.AcknowledgeFinished(match, flag =>
                    {
                        view.StopProgressUI("Acknowledging finished match: " + GetResultMessage(flag));
                        view.CreateBoard(model);
                        view.ShowGameOverUI(model.LocalFinalResult, true);

                    });
                return;
            }

            var opponent = match.Participants.Where(p => p.ParticipantId != match.SelfParticipantId).FirstOrDefault();
            if (opponent != default(Participant) &&
                (opponent.Status == Participant.ParticipantStatus.Done || opponent.Status == Participant.ParticipantStatus.Left))
            {
                view.ShowGameOverUI(MatchOutcome.ParticipantResult.Won, false);
                NativeUI.Alert("Game Over", "You won. Your opponent has left the match.");
                return;
            }

            CanMove = true;
            view.CreateBoard(model);
        }

        private void EndGame(MatchOutcome.ParticipantResult localPlayerResult, MatchOutcome.ParticipantResult opponentResult,
                             int x, int y, Action<TicTacToeGameModel.Mark> successCallback)
        {
            if (model == null)
                return;

            MatchOutcome outcome = new MatchOutcome();
            outcome.SetParticipantResult(model.Match.SelfParticipantId, localPlayerResult);
            outcome.SetParticipantResult(model.Opponent.ParticipantId, opponentResult);

            model.TransferDatas.IsGameOver = true;
            model.TransferDatas.FinalResult = outcome;
            model.TransferDatas.CurrentTurn = GetOppositeMark(model.TransferDatas.CurrentTurn);
            model.TransferDatas.MoveCount++;
            CanMove = false;

            view.StartProgressUI("Ending game");
            GameServices.TurnBased.Finish(model.Match, model.TransferDatas.ToByteArray(), outcome,
                success =>
                {
                    view.StopProgressUI("Ending game: " + GetResultMessage(success));

                    if (!success)
                    {
                        CanMove = true;
                        model.TransferDatas.CurrentTurn = GetOppositeMark(model.TransferDatas.CurrentTurn);
                        model.TransferDatas.IsGameOver = false;
                        model.TransferDatas.FinalResult = null;
                        model.TransferDatas.MoveCount--;
                        model.TransferDatas.Board[x][y] = TicTacToeGameModel.TileState.Blank;
                    }
                    else
                    {
                        view.EndGame(localPlayerResult);

                        if (successCallback != null)
                            successCallback(GetOppositeMark(model.TransferDatas.CurrentTurn));
                    }
                });
        }

        private string GetResultMessage(bool flag)
        {
            return flag ? "<color=green>success. </color>" : "<color=red>failed. </color>";
        }

        private TicTacToeGameModel.Mark GetOppositeMark(TicTacToeGameModel.Mark mark)
        {
            if (mark == TicTacToeGameModel.Mark.O)
                return TicTacToeGameModel.Mark.X;

            return TicTacToeGameModel.Mark.O;
        }

        private MatchRequest GetMatchRequest(int size)
        {
            return new MatchRequest()
            {
                MinPlayers = 2,
                MaxPlayers = 2,
                Variant = (uint)size,
            };
        }

        private void OnCreateQuickMatch(bool success, TurnBasedMatch match)
        {
            if (IsDestroyed)
                return;

            StopCreateQuickMatchSpinningUI();
            if (!success)
            {
                view.ShowAlert("Failed to create a quick match.");
                return;
            }

            OnMatchReceived(match, true, false);
        }
    }
}
