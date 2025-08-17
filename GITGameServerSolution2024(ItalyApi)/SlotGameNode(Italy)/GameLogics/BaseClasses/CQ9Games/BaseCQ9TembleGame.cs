using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.IO;
using Akka.Actor;
using Newtonsoft.Json;
using Akka.Configuration;
using Newtonsoft.Json.Linq;

namespace SlotGamesNode.GameLogics
{
   
    public class BaseCQ9TembleGame : BaseCQ9SlotGame
    {
        protected override void addDefaultParams(dynamic resultContext, CQ9Actions action)
        {
            base.addDefaultParams(resultContext as JObject, action);
            if (action == CQ9Actions.TembleSpin)
            {
                resultContext["GamePlaySerialNumber"]   = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds / 10 + 455000000000);
                resultContext["EmulatorType"]           = 0;
            }
        }

        protected override async Task<double> procRemainResponse(string strUserID, int agentID, BaseCQ9SlotBetInfo betInfo, double userBalance)
        {
            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            double winMoney = _dicUserResultInfos[strGlobalUserID].TotalWin;

            do
            {
                BaseCQ9ActionToResponse actionToResponse    = betInfo.pullRemainResponse();
                BaseCQ9SlotSpinResult   result              = calculateResult(betInfo, actionToResponse.Response, false);

                if (_dicUserHistory.ContainsKey(strGlobalUserID))
                    _dicUserHistory[strGlobalUserID].Responses.Add(new CQ9ResponseHistory(result.Action, DateTime.UtcNow, result.ResultString));

                if (result.Action == CQ9Actions.FreeSpinResult)
                    winMoney    = result.TotalWin;
                else
                    winMoney    += result.TotalWin;

                //프리,텀블게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                {
                    betInfo.RemainReponses = null;
                    break;
                }
            } while (true);
            return winMoney;
        }
    }
}
