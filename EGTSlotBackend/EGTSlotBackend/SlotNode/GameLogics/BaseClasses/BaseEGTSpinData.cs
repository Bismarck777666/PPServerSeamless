using GITProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics.BaseClasses
{
    public class BaseEGTSpinData
    {
        public double SpinOdd { get; set; }
        public int SpinType { get; set; } //0:일반스핀, 1:시작프리스핀 + 프리스핀, 100:시작프리스핀, 200, 201, 202...: 그에 상응한 프리스핀조합 
        public List<string> SpinStrings { get; set; }
        public BaseEGTSpinData() { }
    }

    public class BaseEGTSlotSpinResult
    {
        public EGTMessageType Action { get; set; }
        public double TotalWin { get; set; }
        public double CollectWin {  get; set; } // only for freespin
        public string ResultString { get; set; }
        public string FirstSpinString { get; set; }
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
            writer.Write(this.CollectWin);
            writer.Write(this.ResultString);
            writer.Write(this.FirstSpinString);
            writer.Write((int)this.Action);
        }
        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.TotalWin = reader.ReadDouble();
            this.CollectWin = reader.ReadDouble();
            this.ResultString = reader.ReadString();
            this.FirstSpinString = reader.ReadString();
            this.Action = (EGTMessageType)reader.ReadInt32();
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
        public BaseEGTSlotSpinResult()
        {
            this.ResultString = "";
        }
    }
    public class BaseEGTActionToResponse
    {
        public EGTMessageType ActionType { get; set; }
        public string Response { get; set; }

        public BaseEGTActionToResponse(EGTMessageType actionType, string strResponse)
        {
            this.ActionType = actionType;
            this.Response = strResponse;
        }
    }
    public class BaseEGTSlotBetInfo
    {
        public int PlayLine { get; set; }   //베팅라인수
        public int PlayBet { get; set; }   //베팅스텝, Real Betting Amount
        public int MoreBet { get; set; }   //앤티스텝
        public Currencies CurrencyInfo { get; set; }   //화페
        public int FeatureId { get; set; }
        public double BetMultiplier { get; set; }
        public int GambleType { get; set; }   //갬블인덱스
        public bool GambleHalf { get; set; }   //갬블하프
        public int GambleRound { get; set; }
        public double GambleInitTotalWin { get; set; }
        public double GambleInitCollectWin { get; set; }
        public List<int> GambleCardHistory { get; set; }
        public int FreespinRoundUsed { get; set; }
        public int FreespinTotalRound { get; set; }

        public string RoundID { get; set; }
        public string BetTransactionID { get; set; }
        public BasePPSlotSpinData SpinData { get; set; }
        public string SpinInfo { get; set; }
        public List<BaseEGTActionToResponse> RemainReponses { get; set; }

        public virtual int RelativeTotalBet => PlayLine * 1;
        public bool isPurchase { get; set; }
        public bool isMoreBet => MoreBet > -1;
        public bool HasRemainResponse
        {
            get
            {
                if (RemainReponses != null && RemainReponses.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public BaseEGTSlotBetInfo()
        {
            GambleCardHistory = new List<int>();
            FeatureId = 0;
            BetMultiplier = 1;
        }

        public BaseEGTActionToResponse pullRemainResponse()
        {
            if (RemainReponses == null || RemainReponses.Count == 0)
                return null;

            BaseEGTActionToResponse response = RemainReponses[0];
            RemainReponses.RemoveAt(0);
            return response;
        }

        public void pushFrontResponse(BaseEGTActionToResponse response)
        {
            RemainReponses.Insert(0, response);
        }

        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.PlayLine = reader.ReadInt32();
            this.PlayBet = reader.ReadInt32();
            this.isPurchase = reader.ReadBoolean();
            this.MoreBet = reader.ReadInt32();
            this.FeatureId = reader.ReadInt32();
            this.BetMultiplier = reader.ReadDouble();
            this.CurrencyInfo = (Currencies)reader.ReadInt32();
            this.FreespinRoundUsed = reader.ReadInt32();
            this.FreespinTotalRound = reader.ReadInt32();
            this.GambleType = reader.ReadInt32();
            this.GambleHalf = reader.ReadBoolean();
            this.GambleRound = reader.ReadInt32();
            int gambleHistoryCount = reader.ReadInt32();
            if(gambleHistoryCount == 0)
            {
                this.GambleCardHistory = new List<int>();
            }
            else
            {
                this.GambleCardHistory = new List<int>();
                for(int i = 0; i < gambleHistoryCount; i++)
                {
                    int history = reader.ReadInt32();
                    this.GambleCardHistory.Add(history);
                }
            }
            int remainCount = reader.ReadInt32();
            if (remainCount == 0)
            {
                this.RemainReponses = null;
            }
            else
            {
                this.RemainReponses = new List<BaseEGTActionToResponse>();
                for (int i = 0; i < remainCount; i++)
                {
                    EGTMessageType actionType = (EGTMessageType)(byte)reader.ReadByte();
                    string strResponse = (string)reader.ReadString();
                    this.RemainReponses.Add(new BaseEGTActionToResponse(actionType, strResponse));
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
            writer.Write(this.isPurchase);
            writer.Write(this.MoreBet);
            writer.Write(this.FeatureId);
            writer.Write(this.BetMultiplier);
            writer.Write((int)this.CurrencyInfo);
            writer.Write(this.FreespinRoundUsed);
            writer.Write(this.FreespinTotalRound);
            writer.Write(this.GambleType);
            writer.Write(this.GambleHalf);
            writer.Write(this.GambleRound);
            if(this.GambleCardHistory == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.GambleCardHistory.Count);
                for(int i = 0; i < this.GambleCardHistory.Count; i++)
                {
                    writer.Write(this.GambleCardHistory[i]);
                }
            }
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
