using GITProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class FirebirdSpiritBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 25.0f;
            }
        }
    }
    class FirebirdSpiritGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaysconcoll";
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
            get { return 25; }
        }
        protected override int ROWS
        {
            get
            {
                return 5;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=8,9,19,11,12,19,11,10,7,6,7,19,11,12,8,9,11,19,4,12,3,8,6,19,7,6,19,8,12,19&cfgs=6530&ver=3&mo_s=18;19;15&mo_v=1250,5000,100000;50,75,100,150,200,250,300,400,500,750,1000,1250,1500,2000,2500,3000,4000,5000,7500,10000,20000;5,8,10,15,20,25&def_sb=12,9,9,6,12,19&reel_set_size=2&def_sa=8,6,6,10,11,19&mo_jp=1250;5000;100000&scatters=1~0,0,0,0,0,0~0,0,0,0,0,0~1,1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"11210762\",max_rnd_win:\"4250\"}}&wl_i=tbm~4250&mo_jp_mask=jp1;jp2;jp3&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&purInit_e=1&wilds=2~0,0,0,0,0,0~1,1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,50,25,10,0,0;0,20,15,8,0,0;0,15,10,5,0,0;0,10,8,5,0,0;0,8,5,3,0,0;0,8,5,3,0,0;0,5,3,2,0,0;0,5,3,2,0,0;0,5,3,2,0,0;0,5,3,2,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=5,5,5,5,5,9,7,3,10,4,4,4,4,4,3,10,11,12,8,8,8,8,8,7,12,7,7,7,6,4,12,12,12,12,12,1,3,10,8,8,8,5,12,11,11,11,11,11,8,4,8,4,8,3,3,3,3,3,5,7,11,12,10,10,10,10,10,11,7,8,7,7,7,7,7,6,8,9,9,9,9,9,11,8,7,8,1,10,7,6,6,6,6,6,11,12,10,5,5,5,6,12,4,4,4~4,5,9,1,10,6,7,3,12,12,12,12,12,2,2,2,2,2,6,5,1,1,1,1,1,12,9,7,11,11,11,11,11,12,11,6,6,6,6,6,8,6,7,6,7,6,9,3,9,10,10,10,10,10,6,2,9,9,9,9,9,5,7,6,12,11,5,6,3,3,3,3,3,4,7,6,8,2,8,4,4,4,4,4,12,10,6,2,10,5,8,8,8,8,8,1,8,9,6,2,5,7,11,6,3,10,7,7,7,7,7,6,11,2,5,5,5,5,5,6,2,6,12,6,3,6,6,6,6,6,8,2,6,7,4,5,8,7,6,2,6,9,6,9,6,3,6,6,2,5,6,7,6,12,7,5,7,8,5,6,5,6,7,2,7,6,9,1,6,10,7,6,8,6,2,11,8,7,2,6,12,6~12,12,12,12,12,4,2,2,2,2,2,5,7,10,10,10,10,10,12,12,1,1,1,1,1,9,5,11,11,11,11,11,3,2,12,8,11,9,6,8,8,8,8,8,5,7,10,12,3,9,1,9,9,9,9,9,12,10,4,9,12,1,4,10,3,3,3,3,3,1,9,4,5,8,8,8,4,6,4,2,9,10,3,5,4,4,4,4,4,9,3,7,7,7,7,7,1,2,6,6,6,6,6,5,2,1,10,12,10,5,5,5,5,5,3,9,2,1,10,4,9,5,12,8,12,1,2,8,6,10,3,9,1,9,1,2,1,4,1,10,9,1,3,1,8,2,1,2,10,6,2,12,4,9,1,4,2,1,9,1,9,12,5,9~1,1,1,1,1,9,8,12,12,12,12,12,3,2,2,2,2,2,10,12,11,3,3,3,3,3,12,11,10,6,6,6,6,6,3,10,12,11,5,7,4,4,4,4,4,5,6,2,7,5,5,5,5,5,9,4,10,10,10,10,10,11,3,8,9,6,5,10,11,11,11,11,11,8,6,4,9,9,9,9,9,2,5,8,8,8,8,8,2,4,12,7,7,7,7,7,10,8,12,4,7,6,4,6,8,2,8,6,12,4,2,10,11,9,12,4,12,8,4,6,2,12,10,4,7,4,12~6,6,6,6,6,11,12,12,12,12,12,6,8,3,3,3,3,3,1,1,1,1,1,11,11,11,11,11,5,12,10,1,7,10,10,10,10,10,4,9,11,1,7,12,9,9,9,9,9,10,1,8,4,4,4,4,4,7,10,8,1,10,5,5,5,5,5,3,1,10,5,3,8,8,8,8,8,7,1,8,10,7,7,7,7,7,4,8,4,9,10,6,3,5,1,5,6,6,6,4,1,11,10,1,12,10,3,12,8,1,10,8,12,1,9,4,3,4,7,10,4,8,10,3,7,5,3,11,1,7,10,4,8,1,4,11,3,10,1,5,11~19,18,19,19,17,19,19,15,19,19,15,19,19,15,17,19,15&reel_set1=7,7,7,5,12,12,12,12,12,11,10,1,1,1,1,6,3,3,3,3,3,7,1,10,10,10,8,9,4,5,3,12,11,11,11,11,11,5,5,5,4,4,4,4,4,6,6,6,6,6,3,3,3,12,8,4,10,1,10,12,10,4,10,6,12,4,3,1,6,4,11,12,10,1,10,11,10,7,7,7,7,7,9,10,3,1,10,8,9,3,6,7,9,10,6,5,5,5,5,5,12,3,10,1,12,9,5,4,10,6,4,10,1,4,6,12,10,12,6,8,8,8,8,8,12,1,9,10,4,12,4,10,3,6,8,6,1,7,3,10,1,11,12,4,6,8,12,12,12,3,10,12,3,12,1,4,6,12,8,4,12,4,3,12,9,9,9,9,9,11,12,4,11,4,10,3,11,11,1,6,12,3,6,3,8,12,6,4,10,9,4,8,12,6,11,12,10,12,4,10,12,4,1,3,10,4,12,8,4,6,4,10,4,1,12,11,3,4,1,3,12,10,4,11,10,9,1,4,11,12,10,10,10,10,10,3,8,9,3,6,1,4,6,3,4,1,10,12,4,10,6,4,6,10,6,3,10,9,10,4,11,12,10,11,12,3,12,10,12,3,4,10,6,3,4,7,12,11,12,1,12,10,11,12,3,12,6,10,4,11,6,12,4,6,4,5,4,9,7,1,6,8,12,4,10,3,12,3,11,4,12,6,12,9,4,10,12,3,6,10,6,8,3,6,11,12,10,1,10,6,11,6,3,4,10,4,12,5,9,12,3,6,10,4,10,3,4,3,12,11,10,4,9,4,10,6,8,3,4,3,4,8,10,4,11,12,6,4,11,4,12,4,5,6,4,12,3,11,12,9,8,6,7,1,12,1,10,9,4,12,1,10,4,10,3,12,10,11,10,12,1,12,4,12,10,4,12,1,4,10,4,3,5,12,11,7,4,8,6,12,10,6,11,9,3,12,5,6,12,11,8,12,10,11,10,4,12,10,4,8,11,4,10,12,4,3,12,10,12,10,9,10,8,3,12,10,11,12,4,11,12,1,10,1,12,3,12,12,8,11,3,9,3,4,10,9,1,12,10,11,4,1,10,11,10,4,10,6,4,12,10,8,1,9,3,9~7,7,7,5,12,12,12,12,12,11,10,1,1,1,1,6,3,3,3,3,3,7,1,10,10,10,8,9,4,5,3,12,11,11,11,11,11,5,5,5,4,4,4,4,4,6,6,6,6,6,3,3,3,12,8,4,10,1,10,12,10,4,10,6,12,4,3,1,6,4,11,12,10,1,10,11,10,7,7,7,7,7,9,10,3,1,10,8,9,3,6,7,9,10,6,5,5,5,5,5,12,3,10,1,12,9,5,4,10,6,4,10,1,4,6,12,10,12,6,8,8,8,8,8,12,1,9,10,4,12,4,10,3,6,8,6,1,7,3,10,1,11,12,4,6,8,12,12,12,3,10,12,3,12,1,4,6,12,8,4,12,4,3,12,9,9,9,9,9,11,12,4,11,4,10,3,11,11,1,6,12,3,6,3,8,12,6,4,10,9,4,8,12,6,11,12,10,12,4,10,12,4,1,3,10,4,12,8,4,6,4,10,4,1,12,11,3,4,1,3,12,10,4,11,10,9,1,4,11,12,10,10,10,10,10,3,8,9,3,6,1,4,6,3,4,1,10,12,4,10,6,4,6,10,6,3,10,9,10,4,11,12,10,11,12,3,12,10,12,3,4,10,6,3,4,7,12,11,12,1,12,10,11,12,3,12,6,10,4,11,6,12,4,6,4,5,4,9,7,1,6,8,12,4,10,3,12,3,11,4,12,6,12,9,4,10,12,3,6,10,6,8,3,6,11,12,10,1,10,6,11,6,3,4,10,4,12,5,9,12,3,6,10,4,10,3,4,3,12,11,10,4,9,4,10,6,8,3,4,3,4,8,10,4,11,12,6,4,11,4,12,4,5,6,4,12,3,11,12,9,8,6,7,1,12,1,10,9,4,12,1,10,4,10,3,12,10,11,10,12,1,12,4,12,10,4,12,1,4,10,4,3,5,12,11,7,4,8,6,12,10,6,11,9,3,12,5,6,12,11,8,12,10,11,10,4,12,10,4,8,11,4,10,12,4,3,12,10,12,10,9,10,8,3,12,10,11,12,4,11,12,1,10,1,12,3,12,12,8,11,3,9,3,4,10,9,1,12,10,11,4,1,10,11,10,4,10,6,4,12,10,8,1,9,3,9~8,8,8,2,3,3,3,9,10,7,7,7,11,5,5,5,6,11,10,10,10,5,3,12,1,2,2,2,7,8,9,9,9,4,4,4,5,5,5,1,1,1,1,1,12,12,12,5,8,8,8,3,11,4,11,11,11,3,9,9,9,1,5,6,6,6,7,9,8,6,4,9~4,4,4,4,4,11,9,9,9,9,9,10,3,6,5,6,6,6,6,6,8,1,2,8,8,8,8,8,12,9,1,1,1,1,1,10,11,11,11,11,11,7,3,3,3,3,3,4,10,10,10,10,10,5,5,5,5,5,2,2,2,2,2,7,7,7,7,7,12,12,12,12,12,3,9,9,9,10,1,10,1,6,2,10,7,10,7,2,12,8,6,11,6,1,9,5,11,12,2,1,7,9,10,12,3,10,3,5,8,2,6,2,5,12,2,10,3,7,11,8,12,9,11,1,6,3,7,9,1,12,1,10,12,9,7,11,12,1,11,10,7,10,8,1,9,2,10,2,7,12,7,2,11,3,9,7,1,11,10,7,11,2,5,1,11,4,12,3,2,12,5,10,2,6,10,1,6,1,11,2,10,5,11,10,3,2,6,10,2,3,2,10,5,7,3,12,3,10,1,2,8,1,12,5,1,7,1,10,12,1,3,1,8,12,11,2,4,3,7,11,12,10,9,7,8,10,11,10,12,8,12,11,5,2,11,9,5,12,1,10,7,9,2,1,5,1,8,7,12,7,6,5,7,2,1,2,8,6,1,3,12,5,3,4,11,6,10,2,9,5,6,10,8,11,1,6,12,3,10,5,9,2,1,6~4,4,4,4,4,11,9,9,9,9,9,10,3,6,5,6,6,6,6,6,8,1,2,8,8,8,8,8,12,9,1,1,1,1,1,10,11,11,11,11,11,7,3,3,3,3,3,4,10,10,10,10,10,5,5,5,5,5,2,2,2,2,2,7,7,7,7,7,12,12,12,12,12,3,9,9,9,10,1,10,1,6,2,10,7,10,7,2,12,8,6,11,6,1,9,5,11,12,2,1,7,9,10,12,3,10,3,5,8,2,6,2,5,12,2,10,3,7,11,8,12,9,11,1,6,3,7,9,1,12,1,10,12,9,7,11,12,1,11,10,7,10,8,1,9,2,10,2,7,12,7,2,11,3,9,7,1,11,10,7,11,2,5,1,11,4,12,3,2,12,5,10,2,6,10,1,6,1,11,2,10,5,11,10,3,2,6,10,2,3,2,10,5,7,3,12,3,10,1,2,8,1,12,5,1,7,1,10,12,1,3,1,8,12,11,2,4,3,7,11,12,10,9,7,8,10,11,10,12,8,12,11,5,2,11,9,5,12,1,10,7,9,2,1,5,1,8,7,12,7,6,5,7,2,1,2,8,6,1,3,12,5,3,4,11,6,10,2,9,5,6,10,8,11,1,6,12,3,10,5,9,2,1,6~19,18,19,19,17,19,19,15,19,19,15,19,19,15,17,19,15&purInit=[{bet:2500,type:\"fs\"}]&total_bet_min=8.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100.0; }
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
        public FirebirdSpiritGameLogic()
        {
            _gameID = GAMEID.FirebirdSpirit;
            GameName = "FirebirdSpirit";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
            dicParams["st"] = "rect";
            dicParams["sw"] = "6";
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                FirebirdSpiritBetInfo betInfo = new FirebirdSpiritBetInfo();
                betInfo.BetPerLine            = (float)message.Pop();
                betInfo.LineCount             = (int)message.Pop();

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in FirebirdSpiritGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FirebirdSpiritGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            FirebirdSpiritBetInfo betInfo = new FirebirdSpiritBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new FirebirdSpiritBetInfo();
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("apwa"))
                dicParams["apwa"] = convertWinByBet(dicParams["apwa"], currentBet);

            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

        }
    }
}
