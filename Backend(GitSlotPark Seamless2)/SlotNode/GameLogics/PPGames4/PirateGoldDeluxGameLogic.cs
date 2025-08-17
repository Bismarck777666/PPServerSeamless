using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class PirateGoldDeluxGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs40pirgold";
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
            get
            {
                return 20;
            }
        }
        protected override int ServerResLineCount
        {
            get
            {
                return 20;
            }
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
        protected override string InitDataString
        {
            get
            {
                return "def_s=6,7,4,2,8,4,3,5,6,7,8,5,7,3,4,4,3,5,6,7&cfgs=4331&ver=2&mo_s=13&def_sb=3,4,7,6,8&mo_v=2,4,8,12,16,20,40,60,80,100,160,200,240,300,400,1000,4000,20000&reel_set_size=2&def_sa=8,7,5,3,7&mo_jp=1000;4000;20000&scatters=1~0,0,0~0,0,0~1,1,1&gmb=0,0,0&rt=d&gameInfo={rtps:{purchase:\"94.46\",regular:\"94.48\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"23809500\",max_rnd_win:\"15000\"}}&wl_i=tbm~15000&mo_jp_mask=jp3;jp2;jp1&sc=10.00,20.00,30.00,40.00,50.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,2000.00,3000.00,4000.00,5000.00&defc=100.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;250,50,10,2,0;250,50,10,2,0;150,25,10,1,0;150,25,10,1,0;100,20,6,1,0;100,20,6,1,0;40,10,4,0,0;40,10,4,0,0;20,8,4,0,0;20,8,4,0,0;0,0,0,0,0;0,0,0,0,0&rtp=94.48&total_bet_max=10,000,000.00&reel_set0=11,8,8,3,9,5,7,5,3,3,3,4,10,6,6,6,4,8,8,10,6,11,7,11,6,13,13,13,10,6,3,11,7,12,6,13,4,4,4~13,13,13,11,3,2,2,2,11,5,10,3,3,3,13,9,3,3,13,9,7,5,13,2,4,4,4,6,9,2,12,8~3,3,3,13,9,7,9,8,4,13,13,13,5,10,12,13,8,8,8,6,2,2,2,4,4,4,5,5,12,5,11,5,2,11,11,10,6,3,12,7,8,8~5,5,5,11,4,4,4,10,7,7,7,13,3,3,3,13,13,13,13,8,8,8,4,5,5,13,4,9,5,11,5,10,9,4,10,2,2,2,12,9,9,6,6,6,8,9,2,10~8,8,8,8,6,6,9,4,7,7,7,9,9,6,9,8,13,13,13,9,12,5,5,5,13,4,4,4,9,3,3,3,11,10,5,9,5,10,9,11,5,3,5,9,4,9,3,11,3,13,6,6,6,2,2,2&reel_set1=3,7,3,3,3,5,7,5,5,3,3,5,5,3,7,3,7,7,7,5,5,5,3,9,11~8,8,8,10,12,4,6,6,6,6,4,6,4,4,4,6,6,6,4,6,6,4,4,4,4~8,9,10,11,12,4,6,5,7,7,3,3,3,5,4,4,4,5,4,5,7,3,7,4,4,5,5,3,7,4,3,3,7,3,7,7,5,3,7,6~8,8,8,9,10,11,12,6,6,4,4,4,4,7,5,6,4,6,6,6,5,5,5,4,4,4,7,7,7,3,3,3,7,4,6,4,4,3,4,7,4,7,3,4,5,6,7,6,4,5~8,8,8,4,4,4,5,4,3,3,3,6,6,5,7,6,6,3,4,5,5,5,6,3,7,7,6,6,6,5,6,4,5,4,3,6,5,7,7,7,7,6,6,6,4,9,10,11,12&purInit=[{type:\"wbg\",bet:2000,game_ids:[2]}]&total_bet_min=10.00";
            }
        }
        #endregion

        public PirateGoldDeluxGameLogic()
        {
            _gameID = GAMEID.PirateGoldDelux;
            GameName = "PirateGoldDelux";
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in PirateGoldDeluxGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in PirateGoldDeluxGameLogic::readBetInfoFromMessage {0}", ex);
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

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            if (!resultParams.ContainsKey("s") && spinParams.ContainsKey("s"))
                resultParams["s"] = spinParams["s"];
            return resultParams;
        }
    }
}
