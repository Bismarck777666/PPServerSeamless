using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using MongoDB.Bson.IO;
using Newtonsoft.Json.Linq;
using PCGSharp;

namespace SlotGamesNode.GameLogics
{
    class JackpotPoolActor : ReceiveActor
    {
        private Dictionary<GAMEID, Dictionary<int, JackpotInfo>> _websiteJackpotinfoPerGames = new Dictionary<GAMEID, Dictionary<int, JackpotInfo>>();
        private double _accumulatePercentLeverI = 0.003;
        private double _accumulatePercentLeverII = 0.007;
        private double _accumulatePercentLeverIII = 0.02;
        private double _accumulatePercentLeverIV = 0.07;

        private double _jackpotProbabilityLevelI = 0.1;
        private double _jackpotProbabilityLevelII = 0.005;
        private double _jackpotProbabilityLevelIII = 0.0001;
        private double _jackpotProbabilityLevelIV = 0.00001;

        private double _jackpotInitLevelI = 100;
        private double _jackpotInitLevelII = 1000;
        private double _jackpotInitLevelIII = 10000;
        private double _jackpotInitLevelIV = 100000;

        public JackpotPoolActor()
        {
            Receive<string>(OnProcCommand);
            ReceiveAsync<GetJackpotState>(OnGetJackpotState);
            Receive<SumUpWebsiteJackpotRequest>(OnSumUpWebsiteJackpot);
        }

        protected override void PreStart()
        {
            base.PreStart();
            Self.Tell("start");
        }

        protected void OnProcCommand(string command)
        {
            if(command == "start")
            {
                
            }
        }

        protected async Task OnGetJackpotState(GetJackpotState request)
        {
            try
            {
                int websiteID = request.WebsiteID;
                GAMEID gameID = request.GameID;

                if (!_websiteJackpotinfoPerGames.ContainsKey(gameID))
                    _websiteJackpotinfoPerGames[gameID] = new Dictionary<int, JackpotInfo>();

                if (!_websiteJackpotinfoPerGames[gameID].ContainsKey(websiteID))
                    _websiteJackpotinfoPerGames[gameID][websiteID] = new JackpotInfo();

                JObject response = JObject.FromObject(_websiteJackpotinfoPerGames[gameID][websiteID]);
                response["currentLevelI"] = (long)(response["currentLevelI"]);
                response["currentLevelII"] = (long)(response["currentLevelII"]);
                response["currentLevelIII"] = (long)(response["currentLevelIII"]);
                response["currentLevelIV"] = (long)(response["currentLevelIV"]);

                Sender.Tell(response);
            }
            catch (Exception ex)
            { }
        }

        protected void OnSumUpWebsiteJackpot(SumUpWebsiteJackpotRequest request)
        {
            int websiteID = request.WebsiteID;
            GAMEID gameID = request.GameID;
            double betMoney = request.BetMoney;

            if (!_websiteJackpotinfoPerGames.ContainsKey(gameID))
                _websiteJackpotinfoPerGames[gameID] = new Dictionary<int, JackpotInfo>();

            if (!_websiteJackpotinfoPerGames[gameID].ContainsKey(websiteID))
                _websiteJackpotinfoPerGames[gameID][websiteID] = new JackpotInfo();

            _websiteJackpotinfoPerGames[gameID][websiteID].currentLevelI += betMoney * _accumulatePercentLeverI;
            _websiteJackpotinfoPerGames[gameID][websiteID].currentLevelII += betMoney * _accumulatePercentLeverII;
            _websiteJackpotinfoPerGames[gameID][websiteID].currentLevelIII += betMoney * _accumulatePercentLeverIII;
            _websiteJackpotinfoPerGames[gameID][websiteID].currentLevelIV += betMoney * _accumulatePercentLeverIV;

            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);

            if (randomDouble <= _jackpotProbabilityLevelIV)
            {
                _websiteJackpotinfoPerGames[gameID][websiteID].lastWinLevelIV = (long)(_websiteJackpotinfoPerGames[gameID][websiteID].currentLevelIV);
                _websiteJackpotinfoPerGames[gameID][websiteID].lastWinDateLevelIV = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                _websiteJackpotinfoPerGames[gameID][websiteID].lastWinUserLevelIV = NameGenerator.GeneratePhoneticName((int)Pcg.Default.NextUInt(7) + 3);
                _websiteJackpotinfoPerGames[gameID][websiteID].currentLevelIV = _jackpotInitLevelIV;
                _websiteJackpotinfoPerGames[gameID][websiteID].winsLevelIV += 1;
                if (_websiteJackpotinfoPerGames[gameID][websiteID].lastWinLevelIV > _websiteJackpotinfoPerGames[gameID][websiteID].largestWinLevelIV)
                {
                    _websiteJackpotinfoPerGames[gameID][websiteID].largestWinLevelIV = _websiteJackpotinfoPerGames[gameID][websiteID].lastWinLevelIV;
                    _websiteJackpotinfoPerGames[gameID][websiteID].largestWinDateLevelIV = _websiteJackpotinfoPerGames[gameID][websiteID].lastWinDateLevelIV;
                    _websiteJackpotinfoPerGames[gameID][websiteID].largestWinUserLevelIV = _websiteJackpotinfoPerGames[gameID][websiteID].lastWinUserLevelIV;
                }
            }
            else if (randomDouble > _jackpotProbabilityLevelIV && randomDouble <= _jackpotProbabilityLevelIII)
            {
                _websiteJackpotinfoPerGames[gameID][websiteID].lastWinLevelIII = (long)(_websiteJackpotinfoPerGames[gameID][websiteID].currentLevelIII);
                _websiteJackpotinfoPerGames[gameID][websiteID].lastWinDateLevelIII = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                _websiteJackpotinfoPerGames[gameID][websiteID].lastWinUserLevelIII = NameGenerator.GeneratePhoneticName((int)Pcg.Default.NextUInt(7) + 3);
                _websiteJackpotinfoPerGames[gameID][websiteID].currentLevelIII = _jackpotInitLevelIII;
                _websiteJackpotinfoPerGames[gameID][websiteID].winsLevelIII += 1;
                if (_websiteJackpotinfoPerGames[gameID][websiteID].lastWinLevelIII > _websiteJackpotinfoPerGames[gameID][websiteID].largestWinLevelIII)
                {
                    _websiteJackpotinfoPerGames[gameID][websiteID].largestWinLevelIII = _websiteJackpotinfoPerGames[gameID][websiteID].lastWinLevelIII;
                    _websiteJackpotinfoPerGames[gameID][websiteID].largestWinDateLevelIII = _websiteJackpotinfoPerGames[gameID][websiteID].lastWinDateLevelIII;
                    _websiteJackpotinfoPerGames[gameID][websiteID].largestWinUserLevelIII = _websiteJackpotinfoPerGames[gameID][websiteID].lastWinUserLevelIII;
                }
            }
            else if (randomDouble > _jackpotProbabilityLevelIII && randomDouble <= _jackpotProbabilityLevelII)
            {
                _websiteJackpotinfoPerGames[gameID][websiteID].lastWinLevelII = (long)(_websiteJackpotinfoPerGames[gameID][websiteID].currentLevelII);
                _websiteJackpotinfoPerGames[gameID][websiteID].lastWinDateLevelII = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                _websiteJackpotinfoPerGames[gameID][websiteID].lastWinUserLevelII = NameGenerator.GeneratePhoneticName((int)Pcg.Default.NextUInt(7) + 3);
                _websiteJackpotinfoPerGames[gameID][websiteID].currentLevelII = _jackpotInitLevelII;
                _websiteJackpotinfoPerGames[gameID][websiteID].winsLevelII += 1;
                if (_websiteJackpotinfoPerGames[gameID][websiteID].lastWinLevelII > _websiteJackpotinfoPerGames[gameID][websiteID].largestWinLevelII)
                {
                    _websiteJackpotinfoPerGames[gameID][websiteID].largestWinLevelII = _websiteJackpotinfoPerGames[gameID][websiteID].lastWinLevelII;
                    _websiteJackpotinfoPerGames[gameID][websiteID].largestWinDateLevelII = _websiteJackpotinfoPerGames[gameID][websiteID].lastWinDateLevelII;
                    _websiteJackpotinfoPerGames[gameID][websiteID].largestWinUserLevelII = _websiteJackpotinfoPerGames[gameID][websiteID].lastWinUserLevelII;
                }
            }
            else if (randomDouble > _jackpotProbabilityLevelII && randomDouble <= _jackpotProbabilityLevelI)
            {
                _websiteJackpotinfoPerGames[gameID][websiteID].lastWinLevelI = (long)(_websiteJackpotinfoPerGames[gameID][websiteID].currentLevelI);
                _websiteJackpotinfoPerGames[gameID][websiteID].lastWinDateLevelI = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                _websiteJackpotinfoPerGames[gameID][websiteID].lastWinUserLevelI = NameGenerator.GeneratePhoneticName((int)Pcg.Default.NextUInt(7) + 3);
                _websiteJackpotinfoPerGames[gameID][websiteID].currentLevelI = _jackpotInitLevelI;
                _websiteJackpotinfoPerGames[gameID][websiteID].winsLevelI += 1;
                if (_websiteJackpotinfoPerGames[gameID][websiteID].lastWinLevelI > _websiteJackpotinfoPerGames[gameID][websiteID].largestWinLevelI)
                {
                    _websiteJackpotinfoPerGames[gameID][websiteID].largestWinLevelI = _websiteJackpotinfoPerGames[gameID][websiteID].lastWinLevelI;
                    _websiteJackpotinfoPerGames[gameID][websiteID].largestWinDateLevelI = _websiteJackpotinfoPerGames[gameID][websiteID].lastWinDateLevelI;
                    _websiteJackpotinfoPerGames[gameID][websiteID].largestWinUserLevelI = _websiteJackpotinfoPerGames[gameID][websiteID].lastWinUserLevelI;
                }
            }
        }
    }

    class JackpotInfo
    {
        public double currentLevelI { get; set; } = 100;
        public int winsLevelI { get; set; } = 50;
        public double largestWinLevelI { get; set; } = 11519320;
        public string largestWinDateLevelI { get; set; } = "2024-11-25T19:30:41.085Z";
        public string largestWinUserLevelI { get; set; } = "James1999";
        public double lastWinLevelI { get; set; } = 1900272;
        public string lastWinDateLevelI { get; set; } = "2025-07-21T06:41:19.126Z";
        public string lastWinUserLevelI { get; set; } = "Frangon";

        public double currentLevelII { get; set; } = 1000;
        public int winsLevelII { get; set; } = 35;
        public double largestWinLevelII { get; set; } = 43668967;
        public string largestWinDateLevelII { get; set; } = "2024-11-16T12:57:21.648Z";
        public string largestWinUserLevelII { get; set; } = "Calefrang";
        public double lastWinLevelII { get; set; } = 11460963;
        public string lastWinDateLevelII { get; set; } = "2025-07-21T00:30:52.037Z";
        public string lastWinUserLevelII { get; set; } = "Maongil";

        public double currentLevelIII { get; set; } = 10000;
        public int winsLevelIII { get; set; } = 15;
        public double largestWinLevelIII { get; set; } = 1316734144;
        public string largestWinDateLevelIII { get; set; } = "2025-01-13T19:44:49.571Z";
        public string largestWinUserLevelIII { get; set; } = "Olibak";
        public double lastWinLevelIII { get; set; } = 49280491;
        public string lastWinDateLevelIII { get; set; } = "2025-07-15T05:05:53.468Z";
        public string lastWinUserLevelIII { get; set; } = "Alibam";

        public double currentLevelIV { get; set; } = 100000;
        public int winsLevelIV { get; set; } = 5;
        public double largestWinLevelIV { get; set; } = 7119677079;
        public string largestWinDateLevelIV { get; set; } = "2024-11-17T01:47:32.764Z";
        public string largestWinUserLevelIV { get; set; } = "Elikon";
        public double lastWinLevelIV { get; set; } = 3740684980;
        public string lastWinDateLevelIV { get; set; } = "2025-05-17T23:51:50.2Z";
        public string lastWinUserLevelIV { get; set; } = "Jalito";
    }

    class SumUpWebsiteJackpotRequest
    {
        public int WebsiteID { get; private set; }
        public GAMEID GameID { get; private set; }
        public double BetMoney { get; private set; }

        public SumUpWebsiteJackpotRequest(int websiteID, GAMEID gameID, double betMoney)
        {
            WebsiteID = websiteID;
            GameID = gameID;
            BetMoney = betMoney;
        }
    }

    class GetJackpotState
    {
        public int WebsiteID { get; private set; }
        public GAMEID GameID { get; private set; }

        public GetJackpotState(int websiteID, GAMEID gameID)
        {
            WebsiteID = websiteID;
            GameID = gameID;
        }
    }

    public static class NameGenerator
    {
        private static Random _rand = new Random();

        private static string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "r", "s", "t", "v", "z" };
        private static string[] vowels = { "a", "e", "i", "o", "u" };

        public static string GeneratePhoneticName(int syllables = 3)
        {
            StringBuilder name = new StringBuilder();

            for (int i = 0; i < syllables; i++)
            {
                string c = consonants[_rand.Next(consonants.Length)];
                string v = vowels[_rand.Next(vowels.Length)];
                name.Append(c + v);
            }

            return char.ToUpper(name[0]) + name.ToString(1, name.Length - 1);
        }
    }
}
