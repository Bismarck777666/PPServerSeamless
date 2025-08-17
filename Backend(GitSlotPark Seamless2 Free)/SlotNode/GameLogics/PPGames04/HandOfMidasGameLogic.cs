using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using SlotGamesNode.Database;
using Akka.Actor;
using System.IO;
using PCGSharp;
using GITProtocol.Utils;
using MongoDB.Bson;

namespace SlotGamesNode.GameLogics
{
    public class HandOfMidasBetInfo : BasePPSlotBetInfo
    {
        public int PurchaseType { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.PurchaseType = reader.ReadInt32();        
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.PurchaseType);
        }

    }
    class HandOfMidasGameLogic : BasePPSlotGame
    {
        protected double[] _totalFreeSpinWinRates = new double[3];
        protected double[] _minFreeSpinWinRates   = new double[3];
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20midas";
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
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100.0; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        protected override double MoreBetMultiple
        {
            get { return 1.25; }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=4,11,6,6,5,13,10,9,13,7,10,9,3,4,11&cfgs=4488&ver=2&def_sb=5,1,12,13,6&prm=2~1,2,3;2~1,2,3&reel_set_size=10&def_sa=6,2,5,3,13&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={rtps:{ante:\"94.51\",purchase:\"94.51\",regular:\"94.51\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"1026437\",max_rnd_win:\"5000\",max_rnd_win_a:\"4000\"}}&wl_i=tbm~5000;tbm_a~4000&sc=10.00,20.00,30.00,40.00,50.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,2000.00,3000.00,4000.00,5000.00&defc=100.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&bls=20,25&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;400,120,40,0,0;300,90,30,0,0;250,70,25,0,0;200,50,20,0,0;150,35,15,0,0;100,20,10,0,0;50,10,5,0,0;50,10,5,0,0;25,5,2,0,0;25,5,2,0,0;25,5,2,0,0&rtp=94.51&total_bet_max=10,000,000.00&reel_set0=10,11,4,6,8,9,3,1,7,13,5,12,1,8,1,8,1,3,1,8,13,3,8,3,8,3,8,11,1,11,5,8,7,3,1,8,12,5,8,11,8,4,1,6,8,1,8,7,3,6,8,4,1,5,3,8,3,7,8,11,6,3,1,8,6,3,8,1,3,1,8,4,8,6,8,12,5,6,4~12,1,13,7,11,8,2,5,10,3,6,4,9,5,3,4,3,13,9,2,7,4,2,7,3,1,9,7,1,7,1,8,3,10,3,13,7,8,9,7,2,7,1,7,9,3,8,2,9,3,7,2,3,13,2,3,1,10~3,8,4,1,5,11,10,2,13,6,7,9,12,8,11,8,6,9,8,7,13,8,5,12,10,8,6,8,12,11,12,9,5,12,8,10,11,8,5,9,6,11,8,11,8,1,8,11,12,9,8,12,10,2,12,8,2,9,8,12,9,8,7,5,11,12~7,3,4,2,8,5,13,10,12,6,11,1,9,3,12,10,11,6,1,6,1,10,6,1,4,11,10,6,1,6,1,12,3,10,1,4,6,11,8,6,4,12,11,9,10,1,13,6,10,4,3,10~7,13,9,12,8,1,6,5,3,11,4,10,11,1,6,13,5,13,5,8,12,1,13,11,3,6,13,1,6,1,6,12,3,1,11,6,11,3,12,1,13,8,12,1,11,1,11,1,11,1,3,5&reel_set2=12,8,11,5,7,6,3,13,1,9,10,4,8,5,10,9,8,1,6,5~12,7,13,2,8,6,11,5,3,10,9,4,10,7,5,7,6,3,13,8,6,10,2,5,2,8,2,5,10,2,4,2,6,10,8,5,8,13,5,6,8,2,7,3,5,3,2,5,2,6,2,10,5,2,8,5,2,5,2~10,6,3,8,11,5,9,2,12,13,1,4,7,2,11,3,1,12,2,5,8,6,1,9,11,1,3,9,6,2,1,9,2,6,3,13~6,7,9,2,3,5,4,13,12,10,11,8,10,9,11,9,13,9,7,13,10,13,7,5,9,13,7,11,7,10,7,9,13,9,10,9,13~3,13,6,1,12,8,7,5,10,4,11,9,4,9,6,8,7,8,12,5,8,6,4,6,9,7,4,13,12,7,9,6,7,13&reel_set1=4,4,4,5,6,8,3,3,3,3,5,5,5,9,10,4,12,13,7,11,8,11,3,12,8,12,13,3,12,8,3,8,11,3,12,3,12,5,8,11,5,7,12,5,8,5,13,8,13,9,13,3,13,12,8,13,5,13,12,3,6,12,7,13,5,6,3,11,12,3,10,13,8,13,8,3,8,12,8,12,8,12,8,11,8,12,8,11,3,12,11,9,3,10,12,7,11~12,7,9,2,10,5,6,8,13,3,4,11,7,5,13,3,5,13,6,3,10,5,4,10,5,4,13,5,9,13,10,5,3,5,4,10,13,3,10,13,7,10,3,4,3,11,13,3,4,8,3,13,11,3,13,5,13,4,3,7,4,13,3,11,13,5,13,4,6,13,8,13,10,5,13~5,12,4,9,11,3,10,2,6,7,13,8,10,2,13,2,8,12,4,13,6,12,6,11,6,10,11,13,2,10,3,6,11,9,3,11,10,9,12,7,6,2,13,10,11,2,11,13,2,8,12,10,7,3,2,11,12,11,10,9,10~12,2,8,9,4,13,10,5,6,3,7,11,9,13,8,5,11,10,11,10,8,9,3,13,2,13,2,13,3,8,9,10,8,3,5,9,8,7,3,9,13,7,3,7,3,6,13~12,3,3,3,7,9,6,3,10,5,8,11,4,13,4,4,4,5,5,5,3,6,3,9,4,3,4,10,5,7,5,9,3,5,9,4,3,4,3,5,9,4,11,9,4,13,4,11,4,5,4,9,6,3,9,3,9,4,3,5,3,5,3,10,3,4,6,11,6,5,4,3,13,3,11,5,4,5,9,3,4,5,9,5,3,11,5,6,4,6,5,3,10,4,9,10,9,4,5,9,4,7,5,3,4,3,4,6&reel_set4=12,7,9,13,4,11,1,3,5,6,8,10,13,10,6,13,11,7,6,5,10,13,8,4,10,6,13,11,13,6,7~6,7,3,1,12,13,4,10,5,9,8,2,11,1,8,7,4,12,4,2,12,4,5,2,13,4,8,2,12,7,8,9,12,11,4,12,4,8,12,9,8,9,11,12,3,1,12,13,1,11~13,7,2,9,12,11,6,3,4,10,8,5,3,12,11,3,4,7,4,3,11~4,1,6,2,5,7,8,9,11,13,12,3,10,8,2~11,13,5,7,10,3,12,4,9,8,1,6,12,9,10,9,10,9,13,3,9,4,13,10,9,10,13,10,3,13,8,13,9,3,9,13&purInit=[{type:\"fsbl\",bet:2000,bet_level:0},{type:\"fsbl\",bet:4000,bet_level:0},{type:\"fsbl\",bet:6000,bet_level:0}]&reel_set3=5,3,3,3,7,4,4,4,12,6,5,5,5,13,10,4,11,9,3,8,3,10,3,8,13,3,8,11,3,12,3,8,7,8,13,3,8,3,11,4,11,4,10,8,7,4,3,7,11,8,3,10,8,11,4,10,4,8,4,11,8~13,2,12,8,10,6,3,7,11,9,5,4,8,9,2,5,9,5,8,12,5,2,6,9,5,8,2,5,7,5,12,8,7,10,8,9,5,2,5,10~9,5,4,6,10,3,12,13,2,8,11,7,5,7,2,4,7,2~7,8,11,4,10,5,12,2,6,3,13,9,3,5,4,8,4,5,3,9,4,12,6,5,12,4,9,3,4,9,3,5,9,3,5~12,11,3,3,3,9,4,8,3,5,5,5,5,6,13,4,4,4,7,10,4,3,4,5,4,3,4,6,13,3,5,3,5,3,4,8,5,6,4,3,5,4,13,4,5,4,3,13,3,5,4,3,4,6,4,5,3,5,4,9,5,8,3,5,6,5,7,5,7,4,3,7,5,8,4,10,8,5,3,7,13,5,6,4,5,8,5,7,5,3,5,3,8,7,5,3,11,3,4,13,11,4,3,5,7,5,8,4,13&reel_set6=5,7,4,6,3,13,8,10,1,12,9,11,3,4,13,9,8,13,4,3,8,6,12,6,9,12,4,7~4,1,13,11,5,6,7,9,8,12,10,3,5,3,9,3,6,7,9,3,5,7~1,10,6,7,8,12,3,11,9,5,4,13,6,11,3,4,11,5,3,9,3,4,12,3,8,6,3,4,5,3,4,3,5,6,8,5,4,5,6,5,6,11~3,13,9,11,12,6,5,4,7,10,1,8,7,9,13,8,4,8,10,8,9,8,13,8,4,9,6,7,4,9,13,7,5,13,1,6,8,5,6,9~10,11,9,6,1,13,5,4,3,12,7,8,13,6,1,5,1,7,1,6,3,1,6,9,1,13,1,3,7,13,9,13,6,1,13,8,12,3,5,11&reel_set5=3,3,3,6,3,12,7,11,5,5,5,10,4,4,4,4,13,9,5,8,9,12,5,13,5,12,4,5,12,4,5,4,5,7,4,5,13,5,11,8,4,13,5,13,5,7,5,12,5,4,12,13,4,5,4,5,4,5,7,13,8,12,11,5,7,11,7,13,4,9,5,11,5,4,12~13,11,12,5,3,2,10,7,8,9,6,4,3,11,5,10,11,5,12,5,12,6,12,2,6,11,12,2,6,5,12,5,6,10,12,5,4,5,2,5,3,6,2,5,2,11,4,5,2,5,3,2,5,12,5,6,9,5,7~2,11,7,6,13,8,9,3,10,5,12,4,5,6,11,8,4,3,8,5,3,10,13,6,3,5,12,3,13,6~11,6,4,3,12,13,2,9,8,7,10,5,8,7,5,2,10,2,6,2,9,5,9,4,6,3,9,2,9,10,8,7,5,4,9,2,9,6,9,10,6,9,2,3,5,2,6,9,6,2,9,2,4,9,6,4,9,6,9~4,4,4,8,5,5,5,13,3,3,3,12,7,3,4,10,11,9,6,5,10,9,6,3,5,9,7,3,7,10,9,3,5,7,3,10,6,10,3,9,8,5,3,5,3,9,6,5,6,7,3,8,5,3,7,6,9,7,3,6,11,5,9,7,9,6,7,3,5,9,5,3,9,5,8,10,9,3,6,5,9,5,10,3,5,9,3,7,10,3&reel_set8=7,4,11,6,10,9,5,8,13,12,3,1,13,8,3,6,11,5,1,5,12,13,3,13,5,9,10,13,3,13,10,13,9,5,9,3,9,11,8,9,5,9,11,4,11,5,13,9,5,9,4,11,13,11,9,8,13,12,4,11~13,2,12,7,9,8,10,6,11,4,5,1,3,4,12,7,4,8,4,7,2,9,2,9,4,10,3,7,9,10,8,2,12,2,7,9,5,4,11,2,7,4,9,7,5,12,2,9,2,4,12,2,4,12,9,4,2,5,2,4,5,2,1,4,9,12,1,7,4,7~6,12,3,1,9,7,8,5,10,4,2,11,13,11,4,12,2,3,11,5,1,12,2,4,13,5,3,2,11,2,4,11,12,9,1,2,4,11,4,5,13,2,7,11,4,3,1,2,9,7,4,7,11,2,3,12,7,11,3,9,2,7,2,1,12,2~4,10,6,8,1,7,3,9,5,12,2,11,13,10,8,12,2,6,10,12,8,6,13,10,12,13,12,2,8,12,3,12,10,2,3,12,2,9,10,13,5,10,13,2,10,12,10,12,9,12,13,6,10,5,9,13,5,13,12,2,10,2,12,2,3,8,2,10,2,5,13,8,13,5,2,10,2,10,12~12,7,4,1,9,10,8,6,5,3,13,11,1,5,13,5,4,7,8,13,10,11,5,13,5,11,10,9,10,6&reel_set7=5,5,5,10,5,11,3,3,3,6,4,4,4,8,9,13,4,12,3,7,9,3,4,3,10,3,7,3,9,13,9,3,9,13,9,7,9,3,4,8,4,3,4,9,4,3,6,9,13,3,9,10,4,3,10,3,9,4,7,10,3,4,9,10,4,12,6,9,4,10,3,11,7,4~3,9,13,11,5,4,8,6,7,10,2,12,7,8,12,5,11,10,11,8,7,8,13,11,13,11,5,8,5,9,8,10,8,5~5,4,10,6,12,7,8,2,13,11,9,7,9,7,13,9,7,13,8,13,8,7,10,13,7,13,7,13,9,13,8,11,7,13,7,2,13,10,7,10~7,12,6,11,4,2,8,5,3,10,9,13,9,6,5,4,10,6,4,6,9,8,9,4,6,4,9,11,4,6,10,6,4,10,4,6,10,11,9,10,3,11,10,4,6,5,9,6,11,9,2,9,4,11,6,9,10,6,5,11~3,3,3,3,6,5,5,5,11,4,4,4,10,7,8,12,5,9,13,4,5,7,5,4,5,8,5,6,12,4,5,12,9,5,4,5,12,9,5,6,5,4&reel_set9=4,4,4,11,6,12,3,3,3,3,5,5,5,5,8,13,9,10,7,4,12,9,8,5,9,8,5,7,12,8,7,9,7,9,6,5,12,6,9,8,10,9,7,5,3,6,9,13,9,6,5,9,3,5,7,3,12,7,3,5,9,6,5,13,9,12,6,9,8,3,5,9,5,13,11,8,9,3,6,7,9,7,5,6,3,9,5,6,3,12,7,9,6,9,5,8,13,7,5,12,9~7,3,4,10,11,13,2,5,8,12,9,6,5,8,10,8,2,9,8,10,5,10,8,3,2,13,3,2,3,9,2,8,9,5,2,5,8,9,12,2,9,2,3,13,9,8,12,11,3,12,13,8,9,5,3,12,3,9,5,9,8,2,10,11,5,11,5~7,13,11,12,8,6,5,9,3,10,2,4,2,6,4,5,2,8,6,9,6,4,3,12,6,10,6,11,8,5,4,11,9,10,6,10,9,12,2,6,9,6,10,12,13,6,3,13,8,6,10,2,6,5,6,4,6,13,12,2~13,10,3,11,6,4,12,8,7,2,9,5,7,2,7,2,7~13,3,3,3,11,10,5,5,5,3,4,4,4,6,7,9,4,5,12,8,10,11,5,12,5,10,12,3,11,5,3,4,6,11,5&total_bet_min=10.00";
            }
        }
        #endregion

        public HandOfMidasGameLogic()
        {
            _gameID = GAMEID.HandOfMidas;
            GameName = "HandOfMidas";
        }

        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                HandOfMidasBetInfo betInfo = new HandOfMidasBetInfo();
                betInfo.BetPerLine         = (float)message.Pop();
                betInfo.LineCount          = (int)message.Pop();

                int bl = (int)message.Pop();
                if (bl == 0)
                    betInfo.MoreBet = false;
                else
                    betInfo.MoreBet = true;

                if (message.DataNum >= 3)
                {
                    betInfo.PurchaseFree = true;
                    betInfo.PurchaseType = (int) message.GetData(2);
                }
                else
                {
                    betInfo.PurchaseFree = false;
                }

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in AztecKingMegaGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in AztecKingMegaGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                    oldBetInfo.MoreBet = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                    (oldBetInfo as HandOfMidasBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in AztecKingMegaGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as HandOfMidasBetInfo).PurchaseType.ToString();
        }
        protected override bool addSpinResultToHistory(string strGlobalUserID, int index, int counter, string strSpinResult, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            if (!_dicUserHistory.ContainsKey(strGlobalUserID))
                return false;

            BasePPHistoryItem historyItem = new BasePPHistoryItem();
            historyItem.cr = string.Format("symbol={0}&c={1}&repeat=0&action=doSpin&index={2}&counter={3}&l={4}", SymbolName, betInfo.BetPerLine, index, counter, ClientReqLineCount);
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                historyItem.cr += ("&pur=" + (betInfo as HandOfMidasBetInfo).PurchaseType.ToString());
            if (SupportMoreBet)
            {
                if (betInfo.MoreBet)
                    historyItem.cr += "&bl=1";
                else
                    historyItem.cr += "&bl=0";
            }
            historyItem.sr = strSpinResult;

            _dicUserHistory[strGlobalUserID].log.Add(historyItem);
            if (betInfo.HasRemainResponse)
                return false;

            _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
            _dicUserHistory[strGlobalUserID].win = spinResult.TotalWin;

            //빈스핀인 경우이다.
            if (spinResult.NextAction == ActionTypes.DOSPIN)
                return true;

            return false;
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            return 100.0 * ((betInfo as HandOfMidasBetInfo).PurchaseType + 1);
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            HandOfMidasBetInfo betInfo = new HandOfMidasBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new HandOfMidasBetInfo();
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet = (double)infoDocument["defaultbet"];
                _normalMaxID        = (int)infoDocument["normalmaxid"];
                _emptySpinCount     = (int)infoDocument["emptycount"];
                _naturalSpinCount   = (int)infoDocument["normalselectcount"];
                var purchaseOdds = infoDocument["purchaseodds"] as BsonArray;
                for(int i = 0; i < 3; i++)
                {
                    _totalFreeSpinWinRates[i] = (double)purchaseOdds[2 * i + 1];
                    _minFreeSpinWinRates[i]   = (double)purchaseOdds[2 * i];

                    if (PurchaseFreeMultiple * (i + 1) > _totalFreeSpinWinRates[i])
                        _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
                }

                _anteBetMinusZeroCount = (int)((1.0 - 1.0 / MoreBetMultiple) * (double)_naturalSpinCount);
                if (_anteBetMinusZeroCount > _emptySpinCount)
                    _logger.Error("More Bet Probabily calculation doesn't work in {0}", this.GameName);

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }

        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int websiteID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus, bool isAffiliate)
        {
            int     purchaseType = (betInfo as HandOfMidasBetInfo).PurchaseType;            
            double  payoutRate   = getPayoutRate(websiteID, isAffiliate);
            double targetC = PurchaseFreeMultiple * (purchaseType + 1) * payoutRate / 100.0;
            if (targetC >= _totalFreeSpinWinRates[purchaseType])
                targetC = _totalFreeSpinWinRates[purchaseType];

            if (targetC < _minFreeSpinWinRates[purchaseType])
                targetC = _minFreeSpinWinRates[purchaseType];

            double x = (_totalFreeSpinWinRates[purchaseType] - targetC) / (_totalFreeSpinWinRates[purchaseType] - _minFreeSpinWinRates[purchaseType]);
            double y = 1.0 - x;

            BasePPSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinData = await selectMinStartFreeSpinData(betInfo);
            else
                spinData = await selectRandomStartFreeSpinData(betInfo);
            return spinData;
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as HandOfMidasBetInfo).PurchaseType;
            try
            {
                BsonDocument        document = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectPurchaseSpinRequest(this.GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType), TimeSpan.FromSeconds(10.0));
                BasePPSlotSpinData  spinData = convertBsonToSpinData(document);
                return spinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HandOfMidasGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as HandOfMidasBetInfo).PurchaseType;
            try
            {
                double purMultiple = getPurchaseMultiple(betInfo);
                BsonDocument        document = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinTypeOddRangeRequest(this.GameName, 1, purMultiple * 0.2, purMultiple * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                BasePPSlotSpinData  spinData = convertBsonToSpinData(document);
                return spinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HandOfMidasGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            HandOfMidasBetInfo handBetInfo = betInfo as HandOfMidasBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, betInfo.MoreBet ? 1 : 0, betInfo.PurchaseFree ? handBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
