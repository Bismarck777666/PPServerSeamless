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
        protected Dictionary<Currencies, Dictionary<string, bool>> _notAvailableGames = new Dictionary<Currencies, Dictionary<string, bool>>();
        public bool loadChipsetFile()
        {
            _notAvailableGames.Clear();
            for (Currencies currency = Currencies.USD; currency < Currencies.COUNT; currency++)
            {
                _notAvailableGames[currency] = new Dictionary<string, bool>();
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
    }
    
}
