using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.IO;

namespace SlotGamesNode.GameLogics
{
    public class CleocatraBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 20.0f;
            }
        }
    }
    public class CleocatraGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20cleocatra";
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
                return 40;
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
        protected override string InitDataString
        {
            get
            {
                return "def_s=6,13,12,5,9,7,8,7,7,10,5,10,10,6,13,4,11,8,4,11&cfgs=6332&ver=3&def_sb=4,11,6,4,10&reel_set_size=5&def_sa=4,10,13,7,12&scatters=1~5,5,5,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"950000\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;400,100,30,0,0;250,75,25,0,0;150,40,15,0,0;100,25,10,0,0;75,15,7,0,0;50,10,5,0,0;30,6,3,0,0;30,6,3,0,0;20,5,2,0,0;20,5,2,0,0;20,5,2,0,0&total_bet_max=10,000,000.00&reel_set0=4,11,8,3,9,10,12,8,5,6,12,9,13,7,1,5,5,5,5,10,7,11,7,4,11,8,13,11,12,4,11,5,13,8,10,3,3,3,3,4,13,4,8,7,11,5,9,12,12,13,10,11,13,10,3,4,4,4,4,12,10,9,7,12,10,7,11,13,13,8,9,13,10,11,7,3,6,6,6,6,8,6,11,3,13,8,12,9,10,9,6,12,9,5,10,6,5,12~7,2,12,11,10,12,9,7,11,5,12,4,3,4,11,12,13,6,5,9,7,13,11,3,6,2,13,8,11,13,6,12,11,12,9,6,4,9,9,10,13,6,9,7,13,12,8,5,10,9,9,11,8,12,7,3,12,8,1,7,11,8,13,8,10,13,12,6,7,12,11,8,10,10,9,13,10,5~10,3,8,11,13,6,9,8,10,7,13,8,1,9,10,13,5,4,10,11,12,8,5,9,11,7,5,13,5,13,9,11,8,7,11,8,2,10,4,2,11,6,12,13,3,12,6,4,7,9,6,9,5,12,12,6~13,13,9,4,8,6,1,12,11,12,8,7,8,11,10,10,7,10,9,13,10,8,2,7,11,9,3,10,12,6,9,12,6,13,12,11,10,4,5,8,13,5,11,13,11,6,7,12,5,10,3,4,5,13,6,7,11,9~9,12,13,13,10,12,10,7,10,13,11,4,13,5,11,4,12,9,10,5,10,6,12,6,7,6,11,13,11,11,3,9,4,12,6,13,3,6,9,7,9,13,4,1,10,12,8,13,11,13,5,11,5,7,10,10,9,8,6,5,3,7,8,9,3,2,4,3,9,11,7,5,8,12,9,8,1,5,13,7,8,7,6,4,13,11,12&reel_set2=8,6,9,6,7,10,4,8,10,13,12,6,10,5,5,5,5,8,13,7,9,9,4,12,12,13,10,3,9,7,13,11,3,3,3,3,11,10,5,7,8,13,12,7,4,10,13,3,10,13,13,4,4,4,4,10,13,12,5,11,12,12,11,7,12,6,9,12,3,6,6,6,6,9,4,5,11,9,4,11,5,8,11,10,8,11,5,8,11~12,13,13,6,7,13,10,7,3,6,8,9,5,8,12,10,10,8,11,7,12,12,13,7,4,2,5,9,11,12,5,13,3,2,12,9,11,11,9,9,4,8,11,6,10~11,13,7,5,11,2,8,12,4,5,2,8,9,13,9,11,6,3,9,10,8,11,6,8,10,8,6,13,5,9,10,4,12,13,5,8,10,11,11,12,4,12,10,11,7,13,5,7,9,3,12,13,10,9,6,10,2,12,7,9,12,11,7,13,13,9,3,8~11,7,13,2,11,5,2,12,13,13,12,2,8,10,9,5,7,12,10,10,12,13,12,5,9,13,7,6,11,7,10,11,10,3,11,8,13,8,9,6,10,13,4,6,9,6,7,10,3,6,11,4,8,4,11,5~8,7,10,6,8,9,4,13,11,13,5,13,13,8,6,11,13,11,13,12,5,2,11,6,10,10,3,5,7,5,11,10,7,5,4,11,13,7,6,13,7,3,10,10,9,8,12,3,9,3,8,9,4,5,12,2,12,9,7,6,9,11,4,12,12,9,6&reel_set1=7,12,8,13,11,6,4,11,12,6,11,13,9,13,13,6,12,8,10,11,9,10,11,9,5,10,10,8,11,9,9,4,12,13,10,6,8,11,10,11,6,12,8,5,11,5,10,10,7,4,12,3,9,9,7,8,7,10,7,5,11,13,8,13,3,8,9,7,1,3,10,13,8,7~9,12,13,9,6,11,10,1,4,10,11,8,13,7,5,8,6,12,13,3,13,2,4,9,6,12,12,11,6,9,12,12,4,7,8,9,8,9,10,5,9,13,7,2,12,11,13,7,2,5,9,10,8,11,12,13,3,10,11,12,5,6,7,13~5,8,11,11,2,11,9,8,13,1,9,10,5,7,11,7,3,4,8,9,13,12,9,8,6,7,3,8,6,13,6,2,13,6,11,12,3,12,10,13,7,12,8,13,5,6,4,9,9,13,9,8,10,1,9,5,7,5,8,10,9,12,12,10,4,12,7,11,8,11~2,13,10,13,6,8,10,9,9,13,13,8,5,2,8,11,7,6,11,13,3,9,7,6,13,10,12,11,10,1,5,4,8,3,9,7,11,7,11,10,10,6,9,12,10,4,7,10,13,11,12,5,12,5,12,6,11,4,13,12,11,7~12,13,4,9,10,8,2,9,5,10,9,11,7,4,1,11,6,1,9,13,2,11,13,6,13,7,3,6,12,12,8,2,9,7,5,6,4,13,3,8,9,10,5,11,8,13,4,10,8,13,4,12,11,5,7,12,5,3,1,10,8,11,11,12,13,7,10,7,3,13,9,6,2,9,11,5&reel_set4=4,6,10,8,6,12,6,4,10,12,10,4,10,10,12,8,8,6,8,8,10,10,8,6,10,8,12,10,4,8,8,12,12~9,9,13,3,7,9,7,9,9,5,9,5,11,11,3,13,13,11,7,13,7,11,5,11,13,13,7~2,7,6,12,7,9,6,3,13,11,11,13,4,12,12,8,5,11,6,10,7,8,5,11,8,10,9,13,8,4,5,9,9,13,7,5,11,13,8,9,12,4,12,10,3~8,13,12,3,8,11,9,6,13,10,7,9,13,5,9,11,8,12,10,2,10,11,9,8,12,12,13,3,13,7,11,4,7,13,10,4,6,10,6,10,11,12,8,12,11,5,8,10,13,7,13,7,6,5,2,9,6,7,11,4,11,10~5,7,13,5,7,13,2,10,12,12,4,5,6,4,11,13,13,10,5,9,10,2,10,9,12,7,10,10,9,9,13,3,9,8,11,12,11,8,11,3,6,13,3,12,5,8,9,7,13,13,11,11,9,10,6,13,8,7,11,6,12,6,2,13,4,8,11,8,9,7,5,12,13,7,4,2,9,7,6,3,5,11,8,5,12,4,2,13,10,6,11,9&purInit=[{bet:2000,free_spins:\"8,12,16\",type:\"free_spins\"}]&reel_set3=11,10,10,7,8,7,9,11,11,12,9,12,13,9,9,13,4,10,13,12,7,8,9,8,10,6,11,7,13,8,11,12,9,13,8,10,12,10,12,11,13,13,10,5,8,12,11,7~9,8,13,9,11,13,4,11,13,9,9,7,12,7,12,11,12,10,10,11,12,10,11,9,8,2,13,3,6,12,13,2,4,13,3,8,9,5,11,13,7,9,6,8,7,6,8,5,12,12,11,5,6,10,2,10,10,12~10,4,9,2,7,8,12,8,9,12,8,12,13,6,3,5,13,12,13,9,8,13,11,2,4,11,8,13,7,12,10,10,9,5,8,3,6,7,13,9,11,5,3,9,4,6,11,5,12,11,11,6,10,10,13,12,11,7,5,8,9,9,12,11,7,11,2,5,13,8,10,7,4,13,9,9,10,6,10,8,11,13,6~13,2,6,10,13,6,11,3,10,10,12,4,6,13,5,11,6,9,8,4,7,12,10,7,3,10,8,6,7,8,9,9,6,8,5,7,12,11,11,10,13,9,9,2,8,12,13,13,11,2,11,10,10,11,13,5,7,8,11,7,12,12,5,4,13~13,4,8,4,7,6,10,11,6,8,7,10,6,2,8,5,4,9,3,9,7,11,4,11,8,10,13,11,12,13,13,5,12,10,5,9,13,3,11,9,9,2,11,7,6,12,7,10,13,13,9,12,8,6,12,5,3&total_bet_min=10.00";
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
        #endregion
        public CleocatraGameLogic()
        {
            _gameID = GAMEID.Cleocatra;
            GameName = "Cleocatra";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                CleocatraBetInfo betInfo = new CleocatraBetInfo();
                betInfo.BetPerLine       = (float)message.Pop();
                betInfo.LineCount        = (int)message.Pop();

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in CleocatraGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in CleocatraGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            CleocatraBetInfo betInfo = new CleocatraBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
            dicParams["st"] = "rect";
            dicParams["sw"] = "5";
        }
    }
}
