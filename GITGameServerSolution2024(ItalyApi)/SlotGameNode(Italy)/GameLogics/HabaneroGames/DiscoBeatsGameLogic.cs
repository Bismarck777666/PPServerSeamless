using Akka.Actor;
using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class DiscoBeatsGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGDiscoBeats";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "f17f4825-7402-4c86-ab8c-067398dc2ecc";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "3253e03a6b2ed6dd09627ff5bbcb205a449ed4c0";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.9757.405";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 9.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 27;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",        name = "Wild"           } },    //와일드(허트)
                    {2,   new HabaneroLogSymbolIDName{id = "idScatter",     name = "Scatter"        } },    //스캐터
                    {3,   new HabaneroLogSymbolIDName{id = "idDiscoBall",   name = "idDiscoBall"    } },    //조명
                    {4,   new HabaneroLogSymbolIDName{id = "idVinylRecord", name = "VinylRecord"    } },    //레코드
                    {5,   new HabaneroLogSymbolIDName{id = "idGoldNote",    name = "idGoldNote"     } },    //노란색 쏠음기호
                    {6,   new HabaneroLogSymbolIDName{id = "idPurpleNote",  name = "PurpleNote"     } },    //분홍색 4분기호
                    {7,   new HabaneroLogSymbolIDName{id = "idBlueNote",    name = "BlueNote"       } },    //파란8분기호
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 623;
            }
        }
        #endregion

        public DiscoBeatsGameLogic()
        {
            _gameID     = GAMEID.DiscoBeats;
            GameName    = "DiscoBeats";
        }

        protected override JObject buildFGMessage(dynamic response)
        {
            JObject cashPrizeItem = new JObject();
            cashPrizeItem["type"]       = "cashprize";
            cashPrizeItem["customtype"] = "DiscoBeatsGameReport";
            cashPrizeItem["pId"]        = response["FGMessage"]["id"];
            cashPrizeItem["wincash"]    = response["FGMessage"]["winCash"];
            return cashPrizeItem;
        }
        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid,HabaneroActionType currentAction)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserId, betInfo, lastResult, gameinstanceid, roundid);

            if (!object.ReferenceEquals(lastResult["FGMessage"], null) && !object.ReferenceEquals(lastResult["FGMessage"]["id"], null))
            {
                for (int i = 0; i < resumeGames.Count; i++)
                {
                    JObject resumeGameItem = resumeGames[i] as JObject;
                    resumeGameItem["lastSpinWinId"] = lastResult["FGMessage"]["id"];
                    resumeGames[i] = resumeGameItem;
                }
            }
            return resumeGames;
        }
    }
}
