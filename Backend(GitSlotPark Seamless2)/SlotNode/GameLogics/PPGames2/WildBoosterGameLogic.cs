using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class WildBoosterGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20wildboost";
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
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=3,4,5,6,7,8,9,10,2,11,7,6,1,4,3&cfgs=6707&ver=2&def_sb=1,2,3,4,5&reel_set_size=10&def_sa=1,2,3,4,5&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={rtps:{purchase:\"96.47\",regular:\"96.47\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"836890\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=10.00,20.00,30.00,40.00,50.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,2000.00,3000.00,4000.00,5000.00&defc=100.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;600,100,50,0,0;300,50,25,0,0;200,40,20,0,0;150,25,12,0,0;100,20,10,0,0;60,12,6,0,0;60,12,6,0,0;50,10,5,0,0;50,10,5,0,0&rtp=96.47&total_bet_max=7,500,000.00&reel_set0=5,8,10,7,11,6,9,11,4,5,5,3,10,10,9,6,4,6,8,9,10,1,11,9,10,8,8,7,7,9,8,3,11,3~7,5,4,1,10,2,7,10,4,8,6,11,9,7,10,3,8,11,9,5,4,9,10,11,9,11,3,4,9,5,6,8,11,11,6,2,8,4,11,8,10,1,5,7,6,9~10,7,9,4,3,11,3,6,2,11,7,5,9,10,1,9,4,7,2,5,11,11,8,10,3,2,10,6,9,9,8,1,5,5,9,10,2,10,11,4,4,7,8,9,5,8,8,4,8,7~10,3,8,2,2,9,10,3,5,4,1,8,9,4,6,9,3,9,11,7,10,4,6,11,5,11,8~9,10,6,9,11,9,4,7,9,4,8,6,4,1,10,7,5,6,3,3,8,11,5,9,8,11,10,3,8,10,5,3,8&accInit=[{id:0,mask:\"cp; tp; lvl; sc; cl\"}]&reel_set2=5,10,9,3,7,4,11,8,7,10,8,6,8,4,10,8,7,3,6,9,6,8,9,5,5,4,9,10,10,8,4,3,4,9,7,11,10,10,9,11,5,11,3,9,11~6,7,9,4,3,2,8,8,9,7,9,5,8,2,11,11,7,10,9,6,6,5,11,10,11,4,11,3,4,11,9,2,10,3,5~9,2,9,11,8,2,10,10,2,11,8~10,7,6,3,5,3,10,11,4,3,9,3,3,9,6,9,10,9,8,11,8,3,11,5,2,5,11,2,8,6,9,5,4,8,7,10,11,9,7,9,5,4,11,8,8,4,10~8,5,10,8,9,9,4,8,9,11,9,3,10,4,11,7,8,5,11,6,7,3,9,4,11,10,6,3,5,6,6,4,10,3,9,8,5,5,10,7,8,7,10,11&reel_set1=10,9,7,7,8,10,11,8,4,5,11,11,9,4,3,8,6,10,10,3,8,10,7,11,8,5,3,5,10,9,4,4,5,9,6,6,3,9,4,9,11,8,7~11,2,2,11,11,8,10,2,9,8,10,9,2,10~7,11,9,9,8,10,9,4,11,5,9,5,4,10,4,3,9,9,10,11,3,6,6,5,8,6,7,4,8,8,3,6,7,7,5,4,8,5,10,10,8,5,10,2,3,2,2,11~4,10,3,10,4,7,9,11,9,3,11,10,8,7,4,5,8,9,5,8,6,2,9,3,5,11,3,8,6,2,11~11,8,10,5,5,3,10,9,5,8,7,5,4,6,9,11,4,6,10,8,5,9,8,3,9,8,11,10,3,9,10,11,4,6,8,11,7,7,9,6,3,7,4&reel_set4=9,8,8,11,5,11,6,10,3,11,1,3,5,10,4,7,9,9,4,8,4,10,7~8,9,11,7,10,2,6,11,5,10,10,11,3,11,5,3,2,7,9,9,2,8,11,6,9,8,2,6,11,4,6,3,5,10,4,2,8,11,5,1,9,9~1,2,2,3,4,4,5,5,6,7,8,8,9,9,10,10,11,11~7,1,6,7,10,1,3,9,11,5,2,3,11,3,9,5,8,9,8,11,11,2,8,9,4,2,10,8,4,6,10~10,8,9,8,1,3,4,7,10,10,7,9,9,10,6,10,11,9,6,5,8,4,3,9,4,8,6,3,5,5,7,11&purInit=[{type:\"fs\",bet:1500}]&reel_set3=10,6,8,5,7,9,11,3,6,3,9,10,4,10,4,11,7,8,9,8,5~10,4,3,8,6,2,11,5,3,6,7,11,9,4,7,10,8,5,11,2,9,9~2,8,7,6,5,9,4,9,8,10,3,2,11,8,9,10,10,11,3,6,6,2,10,7,4,11,5,5,3,5,8,9,4~9,11,9,2,8,2,10,11,10,8,2~7,8,3,9,6,9,8,3,4,9,11,11,5,11,6,4,8,7,6,3,11,7,9,5,8,5,8,4,10,8,8,7,3,8,9,10,7,10,4,8,10,3,6,5,9,10,9,4,11,10,7,5,10,11,11,9,4,6,3,10,9,5,6,5&reel_set6=8,10,8,10,9,5,11,4,3,7,7,3,11,11,8,6,5,9,9,7,10,6,11,10,10,9,8,4,3,9,4,5,4,1~10,11,8,9,10,5,10,9,2,2,11,9,6,3,9,3,6,11,7,8,9,11,5,4,8,11,2,6,5,11,7,5,4,1,2,8,11,10,9,2,6,4~4,2,3,4,9,9,8,4,9,3,5,2,5,8,7,7,6,10,11,2,1,6,10,5,8,8,9,10,11,10,11~9,3,3,4,10,8,2,11,7,11,2,9,8,9,1,6,5,10~9,11,7,9,3,5,7,10,8,10,8,9,6,11,6,10,3,5,8,4,8,5,6,11,9,1,3,4&reel_set5=6,11,8,5,3,8,6,4,9,10,5,8,5,8,10,8,9,5,8,11,4,9,9,10,10,3,10,1,9,3,10,9,4,9,7,6,7,4,3,11,7,10,11~1,9,11,5,9,7,5,4,2,2,3,6,11,2,8,11,10,7,11,10,8,5,5,3,2,4,3,9,3,4,10,6,8,9,10,8,11~9,7,5,1,4,8,11,10,6,4,10,8,3,1,8,2,3,4,5,10,11,8,7,9,11,9,7,3,9,5,6,5,9,2,10,11,10,5,2,6,2,8,4,11~1,2,2,3,3,4,5,6,7,8,8,9,9,10,10,11,11~1,3,3,4,5,6,7,8,8,9,9,10,11,11&reel_set8=5,4,11,8,10,5,6,7,1,7,4,10,9,10,8,7,9,9,3,3,8,10,9,5,5,8,11,11,7,9,10,9,10,9,10,4,3,4,4,8,11,11,8,6,3~9,2,11,5,6,1,10,11,3,11,2,5,6,8,4,2,7,4,9,11,10,9,8~10,11,11,7,3,5,11,9,10,4,9,4,11,5,9,5,1,7,5,4,8,10,10,9,9,6,2,8,3,1,10,4,8,5,8,6,2,3,7,6,2,8,2~2,3,8,11,9,5,5,6,10,8,3,9,7,4,11,1,2,4,10,11,6,8,9~1,10,8,11,9,4,4,3,10,8,9,3,8,10,6,7,3,6,11,6,8,10,3,6,7,10,5,1,5,7,9,8,9,3,8,11,5,9,5,9,8,9,3,11,5,4,7,6,11,10,4&reel_set7=9,7,8,4,11,5,3,1,9,8,3,6,10,10,4,9,8~8,4,2,8,11,6,9,3,9,9,11,4,10,2,2,3,5,9,4,10,6,9,11,11,9,10,6,7,8,5,3,1,5,2,8,7,11,6,11,2,10~2,11,8,9,5,10,11,11,7,2,9,10,3,4,1,6,8,2,8,9,6,10,9,3,5,4,6,7,5,4,5,3,10,5~1,2,2,3,3,4,5,6,7,8,8,9,9,10,10,11,11~9,4,7,11,5,5,8,11,7,4,5,6,11,9,7,9,8,8,5,6,10,10,6,4,8,1,9,10,11,3,9,8,4,6,1,10,7,8,8,11,3,3,9,10,3,10,8,6,8,5,10,9,3,3&reel_set9=8,9,5,10,9,10,8,4,11,1,6,10,4,11,7,5,3,9,3~9,9,10,8,9,8,10,11,5,9,2,4,11,6,1,3,2,6,11,11,5,2,7~4,6,6,10,2,10,9,9,5,9,3,11,3,2,11,7,7,2,5,9,4,8,2,1,6,4,11,1,10,10,8,4,3,5,4,3,2,10,8,8,9,5,10,11,11,6,7,9,9,5,8,8,6,11~4,2,7,2,10,6,9,3,8,11,1,5,11,6,5,8,3,10,9,11,9,10,2~3,7,8,4,5,8,9,1,5,5,8,7,11,11,9,10,9,3,6,6,10,7,8,8,3,11,9,9,8,3,6,10,6,9,1,11,4,8,5,10,11,4,10,9,9,10,5,10,11,4,3,10,4,7,6&total_bet_min=10.00";
            }
        }

        protected override int FreeSpinTypeCount
        {
            get { return 8; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            switch (freeSpinGroup)
            {
                case 0:
                    return new int[] { 200, 201 };
                case 1:
                    return new int[] { 202, 203 };
                case 2:
                    return new int[] { 204, 205 };
                case 3:
                    return new int[] { 206, 207 };
            }
            return null;
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 75.0; }
        }
        #endregion

        public WildBoosterGameLogic()
        {
            _gameID = GAMEID.WildBooster;
            GameName = "WildBooster";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in WildBoosterGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in WildBoosterGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";

        }
       
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul","fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total","n_reel_set",
                "s", "w", "tw", "acci", "accm" , "accv" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]) || resultParams.ContainsKey(toCopyParams[i]))
                    continue;

                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            if (!resultParams.ContainsKey("na") || resultParams["na"] != "fso")
            {
                resultParams.Remove("fs_opt");
                resultParams.Remove("fs_opt_mask");
            }
            return resultParams;
        }
    }
}
