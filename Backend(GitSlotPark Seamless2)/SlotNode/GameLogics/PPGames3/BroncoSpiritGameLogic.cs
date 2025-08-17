using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Newtonsoft.Json;
using Akka.Actor;
using Akka.Util;
using Microsoft.Extensions.Logging;

namespace SlotGamesNode.GameLogics
{
    public class BroncoSpiritBetInfo : BasePPGroupedSlotBetInfo
    {
        public override float TotalBet
        {
            get { return BetPerLine * 25.0f; }
        }
    }
    public class BroncoSpiritGameLogic : BaseSelFreeGroupedPPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs75bronco";
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
                return 75;
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
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=11,13,14,10,12,8,7,5,13,7,13,9,15,14,11,14,9,15,7,10&cfgs=3760&ver=2&reel_set_size=4&def_sb=10,10,10,10,5&def_sa=12,12,12,12,15&scatters=1~0,0,0,0,0,0~0,0,0,0,0~1,1,1,1,1,1&gmb=0,0,0&rt=d&sc=10.00,20.00,30.00,40.00,50.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,2000.00,3000.00,4000.00,5000.00&defc=100.00&wilds=2~250,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;250,60,20,0,0;125,25,10,0,0;125,25,10,0,0;75,20,8,0,0;75,20,8,0,0;50,12,5,0,0;50,12,5,0,0;20,7,3,0,0;20,7,3,0,0;20,7,3,0,0;20,7,3,0,0;20,7,3,0,0&rtp=94.60&reel_set0=12,12,12,12,15,15,15,15,7,7,7,7,8,8,8,8,12,13,13,13,13,11,11,11,11,6,6,6,6,11,12,14,7,14,11,14,10,10,10,10,9,9,9,9,12,13,10,11,8,14,15,9,6,14,13,15,5,5,5,5,15,7,14,14,11,8,7,13,3,8,15,11,15,10,4,4,4,4,8,11,13,9,13,11,12,10,8,11,13,6,9,15,13,14,6,12,10,3,6,15,11,5,12,13,14,15,7,14,11,14,13,12,15,13,15,3,7,15,7,9,11,12,5,1,12,15,13,13,12,15,12,12,14,4,9,10,10,13,14,1,14,11,10~7,7,7,7,15,15,15,15,7,15,15,9,9,9,9,13,13,13,13,3,10,10,10,10,11,11,11,11,11,10,13,14,14,14,14,5,5,5,5,8,8,8,8,14,14,14,8,12,12,12,12,3,6,6,6,6,8,7,12,11,14,14,10,10,12,7,13,13,5,15,10,10,10,12,15,12,12,9,5,13,13,13,14,11,11,6,10,9,7,15,13,12,7,9,11,11,7,11,13,9,1,6,13,12,11,8,15,6,8,9,3,10,15,8,11,12,8,12,11,5,13,5,12,13,13,13,14,14,7,4,4,4,4,7,8,10,14,7,14,15,9,6,1,15,4,1,10,9~15,15,15,15,14,14,14,14,1,9,9,9,9,11,11,11,11,7,7,7,7,13,13,13,13,15,4,4,4,4,3,14,9,13,11,11,8,8,8,8,8,11,10,10,10,10,15,7,5,5,5,5,13,12,12,12,12,8,9,14,11,9,13,9,5,10,6,6,6,6,7,8,13,15,12,12,7,14,12,10,11,15,9,3,14,7,7,3,8,11,12,7,8,14,15,6,15,6,10,14,5,9,12,8,14,10,10,8,9,13,12,15,11,10,9,14,12,13,15,11,1,13,15,13,3,14,11,5,15,12,8,13,11,13,5,8,13,12,6,6,13,13,15,13,7,14,12~7,7,7,7,14,14,14,14,8,8,8,8,5,5,5,5,8,14,11,11,11,13,13,13,13,14,14,13,8,11,7,12,12,12,12,14,13,12,11,9,9,9,9,12,13,5,11,10,10,10,10,14,5,15,15,15,15,15,1,11,7,6,6,6,6,10,6,12,7,3,13,15,3,13,8,9,15,11,3,11,11,8,15,12,13,9,15,10,13,5,1,6,15,7,7,8,13,4,4,4,4,14,12,6,13,7,7,10,14,12,6,10,15,8,1,12,15,13,14,13,14,12,3,12,9,13,11,13,9,12,9,14,11,12,10,15,5,5,10,10,6,10,11,5,8,8,12,9,11~10,10,10,10,5,5,5,5,6,6,6,6,6,12,12,12,12,12,9,9,9,9,15,15,15,15,6,12,13,13,13,13,14,14,14,14,7,7,7,7,10,13,15,10,12,7,15,11,11,11,11,8,8,8,8,12,13,5,12,4,4,4,4,5,7,15,5,15,15,15,13,3,14,11,14,6,15,6,12,15,7,15,6,11,13,8,5,12,9,10,3,8,12,11,5,13,8,14,9,13,1,14,7,11,15,14,13,6,12,10,14,11,11,7,8,7,11,10,8,8,13,10,10,14,9,14,1,5,12,9,3,11,9,6,1,13,8,4,11,14,14,12,10,5,11,13,7&accInit=[{id:0,mask:\"cp;tp;s;sa;msa\"}]&reel_set2=9,11,9,10,5,4,4,4,4,11,7,1,13,13,8,12,10,13,11,9,8,10,12,14,14,11,4,15,6,15,8,14,11,14,15,7,7,12,12,13,11,8,13,10,14,8,12,12,14,9,13,9,13,12,15~13,8,7,6,8,14,11,7,14,10,10,9,15,12,13,15,4,4,4,4,14,13,11,12,10,11,7,5,14,15,14,13,13,15,12,6,8,15,9,1,11,12,12,10,4,13,1,9,11,15,4,13,6,12,10,12,11,5,8,14,5~13,8,15,12,11,15,8,4,4,4,4,8,13,10,14,9,15,9,5,7,12,6,14,13,10,7,13,7,9,12,1,11,9,8,7,8,11,5,15,8,11,6,4,9,7,14,12,13,12,15,5,10,13,10,13,10,6,12,14~10,11,8,12,15,9,15,11,11,10,7,8,15,12,7,13,6,11,4,4,4,4,13,11,10,6,7,7,15,14,12,5,10,13,14,14,12,13,14,11,5,13,9,9,12,11,10,9,6,8,13,8,14,7,10,14,13,1~10,9,8,14,11,13,4,4,4,4,13,15,7,9,10,9,15,11,8,8,12,14,14,11,12,9,6,15,10,7,12,13,9,14,15,12,5,12,11,12,13,5,7,15,8,13,9,13,1,4,11,8,14,15,13,10&reel_set1=8,13,12,8,13,15,14,7,7,11,9,13,9,12,11,7,15,12,10,9,14,9,11,14,10,4,4,4,4,10,13,15,14,14,12,12,6,11,15,12,12,15,12,5,6,8,11,7,8,10,8,9,14,13,9,15,14,4,14,12,11,7,8,12,14,8,7,5,12,14,13,12,12,10,11,11,10,15,8,15,11,4,6,1,13,11,11,10,11,1,15,7,10,10,9,13,10,6,14,12,8,12,14,9,13,4,13,7,13,13~9,12,14,1,5,4,4,4,4,15,15,9,13,8,4,13,12,11,9,13,15,12,15,12,13,8,11,15,6,6,13,4,15,10,14,4,13,12,13,11,9,14,8,7,13,8,15,10,14,10,7,7,13,8,12,12,8,12,11,10,1,10,14,13,6,7,12,6,11,12,13,7,14,11,10,15,11,8,9,8,11,13,10,5,5,12,15,7,15,4,10,6,14,14,5,7,13,11,13,11,14,9,11,12,14,11,8,9,9,12~12,8,7,8,11,6,11,7,8,7,14,12,4,4,4,4,14,9,12,12,14,13,8,9,15,7,9,9,14,4,6,15,11,5,12,9,13,13,12,12,11,10,6,10,13,12,5,12,15,9,13,9,12,9,4,8,9,15,13,12,15,13,6,13,15,10,8,10,10,7,14,11,13,10,12,8,5,1,12,14,11,9,11,13,6,14,14,7,13,1,5,15,13,8,10,11,7,15,10,8,15,13,11,8,7,14,7,10,9,11,10,13,9~10,11,13,15,13,8,7,9,13,11,10,5,15,9,13,14,5,15,6,7,12,9,5,12,7,15,14,9,14,6,10,15,12,13,14,1,15,8,14,9,12,13,8,8,7,10,10,7,14,13,13,14,14,5,11,12,10,9,13,11,6,11,13,11,12,4,4,4,4,11,7,8,12,7,1,4,14,7,8,5,10,11,6,13,13,12,14,12,12,11,12,11,9,1,15,9,8,13,10,12,7,10,13,14,14,6,8,11,15,11,9,11~11,1,11,8,4,4,4,4,10,15,6,13,14,9,14,7,7,7,7,7,12,6,7,12,10,12,7,14,14,10,9,9,7,5,8,11,7,10,13,13,15,14,9,12,8,13,9,10,9,14,13,9,12,13,10,9,15,15,12,11,5,6,8,8,10,9,10,10,14,13,15,13,15,7,8,8,15,14,9,11,11,12,9,12,15,14,1,4,7,11,8,12,15,15,12,12,13,8,14,8,11,13,12,14,13,15,11,13,11,7,11,10,5,13,7,11,15,8&reel_set3=11,14,11,14,11,12,6,14,12,11,14,13,14,12,13,7,13,11,14,15,10,13,8,5,4,4,4,4,12,14,11,14,11,1,14,15,14,13,15,9,12,15,11,15,13,11,15,4,15,13,13,15,13,7,15~8,13,12,11,11,11,13,11,12,14,15,15,8,1,14,15,13,14,11,12,13,11,10,15,14,13,12,15,12,9,13,13,11,10,14,15,10,9,12,11,14,9,7,12,11,4,4,4,4,15,6,15,13,14,5,15,11,13,13,12,7,14~15,11,11,7,9,15,12,13,14,10,12,14,13,12,11,10,11,5,15,11,13,12,6,11,13,12,11,9,13,13,8,11,15,15,9,12,4,4,4,4,1,14,14,15,12,13,9,15,7,13,8,11,14,12,12,15,14~15,5,14,7,15,14,15,14,7,11,12,9,15,9,11,10,8,11,12,13,7,13,11,11,12,11,14,14,1,13,8,15,10,12,13,11,7,12,13,11,10,13,14,8,13,14,12,14,4,4,4,4,15,12,12,10,6,14,15~12,12,11,12,12,11,15,13,4,4,4,4,13,12,10,11,10,7,13,8,14,11,14,7,9,9,6,14,14,11,12,1,5,8,15,13,15,11,12,14,14,10,9,15,15,12,11,13,14,15,14,15,14,13,13,11,15";
            }
        }
        protected override int FreeSpinTypeCount
        {
            get { return 6; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            switch(freeSpinGroup)
            {
                case 0:
                    return new int[] { 200, 201 };
                case 1:
                    return new int[] { 202, 203 };
                case 2:
                    return new int[] { 204, 205 };
            }
            return null;
        }
        #endregion
        public BroncoSpiritGameLogic()
        {
            _gameID = GAMEID.BroncoSpirit;
            GameName = "BroncoSpirit";
        }

        protected override void onDoCollect(int websiteID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    _logger.Error("{0} result information has not been found in BasePPSlotGame::onDoCollect.", strGlobalUserID);
                    return;
                }

                BasePPSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                if (result.NextAction != ActionTypes.DOCOLLECT)
                {
                    _logger.Error("{0} next action is not DOCOLLECT just {1} in BasePPSlotGame::onDoCollect.", strGlobalUserID, result.NextAction);
                    return;
                }

                int index = (int)message.Pop();
                int counter = (int)message.Pop();
                Dictionary<string, string> responseParams = new Dictionary<string, string>();
                responseParams.Add("balance", Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_cash", Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_bonus", "0.00");
                responseParams.Add("na", "s");
                responseParams.Add("stime", GameUtils.GetCurrentUnixTimestampMillis().ToString());
                responseParams.Add("sver", "5");
                responseParams.Add("index", index.ToString());
                responseParams.Add("counter", (counter + 1).ToString());

                GITMessage reponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOCOLLECT);
                string strCollectResponse = convertKeyValuesToString(responseParams);
                reponseMessage.Append(strCollectResponse);

                Context.System.Scheduler.ScheduleTellOnce(150, Sender, new ToUserMessage((int)_gameID, reponseMessage), Self);

                addActionHistory(strGlobalUserID, "doCollect", strCollectResponse, index, counter);
                saveHistory(websiteID, strUserID, index, counter, userBalance, currency);

                result.NextAction = ActionTypes.DOSPIN;
                saveBetResultInfo(strGlobalUserID);

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BroncoSpiritGameLogic::onDoCollect {0}", ex);
            }
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
            dicParams["accm"]     = "cp~tp~s~sa~msa";
            dicParams["acci"]     = "0";
            dicParams["accv"]     = "0~10~3~0~20";
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            BroncoSpiritBetInfo broncoBetInfo = betInfo as BroncoSpiritBetInfo;

            List<TrailInfo> trails = new List<TrailInfo>();
            foreach(KeyValuePair<double, SelectedGroupSequence> pair in broncoBetInfo.SeqPerBet)
            {
                TrailInfo info = new TrailInfo();
                if (Math.Round(broncoBetInfo.TotalBet, 2) == pair.Key)
                    continue;

                if (pair.Value.IsEnded)
                    continue;

                info.counter   = pair.Value.LastID;
                info.data      = pair.Value.Samples[pair.Value.LastID - 1].CumulatedGroupSum.ToString();
                info.bet       = Math.Round(pair.Key, 2);
                trails.Add(info);
            }
            if(trails.Count > 0)
            {
                dicParams["trail"] = "bets~" + JsonConvert.SerializeObject(trails);
            }
        }
        protected override void processSlotSpinData(BasePPSlotSpinData spinData, int index, int group, int cumBonusSum)
        {
            for(int i = 0; i < spinData.SpinStrings.Count; i++)
            {
                string strSpinData = spinData.SpinStrings[i];
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinData);
                if (!dicParams.ContainsKey("accv"))
                    continue;

                dicParams["accv"] = string.Format("{0}~10~3~{1}~20", index, cumBonusSum);
                spinData.SpinStrings[i] = convertKeyValuesToString(dicParams);
            }
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                BroncoSpiritBetInfo betInfo = new BroncoSpiritBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount  = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BroncoSpiritGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in BroncoSpiritGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            BroncoSpiritBetInfo betInfo = new BroncoSpiritBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }

        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul","fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total","reel_set",
                "s", "purtr", "w", "tw", "accm", "accv",  "acci"};
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

        public class TrailInfo
        {
            public string data      { get; set; }
            public int    counter   { get; set; }
            public double bet       { get; set; }
        }
    }
}
