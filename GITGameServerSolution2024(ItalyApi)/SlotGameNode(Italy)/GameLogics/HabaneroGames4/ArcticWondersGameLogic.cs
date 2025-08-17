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
    public class ArcticWondersGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGArcticWonders";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "a6e35ae0-f887-4a55-aaf3-2c7047183647";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "0a3f9f2de6000d574517c467951e898d6e5ab32a";
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
                return 25;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idNorthernLights",  name = "NorthernLights" } },
                    {2,   new HabaneroLogSymbolIDName{id = "idPolarBear",       name = "PolarBear"      } },
                    {3,   new HabaneroLogSymbolIDName{id = "idPenguin",         name = "Penguin"        } },
                    {4,   new HabaneroLogSymbolIDName{id = "idEskimo",          name = "Eskimo"         } },
                    {5,   new HabaneroLogSymbolIDName{id = "idHusky",           name = "Husky"          } },
                    {6,   new HabaneroLogSymbolIDName{id = "idSeal",            name = "Seal"           } },
                    {7,   new HabaneroLogSymbolIDName{id = "idIgloo",           name = "Igloo"          } },
                    {8,   new HabaneroLogSymbolIDName{id = "idGloves",          name = "Gloves"         } },
                    {9,   new HabaneroLogSymbolIDName{id = "idGoggles",         name = "Goggles"        } },
                    {10,  new HabaneroLogSymbolIDName{id = "idMap",             name = "Map"            } },
                    {11,  new HabaneroLogSymbolIDName{id = "idFish",            name = "Fish"           } },
                    {12,  new HabaneroLogSymbolIDName{id = "idIcepick",         name = "Icepick"        } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 129;
            }
        }
        #endregion

        public ArcticWondersGameLogic()
        {
            _gameID     = GAMEID.ArcticWonders;
            GameName    = "ArcticWonders";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(response["videoslotstate"]["gamemultiplier"], null))
                eventItem["multiplier"] = response["videoslotstate"]["gamemultiplier"];

            if (!object.ReferenceEquals(response["videoslotstate"]["expandingwildlist"], null) && response["videoslotstate"]["expandingwildlist"].Count > 0)
            {
                for(int i = 0; i < response["videoslotstate"]["expandingwildlist"].Count; i++)
                {
                    int reelIndex   = response["videoslotstate"]["expandingwildlist"][i]["reelindex"];
                    int symbolId    = response["videoslotstate"]["expandingwildlist"][i]["symbolid"];

                    string symbol = SymbolIdStringForLog[symbolId].id;
                    for(int j = 0; j < 3; j++)
                    {
                        eventItem["reels"][reelIndex][j] = symbol;
                    }
                }
            }
            
            return eventItem;
        }

        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            dynamic lastContext = lastResult as dynamic;
            resumeGames[0]["states"][0]["name"]         = lastResult["videoslotstate"]["gamemodename"];
            resumeGames[0]["states"][0]["virtualreels"] = lastResult["videoslotstate"]["virtualreellist"];
            if (!object.ReferenceEquals(lastContext["videoslotstate"]["expandingwildlist"], null) && lastContext["videoslotstate"]["expandingwildlist"].Count > 0)
            {
                for (int i = 0; i < lastContext["videoslotstate"]["expandingwildlist"].Count; i++)
                {
                    int reelIndex   = lastContext["videoslotstate"]["expandingwildlist"][i]["reelindex"];
                    int symbolId    = lastContext["videoslotstate"]["expandingwildlist"][i]["symbolid"];

                    for (int j = 0; j < 3; j++)
                    {
                        resumeGames[0]["states"][0]["virtualreels"][reelIndex][j + 2] = symbolId;
                    }
                }
            }


            return resumeGames;
        }
    }
}
