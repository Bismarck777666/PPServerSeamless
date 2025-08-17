using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics.AristoGames
{
    internal class Dragons50GameLogic : BasePPSlotGame
    {
        protected override string SymbolName
        {
            get
            {
                return "50_dragons";
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
                return "{\"status\":\"success\",\"microtime\":0.001664,\"dateTime\":\"2025-07-14 14:22:18\",\"error\":\"\",\"content\":{\"cmd\":\"gameInit\",\"balance\":10000000,\"session\":\"18732734_71876558060e01a9413ef56d39c5b9a8\",\"betInfo\":{\"balanceInCredit\":false,\"denomination\":0.01,\"bet\":1,\"lines\":50},\"betSettings\":{\"balanceInCredit\":false,\"denomination\":[0.01],\"bets\":[1,2,3,4,5,10,20,30,40,50,100,200,300,400,500,600,700,800,900,1000,2000,3000,4000,5000],\"lines\":[50]},\"symbols\":null,\"reels\":{\"base\":{\"1\":[\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_8\",\"symbol_12\",\"symbol_10\",\"symbol_11\",\"symbol_4\",\"symbol_6\",\"symbol_3\",\"symbol_12\",\"symbol_2\",\"symbol_6\",\"symbol_8\",\"symbol_4\",\"symbol_3\",\"symbol_7\",\"symbol_2\",\"symbol_8\",\"symbol_5\",\"symbol_11\",\"symbol_9\",\"symbol_2\",\"symbol_7\",\"symbol_5\",\"symbol_10\",\"symbol_3\",\"symbol_8\",\"symbol_4\",\"symbol_6\",\"symbol_7\",\"symbol_3\",\"symbol_5\",\"symbol_11\",\"symbol_12\",\"symbol_9\",\"symbol_2\"],\"2\":[\"symbol_8\",\"symbol_3\",\"symbol_6\",\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_3\",\"symbol_11\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_4\",\"symbol_12\",\"symbol_10\",\"symbol_3\",\"symbol_11\",\"symbol_5\",\"symbol_7\",\"symbol_2\",\"symbol_6\",\"symbol_5\",\"symbol_7\",\"symbol_9\",\"symbol_3\",\"symbol_2\",\"symbol_4\",\"symbol_5\",\"symbol_3\",\"symbol_8\",\"symbol_4\",\"symbol_10\",\"symbol_2\",\"symbol_9\",\"symbol_6\",\"symbol_8\",\"symbol_11\",\"symbol_7\",\"symbol_12\",\"symbol_9\",\"symbol_13\"],\"3\":[\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_8\",\"symbol_3\",\"symbol_11\",\"symbol_12\",\"symbol_3\",\"symbol_8\",\"symbol_7\",\"symbol_2\",\"symbol_6\",\"symbol_4\",\"symbol_6\",\"symbol_10\",\"symbol_3\",\"symbol_7\",\"symbol_9\",\"symbol_5\",\"symbol_10\",\"symbol_6\",\"symbol_2\",\"symbol_4\",\"symbol_7\",\"symbol_5\",\"symbol_10\",\"symbol_3\",\"symbol_11\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_4\",\"symbol_5\",\"symbol_10\",\"symbol_12\",\"symbol_11\",\"symbol_5\",\"symbol_13\"],\"4\":[\"symbol_8\",\"symbol_11\",\"symbol_7\",\"symbol_10\",\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_6\",\"symbol_8\",\"symbol_3\",\"symbol_11\",\"symbol_4\",\"symbol_9\",\"symbol_5\",\"symbol_3\",\"symbol_7\",\"symbol_10\",\"symbol_6\",\"symbol_2\",\"symbol_7\",\"symbol_11\",\"symbol_4\",\"symbol_10\",\"symbol_2\",\"symbol_8\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_5\",\"symbol_11\",\"symbol_13\"],\"5\":[\"symbol_8\",\"symbol_7\",\"symbol_11\",\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_10\",\"symbol_8\",\"symbol_4\",\"symbol_11\",\"symbol_6\",\"symbol_10\",\"symbol_8\",\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_5\",\"symbol_8\",\"symbol_7\",\"symbol_11\",\"symbol_9\",\"symbol_6\",\"symbol_10\",\"symbol_4\",\"symbol_7\",\"symbol_5\",\"symbol_3\",\"symbol_11\",\"symbol_10\",\"symbol_8\",\"symbol_2\",\"symbol_11\",\"symbol_9\",\"symbol_3\",\"symbol_13\"]},\"free\":{\"1\":[\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_8\",\"symbol_12\",\"symbol_10\",\"symbol_11\",\"symbol_4\",\"symbol_6\",\"symbol_3\",\"symbol_12\",\"symbol_2\",\"symbol_6\",\"symbol_4\",\"symbol_3\",\"symbol_7\",\"symbol_2\",\"symbol_8\",\"symbol_5\",\"symbol_11\",\"symbol_9\",\"symbol_2\",\"symbol_7\",\"symbol_5\",\"symbol_10\",\"symbol_3\",\"symbol_8\",\"symbol_11\",\"symbol_4\",\"symbol_6\",\"symbol_7\",\"symbol_5\",\"symbol_3\",\"symbol_12\",\"symbol_9\",\"symbol_2\"],\"2\":[\"symbol_8\",\"symbol_3\",\"symbol_6\",\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_3\",\"symbol_11\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_4\",\"symbol_12\",\"symbol_10\",\"symbol_3\",\"symbol_11\",\"symbol_5\",\"symbol_7\",\"symbol_2\",\"symbol_6\",\"symbol_5\",\"symbol_7\",\"symbol_3\",\"symbol_2\",\"symbol_4\",\"symbol_5\",\"symbol_3\",\"symbol_8\",\"symbol_4\",\"symbol_10\",\"symbol_2\",\"symbol_9\",\"symbol_6\",\"symbol_8\",\"symbol_11\",\"symbol_7\",\"symbol_12\",\"symbol_9\",\"symbol_13\"],\"3\":[\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_8\",\"symbol_3\",\"symbol_11\",\"symbol_12\",\"symbol_3\",\"symbol_8\",\"symbol_7\",\"symbol_2\",\"symbol_6\",\"symbol_4\",\"symbol_6\",\"symbol_10\",\"symbol_3\",\"symbol_7\",\"symbol_9\",\"symbol_5\",\"symbol_6\",\"symbol_2\",\"symbol_4\",\"symbol_7\",\"symbol_5\",\"symbol_10\",\"symbol_3\",\"symbol_11\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_4\",\"symbol_5\",\"symbol_10\",\"symbol_12\",\"symbol_11\",\"symbol_5\",\"symbol_13\"],\"4\":[\"symbol_8\",\"symbol_11\",\"symbol_7\",\"symbol_10\",\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_6\",\"symbol_8\",\"symbol_3\",\"symbol_11\",\"symbol_4\",\"symbol_9\",\"symbol_5\",\"symbol_3\",\"symbol_7\",\"symbol_10\",\"symbol_6\",\"symbol_2\",\"symbol_7\",\"symbol_4\",\"symbol_10\",\"symbol_2\",\"symbol_8\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_5\",\"symbol_11\",\"symbol_13\"],\"5\":[\"symbol_7\",\"symbol_11\",\"symbol_9\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_1\",\"symbol_10\",\"symbol_8\",\"symbol_4\",\"symbol_6\",\"symbol_8\",\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_5\",\"symbol_8\",\"symbol_7\",\"symbol_11\",\"symbol_9\",\"symbol_6\",\"symbol_10\",\"symbol_4\",\"symbol_7\",\"symbol_5\",\"symbol_3\",\"symbol_11\",\"symbol_10\",\"symbol_8\",\"symbol_2\",\"symbol_11\",\"symbol_9\",\"symbol_3\",\"symbol_13\"]}},\"exitUrl\":\"https://aristocrat.goldslotpalace.com/\",\"pingInterval\":60000,\"restore\":false,\"hash\":\"6382b72eb276cd20ec4b3edc16ab100c\"}}";
            }
        }
        public Dragons50GameLogic()
        {
            _gameID = GAMEID.Dragons50;
            GameName = "Dragons50";
        }
    }
}
