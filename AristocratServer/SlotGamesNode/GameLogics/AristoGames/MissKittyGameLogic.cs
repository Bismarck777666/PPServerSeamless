using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics.AristoGames
{
    internal class MissKittyGameLogic : BasePPSlotGame
    {
        protected override string SymbolName
        {
            get
            {
                return "miss_kitty";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 50; }
        }
        protected override int ServerResLineCount
        {
            get { return 50; }
        }
        protected override string InitDataString
        {
            get
            {
                return "{\"status\":\"success\",\"microtime\":0.001451,\"dateTime\":\"2025-07-16 13:03:00\",\"error\":\"\",\"content\":{\"cmd\":\"gameInit\",\"balance\":9999190,\"session\":\"18877101_3d083951094c5c8625baa12d79415025\",\"betInfo\":{\"balanceInCredit\":false,\"denomination\":0.01,\"bet\":1,\"lines\":50},\"betSettings\":{\"balanceInCredit\":false,\"denomination\":[0.01],\"bets\":[1,2,3,4,5,10,20,30,40,50,100,200,300,400,500,600,700,800,900,1000,2000,3000,4000,5000],\"lines\":[50]},\"symbols\":{\"1\":{\"1\":\"symbol_10\",\"2\":\"symbol_8\",\"3\":\"symbol_3\",\"4\":\"symbol_6\"},\"2\":{\"1\":\"symbol_3\",\"2\":\"symbol_13\",\"3\":\"symbol_6\",\"4\":\"symbol_10\"},\"3\":{\"1\":\"symbol_3\",\"2\":\"symbol_13\",\"3\":\"symbol_4\",\"4\":\"symbol_6\"},\"4\":{\"1\":\"symbol_1\",\"2\":\"symbol_1\",\"3\":\"symbol_8\",\"4\":\"symbol_9\"},\"5\":{\"1\":\"symbol_8\",\"2\":\"symbol_7\",\"3\":\"symbol_4\",\"4\":\"symbol_9\"}},\"reels\":{\"base\":{\"1\":[\"symbol_10\",\"symbol_7\",\"symbol_4\",\"symbol_8\",\"symbol_12\",\"symbol_2\",\"symbol_5\",\"symbol_11\",\"symbol_3\",\"symbol_9\",\"symbol_12\",\"symbol_4\",\"symbol_7\",\"symbol_3\",\"symbol_6\",\"symbol_2\",\"symbol_11\",\"symbol_7\",\"symbol_10\",\"symbol_8\",\"symbol_3\",\"symbol_6\",\"symbol_4\",\"symbol_8\",\"symbol_10\",\"symbol_7\",\"symbol_10\",\"symbol_9\",\"symbol_5\",\"symbol_8\",\"symbol_12\",\"symbol_6\",\"symbol_5\",\"symbol_10\",\"symbol_2\",\"symbol_11\",\"symbol_12\",\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\"],\"2\":[\"symbol_4\",\"symbol_13\",\"symbol_2\",\"symbol_9\",\"symbol_4\",\"symbol_8\",\"symbol_11\",\"symbol_4\",\"symbol_7\",\"symbol_12\",\"symbol_6\",\"symbol_3\",\"symbol_5\",\"symbol_9\",\"symbol_13\",\"symbol_7\",\"symbol_6\",\"symbol_10\",\"symbol_12\",\"symbol_2\",\"symbol_11\",\"symbol_10\",\"symbol_13\",\"symbol_7\",\"symbol_8\",\"symbol_9\",\"symbol_3\",\"symbol_8\",\"symbol_7\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_10\",\"symbol_3\",\"symbol_13\",\"symbol_6\",\"symbol_10\",\"symbol_12\",\"symbol_7\",\"symbol_3\",\"symbol_11\",\"symbol_5\",\"symbol_8\",\"symbol_2\",\"symbol_11\",\"symbol_5\",\"symbol_8\",\"symbol_3\",\"symbol_13\",\"symbol_5\",\"symbol_11\",\"symbol_4\",\"symbol_6\",\"symbol_9\",\"symbol_11\"],\"3\":[\"symbol_2\",\"symbol_11\",\"symbol_4\",\"symbol_13\",\"symbol_5\",\"symbol_7\",\"symbol_4\",\"symbol_6\",\"symbol_5\",\"symbol_10\",\"symbol_8\",\"symbol_11\",\"symbol_7\",\"symbol_5\",\"symbol_9\",\"symbol_10\",\"symbol_13\",\"symbol_7\",\"symbol_3\",\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_9\",\"symbol_11\",\"symbol_2\",\"symbol_7\",\"symbol_3\",\"symbol_11\",\"symbol_12\",\"symbol_6\",\"symbol_3\",\"symbol_4\",\"symbol_7\",\"symbol_3\",\"symbol_9\",\"symbol_6\",\"symbol_8\",\"symbol_13\",\"symbol_10\",\"symbol_9\",\"symbol_6\",\"symbol_3\",\"symbol_13\",\"symbol_4\",\"symbol_6\",\"symbol_2\"],\"4\":[\"symbol_10\",\"symbol_2\",\"symbol_3\",\"symbol_6\",\"symbol_9\",\"symbol_13\",\"symbol_10\",\"symbol_11\",\"symbol_7\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_8\",\"symbol_9\",\"symbol_6\",\"symbol_5\",\"symbol_4\",\"symbol_2\",\"symbol_8\",\"symbol_11\",\"symbol_5\",\"symbol_10\",\"symbol_4\",\"symbol_8\",\"symbol_6\",\"symbol_3\",\"symbol_7\",\"symbol_4\",\"symbol_13\",\"symbol_5\",\"symbol_3\",\"symbol_11\",\"symbol_3\",\"symbol_9\",\"symbol_2\"],\"5\":[\"symbol_3\",\"symbol_11\",\"symbol_10\",\"symbol_6\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_10\",\"symbol_7\",\"symbol_6\",\"symbol_8\",\"symbol_7\",\"symbol_4\",\"symbol_9\",\"symbol_7\",\"symbol_11\",\"symbol_5\",\"symbol_2\",\"symbol_9\",\"symbol_6\",\"symbol_13\",\"symbol_10\",\"symbol_4\",\"symbol_7\",\"symbol_3\",\"symbol_5\",\"symbol_8\",\"symbol_2\",\"symbol_9\",\"symbol_11\",\"symbol_13\",\"symbol_8\"]}},\"exitUrl\":\"https://aristocrat.goldslotpalace.com/\",\"pingInterval\":60000,\"restore\":false,\"hash\":\"5477b5cbe82d1c08af3f842414cfbd05\"}}";
            }
        }
        public MissKittyGameLogic()
        {
            _gameID = GAMEID.MissKitty;
            GameName = "MissKitty";
        }
    }
}
