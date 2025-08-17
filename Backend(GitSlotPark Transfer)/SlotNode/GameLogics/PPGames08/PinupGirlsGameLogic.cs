using Akka.Actor;
using GITProtocol;
using GITProtocol.Utils;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Akka.Event;

namespace SlotGamesNode.GameLogics
{
    class PinupGirlsBetInfo : BasePPSlotBetInfo
    {
        public Dictionary<double, List<BasePPSlotSpinData>> SpinDataPerBet { get; set; }
        public Dictionary<double, string> LastAccvPerBet { get; set; }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.SpinDataPerBet.Count);
            foreach (KeyValuePair<double, List<BasePPSlotSpinData>> pair in this.SpinDataPerBet)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value.Count);
                for (int i = 0; i < pair.Value.Count; i++)
                    pair.Value[i].SerializeTo(writer);
            }
            writer.Write(this.LastAccvPerBet.Count);
            foreach (KeyValuePair<double, string> pair in this.LastAccvPerBet)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }
        }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            int count = reader.ReadInt32();
            this.SpinDataPerBet = new Dictionary<double, List<BasePPSlotSpinData>>();
            for (int i = 0; i < count; i++)
            {
                double key = reader.ReadDouble();
                var spinDatas = new List<BasePPSlotSpinData>();
                int spinDataCount = reader.ReadInt32();
                for (int j = 0; j < spinDataCount; j++)
                {
                    BasePPSlotSpinData spinData = new BasePPSlotSpinData();
                    spinData.SerializeFrom(reader);
                    spinDatas.Add(spinData);
                }
                this.SpinDataPerBet.Add(key, spinDatas);
            }
            count = reader.ReadInt32();
            this.LastAccvPerBet = new Dictionary<double, string>();
            for (int i = 0; i < count; i++)
            {
                double key = reader.ReadDouble();
                string value = reader.ReadString();
                this.LastAccvPerBet[key] = value;
            }
        }
        public PinupGirlsBetInfo()
        {
            this.SpinDataPerBet = new Dictionary<double, List<BasePPSlotSpinData>>();
            this.LastAccvPerBet = new Dictionary<double, string>();
        }
    }
    class PinupGirlsGameLogic : BasePPSlotGame
    {
        private double _spinDataRTP     = 0.0;
        private double _minSpinDataRTP  = 0.0;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20ltng";
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
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100.0; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=6,10,13,3,8,3,11,11,4,9,5,10,8,5,13,7,13,13,4,9&cfgs=6926&ver=3&def_sb=6,12,9,7,9&reel_set_size=7&def_sa=3,13,12,4,12&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"25000000\",max_rnd_win:\"3000\"}}&wl_i=tbm~3000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;750,200,100,0,0;400,120,60,0,0;250,80,40,0,0;175,50,25,0,0;125,40,20,0,0;100,30,15,0,0;60,20,10,0,0;60,20,10,0,0;40,15,8,0,0;40,15,8,0,0;40,15,8,0,0&total_bet_max=10,000,000.00&reel_set0=7,9,8,1,13,9,5,4,13,5,11,12,4,12,9,9,10,13,13,8,10,8,9,9,10,11,3,12,6,5,6,8,9,11,8,10,3,6,7,11,13,13,11,6,9,7,13,10,7,7,1,3,11,12,12,10,11,12,10,5~2,13,10,12,13,6,13,10,9,5,8,13,5,9,7,10,4,8,10,11,5,9,8,6,12,10,13,11,7,12,2,6,13,13,13,6,11,7,1,3,13,13,10,11,4,6,7,4,3,8,7,2,12,5,12,12,8,3,11,11,2,9,1,4,9,12,11,13,8,12~10,10,4,11,3,12,9,8,10,7,13,7,5,11,6,13,12,9,4,9,12,7,1,4,10,10,11,5,13,11,11,11,9,2,12,13,11,12,11,8,3,6,9,2,5,12,4,8,7,8,3,11,6,9,13,11,6,11,13,8,12,10,13,9~3,7,4,6,9,6,8,10,5,4,6,10,12,11,4,13,7,3,11,9,11,2,13,13,8,13,10,4,2,11,10,12,12,2,7,13,11,8,12,9,7,1,12,8,4,8,6,10,8,5,11,7,6,3,10,7,13,8,1,10,2,9,7,9,3,8,13,10,6,12,9,4,10,7,10,5,10,3,5,2,5,12,11,5,13~8,9,13,4,10,12,9,5,7,11,12,7,9,12,4,8,9,10,6,9,1,13,12,11,5,10,11,13,4,9,3,10,8,7,13,3,6,7,13,6,3,11,13,9,6,10,13,11,10,9,7,11,6,5,12,12,11,6,5,11,9,3,8,11,5,13,11,12&accInit=[{id:0,mask:\"cp;tp;sms\"},{id:1,mask:\"cp;tp;sms\"}]&reel_set2=12,13,11,6,13,8,9,13,4,13,11,13,11,5,10,11,4,11,13,9,3,7,13,4,10,11,7,12,9,5,5,10,8,13,11,12,12,8,10,5,8,7,12,6,11,12,6,10,13,8,12,10,9,9,12,12,7,8,6,12,10,13,10,7,13,8,11,10~9,12,9,4,9,9,12,8,11,6,2,7,13,11,9,11,5,8,13,7,2,5,2,7,12,12,13,11,13,12,6,3,2,10,5,11,12,10,7,13,7,11,2,8,13,6,9,3,10,8,10,13,12,4,8,13,10,11,8,11,11,9,7,12~7,9,12,2,12,13,6,11,10,4,12,9,8,12,11,2,7,10,4,13,7,9,13,3,9,6,10,11,8,5,13,5,12,13,3,11,8,10,8,12,2,6,2,7,4~6,8,3,13,12,8,5,2,4,7,2,7,8,6,2,12,2,11,5,10,9,9,7,8,11,13,3,10,8,6,5,10,13,4,12,12,9,12,13,10,7,2,11,7,2,5,13,11,6,9,10,13,6,10,8,10,9,11,11,9,4,13,13,6~12,5,6,8,13,9,10,4,10,3,5,10,8,9,9,11,11,12,6,7,13,3,3,3,13,3,3,11,12,13,11,6,7,9,12,8,13,10,4,7,11,12,7,4,6,12,11,13,7&reel_set1=7,9,13,9,10,1,5,9,12,11,12,13,12,8,10,4,6,13,13,9,5,6,11,13,11,8,4,11,12,7,10,11,9,9,9,4,7,8,3,9,12,1,4,10,8,10,10,8,13,9,3,6,13,8,11,12,5,11,9,12,3,13,10,7,8,6,7,5,9~6,4,10,11,8,13,13,9,8,9,2,9,7,12,11,6,2,12,12,8,9,9,9,11,6,1,2,11,10,8,6,11,12,9,3,2,9,5,11,11,10,2,13,7,13,13,13,10,12,13,11,11,5,12,4,7,12,13,9,3,4,10,13,10,2,13,13,12,3,11,11,11,8,12,13,8,5,3,8,13,11,11,8,12,7,9,7,10,7,6,9,4,1,13,2,11~4,9,13,7,10,6,8,12,2,9,6,11,13,11,13,9,11,3,8,10,12,2,8,12,8,11,9,9,9,2,4,9,7,13,3,2,8,2,12,10,2,7,10,9,11,12,10,5,8,12,9,8,4,7,12,6,8,13,13,13,5,13,12,12,11,4,8,13,6,12,6,9,11,13,10,10,2,5,11,13,13,9,1,5,11,7,11,9~6,11,2,8,1,10,13,5,13,6,8,12,5,13,4,6,13,3,9,7,10,2,8,2,10,11,7,13,8,12,11,2,9,9,9,10,10,12,11,9,3,10,2,9,1,4,8,13,11,10,13,7,3,6,5,4,6,5,9,9,3,5,4,10,12,7,10,12,8,3,7~11,9,5,8,10,10,11,8,3,5,6,10,11,13,8,12,12,9,13,4,9,4,12,11,9,9,6,9,9,9,11,4,10,5,9,8,13,6,12,13,3,6,11,11,3,13,13,7,6,12,9,7,13,7,5,7,1,3,11,10&reel_set4=8,6,13,5,6,13,11,10,4,9,11,10,10,13,8,11,12,12,6,10,3,11,9,12,5,12,7,12,10,13,11,8,12,13,11,13,11,13,7,9,9,8,12,4,13,12,7,10,8,13,6,10,11,12,5,10,10,13,9,12,11,8,10,13,13,8,4,9,12~6,8,12,13,7,5,11,8,4,12,6,9,9,13,2,11,12,7,11,11,3,11,9,5,9,13,11,12,10,7,11,6,10,10,11,9,7,13,13,8,8,13,10,2,13,12,7,8,9,7,10,2,4,12,9,6,10,2,11,11,12,9,12,2,13~13,6,13,12,7,2,13,6,9,10,2,11,11,8,13,13,12,7,6,12,11,5,10,8,7,10,2,8,13,9,9,9,4,3,9,4,12,10,7,12,10,13,8,12,2,9,6,8,12,5,9,2,8,3,6,12,11,9,11,13,7,11,8~6,5,3,11,13,2,7,8,9,13,2,5,13,7,2,6,11,6,10,7,8,12,13,7,9,12,8,5,2,11,13,4,10,9,12,10,10,11,9,6,13,10~11,6,11,10,8,4,3,13,6,7,3,13,12,5,6,12,7,11,12,6,3,3,3,8,11,12,3,7,5,12,8,4,13,11,13,9,8,7,4,10,9,10,9,6,12,13&purInit=[{bet:2000,type:\"default\"}]&reel_set3=13,4,13,11,12,8,4,5,11,12,11,13,12,11,11,13,12,11,11,11,5,13,8,12,11,4,12,11,11,6,4,12,13,11,6,5,8,4,12,12,6,13,12,5,12,12,6,13,12,13,11,11,12,12,12,8,13,6,12,11~3,11,5,11,11,3,10,10,11,11,10,3,7,10,10,5,11,11,11,10,5,9,9,11,9,7,11,6,9,3,6,9,10,11,5,7,6,7~3,10,4,9,9,13,8,7,9,8,12,9,8,7,4,3,10,12,13,9,7,8,8,10,9,4,10,9,8,10,3,8,4,3,8,10,8,4,9,12,12,13,13,7,10,12,4,3,7,12,8,4~12,8,13,6,10,6,9,4,9,8,7,4,10,11,4,3,5,4,13,7,13,9,11,10,11,12,11,7,5,9,10,12,3~6,13,3,12,5,4,8,10,11,10,6,12,9,11,13,5,3,9,8,12,6,5,13,11,10,10,8,7,9,13,7,7,13&reel_set6=13,1,12,7,4,1,13,1,5,1,4,10,8,10,4,12,6,3,6,1,9,7,9,12,11,5,1,8,13,1,3,1~9,4,8,1,12,10,1,2,1,8,1,7,5,13,11,6,9,13,4,7,1,5,6,10,3,2~13,1,4,7,10,1,5,9,3,8,2,7,2,12,3,11,6,5,12,13,1,13,1,4,10,6~1,7,8,2,7,2,11,1,9,10,6,9,1,4,8,10,3,1,4,1,13,12,13,3,12,13,1,5~6,10,3,11,9,1,4,13,10,1,5,8,3,1,5,8,13,6,1,12,13,7,1,9,4,7&reel_set5=11,9,13,8,6,12,7,6,8,10,6,8,3,10,7,4,7,13,4,5,13,12,6,11,11,12,12,9,10,11,7,10,12,8,9,11,5,13,11,10,4,8,13,9,7,6,13,11,13,11,12,3,11,6,5,10,11,12,5,13,12,10,13,8,11,9,7,13,6,3,7,8,10,9,5,12,8,10,8,9,10,5,3,9,7,7,5,6,9,12,13,9,4~8,13,13,12,9,3,13,10,10,6,13,9,11,5,8,11,7,6,13,8,9,11,13,9,9,8,6,12,12,2,10,7,12,12,9,8,7,11,13,4,11,6,12,11,10,13,11,13,13,13,11,3,11,13,10,7,8,9,6,10,2,12,12,13,4,10,13,5,12,8,6,4,2,6,10,12,2,11,5,7,12,7,3,10,9,13,8,7,8,13,12,4,12,2,12,5,7~12,3,10,13,8,11,8,13,3,8,9,9,5,13,12,9,10,7,6,11,9,11,7,12,6,12,10,5,4,10,8,10,12,9,9,5,3,11,8,10,2,13,9,4,8,2,8,11,13,12,13,13,9,13,8,11,12,13,10,7,11,8,2,4,6,4,7,11,12,11,6,12~3,2,9,13,6,4,9,2,10,7,13,7,10,7,12,5,13,12,4,5,7,4,7,12,11,3,13,13,10,10,6,7,5,7,5,9,3,8,4,8,10,6,7,9,6,12,8,4,13,10,11,8,11,11,8,6,5,2,11,13,5,8,13,10,13,11,7,10,10,12,11,8,9,4,10,9,12,10,2,3,2,8,6,12,11~10,10,6,10,8,12,5,11,7,4,13,5,12,13,9,5,4,6,11,11,3,13,7,9,11,10,5,8,11,7,9,6,12,12,6,11,10,13,13,6,4,12,7,7,9,9,8,11,11,9,13,3,12,9,3,9,11,11,6,4,13,3,8,5,13,11,3,13,5,8,9,10,10,12,6,8&total_bet_min=10.00";
            }
        }        
        #endregion

        public PinupGirlsGameLogic()
        {
            _gameID  = GAMEID.PinupGirls;
            GameName = "PinupGirls";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["st"]         = "rect";
            dicParams["sw"]         = "5";
            dicParams["reel_set"]   = "0";
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet = (double)infoDocument["defaultbet"];
                _normalMaxID        = (int)infoDocument["normalmaxid"];
                _emptySpinCount     = (int)infoDocument["emptycount"];
                _naturalSpinCount   = (int)infoDocument["normalselectcount"];

                _spinDataRTP        = (double)infoDocument["normalRTP"];
                _minSpinDataRTP     = (double)infoDocument["emptyRTP"];

                if (SupportPurchaseFree)
                {
                    var purchaseOdds = infoDocument["purchaseodds"] as BsonArray;
                    _totalFreeSpinWinRate = (double)purchaseOdds[1];
                    _minFreeSpinWinRate = (double)purchaseOdds[0];
                }

                if (this.SupportMoreBet)
                {
                    _anteBetMinusZeroCount = (int)((1.0 - 1.0 / MoreBetMultiple) * (double)_naturalSpinCount);
                    if (_anteBetMinusZeroCount > _emptySpinCount)
                        _logger.Error("More Bet Probabily calculation doesn't work in {0}", this.GameName);
                }

                if (this.SupportPurchaseFree && this.PurchaseFreeMultiple > _totalFreeSpinWinRate)
                    _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            PinupGirlsBetInfo betInfo = new PinupGirlsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new PinupGirlsBetInfo();
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                PinupGirlsBetInfo betInfo   = new PinupGirlsBetInfo();
                betInfo.BetPerLine          = (float)message.Pop();
                betInfo.LineCount           = (int)message.Pop();

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in PinupGirlsGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PinupGirlsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        public override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, bool isChangedLineCount, BasePPSlotBetInfo betInfo)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                return await selectPurchaseFreeSpin(agentID, betInfo, baseBet, userBonus);

            return await selectRandomSpinData(agentID, betInfo);
        }

        protected async Task<BasePPSlotSpinData> selectRandomSpinData(int agentID, BasePPSlotBetInfo betInfo)
        {
            var     pinupGirlsBetInfo = betInfo as PinupGirlsBetInfo;
            double  totalBet          = Math.Round(betInfo.TotalBet, 2);

            if(pinupGirlsBetInfo.SpinDataPerBet.ContainsKey(totalBet))
            {
                if (pinupGirlsBetInfo.SpinDataPerBet[totalBet].Count > 0)
                {
                    BasePPSlotSpinData pickedSpinData = pinupGirlsBetInfo.SpinDataPerBet[totalBet][0];
                    pinupGirlsBetInfo.SpinDataPerBet[totalBet].RemoveAt(0);
                    return pickedSpinData;
                }
                pinupGirlsBetInfo.SpinDataPerBet.Remove(totalBet);
            }
            double payoutRate = getPayoutRate(agentID);
            double targetC    = 1.0 * payoutRate / 100.0;
            if (targetC >= _spinDataRTP)
                targetC = _spinDataRTP;

            if (targetC < _minSpinDataRTP)
                targetC = _minSpinDataRTP;

            double x = (_spinDataRTP - targetC) / (_spinDataRTP - _minSpinDataRTP);
            double y = 1.0 - x;

            int spinGroupID = 0;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinGroupID = Pcg.Default.Next(1, _emptySpinCount + 1 );
            else
                spinGroupID = Pcg.Default.Next(1, _naturalSpinCount + 1);

            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, spinGroupID), TimeSpan.FromSeconds(10.0));
            var groupSpinData    = convertBsonToGroupSpinData(spinDataDocument);

            if(!await checkWebsitePayoutRate(agentID, totalBet * 10.0, 10.0 * totalBet * groupSpinData.GroupOdd))
            {
                spinGroupID         = Pcg.Default.Next(1, _emptySpinCount + 1);
                spinDataDocument    = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, spinGroupID), TimeSpan.FromSeconds(10.0));
                groupSpinData       = convertBsonToGroupSpinData(spinDataDocument);
                sumUpWebsiteBetWin(agentID, totalBet * 10.0, 10.0 * totalBet * groupSpinData.GroupOdd);
            }

            BasePPSlotSpinData spinData = groupSpinData.SpinDatas[0];
            groupSpinData.SpinDatas.RemoveAt(0);
            pinupGirlsBetInfo.SpinDataPerBet.Add(totalBet, groupSpinData.SpinDatas);
            return spinData;
        }
        protected virtual GroupedSpinData convertBsonToGroupSpinData(BsonDocument document)
        {
            double spinOdd  = (double)document["odd"];
            string strData  = (string)document["data"];

            string strSpinTypesData = (string)document["spintypes"];
            string strOddsData      = (string)document["odds"];

            string[] strSpinDatas   = strData.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            string[] strSpinTypes   = strSpinTypesData.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string[] strSpinOdds    = strOddsData.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            List<BasePPSlotSpinData> spinDatas = new List<BasePPSlotSpinData>();
            for (int i = 0; i < 10; i++)
            {
                List<string> spinResponses = new List<string>(strSpinDatas[i].Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                int childSpinType = int.Parse(strSpinTypes[i]);
                double childSpinOdd = double.Parse(strSpinOdds[i]);

                spinDatas.Add(new BasePPSlotSpinData(childSpinType, childSpinOdd, spinResponses));
            }
            return new GroupedSpinData(spinDatas, spinOdd);
        }
        protected override async Task<BasePPSlotSpinResult> generateSpinResult(BasePPSlotBetInfo betInfo, string strUserID, int websiteID, UserBonus userBonus, bool usePayLimit)
        {
            BasePPSlotSpinData spinData = null;
            BasePPSlotSpinResult result = null;

            if (betInfo.HasRemainResponse)
            {
                BasePPActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, nextResponse.Response, false);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            float totalBet = betInfo.TotalBet;
            double realBetMoney = totalBet;

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo); //100.0

            spinData = await selectRandomStop(websiteID, userBonus, totalBet, false, betInfo);

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = totalBet * spinData.SpinOdd;
            if (!usePayLimit || !betInfo.PurchaseFree || await checkWebsitePayoutRate(websiteID, realBetMoney, totalWin))
            {
                result = calculateResult(betInfo, spinData.SpinStrings[0], true);
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                return result;
            }

            double emptyWin = 0.0;
            if (betInfo.PurchaseFree)
            {
                spinData = await selectMinStartFreeSpinData(betInfo);
                result = calculateResult(betInfo, spinData.SpinStrings[0], true);
                emptyWin = totalBet * spinData.SpinOdd;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            sumUpWebsiteBetWin(websiteID, realBetMoney, emptyWin);
            return result;
        }
        protected override void saveBetResultInfo(string strGlobalUserID)
        {
            try
            {
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    var betInfo = _dicUserBetInfos[strGlobalUserID] as PinupGirlsBetInfo;
                    Dictionary<string, string> dicParamas = splitResponseToParams(_dicUserResultInfos[strGlobalUserID].ResultString);

                    double totalBet = Math.Round(betInfo.TotalBet, 2);
                    if (dicParamas.ContainsKey("accv"))
                        betInfo.LastAccvPerBet[totalBet] = dicParamas["accv"];
                }
                base.saveBetResultInfo(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PinupGirlsGameLogic::saveBetInfo {0}", ex);
            }
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo baseBetInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, baseBetInfo, spinResult);
            PinupGirlsBetInfo betInfo = baseBetInfo as PinupGirlsBetInfo;
            if (betInfo.LastAccvPerBet != null && betInfo.LastAccvPerBet.Count > 0)
            {
                JArray accbArray = new JArray();
                foreach (KeyValuePair<double, string> pair in betInfo.LastAccvPerBet)
                {
                    JToken accbObj = JToken.Parse("{}");
                    accbObj["b"]        = pair.Key.ToString();
                    accbObj["m"]        = "0";
                    accbObj["v"]        = JToken.Parse("{}");
                    accbObj["v"]["0"]   = pair.Value;
                    accbArray.Add(accbObj);
                }
                dicParams["accb"] = JsonConvert.SerializeObject(accbArray);
            }
        }
    }
    public class GroupedSpinData
    {
        public List<BasePPSlotSpinData> SpinDatas { get; private set; }
        public double                   GroupOdd  { get; private set; }
        public GroupedSpinData(List<BasePPSlotSpinData> spinDatas, double groupOdd)
        {
            SpinDatas = spinDatas;
            GroupOdd = groupOdd;
        }
    }
}
