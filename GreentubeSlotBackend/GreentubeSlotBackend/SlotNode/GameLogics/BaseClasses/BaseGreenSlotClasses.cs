using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using GITProtocol;
using Newtonsoft.Json.Linq;
using PCGSharp;
using MongoDB.Bson;

namespace SlotGamesNode.GameLogics
{
    public class BasePPSlotSpinData
    {
        public double       SpinOdd         { get; set; }
        public int          SpinType        { get; set; } 
        public List<string> SpinStrings     { get; set; }

        public BasePPSlotSpinData()
        {
        }

        public BasePPSlotSpinData(int spinType, double spinOdd,List<string> spinStrings)
        {
            this.SpinType       = spinType;
            this.SpinOdd        = spinOdd;
            this.SpinStrings    = spinStrings;
        }

        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(this.SpinType);
            writer.Write(this.SpinOdd);
            SerializeUtils.writeStringList(writer, this.SpinStrings);
        }
        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.SpinType = reader.ReadInt32();
            this.SpinOdd = reader.ReadDouble();
            this.SpinStrings = SerializeUtils.readStringList(reader);
        }
    }

    public class BasePPSlotStartSpinData : BasePPSlotSpinData
    {
        public double                           StartOdd        { get; set; }        
        public int                              FreeSpinGroup   { get; set; }
        public List<int>                        PossibleRanges  { get; set; }
        public List<BsonDocument>               FreeSpins       { get; set; }
        public double                           MaxOdd          { get; set; }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.StartOdd       = reader.ReadDouble();
            this.FreeSpinGroup  = reader.ReadInt32();
            this.PossibleRanges = SerializeUtils.readIntList(reader);
            this.MaxOdd         = reader.ReadDouble();

            this.FreeSpins      = new List<BsonDocument>();
            int count = reader.ReadInt32();
            for(int i = 0; i < count; i++)
            {
                string strJson = reader.ReadString();
                this.FreeSpins.Add(BsonDocument.Parse(strJson));
            }
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.StartOdd);
            writer.Write(this.FreeSpinGroup);
            SerializeUtils.writeIntList(writer, this.PossibleRanges);
            writer.Write(this.MaxOdd);
            if (this.FreeSpins == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.FreeSpins.Count);
                for (int i = 0; i < this.FreeSpins.Count; i++)
                    writer.Write(this.FreeSpins[i].ToJson());
            }            
        }

    }

    public enum StartSpinBuildTypes
    {
        IsNaturalRandom = 0,
        IsTotalRandom   = 1,
        IsRangeLimited  = 2,
    }

    public class OddAndIDData
    {
        public int      ID      { get; set; }
        public double   Odd     { get; set; }

        public OddAndIDData()
        {

        }
        
        public OddAndIDData(int id, double odd)
        {
            this.ID     = id;
            this.Odd    = odd;
        }
    }
    
    public class BaseGreenSlotSpinResult
    {
        public GreenMessageType    Action          { get; set; }
        public double               TotalWin        { get; set; }
        public string               ResultString    { get; set; }
        public virtual double       WinMoney
        {
            get
            {
                return this.TotalWin;
            }
        }
        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(this.TotalWin);
            writer.Write(this.ResultString);
            writer.Write((int)this.Action);
        }
        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.TotalWin           = reader.ReadDouble();
            this.ResultString       = reader.ReadString();
            this.Action             = (GreenMessageType)reader.ReadInt32();
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
        public BaseGreenSlotSpinResult()
        {
            this.ResultString       = "";
        }
    }

    public class BaseGreenActionToResponse
    {
        public GreenMessageType    ActionType  { get; set; }
        public string               Response    { get; set; }

        public BaseGreenActionToResponse(GreenMessageType actionType, string strResponse)
        {
            this.ActionType = actionType;
            this.Response   = strResponse;
        }
    }

    public class BaseGreenSlotBetInfo
    {
        public int          PlayLine            { get; set; } 
        public int          PlayBet             { get; set; } 
        public int          PurchaseStep        { get; set; } 
        public int          MoreBet             { get; set; }
        public Currencies   CurrencyInfo        { get; set; }  
        public int          GambleType          { get; set; } 
        public bool         GambleHalf          { get; set; } 
        public string       RoundID             { get; set; }   
        public string       BetTransactionID    { get; set; }
        public BasePPSlotSpinData               SpinData            { get; set; }   
        public List<BaseGreenActionToResponse> RemainReponses      { get; set; }

        public virtual int  RelativeTotalBet    => PlayLine * 1;
        public bool         isPurchase          => PurchaseStep > -1;
        public bool         isMoreBet           => MoreBet > -1;
        public bool         HasRemainResponse
        {
            get
            {
                if (RemainReponses != null && RemainReponses.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public BaseGreenSlotBetInfo()
        {
        }

        public BaseGreenActionToResponse pullRemainResponse()
        {
            if (RemainReponses == null || RemainReponses.Count == 0)
                return null;

            BaseGreenActionToResponse response = RemainReponses[0];
            RemainReponses.RemoveAt(0);
            return response;
        }

        public void pushFrontResponse(BaseGreenActionToResponse response)
        {
            RemainReponses.Insert(0, response);
        }

        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.PlayLine       = reader.ReadInt32();
            this.PlayBet        = reader.ReadInt32();
            this.PurchaseStep   = reader.ReadInt32();
            this.MoreBet        = reader.ReadInt32();
            this.CurrencyInfo   = (Currencies)reader.ReadInt32();
            this.GambleType     = reader.ReadInt32();
            this.GambleHalf     = reader.ReadBoolean();
            int remainCount     = reader.ReadInt32();
            if (remainCount == 0)
            {
                this.RemainReponses = null;
            }
            else
            {
                this.RemainReponses = new List<BaseGreenActionToResponse>();
                for(int i = 0; i < remainCount; i++)
                {
                    GreenMessageType actionType   = (GreenMessageType)(byte)reader.ReadByte();
                    string      strResponse = (string)reader.ReadString();
                    this.RemainReponses.Add(new BaseGreenActionToResponse(actionType, strResponse));
                }
            }
            int spinDataType    = reader.ReadInt32();
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
            bool hasValue = reader.ReadBoolean();
            if (hasValue)
                this.RoundID = reader.ReadString();

            hasValue = reader.ReadBoolean();
            if (hasValue)
                this.BetTransactionID = reader.ReadString();
        }

        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(this.PlayLine);
            writer.Write(this.PlayBet);
            writer.Write(this.PurchaseStep);
            writer.Write(this.MoreBet);
            writer.Write((int)this.CurrencyInfo);
            writer.Write(this.GambleType);
            writer.Write(this.GambleHalf);
            if (this.RemainReponses == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.RemainReponses.Count);
                for(int i = 0; i < this.RemainReponses.Count; i++)
                {
                    writer.Write((byte)     this.RemainReponses[i].ActionType);
                    writer.Write((string)   this.RemainReponses[i].Response);
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

            if (this.RoundID == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(this.RoundID);
            }
            if (this.BetTransactionID == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(this.BetTransactionID);
            }
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
}
