using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics.AristoGames
{
    internal class BigRedGameLogic : BasePPSlotGame
    {
        protected override string SymbolName
        {
            get
            {
                return "big_red";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 5; }
        }
        protected override int ServerResLineCount
        {
            get { return 5; }
        }
        protected override string InitDataString
        {
            get
            {
                return "{\"status\":\"success\",\"microtime\":0.001126,\"dateTime\":\"2025-07-15 06:47:26\",\"error\":\"\",\"content\":{\"cmd\":\"gameInit\",\"balance\":9999989,\"session\":\"18778639_b98a8d2fa80044e44557ed0fda85f528\",\"betInfo\":{\"balanceInCredit\":false,\"denomination\":0.01,\"bet\":1,\"lines\":5},\"betSettings\":{\"balanceInCredit\":false,\"denomination\":[0.01],\"bets\":[1,2,3,4,5,10,20,30,40,50,100,200,300,400,500,600,700,800,900,1000,2000,3000,4000,5000,10000,20000,30000,40000,50000],\"lines\":[5]},\"symbols\":{\"1\":{\"1\":\"symbol_5\",\"2\":\"symbol_3\",\"3\":\"symbol_7\"},\"2\":{\"1\":\"symbol_8\",\"2\":\"symbol_2\",\"3\":\"symbol_5\"},\"3\":{\"1\":\"symbol_3\",\"2\":\"symbol_7\",\"3\":\"symbol_4\"},\"4\":{\"1\":\"symbol_4\",\"2\":\"symbol_6\",\"3\":\"symbol_2\"},\"5\":{\"1\":\"symbol_5\",\"2\":\"symbol_2\",\"3\":\"symbol_7\"}},\"reels\":{\"base\":{\"1\":[\"symbol_3\",\"symbol_9\",\"symbol_5\",\"symbol_10\",\"symbol_6\",\"symbol_9\",\"symbol_3\",\"symbol_7\",\"symbol_4\",\"symbol_5\",\"symbol_3\",\"symbol_9\",\"symbol_8\",\"symbol_4\",\"symbol_5\",\"symbol_7\",\"symbol_3\",\"symbol_8\",\"symbol_9\",\"symbol_1\",\"symbol_6\",\"symbol_3\",\"symbol_9\",\"symbol_2\",\"symbol_5\",\"symbol_3\",\"symbol_8\",\"symbol_4\",\"symbol_9\",\"symbol_5\",\"symbol_3\",\"symbol_7\",\"symbol_6\"],\"2\":[\"symbol_3\",\"symbol_7\",\"symbol_4\",\"symbol_9\",\"symbol_6\",\"symbol_9\",\"symbol_7\",\"symbol_1\",\"symbol_8\",\"symbol_2\",\"symbol_9\",\"symbol_3\",\"symbol_8\",\"symbol_4\",\"symbol_7\",\"symbol_3\",\"symbol_8\",\"symbol_2\",\"symbol_5\",\"symbol_3\",\"symbol_9\",\"symbol_8\",\"symbol_10\",\"symbol_5\",\"symbol_9\",\"symbol_3\",\"symbol_8\",\"symbol_1\",\"symbol_7\",\"symbol_4\",\"symbol_8\",\"symbol_3\",\"symbol_9\",\"symbol_1\",\"symbol_6\",\"symbol_3\",\"symbol_6\",\"symbol_4\",\"symbol_8\",\"symbol_2\",\"symbol_6\",\"symbol_4\",\"symbol_5\",\"symbol_3\",\"symbol_6\",\"symbol_9\",\"symbol_10\",\"symbol_7\",\"symbol_8\"],\"3\":[\"symbol_10\",\"symbol_6\",\"symbol_2\",\"symbol_9\",\"symbol_5\",\"symbol_11\",\"symbol_8\",\"symbol_3\",\"symbol_5\",\"symbol_4\",\"symbol_7\",\"symbol_3\",\"symbol_6\",\"symbol_1\",\"symbol_8\",\"symbol_3\",\"symbol_5\",\"symbol_4\",\"symbol_6\",\"symbol_10\",\"symbol_9\",\"symbol_2\",\"symbol_5\",\"symbol_4\",\"symbol_6\",\"symbol_1\",\"symbol_8\",\"symbol_3\",\"symbol_6\",\"symbol_1\",\"symbol_5\",\"symbol_10\",\"symbol_9\",\"symbol_3\",\"symbol_6\",\"symbol_4\",\"symbol_9\",\"symbol_3\",\"symbol_7\",\"symbol_4\",\"symbol_9\",\"symbol_3\",\"symbol_7\",\"symbol_4\"],\"4\":[\"symbol_3\",\"symbol_10\",\"symbol_8\",\"symbol_4\",\"symbol_6\",\"symbol_11\",\"symbol_7\",\"symbol_1\",\"symbol_9\",\"symbol_4\",\"symbol_7\",\"symbol_9\",\"symbol_3\",\"symbol_5\",\"symbol_8\",\"symbol_4\",\"symbol_6\",\"symbol_2\",\"symbol_8\",\"symbol_10\",\"symbol_7\",\"symbol_6\",\"symbol_3\",\"symbol_8\",\"symbol_4\",\"symbol_5\",\"symbol_1\",\"symbol_6\",\"symbol_4\",\"symbol_8\",\"symbol_2\",\"symbol_9\",\"symbol_3\",\"symbol_7\",\"symbol_10\",\"symbol_6\",\"symbol_5\",\"symbol_4\",\"symbol_9\",\"symbol_5\"],\"5\":[\"symbol_4\",\"symbol_8\",\"symbol_1\",\"symbol_11\",\"symbol_5\",\"symbol_10\",\"symbol_7\",\"symbol_8\",\"symbol_2\",\"symbol_5\",\"symbol_4\",\"symbol_7\",\"symbol_2\",\"symbol_6\",\"symbol_4\",\"symbol_8\",\"symbol_3\",\"symbol_7\",\"symbol_4\",\"symbol_6\",\"symbol_1\",\"symbol_9\",\"symbol_2\",\"symbol_7\",\"symbol_4\",\"symbol_9\",\"symbol_1\",\"symbol_8\",\"symbol_5\",\"symbol_2\",\"symbol_7\",\"symbol_6\"]}},\"exitUrl\":\"https://aristocrat.goldslotpalace.com/\",\"pingInterval\":60000,\"restore\":false,\"hash\":\"74b626c87f0ba58323d333b5b175ccad\"}}";
            }
        }
        public BigRedGameLogic()
        {
            _gameID = GAMEID.BigRed;
            GameName = "BigRed";
        }
    }
}
