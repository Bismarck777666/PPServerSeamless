using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics.AristoGames
{
    internal class Lions50GameLogic : BasePPSlotGame
    {
        protected override string SymbolName
        {
            get
            {
                return "50_lions";
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
                return "{\"status\":\"success\",\"microtime\":0.001146,\"dateTime\":\"2025-07-14 20:54:12\",\"error\":\"\",\"content\":{\"cmd\":\"gameInit\",\"balance\":9999658,\"session\":\"18753664_a62281b84df4caa19fbf233680ce4ffa\",\"betInfo\":{\"balanceInCredit\":false,\"denomination\":0.01,\"bet\":1,\"lines\":50},\"betSettings\":{\"balanceInCredit\":false,\"denomination\":[0.01],\"bets\":[1,2,3,4,5,10,20,30,40,50,100,200,300,400,500,600,700,800,900,1000,2000,3000,4000,5000],\"lines\":[50]},\"symbols\":{\"1\":{\"1\":\"symbol_3\",\"2\":\"symbol_8\",\"3\":\"symbol_4\",\"4\":\"symbol_6\"},\"2\":{\"1\":\"symbol_5\",\"2\":\"symbol_3\",\"3\":\"symbol_8\",\"4\":\"symbol_4\"},\"3\":{\"1\":\"symbol_1\",\"2\":\"symbol_1\",\"3\":\"symbol_1\",\"4\":\"symbol_8\"},\"4\":{\"1\":\"symbol_7\",\"2\":\"symbol_11\",\"3\":\"symbol_4\",\"4\":\"symbol_10\"},\"5\":{\"1\":\"symbol_9\",\"2\":\"symbol_3\",\"3\":\"symbol_13\",\"4\":\"symbol_8\"}},\"reels\":{\"base\":{\"1\":[\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_8\",\"symbol_12\",\"symbol_10\",\"symbol_11\",\"symbol_4\",\"symbol_6\",\"symbol_3\",\"symbol_12\",\"symbol_2\",\"symbol_6\",\"symbol_8\",\"symbol_4\",\"symbol_3\",\"symbol_7\",\"symbol_2\",\"symbol_8\",\"symbol_5\",\"symbol_11\",\"symbol_9\",\"symbol_2\",\"symbol_7\",\"symbol_5\",\"symbol_10\",\"symbol_3\",\"symbol_8\",\"symbol_4\",\"symbol_6\",\"symbol_7\",\"symbol_3\",\"symbol_5\",\"symbol_11\",\"symbol_12\",\"symbol_9\",\"symbol_2\"],\"2\":[\"symbol_8\",\"symbol_3\",\"symbol_6\",\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_3\",\"symbol_11\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_4\",\"symbol_12\",\"symbol_10\",\"symbol_3\",\"symbol_11\",\"symbol_5\",\"symbol_7\",\"symbol_2\",\"symbol_6\",\"symbol_5\",\"symbol_7\",\"symbol_9\",\"symbol_3\",\"symbol_2\",\"symbol_4\",\"symbol_5\",\"symbol_3\",\"symbol_8\",\"symbol_4\",\"symbol_10\",\"symbol_2\",\"symbol_9\",\"symbol_6\",\"symbol_8\",\"symbol_11\",\"symbol_7\",\"symbol_12\",\"symbol_9\",\"symbol_13\"],\"3\":[\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_8\",\"symbol_3\",\"symbol_11\",\"symbol_12\",\"symbol_3\",\"symbol_8\",\"symbol_7\",\"symbol_2\",\"symbol_6\",\"symbol_4\",\"symbol_6\",\"symbol_10\",\"symbol_3\",\"symbol_7\",\"symbol_9\",\"symbol_5\",\"symbol_10\",\"symbol_6\",\"symbol_2\",\"symbol_4\",\"symbol_7\",\"symbol_5\",\"symbol_10\",\"symbol_3\",\"symbol_11\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_4\",\"symbol_5\",\"symbol_10\",\"symbol_12\",\"symbol_11\",\"symbol_5\",\"symbol_13\"],\"4\":[\"symbol_8\",\"symbol_11\",\"symbol_7\",\"symbol_10\",\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_6\",\"symbol_8\",\"symbol_3\",\"symbol_11\",\"symbol_4\",\"symbol_9\",\"symbol_5\",\"symbol_3\",\"symbol_7\",\"symbol_10\",\"symbol_6\",\"symbol_2\",\"symbol_7\",\"symbol_11\",\"symbol_4\",\"symbol_10\",\"symbol_2\",\"symbol_8\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_5\",\"symbol_11\",\"symbol_13\"],\"5\":[\"symbol_8\",\"symbol_7\",\"symbol_11\",\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_10\",\"symbol_8\",\"symbol_4\",\"symbol_11\",\"symbol_6\",\"symbol_10\",\"symbol_8\",\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_5\",\"symbol_8\",\"symbol_7\",\"symbol_11\",\"symbol_9\",\"symbol_6\",\"symbol_10\",\"symbol_4\",\"symbol_7\",\"symbol_5\",\"symbol_3\",\"symbol_11\",\"symbol_10\",\"symbol_8\",\"symbol_2\",\"symbol_11\",\"symbol_9\",\"symbol_3\",\"symbol_13\"]},\"free\":{\"1\":[\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_8\",\"symbol_12\",\"symbol_10\",\"symbol_11\",\"symbol_4\",\"symbol_6\",\"symbol_3\",\"symbol_12\",\"symbol_2\",\"symbol_6\",\"symbol_4\",\"symbol_3\",\"symbol_7\",\"symbol_2\",\"symbol_8\",\"symbol_5\",\"symbol_11\",\"symbol_9\",\"symbol_2\",\"symbol_7\",\"symbol_5\",\"symbol_10\",\"symbol_3\",\"symbol_8\",\"symbol_11\",\"symbol_4\",\"symbol_6\",\"symbol_7\",\"symbol_5\",\"symbol_3\",\"symbol_12\",\"symbol_9\",\"symbol_2\"],\"2\":[\"symbol_8\",\"symbol_3\",\"symbol_6\",\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_3\",\"symbol_11\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_4\",\"symbol_12\",\"symbol_10\",\"symbol_3\",\"symbol_11\",\"symbol_5\",\"symbol_7\",\"symbol_2\",\"symbol_6\",\"symbol_5\",\"symbol_7\",\"symbol_3\",\"symbol_2\",\"symbol_4\",\"symbol_5\",\"symbol_3\",\"symbol_8\",\"symbol_4\",\"symbol_10\",\"symbol_2\",\"symbol_9\",\"symbol_6\",\"symbol_8\",\"symbol_11\",\"symbol_7\",\"symbol_12\",\"symbol_9\",\"symbol_13\"],\"3\":[\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_8\",\"symbol_3\",\"symbol_11\",\"symbol_12\",\"symbol_3\",\"symbol_8\",\"symbol_7\",\"symbol_2\",\"symbol_6\",\"symbol_4\",\"symbol_6\",\"symbol_10\",\"symbol_3\",\"symbol_7\",\"symbol_9\",\"symbol_5\",\"symbol_6\",\"symbol_2\",\"symbol_4\",\"symbol_7\",\"symbol_5\",\"symbol_10\",\"symbol_3\",\"symbol_11\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_4\",\"symbol_5\",\"symbol_10\",\"symbol_12\",\"symbol_11\",\"symbol_5\",\"symbol_13\"],\"4\":[\"symbol_8\",\"symbol_11\",\"symbol_7\",\"symbol_10\",\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_6\",\"symbol_8\",\"symbol_3\",\"symbol_11\",\"symbol_4\",\"symbol_9\",\"symbol_5\",\"symbol_3\",\"symbol_7\",\"symbol_10\",\"symbol_6\",\"symbol_2\",\"symbol_7\",\"symbol_4\",\"symbol_10\",\"symbol_2\",\"symbol_8\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_5\",\"symbol_11\",\"symbol_13\"],\"5\":[\"symbol_7\",\"symbol_11\",\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_10\",\"symbol_8\",\"symbol_4\",\"symbol_6\",\"symbol_8\",\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_5\",\"symbol_8\",\"symbol_7\",\"symbol_11\",\"symbol_9\",\"symbol_6\",\"symbol_10\",\"symbol_4\",\"symbol_7\",\"symbol_5\",\"symbol_3\",\"symbol_11\",\"symbol_10\",\"symbol_8\",\"symbol_2\",\"symbol_11\",\"symbol_9\",\"symbol_3\",\"symbol_13\"]}},\"exitUrl\":\"https://aristocrat.goldslotpalace.com/\",\"pingInterval\":60000,\"restore\":false,\"hash\":\"71820f5c62022c0d9c261f11671704cc\"}}";
            }
        }
        public Lions50GameLogic()
        {
            _gameID = GAMEID.Lions50;
            GameName = "Lions50";
        }
    }
}
