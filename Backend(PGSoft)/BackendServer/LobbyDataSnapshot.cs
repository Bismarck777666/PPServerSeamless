using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using SlotGamesNode.Database;
using MongoDB.Bson.IO;
using Newtonsoft.Json;

namespace SlotGamesNode
{
    internal class LobbyDataSnapshot
    {
        private static LobbyDataSnapshot _sInstance = new LobbyDataSnapshot();
        public static LobbyDataSnapshot Instance
        {
            get { return _sInstance; }
        }
        private Dictionary<LangCodes, string> _lobbyGetJsons            = new Dictionary<LangCodes, string>();
        private Dictionary<LangCodes, string> _lobbyGameResourcesJsons  = new Dictionary<LangCodes, string>();
        private Dictionary<LangCodes, string> _lobbyGameDetailsJsons    = new Dictionary<LangCodes, string>();

        public bool loadJsons()
        {
            try
            {
                for (LangCodes langCode = LangCodes.en_US; langCode <= LangCodes.ko_KO; langCode++)
                {
                    string strFileName = string.Format("lobbydata/LobbyGet-{0}.json", convertLangCodeToString(langCode));
                    _lobbyGetJsons.Add(langCode, File.ReadAllText(strFileName));

                    strFileName = string.Format("lobbydata/LobbyGameResources-{0}.json", convertLangCodeToString(langCode));
                    _lobbyGameResourcesJsons.Add(langCode, File.ReadAllText(strFileName));

                    strFileName = string.Format("lobbydata/LobbyGameDetails-{0}.json", convertLangCodeToString(langCode));
                    _lobbyGameDetailsJsons.Add(langCode, File.ReadAllText(strFileName));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }        
        public void removeGamesNotReady()
        {
            for(int i = 0; i < _lobbyGetJsons.Keys.Count; i++)
            {
                LangCodes langCode  = _lobbyGetJsons.Keys.ElementAt(i);
                string    strJson   = _lobbyGetJsons[langCode];
                JToken    jsonData  = JToken.Parse(strJson);

                JArray gameDataArray = jsonData["dt"]["wlii"]["gmi"]["gm"] as JArray;
                //Remove games which has not been developed yet.
                int index = 0;
                do
                {
                    JToken gameData = gameDataArray[index];
                    int gid = (int)gameData["gid"];
                    if (gid > 0 && PGGamesSnapshot.Instance.findGameStringFromID(gid) == null)
                    {
                        gameDataArray.RemoveAt(index);
                        continue;
                    }
                    index++;
                } while (index < gameDataArray.Count);

                gameDataArray = jsonData["dt"]["wlii"]["gri"]["gr"] as JArray;

                //Remove games which has not been developed yet.
                index = 0;
                do
                {
                    JToken gameData = gameDataArray[index];
                    int gid = (int)gameData["rid"];
                    if (gid > 0 && PGGamesSnapshot.Instance.findGameStringFromID(gid) == null)
                    {
                        gameDataArray.RemoveAt(index);
                        continue;
                    }
                    index++;
                } while (index < gameDataArray.Count);

                gameDataArray = jsonData["dt"]["wlii"]["opci"]["op"] as JArray; //player count information
                //Remove games which has not been developed yet.
                index = 0;
                do
                {
                    JToken gameData = gameDataArray[index];
                    int gid = (int)gameData["gid"];
                    if (gid > 0 && PGGamesSnapshot.Instance.findGameStringFromID(gid) == null)
                    {
                        gameDataArray.RemoveAt(index);
                        continue;
                    }
                    index++;
                } while (index < gameDataArray.Count);
                
                var categoryInfos = jsonData["dt"]["wlii"]["ti"]["t"] as JArray;
                foreach(var categoryInfo in categoryInfos)
                {
                    var gameIdArray = categoryInfo["gid"] as JArray;
                    index = 0;
                    do
                    {
                        int gid = (int)gameIdArray[index]; 
                        if (gid > 0 && PGGamesSnapshot.Instance.findGameStringFromID(gid) == null)
                        {
                            gameIdArray.RemoveAt(index);
                            continue;
                        }
                        index++;
                    } while (index < gameIdArray.Count);
                }
                _lobbyGetJsons[langCode] = jsonData.ToString();
            }

            for(int i = 0; i < _lobbyGameResourcesJsons.Keys.Count; i++)
            {
                LangCodes langCode      = _lobbyGameResourcesJsons.Keys.ElementAt(i);
                string    strJson       = _lobbyGameResourcesJsons[langCode];
                JToken    jsonData      = JToken.Parse(strJson);
                JArray    gameDataArray = jsonData["dt"]["gr"] as JArray;
                //Remove games which has not been developed yet.
                int index = 0;
                do
                {
                    JToken gameData = gameDataArray[index];
                    int gid = (int)gameData["rid"];
                    if (gid > 0 && PGGamesSnapshot.Instance.findGameStringFromID(gid) == null)
                    {
                        gameDataArray.RemoveAt(index);
                        continue;
                    }
                    index++;
                } while (index < gameDataArray.Count);
                _lobbyGameResourcesJsons[langCode] = jsonData.ToString();
            }
            for(int i = 0; i < _lobbyGameDetailsJsons.Keys.Count; i++)
            {
                LangCodes   langCode        = _lobbyGameDetailsJsons.Keys.ElementAt(i);
                string      strJson         = _lobbyGameDetailsJsons[langCode];
                JToken      jsonData        = JToken.Parse(strJson);
                JArray      gameDataArray   = jsonData["dt"]["gm"] as JArray;
                //Remove games which has not been developed yet.
                int index = 0;
                do
                {
                    JToken gameData = gameDataArray[index];
                    int gid = (int)gameData["gid"];
                    if (gid > 0 && PGGamesSnapshot.Instance.findGameStringFromID(gid) == null)
                    {
                        gameDataArray.RemoveAt(index);
                        continue;
                    }
                    index++;
                } while (index < gameDataArray.Count);
                _lobbyGameDetailsJsons[langCode] = jsonData.ToString();
            }
        }
        public string getLobbyGetJson(string lang)
        {
            LangCodes langCode = convertStringToLangCode(lang);
            if(_lobbyGetJsons.ContainsKey(langCode))
                return _lobbyGetJsons[langCode];

            return _lobbyGetJsons[LangCodes.en_US];
        }
        public string getLobbyGameResourcesJson(string lang)
        {
            LangCodes langCode = convertStringToLangCode(lang);
            if (_lobbyGameResourcesJsons.ContainsKey(langCode))
                return _lobbyGameResourcesJsons[langCode];

            return _lobbyGameResourcesJsons[LangCodes.en_US];
        }
        public string getLobbyGameDetailsJson(string lang)
        {
            LangCodes langCode = convertStringToLangCode(lang);
            if (_lobbyGameDetailsJsons.ContainsKey(langCode))
                return _lobbyGameDetailsJsons[langCode];

            return _lobbyGameDetailsJsons[LangCodes.en_US];
        }
        private string convertLangCodeToString(LangCodes langCode)
        {
            switch(langCode)
            {
                case LangCodes.en_US:
                    return "en-US";
                case LangCodes.zh_CN:
                    return "zh-CN";
                case LangCodes.ja_JP:
                    return "ja-JP";
                case LangCodes.th_TH:
                    return "th-TH";
                case LangCodes.vi_VN:
                    return "vi-VN";
                case LangCodes.ko_KO:
                    return "ko-KO";
                case LangCodes.id_ID:
                    return "id-ID";
            }
            return "en-US";
        }
        private LangCodes convertStringToLangCode(string strLangCode)
        {
            switch(strLangCode)
            {
                case "en-US":
                    return LangCodes.en_US;
                case "zh-CN":
                    return LangCodes.zh_CN;
                case "ja-JP":
                    return LangCodes.ja_JP;
                case "th-TH":
                    return LangCodes.th_TH;
                case "vi-VN":
                    return LangCodes.vi_VN;
                case "ko-KO":
                    return LangCodes.ko_KO;
                case "id-ID":
                    return LangCodes.id_ID;
            }
            return LangCodes.en_US;
        }

    }
    public enum LangCodes
    {
        en_US = 0,
        zh_CN,
        ja_JP,
        th_TH,
        vi_VN,
        ko_KO,
        id_ID,
    }
}
