using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SlotGamesNode.GameLogics
{
    class SunOfEgypt3GameLogic : BaseBNGSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "sun_of_egypt_3";
            }
        }
        protected override string InitResultString
        {
            get
            {
                return "{\"actions\":[\"spin\"],\"current\":\"spins\",\"last_action\":\"init\",\"last_args\":{},\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bet_per_line\":60,\"board\":[[8,8,8],[9,9,4],[9,9,3],[9,2,1],[11,12,1]],\"bs_v\":[[0,0,0],[0,0,0],[0,0,0],[0,0,0],[\"minor\",0,0]],\"bs_values\":[[0,0,0],[0,0,0],[0,0,0],[0,0,0],[50,0,0]],\"is_sb_trigger\":false,\"is_super_state\":false,\"lines\":25,\"round_bet\":1500,\"round_win\":0,\"total_win\":0},\"version\":1}";
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 25;
            }
        }
        protected override string SettingString
        {
            get
            {
                return "{\"bet_factor\":[25],\"bets\":[4,8,10,16,20,30,32,40,50,60,80,100,120,160,200,240,300,400,500,600,800],\"big_win\":[30,50,70],\"bonus_symbols\":[1,2,3,4,5,6,7,8,9,10,12,15,\"mini\",\"minor\",\"major\"],\"cols\":5,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"jackpots\":{\"grand\":2000,\"major\":150,\"mini\":20,\"minor\":50,\"royal\":10000},\"lines\":[25],\"paylines\":[[1,1,1,1,1],[0,0,0,0,0],[2,2,2,2,2],[0,1,2,1,0],[2,1,0,1,2],[1,0,0,0,1],[1,2,2,2,1],[0,0,1,2,2],[2,2,1,0,0],[1,2,1,0,1],[1,0,1,2,1],[0,1,1,1,0],[2,1,1,1,2],[0,1,0,1,0],[2,1,2,1,2],[1,1,0,1,1],[1,1,2,1,1],[0,0,2,0,0],[2,2,0,2,2],[0,2,2,2,0],[2,0,0,0,2],[1,2,0,2,1],[1,0,2,0,1],[0,2,0,2,0],[2,0,2,0,2]],\"paytable\":{\"1\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"2\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"3\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"4\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":5,\"type\":\"lb\"}],\"5\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":25,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":5,\"type\":\"lb\"}],\"6\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":150,\"occurrences\":5,\"type\":\"lb\"}],\"7\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":200,\"occurrences\":5,\"type\":\"lb\"}],\"8\":[{\"multiplier\":15,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":50,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":250,\"occurrences\":5,\"type\":\"lb\"}],\"9\":[{\"multiplier\":20,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":60,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":300,\"occurrences\":5,\"type\":\"lb\"}],\"10\":[{\"freespins\":8,\"multiplier\":2,\"occurrences\":3,\"trigger\":\"freespins\",\"type\":\"tb\"}]},\"reelsamples\":{\"freespins\":[[5,6,7,8,9,11],[5,6,7,8,9,10,11],[5,6,7,8,9,10,11],[5,6,7,8,9,10,11],[5,6,7,8,9,11,12]],\"last_freespins\":[[5,6,7,8,9],[5,6,7,8,9,10],[5,6,7,8,9,10],[5,6,7,8,9,10],[5,6,7,8,9]],\"spins\":[[1,2,3,4,5,6,7,8,9,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,10,11],[1,2,3,4,5,6,7,8,9,11,12]]},\"rows\":3,\"symbols\":[{\"id\":1,\"name\":\"el_01\",\"type\":\"line\"},{\"id\":2,\"name\":\"el_02\",\"type\":\"line\"},{\"id\":3,\"name\":\"el_03\",\"type\":\"line\"},{\"id\":4,\"name\":\"el_04\",\"type\":\"line\"},{\"id\":5,\"name\":\"el_05\",\"type\":\"line\"},{\"id\":6,\"name\":\"el_06\",\"type\":\"line\"},{\"id\":7,\"name\":\"el_07\",\"type\":\"line\"},{\"id\":8,\"name\":\"el_08\",\"type\":\"line\"},{\"id\":9,\"name\":\"el_wild\",\"type\":\"wild\"},{\"id\":10,\"name\":\"el_scatter\",\"type\":\"scat\"},{\"id\":11,\"name\":\"el_bonus\",\"type\":\"scat\"},{\"id\":12,\"name\":\"el_super_bonus\",\"type\":\"scat\"},{\"id\":13,\"name\":\"el_mystery\",\"type\":\"scat\"}],\"symbols_line\":[1,2,3,4,5,6,7,8],\"symbols_scat\":[10,11,12,13],\"symbols_wild\":[9],\"transitions\":[{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"init\",\"win\":false},\"dst\":\"spins\",\"src\":\"none\"},{\"act\":{\"args\":[\"bet_per_line\",\"lines\"],\"bet\":true,\"cheat\":true,\"name\":\"spin\",\"win\":true},\"dst\":\"spins\",\"src\":\"spins\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"freespin_init\",\"win\":false},\"dst\":\"freespins\",\"src\":\"spins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"freespin\",\"win\":true},\"dst\":\"freespins\",\"src\":\"freespins\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"freespin_stop\",\"win\":false},\"dst\":\"spins\",\"src\":\"freespins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"bonus_init\",\"win\":false},\"dst\":\"bonus\",\"src\":\"freespins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"bonus_init\",\"win\":false},\"dst\":\"bonus\",\"src\":\"spins\"},{\"act\":{\"bet\":false,\"cheat\":true,\"name\":\"respin\",\"win\":true},\"dst\":\"bonus\",\"src\":\"bonus\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"bonus_spins_stop\",\"win\":false},\"dst\":\"spins\",\"src\":\"bonus\"},{\"act\":{\"bet\":false,\"cheat\":false,\"name\":\"bonus_freespins_stop\",\"win\":false},\"dst\":\"freespins\",\"src\":\"bonus\"}],\"version\":\"a\"}";
            }
        }
        #endregion
        public SunOfEgypt3GameLogic()
        {
            _gameID = GAMEID.SunOfEgypt3;
            GameName = "SunOfEgypt3";
        }

        protected override void addInfoForStart(dynamic context, string strGlobalUserID)
        {
            if (context.current != "bonus")
                return;

            string bonusBackTo = "";
            if (context.bonus.back_to != null)
                bonusBackTo = context.bonus.back_to;

            if (!_dicUserBetInfos.ContainsKey(strGlobalUserID))
                return;

            BaseBNGSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
            dynamic lastResponse = null;
            for (int i = 0; i < betInfo.RemainReponses.Count; i++)
            {
                dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(betInfo.RemainReponses[i].Response);
                if (resultContext["current"] == bonusBackTo)
                {
                    lastResponse = resultContext;
                    break;
                }
            }
            if (lastResponse == null)
                return;

            if (bonusBackTo == "freespins")
            {
                context.freespins = JToken.Parse("{}");
                context.freespins["board"] = JToken.Parse(lastResponse.freespins.board.ToString());
            }
            else if (bonusBackTo == "spins")
            {
                context.spins = JToken.Parse("{}");
                context.spins["board"] = JToken.Parse(lastResponse.spins.board.ToString());
            }
        }
    }
}
