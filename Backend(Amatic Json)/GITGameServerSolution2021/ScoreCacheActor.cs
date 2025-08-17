using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace CommNode
{
    public class ScoreCacheActor : ReceiveActor
    {
        private const int CACHE_SIZE = 90;     //대략적으로 90초정도의 스코로그자료를 가지고 있음
        private Dictionary<string, SortedList<long, SetScoreData>>      _dicCacheData           = new Dictionary<string, SortedList<long, SetScoreData>>();
        private List<List<long>>                                        _listCachedScoreIDs     = new List<List<long>>();
        private Dictionary<long, string>                                _mapScoreIDToUserId     = new Dictionary<long, string>();

        public ScoreCacheActor()
        {
            Receive<List<SetScoreData>>      (onCacheData);
            Receive<GetScoreCacheMessage>    (onGetScoreCache);
        }

        private void onCacheData(List<SetScoreData> scoreDataList)
        {
            if (_listCachedScoreIDs.Count >= CACHE_SIZE)
                removeFirstData();

            List<long> newScoreIds = new List<long>();
            for(int i = 0; i < scoreDataList.Count; i++)
            {
                SetScoreData scoreData = scoreDataList[i];
                newScoreIds.Add(scoreData.ID);

                _mapScoreIDToUserId[scoreData.ID] = scoreData.UserID;

                if (!_dicCacheData.ContainsKey(scoreData.UserID))
                    _dicCacheData.Add(scoreData.UserID, new SortedList<long, SetScoreData>());

                _dicCacheData[scoreData.UserID].Add(scoreData.ID, scoreData);
            }
            _listCachedScoreIDs.Add(newScoreIds);

        }    

        private void onGetScoreCache(GetScoreCacheMessage message)
        {
            string strUserID = message.UserID;

            //캐시자료 없음
            if (!_dicCacheData.ContainsKey(strUserID))
            {
                Sender.Tell(null);
                return;
            }

            SortedList<long, SetScoreData> foundCacheData = new SortedList<long, SetScoreData>(_dicCacheData[strUserID]);
            Sender.Tell(foundCacheData);
        }

        private void removeFirstData()
        {
            foreach(long scoreID in _listCachedScoreIDs[0])
            {
                if (!_mapScoreIDToUserId.ContainsKey(scoreID))
                    continue;

                string strUserID = _mapScoreIDToUserId[scoreID];
                _mapScoreIDToUserId.Remove(scoreID);

                //캐시에서 삭제
                if (!_dicCacheData.ContainsKey(strUserID))
                    continue;

                _dicCacheData[strUserID].Remove(scoreID);
                if (_dicCacheData[strUserID].Count == 0)
                    _dicCacheData.Remove(strUserID);

            }
            _listCachedScoreIDs.RemoveAt(0);
        }
    }

    public class GetScoreCacheMessage
    {
        public string UserID { get; private set; }

        public GetScoreCacheMessage(string strUserID)
        {
            this.UserID = strUserID;
        }
    }    
}
