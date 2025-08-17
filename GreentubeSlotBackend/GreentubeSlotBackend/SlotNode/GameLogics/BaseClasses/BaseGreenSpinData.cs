using GITProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics.BaseClasses
{
    public class BaseGreenSpinData
    {
        public double SpinOdd { get; set; }
        public int SpinType { get; set; } 
        public List<string> SpinStrings { get; set; }
        public BaseGreenSpinData() { }
    }

    public class BaseGreenSlotSpinResult
    {
        public GreenMessageType Action { get; set; }
        public double TotalWin { get; set; }
        public double CollectWin {  get; set; } // only for freespin
        public string ResultString { get; set; }
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
            writer.Write((int)this.Action);
        }
        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.TotalWin = reader.ReadDouble();
            this.CollectWin = reader.ReadDouble();
            this.ResultString = reader.ReadString();
            this.Action = (GreenMessageType)reader.ReadInt32();
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
            this.ResultString = "";
        }
    }
    public class BaseGreenActionToResponse
    {
        public GreenMessageType ActionType { get; set; }
        public string Response { get; set; }

        public BaseGreenActionToResponse(GreenMessageType actionType, string strResponse)
        {
            this.ActionType = actionType;
            this.Response = strResponse;
        }
    }
    public class BaseGreenSlotBetInfo
    {
        public int PlayLine { get; set; } 
        public int PlayBet { get; set; } 
        public int PurchaseStep { get; set; } 
        public int MoreBet { get; set; }   
        public Currencies CurrencyInfo { get; set; }   
        public int BonusOption { get; set; }    
        public int GambleType { get; set; }  
        public bool GambleHalf { get; set; }   
        public int GambleRound { get; set; }
        public double GambleInitTotalWin { get; set; }
        public double GambleInitCollectWin { get; set; }

        public string RoundID { get; set; }
        public string BetTransactionID { get; set; }
        public BasePPSlotSpinData SpinData { get; set; }
        public string SpinInfo { get; set; }
        public List<string> GambleCardHistory { get; set; }
        public List<BaseGreenActionToResponse> RemainReponses { get; set; }

        public virtual int RelativeTotalBet => PlayLine * 1;
        public bool isPurchase => PurchaseStep > -1;
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

        public BaseGreenSlotBetInfo()
        {
            GambleCardHistory = new List<string>();
            BonusOption = -1;
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
            this.PlayLine = reader.ReadInt32();
            this.PlayBet = reader.ReadInt32();
            this.PurchaseStep = reader.ReadInt32();
            this.MoreBet = reader.ReadInt32();
            this.CurrencyInfo = (Currencies)reader.ReadInt32();
            this.GambleType = reader.ReadInt32();
            this.GambleHalf = reader.ReadBoolean();
            int gambleHistoryCount = reader.ReadInt32();
            if(gambleHistoryCount == 0)
            {
                this.GambleCardHistory = new List<string>();
            } else
            {
                this.GambleCardHistory = new List<string>();
                for(int i = 0; i < gambleHistoryCount; i++)
                {
                    string history = (string)reader.ReadString();
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
                this.RemainReponses = new List<BaseGreenActionToResponse>();
                for (int i = 0; i < remainCount; i++)
                {
                    GreenMessageType actionType = (GreenMessageType)(byte)reader.ReadByte();
                    string strResponse = (string)reader.ReadString();
                    this.RemainReponses.Add(new BaseGreenActionToResponse(actionType, strResponse));
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
            writer.Write(this.PurchaseStep);
            writer.Write(this.MoreBet);
            writer.Write((int)this.CurrencyInfo);
            writer.Write(this.GambleType);
            writer.Write(this.GambleHalf);
            if(this.GambleCardHistory == null)
            {
                writer.Write(0);
            } else
            {
                writer.Write(this.GambleCardHistory.Count);
                for(int i = 0; i < this.GambleCardHistory.Count; i++)
                {
                    writer.Write((string)this.GambleCardHistory[i]);
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
