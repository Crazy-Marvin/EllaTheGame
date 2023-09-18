using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EasyMobile.Demo
{
    [Serializable]
    public class TicTacToeGameModel
    {
        public enum TileState
        {
            Blank,
            X,
            O,
        };

        public enum Mark
        {
            /// <summary>
            /// X. Always go first.
            /// </summary>
            X,

            /// <summary>
            /// O. Always go second.
            /// </summary>
            O,
        }

        [Serializable]
        public class TransferableDatas
        {
            public int Size { get; private set; }
            public int MoveCount { get; set; }
            public bool IsGameOver { get; set; }
            public TileState[][] Board { get; private set; }
            public Mark CurrentTurn { get; set; }
            public MatchOutcome FinalResult { get; set; }
            public string XPlayerId { get; set; }

            public TransferableDatas(int size)
            {
                Size = size;
                XPlayerId = string.Empty;
                CurrentTurn = Mark.X;

                Board = new TileState[Size][];
                for (int i = 0; i < Board.Length; i++)
                    Board[i] = new TileState[Size];
            }

            /// <summary>
            /// Convert this object to a byte array.
            /// </summary>
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
                catch(Exception)
                {                    
                    return null;
                }
            }

            public override string ToString()
            {
                return string.Format("[TransferableDatas: Size ={0}, MoveCount ={1}, IsGameOver ={2}," +
                                     "Board ={3}, CurrentTurn ={4}, FinalResult ={5}, XPlayerId ={6}]",
                                     Size, MoveCount, IsGameOver,
                                     string.Join(", ", Board.SelectMany(ar => ar).Select(t => t.ToString()).ToArray()),
                                     CurrentTurn, FinalResult, XPlayerId);
            }

            /// <summary>
            /// Construct new <see cref="TransferableDatas"/> object from byte array.
            /// </summary>
            public static TransferableDatas FromByteArray(byte[] bytes)
            {
                if (bytes == null)
                    throw new ArgumentNullException();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    memoryStream.Write(bytes, 0, bytes.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    object obj = binaryFormatter.Deserialize(memoryStream);
                    return obj is TransferableDatas ? obj as TransferableDatas : null;
                }
            }
        }

        public TransferableDatas TransferDatas { get; private set; }
        public TurnBasedMatch Match { get; private set; }
        public Mark LocalPlayerMark { get; private set; }

        public Mark OpponentMark
        {
            get
            {
                if (LocalPlayerMark == Mark.O)
                    return Mark.X;

                return Mark.O;
            }
        }

        public Participant Opponent
        {
            get
            {
                if (Match == null || Match.Participants == null)
                    return null;

                foreach (var participant in Match.Participants)
                {
                    if (participant.ParticipantId != Match.SelfParticipantId)
                        return participant;
                }

                return null;
            }
        }

        public string OpponentId
        {
            get
            {
                if (Opponent == null)
                    return null;

                return Opponent.ParticipantId;
            }
        }

        public MatchOutcome.ParticipantResult LocalFinalResult
        {
            get
            {
                if (TransferDatas == null || TransferDatas.FinalResult == null || Match == null)
                    return MatchOutcome.ParticipantResult.None;

                return TransferDatas.FinalResult.GetParticipantResult(Match.SelfParticipantId);
            }
        }

        public TicTacToeGameModel(TurnBasedMatch match, int size)
        {
            if (match == null)
                throw new ArgumentNullException();

            Match = match;
            TransferDatas = Match.Data == null ? new TransferableDatas(size) : TransferableDatas.FromByteArray(Match.Data);
            LocalPlayerMark = CalculateLocalPlayerTurn();
        }

        public override string ToString()
        {
            return string.Format("[TicTacToeGameModel: TransferDatas ={0}, Match={1}, LocalPlayerMark={2}, Opponent={3}]",
                                 TransferDatas != null ? TransferDatas.ToString() : "NULL",
                                 Match != null ? Match.ToString() : "NULL", LocalPlayerMark,
                                 Opponent != null ? Opponent.ToString() : "NULL");
        }

        private Mark CalculateLocalPlayerTurn()
        {
            if (TransferDatas.XPlayerId.Equals(string.Empty))
                TransferDatas.XPlayerId = Match.SelfParticipantId;

            return Match.SelfParticipantId.Equals(TransferDatas.XPlayerId) ? Mark.X : Mark.O;
        }
    }

    public static class TicTacToeModelExtension
    {
        public static TicTacToeGameModel.TileState ToTileState(this TicTacToeGameModel.Mark turn)
        {
            switch (turn)
            {
                case TicTacToeGameModel.Mark.X: return TicTacToeGameModel.TileState.X;
                case TicTacToeGameModel.Mark.O: return TicTacToeGameModel.TileState.O;

                default: throw new InvalidCastException();
            }
        }

        /// <summary>
        /// Construct new <see cref="TicTacToeGameModel.TransferableDatas"/> object from byte array.
        /// </summary>
        public static TicTacToeGameModel.TransferableDatas ToTicTacToeGameTranferDatas(this byte[] bytes)
        {
            return TicTacToeGameModel.TransferableDatas.FromByteArray(bytes);
        }
    }
}
