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
    public class SkysTheLimitGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGSkysTheLimit";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "4a95d81b-da9c-4d66-aab3-7c66051896dc";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "83e83b18895e21b9fe77c5f4c29cad2aedd61667";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idSkydiver",        name = "Skydiver"           } },
                    {2,   new HabaneroLogSymbolIDName{id = "idTandemSkydiver",  name = "TandemSkydiver"     } },
                    {3,   new HabaneroLogSymbolIDName{id = "idParachutist",     name = "Parachutist"        } },
                    {4,   new HabaneroLogSymbolIDName{id = "idPlane",           name = "Plane"              } },
                    {5,   new HabaneroLogSymbolIDName{id = "idParachute",       name = "Parachute"          } },
                    {6,   new HabaneroLogSymbolIDName{id = "idHelmet",          name = "Helmet"             } },
                    {7,   new HabaneroLogSymbolIDName{id = "idVideoCamera",     name = "VideoCamera"        } },
                    {8,   new HabaneroLogSymbolIDName{id = "idGoggles",         name = "Goggles"            } },
                    {9,   new HabaneroLogSymbolIDName{id = "idWatch",           name = "Watch"              } },
                    {10,  new HabaneroLogSymbolIDName{id = "idGloves",          name = "Gloves"             } },
                    {11,  new HabaneroLogSymbolIDName{id = "idLogBook",         name = "LogBook"            } },
                    {12,  new HabaneroLogSymbolIDName{id = "idAltimeter",       name = "Altimeter"          } },
                    {13,  new HabaneroLogSymbolIDName{id = "idFreeParachutist", name = "FreeParachutist"    } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 78;
            }
        }
        #endregion

        public SkysTheLimitGameLogic()
        {
            _gameID     = GAMEID.SkysTheLimit;
            GameName    = "SkysTheLimit";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);

            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserId].Responses[currentIndex].Response);

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
            {
                int multiplier = 2;
                if (Convert.ToInt32(resultContext["currentfreegame"]) > 6)
                    multiplier = 4;

                eventItem["multiplier"] = multiplier;
            }

            if (!object.ReferenceEquals(resultContext["reels"], null))
            {
                int winFreeCnt = 0;
                JArray reelArray = resultContext["reels"];
                for (int i = 0; i < reelArray.Count; i++)
                {
                    JArray reelRow = reelArray[i] as JArray;
                    for (int j = 0; j < reelRow.Count; j++)
                    {
                        if (!object.ReferenceEquals(reelRow[j]["winfreegames"], null) && Convert.ToInt32(reelRow[j]["winfreegames"]) > 0)
                        {
                            winFreeCnt += Convert.ToInt32(reelRow[j]["winfreegames"]);
                        }
                    }
                }
                if(winFreeCnt > 0)
                    eventItem["winfreegames"] = winFreeCnt;
            }

            dynamic eventContext = eventItem as dynamic;
            if (!object.ReferenceEquals(eventContext["subevents"], null) && eventContext["subevents"].Count > 0)
            {
                for (int i = 0; i < eventContext["subevents"].Count; i++)
                {
                    if (eventContext["subevents"][i]["type"] == "scatter")
                    {
                        eventContext["subevents"][i]["symbol"] = SymbolIdStringForLog[3].name;
                    }
                }
            }
            return eventItem;
        }
    }
}
