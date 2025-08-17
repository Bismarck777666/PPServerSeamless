using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class GreekGodsBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 25.0f; }
        }
    }
    class GreekGodsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs243fortseren";
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
            get { return 243; }
        }
        protected override int ServerResLineCount
        {
            get { return 25; }
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
                return "wof_mask=w,w,w,w,w,w,w,w,w,w,w,w&wof_set=10,15,20,25,30,35,40,50,75,150,250,1000&def_s=2,3,4,3,2,2,3,4,3,2,2,3,4,3,2&cfgs=2600&ver=2&reel_set_size=2&def_sb=6,5,9,7,10&def_sa=10,4,7,11,9&pb_imw=0.00;0.00;0.00&scatters=1~0,0,0,0,0,0,0,0,0~12,11,10,9,8,0,0,0,0~1,1,1,1,1,1,1,1,1;12~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~15,14,13,12,11,10,9,8,7,6,5,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&gmb=0,0,0&rt=d&pb_iv=2~aw~50;3~fs~8;4~bg~0&pb_iw=0;0;0&pb_im=r;r;r&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&n_reel_set=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;150,60,25,0,0;125,50,20,0,0;100,40,10,0,0;75,30,10,0,0;50,25,5,0,0;50,20,5,0,0;50,20,5,0,0;50,15,5,0,0;50,15,5,0,0;0,0,0,0,0&reel_set0=7,4,4,9,6,11,6,11,7,4,4,11,6,5,7,9,10,4,4,7,6,3,3,10,11,9,10,6,5,6,7,5,8,6,10~2,2,6,8,5,11,4,4,11,6,5,9,7,11,8,2,2,9,5,9,8,3,3,5,11,9,5,10,7,5,9~11,10,7,8,11,8,7,6,8,10,3,3,7,6,8,1,1,1,5,10,8,3,3,9,6,8,4,4,11,6,8,6,11,3,3,8,11,8,10~2,2,2,2,11,3,3,5,4,4,8,1,1,1,7,10,11,7,5,11,7,10,9,4,4,9,10,5,11,8,6,4,4,3,3,11,11~11,9,5,8,3,3,9,10,1,1,1,7,9,4,4,9,3,3,5,6,10,8,3,3,11,6,4,4,8,5,10,4,7,11&t=243&reel_set1=4,4,5,7,6,12,12,12,4,4,9,6,8,6,5,7,6,10,9,4,4,7,9,6,3,3,12,12,12,6,11,10,7,5,6,10,9~2,2,2,2,6,9,11,4,4,11,5,2,2,11,9,11,12,12,12,8,5,9,3,3,9,2,2,8,12,12,12,10,7,9~11,8,7,10,4,8,8,3,3,7,11,6,7,12,12,12,8,5,10,6,8,3,3,8,9,12,12,12,4,8,11,6,8,6,11,9,3,3,5,10~2,2,2,2,11,3,3,9,4,4,12,12,12,7,11,9,2,2,11,6,9,12,12,12,9,5,12,12,12,9,7,4,4,3,3,10,11,8,11~10,5,11,8,5,8,3,3,9,10,12,12,12,7,10,9,4,4,10,10,3,3,7,5,12,12,12,10,9,3,3,10,6,11,4,4,8,9,5,12,12,12,5,11";
            }
        }
	
	
        #endregion
        public GreekGodsGameLogic()
        {
            _gameID = GAMEID.GreekGods;
            GameName = "GreekGods";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("pb_mw"))
            {
                string[] strParts = dicParams["pb_mw"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for(int i = 0; i < strParts.Length; i++)
                    strParts[i] = convertWinByBet(strParts[i], currentBet);

                dicParams["pb_mw"] = string.Join(";", strParts);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            GreekGodsBetInfo betInfo = new GreekGodsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                GreekGodsBetInfo betInfo = new GreekGodsBetInfo();
                betInfo.BetPerLine       = (float)message.Pop();
                betInfo.LineCount        = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f || float.IsNaN(betInfo.BetPerLine) || float.IsInfinity(betInfo.BetPerLine))
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in GreekGodsGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in GreekGodsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "gsf_r", "gsf" };
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
