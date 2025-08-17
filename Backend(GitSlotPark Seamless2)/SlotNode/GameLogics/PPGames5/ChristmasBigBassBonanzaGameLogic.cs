using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class ChristmasBigBassBonanzaGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10bxmasbnza";
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
                return 10;
            }
        }
        protected override int ServerResLineCount
        {
            get { return 10; }
        }
        protected override int ROWS
        {
            get
            {
                return 3;
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
                return "def_s=5,6,9,6,6,8,11,8,9,9,6,9,12,6,6&cfgs=4798&ver=2&def_sb=10,7,7,8,11&reel_set_size=5&def_sa=11,10,7,10,4&scatters=1~0,0,0,0,0~20,15,10,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"3907776\",max_rnd_win:\"2100\"}}&wl_i=tbm~2100&sc=20.00,30.00,40.00,50.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,2000.00,3000.00,4000.00,5000.00,6000.00,7000.00,8000.00,10000.00&defc=200.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,200,50,5,0;1000,150,30,0,0;500,100,20,0,0;500,100,20,0,0;200,50,10,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0&total_bet_max=10,000,000.00&reel_set0=6,7,8,10,10,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,9,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,8,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,9,12,9,10,4,3,11,5,9,9,8,7,7,7,7,7~12,3,3,6,7,11,3,5,7,10,4,3,5,10,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,5,10,3,6,4,3,8,7,7,7,7,7~5,7,6,4,12,5,4,8,7,6,4,11,3,9,7,7,10,6,3,5,3,7,7,7,7,7~4,6,7,11,11,4,8,8,7,5,3,12,7,7,10,8,6,9,7,7,7,7,7&accInit=[{id:0,mask:\"cp\"},{id:1,mask:\"cp;mp\"}]&reel_set2=6,7,8,10,10,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,9,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,8,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,9,12,9,10,4,3,11,5,9,9,8,7,7,7,7,7~12,3,3,6,7,11,3,5,7,10,4,3,5,10,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,5,10,3,6,4,3,8,7,7,7,7,7~5,7,6,4,12,5,4,8,7,6,4,11,3,9,7,7,10,6,3,5,3,7,7,7,7,7~4,6,7,11,11,4,8,8,7,5,3,12,7,7,10,8,6,9,7,7,7,7,7&reel_set1=6,7,8,10,10,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,9,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,8,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,9,12,9,10,4,3,11,5,9,9,8,7,7,7,7,7~12,3,3,6,7,11,3,5,7,10,4,3,5,10,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,5,10,3,6,4,3,8,7,7,7,7,7~5,7,6,4,12,5,4,8,7,6,4,11,3,9,7,7,10,6,3,5,3,7,7,7,7,7~4,6,7,11,11,4,8,8,7,5,3,12,7,7,10,8,6,9,7,7,7,7,7&reel_set4=6,7,8,10,10,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,9,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,8,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,9,12,9,10,4,3,11,5,9,9,8,7,7,7,7,7~12,3,3,6,7,11,3,5,7,10,4,3,5,10,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,5,10,3,6,4,3,8,7,7,7,7,7~5,7,6,4,12,5,4,8,7,6,4,11,3,9,7,7,10,6,3,5,3,7,7,7,7,7~4,6,7,11,11,4,8,8,7,5,3,12,7,7,10,8,6,9,7,7,7,7,7&purInit=[{type:\"fsbl\",bet:1000,bet_level:0}]&reel_set3=6,7,8,10,2,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,2,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,2,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,2,12,9,10,4,3,11,5,2,9,8,7,7,7,7,7~12,2,3,6,7,11,3,5,7,10,4,3,5,2,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,2,10,3,6,4,3,8,7,7,7,7,7~5,7,6,2,12,5,2,8,7,6,4,11,3,9,7,7,10,6,3,5,2,7,7,7,7,7~4,6,7,11,2,4,8,2,7,5,3,12,7,7,10,2,6,9,7,7,7,7,7&total_bet_min=20.00";
            }
        }
        #endregion
        public ChristmasBigBassBonanzaGameLogic()
        {
            _gameID = GAMEID.ChristmasBigBassBonanza;
            GameName = "ChristmasBigBassBonanza";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo   = new BasePPSlotBetInfo();
                betInfo.BetPerLine          = (float)message.Pop();
                betInfo.LineCount           = (int)message.Pop();

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in ChristmasBigBassBonanzaGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in ChristmasBigBassBonanzaGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
