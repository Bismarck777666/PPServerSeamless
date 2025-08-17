using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SlotGamesNode.GameLogics
{
    public enum SpinTypes
    {
        NORMAL  = 0,
        FREE    = 1,
        FREELAST= 2,
    }
    public enum ActionTypes
    {
        NONE            = 0,
        DOSPIN          = 1,
        DOCOLLECT       = 2,
        DOBONUS         = 3,
        DOCOLLECTBONUS  = 4,
        DOFSOPTION      = 5,
        DOMYSTERY       = 6,
        DOFSBONUS       = 7,
        DOGAMBLEOPTION  = 8,
        DOGAMBLE        = 9,
    }
    public class BasePPSlotSpinData
    {
        public double       SpinOdd     { get; set; }
        public int          SpinType    { get; set; }
        public List<string> SpinStrings { get; set; }
        public bool         IsEvent     { get; set; }

        public BasePPSlotSpinData()
        {
            IsEvent = false;
        }

        public BasePPSlotSpinData(int spinType, double spinOdd, List<string> spinStrings)
        {
            SpinType    = spinType;
            SpinOdd     = spinOdd;
            SpinStrings = spinStrings;
            IsEvent     = false;
        }

        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(SpinType);
            writer.Write(SpinOdd);
            writer.Write(IsEvent);
            SerializeUtils.writeStringList(writer, SpinStrings);
        }

        public virtual void SerializeFrom(BinaryReader reader)
        {
            SpinType = reader.ReadInt32();
            SpinOdd = reader.ReadDouble();
            IsEvent = reader.ReadBoolean();
            SpinStrings = SerializeUtils.readStringList(reader);
        }
    }
    public class BasePPSlotSpinResult
    {
        public double           TotalWin            { get; set; }
        public ActionTypes      NextAction          { get; set; }
        public string           ResultString        { get; set; }
        public string           BonusResultString   { get; set; }
        public virtual double WinMoney
        {
            get
            {
                return TotalWin;
            }
        }
        public bool HasBonusResult
        {
            get
            {
                return !string.IsNullOrEmpty(BonusResultString);
            }
        }

        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(TotalWin);
            writer.Write((byte)NextAction);
            writer.Write(ResultString);
            writer.Write(BonusResultString);
        }

        public virtual void SerializeFrom(BinaryReader reader)
        {
            TotalWin            = reader.ReadDouble();
            NextAction          = (ActionTypes)reader.ReadByte();
            ResultString        = reader.ReadString();
            BonusResultString   = reader.ReadString();
        }

        public byte[] convertToByte()
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(output))
                    SerializeTo(writer);
                return output.ToArray();
            }
        }

        public BasePPSlotSpinResult()
        {
            ResultString        = "";
            BonusResultString   = "";
        }
    }
    public class BasePPActionToResponse
    {
        public ActionTypes  ActionType  { get; set; }
        public string       Response    { get; set; }

        public BasePPActionToResponse(ActionTypes actionType, string strResponse)
        {
            ActionType  = actionType;
            Response    = strResponse;
        }
    }
    public class BasePPSlotBetInfo
    {
        public float                        BetPerLine { get; set; }
        public int                          LineCount { get; set; }
        public bool                         PurchaseFree { get; set; }
        public bool                         MoreBet { get; set; }
        public string                       RoundID { get; set; }
        public string                       BetTransactionID { get; set; }
        public BasePPSlotSpinData           SpinData { get; set; }
        public List<BasePPActionToResponse> RemainReponses { get; set; }
        public bool HasRemainResponse
        {
            get
            {
                return RemainReponses != null && RemainReponses.Count > 0;
            }
        }
        public virtual float TotalBet
        {
            get
            {
                return BetPerLine * (float)LineCount;
            }
        }

        public BasePPSlotBetInfo()
        {
            PurchaseFree    = false;
            MoreBet         = false;
        }

        public BasePPActionToResponse pullRemainResponse()
        {
            if (RemainReponses == null || RemainReponses.Count == 0)
                return null;

            BasePPActionToResponse remainReponse = RemainReponses[0];
            RemainReponses.RemoveAt(0);
            
            return remainReponse;
        }

        public void pushFrontResponse(BasePPActionToResponse response)
        {
            RemainReponses.Insert(0, response);
        }

        public virtual void SerializeFrom(BinaryReader reader)
        {
            BetPerLine      = reader.ReadSingle();
            LineCount       = reader.ReadInt32();
            PurchaseFree    = reader.ReadBoolean();
            MoreBet         = reader.ReadBoolean();
            int cnt         = reader.ReadInt32();
            if (cnt == 0)
            {
                RemainReponses = null;
            }
            else
            {
                RemainReponses = new List<BasePPActionToResponse>();
                for (int i = 0; i < cnt; i++)
                    RemainReponses.Add(new BasePPActionToResponse((ActionTypes)reader.ReadByte(), reader.ReadString()));
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

            bool hasValue = reader.ReadBoolean();
            if (hasValue)
                RoundID = reader.ReadString();

            hasValue = reader.ReadBoolean();
            if (hasValue)
                BetTransactionID = reader.ReadString();
        }

        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(BetPerLine);
            writer.Write(LineCount);
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
                    writer.Write(RemainReponses[i].Response);
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

            if (RoundID == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(RoundID);
            }

            if (BetTransactionID == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(BetTransactionID);
            }
        }

        public byte[] convertToByte()
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(output))
                {
                    SerializeTo(writer);
                }

                return output.ToArray();
            }
        }
    }
    public class BasePPGroupedSlotBetInfo : BasePPSlotBetInfo
    {
        public Dictionary<double, SelectedGroupSequence> SeqPerBet { get; set; }
        public BasePPGroupedSlotBetInfo()
        {
            SeqPerBet = new Dictionary<double, SelectedGroupSequence>();
        }
        public void undoSequence()
        {
            double totalBet = Math.Round(this.TotalBet, 2);
            if (!SeqPerBet.ContainsKey(totalBet))
                return;

            SeqPerBet[totalBet].undoSample();
        }

        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.SeqPerBet.Count);
            foreach(KeyValuePair<double, SelectedGroupSequence> pair in this.SeqPerBet)
            {
                writer.Write(pair.Key);
                pair.Value.SerializeTo(writer);
            }
        }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            int count = reader.ReadInt32();
            this.SeqPerBet = new Dictionary<double, SelectedGroupSequence>();
            for(int i = 0; i < count; i++)
            {
                double                  key         = reader.ReadDouble();
                SelectedGroupSequence   selectedSeq = new SelectedGroupSequence();
                selectedSeq.SerializeFrom(reader);
                this.SeqPerBet.Add(key, selectedSeq);
            }
        }
    }
    public class SelectedGroupSequence
    {
        public int                          LastID      { get; set; }
        public List<GroupSequenceSample>    Samples     { get; set; }
        public double                       CreditMoney { get; set; }
        public bool IsEnded
        {
            get { return Samples.Count == LastID;}
        }

        public SelectedGroupSequence()
        {
            this.LastID     = 0;
            this.Samples    = new List<GroupSequenceSample>();
        }

        public void undoSample()
        {
            if (this.LastID > 0)
                this.LastID--;
        }
        public GroupSequenceSample nextSample()
        {
            if (Samples == null || LastID >= Samples.Count)
                return null;

            LastID++;
            return Samples[LastID - 1];
        }
        public void SerializeFrom(BinaryReader reader)
        {
            this.LastID = reader.ReadInt32();
            int count = reader.ReadInt32();
            this.Samples = new List<GroupSequenceSample>();
            for(int i = 0; i < count; i++)
            {
                GroupSequenceSample sample = new GroupSequenceSample();
                sample.SerializeFrom(reader);
                this.Samples.Add(sample);
            }
        }
        public void SerializeTo(BinaryWriter writer)
        {
            writer.Write(this.LastID);
            writer.Write(this.Samples.Count);
            for (int i = 0; i < this.Samples.Count; i++)
                this.Samples[i].SerializeTo(writer);
        }
    }
    public class GroupSequenceSample
    {
        public int Index                { get; set; }
        public int CumulatedGroupSum    { get; set; }
        public int Group                { get; set; }

        public GroupSequenceSample()
        {

        }
        public GroupSequenceSample(int index, int cumulatedGroupSum, int group)
        {
            Index               = index;
            CumulatedGroupSum   = cumulatedGroupSum;
            Group               = group;
        }
        public void SerializeFrom(BinaryReader reader)
        {
            Index               = reader.ReadInt32();
            CumulatedGroupSum   = reader.ReadInt32();
            Group               = reader.ReadInt32();
        }
        public void SerializeTo(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(CumulatedGroupSum);
            writer.Write(Group);
        }
    }
    public class BasePPHistory
    {
        public int                      error           { get; set; }
        public string                   description     { get; set; }
        public string                   init            { get; set; }
        public List<BasePPHistoryItem>  log             { get; set; }

        [JsonIgnore]
        public double bet       { get; set; }
        [JsonIgnore]
        public double baseBet   { get; set; }
        [JsonIgnore]
        public double win       { get; set; }
        [JsonIgnore]
        public double rtp       { get; set; }
        public BasePPHistory()
        {
            error       = 0;
            description = "OK";
            log         = new List<BasePPHistoryItem>();
        }

        public byte[] convertToByte()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    serializeTo(bw);
                }
                return ms.ToArray();
            }
        }
        public void serializeTo(BinaryWriter writer)
        {
            writer.Write(error);
            writer.Write(description);
            writer.Write(init);
            writer.Write(log.Count);
            for (int i = 0; i < log.Count; i++)
            {
                writer.Write(log[i].cr);
                writer.Write(log[i].sr);
            }
            writer.Write(bet);
            writer.Write(baseBet);
            writer.Write(win);
            writer.Write(rtp);
        }
        public void serializeFrom(BinaryReader reader)
        {
            error           = reader.ReadInt32();
            description     = reader.ReadString();
            init            = reader.ReadString();
            int logCount    = reader.ReadInt32();
            log.Clear();

            for (int i = 0; i < logCount; i++)
            {
                BasePPHistoryItem item = new BasePPHistoryItem();
                item.cr = reader.ReadString();
                item.sr = reader.ReadString();
                log.Add(item);
            }

            bet     = reader.ReadDouble();
            baseBet = reader.ReadDouble();
            win     = reader.ReadDouble();
            rtp     = reader.ReadDouble();
        }
    }
    public class BasePPHistoryItem
    {
        public string cr { get; set; }  //client request
        public string sr { get; set; }  //server response
    }

}
