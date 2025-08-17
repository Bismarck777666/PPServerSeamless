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
   
    class PharaohsGoldGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "47";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 9;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 200, 300, 500, 1000, 2000, 3000, 5000, 10000, 20000, 5, 10, 20, 30, 50, 100 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 20000;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "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";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Pharaoh's Gold";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Pharaoh's Gold\"}," +
                    "{\"lang\":\"ko\",\"name\":\"파라오 골드\"}," +
                    "{\"lang\":\"th\",\"name\":\"ทองของฟาโรห์\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Ouro do Faraó\"}," +
                    "{\"lang\":\"id\",\"name\":\"Emas Firaun\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Vàng Pharaoh\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"法老宝藏\"}]";
            }
        }
        #endregion

        public PharaohsGoldGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet; 
            _gameID                 = GAMEID.PharaohsGold;
            GameName                = "PharaohsGold";
        }

        protected override async Task onDoSpin(string strUserID, int agentID, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            if(message.MsgCode != (ushort)CSMSG_CODE.CS_CQ9_FreeSpinOptSelectRequest)
            {
                await base.onDoSpin(strUserID, agentID, message, userBonus, userBalance, isMustLose);
                return;
            }

            try
            {
                BaseCQ9SlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strUserID, out betInfo))
                {
                    _logger.Error("Exception has been occurred in EcstaticCircusGameLogic::onDoSpin {0}");
                }

                int selectOption = (int)message.Pop();
                dynamic freeOptResultContext = JsonConvert.DeserializeObject<dynamic>(betInfo.RemainReponses[0].Response);

                int oldPlayerSelected   = (int)freeOptResultContext["udcDataSet"]["PlayerSelected"][0];
                int oldFreeSpinCnt      = (int)freeOptResultContext["udcDataSet"]["SelMultiplier"][oldPlayerSelected];
                int newFreeSpinCnt      = (int)freeOptResultContext["udcDataSet"]["SelMultiplier"][selectOption];

                freeOptResultContext["udcDataSet"]["SelMultiplier"][oldPlayerSelected]  = newFreeSpinCnt;
                freeOptResultContext["udcDataSet"]["SelMultiplier"][selectOption]       = oldFreeSpinCnt;
                freeOptResultContext["udcDataSet"]["PlayerSelected"][0]                 = selectOption;

                betInfo.RemainReponses[0].Response = JsonConvert.SerializeObject(freeOptResultContext);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
            
            await spinGame(strUserID, agentID, userBonus, isMustLose, userBalance);
        }
    }
}
