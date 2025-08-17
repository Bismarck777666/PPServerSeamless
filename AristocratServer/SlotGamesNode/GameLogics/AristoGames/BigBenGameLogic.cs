using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics.AristoGames
{
    internal class BigBenGameLogic : BasePPSlotGame
    {
        protected override string SymbolName
        {
            get
            {
                return "big_ben";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 25; }
        }
        protected override int ServerResLineCount
        {
            get { return 25; }
        }
        protected override string InitDataString
        {
            get
            {
                return "{\"status\":\"success\",\"microtime\":0.001264,\"dateTime\":\"2025-07-15 10:59:32\",\"error\":\"\",\"content\":{\"cmd\":\"gameInit\",\"balance\":10000420,\"session\":\"18793005_680ca3925492c91a9bc2a9874ba5a491\",\"betInfo\":{\"balanceInCredit\":false,\"denomination\":0.01,\"bet\":1,\"lines\":25},\"betSettings\":{\"balanceInCredit\":false,\"denomination\":[0.01],\"bets\":[1,2,3,4,5,10,20,30,40,50,100,200,300,400,500,600,700,800,900,1000,2000,3000,4000,5000,10000],\"lines\":[25]},\"symbols\":{\"1\":{\"1\":\"symbol_12\",\"2\":\"symbol_2\",\"3\":\"symbol_11\"},\"2\":{\"1\":\"symbol_4\",\"2\":\"symbol_7\",\"3\":\"symbol_12\"},\"3\":{\"1\":\"symbol_13\",\"2\":\"symbol_5\",\"3\":\"symbol_11\"},\"4\":{\"1\":\"symbol_10\",\"2\":\"symbol_3\",\"3\":\"symbol_8\"},\"5\":{\"1\":\"symbol_8\",\"2\":\"symbol_12\",\"3\":\"symbol_5\"}},\"reels\":{\"base\":{\"1\":[\"symbol_6\",\"symbol_8\",\"symbol_2\",\"symbol_1\",\"symbol_13\",\"symbol_11\",\"symbol_6\",\"symbol_4\",\"symbol_8\",\"symbol_12\",\"symbol_10\",\"symbol_7\",\"symbol_6\",\"symbol_9\",\"symbol_5\",\"symbol_7\",\"symbol_13\",\"symbol_9\",\"symbol_4\",\"symbol_3\",\"symbol_10\",\"symbol_1\",\"symbol_12\",\"symbol_2\",\"symbol_10\",\"symbol_8\",\"symbol_4\",\"symbol_12\",\"symbol_9\",\"symbol_11\",\"symbol_7\",\"symbol_5\",\"symbol_10\",\"symbol_6\",\"symbol_9\",\"symbol_5\",\"symbol_3\",\"symbol_12\",\"symbol_2\",\"symbol_11\"],\"2\":[\"symbol_8\",\"symbol_1\",\"symbol_9\",\"symbol_6\",\"symbol_10\",\"symbol_5\",\"symbol_11\",\"symbol_8\",\"symbol_4\",\"symbol_7\",\"symbol_12\",\"symbol_5\",\"symbol_10\",\"symbol_9\",\"symbol_6\",\"symbol_8\",\"symbol_4\",\"symbol_7\",\"symbol_10\",\"symbol_5\",\"symbol_12\",\"symbol_3\",\"symbol_10\",\"symbol_13\",\"symbol_9\",\"symbol_7\",\"symbol_4\",\"symbol_10\",\"symbol_5\",\"symbol_11\",\"symbol_6\",\"symbol_10\",\"symbol_3\",\"symbol_2\",\"symbol_10\",\"symbol_1\",\"symbol_12\",\"symbol_3\"],\"3\":[\"symbol_11\",\"symbol_12\",\"symbol_10\",\"symbol_5\",\"symbol_4\",\"symbol_8\",\"symbol_6\",\"symbol_9\",\"symbol_5\",\"symbol_3\",\"symbol_8\",\"symbol_9\",\"symbol_4\",\"symbol_8\",\"symbol_2\",\"symbol_10\",\"symbol_9\",\"symbol_7\",\"symbol_8\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_11\",\"symbol_2\",\"symbol_4\",\"symbol_10\",\"symbol_12\",\"symbol_7\",\"symbol_6\",\"symbol_13\",\"symbol_5\",\"symbol_11\",\"symbol_9\",\"symbol_1\",\"symbol_11\",\"symbol_9\",\"symbol_12\",\"symbol_3\",\"symbol_6\",\"symbol_11\",\"symbol_9\",\"symbol_7\"],\"4\":[\"symbol_12\",\"symbol_7\",\"symbol_8\",\"symbol_4\",\"symbol_12\",\"symbol_3\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_11\",\"symbol_6\",\"symbol_12\",\"symbol_1\",\"symbol_8\",\"symbol_7\",\"symbol_12\",\"symbol_2\",\"symbol_4\",\"symbol_12\",\"symbol_3\",\"symbol_9\",\"symbol_12\",\"symbol_5\",\"symbol_11\",\"symbol_12\",\"symbol_2\",\"symbol_11\",\"symbol_5\",\"symbol_12\",\"symbol_7\",\"symbol_8\",\"symbol_3\",\"symbol_12\",\"symbol_13\",\"symbol_8\",\"symbol_6\",\"symbol_12\",\"symbol_5\",\"symbol_10\",\"symbol_6\",\"symbol_4\",\"symbol_8\",\"symbol_1\",\"symbol_12\",\"symbol_9\",\"symbol_7\",\"symbol_10\",\"symbol_3\",\"symbol_8\",\"symbol_4\"],\"5\":[\"symbol_13\",\"symbol_9\",\"symbol_5\",\"symbol_2\",\"symbol_4\",\"symbol_7\",\"symbol_6\",\"symbol_9\",\"symbol_3\",\"symbol_10\",\"symbol_4\",\"symbol_8\",\"symbol_9\",\"symbol_1\",\"symbol_10\",\"symbol_3\",\"symbol_13\",\"symbol_7\",\"symbol_12\",\"symbol_5\",\"symbol_10\",\"symbol_4\",\"symbol_8\",\"symbol_12\",\"symbol_5\",\"symbol_6\",\"symbol_11\",\"symbol_4\",\"symbol_7\",\"symbol_11\",\"symbol_6\",\"symbol_5\",\"symbol_9\",\"symbol_4\",\"symbol_2\",\"symbol_11\",\"symbol_6\",\"symbol_4\",\"symbol_11\",\"symbol_7\",\"symbol_12\",\"symbol_2\",\"symbol_8\",\"symbol_4\",\"symbol_3\",\"symbol_11\",\"symbol_13\",\"symbol_5\",\"symbol_6\",\"symbol_11\",\"symbol_1\",\"symbol_10\",\"symbol_3\",\"symbol_8\",\"symbol_4\",\"symbol_11\",\"symbol_3\"]},\"free\":{\"1\":[\"symbol_6\",\"symbol_8\",\"symbol_2\",\"symbol_1\",\"symbol_13\",\"symbol_11\",\"symbol_6\",\"symbol_4\",\"symbol_8\",\"symbol_12\",\"symbol_10\",\"symbol_7\",\"symbol_6\",\"symbol_9\",\"symbol_5\",\"symbol_7\",\"symbol_9\",\"symbol_4\",\"symbol_3\",\"symbol_10\",\"symbol_1\",\"symbol_12\",\"symbol_2\",\"symbol_10\",\"symbol_8\",\"symbol_4\",\"symbol_12\",\"symbol_9\",\"symbol_11\",\"symbol_7\",\"symbol_5\",\"symbol_10\",\"symbol_6\",\"symbol_9\",\"symbol_5\",\"symbol_3\",\"symbol_12\",\"symbol_2\",\"symbol_11\"],\"2\":[\"symbol_8\",\"symbol_1\",\"symbol_9\",\"symbol_6\",\"symbol_10\",\"symbol_5\",\"symbol_11\",\"symbol_8\",\"symbol_4\",\"symbol_7\",\"symbol_12\",\"symbol_5\",\"symbol_10\",\"symbol_9\",\"symbol_6\",\"symbol_8\",\"symbol_4\",\"symbol_7\",\"symbol_10\",\"symbol_5\",\"symbol_12\",\"symbol_3\",\"symbol_10\",\"symbol_13\",\"symbol_9\",\"symbol_7\",\"symbol_4\",\"symbol_10\",\"symbol_5\",\"symbol_11\",\"symbol_6\",\"symbol_10\",\"symbol_3\",\"symbol_2\",\"symbol_10\",\"symbol_1\",\"symbol_12\",\"symbol_3\"],\"3\":[\"symbol_11\",\"symbol_12\",\"symbol_10\",\"symbol_5\",\"symbol_4\",\"symbol_8\",\"symbol_6\",\"symbol_9\",\"symbol_5\",\"symbol_3\",\"symbol_8\",\"symbol_9\",\"symbol_4\",\"symbol_8\",\"symbol_2\",\"symbol_10\",\"symbol_9\",\"symbol_7\",\"symbol_8\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_11\",\"symbol_2\",\"symbol_4\",\"symbol_10\",\"symbol_12\",\"symbol_7\",\"symbol_6\",\"symbol_13\",\"symbol_5\",\"symbol_11\",\"symbol_9\",\"symbol_1\",\"symbol_11\",\"symbol_9\",\"symbol_12\",\"symbol_3\",\"symbol_6\",\"symbol_11\",\"symbol_9\",\"symbol_7\"],\"4\":[\"symbol_12\",\"symbol_7\",\"symbol_8\",\"symbol_4\",\"symbol_12\",\"symbol_3\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_1\",\"symbol_11\",\"symbol_6\",\"symbol_12\",\"symbol_1\",\"symbol_8\",\"symbol_7\",\"symbol_12\",\"symbol_2\",\"symbol_4\",\"symbol_12\",\"symbol_3\",\"symbol_9\",\"symbol_12\",\"symbol_5\",\"symbol_11\",\"symbol_12\",\"symbol_2\",\"symbol_11\",\"symbol_5\",\"symbol_12\",\"symbol_7\",\"symbol_8\",\"symbol_3\",\"symbol_12\",\"symbol_13\",\"symbol_8\",\"symbol_6\",\"symbol_12\",\"symbol_5\",\"symbol_10\",\"symbol_6\",\"symbol_4\",\"symbol_8\",\"symbol_1\",\"symbol_12\",\"symbol_9\",\"symbol_7\",\"symbol_10\",\"symbol_3\",\"symbol_8\",\"symbol_4\"],\"5\":[\"symbol_13\",\"symbol_9\",\"symbol_5\",\"symbol_2\",\"symbol_4\",\"symbol_7\",\"symbol_6\",\"symbol_9\",\"symbol_3\",\"symbol_10\",\"symbol_4\",\"symbol_8\",\"symbol_9\",\"symbol_1\",\"symbol_10\",\"symbol_3\",\"symbol_7\",\"symbol_12\",\"symbol_5\",\"symbol_10\",\"symbol_4\",\"symbol_8\",\"symbol_12\",\"symbol_5\",\"symbol_6\",\"symbol_11\",\"symbol_4\",\"symbol_7\",\"symbol_11\",\"symbol_6\",\"symbol_5\",\"symbol_9\",\"symbol_4\",\"symbol_2\",\"symbol_11\",\"symbol_6\",\"symbol_4\",\"symbol_11\",\"symbol_7\",\"symbol_12\",\"symbol_2\",\"symbol_8\",\"symbol_4\",\"symbol_3\",\"symbol_11\",\"symbol_5\",\"symbol_6\",\"symbol_11\",\"symbol_1\",\"symbol_10\",\"symbol_3\",\"symbol_8\",\"symbol_4\",\"symbol_11\",\"symbol_3\"]}},\"exitUrl\":\"https://aristocrat.goldslotpalace.com/\",\"pingInterval\":60000,\"restore\":false,\"hash\":\"751eb135c1b78c7f74b8fb7c7c445b41\"}}";
            }
        }
        public BigBenGameLogic()
        {
            _gameID = GAMEID.BigBen;
            GameName = "BigBen";
        }
    }
}
