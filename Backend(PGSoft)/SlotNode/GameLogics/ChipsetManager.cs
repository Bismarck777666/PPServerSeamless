using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class ChipsetManager
    {
        private static ChipsetManager _sInstance = new ChipsetManager();
        public static ChipsetManager Instance
        {
            get { return _sInstance; }
        }
        protected Dictionary<Currencies, CurrencyChipset> _currencyChipsets = new Dictionary<Currencies, CurrencyChipset>();
        
        public bool loadChipsetFile()
        {
            _currencyChipsets.Clear();
            for (Currencies currency = Currencies.BRL; currency < Currencies.COUNT; currency++)
            {
                CurrencyChipset chipset = new CurrencyChipset(currency);
                if (!chipset.loadFromFile())
                    return false;
                _currencyChipsets[currency] = chipset;
            }
            return true;
        }
        public string getDefaultChipset(Currencies currency, GAMEID gameID)
        {
            if (_currencyChipsets.ContainsKey(currency))
                return _currencyChipsets[currency].getDefaultChipset(gameID);
            return null;
        }
        public int getDefaultBetLevel(Currencies currency, GAMEID gameID)
        {
            if (_currencyChipsets.ContainsKey(currency))
                return _currencyChipsets[currency].getDefaultBetLevel(gameID);
            return 0;
        }

        public void convertTo(Currencies currency, GAMEID gameID, PGGameConfig pgGameConfig)
        {
            if (!_currencyChipsets.ContainsKey(currency))
                return;

            _currencyChipsets[currency].convertTo(gameID, pgGameConfig);
        }
        private Dictionary<string, string> splitResponseToParams(string strResponse)
        {
            string[] strParts = strResponse.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts == null || strParts.Length == 0)
                return new Dictionary<string, string>();

            Dictionary<string, string> dicParamValues = new Dictionary<string, string>();
            for (int i = 0; i < strParts.Length; i++)
            {
                string[] strParamValues = strParts[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParamValues.Length == 2)
                    dicParamValues[strParamValues[0]] = strParamValues[1];
                else if (strParamValues.Length == 1)
                    dicParamValues[strParamValues[0]] = null;
            }
            return dicParamValues;
        }
        private string convertKeyValuesToString(Dictionary<string, string> keyValues)
        {
            List<string> parts = new List<string>();
            foreach (KeyValuePair<string, string> pair in keyValues)
            {
                if (pair.Value == null)
                    parts.Add(string.Format("{0}=", pair.Key));
                else
                    parts.Add(string.Format("{0}={1}", pair.Key, pair.Value));
            }
            return string.Join("&", parts.ToArray());
        }
    }
    public class PGGameChipsetInfo
    {
        public string           defaultcs       { get; set; }
        public int              defaultbetlevel { get; set; }
        public List<double>     cs              { get; set; }
        public List<int>        ml              { get; set; }

        public PGGameChipsetInfo()
        {
            this.cs = new List<double>();
            this.ml = new List<int>();
        }
    }
    
    public class CurrencyChipset
    {
        private Dictionary<GAMEID, PGGameChipsetInfo>       _chipsetInfos = new Dictionary<GAMEID, PGGameChipsetInfo>();
        private Currencies                                  _currency     = Currencies.THB;
       
        public CurrencyChipset(Currencies currency)
        {
            _chipsetInfos   = new Dictionary<GAMEID, PGGameChipsetInfo>();
            _currency       = currency;
        }
        public bool loadFromFile()
        {
            try
            {
                _chipsetInfos.Clear();
                string strFileName = string.Format("Chipset/Chipset({0}).info", _currency);
                using (var reader  = new BinaryReader(File.Open(strFileName, FileMode.Open)))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        GAMEID gameID = (GAMEID)(int)reader.ReadInt32();
                        var chipsetInfo                 = new PGGameChipsetInfo();
                        chipsetInfo.defaultbetlevel     = reader.ReadInt32();
                        chipsetInfo.defaultcs           = reader.ReadString();

                        int subCount = reader.ReadInt32();
                        for(int j = 0; j < subCount; j++)
                            chipsetInfo.cs.Add(Math.Round(double.Parse(reader.ReadString()), 2));

                        subCount = reader.ReadInt32();
                        for (int j = 0; j < subCount; j++)
                            chipsetInfo.ml.Add(reader.ReadInt32());

                        _chipsetInfos[gameID] = chipsetInfo;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string getDefaultChipset(GAMEID gameID)
        {
            if (_chipsetInfos.ContainsKey(gameID))
                return _chipsetInfos[gameID].defaultcs;
            return null;
        }
        public int getDefaultBetLevel(GAMEID gameID)
        {
            if (_chipsetInfos.ContainsKey(gameID))
                return _chipsetInfos[gameID].defaultbetlevel;
            return 0;
        }
        public void convertTo(GAMEID gameID, PGGameConfig pgGameConfig)
        {
            if (!_chipsetInfos.ContainsKey(gameID))
                return;

            pgGameConfig.ml = _chipsetInfos[gameID].ml;
            pgGameConfig.cs = _chipsetInfos[gameID].cs;
        }
        private Dictionary<string, string> splitResponseToParams(string strResponse)
        {
            string[] strParts = strResponse.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts == null || strParts.Length == 0)
                return new Dictionary<string, string>();

            Dictionary<string, string> dicParamValues = new Dictionary<string, string>();
            for (int i = 0; i < strParts.Length; i++)
            {
                string[] strParamValues = strParts[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParamValues.Length == 2)
                    dicParamValues[strParamValues[0]] = strParamValues[1];
                else if (strParamValues.Length == 1)
                    dicParamValues[strParamValues[0]] = null;
            }
            return dicParamValues;
        }
        private string convertKeyValuesToString(Dictionary<string, string> keyValues)
        {
            List<string> parts = new List<string>();
            foreach (KeyValuePair<string, string> pair in keyValues)
            {
                if (pair.Value == null)
                    parts.Add(string.Format("{0}=", pair.Key));
                else
                    parts.Add(string.Format("{0}={1}", pair.Key, pair.Value));
            }
            return string.Join("&", parts.ToArray());
        }
    }
}
