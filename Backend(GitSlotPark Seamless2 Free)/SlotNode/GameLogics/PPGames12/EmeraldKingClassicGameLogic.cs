using Akka.Actor;
using GITProtocol;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using MongoDB.Bson;

namespace SlotGamesNode.GameLogics
{
    class EmeraldKingClassicBetInfo : BasePPSlotBetInfo
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
        public EmeraldKingClassicBetInfo()
        {
            this.SpinDataPerBet = new Dictionary<double, List<BasePPSlotSpinData>>();
            this.LastAccvPerBet = new Dictionary<double, string>();
        }
    }
    class EmeraldKingClassicGameLogic : BasePPSlotGame
    {
        private double _spinDataRTP     = 0.0;
        private double _minSpinDataRTP  = 0.0;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20eking";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return false;
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 20; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
        }
        protected override int ROWS
        {
            get
            {
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=3,4,5,6,7,3,4,5,6,7,3,4,5,6,7&cfgs=3444&accm=cp~pp;cp~pp&ver=2&acci=0;1&def_sb=5,7,7,8,2&reel_set_size=3&def_sa=8,3,4,3,3&accv=1~1;0~0&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&wl_i=tbm~20000&cpri=2&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~500,100,50,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,100,50,0,0;100,50,30,0,0;50,30,20,0,0;30,20,10,0,0;30,20,10,0,0;20,10,5,0,0;20,10,5,0,0;400,0,0;300,0,0;200,0,0;0,0,0&reel_set0=3,6,8,9,7,7,9,9,9,7,9,9,9,5,5,7,9,4,6,5,5,5,7,8,7,8,7,5,7,7,7,3,9,9,9,9,2,5,6,9,9,9,7,7,7,7~3,8,8,8,9,9,4,4,4,8,8,8,6,6,6,8,8,6,6,6,4,4,8,5,8,8,6,6,8,8,8,8,6,6,6,2,6,6,8,4,7,3,3~3,9,9,9,7,9,7,9,7,7,7,4,4,4,9,9,9,5,9,9,8,8,8,6,6,6,5,5,5,9,7,2,9,3,3~3,7,8,6,6,8,8,8,6,5,5,5,8,8,8,6,5,9,7,7,7,7,4,4,4,2,8,9,9,9,4,8,3,3,3,6,6,6~3,9,5,7,7,7,4,4,4,9,9,7,7,7,9,9,9,8,8,8,7,5,5,5,5,9,9,5,5,5,6,9,9,9,6,6,6,2,5,2,3,3,3,5&accInit=[{id:0,mask:\"cp;pp;mp\"},{id:1,mask:\"cp;pp;mp\"}]&reel_set2=10,12,12,12,11,11,10,12,12,10,10,11,13,13,13,10,10,10,12,12,11,11,11,11,13~10,12,12,12,10,10,10,11,13,13,11,11,10,10,10,11,11,11,12,12,12,12,13~10,12,12,12,13,13,10,10,13,13,13,11,11,10,10,10,11&reel_set1=3,3,3,6,6,2,4,4,4,9,9,2,2,2,5,5,5,7,2,3,2,2,2,9,8,8,8,8,7,9,9,9,9,7,7,7,2,2,9,7,9~3,3,3,8,8,2,9,6,4,4,4,8,8,8,9,9,9,6,6,6,2,2,4,5,5,5,8,8,7,7,7,6,2,2,2,3~3,3,3,6,6,6,2,7,3,8,7,7,7,5,5,9,8,8,8,2,2,2,4,4,4,2,6,6,5,5,5,2,2,9,9,9,9,7~3,3,3,7,8,9,9,9,7,8,2,2,8,5,8,8,8,6,2,2,2,7,7,7,2,2,2,8,6,6,6,6,4,4,7,4,4,4,8~3,3,3,9,5,7,7,4,4,4,6,2,2,2,8,7,9,9,8,8,8,5,3,9,2,6,5,2,2,2,9,9,9,7,7,7,2,9,9&cpri_mask=tbw";
            }
        }
	
	
        #endregion
        public EmeraldKingClassicGameLogic()
        {
            _gameID = GAMEID.EmeraldKingClassic;
            GameName = "EmeraldKingClassic";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"]  = "{comm:{reel_set:\"2\",screenOrchInit:\"{type:\\\"mini_slots\\\",layout_h:3,layout_w:5}\"},ms00:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms01:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms02:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms03:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms04:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms05:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms06:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms07:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms08:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms09:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms10:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms11:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms12:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms13:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms14:{def_s:\"13,13,13\",sh:\"1\",st:\"rect\",sw:\"3\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            
            if (dicParams.ContainsKey("cprw"))
                dicParams["cprw"] = convertWinByBet(dicParams["cprw"], currentBet);

            if(dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                if (gParam["comm"] != null)
                {
                    string strSNI = gParam["comm"]["sn_i"].ToString();
                    string[] strParams = strSNI.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < strParams.Length; i++)
                    {
                        if (gParam[strParams[i]] != null && gParam[strParams[i]]["l0"] != null)
                        {
                            string[] strLWParts = gParam[strParams[i]]["l0"].ToString().Split(new string[] { "~" }, StringSplitOptions.None);
                            if (strLWParts.Length >= 2)
                            {
                                strLWParts[1] = convertWinByBet(strLWParts[1], currentBet);
                                gParam[strParams[i]]["l0"] = string.Join("~", strLWParts);
                            }
                        }
                    }
                    dicParams["g"] = serializeJsonSpecial(gParam);
                }
            }
        }

        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            EmeraldKingClassicBetInfo betInfo = new EmeraldKingClassicBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new EmeraldKingClassicBetInfo();
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
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                EmeraldKingClassicBetInfo betInfo = new EmeraldKingClassicBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in EmeraldKingClassicGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
                if (!isNotIntergerMultipleBetPerLine(betInfo.BetPerLine, minChip))
                {
                    _logger.Error("{0} betInfo.BetPerLine is illegual: {1} != {2} * integer", strGlobalUserID, betInfo.BetPerLine, minChip);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1} != {2}", strGlobalUserID, betInfo.LineCount, this.ClientReqLineCount);
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
                _logger.Error("Exception has been occurred in EmeraldKingClassicGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        public override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, bool isChangedLineCount, BasePPSlotBetInfo betInfo, bool isAffiliate)
        {
            if (this.SupportPurchaseFree && betInfo.PurchaseFree)
                return await selectPurchaseFreeSpin(agentID, betInfo, baseBet, userBonus, isAffiliate);

            return await selectRandomSpinData(agentID, betInfo, isAffiliate);
        }
        protected async Task<BasePPSlotSpinData> selectRandomSpinData(int agentID, BasePPSlotBetInfo betInfo, bool isAffiliate)
        {
            var emeraldBetInfo = betInfo as EmeraldKingClassicBetInfo;
            double totalBet = Math.Round(betInfo.TotalBet, 2);

            if (emeraldBetInfo.SpinDataPerBet.ContainsKey(totalBet))
            {
                if (emeraldBetInfo.SpinDataPerBet[totalBet].Count > 0)
                {
                    BasePPSlotSpinData pickedSpinData = emeraldBetInfo.SpinDataPerBet[totalBet][0];
                    emeraldBetInfo.SpinDataPerBet[totalBet].RemoveAt(0);
                    return pickedSpinData;
                }
                emeraldBetInfo.SpinDataPerBet.Remove(totalBet);
            }
            double payoutRate = getPayoutRate(agentID, isAffiliate);
            double targetC = 1.0 * payoutRate / 100.0;
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
            var groupSpinData = convertBsonToGroupSpinData(spinDataDocument);
            int count = groupSpinData.SpinDatas.Count;

            if (!isAffiliate && !await checkWebsitePayoutRate(agentID, totalBet * count, count * totalBet * groupSpinData.GroupOdd))
            {
                spinGroupID = Pcg.Default.Next(1, _emptySpinCount + 1);
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, spinGroupID), TimeSpan.FromSeconds(10.0));
                groupSpinData = convertBsonToGroupSpinData(spinDataDocument);
                count = groupSpinData.SpinDatas.Count;
                sumUpWebsiteBetWin(agentID, totalBet * count, count * totalBet * groupSpinData.GroupOdd);
            }
            BasePPSlotSpinData spinData = groupSpinData.SpinDatas[0];
            groupSpinData.SpinDatas.RemoveAt(0);
            emeraldBetInfo.SpinDataPerBet.Add(totalBet, groupSpinData.SpinDatas);
            return spinData;
        }
        protected virtual GroupedSpinData convertBsonToGroupSpinData(BsonDocument document)
        {
            double spinOdd = (double)document["odd"];
            string strData = (string)document["data"];
            string strOddsData = (string)document["odds"];
            string[] strSpinDatas = strData.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            string[] strSpinOdds = strOddsData.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int count = (int)document["count"];
            List<BasePPSlotSpinData> spinDatas = new List<BasePPSlotSpinData>();
            for (int i = 0; i < count; i++)
            {
                List<string> spinResponses = new List<string>(strSpinDatas[i].Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                double childSpinOdd = double.Parse(strSpinOdds[i]);
                spinDatas.Add(new BasePPSlotSpinData(0, childSpinOdd, spinResponses));
            }
            return new GroupedSpinData(spinDatas, spinOdd);
        }
        protected override async Task<BasePPSlotSpinResult> generateSpinResult(BasePPSlotBetInfo betInfo, string strUserID, int websiteID, UserBonus userBonus, bool usePayLimit, bool isAffiliate, PPFreeSpinInfo freeSpinInfo)
        {
            BasePPSlotSpinData spinData = null;
            BasePPSlotSpinResult result = null;

            if (betInfo.HasRemainResponse)
            {
                BasePPActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, nextResponse.Response, false, freeSpinInfo);

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

            spinData = await selectRandomStop(websiteID, userBonus, totalBet, false, betInfo, isAffiliate);

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = totalBet * spinData.SpinOdd;
            if (isAffiliate || (freeSpinInfo != null) || !usePayLimit || !betInfo.PurchaseFree || await checkWebsitePayoutRate(websiteID, realBetMoney, totalWin))
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
                    result = calculateResult(betInfo, spinData.SpinStrings[0], true, freeSpinInfo);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    return result;
                } while (false);
            }

            double emptyWin = 0.0;
            if (betInfo.PurchaseFree)
            {
                spinData = await selectMinStartFreeSpinData(betInfo);
                result = calculateResult(betInfo, spinData.SpinStrings[0], true, freeSpinInfo);
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
                    var betInfo = _dicUserBetInfos[strGlobalUserID] as EmeraldKingClassicBetInfo;
                    Dictionary<string, string> dicParamas = splitResponseToParams(_dicUserResultInfos[strGlobalUserID].ResultString);

                    double totalBet = Math.Round(betInfo.TotalBet, 2);
                    if (dicParamas.ContainsKey("accv"))
                        betInfo.LastAccvPerBet[totalBet] = dicParamas["accv"];
                }
                base.saveBetResultInfo(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in EmeraldKingClassicGameLogic::saveBetInfo {0}", ex);
            }
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo baseBetInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, baseBetInfo, spinResult);
            EmeraldKingClassicBetInfo betInfo = baseBetInfo as EmeraldKingClassicBetInfo;
            if (betInfo.LastAccvPerBet != null && betInfo.LastAccvPerBet.Count > 0)
            {
                JArray accbArray = new JArray();
                foreach (KeyValuePair<double, string> pair in betInfo.LastAccvPerBet)
                {
                    JToken accbObj = JToken.Parse("{}");
                    accbObj["b"] = pair.Key.ToString();
                    accbObj["v"] = JToken.Parse("{}");

                    string[] strParts = pair.Value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    accbObj["v"]["0"] = strParts[0];
                    accbObj["v"]["1"] = strParts[1];
                    accbArray.Add(accbObj);
                }
                dicParams["accb"] = JsonConvert.SerializeObject(accbArray);
            }
        }

    }
}
