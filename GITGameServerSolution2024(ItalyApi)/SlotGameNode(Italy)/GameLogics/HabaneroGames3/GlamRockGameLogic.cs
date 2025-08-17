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
    public class GlamRockGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGGlamRock";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "4f894941-fe53-4673-9678-f203a4defa97";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "ceae0ca0350d408c225cbb619883899e49b69226";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idLeadSinger",      name = "LeadSinger"     } },
                    {2,   new HabaneroLogSymbolIDName{id = "idGlam",            name = "Glam"           } },
                    {3,   new HabaneroLogSymbolIDName{id = "idRock",            name = "Rock"           } },
                    {4,   new HabaneroLogSymbolIDName{id = "idGroupie",         name = "Groupie"        } },
                    {5,   new HabaneroLogSymbolIDName{id = "idGuitarist",       name = "Guitarist"      } },
                    {6,   new HabaneroLogSymbolIDName{id = "idStage",           name = "Stage"          } },
                    {7,   new HabaneroLogSymbolIDName{id = "idDrummer",         name = "Drummer"        } },
                    {8,   new HabaneroLogSymbolIDName{id = "idRoadie",          name = "Roadie"         } },
                    {9,   new HabaneroLogSymbolIDName{id = "idAmp",             name = "Amp"            } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idJacket",          name = "Jacket"         } },
                    {11,  new HabaneroLogSymbolIDName{id = "idMic",             name = "Mic"            } },
                    {12,  new HabaneroLogSymbolIDName{id = "idBoots",           name = "Boots"          } },
                    {13,  new HabaneroLogSymbolIDName{id = "idBus",             name = "Bus"            } },
                    {14,  new HabaneroLogSymbolIDName{id = "idPedal",           name = "Pedal"          } },
                    {15,  new HabaneroLogSymbolIDName{id = "idPick",            name = "Pick"           } },
                    {16,  new HabaneroLogSymbolIDName{id = "idPoster",          name = "Poster"         } },
                    {17,  new HabaneroLogSymbolIDName{id = "idGuitaristBonus",  name = "GuitaristBonus" } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 258;
            }
        }
        #endregion

        public GlamRockGameLogic()
        {
            _gameID     = GAMEID.GlamRock;
            GameName    = "GlamRock";
        }

        protected override JArray buildHabaneroLogReels(string strUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            JArray reels = new JArray();
            for (int j = 0; j < response["videoslotstate"]["virtualreellist"].Count; j++)
            {
                JArray col = new JArray();
                for (int k = 2; k < response["videoslotstate"]["virtualreellist"][j].Count - 2; k++)
                {
                    int symbol = Convert.ToInt32(response["videoslotstate"]["virtualreellist"][j][k]);
                    try
                    {
                        string symbolid = SymbolIdStringForLog[symbol].id;
                        col.Add(symbolid);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("{0}", symbol);
                    }
                }
                reels.Add(col);
            }

            if (!object.ReferenceEquals(response["videoslotstate"]["expandingwildlist"], null))
            {
                for (int i = 0; i < response["videoslotstate"]["expandingwildlist"].Count; i++)
                {
                    int reelIndex = Convert.ToInt32(response["videoslotstate"]["expandingwildlist"][i]["reelindex"]);
                    int symbolId = Convert.ToInt32(response["videoslotstate"]["expandingwildlist"][i]["symbolid"]);

                    for (int j = 0; j < reels[reelIndex].Count(); j++)
                    {
                        reels[reelIndex][j] = SymbolIdStringForLog[symbolId].id;
                    }
                }
            }
            return reels;
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(response["videoslotstate"]["gamemultiplier"], null))
                eventItem["multiplier"] = response["videoslotstate"]["gamemultiplier"];

            return eventItem;
        }
    }
}
