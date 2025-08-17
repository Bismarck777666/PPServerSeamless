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
    public class LegendaryBeastsGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGLegendaryBeasts";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "4a4997c5-93ed-41e0-b6d7-6180f2a9c8e9";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "90ea1ffdf714cd10c5ea63143fc8c8baf9de3944";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.10808.424";
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
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",          name = "Wild"           } },
                    {2,     new HabaneroLogSymbolIDName{id = "idDragon",        name = "Dragon"         } },
                    {3,     new HabaneroLogSymbolIDName{id = "idPhoenix",       name = "Phoenix"        } },
                    {4,     new HabaneroLogSymbolIDName{id = "idTiger",         name = "Tiger"          } },
                    {5,     new HabaneroLogSymbolIDName{id = "idGoldCoin",      name = "GoldCoin"       } },
                    {6,     new HabaneroLogSymbolIDName{id = "idSilverCoin",    name = "SilverCoin"     } },
                    {7,     new HabaneroLogSymbolIDName{id = "idBronzeCoin",    name = "BronzeCoin"     } },
                    {8,     new HabaneroLogSymbolIDName{id = "idMult1",         name = "Mult1"          } },
                    {9,     new HabaneroLogSymbolIDName{id = "idMult2",         name = "Mult2"          } },
                    {10,    new HabaneroLogSymbolIDName{id = "idMult3",         name = "Mult3"          } },
                    {11,    new HabaneroLogSymbolIDName{id = "idMult6",         name = "Mult6"          } },
                    {12,    new HabaneroLogSymbolIDName{id = "idMult9",         name = "Mult9"          } },

                    {13,    new HabaneroLogSymbolIDName{id = "idWildX2",        name = "WildX2"         } },
                    {14,    new HabaneroLogSymbolIDName{id = "idDragonX2",      name = "DragonX2"       } },
                    {15,    new HabaneroLogSymbolIDName{id = "idPhoenixX2",     name = "PhoenixX2"      } },
                    {16,    new HabaneroLogSymbolIDName{id = "idTigerX2",       name = "TigerX2"        } },
                    {17,    new HabaneroLogSymbolIDName{id = "idGoldCoinX2",    name = "GoldCoinX2"     } },
                    {18,    new HabaneroLogSymbolIDName{id = "idSilverCoinX2",  name = "SilverCoinX2"   } },
                    {19,    new HabaneroLogSymbolIDName{id = "idBronzeCoinX2",  name = "BronzeCoinX2"   } },
                    
                    {20,    new HabaneroLogSymbolIDName{id = "idWildX4",        name = "WildX4"         } },
                    {21,    new HabaneroLogSymbolIDName{id = "idDragonX4",      name = "DragonX4"       } },
                    {22,    new HabaneroLogSymbolIDName{id = "idPhoenixX4",     name = "PhoenixX4"      } },
                    {23,    new HabaneroLogSymbolIDName{id = "idTigerX4",       name = "TigerX4"        } },
                    {24,    new HabaneroLogSymbolIDName{id = "idGoldCoinX4",    name = "GoldCoinX4"     } },
                    {25,    new HabaneroLogSymbolIDName{id = "idSilverCoinX4",  name = "SilverCoinX4"   } },
                    {26,    new HabaneroLogSymbolIDName{id = "idBronzeCoinX4",  name = "BronzeCoinX4"   } },


                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 753;
            }
        }
        protected override string BetType
        {
            get
            {
                return "Ways";
            }
        }

        #endregion

        public LegendaryBeastsGameLogic()
        {
            _gameID     = GAMEID.LegendaryBeasts;
            GameName    = "LegendaryBeasts";
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserID, currentIndex);
            
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            int multiplier  = 1;
            int multisymbol = response["reels"][3][1]["symbolid"];
            if(multisymbol == 8)
                multiplier = 1;
            else if (multisymbol == 9)
                multiplier = 2;
            else if (multisymbol == 10)
                multiplier = 3;
            else if (multisymbol == 11)
                multiplier = 6;
            else if (multisymbol == 12)
                multiplier = 9;
            eventItem["multiplier"] = multiplier;

            if(!object.ReferenceEquals(response["legendaryBeasts_waymultiplierlist"], null))
            {
                for (int i = 0; i < response["legendaryBeasts_waymultiplierlist"].Count; i++)
                {
                    int mul         = response["legendaryBeasts_waymultiplierlist"][i]["multiplier"];
                    int reelindex   = response["legendaryBeasts_waymultiplierlist"][i]["pos"]["reelindex"];
                    int symbolindex = response["legendaryBeasts_waymultiplierlist"][i]["pos"]["symbolindex"];
                    int symbolid    = response["reels"][reelindex][symbolindex]["symbolid"];

                    if(mul == 2)
                        symbolid += 12;
                    else if(mul == 4)
                        symbolid += 19;
                    eventItem["reels"][reelindex][symbolindex] = SymbolIdStringForLog[symbolid].id;
                }

                JArray reelslist        = new JArray();
                JObject reelslistitem   = new JObject();
                JArray reels = buildHabaneroLogReels(strGlobalUserID, currentIndex, response);
                reelslistitem["reels"] = reels;
                reelslist.Add(reelslistitem);

                eventItem["reelslist"] = reelslist;
            }
            
            return eventItem;
        }
    }
}
