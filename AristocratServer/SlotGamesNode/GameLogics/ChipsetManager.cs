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
        protected Dictionary<Currencies,    CurrencyChipset>        _currencyChipsets   = new Dictionary<Currencies, CurrencyChipset>();
        
        public bool loadChipsetFile()
        {
            _currencyChipsets.Clear();
            for (Currencies currency = Currencies.MYR; currency < Currencies.COUNT; currency++)
            {
                if (currency == Currencies.MYR || currency == Currencies.SGD)
                    continue;

                CurrencyChipset chipset = new CurrencyChipset(currency);
                if (!chipset.loadFromFile())
                    return false;
                _currencyChipsets[currency] = chipset;
            }
            return true;
        }
        public string convertTo(Currencies currency, string strInitString, string strSymbol)
        {
            if (currency == Currencies.MYR || currency == Currencies.SGD)
                return strInitString;

            if (!_currencyChipsets.ContainsKey(currency))
                return "";

            return _currencyChipsets[currency].convertTo(strInitString, strSymbol);
        }
    }
    public class PPGameChipsetInfo
    {
        public string sc            { get; set; }
        public string defc          { get; set; }
        public string totalBetMax   { get; set; }
        public string totalBetMin   { get; set; }
    }
    public class PPGameRTPInfo
    {
        public string rtp       { get; set; }
        public string gameInfo  { get; set; }
    }
    public class CurrencyChipset
    {
        private Dictionary<string, PPGameChipsetInfo> _chipsetInfos = new Dictionary<string, PPGameChipsetInfo>();
        private Currencies                            _currency     = Currencies.MYR;
       
        public CurrencyChipset(Currencies currency)
        {
            _chipsetInfos   = new Dictionary<string, PPGameChipsetInfo>();
            _currency       = currency;
        }
        public bool loadFromFile()
        {
            try
            {
                _chipsetInfos.Clear();
                string strFileName = string.Format("Chipset/chipset({0}).info", _currency);
                using (var reader  = new BinaryReader(File.Open(strFileName, FileMode.Open)))
                {
                    int count = reader.ReadInt32();
                    for(int i = 0; i < count; i++)
                    {
                        string strGameSymbol = reader.ReadString();
                    }
                    count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string strGameSymbol            = reader.ReadString();
                        var chipsetInfo                 = new PPGameChipsetInfo();
                        chipsetInfo.sc                  = reader.ReadString();
                        chipsetInfo.defc                = reader.ReadString();
                        chipsetInfo.totalBetMin         = reader.ReadString();
                        chipsetInfo.totalBetMax         = reader.ReadString();
                        _chipsetInfos[strGameSymbol]    = chipsetInfo;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string convertTo(string strInitString, string strSymbol)
        {
            if (!_chipsetInfos.ContainsKey(strSymbol))
                return strInitString;

            var dicParams   = splitResponseToParams(strInitString);
            var chipset     = _chipsetInfos[strSymbol];
            if (dicParams.ContainsKey("sc") && dicParams["sc"] != chipset.sc)
                dicParams["sc"] = chipset.sc;

            if (dicParams.ContainsKey("defc") && dicParams["defc"] != chipset.defc)
                dicParams["defc"] = chipset.defc;

            if (dicParams.ContainsKey("total_bet_min") && !string.IsNullOrEmpty(chipset.totalBetMin))
                dicParams["total_bet_min"] = chipset.totalBetMin;

            if (dicParams.ContainsKey("total_bet_max") && !string.IsNullOrEmpty(chipset.totalBetMax))
                dicParams["total_bet_max"] = chipset.totalBetMax;

            return convertKeyValuesToString(dicParams);
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
