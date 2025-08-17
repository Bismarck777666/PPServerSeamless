using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics.AristoGames
{
    internal class GeishaGameLogic : BasePPSlotGame
    {
        protected override string SymbolName
        {
            get
            {
                return "geisha";
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
                return "{\"status\":\"success\",\"microtime\":0.002765,\"dateTime\":\"2025-07-14 10:07:33\",\"error\":\"\",\"content\":{\"cmd\":\"gameInit\",\"balance\":10000000,\"session\":\"18719242_2857f23e834158c08b43a814ba69db0d\",\"betInfo\":{\"balanceInCredit\":false,\"denomination\":0.01,\"bet\":1,\"lines\":25},\"betSettings\":{\"balanceInCredit\":false,\"denomination\":[0.01],\"bets\":[1,2,3,4,5,10,20,30,40,50,100,200,300,400,500,600,700,800,900,1000,2000,3000,4000,5000,10000],\"lines\":[25]},\"symbols\":null,\"reels\":{\"base\":{\"1\":[\"symbol_6\",\"symbol_8\",\"symbol_3\",\"symbol_11\",\"symbol_6\",\"symbol_12\",\"symbol_4\",\"symbol_7\",\"symbol_6\",\"symbol_9\",\"symbol_13\",\"symbol_10\",\"symbol_1\",\"symbol_12\",\"symbol_6\",\"symbol_10\",\"symbol_4\",\"symbol_9\",\"symbol_5\",\"symbol_10\",\"symbol_2\",\"symbol_9\",\"symbol_3\",\"symbol_10\",\"symbol_2\",\"symbol_11\"],\"2\":[\"symbol_8\",\"symbol_3\",\"symbol_10\",\"symbol_12\",\"symbol_5\",\"symbol_10\",\"symbol_12\",\"symbol_5\",\"symbol_10\",\"symbol_4\",\"symbol_12\",\"symbol_10\",\"symbol_5\",\"symbol_8\",\"symbol_3\",\"symbol_10\",\"symbol_13\",\"symbol_9\",\"symbol_11\",\"symbol_4\",\"symbol_10\",\"symbol_5\",\"symbol_12\",\"symbol_6\",\"symbol_7\",\"symbol_2\",\"symbol_10\",\"symbol_1\",\"symbol_7\",\"symbol_6\"],\"3\":[\"symbol_11\",\"symbol_12\",\"symbol_5\",\"symbol_7\",\"symbol_9\",\"symbol_3\",\"symbol_8\",\"symbol_9\",\"symbol_4\",\"symbol_8\",\"symbol_11\",\"symbol_2\",\"symbol_9\",\"symbol_8\",\"symbol_2\",\"symbol_7\",\"symbol_9\",\"symbol_2\",\"symbol_11\",\"symbol_10\",\"symbol_13\",\"symbol_11\",\"symbol_9\",\"symbol_1\",\"symbol_11\",\"symbol_9\",\"symbol_6\",\"symbol_11\",\"symbol_8\",\"symbol_9\"],\"4\":[\"symbol_10\",\"symbol_7\",\"symbol_4\",\"symbol_8\",\"symbol_2\",\"symbol_9\",\"symbol_6\",\"symbol_12\",\"symbol_8\",\"symbol_6\",\"symbol_12\",\"symbol_1\",\"symbol_8\",\"symbol_7\",\"symbol_2\",\"symbol_8\",\"symbol_3\",\"symbol_12\",\"symbol_8\",\"symbol_5\",\"symbol_7\",\"symbol_3\",\"symbol_11\",\"symbol_13\",\"symbol_10\",\"symbol_6\",\"symbol_12\",\"symbol_5\",\"symbol_11\",\"symbol_4\"],\"5\":[\"symbol_13\",\"symbol_9\",\"symbol_5\",\"symbol_10\",\"symbol_2\",\"symbol_9\",\"symbol_6\",\"symbol_11\",\"symbol_3\",\"symbol_10\",\"symbol_4\",\"symbol_8\",\"symbol_2\",\"symbol_9\",\"symbol_3\",\"symbol_8\",\"symbol_1\",\"symbol_10\",\"symbol_3\",\"symbol_7\",\"symbol_2\",\"symbol_12\",\"symbol_5\",\"symbol_10\",\"symbol_3\",\"symbol_7\",\"symbol_5\",\"symbol_12\",\"symbol_6\",\"symbol_11\",\"symbol_4\",\"symbol_7\",\"symbol_6\",\"symbol_11\",\"symbol_5\",\"symbol_9\",\"symbol_4\",\"symbol_2\",\"symbol_11\",\"symbol_6\",\"symbol_4\",\"symbol_7\",\"symbol_3\",\"symbol_8\"]},\"free\":{\"1\":[\"symbol_6\",\"symbol_1\",\"symbol_8\",\"symbol_3\",\"symbol_11\",\"symbol_6\",\"symbol_12\",\"symbol_4\",\"symbol_7\",\"symbol_6\",\"symbol_9\",\"symbol_13\",\"symbol_10\",\"symbol_1\",\"symbol_12\",\"symbol_6\",\"symbol_10\",\"symbol_4\",\"symbol_9\",\"symbol_5\",\"symbol_10\",\"symbol_2\",\"symbol_9\",\"symbol_3\",\"symbol_10\",\"symbol_2\",\"symbol_11\"],\"2\":[\"symbol_8\",\"symbol_3\",\"symbol_1\",\"symbol_10\",\"symbol_12\",\"symbol_5\",\"symbol_10\",\"symbol_12\",\"symbol_5\",\"symbol_10\",\"symbol_4\",\"symbol_12\",\"symbol_10\",\"symbol_5\",\"symbol_8\",\"symbol_3\",\"symbol_10\",\"symbol_13\",\"symbol_9\",\"symbol_11\",\"symbol_4\",\"symbol_10\",\"symbol_5\",\"symbol_12\",\"symbol_6\",\"symbol_7\",\"symbol_2\",\"symbol_10\",\"symbol_1\",\"symbol_7\",\"symbol_6\"],\"3\":[\"symbol_11\",\"symbol_12\",\"symbol_5\",\"symbol_7\",\"symbol_9\",\"symbol_3\",\"symbol_8\",\"symbol_9\",\"symbol_4\",\"symbol_8\",\"symbol_11\",\"symbol_2\",\"symbol_9\",\"symbol_8\",\"symbol_2\",\"symbol_7\",\"symbol_9\",\"symbol_2\",\"symbol_11\",\"symbol_10\",\"symbol_13\",\"symbol_11\",\"symbol_9\",\"symbol_1\",\"symbol_11\",\"symbol_9\",\"symbol_6\",\"symbol_11\",\"symbol_8\",\"symbol_9\"],\"4\":[\"symbol_10\",\"symbol_7\",\"symbol_4\",\"symbol_8\",\"symbol_1\",\"symbol_2\",\"symbol_9\",\"symbol_6\",\"symbol_12\",\"symbol_8\",\"symbol_6\",\"symbol_12\",\"symbol_1\",\"symbol_8\",\"symbol_7\",\"symbol_2\",\"symbol_8\",\"symbol_3\",\"symbol_12\",\"symbol_8\",\"symbol_5\",\"symbol_7\",\"symbol_3\",\"symbol_11\",\"symbol_13\",\"symbol_10\",\"symbol_6\",\"symbol_12\",\"symbol_5\",\"symbol_11\",\"symbol_4\"],\"5\":[\"symbol_13\",\"symbol_9\",\"symbol_5\",\"symbol_10\",\"symbol_2\",\"symbol_1\",\"symbol_9\",\"symbol_6\",\"symbol_11\",\"symbol_3\",\"symbol_10\",\"symbol_4\",\"symbol_8\",\"symbol_2\",\"symbol_9\",\"symbol_3\",\"symbol_8\",\"symbol_1\",\"symbol_10\",\"symbol_3\",\"symbol_7\",\"symbol_2\",\"symbol_12\",\"symbol_5\",\"symbol_10\",\"symbol_3\",\"symbol_7\",\"symbol_5\",\"symbol_12\",\"symbol_6\",\"symbol_11\",\"symbol_4\",\"symbol_7\",\"symbol_6\",\"symbol_11\",\"symbol_5\",\"symbol_9\",\"symbol_4\",\"symbol_2\",\"symbol_11\",\"symbol_6\",\"symbol_4\",\"symbol_7\",\"symbol_3\",\"symbol_8\"]}},\"exitUrl\":\"https://aristocrat.goldslotpalace.com\",\"pingInterval\":60000,\"restore\":false,\"hash\":\"51dd4c4133c4b56cc43a838459d95188\"}}";
            }
        }
        public GeishaGameLogic()
        {
            _gameID = GAMEID.Geisha;
            GameName = "Geisha";
        }
    }
}
