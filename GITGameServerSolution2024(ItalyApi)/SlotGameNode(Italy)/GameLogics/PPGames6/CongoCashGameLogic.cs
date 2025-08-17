using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Akka.Actor;
using SlotGamesNode.Database;

namespace SlotGamesNode.GameLogics
{
    public class CongoCashAccV
    {
        public string               Patterns        { get; set; }
        public string               Multiples       { get; set; }        
        public string               FSCounts        { get; set; }
        public CongoCashSpinData    ReservedSpin    { get; set; }

        public CongoCashAccV()
        {
            this.Patterns  = "";
            this.Multiples = "";
            this.FSCounts  = "";
        }
        public void SerializeFrom(BinaryReader reader)
        {
            this.Patterns   = reader.ReadString();
            this.Multiples  = reader.ReadString();
            this.FSCounts   = reader.ReadString();
            bool hasReservedSpin = reader.ReadBoolean();
            if(hasReservedSpin)
            {
                this.ReservedSpin = new CongoCashSpinData();
                this.ReservedSpin.SerializeFrom(reader);
            }
        }
        public void SerializeTo(BinaryWriter writer)
        {
            writer.Write(this.Patterns);
            writer.Write(this.Multiples);
            writer.Write(this.FSCounts);
            if(this.ReservedSpin == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                this.ReservedSpin.SerializeTo(writer);
            }
        }
    }
    public class CongoCashBetInfo : BasePPSlotBetInfo
    {
        public Dictionary<double, CongoCashAccV> AccvPerBets { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.AccvPerBets = new Dictionary<double, CongoCashAccV>();
            int count = reader.ReadInt32();
            for(int i = 0; i < count; i++)
            {
                double          bet     = reader.ReadDouble();
                CongoCashAccV   accv    = new CongoCashAccV();
                accv.SerializeFrom(reader);
                this.AccvPerBets[bet] = accv;
            }
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            if(this.AccvPerBets == null)
            {
                writer.Write((int)0);
                return;
            }
            writer.Write(this.AccvPerBets.Count);
            foreach(KeyValuePair<double, CongoCashAccV> kvp in this.AccvPerBets)
            {
                writer.Write(kvp.Key);
                kvp.Value.SerializeTo(writer);
            }
        }
    }
    public class CongoCashSpinData : BasePPSlotSpinData
    {
        public bool                 IsSpecialReel       { get; set; }
        public bool                 IsReservedSpin      { get; set; }
        public CongoCashSpinData    ReservedSpin        { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.IsSpecialReel   = reader.ReadBoolean();
            this.IsReservedSpin  = reader.ReadBoolean();
            bool hasReservedSpin = reader.ReadBoolean();
            if(hasReservedSpin)
            {
                this.ReservedSpin = new CongoCashSpinData();
                this.ReservedSpin.SerializeFrom(reader);
            }
            else
            {
                this.ReservedSpin = null;
            }
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.IsSpecialReel);
            writer.Write(this.IsReservedSpin);
            if (this.ReservedSpin == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                this.ReservedSpin.SerializeTo(writer);
            }
        }
        public CongoCashSpinData()
        {

        }
        public CongoCashSpinData(int spinType, double spinOdd, List<string> spinStrings, bool isSpecial) : base(spinType, spinOdd, spinStrings)
        {
            this.IsSpecialReel = isSpecial;
        }
    }
    public class CongoCashGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs432congocash";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return true;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 20;
            }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
        }
        protected override int ROWS
        {
            get
            {
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=6,7,4,10,8,9,8,5,6,7,8,6,7,3,9,10,4,3,7,8&nas=15&cfgs=3868&accm=def~jwt~ma~fgc~h&ver=2&acci=0&def_sb=10,11,3,4,7&reel_set_size=9&def_sa=8,7,5,6,5&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"4224757\",jwt_jp:\"jp1,jp2,jp3\",max_rnd_win:\"2200\",ma_jp:\"40000,2000,500\"}}&wl_i=tbm~2200&sc=10.00,20.00,30.00,50.00,80.00,100.00,200.00,400.00,500.00,800.00,1200.00,2000.00,3000.00,5000.00&defc=100.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,50,20,10,0;50,20,10,0,0;40,20,10,0,0;20,15,5,0,0;20,15,5,0,0;15,10,5,0,0;15,10,5,0,0;15,10,5,0,0;15,10,5,0,0;15,10,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=8,13,13,13,13,10,11,5,3,6,12,1,9,4,7,13,10,5,13,7,3,7,5,11,7,13,12,13,11,5,12,13,10,7,10,12,13,12,13,12,5,13,10,7,11,10,13,12,13,5,7,12,13,10,5,10,6,13,5,13,7,13,5,11,10,6,3,13,10,13,12,9,13,11,13,11,13,12,11,10,12,13,12,10,12,10,12,5,10,7,5,7,10,7,3,10,13,10,5,10,11,12,3,10~5,13,13,13,7,12,13,10,8,3,9,6,11,2,4,14,13,10,2,13,2,7,11,6,8,2,13,8,10,3,12,8,13,12,2,6,10,2,13,3,8,13,2,11,10,2,8,2,11,8,3,10,11,2,3,13,2,13,2,14,3,13,8,10,12,2,11,7,11,13,10,2,13,8,12,8,13,11,12,13,14,8,10,13,12,6,13,4,13,11,14,3,6,11,12,10,8,2,14,10,6,10,2,13,11,12,10,8,14,9,3,8,10,2,13,12,2,9,7,2,11,14,13,10,4,6,9,13,11~13,13,13,13,10,2,3,12,4,7,6,5,11,14,8,9,11,10,2,3,12,11,12,14,8,12,2,11,6,11,14,10,2,14,12,6,12,3,12,11,12,14,3,11,14,12,14,12,8,12,3,14,12,8,14,3,4,8,10,8,12,6,2,3,6,14,8,5~2,10,3,13,13,13,4,5,9,7,8,6,14,13,12,11,14,3,12,14,3,13,12,13,8,4,7,5,6,14,4,8,7,12,6,7,14,11,14,11,6,13,3,14,6,3,9,13,14,11,9,4,7,3,6,11,13,14,3,13,7,4,13,14,10,13,6,13,11,3,8,12,14,10,3,13,12,13,14,6,13,14,4,14,4,7,6,5~10,3,4,8,1,12,13,9,7,6,11,2,5,3,9,3,2,8,2,11,3,7,6,8,3,2,3,6,11,12,3,12,5,6,12,9,4,3,12,3,7,4,2,9,8,7,5&accInit=[{id:0,mask:\"jwt;ma;fgc\",fgc:\"15,12,10,8,0,0,0,0,0,0,0,0,0,0,0,20,0,0,0,100,50,0,0,0,0\",ma:\"20,20,20,20,2000,500,1000,500,400,300,240,200,150,125,100,20,800,600,320,20,20,40000,4000,2000,1500\",jwt:\"fg,fg,fg,fg,jp2,jp3,cv,cv,cv,cv,cv,cv,cv,cv,cv,fg,cv,cv,cv,fg,fg,jp1,cv,cv,cv\"},{id:1,mask:\"jwt;ma\",ma:\"2000,500,1000,500,400,300,240,200,150,125,100,800,600,320,40000,4000,2000,1500\",jwt:\"jp2,jp3,cv,cv,cv,cv,cv,cv,cv,cv,cv,cv,cv,cv,jp1,cv,cv,cv\"}]&reel_set2=3,11,12,7,10,1,6,5,9,4,8,12,5,9,8,11,5,12,9,12,1,7,6,5,11,6,12,1,6,10,7,4,12,7,12,5,10,4,11,9,7,5,8,9,12,9,7,9,12,9,5,11,12,5,12,1,11,5,12,9~2,2,2,5,3,9,11,14,4,2,10,12,7,8,6,3,12,10,5,11,5,10,11,5,10,11,5,12,11,6~14,12,4,9,10,8,5,11,6,2,7,3,11,5,11,2,3,10,11,7,2,11,8,11,3,8,10,2,11,7,2,6,5,2,5,8,4,11,2,7,4,6,12,2,10~2,5,8,7,9,3,11,12,10,14,4,6,14,6,14,5,14,11,9,11,4,14,11,9,11,14,11,8,14,9,14,9,11,12,10,14,12,9,14,6,14,9,5,8,11,9,12,6,14,6,14,11,9,7,8,5,11,14~2,4,1,6,8,5,9,12,7,11,10,3,1,3,11,4,7,4,7,6,7,11,10,8,7,1,11,6,11,7,10,11,7,11,3,6,4,7,10,1,6,1,8,1,3,6,4,10,6,7,6,10,7,4,3,1,6,8,6,10,3,11,10,1,3&t=243&reel_set1=4,11,13,13,13,6,8,7,9,5,12,13,3,1,10,5,13,7,10,7,13,5,10,13,5,13,6,13,10,13,5,6,8,6,13,5,1,5,7,3,8,7,5~8,13,13,13,6,5,3,11,2,7,10,14,13,4,12,9,2,10,13,2,13,5,13,3,4,2,7,13,10,7,2,7,6,13,11,3,13,11,3,13,9,13,2,4,13,7,13,6,2,13,2,13,7,14,11,7,11,4,7,13,2,7,13,3,2,13,7,3~5,3,13,13,13,2,14,12,8,11,13,6,10,4,9,7,8,12,11,12,2,10,11,13,14,11,2,11,8,13,12,14,11,8,4,2,10,4,13,2,10,14,10,2,11,13,8,10,11,13,11,10,6,11,12,2,3,12,10,8,7,14,13,3,12,2,11,10,11,8,13,2,11,12,8,12,13,8,14,2,10,8,10,3,8,10,13,10,8,2~13,13,13,3,10,12,9,8,13,7,11,5,14,4,6,2,5,3,11,5,6,7,14,3,8,5,3,5,10,8,6,11,5,11,5,3,5,7,11,7,8,11,8,14,10,5,9,3,11,9,14,3,7,6,11,3,4,14,5,3,11,3,7,11,8,9,3,5,11,10,4,5,14,8,11,5,14,8,3,11,10,3,8,11,8~1,3,4,7,11,5,2,13,12,12,12,9,12,8,10,6,12,5,2,8,12,11,3,2,6,2,5,8,6,5,12,2,4,2,12,5,12,2,12,6,10,5,12,8,6,2,12,11,12,2,12,2,12,2,9,5,12,8,11,12,9,2,5,12,2,12,6,11,6,12,8,3,2,4,6,12,5,12,3,6,12,6,12,10,4,12,11,12,9,5,12,2,3,12,6,3,5,8,12,2,11,2,12,7,11,4,5,12,3,4,6,4,3,11,12,3,11,12,6,3,12,4,6,12,8,5,2,12,3,12,10,3,2&reel_set4=8,7,4,5,1,9,10,6,11,3,12,7,5,3,11,5,3,5,10,1,7,3,7,12,5~12,11,5,10,6,9,2,7,4,3,14,8,3,8,2,3,4,6,5,4~9,2,3,14,10,4,5,11,7,6,8,12,14,4,8,4,14,2,3,2,14,8,2,7,14,7,11,2,4,2,10,2,14,2,14,11,7,14,6,7,2,10,14,8,2,8,2,14,7,14,8,10,6,4,7,2,14,2,7,6,14,8,14,2,8,14,2,14,4,10~12,2,2,2,7,14,3,9,5,2,6,8,10,4,11,2,7,2,8,4,6,8,4,7,10,11,4,8,2~10,12,8,5,3,11,4,2,7,1,9,6,4,8,4,6,2,8,4,6,1,8,2,4,1,6,8,6,4,1,2,12,8,6,1,12,8,6,3,9,11,2,3,12,11,12,6,12,1,11,1,8,4,8,12,7,4,8,1,11,1,8,6,5,8,1,8,2,8,1,12,6,4,11,6,3&reel_set3=7,11,8,5,6,4,3,1,9,10,12,4,5,1,11,1,4,10,11,10,5,11,5,11,4,5,10,11,10,1,5,10,1~14,10,4,8,2,6,5,7,9,12,11,3,12,2,8,9,12,2,9,6,9,8,6,9,4,12,2,10,2,10,12,8,12,9,10,3,8,12,9,10,6,2,12,2,9,2,6,8,5,10,2,12,2,9,8,9,11,2,9,6,2,4,5,2,8,2,9,6,12,3,9,10,3,9,12,9~5,3,8,2,2,2,2,6,10,4,12,7,9,11,14,10,6,12,6,2,14,6,2,7,14,6,2,8,12,6,2,7,14,2,14,4,6,8,2,4,6,7,8,14,2,6,3~10,8,2,9,3,5,6,11,4,14,7,12,5,8,2,11,9,11,2,11,9,2,11,9,2,11,12,11,9,4,9,11,14,2,4,2,5,2,3,11,4,12,2,12,2,12,11,4,8,9,4,3,11,4,3,4,5,9,11,9,2,4,3,2,7,11,9,11,2,4,9,2,11,3,11,2,11,9,11,4~10,6,12,3,5,8,4,1,2,7,11,9,12,5,7,11,12,1,8,5,1,11,6,5,11,9,1,6,1,8,12,6,5,1,12,7,9,5&reel_set6=3,5,9,6,4,1,7,10,8,12,11,9,7,11,4,7,5,9,7,8,4,12,4,7,10,12,4,12,11,4,12,10,7,12,7,12,7,1,4,9,8,12,8,4,7,4,12,8,1,12,4,12,7~2,2,2,6,9,3,2,8,14,4,10,5,11,7,12,9,5,7,14,5,7~12,7,5,11,14,6,3,4,2,9,10,8,6,5,4,2,4,5,4,6,2,4,5,4,5,2,9,5,7,9,5,3,11,5,9,14,4,6,11,3,8,14,3,7,9,4,5,9,5,6,5,4,6,4,7,3,4,7,11,2,9,4,5,14,7,9,2,4,5,9,6,9,3,4,6,5,3,6,4~5,2,2,2,4,12,2,8,7,9,11,3,6,14,10,12,2,4,3,12,2,12,2,10,2,12,8,2,4,8,6,2,4~11,7,1,3,5,9,2,12,4,6,8,10,2,1,9,4,6,8,1,2,4,9,2,9,2,12,9,8,4,10,12,6,2&reel_set5=8,12,10,7,3,5,11,6,1,4,9,12,9,5,4,7,12,7,3,7,12,7,3,7,12,3,12,5,7,12,7,12,9,3,7,5,7,12,3,12,7~2,2,2,2,11,14,5,10,7,6,8,3,12,4,9,11,6,11,4~14,2,2,2,6,7,11,12,2,4,10,3,5,9,8,2,7,10,4,8,2,5,2,10,8,12,8,10,11,7,2,11,5,4,5,4,11,12,2,5,12,2,4,11,5,12~5,14,2,3,4,12,9,10,11,7,8,6,2,14,2,4,12,7,2,14,7,2,7,12,7,2,12,2,12,7,12,3,12,4,2,11,7,2,6,7,2,7,12,7,11,7,2,7,11,12,7,12,11,2,7,4,2,8,7,4,11,7,4,7,4,7,12,8,11,12,7,4,2,11,2,11,12~11,3,6,4,8,9,5,12,1,2,7,10,6,2,10,6,10,6,5,10,5,7,6,5,10,4,5,12,2,6,7,2,9,6,2,5,7,10,12,5,6,10,6,5,2,9,5,1,2,10,7,9,2,4,2,12,6,9,5,9,7,5,10,9,10,2,6,7,5,9,6,7,5,8,5,2,6,7,4,2,6,5,10,6&reel_set8=4,10,6,9,12,1,11,7,3,8,5,3,5,12,11,9,5,9,5,9,1,5,1,9,7,9,5,12,1,8,11,6,11,5,10,3,6,11,8,1,5,7,9,11,9,11,1,9,5,9,1,12,5,3,12,9,11,3,5,1,5,11,7,5,12,9,5,9,11,10,5,9,10,6,11,5,3,9,5,9,10,11,9,5,9,5~11,2,2,2,3,7,6,12,5,2,8,14,9,4,10,2,9,3,7,2~2,2,2,8,11,6,10,4,14,12,9,3,7,2,5,4,9,5,8,6,3,7,4,8,4,9,4,10,8,6,5,3,8,12,6,5,3,12,14,10,7,5,8,12,4,8,6~2,2,2,9,5,14,11,12,10,6,8,7,2,4,3,4,6,8,3,7,4,3,11,6,3,5,11,14,12,9,3~9,1,3,6,10,7,8,12,2,11,5,4,6,12,5,10,8,12,6,10,1,10,5,8,12,8,2,6,12,5,4,6,7,6&reel_set7=4,7,10,3,1,5,6,12,8,11,9,8,6,5,11,8,11,3,1,6,1,10,12,10,12~6,3,14,4,2,7,8,10,9,12,11,5,10,9,10,4,10,14,9,7,10,5,9,14,3,9,5,10,9,14,10,11,10,11,8,5,14,5~5,2,2,2,14,12,6,10,7,2,8,4,3,11,9,12,2,9~7,2,2,2,2,10,12,11,8,5,3,9,6,4,14,2,11,4,5~3,2,5,4,9,1,11,12,8,6,7,10,8,11,9,8,6,11,10,9,5,11,12,5,11,7,8,2,6,11,8,1,6,10,11,8,6,11,12,5,6,12,8,11,7,6,1,12,7,11,8,10,11,7,11,6,12,8,11,8,1,10,6,1,10,6,5,4,11,1,9,5,11,12,8,6,7,6,12,8";
            }
        }
        #endregion
        public CongoCashGameLogic()
        {
            _gameID = GAMEID.CongoCash;
            GameName = "CongoCash";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, int currency, double userBalance, int index, int counter)
        {
            base.setupDefaultResultParams(dicParams, currency, userBalance, index, counter);
            dicParams["reel_set"] = "0";
            dicParams["accv"] = "1~cv,cv,fg,cv,fg,cv,jp3,cv,fg,cv,cv,cv,cv,fg,cv,cv,fg,jp2~125,300,20,125,20,1000,500,400,20,125,800,1500,125,20,1000,300,20,2000~0,0,50,0,15,0,0,0,20,0,0,0,0,15,0,0,10,0~6";
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                CongoCashBetInfo betInfo = new CongoCashBetInfo();
                betInfo.BetPerLine       = (float)message.Pop();
                betInfo.LineCount        = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in CongoCashGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strGlobalUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in CongoCashGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            CongoCashBetInfo betInfo = new CongoCashBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        public override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, bool isChangedLineCount, bool isMustLose, BasePPSlotBetInfo betInfo)
        {

            CongoCashBetInfo congoBetInfo = (betInfo as CongoCashBetInfo);
            CongoCashAccV    congoAccV    = null;
            if (congoBetInfo.AccvPerBets.ContainsKey(baseBet))
                congoAccV = congoBetInfo.AccvPerBets[baseBet];

            if(congoAccV != null && congoAccV.ReservedSpin != null)
            {
                CongoCashSpinData reservedSpinData = congoAccV.ReservedSpin;
                congoAccV.ReservedSpin = null;
                return reservedSpinData;
            }

            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalSpinOddIds, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
                if (oddAndID != null)
                {
                    CongoCashSpinData spinDataEvent = await _spinDatabase.Ask<CongoCashSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                    if (!spinDataEvent.IsSpecialReel)
                    {
                        spinDataEvent.IsEvent = true;
                        return spinDataEvent;
                    }
                    else
                    {
                        CongoCashSpinData emptySpin = await selectEmptySpin(agentID, betInfo) as CongoCashSpinData;
                        modifyReserveNextReel(emptySpin, spinDataEvent);
                        spinDataEvent.IsReservedSpin    = true;
                        emptySpin.IsEvent               = true;
                        emptySpin.SpinOdd               = spinDataEvent.SpinOdd;
                        emptySpin.ReservedSpin          = spinDataEvent;
                        return emptySpin;
                    }
                }
            }
            return await selectRandomStop(agentID, betInfo, false);
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, BasePPSlotBetInfo betInfo, bool isMoreBet)
        {
            OddAndIDData        selectedOddAndID = selectRandomOddAndID(agentID, betInfo, isMoreBet);
            CongoCashSpinData   spinData         =  await _spinDatabase.Ask<CongoCashSpinData>(new SelectSpinDataByIDRequest(selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
            if(spinData.IsSpecialReel)
            {
                CongoCashSpinData emptySpin = await selectEmptySpin(agentID, betInfo) as CongoCashSpinData;
                emptySpin.SpinOdd           = spinData.SpinOdd;
                emptySpin.ReservedSpin      = spinData;
                spinData.IsReservedSpin     = true;
                modifyReserveNextReel(emptySpin, spinData);
                return emptySpin;
            }
            else
            {
                return spinData;
            }
        }
        protected void modifyReserveNextReel(CongoCashSpinData firstSpin, CongoCashSpinData nextSpin)
        {
            Dictionary<string, string> dicFirstParams = splitResponseToParams(firstSpin.SpinStrings[0]);
            Dictionary<string, string> dicSecondParams = splitResponseToParams(nextSpin.SpinStrings[0]);

            string      strAccv      = dicFirstParams["accv"];
            string []   strParts     = strAccv.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            string []   strSubParts  = strParts[0].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
            string []   strMasks     = strSubParts[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string []   strMultiples = strSubParts[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string []   strFSCounts  = strSubParts[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);


            string   strAccv2       = dicSecondParams["accv"];
            string[] strParts2      = strAccv2.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            string[] strSubParts2   = strParts2[0].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
            string[] strMasks2      = strSubParts2[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string[] strMultiples2  = strSubParts2[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string[] strFSCounts2   = strSubParts2[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            for(int i = 0; i < 3; i++)
            {
                strMasks[12 + i]       = strMasks2[15 + i];
                strMultiples[12 + i]   = strMultiples2[15 + i];
                strFSCounts[12 + i]    = strFSCounts2[15 + i];
            }
            strSubParts[0]          = string.Join(",", strMasks);
            strSubParts[1]          = string.Join(",", strMultiples);
            strSubParts[2]          = string.Join(",", strFSCounts);
            strParts[0]             = string.Join("~", strSubParts);
            dicFirstParams["accv"]  = string.Join(";", strParts);
            firstSpin.SpinStrings[0] = convertKeyValuesToString(dicFirstParams);
        }

        protected override async Task<BasePPSlotSpinResult> generateSpinResult(BasePPSlotBetInfo betInfo, string strUserID, int agentID, UserBonus userBonus, bool usePayLimit, bool isMustLose)
        {
            CongoCashSpinData   spinData = null;
            BasePPSlotSpinResult result   = null;

            ActionTypes action      = ActionTypes.DOSPIN;
            string strGlobalUserID  = string.Format("{0}_{1}", agentID, strUserID);
            if (betInfo.HasRemainResponse)
            {
                BasePPActionToResponse nextResponse = betInfo.pullRemainResponse();
                action = nextResponse.ActionType;
                result = calculateResult(strGlobalUserID, betInfo, nextResponse.Response, false, action);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            float  totalBet     = betInfo.TotalBet;
            double realBetMoney = totalBet;

            spinData = await selectRandomStop(agentID, userBonus, totalBet, false, isMustLose, betInfo) as CongoCashSpinData;

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = totalBet * spinData.SpinOdd;
            if (!usePayLimit || spinData.IsEvent || spinData.IsReservedSpin || checkCompanyPayoutRate(agentID, realBetMoney, totalWin))
            {
                if (spinData.IsEvent)
                {
                    _bonusSendMessage = null;
                    _rewardedBonusMoney = totalWin;
                    _isRewardedBonus = true;
                }

                result = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true, spinData, action);
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                return result;
            }

            double emptyWin = 0.0;            
            spinData    = await selectEmptySpin(agentID, betInfo) as CongoCashSpinData;
            result      = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true, spinData, action);
            sumUpCompanyBetWin(agentID, realBetMoney, emptyWin);
            return result;
        }
        protected void setupDefaultAccv(Dictionary<string, string> dicParams)
        {
            string strAccv          = dicParams["accv"];
            string[] strParts       = strAccv.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            string[] strSubParts    = strParts[0].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
            string[] strMasks       = strSubParts[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string[] strMultiples   = strSubParts[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string[] strFSCounts    = strSubParts[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            strMasks[9] = "cv";  strMasks[10] = "cv"; strMasks[11] = "cv";
            strMasks[12] = "cv"; strMasks[13] = "fg"; strMasks[14] = "cv";
        }
        protected void setupLastAccv(Dictionary<string, string> dicParams, CongoCashAccV accv)
        {

        }
        protected BasePPSlotSpinResult calculateResult(string strGlobalUserID, BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst, CongoCashSpinData spinData, ActionTypes action)
        {
            try
            {
                BasePPSlotSpinResult spinResult = new BasePPSlotSpinResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                CongoCashBetInfo congoBetInfo = betInfo as CongoCashBetInfo;
                double totalBet = Math.Round(betInfo.TotalBet, 2);
                CongoCashAccV accvInfo = null;
                if(congoBetInfo.AccvPerBets.ContainsKey(totalBet))
                    accvInfo = congoBetInfo.AccvPerBets[totalBet];

                if (accvInfo == null)
                    setupDefaultAccv(dicParams);
                else
                    setupLastAccv(dicParams, accvInfo);

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                spinResult.ResultString = convertKeyValuesToString(dicParams);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                if (!_dicUserHistory.ContainsKey(strGlobalUserID))
                    spinResult.RoundID = ((long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
                else
                    spinResult.RoundID = _dicUserHistory[strGlobalUserID].roundid;

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::calculateResult {0}", ex);
                return null;
            }
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("apwa"))
                dicParams["apwa"] = convertWinByBet(dicParams["apwa"], currentBet);

            if (dicParams.ContainsKey("wlc_v"))
            {
                string strWlc_v = dicParams["wlc_v"];
                string[] strParts = strWlc_v.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strSubParts = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    if (strSubParts.Length >= 2)
                        strSubParts[1] = convertWinByBet(strSubParts[1], currentBet);

                    strParts[i] = string.Join("~", strSubParts);
                }
                dicParams["wlc_v"] = string.Join(";", strParts);
            }
        }
    }
}
