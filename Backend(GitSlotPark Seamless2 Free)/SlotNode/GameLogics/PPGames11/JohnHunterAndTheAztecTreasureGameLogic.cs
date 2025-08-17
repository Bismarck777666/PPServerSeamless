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
    class JohnHunterAndTheAztecTreasureBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 20.0f; }
        }
    }
    class JohnHunterAndTheAztecTreasureGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs7776secrets";
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
            get { return 7776; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
        }
        protected override int ROWS
        {
            get
            {
                return 6;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "msi=13~14&def_s=9,3,11,6,6,11,5,9,11,4,6,12,4,9,9,6,8,11,9,9,1,3,11,9,9,4,3,8,4,4&msr=8~2&cfgs=2443&nas=15&ver=2&reel_set_size=2&def_sb=5,9,3,10,3&def_sa=4,9,7,9,10&bonusInit=[{bgid:0,bgt:21,bg_i:\"5,10,15,20,25\",bg_i_mask:\"nff,nff,nff,nff,nff\"},{bgid:1,bgt:21,bg_i:\"10,15,20,25\",bg_i_mask:\"nff,nff,nff,nff\"},{bgid:2,bgt:21,bg_i:\"15,20,25\",bg_i_mask:\"nff,nff,nff\"},{bgid:3,bgt:21,bg_i:\"3,4,5,10,15\",bg_i_mask:\"nff,nff,nff,nff,nff\"},{bgid:4,bgt:21,bg_i:\"4,5,10,15\",bg_i_mask:\"nff,nff,nff,nff\"},{bgid:5,bgt:21,bg_i:\"5,10,15\",bg_i_mask:\"nff,nff,nff\"},{bgid:6,bgt:21,bg_i:\"10,15\",bg_i_mask:\"nff,nff\"}]&prg_cfg_m=rtfs_left&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&prg_cfg=0&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&aw_reel_count=6&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&aw_reel0=m~2;m~3;m~5;m~7;m~10&aw_reel2=m~3&aw_reel1=m~2&n_reel_set=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;80,30,15,0,0;50,25,12,0,0;30,15,10,0,0;20,12,8,0,0;20,12,8,0,0;15,10,5,0,0;10,8,5,0,0;10,8,5,0,0;8,6,4,0,0;8,6,4,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=7,6,7,8,12,4,5,5,8,8,10,11,11,6,10,4,9,9,7,7,7,3,3,8,5,8,10,11,11,11,8,9,6,6,10,10,12,12,6,9,9,9,9,1,7,8,7,7,10,12,12,5,12,5,8,4,11,7,3~11,10,9,5,9,10,10,2,11,12,12,12,1,3,3,5,5,10,10,8,11,11,9,9,8,8,7,7,7,8,8,8,11,11,11,12,4,4,12,12,6,6,9,9,9,5,5,7,7,6,6,6,2,2,5,10,10,1,10,3,4,4,7,6,8,6,5,4,8,8,10,4,4~7,9,11,9,4,4,11,11,8,8,2,2,9,1,3,3,11,8,7,7,11,12,12,6,6,9,9,4,4,1,7,10,10,11,2,9,7,12,7,9,9,9,5,3,8,5,12,12,4,4,10,11,1,8,5,5,11,8,6,8,4,10,10,10,4,5,12,12,6~10,6,11,9,6,6,8,8,12,5,5,5,5,11,1,9,9,8,8,11,3,3,4,4,2,2,4,6,11,11,5,5,12,12,7,7,6,3,3,4,4,9,9,7,8,10,10,2,11,4,11,10,10,1,6,11,11,12,12,12,6,8,8,5,7,10,4~4,5,10,8,4,4,7,12,12,8,8,9,3,6,6,4,7,7,6,8,1,10,8,8,8,9,5,5,7,7,7,6,6,6,3,3,4,10,5,12,11,11,11,5,5,9,10,10,9,9,12,12,11,4,11,11,5,8&t=243&reel_set1=7,6,7,8,12,12,12,4,9,9,5,5,8,10,11,11,6,9,10,3,3,1,6,6,10,10,11,11,11,12,6,9,9,9,9,3,7,7,8,7,11,10,12,12,5,12,5,8,4,11,7,3~11,10,9,5,5,9,10,11,11,1,7,7,9,6,8,2,2,3,8,7,7,7,8,8,12,10,10,10,4,4,12,12,6,9,9,9,5,7,2,5,10,10,4,10,12,12,12,3,4,7,6,8,6,5,4,8,8,10,4,4~7,9,11,9,4,11,11,8,8,2,9,6,11,12,3,3,4,7,1,7,8,10,10,11,9,7,2,2,6,6,6,12,7,7,10,9,5,5,5,3,8,12,12,4,4,10,9,11,8,5,5,12,11,8,6,6,8,10,4,10,5,12,6~10,6,11,9,7,6,6,8,8,11,11,10,10,10,12,3,9,9,7,4,1,6,11,11,11,5,5,8,2,12,12,7,6,4,9,9,9,7,8,3,3,10,10,11,4,11,10,6,11,12,12,12,6,8,8,11,10,5,7,11,10,4~4,5,10,8,4,4,7,10,12,12,1,8,8,9,6,6,6,7,7,9,5,12,3,6,6,6,10,7,7,7,5,12,12,11,8,8,8,5,5,9,10,9,12,11,4,11,11,5,8&aw_reel4=m~7&aw_reel3=m~5&aw_reel5=m~10";
            }
        }
	
	
        #endregion
        public JohnHunterAndTheAztecTreasureGameLogic()
        {
            _gameID = GAMEID.JohnHunterAndTheAztecTreasure;
            GameName = "JohnHunterAndTheAztecTreasure";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }

        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            JohnHunterAndTheAztecTreasureBetInfo betInfo = new JohnHunterAndTheAztecTreasureBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new JohnHunterAndTheAztecTreasureBetInfo();
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                JohnHunterAndTheAztecTreasureBetInfo betInfo = new JohnHunterAndTheAztecTreasureBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f || float.IsNaN(betInfo.BetPerLine) || float.IsInfinity(betInfo.BetPerLine))
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in JohnHunterAndTheAztecTreasureGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in JohnHunterAndTheAztecTreasureGameLogic::readBetInfoFromMessage {0}", ex);
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
