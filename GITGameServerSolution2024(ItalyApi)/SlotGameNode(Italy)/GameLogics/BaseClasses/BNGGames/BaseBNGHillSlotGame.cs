using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlotGamesNode.GameLogics
{
    class BaseBNGHillSlotGame : BaseBNGSlotGame
    {
        #region 게임고유속성값
        protected virtual int[] HillValues
        {
            get
            {
                return new int[] { 3, 7, 10, 10, 10, 10, 10, 10, 10, 1 };
            }
        }
        protected virtual bool ResetHillInBonus
        {
            get
            {
                return true;
            }
        }
        #endregion
        
        protected override BaseBNGSlotSpinResult calculateResult(string strGlobalUserID,int currency, BaseBNGSlotBetInfo betInfo, string strSpinResponse, bool isFirst, BNGActionTypes action)
        {
            BaseBNGSlotSpinResult spinResult = base.calculateResult(strGlobalUserID,currency, betInfo, strSpinResponse, isFirst, action);

            //결과생성후 hill값 변환
            spinResult.ResultString = changeHillofSpinResult(spinResult.ResultString, strGlobalUserID, currency, action);
            return spinResult;
        }

        private string changeHillofSpinResult(string resultString, string strGlobalUserID, int currency, BNGActionTypes action)
        {
            dynamic lastContext = new JObject();
            if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                lastContext = JsonConvert.DeserializeObject<dynamic>(_dicUserResultInfos[strGlobalUserID].ResultString);
            else
                lastContext = JsonConvert.DeserializeObject<dynamic>(convertInitResultString(currency));

            dynamic context     = JsonConvert.DeserializeObject<dynamic>(resultString);
            string  strCurrent  = context["current"];

            int[] lastHillValues = getHillValuesFromContext(lastContext);
            int[] newHillValues  = new int[] { 0, 0 };

            if(strCurrent != "bonus" && action != BNGActionTypes.BONUSSTOP)
            {
                int bsVlaueSum = 0;
                JArray bsValueArray = context[strCurrent]["bs_values"] as JArray;
                for (int i = 0; i < bsValueArray.Count; i++)
                {
                    if (!object.ReferenceEquals(bsValueArray[i], null))
                    {
                        JArray bsValueInArray = bsValueArray[i] as JArray;
                        for (int j = 0; j < bsValueInArray.Count; j++)
                        {
                            int bsValue;
                            if (int.TryParse(bsValueInArray[j].ToString(), out bsValue))
                                bsVlaueSum += bsValue;
                            
                            if (bsVlaueSum > 0) 
                                break;
                        }
                    }
                    if (bsVlaueSum > 0) 
                        break;
                }
                if (bsVlaueSum > 0)
                    newHillValues = increaseHillValues(lastHillValues);
                else
                    newHillValues = lastHillValues;

                context[strCurrent]["hill"][0] = newHillValues[0];
                context[strCurrent]["hill"][1] = newHillValues[1];
            }
            else
            {
                if(!ResetHillInBonus)
                {
                    if (lastHillValues[0] == HillValues.Length - 1)
                        newHillValues = new int[] { 0, 0 };
                    else
                        newHillValues = lastHillValues;

                    context[strCurrent]["hill"][0] = newHillValues[0];
                    context[strCurrent]["hill"][1] = newHillValues[1];
                }
            }
            return JsonConvert.SerializeObject(context);
        }

        private int[] getHillValuesFromContext(dynamic context)
        {
            int[]   hillValues  = new int[] { 0, 0 };
            string  strCurrent  = (string)context["current"];
            dynamic spinContext = context[strCurrent];

            JArray hillArray = spinContext["hill"] as JArray;
            for (int i = 0; i < hillArray.Count; i++)
                int.TryParse(hillArray[i].ToString(), out hillValues[i]);
            
            return hillValues;
        }

        private int[] increaseHillValues(int[] lastHillValues)
        {
            int highHill    = lastHillValues[0];
            int lowHill     = lastHillValues[1];
            int maxHighHill = HillValues.Length - 1;
            if(highHill < maxHighHill || lowHill < HillValues[maxHighHill] - 1)
            {
                lowHill++;
                if (lowHill >= HillValues[highHill])
                {
                    highHill++;
                    lowHill = 0;
                }
            }
            if(highHill >= maxHighHill && lowHill >= HillValues[maxHighHill])
            {
                highHill = maxHighHill;
                lowHill  = HillValues[maxHighHill] - 1;
            }
            return new int[] { highHill, lowHill };
        }
    }
}
