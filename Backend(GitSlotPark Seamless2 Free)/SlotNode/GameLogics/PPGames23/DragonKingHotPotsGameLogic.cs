using GITProtocol;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class DragonKingHotPotsBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 5.0f;
            }
        }
    }
    class DragonKingHotPotsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10dkinghp";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 10; }
        }
        protected override int ServerResLineCount
        {
            get { return 5; }
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
                return "def_s=5,8,7,4,9,3,6,3,2,6,5,9,9,4,9&cfgs=1&ver=3&def_sb=3,9,5,4,7&reel_set_size=7&def_sa=5,8,7,4,8&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.54\",purchase:\"96.54\",regular:\"96.54\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"433029\",max_rnd_win:\"5000\",max_rnd_win_a:\"2500\",max_rnd_hr_a:\"220746\"}}&wl_i=tbm~5000;tbm_a~2500&sc=20.00,40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&purInit_e=1&wilds=2~300,100,50,0,0~1,1,1,1,1&bonuses=0&bls=5,10&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;300,100,50,0,0;75,25,10,0,0;30,10,5,0,0;30,10,5,0,0;15,5,2,0,0;15,5,2,0,0;15,5,2,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=9,5,2,4,5,1,3,9,9,9,6,7,9,6,9,5,8~9,4,9,8,9,2,1,8,8,8,7,8,8,6,8,7,8,3,5~2,8,9,3,7,7,5,7,7,7,9,8,5,1,6,7,6,4,7~5,9,8,4,1,2,7,3,8,6,7,8,7~8,6,7,9,1,9,9,9,6,4,3,6,2,9,9,5&reel_set2=9,7,6,1,8,9,5,9,9,9,6,7,9,5,4,9,5,3,8~7,8,4,9,8,6,8,8,8,9,8,8,7,3,8,5~8,6,1,5,7,8,7,7,7,3,5,7,6,9,4,7,7,9~4,5,7,6,7,8,3,8,9,8,6,7~9,6,8,9,3,9,9,9,5,8,6,7,9,4,6,6&reel_set1=4,7,3,9,5,8,9,9,9,8,6,9,7,6,1,5~1,8,7,9,3,8,8,8,9,4,9,8,7,5,6~6,7,8,5,9,7,7,7,8,4,6,5,7,7,3,9~4,9,7,8,7,6,5,7,8,6,9,8,3,7,8~8,6,9,6,7,5,9,9,9,4,6,9,6,7,9,3,9,8&reel_set4=7,9,4,6,9,7,5,9,9,9,6,9,9,8,5,5,8,5,3~4,6,8,1,7,9,8,8,8,9,8,3,8,9,5,8,7,8~8,6,7,5,7,9,7,7,7,8,6,7,4,9,5,3~9,8,1,8,8,9,5,4,7,3,6,7,7,6~9,6,5,3,6,4,9,9,9,7,9,8,7,6,8,6,9&purInit=[{bet:500,type:\"default\"}]&reel_set3=7,6,7,4,9,9,9,6,9,5,3,5,8,9,8~8,7,8,5,9,7,8,8,8,9,8,8,1,6,4,3,9,8~7,1,5,7,3,6,7,7,7,4,9,8,6,8,9,7,5~3,4,8,7,6,8,7,5,9,7,7,8,6,8,9~5,9,7,8,6,9,9,9,7,6,9,3,8,4,6,9&reel_set6=13,11,11,11,12,12,11,12,12,12,10,10,10,11,10,12,11~11,11,11,12,12,12,10,10,10,13,10,10~11,12,11,11,11,10,13,11,11,12,12,12,13,10,10,10,11,12,12,10,10&reel_set5=11,12,11,11,11,11,12,11,12,12,12,12,12,10,10,11,10,10,10,12,11,12,10~10,11,11,11,11,12,11,12,12,12,12,12,10,10,10,11,11,12,12~11,12,11,11,11,11,12,12,12,12,12,10,11,10,10,10,12,12,10,10&total_bet_min=100.00";
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
        protected override double MoreBetMultiple
        {
            get { return 2; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        #endregion
        
        public DragonKingHotPotsGameLogic()
        {
            _gameID     = GAMEID.DragonKingHotPots;
            GameName    = "DragonKingHotPots";
        }
        protected override string genInitResponse(string strGlobalUserID, double userBalance, int index, int counter, bool useDefault, Currencies currency, UserBonus userBonus)
        {
            string strInitString = base.genInitResponse(strGlobalUserID, userBalance, index, counter, useDefault, currency, userBonus);
            var dicParams = splitResponseToParams(strInitString);

            if (dicParams.ContainsKey("g")) 
            {
                JObject defaultObject = new JObject();
                defaultObject["def_s"]      = "10,12,13";
                defaultObject["def_sa"]     = "10,12,13";
                defaultObject["def_sb"]     = "10,12,13";
                defaultObject["reel_set"]   = "5";
                defaultObject["s"]          = "10,12,13";
                defaultObject["sa"]         = "10,12,13";
                defaultObject["sb"]         = "10,12,13";
                defaultObject["sh"]         = "1";
                defaultObject["st"]         = "rect";
                defaultObject["sw"]         = "3";


                JToken gParam = JToken.Parse(dicParams["g"]);
                if (!((JObject)gParam).ContainsKey("hot_pot"))
                    gParam["hot_pot"] = defaultObject;
                
                for (int i = 0; i < 5; i++)
                {
                    string strKey = string.Format("ms_{0}", i);
                    if (!((JObject)gParam).ContainsKey(strKey))
                        gParam[strKey] = defaultObject;
                    else
                    {
                        gParam[strKey]["def_s"]     = "10,12,13";
                        gParam[strKey]["def_sa"]    = "10,12,13";
                        gParam[strKey]["def_sb"]    = "10,12,13";
                    }
                }
                dicParams["g"] = serializeJsonSpecial(gParam);
            }

            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";

            return convertKeyValuesToString(dicParams);
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{hot_pot:{def_s:\"10,12,13\",def_sa:\"10,12,13\",def_sb:\"10,12,13\",reel_set:\"5\",s:\"10,12,13\",sa:\"10,12,13\",sb:\"10,12,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms_0:{def_s:\"10,12,13\",def_sa:\"10,12,13\",def_sb:\"10,12,13\",reel_set:\"5\",s:\"10,12,13\",sa:\"10,12,13\",sb:\"10,12,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms_1:{def_s:\"10,12,13\",def_sa:\"10,12,13\",def_sb:\"10,12,13\",reel_set:\"5\",s:\"10,12,13\",sa:\"10,12,13\",sb:\"10,12,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms_2:{def_s:\"10,12,13\",def_sa:\"10,12,13\",def_sb:\"10,12,13\",reel_set:\"5\",s:\"10,12,13\",sa:\"10,12,13\",sb:\"10,12,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms_3:{def_s:\"10,12,13\",def_sa:\"10,12,13\",def_sb:\"10,12,13\",reel_set:\"5\",s:\"10,12,13\",sa:\"10,12,13\",sb:\"10,12,13\",sh:\"1\",st:\"rect\",sw:\"3\"},ms_4:{def_s:\"10,12,13\",def_sa:\"10,12,13\",def_sb:\"10,12,13\",reel_set:\"5\",s:\"10,12,13\",sa:\"10,12,13\",sb:\"10,12,13\",sh:\"1\",st:\"rect\",sw:\"3\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
	        dicParams["bl"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("apwa"))
            {
                string[] strParts = dicParams["apwa"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                    strParts[i] = convertWinByBet(strParts[i], currentBet);
                dicParams["apwa"] = string.Join(",", strParts);
            }

            if (dicParams.ContainsKey("trail"))
            {
                string strTrail = dicParams["trail"];
                string[] strParts = strTrail.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                
                for(int i = 0; i < strParts.Length; i++)
                {
                    string[] strSubParts = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    if(strSubParts.Length > 1)
                    {
                        if(strSubParts[0] == "msw" ||  strSubParts[0] == "mst")
                        {
                            string[] strSubSubParts = strSubParts[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            for(int j = 0; j < strSubSubParts.Length; j++)
                                strSubSubParts[j] = convertWinByBet(strSubSubParts[j], currentBet);

                            strSubParts[1] = string.Join(",", strSubSubParts);
                        }
                    }

                    strParts[i] = string.Join("~", strSubParts);
                }
                
                dicParams["trail"] = string.Join(";", strParts);

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


                DragonKingHotPotsBetInfo betInfo = new DragonKingHotPotsBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
                int bl = (int)message.Pop();
                if (bl == 0)
                    betInfo.MoreBet = false;
                else
                    betInfo.MoreBet = true;
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in DragonKingHotPotsGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in DragonKingHotPotsGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DragonKingHotPotsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new DragonKingHotPotsBetInfo();
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            DragonKingHotPotsBetInfo betInfo = new DragonKingHotPotsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
