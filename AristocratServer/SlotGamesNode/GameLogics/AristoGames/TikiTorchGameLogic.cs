using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics.AristoGames
{
    internal class TikiTorchGameLogic : BasePPSlotGame
    {
        protected override string SymbolName
        {
            get
            {
                return "tiki_torch";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 20; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
        }
        protected override string InitDataString
        {
            get
            {
                return "{\"status\":\"success\",\"microtime\":0.001217,\"dateTime\":\"2025-07-16 13:18:48\",\"error\":\"\",\"content\":{\"cmd\":\"gameInit\",\"balance\":10000000,\"session\":\"18877969_f89742b925dd784e07bbbd6b64b947e3\",\"betInfo\":{\"balanceInCredit\":false,\"denomination\":0.01,\"bet\":1,\"lines\":20},\"betSettings\":{\"balanceInCredit\":false,\"denomination\":[0.01],\"bets\":[1,2,3,4,5,10,20,30,40,50,100,200,300,400,500,600,700,800,900,1000,2000,3000,4000,5000,10000],\"lines\":[20]},\"symbols\":null,\"reels\":{\"base\":{\"1\":[\"symbol_5\",\"symbol_7\",\"symbol_9\",\"symbol_1\",\"symbol_10\",\"symbol_5\",\"symbol_2\",\"symbol_6\",\"symbol_9\",\"symbol_1\",\"symbol_8\",\"symbol_7\",\"symbol_12\",\"symbol_6\",\"symbol_10\",\"symbol_3\",\"symbol_7\",\"symbol_9\",\"symbol_2\",\"symbol_8\",\"symbol_9\",\"symbol_12\",\"symbol_7\",\"symbol_9\",\"symbol_2\",\"symbol_7\",\"symbol_6\",\"symbol_1\",\"symbol_8\",\"symbol_5\",\"symbol_9\",\"symbol_7\",\"symbol_11\",\"symbol_10\",\"symbol_3\",\"symbol_9\",\"symbol_7\",\"symbol_4\",\"symbol_9\",\"symbol_11\",\"symbol_12\",\"symbol_10\",\"symbol_11\",\"symbol_4\",\"symbol_7\",\"symbol_11\",\"symbol_12\"],\"2\":[\"symbol_9\",\"symbol_10\",\"symbol_1\",\"symbol_5\",\"symbol_6\",\"symbol_2\",\"symbol_8\",\"symbol_6\",\"symbol_10\",\"symbol_4\",\"symbol_8\",\"symbol_6\",\"symbol_7\",\"symbol_8\",\"symbol_6\",\"symbol_1\",\"symbol_5\",\"symbol_11\",\"symbol_8\",\"symbol_5\",\"symbol_10\",\"symbol_8\",\"symbol_12\",\"symbol_6\",\"symbol_8\",\"symbol_10\",\"symbol_3\",\"symbol_5\",\"symbol_6\",\"symbol_8\",\"symbol_4\",\"symbol_5\",\"symbol_9\",\"symbol_12\",\"symbol_11\",\"symbol_10\",\"symbol_2\",\"symbol_11\",\"symbol_8\",\"symbol_10\",\"symbol_7\",\"symbol_12\",\"symbol_10\",\"symbol_6\",\"symbol_7\",\"symbol_3\",\"symbol_10\",\"symbol_6\"],\"3\":[\"symbol_9\",\"symbol_8\",\"symbol_1\",\"symbol_9\",\"symbol_11\",\"symbol_4\",\"symbol_7\",\"symbol_11\",\"symbol_6\",\"symbol_1\",\"symbol_8\",\"symbol_10\",\"symbol_2\",\"symbol_7\",\"symbol_11\",\"symbol_3\",\"symbol_7\",\"symbol_8\",\"symbol_5\",\"symbol_2\",\"symbol_11\",\"symbol_8\",\"symbol_12\",\"symbol_9\",\"symbol_5\",\"symbol_1\",\"symbol_6\",\"symbol_3\",\"symbol_9\",\"symbol_5\",\"symbol_4\",\"symbol_9\",\"symbol_10\",\"symbol_12\",\"symbol_6\",\"symbol_10\",\"symbol_2\",\"symbol_11\",\"symbol_5\",\"symbol_2\",\"symbol_11\"],\"4\":[\"symbol_6\",\"symbol_7\",\"symbol_1\",\"symbol_9\",\"symbol_11\",\"symbol_4\",\"symbol_10\",\"symbol_2\",\"symbol_5\",\"symbol_9\",\"symbol_3\",\"symbol_11\",\"symbol_10\",\"symbol_9\",\"symbol_5\",\"symbol_6\",\"symbol_12\",\"symbol_9\",\"symbol_11\",\"symbol_8\",\"symbol_4\",\"symbol_11\",\"symbol_8\",\"symbol_12\",\"symbol_11\",\"symbol_1\",\"symbol_9\",\"symbol_8\",\"symbol_3\",\"symbol_5\",\"symbol_6\",\"symbol_11\",\"symbol_10\",\"symbol_5\",\"symbol_7\",\"symbol_2\",\"symbol_11\",\"symbol_7\",\"symbol_12\",\"symbol_11\",\"symbol_10\",\"symbol_7\",\"symbol_3\",\"symbol_10\"],\"5\":[\"symbol_9\",\"symbol_10\",\"symbol_1\",\"symbol_8\",\"symbol_11\",\"symbol_2\",\"symbol_10\",\"symbol_6\",\"symbol_5\",\"symbol_7\",\"symbol_10\",\"symbol_11\",\"symbol_6\",\"symbol_12\",\"symbol_11\",\"symbol_9\",\"symbol_8\",\"symbol_7\",\"symbol_9\",\"symbol_4\",\"symbol_10\",\"symbol_5\",\"symbol_9\",\"symbol_3\",\"symbol_7\",\"symbol_1\",\"symbol_9\",\"symbol_12\",\"symbol_5\",\"symbol_6\",\"symbol_4\",\"symbol_10\",\"symbol_8\",\"symbol_3\",\"symbol_11\",\"symbol_10\",\"symbol_12\",\"symbol_11\",\"symbol_9\",\"symbol_4\",\"symbol_8\"]}},\"exitUrl\":\"https://aristocrat.goldslotpalace.com/\",\"pingInterval\":60000,\"restore\":false,\"hash\":\"5f3664fcc2a8e77e09bf461e5d0fe4c6\"}}";
            }
        }
        public TikiTorchGameLogic()
        {
            _gameID = GAMEID.TikiTorch;
            GameName = "TikiTorch";
        }
    }
}
