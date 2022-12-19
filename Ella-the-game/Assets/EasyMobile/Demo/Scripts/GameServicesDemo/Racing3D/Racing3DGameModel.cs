using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace EasyMobile.Demo
{
    public class Racing3DGameModel
    {
        public enum MessageTypes
        {
            None = 0,
            StartGame,
            Ready,
            Move,
            UseNitro,
            HitPowerUp,
            FinishRace,
            RematchRequest,
            RematchResponse,
        }

        public enum MoveDirections
        {
            Left,
            Right
        }

        public enum GameoverReason
        {
            None = 0,
            FinishRace,
            OpponentFinishRace,
            HitPowerUp,
            OpponentHitPowerUp,
            OpponentLeftRoom,
        }

        public string SelfId { get; private set; }
        public string OpponentId { get; private set; }
        public List<Vector3> PowerUpsPosition { get; set; }
        public List<Vector3> SideObjectsPosition { get; set; }

        public bool IsHost
        {
            get { return SelfId.CompareTo(OpponentId) > 0; }
        }

        public Racing3DGameModel(string selfId, string opponentId)
        {
            SelfId = selfId;
            OpponentId = opponentId;
        }

        public byte[] CreateStartGameMessage()
        {
            return new StartGameMessage(PowerUpsPosition, SideObjectsPosition).ToByteArray();
        }

        public byte[] CreateReadyMessage()
        {
            return new ReadyMessage().ToByteArray();
        }

        public byte[] CreateMoveMessage(MoveDirections direction)
        {
            return new MoveMessage(direction).ToByteArray();
        }

        public byte[] CreateUseNitroMessage()
        {
            return new UseNitroMessage().ToByteArray();
        }

        public byte[] CreateHitPowerUpMessage()
        {
            return new HitPowerUpMessage().ToByteArray();
        }

        public byte[] CreateFinishRaceMessage()
        {
            return new FinishRaceMessage().ToByteArray();
        }

        public byte[] CreateRematchRequestMessage()
        {
            return new RematchRequestchMessage().ToByteArray();
        }

        public byte[] CreateRematchReponseMessage(bool accepted)
        {
            return new RematchResponseMaessage(accepted).ToByteArray();
        }
        
        public RealtimeMultiplayerMessage FromByteArray(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException();

            if (bytes.Length > 10)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var binaryFormatter = new BinaryFormatter();
                    memoryStream.Write(bytes, 0, bytes.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var obj = (RealtimeMultiplayerMessage)binaryFormatter.Deserialize(memoryStream);
                    return obj;
                }
            }

            if (bytes[0] == 2)
                return new ReadyMessage();

            if (bytes[0] == 3)
                return new MoveMessage((MoveDirections)bytes[1]);

            if (bytes[0] == 4)
                return new UseNitroMessage();

            if (bytes[0] == 5)
                return new HitPowerUpMessage();

            if (bytes[0] == 6)
                return new FinishRaceMessage();

            if (bytes[0] == 7)
                return new RematchRequestchMessage();

            if (bytes[0] == 8)
                return new RematchResponseMaessage(bytes[1] == 1);

            return null;
        }

        [Serializable]
        public abstract class RealtimeMultiplayerMessage
        {
            public abstract MessageTypes Type { get; }
            public abstract byte[] ToByteArray();
        }

        [Serializable]
        public class StartGameMessage : RealtimeMultiplayerMessage
        {
            public override MessageTypes Type { get { return MessageTypes.StartGame; } }

            public List<Vector3> PowerUpsPosition
            {
                get { return powerUpsPosition.Select(v => (Vector3)v).ToList(); }
            }

            public List<Vector3> SideObjectsPosition
            {
                get { return sideObjectsPosition.Select(v => (Vector3)v).ToList(); }
            }

            private List<SerializableVector3> powerUpsPosition;

            private List<SerializableVector3> sideObjectsPosition;

            public StartGameMessage(List<Vector3> powerUpsPosition, List<Vector3> sideObjectsPosition)
            {
                this.sideObjectsPosition = new List<SerializableVector3>();
                this.powerUpsPosition = new List<SerializableVector3>();
                powerUpsPosition.ForEach(op => this.powerUpsPosition.Add(op));
                sideObjectsPosition.ForEach(op => this.sideObjectsPosition.Add(op));
            }

            public override byte[] ToByteArray()
            {
                using (var memoryStream = new MemoryStream())
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, this);
                    return memoryStream.ToArray();
                }
            }
        }

        [Serializable]
        public class ReadyMessage : RealtimeMultiplayerMessage
        {
            public override MessageTypes Type { get { return MessageTypes.Ready; } }

            public override byte[] ToByteArray()
            {
                return new byte[] { 2 };
            }
        }

        [Serializable]
        public class MoveMessage : RealtimeMultiplayerMessage
        {
            public override MessageTypes Type { get { return MessageTypes.Move; } }
            public MoveDirections Direction { get; private set; }

            public MoveMessage(MoveDirections direction) : base()
            {
                Direction = direction;
            }

            public override byte[] ToByteArray()
            {
                return new byte[] { 3, (byte)Direction };
            }
        }

        [Serializable]
        public class UseNitroMessage : RealtimeMultiplayerMessage
        {
            public override MessageTypes Type { get { return MessageTypes.UseNitro; } }

            public override byte[] ToByteArray()
            {
                return new byte[] { 4 };
            }
        }

        [Serializable]
        public class HitPowerUpMessage : RealtimeMultiplayerMessage
        {
            public override MessageTypes Type { get { return MessageTypes.HitPowerUp; } }

            public override byte[] ToByteArray()
            {
                return new byte[] { 5 };
            }
        }

        [Serializable]
        public class FinishRaceMessage : RealtimeMultiplayerMessage
        {
            public override MessageTypes Type { get { return MessageTypes.FinishRace; } }

            public override byte[] ToByteArray()
            {
                return new byte[] { 6 };
            }
        }

        [Serializable]
        public class RematchRequestchMessage : RealtimeMultiplayerMessage
        {
            public override MessageTypes Type { get { return MessageTypes.RematchRequest; } }

            public override byte[] ToByteArray()
            {
                return new byte[] { 7 };
            }
        }

        [Serializable]
        public class RematchResponseMaessage : RealtimeMultiplayerMessage
        {
            public override MessageTypes Type { get { return MessageTypes.RematchResponse; } }
            public bool Accepted { get { return accepted == 1; } }

            private byte accepted = 0;

            public RematchResponseMaessage(bool accepted)
            {
                this.accepted = accepted ? (byte)1 : (byte)0;
            }

            public override byte[] ToByteArray()
            {
                return new byte[] { 8, accepted };
            }
        }

        [Serializable]
        public struct SerializableVector3
        {
            public float x, y, z;

            public SerializableVector3(float x, float y, float z = 0)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static explicit operator Vector3(SerializableVector3 sVector3)
            {
                return new Vector3(sVector3.x, sVector3.y, sVector3.z);
            }

            public static implicit operator SerializableVector3(Vector3 vector3)
            {
                return new SerializableVector3(vector3.x, vector3.y, vector3.z);
            }
        }
    }
}
