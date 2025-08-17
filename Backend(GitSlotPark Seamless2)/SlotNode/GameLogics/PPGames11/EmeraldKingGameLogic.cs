using Akka.Actor;
using GITProtocol;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class EmeraldKingResult : BasePPSlotSpinResult
    {
        public List<int> BonusSelections { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            BonusSelections = SerializeUtils.readIntList(reader);
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            SerializeUtils.writeIntList(writer, BonusSelections);
        }
    }
    class EmeraldKingGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20ekingrr";
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
                return "def_s=3,4,5,6,7,3,4,5,6,7,3,4,5,6,7&cfgs=3800&accm=cp~tp~lvl~sc&ver=2&acci=0&def_sb=5,7,7,8,2&reel_set_size=12&def_sa=8,3,4,3,3&accv=0~16~0~0&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"402928\",max_rnd_win:\"20000\"}}&wl_i=tbm~20000&reel_set10=7,5,3,3,3,10,3,10,3,5,3,10,5,3,10,3,5,3,10,3~8,6,9,3,3,3,3,4,9,3,9,3,9,3,9,3,9~3,3,3,3,6,4,5,7,9,8,5,8,4,9,8,9,4,5,9,8,5,4,8,4,8,4,9,8,9,4,8,4~3,3,3,3,7,5,6,8,4,10,6,7,10,5,6,10,6,5,6~3,3,3,5,7,6,8,3,4,6,8&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&reel_set11=9,3,8,4,3,3,3,6,3,4,3,4,3,4,3,6,3,4,3,4,3,4,8~4,3,3,3,3,6,8,5,7,9,3,7,3,7,9,3,6,3,9~7,10,5,3,3,3,3,5~3,3,3,7,5,6,3,8,10,4,7,5,10,5,7,5,4,6,4~3,3,3,7,5,8,3,4,6,4,6,8,5,8,5,4,6,8,6,4,6,8&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,300,100,0,0;300,100,20,0,0;300,100,20,0,0;200,20,10,0,0;200,20,10,0,0;200,20,10,0,0;0,0,0,0,0;0,0,0,0,0;300,0,0;200,0,0;100,0,0;0,0,0&reel_set0=9,4,3,3,3,7,6,3,10,5,8,2,7,6,3,7,2,5,3,2,7,6,7,3,7,4,5,7,3,7,3,6,7,3,4,7,3,2,4,8,5,4,3,4,2~8,4,6,7,3,9,3,3,3,2,5,10,2,2,2,3,6,10,3,2,3,10,5,10,3,10,2,3,6,3,4,3,6,2,5,10,3,9,7,4,5,3,9,6,9,10,3,10,5,2,10,3,10,7,2,3,10,6,5,3,10,6,10,3,10,3,5,10,2,10,4,2,10,3,9~3,3,3,9,10,5,4,3,7,6,4,7,10,9,6,7,6,9,4,9,7,6,9,6,9,10,9,4,10,7,9,6,9,4,10,4,7,6,10,6,7,4,6,7~10,3,3,3,3,9,8,4,5,2,6,7,3,7,6,3,7,2,8,7,6,8,7,3,5,3,4,3,9,6,8,3,8,6,8,9,8,7,3,9,6,3,7,9,4,5,9,5,4,7,2,8,7,8,3~10,3,3,3,5,7,4,9,3,8,6,2,3,4,3,8,3,7,8,9,7,8,3,4,9,3,4,8,7,8,3,4,2,3,8,4,3&accInit=[{id:0,mask:\"cp;tp;lvl;sc;cl\"}]&reel_set2=10,7,3,3,3,4,2,6,3,5,8,9,3,2,7,4,5,3,9,4,3,9,8,3,7,9,7,8,5,3,4,3,6,7,3,7,8,4,8,7,8,5,3,7,6,4,3,8,4,6,7,4,9,8,7,8,7,8,3,7,3,4,7,3,6,7,8,4,6,8,6,8,3,4,7,4,3,4,6,8,3,8,9,3,8,7,4,8,3,4,8,3,8,3,4,3,4,3,2,7,8,6,4,5,4,8,9,3,6,7,5,3,7,8,4,8,3~3,3,3,5,4,9,6,10,7,3,6,7,5,6,7,6,7,4,6,5,10,7,6,5,6,10,4,5,7,4,10,6,7,10,4,10,6,7,9,7,10,5,7,10,7,10,9,10,4,6,10,9,5,6,5,10,5,10,9,6,4,6,5,10,7,6,5,6,5,10,4,6,7,6,4,6,10,6~3,3,3,6,2,2,2,10,3,9,2,8,7,5,4,8,6,8,2,5,2,8,2,9,6,4,5,2,9,4,5,8,4,8,9,2,10,8,9,2,8,2,8,6,4,6,8,5,10,9,2,9,6,7,5,9,8,5,2,5,9,8,9,5,9,2,6,9,8,5,8,6,9,2,10,9,8,9,8,2,4,5,9,8,9,10,5,2,10,8,9,8,10,9,6,9,5,4,9,8,9,2,6,8,5,2,8,2,9,5,2,9,5,2,9,8,2,8,2,4,8,4,9,8,4,2,8,9,8,2,5,9,2~2,3,3,3,6,10,7,8,4,9,5,3,6,10,6,3,5,10,5,3,6,7,3,6,10,9,7,3,6,10,5,3,6,5,9,4,3,6,10,9,10,9,3,9,7,3,10,3,10,9,10,6,5,6,9,7,9,6,9,6,4,9,10,6,5,10,6,3~9,10,3,3,3,3,5,7,4,8,6,2,6,10,8,4,6,8,3,6,3,8,3,8,7,3,8,3,4,3,8,10,3,8,6,3,8,6,3,8,3,4,7,3,6,3,7,3,7,3,8,4,6,8,4,8,3,8,7,3,4,6,4,2,4,7,8,3,2,8,4,8,5,3,4,5,3,4,3,10,3&reel_set1=2,2,2,6,8,3,3,3,9,4,10,2,7,3,5,3,10,6,9,8,3,6,10,6,9,3,8,4,9,3,9,4,6,3,10,9,6,3,5,3,4,10,4,9,3,10,3,9,3,10,8,3,9,6,8,3,5,3,8,6,10,3,6,10,8,10,3,5,8,9,5,7,5,9,3,5,3,7,3,4,3,6,3,5,6,9,6,9,10,3,4,3,5,9,8,10,7,6,4,10,8,6,3,9~3,7,3,3,3,8,4,6,9,5,10,2,5~3,3,3,10,4,7,6,5,3,9,10,7,10,5,4,7,10,5,10,7,10,4,10,7,5,7,5,7,5,4,10,7,4,6,7,10,5,10,5,10,4,5,4,7,10,6,5,7,4,10,4,7,5,7,5,10,5,10,4,5,7,4,7,10,5,10,5,10,4,10,5,7~8,9,10,7,4,6,3,5,3,3,3,2,6,7,3,4,7,2,6,3,4,6,10,7,5,6,4,2,3,4,3,6,7,2,3,6,3,9,7,3,2,3,7,2,6,2,6,2,3,4,3,6,3,7,6,3,4,6,3,2,7,3,7,9,4,3,10,3,4,7,2,7,10,7,6,3~3,3,3,4,7,8,5,9,2,6,3,10,9,7,8,7,9,10,7,6,2,9,7,4,10,9,10,7,2,4,9,2,7,9,10,9,4,7,4,6,2,8,7,6,7,2,4,7,2,4,7,9,5,9,7,2,10,2,8,2,7,4,9,2,9,6,2,10,9,8,2,4,7,2,4,8,10,2,9,2,8,9,10,9,8,4,10,7,9,2,9,4,9,7,9,2,9,4,10,4,10,4,2,9,2,4,9,7,2,6,7,6,10,8,7,6,2&reel_set4=3,3,3,6,9,4,3,8,5,10,4,5,4,8,4,8,4,10,8,4,8,4,10,9,10,8,9,4,10,8,4,10,5,4,8,10,9,10,4,8,4,10,4,10,4,10,4,8,10,4,10,4,9,4,10,4,8,4,10,6,4,8,9,10,4,6,10~3,3,3,2,9,8,4,5,7,3,10,4,8,4,7,8,7,8,9,10,4,8,7,4,10,2,4,5,7,4,7,8,4,5,4,8,7,4,10~9,3,3,3,2,6,3,5,8,4,7,3,6,3,6,2,7,3,6,2,6,8~3,3,3,7,6,10,9,2,5,8,4,3,8,5,10,8,10,7,9,8,9,10,2,5,9,5,6,8,9,6,5~3,3,3,6,9,2,10,5,8,3,7,4,8,10,8,2,4,6,8,4,2,5,9,2,10,6,9,4,6,2,8,7,2,6,8,9,4,10,2,4,2,5,8,6,5,2,8,2,5,8,2,4,5,2,7,5,2,5,2,5,4,6,5,4,8,10&reel_set3=3,3,3,8,10,2,3,7,4,5,9,8,10,5,4,10,8,10,8,7,8,5,8,4,8,4,8,7,8,4,5,8,9,5,7,5,8,4,8,4,9,5,9,7,4,7,4,8,7,8,2,4,5,7,9~4,6,9,10,3,3,3,5,8,3,6,3,9,3,10,8,3,6,3,10,6,3,9,3,8,6,9,8,3,10,8,9,10,6,8,3,8,10,9,10,3,9,3,6,3,10,8,9,3,6,9,6,3,6,3~8,3,3,3,9,7,5,6,3,2,4,6,3,7,9,3,7,6,3,9,3,9,3,7,6,3,5,7,3,7,3,7,3,4,7,3,7,3,5,7,3,7,3,7,9,5,7,9,3,7,3,7,3,5,9,3,7,3,7,3,5~4,3,3,3,2,3,6,8,9,7,10,5,7,6,7,9,2,7,5,6,7,3,7,5,9,3,8,7,6,8,6,2,3,7,10,3,7,6,3,10,7,8,2,7,3,7,8,5,7,3,7,2,7,3,7,3,7,2,3,6,2,7,2,3,7,3,2,3,6,3,8,2,9,2,7,3,8,7,6,3,8,7,2,9,7,8,3,8,3,8,3,7,10,7,10,7,2,10,6~3,3,3,7,10,9,5,2,8,4,3,6,10,7,10,7,2,5,10,7,9,8,10,8,10,4,10,4,6,7,6,8,2,7,10,8,4,8,7,10,8,7,5,4,10,8,10,8,4,8,6,10,8,10,8,10,8,5,10,8,10,7,8,10,8,7,8,10,8,7,4,8,10,7,10,4,10,4,5,10,7,2,7,10,2,8,4,10,4&reel_set6=5,10,7,9,2,6,4,10,7,4,9,7,2,7,10,2,7,10,2,10,7,10,2,10,7,9,7,9,10,7,10,2,7,10,2,7,4,10,7,2,10,7,10,7~6,8,3,4,10,3,4,8,4,8,3,8,4,3,4,8,3,4,8,3,8,3,8,3,8,10,3,8,4,8,4,3,10,3,8,4,3,8,3,8,3,8,3,8,4,3,8,4,8,3,4,10,8,4,3,4,8,3,8,3,8,3,8,4,3,8,3,4,10,3,4,8,3,4,8,3,8,3,8,4,8,4,8,3,4,10,4,3~3,5,9,7,8,3,3,3,7,8,9,8,7,8,7,8,7,8,7,5,8,7,8,7,9,8,9,7,8,7,8,9,8,5,8,5,8,7,8,9,8,7,8,7,5,8,5,8,7,8,7,8,9,5,7,8,7,5,7,8,7,9,7,8,7,8,7,8,7,9,7,9~2,2,2,10,8,6,3,4,5,2,7,10,4,5,10,3,10,5,6,7,4,7,8,7,3,7,5,6,4,3,5,7,4,10,4,5,7,3,10,3,5,7,3,10,7,5,7,6,7,5,3,8,10,7,5,8,3,7,10,7,10,8,7,5,4,10,6,3,5,6,5,7,4,6,5,7,3,10,3,7,8,5,10,7,4,3,5~2,2,2,6,4,7,5,2,3,8,10,3,3,3,4,10,3,7&reel_set5=3,3,3,2,4,7,3,5,10,9,8,4,5,7,8,9,8,5,7,4,10,4,5~3,3,3,9,6,5,4,2,3,8,7,2,8,7,9,8,4,7,9,4,8,9,7,9,8,9,8,2,8,4,7,9,4,8,9,4,9,8,4,7,9,7,8,2,9,8,9,7,4,7,4,2,7,8,9,4,9,4,8,7,4,9,8,5,7,4,8,7,4,8,4,9,7,2,5,4,9,4,9,8,7,8,9,8,4~3,3,3,4,3,8,6,5,10,9,10,4,6,4,8,10,8,10,8,10,4,10,4,8,4,10,4,10,4,10,8,10,4,8,4,6,10,4,10,6,10,6,8,4,10,4,6,5,10,6,10,8,6,4,8,10,4,8,4,6,10,6,8,4,6,10,4,6,8,10,8,4,10,8,6,4,8,4,6,10,4~2,3,3,3,10,9,3,8,5,6,4,7,3,5,3,9,5,7,8,3,7~3,8,7,3,3,3,2,6,10,4,5,9,7,9,4,9,5,7,4,7,10,5,4,10,8,4,5,10,8,4,6,9,5,4,5,8,5,8,4,6,7,10,4,7,9,7,10,7,5,4,5,4,5,4,10,6,7,9,5,7,5&reel_set8=10,2,6,9,5,7,4,5,7,9,2,9,5,9,7,2,5,9,5,6,2,9,7,2,5,2,5,2,4,5,2,9,5,9,7,9,7,2,9,2,5,2,9,2,9,5,2,5,7,2,7,2,7,9,2,9,2,7,5,9,2,9,7,5,9,5,2,9~3,3,3,9,7,5,3,8,5,7,5,8,5,8,5,8,5,8,5,9,5~4,3,10,6,8,10,3,10,3,10,8,10,8,3,8,10,8,3,8~2,4,2,2,2,5,10,6,3,7,8,3,6,3,5,3,8,3,6,3,6,3,6,3,6,3,6,5,8,3,10,3,5,6,3,6,3,8,3,7,5,8,7,6,8,6,8,10,6,7,6,5,3,6,3,6,3,5,7,6,4,7,5,7,6,3,6,5,6,5,3,4,8,5,3,6,5,3,6,3,6,7,6,3,6,10,8,6,3,6,5,3,6,7,6,3,8,3,7~2,2,2,5,2,7,3,3,3,4,10,3,8,6,5,4,6,4,3,5,8,5,7,8,3,4,6,8,7,6,5,7,5,3,5,8,5,7,8,6,5,6,5,8,5,7,6,8,6,5,7,5,6,7,5,3,8,5,6,7,8,5,6,8,6,5,4,3,5,10,7,8,5,6,3,4&reel_set7=10,3,4,6,8,6~6,2,7,5,9,10,4,10,7,5,9,2,10,2,7,10,7,9,7,2,7,5,2,7,9,7,2,7,10,9,5,2,7,10,2,7,2,5,2,7,4,7,9,10,7,4,2,7,9,2,7,9,2,4,10,7,10,2,10,2,10,2,9,7,10,7,10,9~7,3,3,3,3,9,8,5,9~2,2,2,5,4,6,3,8,2,10,7,6,8,4,8,7,8,7,8,10,4,7,10,5,10,7,8,10,6,7,4,7,8,10,8,5~2,2,2,5,4,3,3,3,10,3,2,7,6,8,4,7,3,6,3,5,3,5,4,7,6,5,8,5,4,3,5,6,3,5,4,5,3,4,5,3,4,5,8,3,7,3,5,7,3,4,6,3,4,8,5,4,6,4,5,3,5,4,7,6,3,8,7,6,4&reel_set9=3,3,3,8,6,9,3,4,9,6,9,4,9,4,6,4,8,4,8,4,6,9,8~5,7,3,3,3,10,3,10,3,7,10,3,10,3,10,3,10,3,10,3,10,3~3,3,3,4,5,3,9,6,7,8,5,6,9,5~4,10,8,3,3,3,3,7,5,6,10,3,7,3,5,7,10,7,5,7,5,3,7,3,7,5,7~7,3,3,3,5,4,6,3,8,4,3,6,4,3,6,3,6,4,3,4,6,3";
            }
        }
	
	
        #endregion
        public EmeraldKingGameLogic()
        {
            _gameID = GAMEID.EmeraldKing;
            GameName = "EmeraldKing";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"]   = "0";
	        dicParams["g"]          = "{ms01:{def_s:\"14,14,14\",sh:\"1\",st:\"rect\",sw:\"3\"},ms02:{def_s:\"14,14,14\",sh:\"1\",st:\"rect\",sw:\"3\"},ms03:{def_s:\"14,14,14\",sh:\"1\",st:\"rect\",sw:\"3\"},ms04:{def_s:\"14,14,14\",sh:\"1\",st:\"rect\",sw:\"3\"},ms05:{def_s:\"14,14,14\",sh:\"1\",st:\"rect\",sw:\"3\"},mss:{screenOrchInit:\"{type:\\\"mini_slots\\\",layout_h:1,layout_w:5}\"}}";
	        dicParams["st"]         = "rect";
	        dicParams["sw"]         = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("bmw"))
                dicParams["bmw"] = convertWinByBet(dicParams["bmw"], currentBet);
            if(dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                for(int i = 1; i <= 5; i++)
                {
                    string key = string.Format("ms0{0}", i);
                    if (gParam[key] == null)
                        continue;

                    int lineIndex = 0;
                    do
                    {
                        string lineKey = "l" + lineIndex.ToString();
                        if (gParam[key][lineKey] == null)
                            break;

                        string[] strParts = gParam[key][lineKey].ToString().Split(new string[] { "~" }, StringSplitOptions.None);
                        if (strParts.Length >= 2)
                        {
                            strParts[1] = convertWinByBet(strParts[1], currentBet);
                            gParam[key][lineKey] = string.Join("~", strParts);
                        }
                        lineIndex++;
                    } while (true);
                }
                dicParams["g"] = serializeJsonSpecial(gParam);
            }
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                EmeraldKingResult spinResult = new EmeraldKingResult();
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
                _logger.Error("Exception has been occurred in WildWestDuelsGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            EmeraldKingResult result = new EmeraldKingResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "gsf_r", "gsf", "st", "sw" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            if (!resultParams.ContainsKey("s") && spinParams.ContainsKey("s"))
                resultParams["s"] = spinParams["s"];

            if (!resultParams.ContainsKey("g"))
                resultParams["g"] = "{}";

            var gParam = JToken.Parse(resultParams["g"]);
            if (spinParams.ContainsKey("g"))
            {
                var gParam2         = JToken.Parse(spinParams["g"]);
                if (gParam["ms01"] == null && gParam2["ms01"] != null)
                    gParam["ms01"] = gParam2["ms01"];
                if (gParam["mss"] == null && gParam2["mss"] != null)
                    gParam["mss"] = gParam2["mss"];
                if (gParam["ms02"] == null && gParam2["ms02"] != null)
                    gParam["ms02"] = gParam2["ms02"];
                if (gParam["ms03"] == null && gParam2["ms03"] != null)
                    gParam["ms03"] = gParam2["ms03"];
                if (gParam["ms04"] == null && gParam2["ms04"] != null)
                    gParam["ms04"] = gParam2["ms04"];
                if (gParam["ms05"] == null && gParam2["ms05"] != null)
                    gParam["ms05"] = gParam2["ms05"];                
            }
            var defaultGParam = JToken.Parse("{ms01:{def_s:\"14,14,14\",sh:\"1\",st:\"rect\",sw:\"3\"},ms02:{def_s:\"14,14,14\",sh:\"1\",st:\"rect\",sw:\"3\"},ms03:{def_s:\"14,14,14\",sh:\"1\",st:\"rect\",sw:\"3\"},ms04:{def_s:\"14,14,14\",sh:\"1\",st:\"rect\",sw:\"3\"},ms05:{def_s:\"14,14,14\",sh:\"1\",st:\"rect\",sw:\"3\"},mss:{screenOrchInit:\"{type:\\\"mini_slots\\\",layout_h:1,layout_w:5}\"}}");
            if (gParam["ms01"] == null && defaultGParam["ms01"] != null)
                gParam["ms01"] = defaultGParam["ms01"];
            if (gParam["mss"] == null && defaultGParam["mss"] != null)
                gParam["mss"] = defaultGParam["mss"];
            if (gParam["ms02"] == null && defaultGParam["ms02"] != null)
                gParam["ms02"] = defaultGParam["ms02"];
            if (gParam["ms03"] == null && defaultGParam["ms03"] != null)
                gParam["ms03"] = defaultGParam["ms03"];
            if (gParam["ms04"] == null && defaultGParam["ms04"] != null)
                gParam["ms04"] = defaultGParam["ms04"];
            if (gParam["ms05"] == null && defaultGParam["ms05"] != null)
                gParam["ms05"] = defaultGParam["ms05"];
            resultParams["g"] = serializeJsonSpecial(gParam);

            return resultParams;
        }

        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin = 0.0;
                string strGameLog = "";
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    EmeraldKingResult result  = _dicUserResultInfos[strGlobalUserID] as EmeraldKingResult;
                    BasePPSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
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

                        if(dicParams.ContainsKey("g"))
                        {
                            var gParam = JToken.Parse(dicParams["g"]);
                            if (gParam["eb"] != null)
                            {                               
                                string[] strParts = gParam["eb"]["status"].ToString().Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
                                bool isAllZero = true;
                                for(int i = 0; i < strParts.Length; i++)
                                {
                                    if (int.Parse(strParts[i]) != 0)
                                        isAllZero = false;
                                }
                                int scatterCount = strParts.Length;
                                if (!isAllZero)
                                {
                                    int ind = (int)message.Pop();
                                    if (result.BonusSelections == null)
                                        result.BonusSelections = new List<int>();

                                    if (result.BonusSelections.Contains(ind))
                                    {
                                        betInfo.pushFrontResponse(actionResponse);
                                        saveBetResultInfo(strGlobalUserID);
                                        throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));
                                    }
                                    result.BonusSelections.Add(ind);
                                    gParam["eb"]["ch"] = string.Format("ind~{0}", ind);

                                    int [] status = new int[scatterCount];
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        status[result.BonusSelections[i]] = i + 1;
                                    gParam["eb"]["status"] = string.Join(",", status);

                                    string[] strWins = gParam["eb"]["wins"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWins = new string[scatterCount];
                                    for (int i = 0; i < scatterCount; i++)
                                        strNewWins[i] = "0";
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        strNewWins[result.BonusSelections[i]] = strWins[i];
                                    gParam["eb"]["wins"] = string.Join(",", strNewWins);

                                    string[] strWinsMask = gParam["eb"]["wins_mask"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWinsMask = new string[scatterCount];
                                    for (int i = 0; i < scatterCount; i++)
                                        strNewWinsMask[i] = "h";
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i];
                                    gParam["eb"]["wins_mask"] = string.Join(",", strNewWinsMask);
                                    dicParams["g"] = serializeJsonSpecial(gParam);
                                }                               
                            }
                        }
                        
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);

                        ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                        string strResponse = convertKeyValuesToString(dicParams);

                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);

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
                _logger.Error("Exception has been occurred in EmeraldKingGameLogic::onDoBonus {0}", ex);
            }
        }

    }
}
