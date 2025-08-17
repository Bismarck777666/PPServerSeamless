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
    public class JugglenautGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGJugglenaut";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "d2773397-33a7-4a3c-84e4-011f6357539b";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "0c0ecd5e5aa98acad69653645239535cd32748da";
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
                return 50.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 50;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idJuggler",   name = "Juggler"    } },
                    {2,     new HabaneroLogSymbolIDName{id = "idClown",     name = "Clown"      } },
                    {3,     new HabaneroLogSymbolIDName{id = "idChainsaw",  name = "Chainsaw"   } },
                    {4,     new HabaneroLogSymbolIDName{id = "idDumbbell",  name = "Dumbbell"   } },
                    {5,     new HabaneroLogSymbolIDName{id = "idCageball",  name = "Cageball"   } },
                    {6,     new HabaneroLogSymbolIDName{id = "idAxe",       name = "Axe"        } },
                    {7,     new HabaneroLogSymbolIDName{id = "idAce",       name = "Ace"        } },
                    {8,     new HabaneroLogSymbolIDName{id = "idKing",      name = "King"       } },
                    {9,     new HabaneroLogSymbolIDName{id = "idQueen",     name = "Queen"      } },
                    {10,    new HabaneroLogSymbolIDName{id = "idJack",      name = "Jack"       } },
                    {11,    new HabaneroLogSymbolIDName{id = "idTen",       name = "Ten"        } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 244;
            }
        }
        #endregion

        public JugglenautGameLogic()
        {
            _gameID     = GAMEID.Jugglenaut;
            GameName    = "Jugglenaut";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);

            dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserId].Responses[currentIndex].Response);
            if (!object.ReferenceEquals(resultContext["videoslotstate"]["gamemultiplier"], null))
                eventItem["multiplier"] = resultContext["videoslotstate"]["gamemultiplier"];
            
            if (!object.ReferenceEquals(resultContext["videoslotstate"]["statemessage"], null))
                eventItem["statemessage"] = resultContext["videoslotstate"]["statemessage"];

            dynamic eventContext    = eventItem as dynamic;
            if (!object.ReferenceEquals(eventContext["subevents"], null) && eventContext["subevents"].Count > 0)
            {
                for (int i = 0; i < eventContext["subevents"].Count; i++)
                {
                    if (eventContext["subevents"][i]["type"] == "scatter")
                    {
                        eventContext["subevents"][i]["symbol"] = SymbolIdStringForLog[2].name;
                    }
                }
            }

            return eventItem;
        }

        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction)
        {
            JArray resumeGames = base.buildInitResumeGame(strUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            dynamic resultContext = lastResult;
            if(!object.ReferenceEquals(resultContext["videoslotstate"]["winfreegame"], null))
            {
                JObject resumeObject = new JObject();
                resumeObject["showsplashscreen"] = true;
                resumeObject["winfreegamecount"] = resultContext["videoslotstate"]["winfreegame"];

                resumeGames[0]["resumeobject"] = JsonConvert.SerializeObject(resumeObject);
                resumeGames[0]["states"][0]["name"] = convertActionToString(currentAction);
            }
            return resumeGames;
        }
    }
}
