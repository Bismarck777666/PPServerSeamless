using Akka.Actor;
using Akka.Util;
using GITProtocol;
using GITProtocol.Utils;
using MongoDB.Bson;
using PCGSharp;
using SlotGamesNode.Database;
using SlotGamesNode.GameLogics.BaseClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    internal class BaseGreenLockRespinSlotGame : BaseGreenSlotGame
    {
        private int TestNumber { get; set; }

        public BaseGreenLockRespinSlotGame()
        {
            ReceiveAsync<PerformanceTestRequest>(onPerformanceTest);
            TestNumber = 0;
        }
        
        protected override void LoadSetting()
        {
            base.LoadSetting();
            initGameData();
        }
        protected override int getSpinWinMoney(string strSpinData, BaseClasses.BaseGreenSlotBetInfo betInfo, bool isFreespin = false)
        {
            string[] selections = strSpinData.Split('ÿ');
            int totalWin = 0;
            totalWin = (int)((double)int.Parse(selections[selections.Length - 1].Split(',')[1]) * ((double)betInfo.PlayBet / (double)_spinDataDefaultBet[betInfo.PlayLine]));
            return totalWin;
        }
        protected override string buildSpinResMsgString(string strGlobalUserID, double balance, double totalWin, double collectWin, BaseClasses.BaseGreenSlotBetInfo betInfo, string spinString, GreenMessageType type, Currencies currency = Currencies.USD)
        {
            List<string> sectorList = new List<string>();
            sectorList.Add(string.Format("\u0006S{0}", balance));
            sectorList.Add("A1");
            sectorList.Add($"C,100.0,{getCurrencySymbol(currency)},0,{getCurrencyCode(currency)}");
            sectorList.Add(string.Format("T,{0},{1},{2}", betInfo.BetTransactionID, betInfo.RelativeTotalBet, type == GreenMessageType.NormalSpin ? totalWin : collectWin - totalWin));
            sectorList.Add(string.Format("R{0}", betInfo.RoundID));
            sectorList.Add("M,1,10000,1,0,1000000");
            if (spinString.Contains("f,"))
            {
                if (betInfo.HasRemainResponse)
                {
                    sectorList.Add("I10");
                    sectorList.Add(string.Format("W,{0}", totalWin));
                }
                else
                    sectorList.Add("I11");
            }
            else
            {
                if (totalWin > 0)
                {
                    sectorList.Add("I10");
                    sectorList.Add(string.Format("W,{0}", totalWin));
                }
                else
                    sectorList.Add("I11");
            }

            sectorList.Add("H,0,0,0,0,0,0");
            sectorList.Add("X");
            sectorList.Add("Y,10,10");
            sectorList.Add("V25");
            sectorList.Add("bs,0,1,0");
            sectorList.Add(":k,null");
            sectorList.Add(string.Format("e,{0},{1},{2},{3}", betInfo.PlayLine, betInfo.PlayBet / betInfo.PlayLine, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : 0, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : -1));
            sectorList.Add("b,1000,1000,0");
            sectorList.Add(":x,50");

            if (spinString.Contains("f,"))
            {
                if (spinString.Contains("v,"))
                {
                    sectorList.Add("s,13");
                }
                else
                {
                    sectorList.Add("s,11");
                }
            }
            else
            {
                if (spinString.Contains("v,"))
                {
                    sectorList.Add("s,13");
                }
                else
                {
                    if (totalWin > 0)
                    {
                        sectorList.Add("s,11");
                        sectorList.Add("q,1");
                    }
                    else
                        sectorList.Add("s,1");
                }
            }

            sectorList.Add(changeSpinString(spinString, betInfo, totalWin, collectWin, false));

            return String.Join("ÿ", sectorList.ToArray());
        }
        protected override string buildCollectResMsgString(string strGlobalUserID, double balance, BaseClasses.BaseGreenSlotBetInfo betInfo, BaseClasses.BaseGreenSlotSpinResult spinResult, GreenMessageType type, Currencies currency = Currencies.USD)
        {

            List<string> sectorList = new List<string>();
            sectorList.Add(string.Format("\u0006S{0}", balance));
            sectorList.Add("A1");
            sectorList.Add($"C,100.0,{getCurrencySymbol(currency)},0,{getCurrencyCode(currency)}");

            bool isFreespin = betInfo.SpinInfo.StartsWith("f");
            sectorList.Add(string.Format("T,{0},{1},{2}", betInfo.BetTransactionID, betInfo.PlayBet, isFreespin ? spinResult.CollectWin : spinResult.TotalWin));
            
            sectorList.Add(string.Format("R{0}", betInfo.RoundID));
            sectorList.Add("M,1,10000,1,0,1000000");
            
            sectorList.Add("H,0,0,0,0,0,0");
            sectorList.Add("X");
            sectorList.Add("Y,10,10");
            sectorList.Add("V25");
            sectorList.Add("bs,0,1,0");
            sectorList.Add(":k,null");
            sectorList.Add(string.Format("e,{0},{1},{2},{3}", betInfo.PlayLine, betInfo.PlayBet / betInfo.PlayLine, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : 0, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : -1));
            sectorList.Add("b,1000,1000,0");
            sectorList.Add(":x,50");
            if (betInfo.HasRemainResponse)
            {
                sectorList.Add(changeSpinString(betInfo.SpinInfo, betInfo, spinResult.TotalWin, spinResult.CollectWin, true));
                sectorList.Add("s,5");

                if (betInfo.HasRemainResponse)
                    sectorList.Add("I10");
                else
                    sectorList.Add("I11");
            }
            else
            {
                sectorList.Add("I11");
                sectorList.Add(changeSpinString(betInfo.SpinInfo, betInfo, spinResult.TotalWin, spinResult.CollectWin, true));
                sectorList.Add("s,1");
            }

            return String.Join("ÿ", sectorList.ToArray());
        }
        protected override string buildGamblePickResMsgString(string strGlobalUserID, double balance, BaseClasses.BaseGreenSlotBetInfo betInfo, BaseClasses.BaseGreenSlotSpinResult spinResult, GreenMessageType type, Currencies currency = Currencies.USD)
        {
            List<string> sectorList = new List<string>();
            sectorList.Add(string.Format("\u0006S{0}", balance));
            sectorList.Add("A1");
            sectorList.Add($"C,100.0,{getCurrencySymbol(currency)},0,{getCurrencyCode(currency)}");
            sectorList.Add(string.Format("T,{0},{1},{2}", betInfo.BetTransactionID, betInfo.PlayBet, spinResult.TotalWin));
            sectorList.Add(string.Format("R{0}", betInfo.RoundID));
            sectorList.Add("M,5,10000,1,0,1000000");
            sectorList.Add("I10");
            sectorList.Add(string.Format("W,{0}", spinResult.TotalWin));
            sectorList.Add("H,0,0,0,0,0,0");
            sectorList.Add("X");
            sectorList.Add("Y,10,10");
            sectorList.Add("V25");
            sectorList.Add("bs,0,1,0");
            sectorList.Add(":k,null");
            sectorList.Add(string.Format("e,{0},{1},{2},{3}", betInfo.PlayLine, betInfo.PlayBet / betInfo.PlayLine, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : 0, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : -1));
            sectorList.Add("b,1000,1000,0");
            sectorList.Add(":x,50");
            sectorList.Add("s,3");
            for(int i = 1; i < 9; i++)
            {
                sectorList.Add(string.Format("q,{0},0", betInfo.GambleInitTotalWin * Math.Pow(2, i)));
            }
            if (betInfo.GambleCardHistory.Count > 0)
            {
                sectorList.Add(string.Format("h,{0}", string.Join("", betInfo.GambleCardHistory.ToArray())));
            }
            sectorList.Add(changeSpinString(betInfo.SpinInfo, betInfo, spinResult.TotalWin, spinResult.CollectWin, true));

            return String.Join("ÿ", sectorList.ToArray());
        }
        protected override string buildGambleResultResMsgString(string strGlobalUserID, double balance, BaseClasses.BaseGreenSlotBetInfo betInfo, BaseClasses.BaseGreenSlotSpinResult spinResult, GreenMessageType type, Currencies currency = Currencies.USD)
        {
            List<string> sectorList = new List<string>();
            sectorList.Add(string.Format("\u0006S{0}", balance));
            sectorList.Add("A1");
            sectorList.Add($"C,100.0,{getCurrencySymbol(currency)},0,{getCurrencyCode(currency)}");
            bool isFreespin = betInfo.SpinInfo.StartsWith("f");
            sectorList.Add(string.Format("T,{0},{1},{2}", betInfo.BetTransactionID, betInfo.PlayBet, isFreespin ? betInfo.GambleInitCollectWin : betInfo.GambleInitTotalWin));
            sectorList.Add(string.Format("R{0}", betInfo.RoundID));
            sectorList.Add("M,5,10000,1,0,1000000");
            if(spinResult.TotalWin > 0)
            {
                sectorList.Add("I10");
                sectorList.Add(string.Format("W,{0}", spinResult.TotalWin));
            }
            else
                sectorList.Add("I11");
            sectorList.Add("H,0,0,0,0,0,0");
            sectorList.Add("X");
            sectorList.Add("Y,10,10");
            sectorList.Add("V25");
            sectorList.Add("bs,0,1,0");
            sectorList.Add(":k,null");
            sectorList.Add(string.Format("e,{0},{1},{2},{3}", betInfo.PlayLine, betInfo.PlayBet / betInfo.PlayLine, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : 0, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : -1));
            sectorList.Add("b,1000,1000,0");
            //sectorList.Add(":x,50");
            
            if (spinResult.TotalWin > 0)
            {
                sectorList.Add("s,3");
                for (int i = 1; i < 9; i++)
                {
                    sectorList.Add(string.Format("q,{0},0", betInfo.GambleInitTotalWin * Math.Pow(2, i)));
                }
            }
            else
                sectorList.Add("s,1");

            if(betInfo.GambleCardHistory.Count > 0)
                sectorList.Add(string.Format("h,{0}", string.Join("", betInfo.GambleCardHistory.ToArray())));
            sectorList.Add(changeSpinStringForGamble(betInfo.SpinInfo, betInfo.PlayBet, betInfo.PlayLine, betInfo.GambleRound, betInfo.GambleInitTotalWin, betInfo.GambleInitCollectWin, spinResult.TotalWin, spinResult.CollectWin));

            return String.Join("ÿ", sectorList.ToArray());
        }
      
    }
}
