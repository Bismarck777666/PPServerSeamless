using Akka.Actor;
using GITProtocol;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Util;

namespace SlotGamesNode.GameLogics
{
    class YumYumPowerWaysResult : BasePPSlotSpinResult
    {
        public int DoBonusCounter { get; set; }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.DoBonusCounter = reader.ReadInt32();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.DoBonusCounter);
        }
    }

    class YumYumPowerWaysGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaysyumyum";
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
                return 6;
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
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=14,11,8,8,12,14,8,3,5,10,9,10,11,5,4,6,5,10,11,9,4,5,7,11,6,7,10,8,6,11,14,13,13,11,11,14&cfgs=4428&ver=2&reel_set_size=17&reel_set=12&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{gamble_reg_8_10:\"44.46%\",max_rnd_sim:\"0\",gamble_pur_6_8:\"42.93%\",gamble_pur_10_12:\"45.50%\",max_rnd_hr:\"196072742\",gamble_reg_4_6:\"50.14%\",max_rnd_win:\"5000\",gamble_pur_4_6:\"50.35%\",gamble_reg_6_8:\"42.88%\",gamble_pur_8_10:\"44.49%\",gamble_reg_10_12:\"45.47%\"}}&wl_i=tbm~5000&reel_set10=12,13,3,3,3,6,6,8,13,8,8,8,11,10,3,7,9,9,9,9,10,6,11,7,7,7,8,13,12,10,10,10,10,11,11,7,11,11,11,8,12,11,12,12,12,12,8,5,10,7,6,6,6,8,13,5,13,13,13,9,6,4,9,5,5,5,7,13,4,5,4,4,4,4,9,9,1,12&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&reel_set11=10,1,1,5,8,13,13,12,8,3,8,11,1,7,1,8,8,8,1,13,1,1,5,13,11,1,10,11,11,12,1,5,11,13,1,5,5,5,4,9,6,13,9,6,1,7,11,6,4,5,12,1,1,7,9,4,4,4,13,12,10,1,10,6,9,3,7,8,1,10,7,1,8,12,12,11&reel_set12=11,3,3,3,6,7,6,8,8,8,4,13,10,9,9,9,8,3,12,7,7,7,7,6,10,10,10,4,10,11,11,11,11,11,9,10,12,12,12,13,5,5,6,6,6,1,12,13,13,13,12,11,8,5,5,5,13,7,12,4,4,4,9,9,8,13~10,3,3,3,4,6,8,8,8,9,8,9,9,9,8,7,7,7,7,5,10,10,10,10,7,12,11,11,11,11,12,11,12,12,12,12,5,6,6,6,11,3,13,13,13,6,13,5,5,5,1,13,4,4,4,13,8,4,9~9,3,3,3,13,11,5,8,8,8,12,10,6,9,9,9,9,9,1,7,7,7,5,8,10,10,10,7,11,4,11,11,11,12,11,7,12,12,12,8,3,12,6,6,6,13,8,13,13,13,12,10,10,5,5,5,6,13,11,4,4,4,13,6,7,4~11,11,4,8,6,4,3,3,3,11,4,5,10,1,10,9,7,8,8,8,12,9,11,12,9,7,11,8,9,9,9,11,12,8,13,7,13,13,3,7,7,7,11,8,13,13,10,13,6,10,10,10,10,8,13,6,13,11,6,11,7,11,11,11,12,7,11,9,10,5,3,8,12,12,12,12,7,9,5,7,12,6,12,6,6,6,8,5,10,12,13,9,9,12,13,13,13,6,13,8,9,7,6,8,8,5,5,5,6,5,10,7,4,13,13,9,4,4,4,12,4,11,10,11,10,5,9,1&purInit_e=1&reel_set13=7,7,12,6,8,13,6,8,13,8,3,3,3,12,11,9,12,6,11,9,9,13,5,12,10,8,8,8,6,10,7,4,10,3,8,7,6,11,12,4,9,9,9,12,7,4,8,13,13,10,9,12,10,13,7,7,7,7,4,11,4,12,13,13,9,5,11,11,9,10,10,10,5,3,5,13,13,6,12,6,8,7,9,10,11,11,11,12,11,8,8,9,11,7,1,8,5,7,6,12,12,12,3,8,11,10,11,11,12,9,7,9,10,12,6,6,6,13,12,13,8,10,11,5,10,7,9,13,13,13,13,5,11,6,8,11,12,5,10,13,11,1,8,5,5,5,7,4,10,9,13,13,10,11,8,12,6,11,4,4,4,12,10,6,9,7,12,12,5,12,6,8,13,10&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&reel_set14=10,13,9,12,10,13,12,4,6,3,9,5,5,4,6,10,13,7,3,3,3,11,6,4,12,12,11,11,3,8,5,13,8,8,11,11,13,10,9,10,8,8,8,13,9,12,10,7,12,12,10,9,8,5,6,11,6,13,10,10,6,5,9,9,9,11,11,9,8,5,6,10,11,9,13,12,9,11,13,7,7,12,11,7,7,7,7,7,4,7,11,7,4,9,13,8,9,7,7,13,8,9,10,8,13,11,13,10,10,10,9,12,12,10,13,8,12,9,12,11,7,11,5,9,6,7,12,13,8,11,11,11,10,7,8,12,5,12,11,6,11,3,1,6,13,9,7,10,9,13,8,12,12,12,6,5,9,8,9,8,10,10,9,8,12,12,6,10,6,7,6,7,9,5,6,6,6,7,13,11,12,10,4,7,13,13,6,4,7,12,5,13,11,13,12,6,13,13,13,12,3,11,8,8,6,5,11,12,5,12,9,13,1,10,4,5,12,11,5,5,5,4,6,12,10,13,8,10,11,10,10,9,11,7,9,7,11,1,6,12,4,4,4,3,7,8,11,8,13,7,13,4,8,13,12,12,9,10,11,11,7,12,12,13&paytable=0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;30,15,10,0,0,0;18,12,8,0,0,0;12,9,6,0,0,0;10,7,4,0,0,0;8,5,3,0,0,0;5,3,2,0,0,0;5,3,2,0,0,0;5,3,2,0,0,0;4,2,1,0,0,0;4,2,1,0,0,0;4,2,1,0,0,0;0,0,0,0,0,0&reel_set15=13,5,12,4,8,3,3,3,6,5,7,9,9,7,10,8,8,8,10,6,6,8,1,11,10,9,9,9,9,5,10,8,10,12,12,7,7,7,8,8,10,6,7,7,4,10,10,10,4,9,13,8,11,11,13,11,11,11,11,9,13,5,8,12,13,12,12,12,9,12,11,9,6,4,6,6,6,6,13,10,13,11,13,11,7,13,13,13,3,3,13,12,12,8,12,5,5,5,12,11,7,5,9,7,9,4,4,4,6,11,10,8,7,13,12,11&reel_set16=13,10,11,13,8,9,1,6,7,11,8,13,3,12,5,7,12,13,9,10,9,3,3,3,9,12,5,12,7,9,13,12,5,13,6,11,11,12,13,8,12,13,10,6,7,11,8,8,8,7,10,8,12,5,4,11,13,8,12,7,10,10,11,13,10,13,3,9,12,8,10,9,9,9,13,7,6,9,3,6,12,10,11,13,8,8,10,7,9,4,9,6,1,9,4,6,7,7,7,8,6,13,8,12,10,5,5,10,5,12,4,8,11,12,11,6,5,13,11,11,13,10,10,10,4,9,10,11,11,13,4,5,6,13,6,13,9,7,9,9,1,6,8,11,12,9,11,11,11,12,13,5,7,11,5,11,5,8,11,11,12,10,9,9,13,13,12,13,3,13,7,12,12,12,10,12,6,4,5,8,9,6,4,7,12,11,12,12,11,13,12,12,8,7,12,13,6,6,6,7,8,10,6,10,6,6,10,11,10,8,13,5,12,11,8,5,6,4,13,6,9,13,13,13,8,10,9,12,7,10,11,5,4,7,8,9,13,11,4,12,9,7,10,11,8,11,5,5,5,9,3,6,7,4,7,11,7,11,6,11,6,7,11,13,10,11,13,12,12,4,8,4,4,4,4,13,8,12,7,13,8,10,7,9,5,10,9,9,3,11,10,10,8,9,12,12,7,7&total_bet_max=10,000,000.00&reel_set0=5,3,3,3,7,10,9,8,8,8,8,11,12,9,9,9,13,13,6,7,7,7,10,7,10,10,10,5,13,13,11,11,11,4,11,12,12,12,12,3,4,12,6,6,6,6,8,13,13,13,6,8,11,5,5,5,12,1,9,4,4,4,9,10,11,7&reel_set2=11,3,3,3,7,7,8,8,8,11,8,11,9,9,9,10,13,7,7,7,4,13,9,10,10,10,1,13,11,11,11,12,6,12,12,12,7,5,8,6,6,6,10,10,13,13,13,4,12,3,5,5,5,6,8,4,4,4,5,12,9,9&t=243&reel_set1=11,12,3,3,3,6,3,5,11,8,8,8,10,13,8,12,9,9,9,7,1,4,7,7,7,4,5,7,7,10,10,10,12,9,5,6,11,11,11,11,7,13,12,12,12,10,6,10,8,6,6,6,11,12,6,10,13,13,13,12,13,13,5,5,5,9,8,9,11,4,4,4,12,9,8,13,10&reel_set4=8,11,6,12,13,11,3,3,3,6,4,11,10,4,7,5,8,8,8,11,10,5,13,13,11,12,8,9,9,9,9,10,13,11,1,7,3,7,7,7,8,5,9,11,10,5,12,8,10,10,10,12,3,4,10,13,9,8,11,11,11,4,13,9,7,6,7,9,12,12,12,8,10,12,10,12,7,8,11,6,6,6,4,6,13,5,12,12,9,13,13,13,9,10,6,12,11,8,8,6,5,5,5,13,10,7,13,12,5,11,4,4,4,6,13,6,9,7,12,7,13,11&purInit=[{type:\"d\",bet:2000}]&reel_set3=6,7,13,10,4,11,5,6,7,13,3,3,3,11,12,5,12,7,7,5,5,10,13,12,8,8,8,7,13,9,7,3,12,13,9,10,6,7,9,9,9,5,11,9,10,9,12,10,13,13,11,8,10,7,7,7,4,10,8,12,13,9,5,6,11,4,7,10,10,10,5,8,11,11,13,13,12,10,13,4,13,11,11,11,12,1,7,1,13,10,9,8,12,8,6,12,12,12,11,7,5,8,9,8,12,11,4,11,12,6,6,6,6,10,13,11,8,10,11,9,13,9,12,5,13,13,13,6,7,6,4,6,9,13,3,9,13,11,5,5,5,10,9,8,6,10,11,1,7,4,3,8,4,4,4,6,12,12,9,11,10,8,12,11,12,11,12,7&reel_set6=4,9,10,13,11,10,12,3,3,3,7,8,11,5,9,11,12,13,8,8,8,11,13,9,6,6,8,7,12,9,9,9,12,11,10,4,11,5,12,10,7,7,7,4,13,13,6,8,13,12,5,10,10,10,9,11,13,10,5,11,9,8,11,11,11,13,6,12,9,11,6,3,13,12,12,12,9,5,4,9,11,10,4,8,6,6,6,12,8,9,7,3,13,8,7,13,13,13,12,8,13,6,5,10,12,8,5,5,5,11,1,7,6,10,13,7,9,4,4,4,7,8,12,10,6,7,12,7,11&reel_set5=12,5,5,12,13,10,8,10,8,13,6,9,13,12,7,11,4,10,6,3,3,3,3,13,9,12,12,13,9,13,8,12,8,9,10,4,11,6,10,7,8,10,8,8,8,11,12,4,7,12,9,6,7,13,8,11,5,6,11,7,12,6,6,12,9,9,9,9,6,13,8,7,11,13,11,9,8,3,9,12,7,3,12,7,8,5,11,5,12,7,7,7,11,4,5,10,13,7,11,13,8,8,13,5,12,9,4,12,10,1,8,10,10,10,10,10,8,12,10,11,1,7,5,12,11,11,12,9,6,13,7,13,12,7,8,11,11,11,13,11,12,7,13,6,10,8,6,11,6,4,6,9,8,13,13,9,5,13,12,12,12,4,12,4,10,5,10,10,9,11,9,13,13,12,6,9,10,8,4,6,12,11,6,6,6,8,7,7,12,9,5,6,11,13,8,10,7,13,9,11,9,6,8,9,7,13,13,13,12,6,9,5,7,10,3,10,9,4,12,7,10,10,4,13,13,11,11,9,5,5,5,4,12,10,11,11,10,11,13,12,6,5,11,5,3,8,1,7,12,13,11,4,4,4,7,13,13,6,10,9,7,13,4,11,9,11,7,10,12,9,7,7,11,8,5,11&reel_set8=1,13,12,11,11,6,7,10,1,11,1,8,6,10,8,1,12,1,8,13,12,11,1,10,13,3,1,3,8,8,8,1,10,13,9,1,5,10,5,7,7,13,11,1,11,6,9,4,8,1,4,7,1,5,8,1,1,3,1,13,5,5,5,9,9,11,11,6,10,7,1,12,1,10,1,1,4,6,1,13,12,8,9,1,5,11,1,1,6,12,1,8,4,4,4,5,7,7,11,12,1,12,12,13,9,5,4,1,1,8,10,1,12,1,13,1,12,13,9,13,8,7,7,10,13,11&reel_set7=13,7,12,3,7,11,9,12,10,11,13,3,3,3,9,11,3,13,13,5,4,7,10,7,6,5,12,8,8,8,13,7,13,7,7,5,12,10,11,11,8,8,4,9,9,9,5,7,4,5,12,11,11,7,9,1,6,10,5,7,7,7,12,8,10,5,11,4,12,9,9,6,12,7,9,10,10,10,8,9,13,9,10,13,13,12,10,5,12,11,11,11,11,11,9,6,13,11,5,6,7,9,8,11,8,5,12,12,12,6,4,12,13,12,13,11,10,6,11,13,9,8,6,6,6,7,11,8,10,10,8,4,10,12,13,9,11,6,13,13,13,12,12,10,12,8,13,11,8,1,4,6,6,12,5,5,5,3,8,12,11,7,10,13,8,4,9,10,3,13,4,4,4,6,8,6,9,7,10,9,6,9,10,13,11,13,12&reel_set9=1,13,10,7,3,10,1,11,1,12,9,12,8,13,1,13,1,1,7,7,9,11,12,5,11,10,1,10,8,12,4,13,8,8,8,6,7,13,11,10,1,12,5,1,7,13,1,10,11,11,3,5,8,9,11,1,6,1,8,12,12,1,6,1,13,3,9,1,11,5,5,5,1,1,13,1,8,1,1,4,13,9,13,4,13,5,4,12,1,4,9,1,11,9,1,13,12,7,11,11,6,1,1,10,12,1,4,4,4,1,5,1,8,5,7,8,6,8,11,8,1,7,1,12,10,6,1,1,9,1,13,9,10,6,11,7,1,7,12,5,10,8,12,13&total_bet_min=10.00";
            }
        }

        protected override int FreeSpinTypeCount
        {
            get { return 5; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            if (freeSpinGroup == 4)
                return new int[] { 204 };

            List<int> freeSpinTypes = new List<int>();
            for (int i = 0; i < 5; i++)
                freeSpinTypes.Add(200 + i);

            return freeSpinTypes.ToArray();
        }
        #endregion

        public YumYumPowerWaysGameLogic()
        {
            _gameID = GAMEID.YumYumPowerWays;
            GameName = "YumYumPowerWays";
        }
        protected override void onDoCollect(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
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

                Context.System.Scheduler.ScheduleTellOnce(100, Sender, new ToUserMessage((int)_gameID, reponseMessage), Self);

                addActionHistory(strGlobalUserID, "doCollect", strCollectResponse, index, counter);
                saveHistory(agentID, strGlobalUserID, index, counter, userBalance, currency);
                if (_dicUserFreeSpinInfos.ContainsKey(strGlobalUserID))
                {
                    addFreeSpinBonusParams(reponseMessage, _dicUserFreeSpinInfos[strGlobalUserID], strGlobalUserID, 0.0);
                    checkFreeSpinCompletion(reponseMessage, _dicUserFreeSpinInfos[strGlobalUserID], strGlobalUserID);
                }

                result.NextAction = ActionTypes.DOSPIN;
                saveBetResultInfo(strGlobalUserID);

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in YumYumPowerWaysGameLogic::onDoCollect {0}", ex);
            }
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in YumYumPowerWaysGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in YumYumPowerWaysGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["st"] = "rect";
            dicParams["sw"] = "6";
            dicParams["g"] = "{s0:{def_s:\"12,8,8,11\",def_sa:\"14\",def_sb:\"14\",reel_set:\"0\",s:\"12,8,8,11\",sa:\"14\",sb:\"14\",sh:\"4\",st:\"rect\",sw:\"1\"},s1:{def_s:\"8,11,11,6\",def_sa:\"14\",def_sb:\"14\",reel_set:\"1\",s:\"8,11,11,6\",sa:\"14\",sb:\"14\",sh:\"4\",st:\"rect\",sw:\"1\"},s2:{def_s:\"13,13,11,11\",def_sa:\"14\",def_sb:\"14\",reel_set:\"2\",s:\"13,13,11,11\",sa:\"14\",sb:\"14\",sh:\"4\",st:\"rect\",sw:\"1\"},s3:{def_s:\"11,11,10,10\",def_sa:\"14\",def_sb:\"14\",reel_set:\"3\",s:\"11,11,10,10\",sa:\"14\",sb:\"14\",sh:\"4\",st:\"rect\",sw:\"1\"},s4:{def_s:\"3,5,10,9,5,4,6,5,9,4,5,7,7,10,8,6\",def_sa:\"14,14,14,14\",def_sb:\"14,14,14,14\",s:\"3,5,10,9,5,4,6,5,9,4,5,7,7,10,8,6\",sa:\"14,14,14,14\",sb:\"14,14,14,14\",sh:\"4\",st:\"rect\",sw:\"4\"}}";
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst, PPFreeSpinInfo freeSpinInfo)
        {
            try
            {
                YumYumPowerWaysResult spinResult = new YumYumPowerWaysResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);
                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

                spinResult.NextAction   = convertStringToActionType(dicParams["na"]);
                spinResult.ResultString = convertKeyValuesToString(dicParams);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                if (spinResult.NextAction == ActionTypes.DOBONUS)
                    spinResult.DoBonusCounter = findFreeSpinTypeFromStatus(dicParams["status"]);

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in YumYumPowerWaysGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        private int findFreeSpinTypeFromStatus(string strStatus)
        {
            switch (strStatus)
            {
                case "1,0,0,0,0":
                    return 0;
                case "0,1,0,0,0":
                    return 1;
                case "0,0,1,0,0":
                    return 2;
                case "0,0,0,1,0":
                    return 3;
                case "0,0,0,0,1":
                    return 4;
            }
            return 0;
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            YumYumPowerWaysResult result = new YumYumPowerWaysResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected string buildStatus(int index)
        {
            int[] intArray = new int[] { 0, 0, 0, 0, 0 };
            if (index < 0)
                intArray[0] = 1;
            else
                intArray[index] = 1;
            return string.Join(",", intArray);
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("tmb_win"))
                dicParams["tmb_win"] = convertWinByBet(dicParams["tmb_win"], currentBet);
            if (dicParams.ContainsKey("tmb_res"))
                dicParams["tmb_res"] = convertWinByBet(dicParams["tmb_res"], currentBet);
            if (dicParams.ContainsKey("wlc_v"))
            {
                string[] strParts = dicParams["wlc_v"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strValues = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    strValues[1] = convertWinByBet(strValues[1], currentBet);
                    strParts[i] = string.Join("~", strValues);
                }
                dicParams["wlc_v"] = string.Join(";", strParts);

            }
        }
        protected Dictionary<string, string> buildDoBonusResponse(YumYumPowerWaysResult result, BasePPSlotBetInfo betInfo, BasePPSlotStartSpinData startSpinData, int doBonusCounter, int ind, bool isNextMove)
        {
            Dictionary<string, string> dicParams = new Dictionary<string, string>();

            var freeSpinCounts      = new int[] { 4, 6, 8,10,12};
            var dicLastParams       = splitResponseToParams(result.ResultString);
            dicParams["tw"]         = dicLastParams["tw"];
            dicParams["bgid"]       = "1";
            dicParams["wins"]       = "4,6,8,10,12";
            dicParams["coef"]       = Math.Round(betInfo.TotalBet, 2).ToString();
            dicParams["level"]      = result.DoBonusCounter.ToString();
            dicParams["status"]     = buildStatus(result.DoBonusCounter);
            dicParams["bgt"]        = "35";
            dicParams["wins_mask"]  = "nff,nff,nff,nff,nff";
            dicParams["wp"]         = "0";

            if (ind == 0)
            {
                dicParams["fsmul"]  = "1";
                dicParams["fsmax"]  = freeSpinCounts[result.DoBonusCounter].ToString();
                dicParams["na"]     = "s";
                dicParams["fswin"]  = "0.00";
                dicParams["rw"]     = "0.00";
                dicParams["fs"]     = "1";
                dicParams["lifes"]  = "1";
                dicParams["end"]    = "1";
                dicParams["fsres"]  = "0.00";
                return dicParams;
            }
            else
            {
                if (!isNextMove)
                {
                    dicParams["rw"]     = "0.0";
                    dicParams["lifes"]  = "0";
                    dicParams["end"]    = "1";
                    if (double.Parse(dicParams["tw"]) > 0.0)
                        dicParams["na"] = "c";
                    else
                        dicParams["na"] = "s";
                }
                else
                {
                    dicParams["lifes"]  = "1";
                    dicParams["end"]    = "0";
                    dicParams["na"]     = "b";
                }
                return dicParams;
            }
        }

        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency, bool isAffiliate)
        {
            try
            {
                int index = (int)message.Pop();
                int counter = (int)message.Pop();
                double realWin = 0.0;
                string strGameLog = "";
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo       betInfo = _dicUserBetInfos[strGlobalUserID];
                    YumYumPowerWaysResult   result  = _dicUserResultInfos[strGlobalUserID] as YumYumPowerWaysResult;
                    if ((result.NextAction != ActionTypes.DOBONUS) || (betInfo.SpinData == null) || !(betInfo.SpinData is BasePPSlotStartSpinData))
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        bool isCollect      = false;
                        bool isEnded        = false;
                        int ind             = (int)message.Pop();
                        var startSpinData   = betInfo.SpinData as BasePPSlotStartSpinData;
                        int stage           = result.DoBonusCounter;
                        Dictionary<string, string> dicParams = null;
                        do
                        {
                            if (ind == 0)
                            {
                                isCollect = true;
                                break;
                            }
                            double[] moveProbs = new double[] { 0.5014, 0.4288, 0.4446, 0.4547 };
                            if (betInfo.SpinData.IsEvent || PCGSharp.Pcg.Default.NextDouble(0.0, 1.0) <= moveProbs[stage])
                            {
                                result.DoBonusCounter++;
                                if (result.DoBonusCounter < 4)
                                {
                                    dicParams = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 1, true);
                                }
                                else
                                {
                                    isCollect = true;
                                    break;
                                }
                            }
                            else
                            {
                                result.DoBonusCounter--;
                                if(result.DoBonusCounter < 0)
                                {
                                    dicParams           = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 1, false);
                                    double selectedWin  = startSpinData.StartOdd * betInfo.TotalBet;
                                    double maxWin       = startSpinData.MaxOdd * betInfo.TotalBet;

                                    //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                                    if (!startSpinData.IsEvent && !isAffiliate)
                                        sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                                    else if (maxWin > selectedWin)
                                        addEventLeftMoney(agentID, strGlobalUserID, maxWin - selectedWin);
                                    isEnded = true;
                                }
                                else
                                {
                                    dicParams = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 1, true);
                                }
                            }
                        } while (false);

                        if (isCollect)
                        {
                            BasePPSlotSpinData freeSpinData = convertBsonToSpinData(startSpinData.FreeSpins[result.DoBonusCounter]);
                            preprocessSelectedFreeSpin(freeSpinData, betInfo);

                            betInfo.SpinData = freeSpinData;
                            List<string> freeSpinStrings = new List<string>();
                            for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                                freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd));

                            betInfo.RemainReponses = buildResponseList(freeSpinStrings, ActionTypes.DOSPIN);
                            double selectedWin = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                            double maxWin = startSpinData.MaxOdd * betInfo.TotalBet;

                            //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                            if (!startSpinData.IsEvent && !isAffiliate)
                                sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                            else if (maxWin > selectedWin)
                                addEventLeftMoney(agentID, strGlobalUserID, maxWin - selectedWin);

                            dicParams = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 0, false);
                        }
                        else
                        {
                            //필요없는 응답을 삭제한다.
                            betInfo.pullRemainResponse();
                        }

                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                        string strResponse = convertKeyValuesToString(dicParams);
                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);

                        result.NextAction = nextAction;
                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        if (isEnded)
                        {
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;

                            string freeSpinID = null;
                            if (_dicUserFreeSpinInfos.ContainsKey(strGlobalUserID) && !_dicUserFreeSpinInfos[strGlobalUserID].Pending)
                            {
                                freeSpinID = _dicUserFreeSpinInfos[strGlobalUserID].FreeSpinID;
                                addFreeSpinBonusParams(responseMessage, _dicUserFreeSpinInfos[strGlobalUserID], strGlobalUserID, realWin);
                            }

                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win = realWin;
                            }
                            else
                            {
                                saveHistory(agentID, strGlobalUserID, index, counter, userBalance, currency);
                                if (_dicUserFreeSpinInfos.ContainsKey(strGlobalUserID) && !_dicUserFreeSpinInfos[strGlobalUserID].Pending)
                                    checkFreeSpinCompletion(responseMessage, _dicUserFreeSpinInfos[strGlobalUserID], strGlobalUserID);
                            }
                            resultMsg = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID = betInfo.BetTransactionID;
                            resultMsg.RoundID = betInfo.RoundID;
                            resultMsg.TransactionID = createTransactionID();
                            resultMsg.FreeSpinID    = freeSpinID;
                        }
                        saveBetResultInfo(strGlobalUserID);
                    }
                }
                if (resultMsg == null)
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(resultMsg);

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseSelFreePPSlotGame::onFSOption {0}", ex);
            }
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

            if (!resultParams.ContainsKey("g") && spinParams.ContainsKey("g"))
                resultParams["g"] = spinParams["g"];
            return resultParams;
        }

        protected override async Task buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd)
        {
            if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                await base.buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsTotalRandom, minOdd, maxOdd);
            else
                await base.buildStartFreeSpinData(startSpinData, buildType, minOdd, maxOdd);
        }
    }
}
