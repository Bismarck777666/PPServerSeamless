using Akka.Actor;
using Akka.Event;
using GITProtocol;
using MongoDB.Bson;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class WildDepthsBetInfo : BasePPSlotBetInfo
    {
        public double                       LastBetAmount   { get; set; }
        public List<BasePPSlotSpinData>     SpinDataLastBet { get; set; }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(SpinDataLastBet.Count);
            for (int i = 0; i < SpinDataLastBet.Count; i++)
                SpinDataLastBet[i].SerializeTo(writer);
            writer.Write(LastBetAmount);
        }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            int count            = reader.ReadInt32();
            this.SpinDataLastBet = new List<BasePPSlotSpinData>();
            for (int i = 0; i < count; i++)
            {
                BasePPSlotSpinData spinData = new BasePPSlotSpinData();
                spinData.SerializeFrom(reader);
                SpinDataLastBet.Add(spinData);
            }
            this.LastBetAmount = reader.ReadDouble();
        }
        public override float TotalBet
        {
            get { return this.BetPerLine * 20.0f; }
        }
        public WildDepthsBetInfo()
        {
            this.SpinDataLastBet = new List<BasePPSlotSpinData>();
        }
    }

    class WildDepthsGameLogic : BasePPSlotGame
    {
        private double _spinDataRTP     = 0.0;
        private double _minSpinDataRTP  = 0.0;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs40wanderw";
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
            get { return 40; }
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
                return "def_s=3,8,5,5,9,6,11,13,3,11,5,9,7,6,13,4,8,11,3,9&cfgs=4906&accm=cp&ver=2&acci=0&def_sb=5,10,9,4,10&reel_set_size=3&def_sa=1,10,2,5,9&accv=1&scatters=1~100,15,2,0,0~12,9,6,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"507872\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0,0,0,0,0,0;400,100,30,0,0;250,75,25,0,0;150,40,15,0,0;100,25,10,0,0;75,15,7,0,0;50,10,5,0,0;30,6,3,0,0;30,6,3,0,0;20,5,2,0,0;20,5,2,0,0;20,5,2,0,0&reel_set0=1,11,8,7,12,9,11,10,10,12,10,8,12,9,11,10,11,3,13,1,11,6,7,6,7,13,8,10,10,9,9,8,5,11,12,6,9,4,13,13,9,12,7,4,8,6,12,5,10,11,13,12,4,11,13,12,10,13,3,13,7,8~5,13,9,6,7,10,10,8,12,10,13,1,11,7,12,8,10,9,12,10,13,11,11,12,11,12,4,6,13,10,6,1,13,11,11,8,7,12,3,12,8,9,12,5,10,9,13,11,13,9,9,7,12,5,12,6,9,11,13,8,4,8,6,11,10,8,10,7,11,12,8,9,11,13,7,5,3,12,9,7,13,2,13,13,3,9,13,4,8,4,10,6,11~8,12,4,10,13,10,6,1,5,12,10,6,8,9,11,11,12,8,4,10,5,12,8,9,7,5,6,13,7,5,8,9,6,13,11,12,13,10,5,9,7,6,12,12,6,3,10,5,4,11,7,11,1,10,13,11,9,4,6,8,11,9,9,7,6,9,5,7,8,12,9,11,5,12,7,9,4,12,13,9,7,8,3,2,11,13,12,5,13,6,13,8,11,10,1,7,2,6,4,13,13,8,11,12,6,7,3,12,9,8,3,13,7,11,10,10,13,10,13,11,8,13,9,11,11,9,9,11,5,10,8,13,13,8,5,10,8,7,13,4,6,12,12,9,6,10,10,9,5,11,9,3,11~10,5,8,9,8,7,11,13,7,11,13,11,8,7,10,6,4,8,13,9,10,5,11,12,11,7,1,12,6,9,13,2,6,3,10,13,4,12,7,12,11,10,9,13,10,6,5,3,12~13,6,2,5,4,7,5,12,13,4,5,4,7,4,7,11,11,3,12,2,6,8,6,8,10,10,9,3,6,9,10,13,12,4,3,10,13,12,9,3,6,12,13,13,8,11,10,8,6,7,9,13,8,5,8,9,3,8,13,12,9,13,5,10,12,11,12,7,5,8,6,5,8,9,6,11,5,13,13,10,9,1,9,10,1,11,6,13,3,7,9,9,12,7,6,4,9,10,13,10,12,5,7,11,7,11,1,5,12,6,11,11,13,8,12,13,4,10,3,10,13,3,12,11,11,10,10,4,7,9,10,12,12,9,5,11,8,9,7,13,11,6,13&accInit=[{id:0,mask:\"cp;mp\"}]&reel_set2=13,6,10,3,11,10,10,7,13,4,8,7,12,3,8,11,11,9,7,12,4,9,8,4,12,7,6,8,10,12,13,11,7,11,8,3,13,10,13,5,3,3,3,3,11,13,8,10,9,13,12,11,10,4,12,5,13,5,10,11,12,6,9,9,11,8,12,3,10,12,7,12,9,10,10,8,9,13,11,13,9,6,5,10,6~10,11,9,9,12,11,7,11,8,7,6,9,9,10,10,12,13,4,5,13,9,8,13,9,9,13,8,9,13,9,4,11,13,9,7,8,3,4,9,6,11,13,12,9,7,13,2,8,7,9,12,5,11,4,12,10,13,12,8,10,12,13,12,8,9,13,9,13,7,12,2,4,12,13,12,10,12,11,7,13,3,11,13,8,7,2,8,6,3,10,13,10,6,11,7,4,13,10,13,11,6,7,11,10,6,10,12,8,12,3,13,4,11,11,12,8,9,8,13,11,5,10,12,10,9,8,2,12,2,6,5,7,10,11,12,4,6,10,5,11,11~6,9,8,9,11,3,8,10,2,10,13,7,11,5,2,6,10,6,8,13,5,4,11,7,13,12,9,12,4,10,9,9,4,5,11,5,8,13,6,7,11,12,13,9,6,10,8,3,7,13,7,12,12,11,8~10,8,12,11,4,7,11,10,12,11,11,4,13,5,13,6,11,7,12,13,12,10,9,7,9,2,12,5,10,9,10,6,5,13,10,11,6,2,7,9,6,7,13,5,7,3,10,10,8,12,13,6,8,7,10,13,8,6,4,12,13,6,13,11,9,10,8,4,11,9,11,13,9,7,3,5,10~3,6,10,12,5,6,4,12,8,13,7,10,4,2,7,11,10,11,6,11,9,12,4,9,12,10,8,5,13,13,3,9,13,11,8,11,7,3,6,5,4,13,11,2,7,5,8,6,9,12,3,10,8,9,13,7,13&reel_set1=11,12,12,3,13,12,7,12,5,7,10,9,11,13,6,8,4,10,8,13,11,3,5,13,10,11,6,3,3,3,3,7,13,10,10,7,9,12,4,10,11,9,8,3,4,11,12,7,11,1,5,11,13,9,9,13,8,12,8,10,8~13,6,10,11,9,13,13,3,6,12,5,8,4,5,10,9,12,12,7,8,7,1,8,12,10,11,10,13,11,6,12,11,9,4,5,13,8,10,13,13,9,7,9,9,8,6,12,10,9,3,13,11,12,2,4,2,7,10,12~9,7,9,8,12,10,5,9,4,8,12,13,9,13,12,2,13,11,5,4,11,6,10,7,9,12,8,12,13,11,7,12,12,13,13,11,3,13,8,6,7,8,10,11,5,7,9,3,5,10,12,11,9,1,4,6,10,6,10,3,9,8,5,10,8,7,11,6~13,7,9,2,3,8,7,13,11,12,12,6,10,2,11,13,12,11,7,8,9,4,6,10,11,13,11,4,7,3,9,10,10,13,7,12,8,1,8,11,13,5,7,5,8,3,6,5,13,12,4,12,10,13,10,12,11,5,10,9,10,11,7,8,6,10,11,13,6,9~4,6,9,8,10,12,6,11,1,13,3,6,8,4,10,7,13,13,11,11,5,11,3,6,7,10,11,12,12,7,8,9,11,7,13,8,12,5,2,9,5,3,10,11,4,12,5,7,13,6,2,8,9,13,9,13,10,9";
            }
        }
	
	
        #endregion
        public WildDepthsGameLogic()
        {
            _gameID = GAMEID.WildDepths;
            GameName = "WildDepths";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet     = (double)infoDocument["defaultbet"];
                _normalMaxID            = (int)infoDocument["normalmaxid"];
                _emptySpinCount         = (int)infoDocument["emptycount"];
                _naturalSpinCount       = (int)infoDocument["normalselectcount"];
                _spinDataRTP            = (double)infoDocument["normalRTP"];
                _minSpinDataRTP         = (double)infoDocument["emptyRTP"];
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            WildDepthsBetInfo betInfo = new WildDepthsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new WildDepthsBetInfo();
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                WildDepthsBetInfo betInfo   = new WildDepthsBetInfo();
                betInfo.BetPerLine          = (float)message.Pop();
                betInfo.LineCount           = (int)message.Pop();
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in WildDepthsGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in WildDepthsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        public override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, bool isChangedLineCount, BasePPSlotBetInfo betInfo)
        {
            return await selectRandomSpinData(agentID, betInfo);
        }
        protected async Task<BasePPSlotSpinData> selectRandomSpinData(int agentID, BasePPSlotBetInfo betInfo)
        {
            var     wildBetInfo = betInfo as WildDepthsBetInfo;
            double  totalBet    = Math.Round(betInfo.TotalBet, 2);

            if (wildBetInfo.LastBetAmount == totalBet)
            {
                if (wildBetInfo.SpinDataLastBet.Count > 0)
                {
                    BasePPSlotSpinData pickedSpinData = wildBetInfo.SpinDataLastBet[0];
                    wildBetInfo.SpinDataLastBet.RemoveAt(0);
                    return pickedSpinData;
                }
                wildBetInfo.SpinDataLastBet.Clear();
            }
            double payoutRate = getPayoutRate(agentID);
            double targetC    = payoutRate / 100.0;
            if (targetC >= _spinDataRTP)
                targetC = _spinDataRTP;

            if (targetC < _minSpinDataRTP)
                targetC = _minSpinDataRTP;

            double x = (_spinDataRTP - targetC) / (_spinDataRTP - _minSpinDataRTP);
            double y = 1.0 - x;

            int spinGroupID = 0;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinGroupID = Pcg.Default.Next(1, _emptySpinCount + 1);
            else
                spinGroupID = Pcg.Default.Next(1, _naturalSpinCount + 1);

            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, spinGroupID), TimeSpan.FromSeconds(10.0));
            var groupSpinData    = convertBsonToGroupSpinData(spinDataDocument);
            int count            = groupSpinData.SpinDatas.Count;

            if (!await checkWebsitePayoutRate(agentID, totalBet * count, count * totalBet * groupSpinData.GroupOdd))
            {
                spinGroupID         = Pcg.Default.Next(1, _emptySpinCount + 1);
                spinDataDocument    = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, spinGroupID), TimeSpan.FromSeconds(10.0));
                groupSpinData       = convertBsonToGroupSpinData(spinDataDocument);
                count               = groupSpinData.SpinDatas.Count;
                sumUpWebsiteBetWin(agentID, totalBet * count, count * totalBet * groupSpinData.GroupOdd);
            }
            BasePPSlotSpinData spinData = groupSpinData.SpinDatas[0];
            groupSpinData.SpinDatas.RemoveAt(0);
            wildBetInfo.LastBetAmount = totalBet;
            wildBetInfo.SpinDataLastBet.Clear();
            wildBetInfo.SpinDataLastBet.AddRange(groupSpinData.SpinDatas);
            return spinData;
        }
        protected GroupedSpinData convertBsonToGroupSpinData(BsonDocument document)
        {
            double spinOdd          = (double)document["odd"];
            string strData          = (string)document["data"];
            string strOddsData      = (string)document["odds"];
            string[] strSpinDatas   = strData.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            string[] strSpinOdds    = strOddsData.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int count               = (int)document["count"];
            List<BasePPSlotSpinData> spinDatas = new List<BasePPSlotSpinData>();
            for (int i = 0; i < count; i++)
            {
                List<string> spinResponses = new List<string>(strSpinDatas[i].Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                double childSpinOdd        = double.Parse(strSpinOdds[i]);
                spinDatas.Add(new BasePPSlotSpinData(0, childSpinOdd, spinResponses));
            }
            return new GroupedSpinData(spinDatas, spinOdd);
        }
        protected override async Task<BasePPSlotSpinResult> generateSpinResult(BasePPSlotBetInfo betInfo, string strUserID, int websiteID, UserBonus userBonus, bool usePayLimit)
        {
            BasePPSlotSpinData      spinData    = null;
            BasePPSlotSpinResult    result      = null;

            if (betInfo.HasRemainResponse)
            {
                BasePPActionToResponse nextResponse = betInfo.pullRemainResponse();
                result                              = calculateResult(betInfo, nextResponse.Response, false);

                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            float   totalBet        = betInfo.TotalBet;
            spinData                = await selectRandomStop(websiteID, userBonus, totalBet, false, betInfo);
            result = calculateResult(betInfo, spinData.SpinStrings[0], true);
            if (spinData.SpinStrings.Count > 1)
                betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            return result;
        }

    }
}
