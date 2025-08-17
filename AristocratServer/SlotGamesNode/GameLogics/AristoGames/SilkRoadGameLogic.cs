using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics.AristoGames
{
    internal class SilkRoadGameLogic : BasePPSlotGame
    {
        protected override string SymbolName
        {
            get
            {
                return "silk_road";
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
                return "{\"status\":\"success\",\"microtime\":0.001211,\"dateTime\":\"2025-07-30 06:19:13\",\"error\":\"\",\"content\":{\"cmd\":\"gameInit\",\"balance\":9999885,\"session\":\"19822260_603dd8c49776971cc0c47607e1f71d34\",\"betInfo\":{\"balanceInCredit\":false,\"denomination\":0.01,\"bet\":1,\"lines\":5},\"betSettings\":{\"balanceInCredit\":false,\"denomination\":[0.01],\"bets\":[1,2,3,4,5,10,20,30,40,50,100,200,300,400,500,600,700,800,900,1000,2000,3000,4000,5000,10000],\"lines\":[5]},\"symbols\":{\"1\":{\"1\":\"symbol_6\",\"2\":\"symbol_9\",\"3\":\"symbol_8\"},\"2\":{\"1\":\"symbol_9\",\"2\":\"symbol_11\",\"3\":\"symbol_4\"},\"3\":{\"1\":\"symbol_8\",\"2\":\"symbol_4\",\"3\":\"symbol_11\"},\"4\":{\"1\":\"symbol_9\",\"2\":\"symbol_4\",\"3\":\"symbol_8\"},\"5\":{\"1\":\"symbol_7\",\"2\":\"symbol_6\",\"3\":\"symbol_8\"}},\"reels\":{\"base\":{\"1\":[\"symbol_6\",\"symbol_8\",\"symbol_3\",\"symbol_6\",\"symbol_4\",\"symbol_7\",\"symbol_6\",\"symbol_9\",\"symbol_8\",\"symbol_5\",\"symbol_7\",\"symbol_6\",\"symbol_9\",\"symbol_11\",\"symbol_5\",\"symbol_10\",\"symbol_1\",\"symbol_6\",\"symbol_10\",\"symbol_8\",\"symbol_4\",\"symbol_9\",\"symbol_7\",\"symbol_9\",\"symbol_5\",\"symbol_10\",\"symbol_8\",\"symbol_7\",\"symbol_9\",\"symbol_3\",\"symbol_10\",\"symbol_2\",\"symbol_11\"],\"2\":[\"symbol_8\",\"symbol_3\",\"symbol_10\",\"symbol_7\",\"symbol_5\",\"symbol_8\",\"symbol_6\",\"symbol_9\",\"symbol_7\",\"symbol_10\",\"symbol_11\",\"symbol_3\",\"symbol_12\",\"symbol_10\",\"symbol_5\",\"symbol_8\",\"symbol_4\",\"symbol_10\",\"symbol_9\",\"symbol_11\",\"symbol_4\",\"symbol_5\",\"symbol_9\",\"symbol_12\",\"symbol_8\",\"symbol_6\",\"symbol_7\",\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_1\",\"symbol_7\",\"symbol_6\"],\"3\":[\"symbol_5\",\"symbol_7\",\"symbol_9\",\"symbol_3\",\"symbol_10\",\"symbol_8\",\"symbol_4\",\"symbol_11\",\"symbol_6\",\"symbol_8\",\"symbol_5\",\"symbol_7\",\"symbol_9\",\"symbol_10\",\"symbol_8\",\"symbol_2\",\"symbol_7\",\"symbol_9\",\"symbol_6\",\"symbol_10\",\"symbol_8\",\"symbol_10\",\"symbol_5\",\"symbol_9\",\"symbol_1\",\"symbol_9\",\"symbol_6\",\"symbol_11\",\"symbol_7\",\"symbol_9\"],\"4\":[\"symbol_10\",\"symbol_7\",\"symbol_9\",\"symbol_4\",\"symbol_8\",\"symbol_5\",\"symbol_7\",\"symbol_9\",\"symbol_6\",\"symbol_10\",\"symbol_8\",\"symbol_6\",\"symbol_1\",\"symbol_8\",\"symbol_7\",\"symbol_2\",\"symbol_10\",\"symbol_8\",\"symbol_12\",\"symbol_9\",\"symbol_8\",\"symbol_5\",\"symbol_10\",\"symbol_7\",\"symbol_3\",\"symbol_11\",\"symbol_10\",\"symbol_6\",\"symbol_9\",\"symbol_5\"],\"5\":[\"symbol_9\",\"symbol_5\",\"symbol_10\",\"symbol_7\",\"symbol_9\",\"symbol_6\",\"symbol_10\",\"symbol_8\",\"symbol_6\",\"symbol_9\",\"symbol_3\",\"symbol_8\",\"symbol_1\",\"symbol_10\",\"symbol_7\",\"symbol_2\",\"symbol_5\",\"symbol_10\",\"symbol_7\",\"symbol_5\",\"symbol_8\",\"symbol_9\",\"symbol_11\",\"symbol_7\",\"symbol_6\",\"symbol_8\",\"symbol_9\",\"symbol_6\",\"symbol_4\",\"symbol_7\",\"symbol_10\",\"symbol_8\"]},\"free\":{\"1\":[\"symbol_6\",\"symbol_8\",\"symbol_3\",\"symbol_6\",\"symbol_4\",\"symbol_7\",\"symbol_6\",\"symbol_9\",\"symbol_8\",\"symbol_5\",\"symbol_7\",\"symbol_6\",\"symbol_9\",\"symbol_11\",\"symbol_5\",\"symbol_10\",\"symbol_1\",\"symbol_6\",\"symbol_10\",\"symbol_8\",\"symbol_4\",\"symbol_9\",\"symbol_7\",\"symbol_9\",\"symbol_5\",\"symbol_10\",\"symbol_8\",\"symbol_7\",\"symbol_9\",\"symbol_3\",\"symbol_10\",\"symbol_2\",\"symbol_11\"],\"2\":[\"symbol_8\",\"symbol_3\",\"symbol_10\",\"symbol_7\",\"symbol_5\",\"symbol_8\",\"symbol_6\",\"symbol_9\",\"symbol_7\",\"symbol_10\",\"symbol_11\",\"symbol_3\",\"symbol_12\",\"symbol_10\",\"symbol_5\",\"symbol_8\",\"symbol_4\",\"symbol_10\",\"symbol_9\",\"symbol_11\",\"symbol_4\",\"symbol_5\",\"symbol_9\",\"symbol_12\",\"symbol_8\",\"symbol_6\",\"symbol_7\",\"symbol_9\",\"symbol_2\",\"symbol_10\",\"symbol_1\",\"symbol_7\",\"symbol_6\"],\"3\":[\"symbol_5\",\"symbol_7\",\"symbol_9\",\"symbol_3\",\"symbol_10\",\"symbol_8\",\"symbol_4\",\"symbol_11\",\"symbol_6\",\"symbol_8\",\"symbol_5\",\"symbol_7\",\"symbol_9\",\"symbol_10\",\"symbol_8\",\"symbol_2\",\"symbol_7\",\"symbol_9\",\"symbol_6\",\"symbol_10\",\"symbol_8\",\"symbol_10\",\"symbol_5\",\"symbol_9\",\"symbol_1\",\"symbol_9\",\"symbol_6\",\"symbol_11\",\"symbol_7\",\"symbol_9\"],\"4\":[\"symbol_10\",\"symbol_7\",\"symbol_9\",\"symbol_4\",\"symbol_8\",\"symbol_5\",\"symbol_7\",\"symbol_9\",\"symbol_6\",\"symbol_10\",\"symbol_8\",\"symbol_6\",\"symbol_1\",\"symbol_8\",\"symbol_7\",\"symbol_2\",\"symbol_10\",\"symbol_8\",\"symbol_12\",\"symbol_9\",\"symbol_8\",\"symbol_5\",\"symbol_10\",\"symbol_7\",\"symbol_3\",\"symbol_11\",\"symbol_10\",\"symbol_6\",\"symbol_9\",\"symbol_5\"],\"5\":[\"symbol_9\",\"symbol_5\",\"symbol_10\",\"symbol_7\",\"symbol_9\",\"symbol_6\",\"symbol_10\",\"symbol_8\",\"symbol_6\",\"symbol_9\",\"symbol_3\",\"symbol_8\",\"symbol_1\",\"symbol_10\",\"symbol_7\",\"symbol_2\",\"symbol_5\",\"symbol_10\",\"symbol_7\",\"symbol_5\",\"symbol_8\",\"symbol_9\",\"symbol_11\",\"symbol_7\",\"symbol_6\",\"symbol_8\",\"symbol_9\",\"symbol_6\",\"symbol_4\",\"symbol_7\",\"symbol_10\",\"symbol_8\"]}},\"exitUrl\":\"https://aristocrat.goldslotpalace.com/\",\"pingInterval\":60000,\"restore\":false,\"hash\":\"cb2f27ab794193e3b52a3985416b6996\"}}";
            }
        }
        public SilkRoadGameLogic()
        {
            _gameID = GAMEID.SilkRoad;
            GameName = "SilkRoad";
        }
    }
}
