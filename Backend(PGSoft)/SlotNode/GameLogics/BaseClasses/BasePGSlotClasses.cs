using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using PCGSharp;
using static System.Net.WebRequestMethods;
using MongoDB.Bson;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class BasePGSlotSpinData
    {
        public double       SpinOdd         { get; set; }
        public int          SpinType        { get; set; } 
        public List<string> SpinStrings     { get; set; }
        public bool         IsEvent         { get; set; }

        public BasePGSlotSpinData()
        {
            this.IsEvent = false;
        }

        public BasePGSlotSpinData(int spinType, double spinOdd,List<string> spinStrings)
        {
            this.SpinType       = spinType;
            this.SpinOdd        = spinOdd;
            this.SpinStrings    = spinStrings;
            this.IsEvent        = false;
        }

        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(this.SpinType);
            writer.Write(this.SpinOdd);
            writer.Write(this.IsEvent);
            SerializeUtils.writeStringList(writer, this.SpinStrings);
        }
        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.SpinType = reader.ReadInt32();
            this.SpinOdd = reader.ReadDouble();
            this.IsEvent = reader.ReadBoolean();
            this.SpinStrings = SerializeUtils.readStringList(reader);
        }
    }
    public class BasePGSlotSpinResult
    {
        public double               TotalWin            { get; set; }
        public string               ResultString        { get; set; }      
        public virtual double   WinMoney
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
            
        }
        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.TotalWin           = reader.ReadDouble();
            this.ResultString       = reader.ReadString();
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
        public BasePGSlotSpinResult()
        {
            this.ResultString       = "";
        }
    }
    public class BasePGResponse
    {
        public string Response { get; set; }

        public BasePGResponse(string strResponse)
        {
            this.Response = strResponse;
        }
    }
    public class BasePGSlotBetInfo
    {
        public float    BetSize             { get; set; }
        public int      BetLevel            { get; set; }
        public int      BaseBet             { get; set; }
        public bool     PurchaseFree        { get; set; }        
        public long     TransactionID       { get; set; }
        public string   RoundID             { get; set; }
        public string   BetTransactionID    { get; set; }

        public BasePGSlotSpinData           SpinData        { get; set; }
        public List<BasePGResponse>         RemainReponses  { get; set; }

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
                return BetSize * BetLevel * BaseBet;
            }
        }
        public BasePGSlotBetInfo(int baseBet)
        {
            this.BaseBet        = baseBet;
            this.PurchaseFree   = false;
        }
        public BasePGResponse pullRemainResponse()
        {
            if (RemainReponses == null || RemainReponses.Count == 0)
                return null;

            BasePGResponse response = RemainReponses[0];
            RemainReponses.RemoveAt(0);
            return response;
        }

        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.BetSize        = reader.ReadSingle();
            this.BetLevel       = reader.ReadInt32();
            this.BaseBet        = reader.ReadInt32();
            this.TransactionID  = reader.ReadInt64();
            this.PurchaseFree   = reader.ReadBoolean();
            int remainCount     = reader.ReadInt32();
            if (remainCount == 0)
            {
                this.RemainReponses = null;
            }
            else
            {
                this.RemainReponses = new List<BasePGResponse>();
                for(int i = 0; i < remainCount; i++)
                {
                    string      strResponse = (string)reader.ReadString();
                    this.RemainReponses.Add(new BasePGResponse(strResponse));
                }
            }
            int spinDataType = reader.ReadInt32();
            if (spinDataType == 0)
            {
                this.SpinData = null;
            }
            else if (spinDataType == 1)
            {
                this.SpinData = new BasePGSlotSpinData();
                this.SpinData.SerializeFrom(reader);
            }
            else if(spinDataType == 2)
            {                
                this.SpinData = new BasePGSlotStartSpinData();
                this.SpinData.SerializeFrom(reader);
            }
            bool hasValue = reader.ReadBoolean();
            if(hasValue)
                this.BetTransactionID   = reader.ReadString();

            hasValue = reader.ReadBoolean();
            if(hasValue)
                this.RoundID            = reader.ReadString();
        }
        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(this.BetSize);
            writer.Write(this.BetLevel);
            writer.Write(this.BaseBet);
            writer.Write(this.TransactionID);
            writer.Write(this.PurchaseFree);
            if (this.RemainReponses == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.RemainReponses.Count);
                for (int i = 0; i < this.RemainReponses.Count; i++)
                    writer.Write((string)this.RemainReponses[i].Response);
            }
            if (this.SpinData == null)
                writer.Write(0);
            else if (this.SpinData is BasePGSlotStartSpinData)
                writer.Write(2);
            else
                writer.Write(1);

            if (this.SpinData != null)
                this.SpinData.SerializeTo(writer);

            if (this.BetTransactionID == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(this.BetTransactionID);
            }
            if (this.RoundID == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(this.RoundID);
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
    public class BasePGHistory
    {
        public string       cc      { get; set; }
        public long         bt      { get; set; }
        public int          fscc    { get; set; }
        public List<int>    ge      { get; set; }
        public int          gid     { get; set; }
        public double       gtba    { get; set; }
        public double       gtwla   { get; set; }
        public int          mgcc    { get; set; }
        public string       tid     { get; set; }
        public List<BasePGHistoryItem> bd { get; set; }
        public BasePGHistory(int gameID)
        {
            this.gid    = gameID;
            this.ge     = new List<int>();
            this.bd     = new List<BasePGHistoryItem>();
            this.tid    = "";
        }
        public void doSummary()
        {
            this.bt     = this.bd[0].bt;
            this.tid    = this.bd[0].tid;
            this.gtba   = 0.0;
            this.gtwla  = 0.0;
            for(int i = 0; i < this.bd.Count; i++)
            {
                this.gtba  += this.bd[i].tba;
                this.gtwla += this.bd[i].twla;
            }
            for(int i = 0; i < this.bd.Count; i++)
            {
                if(!BasePGSlotGame.IsNullOrEmpty(this.bd[i].gd["ge"]))
                {
                    List<int> gameElements = JsonConvert.DeserializeObject<List<int>>(this.bd[i].gd["ge"].ToString());
                    for(int j = 0; j < gameElements.Count; j++)
                    {
                        if (!this.ge.Contains(gameElements[j]))
                            this.ge.Add(gameElements[j]);
                    }
                }
            }
        }
        public byte[] convertToByte()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    this.serializeTo(bw);
                }
                return ms.ToArray();
            }
        }
        public void serializeTo(BinaryWriter writer)
        {
            writer.Write(this.cc);
            writer.Write(this.bt);
            writer.Write(this.fscc);
            writer.Write(this.ge.Count);
            for (int i = 0; i < this.ge.Count; i++)
                writer.Write(this.ge[i]);

            writer.Write(gid);
            writer.Write(gtba);
            writer.Write(gtwla);
            writer.Write(mgcc);
            writer.Write(tid);
            writer.Write(this.bd.Count);
            for (int i = 0; i < this.bd.Count; i++)
                writer.Write(JsonConvert.SerializeObject(this.bd[i]));
        }
        public void serializeFrom(BinaryReader reader)
        {
            this.cc = reader.ReadString();
            this.bt = reader.ReadInt64();
            this.fscc = reader.ReadInt32();
            int geCount = reader.ReadInt32();
            for (int i = 0; i < geCount; i++)
                this.ge.Add(reader.ReadInt32());
            this.gid = reader.ReadInt32();
            this.gtba = reader.ReadDouble();
            this.gtwla = reader.ReadDouble();
            this.mgcc = reader.ReadInt32();
            this.tid = reader.ReadString();
            int bdCount = reader.ReadInt32();
            for (int i = 0; i < bdCount; i++)
                this.bd.Add(JsonConvert.DeserializeObject<BasePGHistoryItem>(reader.ReadString()));
        }
    }
    public class BasePGHistoryItem
    {
        public double   bl      { get; set; }
        public long     bt      { get; set; }
        public double   tba     { get; set; }
        public double   twla    { get; set; }
        public string   tid     { get; set; }        
        public dynamic  gd      { get; set; }

        public BasePGHistoryItem()
        {

        }
        public BasePGHistoryItem(double balance, long timestamp, double totalBet, double totalWin, string tid, dynamic gd)
        {
            this.bl     = balance;
            this.bt     = timestamp;
            this.tba    = totalBet;
            this.twla   = totalWin - totalBet;
            this.tid    = tid;
            this.gd     = gd;
        }
    }

    public class BasePGSlotStartSpinData : BasePGSlotSpinData
    {
        public double               StartOdd        { get; set; }
        public int                  FreeSpinGroup   { get; set; }
        public List<BsonDocument>   FreeSpins       { get; set; }
        public double               MaxOdd          { get; set; }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.StartOdd       = reader.ReadDouble();
            this.FreeSpinGroup  = reader.ReadInt32();
            this.MaxOdd         = reader.ReadDouble();

            this.FreeSpins = new List<BsonDocument>();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
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

}
