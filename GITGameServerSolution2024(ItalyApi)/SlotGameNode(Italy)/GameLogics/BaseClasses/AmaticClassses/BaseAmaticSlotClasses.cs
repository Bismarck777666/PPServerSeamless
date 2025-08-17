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

namespace SlotGamesNode.GameLogics
{
    public class BaseAmaticSlotSpinResult
    {
        public AmaticMessageType    Action          { get; set; }
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
            this.Action             = (AmaticMessageType)reader.ReadInt32();
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
        public BaseAmaticSlotSpinResult()
        {
            this.ResultString       = "";
        }
    }

    public class BaseAmaticActionToResponse
    {
        public AmaticMessageType    ActionType  { get; set; }
        public string               Response    { get; set; }

        public BaseAmaticActionToResponse(AmaticMessageType actionType, string strResponse)
        {
            this.ActionType = actionType;
            this.Response   = strResponse;
        }
    }

    public class BaseAmaticSlotBetInfo
    {
        public int                              PlayLine            { get; set; }   //베팅라인수
        public int                              PlayBet             { get; set; }   //베팅스텝
        public int                              PurchaseStep        { get; set; }   //구매스텝
        public int                              MoreBet             { get; set; }   //앤티스텝
        public CurrencyEnum                     CurrencyInfo        { get; set; }   //화페
        public int                              GambleType          { get; set; }   //갬블인덱스
        public bool                             GambleHalf          { get; set; }   //갬블하프
        public BasePPSlotSpinData               SpinData            { get; set; }   
        public List<BaseAmaticActionToResponse> RemainReponses      { get; set; }

        public virtual int  RelativeTotalBet    => PlayLine * 1;
        public bool         isPurchase          => PurchaseStep > -1;
        public bool         isMoreBet           => MoreBet > -1;
        public bool         HasRemainResponse
        {
            get
            {
                if (this.RemainReponses != null && this.RemainReponses.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public BaseAmaticSlotBetInfo()
        {
        }

        public BaseAmaticActionToResponse pullRemainResponse()
        {
            if (RemainReponses == null || RemainReponses.Count == 0)
                return null;

            BaseAmaticActionToResponse response = RemainReponses[0];
            RemainReponses.RemoveAt(0);
            return response;
        }

        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.PlayLine       = reader.ReadInt32();
            this.PlayBet        = reader.ReadInt32();
            this.PurchaseStep   = reader.ReadInt32();
            this.MoreBet        = reader.ReadInt32();
            this.CurrencyInfo   = (CurrencyEnum)reader.ReadInt32();
            this.GambleType     = reader.ReadInt32();
            this.GambleHalf     = reader.ReadBoolean();
            int remainCount     = reader.ReadInt32();
            if (remainCount == 0)
            {
                this.RemainReponses = null;
            }
            else
            {
                this.RemainReponses = new List<BaseAmaticActionToResponse>();
                for(int i = 0; i < remainCount; i++)
                {
                    AmaticMessageType actionType   = (AmaticMessageType)(byte)reader.ReadByte();
                    string      strResponse = (string)reader.ReadString();
                    this.RemainReponses.Add(new BaseAmaticActionToResponse(actionType, strResponse));
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
