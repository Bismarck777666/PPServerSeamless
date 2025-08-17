using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class FiveLionsBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 50.0f; }
        }
    }
    public class FiveLionsGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs243lions";
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
                return 1024;
            }
        }
        protected override int ServerResLineCount
        {
            get { return 50; }
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
                return "def_s=14,14,14,14,14,8,9,5,10,3,9,1,13,5,8,6,12,7,12,5&cfgs=2211&nas=14&ver=2&reel_set_size=2&def_sb=6,7,7,9,7&def_sa=3,5,4,13,9&wrlm_sets=2~0~1,2,3,5,8,10,15,30,40~1~2,3,5~2~3,5,8~3~5,8,10~4~8,10,15~5~10,15,30~6~15,30,40&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&sc=0.01,0.02,0.05,0.10,0.20,0.50,1.00,2.00&defc=0.05&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,100,50,0,0;800,100,35,0,0;800,100,30,0,0;300,50,20,0,0;300,35,15,0,0;200,30,10,0,0;200,20,10,0,0;100,15,10,0,0;100,15,10,0,0;100,15,5,0,0;100,10,5,0,0;0,0,0,0,0&rtp=95.51&reel_set0=4,8,7,10,13,3,8,1,5,11,13,6,12,9,4,8,7,10,13,3,8,1,5,11,13,6,12,9~5,9,11,2,13,11,3,10,4,8,6,1,7,12,5,9,2,10,13,11,3,10,4,8,6,1,7,12~4,2,3,10,5,13,7,9,1,8,5,12,6,11,4,9,3,10,5,13,7,2,4,8,5,12,6,11~6,11,2,13,11,4,9,5,12,3,10,7,8,1,6,11,5,13,11,4,9,5,12,3,10,7,8,3~9,3,8,5,9,13,6,8,10,4,11,1,7,12,9,3,8,5,9,13,6,8,10,4,11,3,7,12&t=243&reel_set1=4,8,7,10,13,3,8,1,5,11,13,6,12,9,4,8,7,10,13,3,8,1,5,11,13,6,12,9~5,9,11,2,13,11,3,10,4,8,6,1,7,12,5,9,2,10,13,11,3,10,4,8,6,1,7,12~4,2,3,10,5,13,7,9,1,8,5,12,6,11,4,9,3,10,5,13,7,2,4,8,5,12,6,11~6,11,2,13,11,4,9,5,12,3,10,7,8,1,6,11,5,13,11,4,9,5,12,3,10,7,8,3~9,3,8,5,9,13,6,8,10,4,11,1,7,12,9,3,8,5,9,13,6,8,10,4,11,3,7,12";
            }
        }
        
        protected override int FreeSpinTypeCount
        {
            get { return 7; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203, 204, 205, 206 };
        }
        #endregion

        public FiveLionsGameLogic()
        {
            _gameID = GAMEID.FiveLions;
            GameName = "FiveLions";
        }

        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            FiveLionsBetInfo betInfo = new FiveLionsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                FiveLionsBetInfo betInfo    = new FiveLionsBetInfo();
                betInfo.BetPerLine          = (float)message.Pop();
                betInfo.LineCount           = (int)message.Pop();                

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in FiveLionsGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FiveLionsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, int currency, double userBalance, int index, int counter)
        {
            base.setupDefaultResultParams(dicParams, currency, userBalance, index, counter);
            dicParams["n_reel_set"] = "0";
        }

        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul","fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total","n_reel_set",
                "s", "purtr", "w", "tw" };
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
