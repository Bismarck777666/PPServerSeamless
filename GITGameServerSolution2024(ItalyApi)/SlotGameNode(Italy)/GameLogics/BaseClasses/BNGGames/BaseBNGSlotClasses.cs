using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SlotGamesNode.GameLogics
{
    public class BaseBNGSlotBetInfo
    {
        public float BetPerLine     { get; set; }
        public int   LineCount       { get; set; }
        public bool  PurchaseFree    { get; set; }
        public bool  MoreBet         { get; set; }
        public long  BalanceVersion  { get; set; }
        public BasePPSlotSpinData               SpinData        { get; set; }
        public List<BaseBNGActionToResponse>    RemainReponses  { get; set; }

        public bool HasRemainResponse
        {
            get
            {
                if (this.RemainReponses != null && this.RemainReponses.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public virtual float TotalBet
        {
            get
            {
                return BetPerLine * LineCount / 100.0f;
            }
        }
        public BaseBNGSlotBetInfo()
        {
            PurchaseFree    = false;
            MoreBet         = false;
            BalanceVersion  = 11;
        }
        public BaseBNGActionToResponse pullRemainResponse()
        {
            if (RemainReponses == null || RemainReponses.Count == 0)
                return null;

            BaseBNGActionToResponse response = RemainReponses[0];
            RemainReponses.RemoveAt(0);
            return response;
        }

        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.BetPerLine     = reader.ReadSingle();
            this.LineCount      = reader.ReadInt32();
            this.PurchaseFree   = reader.ReadBoolean();
            this.MoreBet        = reader.ReadBoolean();
            int remainCount = reader.ReadInt32();
            if (remainCount == 0)
            {
                this.RemainReponses = null;
            }
            else
            {
                this.RemainReponses = new List<BaseBNGActionToResponse>();
                for (int i = 0; i < remainCount; i++)
                {
                    BNGActionTypes actionType = (BNGActionTypes)(byte)reader.ReadByte();
                    string strResponse = (string)reader.ReadString();
                    this.RemainReponses.Add(new BaseBNGActionToResponse(actionType, strResponse));
                }
            }
            int spinDataType = reader.ReadInt32();
            if (spinDataType == 0)
            {
                this.SpinData = null;
            }
            else if (spinDataType == 1)
            {
                this.SpinData = new BasePPSlotSpinData();
                this.SpinData.SerializeFrom(reader);
            }
            else
            {
                this.SpinData = new BasePPSlotStartSpinData();
                this.SpinData.SerializeFrom(reader);
            }
            this.BalanceVersion = reader.ReadInt64();
        }
        
        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(this.BetPerLine);
            writer.Write(this.LineCount);
            writer.Write(this.PurchaseFree);
            writer.Write(this.MoreBet);
            if (this.RemainReponses == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.RemainReponses.Count);
                for (int i = 0; i < this.RemainReponses.Count; i++)
                {
                    writer.Write((byte)this.RemainReponses[i].ActionType);
                    writer.Write((string)this.RemainReponses[i].Response);
                }
            }
            if (this.SpinData == null)
                writer.Write(0);
            else if (this.SpinData is BasePPSlotStartSpinData)
                writer.Write(2);
            else
                writer.Write(1);
            if (this.SpinData != null)
                this.SpinData.SerializeTo(writer);
            writer.Write(this.BalanceVersion);
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

    public class BaseBNGMultiFreeSlotBetInfo : BaseBNGSlotBetInfo
    {
        public int FreeSpinType { get; set; }
        public BaseBNGMultiFreeSlotBetInfo()
        {

        }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.FreeSpinType = reader.ReadInt32();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.FreeSpinType);
        }
    }

    public class BaseBNGActionToResponse
    {
        public BNGActionTypes ActionType { get; set; }
        public string         Response   { get; set; }

        public BaseBNGActionToResponse(BNGActionTypes actionType, string strResponse)
        {
            this.ActionType = actionType;
            this.Response   = strResponse;
        }
    }
    
    public class BaseBNGSlotSpinResult
    {
        public double           TotalWin                { get; set; }
        public double           LastWin                 { get; set; }
        public BNGActionTypes   NextAction              { get; set; }
        public string           ResultString            { get; set; }
        public string           RoundID                 { get; set; }
        public string           TransactionID           { get; set; }
        public virtual double WinMoney
        {
            get
            {
                return this.TotalWin;
            }
        }
        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(this.TotalWin);
            writer.Write((byte)this.NextAction);
            writer.Write(this.ResultString);
            writer.Write(this.LastWin);
            writer.Write(this.RoundID);
            writer.Write(this.TransactionID);

        }
        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.TotalWin       = reader.ReadDouble();
            this.NextAction     = (BNGActionTypes)reader.ReadByte();
            this.ResultString   = reader.ReadString();
            this.LastWin        = reader.ReadDouble();
            this.RoundID        = reader.ReadString();
            this.TransactionID  = reader.ReadString();
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
        public BaseBNGSlotSpinResult()
        {
            this.ResultString = "";
        }
    }

    public enum BNGActionTypes
    {
        NONE                = 0,
        SPIN                = 1,
        FREESPININIT        = 2,
        FREESPIN            = 3,
        FREESPINSTOP        = 4,
        BONUSINIT           = 5,
        RESPIN              = 6,     
        BONUSSTOP           = 7, 
        BONUSFREESPINSTOP   = 8,
        BUYSPIN             = 9,
    }
}
