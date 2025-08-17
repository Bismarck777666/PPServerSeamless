using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace QueenApiNode
{
    public class ChipsetManager
    {
        private static ChipsetManager _sInstance = new ChipsetManager();
        public static ChipsetManager Instance
        {
            get { return _sInstance; }
        }
        protected Dictionary<Currencies, Dictionary<string, bool>>  _notAvailableGames              = new Dictionary<Currencies, Dictionary<string, bool>>();
        protected Dictionary<Currencies, Dictionary<string, PPGameChipsetInfo>> _dicChipSetInfos    = new Dictionary<Currencies, Dictionary<string, PPGameChipsetInfo>>();

        public bool loadChipsetFile()
        {
            _notAvailableGames.Clear();
            _dicChipSetInfos.Clear();
            for (Currencies currency = Currencies.USD; currency < Currencies.COUNT; currency++)
            {
                _notAvailableGames[currency]    = new Dictionary<string, bool>();
                _dicChipSetInfos[currency]              = new Dictionary<string, PPGameChipsetInfo>();
                if (currency == Currencies.KRW)
                    continue;

                string strFileName = string.Format("Chipset/chipset({0}).info", currency);
                using (var reader = new BinaryReader(File.Open(strFileName, FileMode.Open)))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string strGameSymbol                        = reader.ReadString();
                        _notAvailableGames[currency][strGameSymbol] = true;

                    }

                    count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string strGameSymbol    = reader.ReadString();
                        string sc               = reader.ReadString();
                        string defc             = reader.ReadString();
                        string totalBetMin      = reader.ReadString();
                        string totalBetMax      = reader.ReadString();
                        int lineCnt             = reader.ReadInt32();

                        PPGameChipsetInfo info = new PPGameChipsetInfo()
                        {
                            sc          = sc,
                            defc        = defc,
                            totalBetMin = totalBetMin,
                            totalBetMax = totalBetMax,
                            lineCnt     = lineCnt
                        };
                        _dicChipSetInfos[currency].Add(strGameSymbol, info);
                    }
                }
            }
            return true;
        }        

        public bool isAvailableGame(Currencies currency, string strSymbol)
        {
            if (!_notAvailableGames.ContainsKey(currency))
                return true;

            if (_notAvailableGames[currency].ContainsKey(strSymbol))
                return false;

            return true;
        }

        public string getSCOfGame(Currencies currency, string strSymbol)
        {
            if(!_dicChipSetInfos.ContainsKey(currency) && !_dicChipSetInfos[currency].ContainsKey(strSymbol))
                return null;

            return _dicChipSetInfos[currency][strSymbol].sc;
        }

        public int getLinesOfGame(Currencies currency, string strSymbol)
        {
            if (!_dicChipSetInfos.ContainsKey(currency) && !_dicChipSetInfos[currency].ContainsKey(strSymbol))
                return 0;

            return _dicChipSetInfos[currency][strSymbol].lineCnt;
        }
    }

    public class PPGameChipsetInfo
    {
        public string   sc              { get; set; }
        public string   defc            { get; set; }
        public string   totalBetMax     { get; set; }
        public string   totalBetMin     { get; set; }
        public int      lineCnt         { get; set; }
    }
}
