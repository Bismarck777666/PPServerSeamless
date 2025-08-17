using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlotGamesNode.GameLogics
{
    class BaseBNGAchieveGame : BaseBNGSlotGame
    {
        protected override BaseBNGSlotSpinResult calculateResult(string strGlobalUserID,int currency, BaseBNGSlotBetInfo betInfo, string strSpinResponse, bool isFirst, BNGActionTypes action)
        {
            BaseBNGSlotSpinResult spinResult = base.calculateResult(strGlobalUserID,currency, betInfo, strSpinResponse, isFirst, action);

            //결과생성후 hill값 변환
            spinResult.ResultString = changeAchievment(spinResult.ResultString, strGlobalUserID, currency, action);
            return spinResult;
        }
        
        private string changeAchievment(string resultString, string strGlobalUserID,int currency, BNGActionTypes action)
        {
            dynamic lastContext = new JObject();
            if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                lastContext = JsonConvert.DeserializeObject<dynamic>(_dicUserResultInfos[strGlobalUserID].ResultString);
            else
                lastContext = JsonConvert.DeserializeObject<dynamic>(convertInitResultString(currency));

            dynamic context     = JsonConvert.DeserializeObject<dynamic>(resultString);
            string strCurrent   = context["current"];

            BNGAchievement lastAchievment = null;
            if (object.ReferenceEquals(lastContext["achievements"], null))
                lastAchievment = new BNGAchievement();
            else
                lastAchievment = JsonConvert.DeserializeObject<BNGAchievement>(JsonConvert.SerializeObject(lastContext["achievements"]));
            
            if (strCurrent != "bonus" && action != BNGActionTypes.BONUSSTOP)
            {
                int bsCount = (int) context[strCurrent]["bs_count"];

                int[] levelCounts = new int[] { 20, 25, 40, 45 };

                lastAchievment.number += bsCount;
                int     sum     = 0;
                int     lastSum = 0;
                bool    isFound = false;
                for(int i = 0; i < levelCounts.Length; i++)
                {
                    lastSum = sum;
                    sum     += levelCounts[i];
                    if(sum > lastAchievment.number)
                    {
                        isFound = true;
                        lastAchievment.level = i;
                        lastAchievment.level_percent = Math.Round((decimal)(lastAchievment.number - lastSum) / (levelCounts[i]), 2);
                        lastAchievment.total_percent = Math.Round((decimal)lastAchievment.number / 130.0M, 3);

                        if (lastAchievment.level_percent > 1.0M)
                            lastAchievment.level_percent = 1.0M;

                        if (lastAchievment.total_percent > 1.0M)
                            lastAchievment.total_percent = 1.0M;
                        break;
                    }
                }
                if(!isFound)
                {
                    lastAchievment.level            = 4;
                    lastAchievment.total_percent    = 1.0M;
                    lastAchievment.level_percent    = 1.0M;
                }
                context["achievements"] = JObject.Parse(JsonConvert.SerializeObject(lastAchievment));
            }
            else
            {
                lastAchievment.level            = 0;
                lastAchievment.number           = 0;
                lastAchievment.level_percent    = 0.0M;
                lastAchievment.total_percent    = 0.0M;
                context["achievements"] = JObject.Parse(JsonConvert.SerializeObject(lastAchievment));
            }
            return JsonConvert.SerializeObject(context);
        }
    }
    class BNGAchievement
    {
        public int      level               { get; set; }
        public decimal  level_percent       { get; set; }
        public int      number              { get; set; }
        public decimal  total_percent       { get; set; }
    }
}
