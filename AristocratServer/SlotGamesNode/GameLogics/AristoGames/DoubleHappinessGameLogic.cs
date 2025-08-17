using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics.AristoGames
{
    internal class DoubleHappinessGameLogic : BasePPSlotGame
    {
        protected override string SymbolName
        {
            get
            {
                return "double_happiness";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 30; }
        }
        protected override int ServerResLineCount
        {
            get { return 30; }
        }
        protected override string InitDataString
        {
            get
            {
                return "{\"status\":\"success\",\"microtime\":0.001776,\"dateTime\":\"2025-07-15 06:19:01\",\"error\":\"\",\"content\":{\"cmd\":\"gameInit\",\"balance\":10000000,\"session\":\"18777827_dd5c5e4c1c33e22abf7fe11ff3800604\",\"betInfo\":{\"balanceInCredit\":false,\"denomination\":0.01,\"bet\":1,\"lines\":25},\"betSettings\":{\"balanceInCredit\":false,\"denomination\":[0.01],\"bets\":[1,2,3,4,5,10,20,30,40,50,100,200,300,400,500,600,700,800,900,1000,2000,3000,4000,5000,10000],\"lines\":[25]},\"symbols\":null,\"reels\":{\"base\":{\"1\":[\"symbol_6\",\"symbol_8\",\"symbol_1\",\"symbol_12\",\"symbol_3\",\"symbol_7\",\"symbol_11\",\"symbol_6\",\"symbol_8\",\"symbol_12\",\"symbol_4\",\"symbol_7\",\"symbol_6\",\"symbol_9\",\"symbol_5\",\"symbol_7\",\"symbol_6\",\"symbol_11\",\"symbol_9\",\"symbol_13\",\"symbol_8\",\"symbol_10\",\"symbol_1\",\"symbol_12\",\"symbol_6\",\"symbol_10\",\"symbol_4\",\"symbol_9\",\"symbol_11\",\"symbol_5\",\"symbol_12\",\"symbol_10\",\"symbol_2\",\"symbol_9\",\"symbol_3\",\"symbol_7\",\"symbol_10\",\"symbol_2\",\"symbol_11\"],\"2\":[\"symbol_8\",\"symbol_3\",\"symbol_10\",\"symbol_9\",\"symbol_6\",\"symbol_12\",\"symbol_5\",\"symbol_11\",\"symbol_7\",\"symbol_9\",\"symbol_10\",\"symbol_6\",\"symbol_12\",\"symbol_5\",\"symbol_1\",\"symbol_10\",\"symbol_3\",\"symbol_11\",\"symbol_7\",\"symbol_12\",\"symbol_8\",\"symbol_10\",\"symbol_5\",\"symbol_8\",\"symbol_4\",\"symbol_10\",\"symbol_13\",\"symbol_9\",\"symbol_11\",\"symbol_4\",\"symbol_5\",\"symbol_12\",\"symbol_6\",\"symbol_11\",\"symbol_7\",\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_1\",\"symbol_7\",\"symbol_6\"],\"3\":[\"symbol_11\",\"symbol_12\",\"symbol_5\",\"symbol_10\",\"symbol_7\",\"symbol_1\",\"symbol_12\",\"symbol_9\",\"symbol_3\",\"symbol_8\",\"symbol_4\",\"symbol_11\",\"symbol_8\",\"symbol_6\",\"symbol_10\",\"symbol_9\",\"symbol_8\",\"symbol_2\",\"symbol_12\",\"symbol_7\",\"symbol_9\",\"symbol_2\",\"symbol_6\",\"symbol_11\",\"symbol_10\",\"symbol_13\",\"symbol_11\",\"symbol_9\",\"symbol_12\",\"symbol_1\",\"symbol_11\",\"symbol_9\",\"symbol_6\",\"symbol_11\",\"symbol_10\",\"symbol_3\",\"symbol_9\"],\"4\":[\"symbol_10\",\"symbol_1\",\"symbol_7\",\"symbol_11\",\"symbol_4\",\"symbol_2\",\"symbol_7\",\"symbol_9\",\"symbol_12\",\"symbol_8\",\"symbol_6\",\"symbol_12\",\"symbol_1\",\"symbol_8\",\"symbol_7\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_3\",\"symbol_11\",\"symbol_10\",\"symbol_6\",\"symbol_12\",\"symbol_8\",\"symbol_10\",\"symbol_5\",\"symbol_7\",\"symbol_9\",\"symbol_3\",\"symbol_11\",\"symbol_13\",\"symbol_10\",\"symbol_6\",\"symbol_12\",\"symbol_5\",\"symbol_11\",\"symbol_9\",\"symbol_4\"],\"5\":[\"symbol_13\",\"symbol_9\",\"symbol_5\",\"symbol_10\",\"symbol_12\",\"symbol_1\",\"symbol_2\",\"symbol_9\",\"symbol_6\",\"symbol_11\",\"symbol_3\",\"symbol_10\",\"symbol_4\",\"symbol_8\",\"symbol_6\",\"symbol_9\",\"symbol_3\",\"symbol_8\",\"symbol_1\",\"symbol_10\",\"symbol_3\",\"symbol_7\",\"symbol_2\",\"symbol_12\",\"symbol_5\",\"symbol_3\",\"symbol_7\",\"symbol_5\",\"symbol_12\",\"symbol_10\",\"symbol_2\",\"symbol_11\",\"symbol_4\",\"symbol_12\",\"symbol_11\",\"symbol_5\",\"symbol_9\",\"symbol_4\",\"symbol_2\",\"symbol_11\",\"symbol_4\",\"symbol_3\",\"symbol_8\"]},\"feature\":{\"1\":[\"symbol_1\",\"symbol_6\",\"symbol_8\",\"symbol_3\",\"symbol_11\",\"symbol_6\",\"symbol_7\",\"symbol_4\",\"symbol_12\",\"symbol_9\",\"symbol_1\",\"symbol_7\",\"symbol_6\",\"symbol_9\",\"symbol_5\",\"symbol_7\",\"symbol_12\",\"symbol_9\",\"symbol_13\",\"symbol_5\",\"symbol_10\",\"symbol_8\",\"symbol_1\",\"symbol_12\",\"symbol_6\",\"symbol_10\",\"symbol_4\",\"symbol_8\",\"symbol_9\",\"symbol_11\",\"symbol_10\",\"symbol_8\",\"symbol_12\",\"symbol_2\",\"symbol_9\",\"symbol_11\",\"symbol_3\",\"symbol_10\",\"symbol_2\",\"symbol_7\",\"symbol_11\"],\"2\":[\"symbol_8\",\"symbol_4\",\"symbol_1\",\"symbol_3\",\"symbol_9\",\"symbol_10\",\"symbol_12\",\"symbol_7\",\"symbol_10\",\"symbol_2\",\"symbol_12\",\"symbol_8\",\"symbol_5\",\"symbol_10\",\"symbol_3\",\"symbol_12\",\"symbol_7\",\"symbol_10\",\"symbol_5\",\"symbol_9\",\"symbol_6\",\"symbol_11\",\"symbol_8\",\"symbol_1\",\"symbol_12\",\"symbol_10\",\"symbol_13\",\"symbol_7\",\"symbol_9\",\"symbol_11\",\"symbol_6\",\"symbol_1\",\"symbol_11\",\"symbol_4\",\"symbol_8\",\"symbol_5\",\"symbol_9\",\"symbol_12\",\"symbol_10\",\"symbol_7\",\"symbol_11\",\"symbol_2\",\"symbol_10\",\"symbol_1\",\"symbol_12\",\"symbol_6\"],\"3\":[\"symbol_11\",\"symbol_12\",\"symbol_8\",\"symbol_1\",\"symbol_5\",\"symbol_10\",\"symbol_7\",\"symbol_9\",\"symbol_3\",\"symbol_12\",\"symbol_4\",\"symbol_8\",\"symbol_11\",\"symbol_6\",\"symbol_9\",\"symbol_12\",\"symbol_2\",\"symbol_10\",\"symbol_8\",\"symbol_7\",\"symbol_9\",\"symbol_4\",\"symbol_11\",\"symbol_5\",\"symbol_10\",\"symbol_7\",\"symbol_3\",\"symbol_6\",\"symbol_13\",\"symbol_11\",\"symbol_9\",\"symbol_12\",\"symbol_1\",\"symbol_11\",\"symbol_10\",\"symbol_9\",\"symbol_6\",\"symbol_11\",\"symbol_8\",\"symbol_9\"],\"4\":[\"symbol_10\",\"symbol_7\",\"symbol_11\",\"symbol_1\",\"symbol_2\",\"symbol_9\",\"symbol_6\",\"symbol_12\",\"symbol_10\",\"symbol_8\",\"symbol_9\",\"symbol_12\",\"symbol_1\",\"symbol_8\",\"symbol_7\",\"symbol_11\",\"symbol_8\",\"symbol_3\",\"symbol_10\",\"symbol_12\",\"symbol_11\",\"symbol_8\",\"symbol_9\",\"symbol_5\",\"symbol_7\",\"symbol_11\",\"symbol_8\",\"symbol_10\",\"symbol_13\",\"symbol_11\",\"symbol_6\",\"symbol_12\",\"symbol_5\",\"symbol_9\",\"symbol_11\",\"symbol_4\"],\"5\":[\"symbol_13\",\"symbol_9\",\"symbol_11\",\"symbol_10\",\"symbol_1\",\"symbol_9\",\"symbol_12\",\"symbol_11\",\"symbol_10\",\"symbol_9\",\"symbol_8\",\"symbol_12\",\"symbol_9\",\"symbol_10\",\"symbol_1\",\"symbol_10\",\"symbol_9\",\"symbol_12\",\"symbol_8\",\"symbol_7\",\"symbol_11\",\"symbol_12\",\"symbol_9\",\"symbol_11\",\"symbol_4\",\"symbol_12\",\"symbol_11\",\"symbol_5\",\"symbol_9\",\"symbol_10\",\"symbol_2\",\"symbol_11\",\"symbol_6\",\"symbol_12\",\"symbol_7\",\"symbol_3\",\"symbol_8\"]}},\"exitUrl\":\"https://aristocrat.goldslotpalace.com/\",\"pingInterval\":60000,\"restore\":false,\"hash\":\"25bfb66e9d66d1df54c393b83ce5dc9e\"}}";
            }
        }
        public DoubleHappinessGameLogic()
        {
            _gameID = GAMEID.DoubleHappiness;
            GameName = "DoubleHappiness";
        }
    }
}
