using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SlotGamesNode.GameLogics
{
    public class BaseHabaneroActionToResponse
    {
        public HabaneroActionType   ActionType  { get; set; }
        public string               Response    { get; set; }

        public BaseHabaneroActionToResponse(HabaneroActionType actionType, string strResponse)
        {
            this.ActionType = actionType;
            this.Response   = strResponse;
        }
    }
    
    public class BaseHabaneroSlotBetInfo
    {
        public float    CoinValue           { get; set; }
        public int      BetLevel            { get; set; }
        public int      LineCount           { get; set; }
        public float    MiniBet             { get; set; }//읽기로부터 얻지않는 고유속성값
        public int      PurchaseFree        { get; set; }
        public int      MoreBet             { get; set; }
        public BasePPSlotSpinData SpinData  { get; set; }
        public List<BaseHabaneroActionToResponse> RemainReponses { get; set; }
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
                return MiniBet * CoinValue * BetLevel;
            }
        }
        public BaseHabaneroSlotBetInfo(float miniBet)
        {
            this.MiniBet = miniBet;
        }
        public BaseHabaneroActionToResponse pullRemainResponse()
        {
            if (RemainReponses == null || RemainReponses.Count == 0)
                return null;

            BaseHabaneroActionToResponse response = RemainReponses[0];
            RemainReponses.RemoveAt(0);
            return response;
        }
        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.CoinValue      = reader.ReadSingle();
            this.BetLevel       = reader.ReadInt32();
            this.LineCount      = reader.ReadInt32();
            this.MiniBet        = reader.ReadSingle();
            this.PurchaseFree   = reader.ReadInt32();
            this.MoreBet        = reader.ReadInt32();
            int remainCount = reader.ReadInt32();
            if (remainCount == 0)
            {
                this.RemainReponses = null;
            }
            else
            {
                this.RemainReponses = new List<BaseHabaneroActionToResponse>();
                for (int i = 0; i < remainCount; i++)
                {
                    HabaneroActionType actionType = (HabaneroActionType)(byte)reader.ReadByte();
                    string strResponse = (string)reader.ReadString();
                    this.RemainReponses.Add(new BaseHabaneroActionToResponse(actionType, strResponse));
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
        }
        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(this.CoinValue);
            writer.Write(this.BetLevel);
            writer.Write(this.LineCount);
            writer.Write(this.MiniBet);
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

    public class BaseHabaneroSlotSpinResult
    {
        public double               TotalWin                { get; set; }
        public HabaneroActionType   CurrentAction           { get; set; }
        public HabaneroActionType   NextAction              { get; set; }
        public string               ResultString            { get; set; }
        public string               GameId                  { get; set; }
        public string               RoundId                 { get; set; }
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
            writer.Write((byte)this.CurrentAction);
            writer.Write((byte)this.NextAction);
            writer.Write(this.ResultString);
            writer.Write(this.GameId);
            writer.Write(this.RoundId);
        }
        public virtual void SerializeFrom(BinaryReader reader)
        {
            this.TotalWin       = reader.ReadDouble();
            this.CurrentAction  = (HabaneroActionType)reader.ReadByte();
            this.NextAction     = (HabaneroActionType)reader.ReadByte();
            this.ResultString   = reader.ReadString();
            this.GameId         = reader.ReadString();
            this.RoundId        = reader.ReadString();
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
        public BaseHabaneroSlotSpinResult()
        {
            this.ResultString = "";
        }
    }

    public enum HabaneroActionType
    {
        NONE                    = 0,
        MAIN                    = 1,
        FREEGAME                = 2,
        BONUS                   = 2,
        RESPIN                  = 3,
        PICKOPTION              = 4,
        FREEGAME0               = 5,
        PENGUINFREEGAME         = 6,
        POLARBEARFREEGAME       = 7,
        PLANETFREEGAME          = 8,
        BONUSFREEGAME           = 9,
        BONUS1                  = 18,
        BONUS2                  = 19,
        BONUS3                  = 20,
        BONUS4                  = 21,
        BONUS5                  = 22,
        BONUS1RESPIN            = 24,
        BONUS2RESPIN            = 25,
        BONUS3RESPIN            = 26,
        BONUS4RESPIN            = 27,
        SYMBOLPICK              = 100,
        FG_HANS                 = 101,
        FG_HEIDI                = 102,
        MAINCASECADE            = 103,
        FREEGAMECASCADE         = 104,
        CHEST                   = 105,
        HOLLYWOODSTARPICK       = 106,
        HOLLYWOODSTARFREEGAME   = 107,
    }

    public class HabaneroHistoryItem
    {
        public string GameId            { get; set; }
        public string RoundId           { get; set; }
        public List<HabaneroHistoryResponses> Responses  { get; set; }
        public HabaneroHistoryItem()
        {
            this.GameId     = "";
            this.RoundId    = "";
            this.Responses = new List<HabaneroHistoryResponses>();
        }
        public void SerializeFrom(BinaryReader reader)
        {
            this.GameId     = reader.ReadString();
            this.RoundId    = reader.ReadString();
            this.Responses  = new List<HabaneroHistoryResponses>();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                HabaneroHistoryResponses responseHistory = new HabaneroHistoryResponses();
                responseHistory.SerializeFrom(reader);
                this.Responses.Add(responseHistory);
            }
        }
        public void SerializeTo(BinaryWriter writer)
        {
            writer.Write(this.GameId);
            writer.Write(this.RoundId);
            if (this.Responses == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(this.Responses.Count);
                for (int i = 0; i < this.Responses.Count; i++)
                    this.Responses[i].SerializeTo(writer);
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

    public class HabaneroHistoryResponses
    {
        public HabaneroActionType   Action      { get; set; }
        public DateTime             Time        { get; set; }
        public string               Response    { get; set; }
        public HabaneroHistoryResponses()
        {

        }
        public HabaneroHistoryResponses(HabaneroActionType action, DateTime time, string strResponse)
        {
            this.Action     = action;
            this.Time       = time;
            this.Response   = strResponse;
        }
        public void SerializeTo(BinaryWriter writer)
        {
            writer.Write((int)this.Action);
            writer.Write(this.Time.ToString());
            writer.Write(this.Response);
        }
        public void SerializeFrom(BinaryReader reader)
        {
            this.Action     = (HabaneroActionType)(int)reader.ReadInt32();
            this.Time       = DateTime.Parse(reader.ReadString());
            this.Response   = reader.ReadString();
        }
    }

    public class HabaneroLogItem
    {
        public int      AgentID         { get; set; }
        public string   UserID          { get; set; }
        public int      GameID          { get; set; }
        public DateTime Time            { get; set; }
        public string   RoundID         { get; set; }
        public string   GameLogID       { get; set; }
        public double   Bet             { get; set; }
        public double   Win             { get; set; }
        public string   Overview        { get; set; }
        public string   Detail          { get; set; }
        public HabaneroLogItem()
        {

        }
        public HabaneroLogItem(HabaneroHistoryItem history)
        {
        }
    }

    public class HabaneroLogItemOverview
    {
        public bool     IsTestSite          { get; set; }
        public long     DateToShow          { get; set; }
        public string   DtCompleted         { get; set; }
        public long     FriendlyId          { get; set; }
        public string   GameInstanceId      { get; set; }
        public double   RealPayout          { get; set; }
        public double   RealStake           { get; set; }
        public string   CurrencyCode        { get; set; }
        public int      GameStateId         { get; set; }
        public string   GameKeyName         { get; set; }
        public object   ExtRoundId          { get; set; }
        public int      Exponent            { get; set; }
        public string   ReplayURL           { get; set; }
        public string   BrandGameId         { get; set; }
        public bool     IsSpecialBrandGame  { get; set; }
        public int      GameTypeId          { get; set; }
    }

    public class HabaneroReelAndSymbolIndex
    {
        public int reelindex    { get; set; }
        public int symbolindex  { get; set; }
    }
}
