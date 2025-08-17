using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class SolarKingGameLogic : BasePlaysonGroupGameLogic
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "solar_king";
            }
        }
        protected override int[] StakeIncrement
        {
            get
            {
                return new int[] { 1, 2, 3, 4, 5, 8, 10, 20, 30, 40, 60, 90, 100, 200, 300, 400, 500 };
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "<server><source game-ver=\"280820\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"wild\" count=\"2\" coef=\"2\"/><combination symbol=\"1\" name=\"wild\" count=\"3\" coef=\"20\"/><combination symbol=\"1\" name=\"wild\" count=\"4\" coef=\"50\"/><combination symbol=\"1\" name=\"wild\" count=\"5\" coef=\"200\"/><combination symbol=\"2\" name=\"topstack\" count=\"2\" coef=\"2\"/><combination symbol=\"2\" name=\"topstack\" count=\"3\" coef=\"20\"/><combination symbol=\"2\" name=\"topstack\" count=\"4\" coef=\"50\"/><combination symbol=\"2\" name=\"topstack\" count=\"5\" coef=\"200\"/><combination symbol=\"3\" name=\"top1\" count=\"3\" coef=\"5\"/><combination symbol=\"3\" name=\"top1\" count=\"4\" coef=\"25\"/><combination symbol=\"3\" name=\"top1\" count=\"5\" coef=\"75\"/><combination symbol=\"4\" name=\"top2\" count=\"3\" coef=\"5\"/><combination symbol=\"4\" name=\"top2\" count=\"4\" coef=\"25\"/><combination symbol=\"4\" name=\"top2\" count=\"5\" coef=\"75\"/><combination symbol=\"5\" name=\"med1\" count=\"3\" coef=\"5\"/><combination symbol=\"5\" name=\"med1\" count=\"4\" coef=\"20\"/><combination symbol=\"5\" name=\"med1\" count=\"5\" coef=\"60\"/><combination symbol=\"6\" name=\"med2\" count=\"3\" coef=\"5\"/><combination symbol=\"6\" name=\"med2\" count=\"4\" coef=\"20\"/><combination symbol=\"6\" name=\"med2\" count=\"5\" coef=\"60\"/><combination symbol=\"7\" name=\"med3\" count=\"3\" coef=\"5\"/><combination symbol=\"7\" name=\"med3\" count=\"4\" coef=\"20\"/><combination symbol=\"7\" name=\"med3\" count=\"5\" coef=\"60\"/><combination symbol=\"8\" name=\"med4\" count=\"3\" coef=\"5\"/><combination symbol=\"8\" name=\"med4\" count=\"4\" coef=\"20\"/><combination symbol=\"8\" name=\"med4\" count=\"5\" coef=\"60\"/><combination symbol=\"9\" name=\"low1\" count=\"3\" coef=\"2\"/><combination symbol=\"9\" name=\"low1\" count=\"4\" coef=\"10\"/><combination symbol=\"9\" name=\"low1\" count=\"5\" coef=\"30\"/><combination symbol=\"10\" name=\"low2\" count=\"3\" coef=\"2\"/><combination symbol=\"10\" name=\"low2\" count=\"4\" coef=\"10\"/><combination symbol=\"10\" name=\"low2\" count=\"5\" coef=\"30\"/><combination symbol=\"11\" name=\"low3\" count=\"3\" coef=\"2\"/><combination symbol=\"11\" name=\"low3\" count=\"4\" coef=\"10\"/><combination symbol=\"11\" name=\"low3\" count=\"5\" coef=\"30\"/><combination symbol=\"12\" name=\"low4\" count=\"3\" coef=\"2\"/><combination symbol=\"12\" name=\"low4\" count=\"4\" coef=\"10\"/><combination symbol=\"12\" name=\"low4\" count=\"5\" coef=\"30\"/><combination symbol=\"13\" name=\"low5\" count=\"3\" coef=\"2\"/><combination symbol=\"13\" name=\"low5\" count=\"4\" coef=\"10\"/><combination symbol=\"13\" name=\"low5\" count=\"5\" coef=\"30\"/><combination symbol=\"15\" name=\"scatter\" count=\"3\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"0\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"2,2,2,2,2\"/><payline id=\"3\" path=\"3,3,3,3,3\"/><payline id=\"4\" path=\"4,4,4,4,4\"/><payline id=\"5\" path=\"1,2,3,2,1\"/><payline id=\"6\" path=\"2,3,4,3,2\"/><payline id=\"7\" path=\"3,2,1,2,3\"/><payline id=\"8\" path=\"4,3,2,3,4\"/><payline id=\"9\" path=\"1,2,2,2,1\"/><payline id=\"10\" path=\"2,3,3,3,2\"/><payline id=\"11\" path=\"3,4,4,4,3\"/><payline id=\"12\" path=\"2,1,1,1,2\"/><payline id=\"13\" path=\"3,2,2,2,3\"/><payline id=\"14\" path=\"4,3,3,3,4\"/><payline id=\"15\" path=\"1,1,2,1,1\"/><payline id=\"16\" path=\"2,2,3,2,2\"/><payline id=\"17\" path=\"3,3,4,3,3\"/><payline id=\"18\" path=\"2,2,1,2,2\"/><payline id=\"19\" path=\"3,3,2,3,3\"/><payline id=\"20\" path=\"4,4,3,4,4\"/></paylines><symbols><symbol id=\"1\" title=\"wild\"/><symbol id=\"2\" title=\"topstack\"/><symbol id=\"3\" title=\"top1\"/><symbol id=\"4\" title=\"top2\"/><symbol id=\"5\" title=\"med1\"/><symbol id=\"6\" title=\"med2\"/><symbol id=\"7\" title=\"med3\"/><symbol id=\"8\" title=\"med4\"/><symbol id=\"9\" title=\"low1\"/><symbol id=\"10\" title=\"low2\"/><symbol id=\"11\" title=\"low3\"/><symbol id=\"12\" title=\"low4\"/><symbol id=\"13\" title=\"low5\"/><symbol id=\"14\" title=\"feature\"/><symbol id=\"15\" title=\"scatter\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"14\" symbols=\"4,10,15,13,3,6,5,2,8,12,14,11,7,9\"/><reel id=\"2\" length=\"13\" symbols=\"2,6,7,5,14,10,8,11,13,3,4,9,12\"/><reel id=\"3\" length=\"14\" symbols=\"11,2,7,14,10,15,9,12,3,8,5,6,4,13\"/><reel id=\"4\" length=\"13\" symbols=\"9,8,12,11,5,4,6,10,3,2,14,13,7\"/><reel id=\"5\" length=\"14\" symbols=\"14,9,4,13,11,7,12,2,8,5,10,15,6,3\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"14\" symbols=\"4,13,15,10,3,6,5,2,12,8,9,14,11,7\"/><reel id=\"2\" length=\"13\" symbols=\"2,6,7,5,14,10,8,11,13,3,4,9,12\"/><reel id=\"3\" length=\"14\" symbols=\"11,2,7,14,15,10,9,12,3,8,5,6,13,4\"/><reel id=\"4\" length=\"13\" symbols=\"9,8,12,11,5,4,6,10,3,2,14,13,7\"/><reel id=\"5\" length=\"14\" symbols=\"14,7,9,4,13,11,15,12,2,8,5,10,6,3\"/></reels><reels id=\"3\"><reel id=\"1\" length=\"14\" symbols=\"6,4,5,3,10,15,9,2,14,8,13,12,7,11\"/><reel id=\"2\" length=\"13\" symbols=\"5,11,7,4,12,6,10,13,9,2,3,14,8\"/><reel id=\"3\" length=\"14\" symbols=\"11,9,7,12,15,5,8,14,2,4,10,3,6,13\"/><reel id=\"4\" length=\"13\" symbols=\"5,2,12,13,8,6,10,11,3,7,9,4,14\"/><reel id=\"5\" length=\"14\" symbols=\"5,9,15,11,6,4,10,12,7,13,14,3,8,2\"/></reels><reels id=\"4\"><reel id=\"1\" length=\"14\" symbols=\"4,9,15,10,8,14,3,5,12,7,2,13,6,11\"/><reel id=\"2\" length=\"13\" symbols=\"14,6,5,9,7,13,4,11,8,3,10,12,2\"/><reel id=\"3\" length=\"14\" symbols=\"2,5,7,3,13,11,15,12,9,4,10,6,14,8\"/><reel id=\"4\" length=\"13\" symbols=\"5,10,7,9,4,14,2,8,6,11,3,13,12\"/><reel id=\"5\" length=\"14\" symbols=\"12,4,8,11,15,9,5,2,7,6,14,10,3,13\"/></reels><reels id=\"5\"><reel id=\"1\" length=\"12\" symbols=\"7,3,8,5,12,4,6,13,9,11,2,10\"/><reel id=\"2\" length=\"12\" symbols=\"9,7,8,12,2,6,10,13,4,5,3,11\"/><reel id=\"3\" length=\"12\" symbols=\"12,5,6,11,7,2,9,8,13,4,10,3\"/><reel id=\"4\" length=\"12\" symbols=\"12,2,11,9,5,7,8,6,10,13,3,4\"/><reel id=\"5\" length=\"12\" symbols=\"5,13,3,11,8,9,7,4,6,10,12,2\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"7,7,4,3\" reel2=\"2,2,2,2\" reel3=\"12,15,11,3\" reel4=\"3,2,2,2\" reel5=\"10,10,15,13\"/><shift_ext><shift server=\"0,0,0,0,0\" reel_set=\"4\" reel1=\"7,3,8,5\" reel2=\"9,7,8,12\" reel3=\"12,5,5,12\" reel4=\"12,2,2,2\" reel5=\"5,13,3,11\"/><shift server=\"0,0,0,0,0\" reel_set=\"4\" reel1=\"7,9,4,4\" reel2=\"5,8,5,5\" reel3=\"3,10,5,4\" reel4=\"12,12,3,10\" reel5=\"5,13,3,11\"/><shift server=\"0,0,0,0,0\" reel_set=\"4\" reel1=\"2,2,2,2\" reel2=\"12,13,9,7\" reel3=\"3,10,5,4\" reel4=\"8,8,12,12\" reel5=\"12,10,7,4\"/></shift_ext><game feature_rounds_limit=\"10\"><bets><bet><value>1</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>2</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>3</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>4</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>5</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>8</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>10</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>20</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>30</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>40</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>60</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>90</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>100</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>200</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>300</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>400</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet><bet><value>500</value><progress><feature_rounds_progress>0</feature_rounds_progress><cell_levels/><cell_multiplers/></progress></bet></bets></game><delivery id=\"562657-9041875290785383639263851\" action=\"create\"/></server>";
            }
        }
        #endregion
        public SolarKingGameLogic()
        {
            _gameID = GAMEID.SolarKing;
            GameName = "SolarKing";
        }

        protected override void addLastResultForStart(XmlDocument responseDoc, string strGlobalUserID)
        {
            base.addLastResultForStart(responseDoc, strGlobalUserID);
            XmlElement betsElement = responseDoc.CreateElement("bets");
            
            if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
            {
                BasePlaysonGroupSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID] as BasePlaysonGroupSlotBetInfo;
                foreach(KeyValuePair<float, string> pair in betInfo.SeqResultStrings)
                {
                    XmlDocument lastResult = new XmlDocument();
                    lastResult.LoadXml(pair.Value);

                    XmlNode lastGameNode = lastResult.SelectSingleNode("/server/game");

                    XmlElement betElement       = responseDoc.CreateElement("bet");
                    XmlElement betValueElement  = responseDoc.CreateElement("value");
                    betValueElement.InnerXml    = ((int)pair.Key).ToString();

                    XmlNode featureProgressNode = lastGameNode.SelectSingleNode("feature_rounds_progress");
                    int featureRoundProgress    = Convert.ToInt32(featureProgressNode.InnerXml);
                    featureRoundProgress %= 10;
                    featureProgressNode.InnerXml= featureRoundProgress.ToString();

                    XmlElement progressElement  = responseDoc.CreateElement("progress");
                    foreach(XmlNode xn in lastGameNode.ChildNodes)
                    {
                        XmlNode copiedNode          = responseDoc.ImportNode(xn, true);
                        progressElement.AppendChild(copiedNode);
                    }

                    betElement.AppendChild(betValueElement);
                    betElement.AppendChild(progressElement);
                    betsElement.AppendChild(betElement);
                }

                XmlNode serverGameNode = responseDoc.SelectSingleNode("/server/game");
                serverGameNode.AppendChild(betsElement);
            }
        }
    }
}
