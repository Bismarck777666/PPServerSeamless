using Akka.Actor;
using GITProtocol;
using Google.Protobuf.WellKnownTypes;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class SpiritOfAdventureResult : BasePPSlotSpinResult
    {
        public double GambleWin { get; set; }
        public double PossibleWin { get; set; }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            GambleWin = reader.ReadDouble();
            PossibleWin = reader.ReadDouble();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(GambleWin);
            writer.Write(PossibleWin);
        }
    }
    class SpiritOfAdventureBetInfo : BasePPSlotBetInfo
    {
        public bool IsEvent { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.IsEvent = reader.ReadBoolean();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.IsEvent);
        }
    }
    class SpiritOfAdventureGameLogic : BasePPSlotGame
    {
        private List<GambleOdd> _totalGambleOdds    = new List<GambleOdd>();
        private List<GambleOdd> _minGambleOdds      = new List<GambleOdd>();
        private double          _totalGambleOdd     = 0.0;
        private double          _minGambleOdd       = 0.0;
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10spiritadv";
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
            get { return 10; }
        }
        protected override int ServerResLineCount
        {
            get { return 10; }
        }
        protected override int ROWS
        {
            get
            {
                return 0;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "cfgs=5471&ver=3&mo_s=16;17;18;15&mo_v=30,50,100,150,200;50,100,150,200,300;100,200,300,500,5000,50000;20,30,50,70,100&reel_set_size=9&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{mystery_box_choice1:\"94.55\",max_rnd_sim:\"1\",mystery_box_choice0:\"94.55\",max_rnd_hr:\"2388535\",max_rnd_win:\"5100\"}}&wl_i=tbm~5100&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,200,50,5,0;1000,150,30,0,0;500,100,20,0,0;500,100,20,0,0;200,50,10,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;0,0,0,0,0;0,0,0,0,0;200,50,10,0,0;200,50,10,0,0;200,50,10,0,0;200,50,10,0,0&total_bet_max=10,000,000.00&reel_set0=4,7,1,11,11,9,12,7,3,8,5,7,8,12,10,11,6,6,10,6,6,6,8,12,7,6,6,9,7,7,3,1,12,4,12,9,6,8,5,7,10,4~6,1,12,11,12,11,8,6,7,5,1,7,8,9,7,3,10,7,6,6,6,11,6,12,5,7,7,5,7,4,9,8,3,6,7,8,7,10,10,6,4~7,11,3,5,8,11,7,1,6,6,7,12,3,9,7,7,10,4,7,10,4,10,5,12,6,9,8,9,7,8,7,11,8,10,5,11,8,7,9,1,10,5,7,11,7,12,6,8,7,7,10,4,9,12,6~6,11,7,9,7,4,8,7,7,9,6,12,7,7,6,8,9,7,10,8,11,8,6,6,6,1,9,1,6,11,8,1,10,7,5,10,3,6,4,12,5,7,7,4,7,3,12,6,12,5~4,6,7,1,11,4,11,1,9,7,12,9,7,12,4,10,1,5,10,7,8,4,6,7,8,6,6,6,7,10,6,5,7,7,9,9,3,11,5,9,7,11,12,3,6,7,6,12,8,7,8,5,6,10,7,10,11,7&reel_set2=5,10,1,7,1,6,1,6,4,6,7,11,7,5,11,7,12,8,4,11,7,7,12,5,7,12,7,1,8,10~6,9,3,7,7,12,9,8,10,3,5,9,6,12,6,6,6,8,7,9,11,8,4,6,5,6,7,5,10,8,7,7,12~8,4,7,6,5,6,9,7,12,9,7,8,12,8,9,9,9,10,7,9,3,9,6,6,7,8,5,11,10,7,3,5,9~8,7,7,9,9,4,6,12,5,6,6,6,7,1,12,6,9,6,10,5,7,3,1~6,6,9,10,7,6,9,6,6,6,5,10,7,3,8,12,7,12,9,9,9,8,3,9,5,4,6,7,9&reel_set1=9,5,9,6,6,5,4,9,7,7,12,7,6,5,9,3,7,6,10,11,3,7,8,7,10,6,6,6,7,4,8,12,9,6,4,7,10,11,7,8,10,12,10,8,7,7,6,7,6,8,12,9,6,10,11,10~8,3,8,5,7,6,6,7,4,9,7,12,11,10,7,6,6,6,4,7,6,8,6,7,12,7,8,10,6,11,10,10,7,5,12,9~7,10,8,9,7,11,7,7,9,7,9,8,10,7,13,4,3,5,6,9,12,13,6,7,4,10,7,10,10,3,11,12,5,6,4,5,11,8,6,8~8,7,11,3,8,10,7,10,10,6,3,10,5,9,6,9,5,7,6,11,8,10,4,7,9,10,6,6,6,7,4,12,6,7,7,6,12,7,12,5,7,9,9,5,8,4,11,6,7,8,7,9,8,6,7,4,12~8,10,6,9,10,11,6,7,11,4,12,9,7,7,3,7,6,6,6,10,4,10,5,7,9,7,6,7,8,6,7,5,12,6,9&reel_set4=6,8,12,7,7,5,7,7,6,8,6,9,12,12,7,10,4,7,11,7,7,5,10,11,7,7,12,5,4,7,7~8,11,9,9,12,11,11,6,7,9,7,4,8,7,8,12,9,3,11,5,11,3,5,8,10,12,11,7,11,5,12~9,8,1,7,9,5,6,1,11,3,12,6,1,4,12,8,3,8,7,6,7,9,5,10,7,9~9,5,10,6,8,7,7,6,6,6,4,9,9,6,5,8,10,3,9,9,9,12,3,7,7,6,8,6~8,12,10,6,1,9,6,5,3,5,7,6,8,4,8,9,3,7,6,1,9,12,7,1,8,7,9&purInit=[{bet:1000,type:\"default\"}]&reel_set3=7,1,5,7,1,12,7,8,4,3,6,7,7,6,11,4,11,9,10,7,7,12,5,10,8~9,4,8,12,8,9,7,8,6,3,11,10,5,7,7,7,1,7,1,7,11,12,7,7,12,7,11,7,3,5,1~8,7,5,9,7,3,9,8,6,10,11,6,5,8,9,9,9,3,10,7,5,9,12,4,6,8,6,9,7,12,7,8,9,9~12,7,10,9,9,7,6,7,6,6,6,10,8,5,8,6,9,12,6,7,9,9,9,4,3,5,6,9,8,6,3~6,8,12,7,9,3,6,5,7,6,6,6,8,9,6,9,4,10,6,7,9,5,6,9,9,9,8,3,9,6,12,7,8,9,5,9,7,10&reel_set6=9,4,6,8,7,1,5,8,5,7,8,1,3,8,10,9,12~11,7,8,5,8,6,1,12,8,3,9,10,4~8,3,11,4,10,5,7,8,9,1,12,8~1,7,12,6,1,10,9,8,9,3,5,4,5,8,7,8~8,4,6,5,9,7,5,1,3,7,1,8,9,8,10,12&reel_set5=10,8,9,11,4,6,7,7,5,7,7,7,8,7,10,11,11,5,7,12,4,6,7,3~8,3,6,12,7,12,9,11,9,8,4,7,5,11,10,9,5~9,12,4,8,6,7,1,5,7,12,11,5,9,1,8,7,1,3,6,10,6,8,9,3~4,3,6,6,12,5,7,1,9,1,8,6,10,9,1,7,9,8,5,7,3,9,12~7,8,9,5,12,6,6,6,4,9,8,6,3,6,9,9,9,7,5,10,9,9,7,6&reel_set8=7,5,10,8,8,11,9,8,10,12,4,3,9,7,12,7,7,7,4,7,12,8,7,8,5,10,9,6,7,7,11,10,8,12~11,7,9,5,7,12,11,12,9,9,7,7,7,10,8,11,5,7,9,9,3,11,11,5,12,12,12,9,3,11,12,6,7,4,11,8,6,11,7,7~8,12,5,8,9,7,3,10,7,12,9,6,7,7,7,12,12,6,4,10,4,11,10,7,7,4,8,5,12,5,13~7,8,12,9,7,10,12,7,7,7,10,7,6,5,3,5,6,4~9,7,7,4,5,12,7,3,12,7,7,7,12,7,8,4,7,7,12,11,5,8,11&reel_set7=10,9,10,12,8,6,4,12,3,8,8,12,5,10,6,9,12,11,12,7,10,8,7,8,10~11,9,5,9,12,11,5,7,9,11,6,4,10,6,7,9,12,3,11,8,11,11,3,12,11,11,8,11,11,9,9,11,11,7,9,11,9,11,9,11~6,9,12,12,8,6,5,4,8,6,4,10,5,10,12,7,4,12,12,10,11,10,12,3,9,10,5,8,6,9,8,7,10,5,12,4,12,8,13~5,12,3,4,12,4,12,8,10,6,5,6,9,7,3,11,10~9,10,6,4,3,8,12,11,6,5,10,9,7,4,8,11,5,12,12,7,12&total_bet_min=20.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100; }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        #endregion
        public SpiritOfAdventureGameLogic()
        {
            _gameID = GAMEID.SpiritOfAdventure;
            GameName = "SpiritOfAdventure";
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            await base.onLoadSpinData(infoDocument);
            List<BsonDocument> gambleOdds = await Context.System.ActorSelection("/user/spinDBReaders").Ask<List<BsonDocument>>(new ReadGambleOddsRequest(this.GameName), TimeSpan.FromSeconds(10.0));

            _totalGambleOdds.Clear();
            _minGambleOdds.Clear();
            _totalGambleOdd = 0.0;
            _minGambleOdd   = 0.0;
            for (int i = 0; i < gambleOdds.Count; i++)
            {
                double minOdd  = (double)gambleOdds[i]["minodd"];
                double maxOdd  = (double)gambleOdds[i]["maxodd"];
                double percent = (double)gambleOdds[i]["percent"];
                double realOdd = (double)gambleOdds[i]["realodd"];

                GambleOdd gambleOdd = new GambleOdd(minOdd, maxOdd, realOdd, percent);
                _totalGambleOdds.Add(gambleOdd);
                _totalGambleOdd += realOdd;

                if (realOdd <= 0.5)
                {
                    _minGambleOdds.Add(gambleOdd);
                    _minGambleOdd += realOdd;
                }
            }
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["g"] = "{reels:{def_s:\"5,3,5,3,3,6,4,6,4,4,6,9,6,9,9\",def_sa:\"9,10,9,10,12\",def_sb:\"6,9,8,9,9\",reel_set:\"0\",s:\"5,3,5,3,3,6,4,6,4,4,6,9,6,9,9\",sa:\"9,10,9,10,12\",sb:\"6,9,8,9,9\",sh:\"3\",st:\"rect\",sw:\"5\"}}";
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
                {
                    betInfo.RemainReponses = null;
                    (betInfo as SpiritOfAdventureBetInfo).IsEvent = false;
                }
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            float totalBet = betInfo.TotalBet;
            double realBetMoney = totalBet;

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo); //100.0

            if (SupportMoreBet && betInfo.MoreBet)
                realBetMoney = totalBet * MoreBetMultiple;

            spinData = await selectRandomStop(websiteID, userBonus, totalBet, false, betInfo);

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = totalBet * spinData.SpinOdd;
            if (!usePayLimit || spinData.IsEvent || await checkWebsitePayoutRate(websiteID, realBetMoney, totalWin))
            {
                do
                {
                    if (spinData.IsEvent)
                    {
                        bool checkRet = await subtractEventMoney(websiteID, strUserID, totalWin);
                        if (!checkRet)
                            break;

                        _bonusSendMessage = null;
                        _rewardedBonusMoney = totalWin;
                        _isRewardedBonus = true;
                    }
                    result = calculateResult(betInfo, spinData.SpinStrings[0], true);
                    if (spinData.SpinStrings.Count > 1)
                    {
                        if(spinData.IsEvent)
                            (betInfo as SpiritOfAdventureBetInfo).IsEvent = false;
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    }
                    return result;
                } while (false);
            }
            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseFree)
            {
                spinData = await selectMinStartFreeSpinData(betInfo);
                result = calculateResult(betInfo, spinData.SpinStrings[0], true);
                emptyWin = totalBet * spinData.SpinOdd;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData = await selectEmptySpin(websiteID, betInfo);
                result = calculateResult(betInfo, spinData.SpinStrings[0], true);
            }
            sumUpWebsiteBetWin(websiteID, realBetMoney, emptyWin);
            return result;            
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if (dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                if (gParam["reels"] == null)
                {
                    var initGParam = JToken.Parse("{reels:{def_s:\"5,3,5,3,3,6,4,6,4,4,6,9,6,9,9\",def_sa:\"9,10,9,10,12\",def_sb:\"6,9,8,9,9\",reel_set:\"0\",s:\"5,3,5,3,3,6,4,6,4,4,6,9,6,9,9\",sa:\"9,10,9,10,12\",sb:\"6,9,8,9,9\",sh:\"3\",st:\"rect\",sw:\"5\"}}");
                    gParam["reels"] = initGParam["reels"];
                    dicParams["g"] = serializeJsonSpecial(gParam);
                }
            }
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("pw"))
                dicParams["pw"] = convertWinByBet(dicParams["pw"], currentBet);

            if (dicParams.ContainsKey("apwa"))
                dicParams["apwa"] = convertWinByBet(dicParams["apwa"], currentBet);

            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);

            if (dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                if (gParam["R_E"] != null && gParam["R_E"]["mo_tw"] != null)
                {
                    gParam["R_E"]["mo_tw"] = convertWinByBet(gParam["R_E"]["mo_tw"].ToString(), currentBet);
                    dicParams["g"] = serializeJsonSpecial(gParam);
                }
                if (gParam["R_S"] != null && gParam["R_S"]["mo_tw"] != null)
                {
                    gParam["R_S"]["mo_tw"] = convertWinByBet(gParam["R_S"]["mo_tw"].ToString(), currentBet);
                    dicParams["g"] = serializeJsonSpecial(gParam);
                }
                if (gParam["R_M"] != null && gParam["R_M"]["mo_tw"] != null)
                {
                    gParam["R_M"]["mo_tw"] = convertWinByBet(gParam["R_M"]["mo_tw"].ToString(), currentBet);
                    dicParams["g"] = serializeJsonSpecial(gParam);
                }
                if (gParam["R_G"] != null && gParam["R_G"]["mo_tw"] != null)
                {
                    gParam["R_G"]["mo_tw"] = convertWinByBet(gParam["R_G"]["mo_tw"].ToString(), currentBet);
                    dicParams["g"] = serializeJsonSpecial(gParam);
                }
                if (gParam["R_C"] != null && gParam["R_C"]["mo_tw"] != null)
                {
                    gParam["R_C"]["mo_tw"] = convertWinByBet(gParam["R_C"]["mo_tw"].ToString(), currentBet);
                    dicParams["g"] = serializeJsonSpecial(gParam);
                }
                if (gParam["bg_0"] != null)
                {
                    if (gParam["bg_0"]["rw"] != null)
                        gParam["bg_0"]["rw"] = convertWinByBet(gParam["bg_0"]["rw"].ToString(), currentBet);

                    if (gParam["bg_0"]["ch_v"] != null)
                    {
                        string[] vArray = gParam["bg_0"]["ch_v"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        vArray[0] = convertWinByBet(vArray[0], currentBet);
                        gParam["bg_0"]["ch_v"] = string.Join(",", vArray);
                    }
                    dicParams["g"] = serializeJsonSpecial(gParam);
                }
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            SpiritOfAdventureBetInfo betInfo = new SpiritOfAdventureBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                SpiritOfAdventureBetInfo betInfo = new SpiritOfAdventureBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();


                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in SpiritOfAdventureGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                    oldBetInfo.MoreBet = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpiritOfAdventureGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override async Task overrideResult(BasePPSlotBetInfo baseBetInfo, BasePPSlotSpinResult baseResult, int agentID)
        {
            SpiritOfAdventureBetInfo betInfo = baseBetInfo as SpiritOfAdventureBetInfo;
            SpiritOfAdventureResult result = baseResult as SpiritOfAdventureResult;
            Dictionary<string, string> dicParams = splitResponseToParams(result.ResultString);

            if ((result.NextAction != ActionTypes.DOBONUS) || !dicParams.ContainsKey("trail"))
                return;

            double      possibleWin = double.Parse(dicParams["pw"]);
            GambleOdd   gambleOdd   = pickGambleOdd(agentID);
            double      gambleWin   = Math.Round(possibleWin * gambleOdd.RealOdd, 2);
            
            if (! await checkWebsitePayoutRate(agentID, possibleWin, gambleWin, 1))
            {
                gambleOdd = _minGambleOdds[Pcg.Default.Next(0, _minGambleOdds.Count)];
                gambleWin = Math.Round(possibleWin * gambleOdd.RealOdd, 2);
                sumUpWebsiteBetWin(agentID, possibleWin, gambleWin, 1);
            }
            string trail = string.Format("mb_range~{0},{1};mb_odds~{2}", Math.Round(possibleWin * gambleOdd.MinOdd, 2),
                Math.Round(possibleWin * gambleOdd.MaxOdd, 2), Math.Round(gambleOdd.Percent, 2));
            dicParams["trail"]  = trail;
            result.PossibleWin  = possibleWin;
            result.GambleWin    = gambleWin;
            result.ResultString = convertKeyValuesToString(dicParams);
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            SpiritOfAdventureResult result = new SpiritOfAdventureResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                SpiritOfAdventureResult spinResult = new SpiritOfAdventureResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);
                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                spinResult.ResultString = convertKeyValuesToString(dicParams);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpiritOfAdventureGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        private GambleOdd pickGambleOdd(int agentID)
        {
            double payoutRate = getPayoutRate(agentID);
            double targetC    = payoutRate / 100.0;
            if (targetC >= _totalGambleOdd)
                targetC = _totalGambleOdd;
            if (targetC < _minGambleOdd)
                targetC = _minGambleOdd;

            double x = (_totalGambleOdd - targetC) / (_totalGambleOdd - _minGambleOdd);
            double y = 1.0 - x;
            GambleOdd gambleOdd = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                gambleOdd = _minGambleOdds[Pcg.Default.Next(0, _minGambleOdds.Count)];
            else
                gambleOdd = _totalGambleOdds[Pcg.Default.Next(0, _totalGambleOdds.Count)];
            return gambleOdd;
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int) message.Pop();
                int counter = (int) message.Pop();
                int ind     = (int) message.Pop();
                var    responseMessage  = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin          = 0.0;
                string strGameLog       = "";
                string strGlobalUserID  = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    SpiritOfAdventureResult result  = _dicUserResultInfos[strGlobalUserID] as SpiritOfAdventureResult;
                    BasePPSlotBetInfo       betInfo = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);
                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                        if (ind == 1)
                        {
                            Dictionary<string, string> newParams = new Dictionary<string, string>();
                            newParams["tw"]                      = result.GambleWin.ToString();
                            newParams["na"]                      = "cb";
                            newParams["rs_win"]                  = "0.00";
                            newParams["rs_t"]                    = dicParams["rs_t"];
                            var gParam                           = JToken.Parse(dicParams["g"]);
                            if (gParam["bg_0"] != null && gParam["bg_0"]["rw"] != null)
                            {
                                gParam["bg_0"]["rw"] = result.GambleWin.ToString();
                                newParams["g"] = serializeJsonSpecial(gParam);
                            }
                            else
                            {
                                newParams["g"] = dicParams["g"];
                            }
                            dicParams = newParams;
                        }
                        else
                        {
                            sumUpWebsiteBetWin(agentID, -result.PossibleWin, -result.GambleWin, 1);
                        }
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);

                        ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                        string strResponse = convertKeyValuesToString(dicParams);

                        responseMessage.Append(strResponse);
                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addIndActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter, ind);

                        if (nextAction == ActionTypes.DOCOLLECT || nextAction == ActionTypes.DOCOLLECTBONUS)
                        {
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;

                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win = realWin;
                            }
                            resultMsg = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID = betInfo.BetTransactionID;
                            resultMsg.RoundID = betInfo.RoundID;
                            resultMsg.TransactionID = createTransactionID();

                        }
                        copyBonusParamsToResult(dicParams, result);
                        result.NextAction = nextAction;
                    }
                    if (!betInfo.HasRemainResponse)
                        betInfo.RemainReponses = null;

                    saveBetResultInfo(strGlobalUserID);
                }
                if (resultMsg == null)
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(resultMsg);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SpiritOfAdventureGameLogic::onDoBonus {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }

            if (!resultParams.ContainsKey("g") && spinParams.ContainsKey("g"))
                resultParams["g"] = spinParams["g"];
            return resultParams;
        }

        protected override void addDefaultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter)
        {
            dicParams["balance"] = Math.Round(userBalance, 2).ToString();        //밸런스
            dicParams["balance_cash"] = Math.Round(userBalance, 2).ToString();        //밸런스
            dicParams["balance_bonus"] = "0.0";
            dicParams["stime"] = GameUtils.GetCurrentUnixTimestampMillis().ToString();
            dicParams["index"] = index.ToString();
            dicParams["counter"] = (counter + 1).ToString();
            dicParams["sver"] = "5";
        }

        protected override string makeSpinResultString(BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult, double betMoney, double userBalance, int index, int counter, bool isInit)
        {
            Dictionary<string, string> dicParams = splitResponseToParams(spinResult.ResultString);

            if (spinResult.HasBonusResult)
            {
                Dictionary<string, string> dicBonusParams = splitResponseToParams(spinResult.BonusResultString);
                dicParams = mergeSpinToBonus(dicParams, dicBonusParams);
            }

            dicParams["balance_bonus"] = "0.00";
            dicParams["stime"] = GameUtils.GetCurrentUnixTimestampMillis().ToString();
            dicParams["sver"] = "5";
            dicParams["l"] = ServerResLineCount.ToString();
            dicParams["c"] = Math.Round(betInfo.BetPerLine, 2).ToString();
            if (index > 0)
            {
                dicParams["index"] = index.ToString();
                dicParams["counter"] = (counter + 1).ToString();
            }

            ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
            if (isInit)
            {
                dicParams["na"] = convertActionTypeToString(spinResult.NextAction);
                dicParams["action"] = convertActionTypeToFullString(spinResult.NextAction);
            }
            else
            {
                dicParams["na"] = convertActionTypeToString(spinResult.NextAction);
            }

            dicParams["balance"] = Math.Round(userBalance - (isInit ? 0.0 : betMoney), 2).ToString();        //밸런스
            dicParams["balance_cash"] = Math.Round(userBalance - (isInit ? 0.0 : betMoney), 2).ToString();        //밸런스케시

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = "0";
            else
                dicParams.Remove("puri");

            if (SupportMoreBet)
            {
                if (betInfo.MoreBet)
                    dicParams["bl"] = "1";
                else
                    dicParams["bl"] = "0";
            }
            if (isInit)
                supplementInitResult(dicParams, betInfo, spinResult);

            overrideSomeParams(betInfo, dicParams);
            return convertKeyValuesToString(dicParams);
        }

    }
}
