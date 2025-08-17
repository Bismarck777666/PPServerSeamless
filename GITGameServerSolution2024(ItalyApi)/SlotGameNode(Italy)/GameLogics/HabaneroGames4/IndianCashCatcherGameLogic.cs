using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class IndianCashCatcherGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGIndianCashCatcher";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "dbc42a0e-fad4-4b5f-a434-da669a5931ec";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "ed2c429d7e4c2983d4ea7834da2c5a3e37359b56";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.1331.93";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 25.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 243;
            }
        }
        protected override string BetType
        {
            get
            {
                return "Ways";
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",        name = "Wild"           } },
                    {2,   new HabaneroLogSymbolIDName{id = "idBonusTotem",  name = "BonusTotem"     } },
                    {3,   new HabaneroLogSymbolIDName{id = "idCashCatcher", name = "CashCatcher"    } },
                    {4,   new HabaneroLogSymbolIDName{id = "idChief",       name = "Chief"          } },
                    {5,   new HabaneroLogSymbolIDName{id = "idGirl",        name = "Girl"           } },
                    {6,   new HabaneroLogSymbolIDName{id = "idWarrior",     name = "Warrior"        } },
                    {7,   new HabaneroLogSymbolIDName{id = "idBison",       name = "Bison"          } },
                    {8,   new HabaneroLogSymbolIDName{id = "idTomahawk",    name = "Tomahawk"       } },
                    {9,   new HabaneroLogSymbolIDName{id = "idAce",         name = "Ace"            } },
                    {10,  new HabaneroLogSymbolIDName{id = "idKing",        name = "King"           } },
                    {11,  new HabaneroLogSymbolIDName{id = "idQueen",       name = "Queen"          } },
                    {12,  new HabaneroLogSymbolIDName{id = "idJack",        name = "Jack"           } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 211;
            }
        }
        #endregion

        public IndianCashCatcherGameLogic()
        {
            _gameID     = GAMEID.IndianCashCatcher;
            GameName    = "IndianCashCatcher";
        }

        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            resumeGames[0]["states"][0]["name"] = lastResult["videoslotstate"]["gamemodename"];
            
            return resumeGames;
        }
    }
}
