using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SlotGamesNode.GameLogics
{
    public class BasePlaysonSlotBetInfo
    {
        public float BetPerLine         { get; set; }
        public float TotalBet           { get; set; }
        public bool  PurchaseFree       { get; set; }
        public bool  MoreBet            { get; set; }
        public BasePPSlotSpinData                   SpinData        { get; set; }
        public List<BasePlaysonActionToResponse>    RemainReponses  { get; set; }

        public virtual bool HasRemainResponse
        {
            get
            {
                if (this.RemainReponses != null && this.RemainReponses.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public BasePlaysonSlotBetInfo()
        {
            PurchaseFree    = false;
            MoreBet         = false;
        }
        public virtual BasePlaysonActionToResponse pullRemainResponse()
        {
            if (RemainReponses == null || RemainReponses.Count == 0)
                return null;

            BasePlaysonActionToResponse response = RemainReponses[0];
            RemainReponses.RemoveAt(0);
            return response;
        }

        public virtual void SerializeFrom(BinaryReader reader)
        {
            BetPerLine      = reader.ReadSingle();
            TotalBet        = reader.ReadSingle();
            PurchaseFree    = reader.ReadBoolean();
            MoreBet         = reader.ReadBoolean();
            int remainCount = reader.ReadInt32();
            if (remainCount == 0)
            {
                RemainReponses = null;
            }
            else
            {
                RemainReponses = new List<BasePlaysonActionToResponse>();
                for (int i = 0; i < remainCount; i++)
                {
                    PlaysonActionTypes actionType = (PlaysonActionTypes)(byte)reader.ReadByte();
                    string strResponse = (string)reader.ReadString();
                    RemainReponses.Add(new BasePlaysonActionToResponse(actionType, strResponse));
                }
            }
            int spinDataType = reader.ReadInt32();
            if (spinDataType == 0)
            {
                SpinData = null;
            }
            else if (spinDataType == 1)
            {
                SpinData = new BasePPSlotSpinData();
                SpinData.SerializeFrom(reader);
            }
            else
            {
                SpinData = new BasePPSlotStartSpinData();
                SpinData.SerializeFrom(reader);
            }
        }
        
        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(BetPerLine);
            writer.Write(TotalBet);
            writer.Write(PurchaseFree);
            writer.Write(MoreBet);
            if (RemainReponses == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(RemainReponses.Count);
                for (int i = 0; i < RemainReponses.Count; i++)
                {
                    writer.Write((byte)RemainReponses[i].ActionType);
                    writer.Write((string)RemainReponses[i].Response);
                }
            }
            if (SpinData == null)
                writer.Write(0);
            else if (SpinData is BasePPSlotStartSpinData)
                writer.Write(2);
            else
                writer.Write(1);
            if (SpinData != null)
                SpinData.SerializeTo(writer);
        }

        public byte[] convertToByte()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    this.SerializeTo(bw);
                }
                return ms.ToArray();
            }
        }
    }

    public class BasePlaysonActionToResponse
    {
        public PlaysonActionTypes   ActionType  { get; set; }
        public string               Response    { get; set; }

        public BasePlaysonActionToResponse(PlaysonActionTypes actionType, string strResponse)
        {
            ActionType  = actionType;
            Response    = strResponse;
        }
    }
    
    public class BasePlaysonSlotSpinResult
    {
        public double               TotalWin                { get; set; }
        public double               FreeTrigerWin           { get; set; }
        public PlaysonActionTypes   CurrentAction           { get; set; }
        public PlaysonActionTypes   NextAction              { get; set; }
        public string               ResultString            { get; set; }
        public string               RoundID                 { get; set; }
        public string               TransactionID           { get; set; }
        public virtual double       WinMoney
        {
            get
            {
                return TotalWin;
            }
        }
        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(TotalWin);
            writer.Write((byte)NextAction);
            writer.Write(ResultString);
            writer.Write(FreeTrigerWin);
            writer.Write(RoundID);
            writer.Write(TransactionID);

        }
        public virtual void SerializeFrom(BinaryReader reader)
        {
            TotalWin        = reader.ReadDouble();
            NextAction      = (PlaysonActionTypes)reader.ReadByte();
            ResultString    = reader.ReadString();
            FreeTrigerWin   = reader.ReadDouble();
            RoundID         = reader.ReadString();
            TransactionID   = reader.ReadString();
        }
        public byte[] convertToByte()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    SerializeTo(bw);
                }
                return ms.ToArray();
            }
        }
        public BasePlaysonSlotSpinResult()
        {
            this.ResultString = "";
        }
    }

    public enum PlaysonActionTypes
    {
        NONE            = 0,
        SPIN            = 1,
        FREESPIN        = 2,
        BONUS           = 3,
        Respin          = 4,
    }
}
