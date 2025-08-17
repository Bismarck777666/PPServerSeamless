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
   
    class EcstaticCircusGameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "51";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 20;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 50, 100, 200, 400, 800, 1200, 2000, 4000, 8000, 1, 2, 5, 10, 30 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 8000;
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
                return "Ecstatic Circus";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"Ecstatic Circus\"}," +
                    "{\"lang\":\"ko\",\"name\":\"엑스타틱 서커스\"}," +
                    "{\"lang\":\"th\",\"name\":\"สุขสันต์ละครสัตว์\"}," +
                    "{\"lang\":\"pt-br\",\"name\":\"Circo em êxtase\"}," +
                    "{\"lang\":\"id\",\"name\":\"Sirkus Ekstatik\"}," +
                    "{\"lang\":\"vn\",\"name\":\"Xiếc xuất thần\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"嗨爆大马戏\"}]";
            }
        }
        #endregion

        public EcstaticCircusGameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.DenomDefine   = DenomDefine;
            _initData.MaxBet        = MaxBet; 
            _gameID                 = GAMEID.EcstaticCircus;
            GameName                = "EcstaticCircus";
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
                int oldFreeSpinCnt      = (int)freeOptResultContext["udcDataSet"]["SelSpinTimes"][oldPlayerSelected];
                int newFreeSpinCnt      = (int)freeOptResultContext["udcDataSet"]["SelSpinTimes"][selectOption];

                freeOptResultContext["udcDataSet"]["SelSpinTimes"][oldPlayerSelected]   = newFreeSpinCnt;
                freeOptResultContext["udcDataSet"]["SelSpinTimes"][selectOption]        = oldFreeSpinCnt;
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
