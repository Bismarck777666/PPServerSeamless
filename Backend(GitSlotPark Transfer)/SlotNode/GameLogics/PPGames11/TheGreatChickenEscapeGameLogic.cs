using Akka.Actor;
using Akka.Event;
using GITProtocol;
using Newtonsoft.Json.Linq;
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
    class TheGreatChickenEscapeResult : BasePPSlotSpinResult
    {
        public int          DoBonusCounter  { get; set; }
        public List<int>    BonusSelections { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.DoBonusCounter  = reader.ReadInt32();
            this.BonusSelections = SerializeUtils.readIntList(reader);

        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.DoBonusCounter);
            SerializeUtils.writeIntList(writer, this.BonusSelections);
        }
    }
    class TheGreatChickenEscapeGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20chicken";
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
                return "msi=12&def_s=8,3,2,3,8,10,4,1,4,10,9,3,4,3,9&cfgs=2451&ver=2&reel_set_size=6&def_sb=8,10,8,11,7&def_sa=8,9,10,11,7&prg_cfg_m=wm,s,s&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&base_aw=n;tt~rwf;tt~rwf;tt~msf&prg_cfg=1,15,16&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~500,200,40,2,0~1,1,1,1,1;13~500,200,40,2,0~1,1,1,1,1;14~500,200,40,2,0~1,1,1,1,1;15~500,200,40,2,0~1,1,1,1,1;16~500,200,40,2,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,200,40,2,0;300,100,20,0,0;300,100,20,0,0;200,60,10,0,0;200,60,10,0,0;100,20,8,0,0;100,20,8,0,0;80,10,4,0,0;80,10,4,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=9,7,11,10,9,2,3,3,3,3,11,1,8,11,9,9,11,9,8,10,7,7,5,4,5,8,5,8,10,8,6,6,11,2,6,6,7,11,4,4,10,5,3,3,9,10~9,7,10,10,2,11,5,5,11,7,11,8,4,4,9,6,6,9,3,3,3,3,6,10,11,5,11,7,1,6,7,7,9,8,10,8,4,2,8,9,9,11,10,8~11,8,11,5,9,4,10,7,3,2,9,7,6,4,10,8,3,3,3,3,5,1,7,4,5,11,7,8,8,6,5,9,11,8,10,3,3,10,10,8,4,9,5,9,6,11,1~4,4,11,10,8,5,6,6,8,11,3,3,3,3,6,1,4,9,5,5,8,9,4,5,11,6,9,2,10,6,8,5,8,11,10,7,7,10,9,10,9,3,3,11,9,7,8,8,11,10~11,5,3,3,3,3,3,4,8,5,1,10,1,11,9,10,8,6,6,10,4,4,10,8,7,7,11,4,10,8,11,11,10,9,9,11,7,2,6,7,7,5,6,11,9,10,9,9,10,8,6,9,10,6,10,10,9,8,5,11,4,8,9,8,11,11,4,6,2,5,5,7,6,9,8,8&reel_set2=9,7,11,10,9,3,3,3,3,11,8,11,9,9,11,9,8,10,7,7,5,4,5,8,5,8,10,8,6,6,11,6,6,7,11,4,4,10,5,3,3,9,10~9,7,10,10,11,5,5,11,7,11,8,4,4,9,6,6,9,3,3,3,3,6,10,11,5,11,7,6,7,7,9,8,10,8,4,8,9,9,11,10,8~11,8,11,5,9,4,10,7,3,9,7,6,4,10,8,3,3,3,3,5,7,4,5,11,7,8,8,6,5,9,11,8,10,3,3,10,10,8,4,9,5,9,6,11~4,4,11,10,8,5,6,6,8,11,3,3,3,3,6,4,9,5,5,8,9,4,5,11,6,9,10,6,8,5,8,11,10,7,7,10,9,10,9,3,3,11,9,7,8,8,11,10~11,5,3,3,3,3,3,4,8,5,10,11,9,10,8,6,6,10,4,4,10,8,7,7,11,4,10,8,11,11,10,9,9,11,7,6,7,7,5,6,11,9,10,9,9,10,8,6,9,10,6,10,10,9,8,5,11,4,8,9,8,11,11,4,6,5,5,7,6,9,8,8&reel_set1=9,12,10,12,12,12,12,12,12,12,12,10,12,9,12,12,7,12,9,2,4,8,6,3,11,12,12,7,8,3,12,9,12,10,12,5~12,12,10,12,11,4,12,10,12,12,12,3,12,11,12,12,12,12,12,12,12,11,12,7,8,12,9,12,11,2,6,12,11,5~7,5,12,12,12,10,3,11,12,4,12,9,12,12,11,12,9,12,12,8,12,12,12,12,12,12,12,12,6,12,8,2,10,6,2,10,12,12~12,5,12,4,9,3,7,2,8,6,12,11,5,12,6,12,12,10,12,7,12,4,5,12,8,2,10,3,9,12,11,12,8,12,10,12,12,9~6,2,7,12,8,12,12,12,5,12,9,12,10,12,9,4,10,3,4,12,12,12,12,12,11,12,8,12&reel_set4=9,7,11,10,9,15,3,3,3,3,11,8,11,9,9,11,9,8,10,7,7,5,4,5,8,5,8,10,8,6,6,11,15,6,6,7,11,4,4,10,5,3,3,9,10~9,7,10,10,15,11,5,5,11,7,11,8,4,4,9,6,6,9,3,3,3,3,6,10,11,5,11,7,6,7,7,9,8,10,8,4,15,8,9,9,11,10,8~11,8,11,5,9,4,10,7,3,15,9,7,6,4,10,8,3,3,3,3,5,7,4,5,11,7,8,8,6,5,9,11,8,10,3,3,10,10,8,4,9,5,9,6,11~4,4,11,10,8,5,6,6,8,11,3,3,3,3,6,4,9,5,5,8,9,4,5,11,6,9,15,10,6,8,5,8,11,10,7,7,10,9,10,9,3,3,11,9,7,8,8,11,10~11,5,3,3,3,3,3,4,8,5,10,11,9,10,8,6,6,10,4,4,10,8,7,7,11,4,10,8,11,11,10,9,9,11,7,15,6,7,7,5,6,11,9,10,9,9,10,8,6,9,10,6,10,10,9,8,5,11,4,8,9,8,11,11,4,6,15,5,5,7,6,9,8,8&reel_set3=9,7,11,10,9,3,3,3,3,11,8,11,9,9,11,9,8,10,7,7,5,4,5,8,5,8,10,8,6,6,11,6,6,7,11,4,4,10,5,3,3,9,10~9,7,10,10,11,5,5,11,7,11,8,4,4,9,6,6,9,3,3,3,3,6,10,11,5,11,7,6,7,7,9,8,10,8,4,8,9,9,11,10,8~11,8,11,5,9,4,10,7,3,9,7,6,4,10,8,3,3,3,3,5,7,4,5,11,7,8,8,6,5,9,11,8,10,3,3,10,10,8,4,9,5,9,6,11~4,4,11,10,8,5,6,6,8,11,3,3,3,3,6,4,9,5,5,8,9,4,5,11,6,9,10,6,8,5,8,11,10,7,7,10,9,10,9,3,3,11,9,7,8,8,11,10~11,5,3,3,3,3,3,4,8,5,10,11,9,10,8,6,6,10,4,4,10,8,7,7,11,4,10,8,11,11,10,9,9,11,7,6,7,7,5,6,11,9,10,9,9,10,8,6,9,10,6,10,10,9,8,5,11,4,8,9,8,11,11,4,6,5,5,7,6,9,8,8&reel_set5=9,7,11,10,9,16,3,3,3,3,11,8,11,9,9,11,9,8,10,7,7,5,4,5,8,5,8,10,8,6,6,11,16,6,6,7,11,4,4,10,5,3,3,9,10~9,7,10,10,16,11,5,5,11,7,11,8,4,4,9,6,6,9,3,3,3,3,6,10,11,5,11,7,6,7,7,9,8,10,8,4,16,8,9,9,11,10,8~11,8,11,5,9,4,10,7,3,16,9,7,6,4,10,8,3,3,3,3,5,7,4,5,11,7,8,8,6,5,9,11,8,10,3,3,10,10,8,4,9,5,9,6,11~4,4,11,10,8,5,6,6,8,11,3,3,3,3,6,4,9,5,5,8,9,4,5,11,6,9,16,10,6,8,5,8,11,10,7,7,10,9,10,9,3,3,11,9,7,8,8,11,10~11,5,3,3,3,3,3,4,8,5,10,11,9,10,8,6,6,10,4,4,10,8,7,7,11,4,10,8,11,11,10,9,9,11,7,16,6,7,7,5,6,11,9,10,9,9,10,8,6,9,10,6,10,10,9,8,5,11,4,8,9,8,11,11,4,6,16,5,5,7,6,9,8,8&awt=rsf";
            }
        }
        protected override int FreeSpinTypeCount
        {
            get { return 5; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            switch (freeSpinGroup)
            {
                case 0:
                    return new int[] { 200, 201, 202, 203, 204 };
                case 1:
                    return new int[] { 201, 202, 203, 204 };
                case 2:
                    return new int[] { 202, 203, 204 };
                case 3:
                    return new int[] { 203, 204 };
                case 4:
                    return new int[] { 204 };
            }
            return new int[] { 200, 201, 202, 203, 204 };
        }
        #endregion
        public TheGreatChickenEscapeGameLogic()
        {
            _gameID = GAMEID.TheGreatChickenEscape;
            GameName = "TheGreatChickenEscape";
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
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                TheGreatChickenEscapeResult spinResult = new TheGreatChickenEscapeResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);
                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                if (spinResult.NextAction == ActionTypes.DOBONUS)
                    spinResult.DoBonusCounter = 0;

                spinResult.ResultString = convertKeyValuesToString(dicParams);

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TheGreatChickenEscapeGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            TheGreatChickenEscapeResult result = new TheGreatChickenEscapeResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected string buildStatus(int level)
        {
            switch(level)
            {
                case 0:
                    return "1,0,0,0,0";
                case 1:
                    return "0,1,0,0,0";
                case 2:
                    return "0,0,1,0,0";
                case 3:
                    return "0,0,0,1,0";
                case 4:
                    return "0,0,0,0,1";
            }
            return "1,0,0,0,0";
        }
        protected Dictionary<string, string> buildDoBonusResponse(TheGreatChickenEscapeResult result, BasePPSlotBetInfo betInfo, BasePPSlotStartSpinData startSpinData, int doBonusCounter, int ind, bool isNextMove)
        {
            Dictionary<string, string> dicParams = new Dictionary<string, string>();
            Dictionary<string, string> dicLastParams = splitResponseToParams(result.BonusResultString);
            Dictionary<string, string> dicLastResultParams = splitResponseToParams(result.ResultString);

            int bonusLevel = result.DoBonusCounter + startSpinData.FreeSpinGroup;

            dicParams["tw"]    = dicLastResultParams["tw"];
            dicParams["coef"]  = Math.Round(betInfo.TotalBet, 2).ToString();
            dicParams["wins"]  = dicLastParams["wins"];
            dicParams["bgid"]  = "6";
            dicParams["bgt"]   = "35";
            dicParams["wins_mask"] = dicLastParams["wins_mask"];
            dicParams["status"] = buildStatus(bonusLevel);
            
            if (!isNextMove)
            {
                dicParams["level"] = (bonusLevel + 1).ToString();
                dicParams["na"]    = "cb";
                dicParams["lifes"] = "0";
                dicParams["end"]   = "1";

                int[] minOdds = new int[] { 4, 5, 6, 8 };
                int[] maxOdds = new int[] { 8, 11, 18, 24 };
                int   apv     = Pcg.Default.Next(minOdds[bonusLevel], maxOdds[bonusLevel] + 1);
                double lastWin      = double.Parse(dicLastResultParams["tw"]);
                dicParams["tw"]     = Math.Round(lastWin + betInfo.TotalBet * apv, 2).ToString();
                dicParams["wp"]     = apv.ToString();
                dicParams["rw"]     = Math.Round(betInfo.TotalBet * apv, 2).ToString();

                betInfo.RemainReponses = new List<BasePPActionToResponse>();
                betInfo.RemainReponses.Add(new BasePPActionToResponse(ActionTypes.DOCOLLECTBONUS, "na=s"));
            }
            else
            {
                dicParams["lifes"] = "1";
                dicParams["level"] = bonusLevel.ToString();
                dicParams["end"]   = "0";
                dicParams["na"]    = "b";
                dicParams["wp"]    = "0";
            }
            return dicParams;
        }

        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int     index       = (int)message.Pop();
                int     counter     = (int)message.Pop();
                double  realWin     = 0.0;
                string  strGameLog  = "";
                int     ind         = -1;
                if(message.DataNum > 0)
                    ind = (int)message.Pop();

                string      strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                GITMessage  responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                ToUserResultMessage resultMsg = null;

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo           betInfo = _dicUserBetInfos[strGlobalUserID];
                    TheGreatChickenEscapeResult result  = _dicUserResultInfos[strGlobalUserID] as TheGreatChickenEscapeResult;
                    if ((result.NextAction != ActionTypes.DOBONUS))
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = new Dictionary<string, string>();
                        if(betInfo.HasRemainResponse)
                        {
                            var actionToResponse = betInfo.pullRemainResponse();
                            dicParams            = splitResponseToParams(actionToResponse.Response);
                            convertWinsByBet(dicParams, betInfo.TotalBet);
                            convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                            if (dicParams.ContainsKey("bgt") && dicParams["bgt"] == "21" && dicParams["status"] == "1,0,0,0,0")
                            {
                                string strTemp = "";
                                string[] strParts = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    strTemp = strParts[0];
                                    strParts[0] = strParts[ind];
                                    strParts[ind] = strTemp;
                                    dicParams["status"] = string.Join(",", strParts);
                                }
                                strParts = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    strTemp = strParts[0];
                                    strParts[0] = strParts[ind];
                                    strParts[ind] = strTemp;
                                    dicParams["wins"] = string.Join(",", strParts);
                                }
                                strParts = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    strTemp = strParts[0];
                                    strParts[0] = strParts[ind];
                                    strParts[ind] = strTemp;
                                    dicParams["wins_mask"] = string.Join(",", strParts);
                                }
                            }
                            else if(dicParams.ContainsKey("bgt") && dicParams["bgt"] == "34" && dicParams["status"] == "1,0,0")
                            {
                                string strTemp = "";
                                string[] strParts = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    strTemp = strParts[0];
                                    strParts[0] = strParts[ind];
                                    strParts[ind] = strTemp;
                                    dicParams["status"] = string.Join(",", strParts);
                                }
                                strParts = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    strTemp = strParts[0];
                                    strParts[0] = strParts[ind];
                                    strParts[ind] = strTemp;
                                    dicParams["wins"] = string.Join(",", strParts);
                                }
                                strParts = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    strTemp = strParts[0];
                                    strParts[0] = strParts[ind];
                                    strParts[ind] = strTemp;
                                    dicParams["wins_mask"] = string.Join(",", strParts);
                                }
                            }
                            else if (dicParams.ContainsKey("bgt") && dicParams["bgt"] == "21" && dicParams["status"] == "1,0,0,0")
                            {
                                string strTemp = "";
                                string[] strParts = dicParams["status"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    strTemp = strParts[0];
                                    strParts[0] = strParts[ind];
                                    strParts[ind] = strTemp;
                                    dicParams["status"] = string.Join(",", strParts);
                                }
                                strParts = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    strTemp = strParts[0];
                                    strParts[0] = strParts[ind];
                                    strParts[ind] = strTemp;
                                    dicParams["wins"] = string.Join(",", strParts);
                                }
                                strParts = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (ind != 0)
                                {
                                    strTemp = strParts[0];
                                    strParts[0] = strParts[ind];
                                    strParts[ind] = strTemp;
                                    dicParams["wins_mask"] = string.Join(",", strParts);
                                }
                            }
                            else if (dicParams.ContainsKey("bgt") && dicParams["bgt"] == "36")
                            {
                                if (result.BonusSelections == null)
                                    result.BonusSelections = new List<int>();

                                if (dicParams["sublevel"] == "1")
                                {
                                    if (ind >= 0)
                                    {
                                        if (result.BonusSelections.Contains(ind))
                                            throw new Exception("User selected wrong index in onDoBonus");
                                        result.BonusSelections.Add(ind);
                                    }
                                    else
                                    {
                                        dicParams["sublevel"] = "0";
                                        betInfo.pushFrontResponse(actionToResponse);
                                    }
                                }
                                int[] status = new int[10];
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    status[result.BonusSelections[i]] = i + 1;
                                int[] wins = new int[10];
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    wins[result.BonusSelections[i]] = 1;
                                string[] wins_mask = new string[10];
                                for (int i = 0; i < 10; i++)
                                    wins_mask[i] = "h";
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    wins_mask[result.BonusSelections[i]] = "s";
                                if (dicParams["na"] == "cb")
                                    wins_mask[result.BonusSelections[result.BonusSelections.Count - 1]] = "np";

                                dicParams["status"]         = string.Join(",", status);
                                dicParams["wins"]           = string.Join(",", wins);
                                dicParams["wins_mask"]      = string.Join(",", wins_mask);
                            }
                        }
                        else
                        {
                            bool    isCollect       = false;
                            bool    isEnded         = false;
                            var     startSpinData   = betInfo.SpinData as BasePPSlotStartSpinData;
                            int     stage           = startSpinData.FreeSpinGroup + result.DoBonusCounter;
                            do
                            {
                                if(stage == 4)
                                {
                                    isCollect = true;
                                    break;
                                }
                                if (ind == 0)
                                {
                                    isCollect = true;
                                    break;
                                }
                                double[] moveProbs = new double[] { 0.6624, 0.7928, 0.40, 0.5067 };
                                if (betInfo.SpinData.IsEvent || Pcg.Default.NextDouble(0.0, 1.0) <= moveProbs[stage])
                                {
                                    result.DoBonusCounter++;
                                    dicParams = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 1, true);
                                    if (startSpinData.FreeSpinGroup + result.DoBonusCounter == 4)
                                        dicParams["end"] = "1";
                                }
                                else
                                {
                                    dicParams = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 1, false);
                                    double selectedWin  = startSpinData.StartOdd * betInfo.TotalBet + double.Parse(dicParams["rw"]);
                                    double maxWin       = startSpinData.MaxOdd * betInfo.TotalBet;
                                    
                                    sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                                    isEnded = true;
                                }
                            } while (false);

                            if (isCollect)
                            {
                                BasePPSlotSpinData freeSpinData = convertBsonToSpinData(startSpinData.FreeSpins[result.DoBonusCounter]);
                                preprocessSelectedFreeSpin(freeSpinData, betInfo);

                                betInfo.SpinData = freeSpinData;
                                List<string> freeSpinStrings = new List<string>();
                                for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                                    freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd));

                                betInfo.RemainReponses  = buildResponseList(freeSpinStrings, ActionTypes.DOBONUS);
                                double selectedWin      = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                                double maxWin           = startSpinData.MaxOdd * betInfo.TotalBet;

                                //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                                sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);

                                var freeResponse = betInfo.pullRemainResponse();
                                dicParams        = splitResponseToParams(freeResponse.Response);
                                convertWinsByBet(dicParams, betInfo.TotalBet);
                                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                            }
                        }
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction   = convertStringToActionType(dicParams["na"]);
                        string strResponse       = convertKeyValuesToString(dicParams);
                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                        {
                            if(ind >= 0)
                                addIndActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter, ind);
                            else
                                addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);
                        }
                        result.NextAction = nextAction;
                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        if (nextAction == ActionTypes.DOCOLLECTBONUS || nextAction == ActionTypes.DOCOLLECT)
                        {
                            realWin     = double.Parse(dicParams["tw"].ToString());
                            strGameLog  = strResponse;

                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win     = realWin;
                            }
                            else
                            {
                                saveHistory(agentID, strUserID, index, counter, userBalance, currency);
                            }
                            resultMsg = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID  = betInfo.BetTransactionID;
                            resultMsg.RoundID           = betInfo.RoundID;
                            resultMsg.TransactionID     = createTransactionID();
                        }
                        saveBetResultInfo(strGlobalUserID);
                    }
                }
                if (resultMsg == null)
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(resultMsg);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TheGreatChickenEscapeGameLogic::onDoBonus {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "tw", "sw", "st", "wmt", "wmv", "rs_t", "rs_win", "gwm", "bw" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            return resultParams;
        }
        protected override async Task buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd)
        {
            if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                await base.buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsTotalRandom, minOdd, maxOdd);
            else
                await base.buildStartFreeSpinData(startSpinData, buildType, minOdd, maxOdd);
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            if (dicParams.ContainsKey("bgt") && dicParams["bgt"] == "36" && !dicParams.ContainsKey("crb_wheel"))
                dicParams["crb_wheel"] = "20,60,200,70,30,80,100,90,10,40,500,50";
        }
    }
}
