using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using PCGSharp;
using MongoDB.Bson;

namespace SlotGamesNode.GameLogics
{
    public class BasePPSlotSpinData
    {
        public double       SpinOdd         { get; set; }
        public int          SpinType        { get; set; } //0:일반스핀, 1:시작프리스핀 + 프리스핀, 100:시작프리스핀, 200, 201, 202...: 그에 상응한 프리스핀조합 
        public List<string> SpinStrings     { get; set; }
        public bool         IsEvent         { get; set; }

        public BasePPSlotSpinData()
        {
            this.IsEvent = false;
        }

        public BasePPSlotSpinData(int spinType, double spinOdd,List<string> spinStrings)
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
}
