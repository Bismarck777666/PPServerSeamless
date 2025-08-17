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
    public class DrFeelgoodGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGDrFeelgood";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "d33291e3-d67a-42d0-8186-76bcfcdf5ca3";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "c69712e46e8d2a89e10d80b8ff62cc51ed90a7a8";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.12512.857";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idNurse",       name = "Nurse"          } },
                    {2,   new HabaneroLogSymbolIDName{id = "idLoveMeter",   name = "LoveMeter"      } },
                    {3,   new HabaneroLogSymbolIDName{id = "idDoctor",      name = "Doctor"         } },
                    {4,   new HabaneroLogSymbolIDName{id = "idPatient",     name = "Patient"        } },
                    {5,   new HabaneroLogSymbolIDName{id = "idHospital",    name = "Hospital"       } },
                    {6,   new HabaneroLogSymbolIDName{id = "idAmbulance",   name = "Ambulance"      } },
                    {7,   new HabaneroLogSymbolIDName{id = "idTelephone",   name = "Telephone"      } },
                    {8,   new HabaneroLogSymbolIDName{id = "idStethoscope", name = "Stethoscope"    } },
                    {9,   new HabaneroLogSymbolIDName{id = "idFlowers",     name = "Flowers"        } },
                    {10,  new HabaneroLogSymbolIDName{id = "idStretcher",   name = "Stretcher"      } },
                    {11,  new HabaneroLogSymbolIDName{id = "idChart",       name = "Chart"          } },
                    {12,  new HabaneroLogSymbolIDName{id = "idTeddyBear",   name = "TeddyBear"      } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 179;
            }
        }
        #endregion

        public DrFeelgoodGameLogic()
        {
            _gameID     = GAMEID.DrFeelgood;
            GameName    = "DrFeelgood";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
                eventItem["multiplier"] = 1;

            return eventItem;
        }
    }
}
