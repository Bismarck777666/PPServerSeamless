using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using SlotGamesNode.Database;
using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using GITProtocol.Utils;
using PCGSharp;
using System.Xml;

namespace SlotGamesNode.GameLogics
{
    public class BasePlaysonHillSpinResult : BasePlaysonSlotSpinResult
    {
        public int      ProgressHill        { get; set; }
        public string   BonusTriggerString  { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            ProgressHill        = reader.ReadInt32();
            BonusTriggerString  = reader.ReadString();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(ProgressHill);
            writer.Write(BonusTriggerString);
        }
    }

    class BasePlaysonHillSlotGame : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected virtual int[] HillValues
        {
            get
            {
                return new int[] { 1, 3, 6, 8, 8, 10, 12, 15, 21, 21 };
            }
        }
        protected virtual bool ResetHillInBonus
        {
            get
            {
                return false;
            }
        }
        #endregion
        
        protected override void addLastResultForStart(XmlDocument responseDoc, string strGlobalUserID)
        {
            if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                XmlNode serverNode  = responseDoc.SelectSingleNode("/server");
                XmlNode gameNode    = serverNode.SelectSingleNode("game");

                XmlDocument lastResult = new XmlDocument();
                lastResult.LoadXml(_dicUserResultInfos[strGlobalUserID].ResultString);
                XmlNode lastResultServerNode    = lastResult.SelectSingleNode("/server");
                XmlNode lastResultBonusNode     = lastResultServerNode.SelectSingleNode("/server/bonus");
                if(lastResultBonusNode != null)
                {
                    XmlNode copiedNode = responseDoc.ImportNode(lastResultBonusNode, true);
                    gameNode.AppendChild(copiedNode);
                }

                if ((_dicUserResultInfos[strGlobalUserID] as BasePlaysonHillSpinResult).BonusTriggerString != "")
                {
                    XmlDocument bonusTriggerResult = new XmlDocument();
                    bonusTriggerResult.LoadXml((_dicUserResultInfos[strGlobalUserID] as BasePlaysonHillSpinResult).BonusTriggerString);
                    XmlNode triggerServerNode = bonusTriggerResult.SelectSingleNode("/server");

                    XmlElement spin_cmdNode = responseDoc.CreateElement("spin_cmd");
                    if (triggerServerNode.Attributes["status"] != null)
                        spin_cmdNode.SetAttribute("status", triggerServerNode.Attributes["status"].Value);

                    foreach (XmlNode xn in triggerServerNode.ChildNodes)
                    {
                        XmlNode copiedNode = responseDoc.ImportNode(xn, true);
                        spin_cmdNode.AppendChild(copiedNode);
                    }

                    XmlNode triggerBonusFeatureNode = triggerServerNode.SelectSingleNode("/server/game/bonus_items_feature_triggered");
                    if(triggerBonusFeatureNode != null)
                    {
                        XmlNode copiedNode = responseDoc.ImportNode(triggerBonusFeatureNode, true);
                        XmlElement game2Node = responseDoc.CreateElement("game");
                        game2Node.AppendChild(copiedNode);
                        gameNode.AppendChild(game2Node);
                    }

                    XmlElement game_cmdNode = responseDoc.CreateElement("game_cmd");
                    if (lastResultServerNode.Attributes["status"] != null)
                        game_cmdNode.SetAttribute("status", lastResultServerNode.Attributes["status"].Value);

                    foreach (XmlNode xn in lastResultServerNode.ChildNodes)
                    {
                        XmlNode copiedNode = responseDoc.ImportNode(xn, true);
                        game_cmdNode.AppendChild(copiedNode);
                    }

                    serverNode.AppendChild(spin_cmdNode);
                    serverNode.AppendChild(game_cmdNode);
                }
                else
                {
                    XmlElement spin_cmdNode = responseDoc.CreateElement("spin_cmd");
                    if(lastResultServerNode.Attributes["status"] != null)
                        spin_cmdNode.SetAttribute("status", lastResultServerNode.Attributes["status"].Value);

                    foreach (XmlNode xn in lastResultServerNode.ChildNodes)
                    {
                        XmlNode copiedNode = responseDoc.ImportNode(xn, true);
                        spin_cmdNode.AppendChild(copiedNode);
                    }
                    serverNode.AppendChild(spin_cmdNode);
                }

                if (_dicUserResultInfos[strGlobalUserID].NextAction != PlaysonActionTypes.SPIN)
                    ((XmlElement)gameNode).SetAttribute("last_nfs_win", _dicUserResultInfos[strGlobalUserID].FreeTrigerWin.ToString());

                string roundnum = _dicUserResultInfos[strGlobalUserID].RoundID;
                XmlElement roundNumNode = responseDoc.CreateElement("roundnum");
                roundNumNode.SetAttribute("value", roundnum);
                serverNode.AppendChild(roundNumNode);
            }

            if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                int progressHill    = (_dicUserResultInfos[strGlobalUserID] as BasePlaysonHillSpinResult).ProgressHill;
                int progressLevel   = 0;
                int sum             = 0;
                while (sum < progressHill && progressLevel < HillValues.Length - 1)
                {
                    sum += HillValues[progressLevel];
                    progressLevel++;
                }

                XmlDocument lastResult = new XmlDocument();
                lastResult.LoadXml(_dicUserResultInfos[strGlobalUserID].ResultString);
                XmlNode gameNode    = responseDoc.SelectSingleNode("/server/game");
                ((XmlElement)gameNode).SetAttribute("progress_level", progressLevel.ToString());
                
                XmlNode lastResultServerNode    = lastResult.SelectSingleNode("/server");
                XmlNode lastResultGameNode      = lastResultServerNode.SelectSingleNode("game");
                if(lastResultGameNode != null)
                {
                    if (lastResultGameNode.Attributes["is_extra_bonus"] != null)
                        ((XmlElement)gameNode).SetAttribute("is_extra_bonus", lastResultGameNode.Attributes["is_extra_bonus"].Value);
                }
            }
        }
        
        protected override void onDoStart(string strUserID, GITMessage message, double userBalance)
        {
            try
            {
                string strRnd   = (string)message.Pop();
                string strToken = (string)message.Pop();

                XmlDocument responseDoc = new XmlDocument();
                responseDoc.LoadXml(InitDataString);

                XmlElement serverNode = responseDoc["server"];
                if(message.MsgCode == (ushort)CSMSG_CODE.CS_PLAYSON_DORECONNECT)
                    serverNode.SetAttribute("command",  "reconnect");
                else
                    serverNode.SetAttribute("command", "start");
                serverNode.SetAttribute("rnd",      strRnd);
                serverNode.SetAttribute("session",  strToken);
                serverNode.SetAttribute("status",   "ok");

                addLastResultForStart(responseDoc,strUserID);

                XmlElement user_newNode = responseDoc.CreateElement("user_new");
                user_newNode.SetAttribute("cash", ((long)(userBalance * 100)).ToString());

                serverNode.AppendChild(user_newNode);
                responseDoc.AppendChild(serverNode);

                GITMessage responseMessage      = new GITMessage((ushort)SCMSG_CODE.SC_PLAYSON_DOSTART);
                responseMessage.Append(serverNode.OuterXml);
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::onDoStart GameID: {0}, {1}", _gameID, ex);
            }
        }
        
        protected override BasePlaysonSlotSpinResult calculateResult(string strGlobalUserID, BasePlaysonSlotBetInfo betInfo, string strSpinResponse, bool isFirst, PlaysonActionTypes action)
        {
            BasePlaysonSlotSpinResult spinResult = base.calculateResult(strGlobalUserID, betInfo, strSpinResponse, isFirst, action);
            
            BasePlaysonHillSpinResult hillSpinResult = new BasePlaysonHillSpinResult();
            hillSpinResult.TotalWin             = spinResult.TotalWin;
            hillSpinResult.FreeTrigerWin        = spinResult.FreeTrigerWin;
            hillSpinResult.CurrentAction        = spinResult.CurrentAction;
            hillSpinResult.NextAction           = spinResult.NextAction;
            hillSpinResult.ResultString         = spinResult.ResultString;
            hillSpinResult.RoundID              = spinResult.RoundID;
            hillSpinResult.TransactionID        = spinResult.TransactionID;
            hillSpinResult.BonusTriggerString   = "";
            if (hillSpinResult.CurrentAction == PlaysonActionTypes.BONUS)
            {
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    if (_dicUserResultInfos[strGlobalUserID].CurrentAction == PlaysonActionTypes.BONUS)
                        hillSpinResult.BonusTriggerString = (_dicUserResultInfos[strGlobalUserID] as BasePlaysonHillSpinResult).BonusTriggerString;
                    else
                        hillSpinResult.BonusTriggerString = _dicUserResultInfos[strGlobalUserID].ResultString;
                }
            }

            //결과생성후 hill값 변환
            hillSpinResult = changeHillofSpinResult(hillSpinResult, strGlobalUserID, action);
            return hillSpinResult;
        }

        private BasePlaysonHillSpinResult changeHillofSpinResult(BasePlaysonSlotSpinResult spinResult, string strUserID, PlaysonActionTypes action)
        {
            BasePlaysonHillSpinResult hillSpinResult = spinResult as BasePlaysonHillSpinResult;
            int progressHill = 0;
            if (_dicUserResultInfos.ContainsKey(strUserID))
                progressHill = (_dicUserResultInfos[strUserID] as BasePlaysonHillSpinResult).ProgressHill;

            hillSpinResult.ProgressHill = progressHill;
            if (action == PlaysonActionTypes.BONUS)
                return changeProgressLevel(hillSpinResult);

            XmlDocument responseDoc = new XmlDocument();
            responseDoc.LoadXml(hillSpinResult.ResultString);

            XmlNode serverNode          = responseDoc.SelectSingleNode("/server");
            XmlNode gameNode            = serverNode.SelectSingleNode("game");
            XmlNode bonusNode           = gameNode.SelectSingleNode("bonus");
            XmlNode bonusFeatureNode    = gameNode.SelectSingleNode("bonus_items_feature_triggered");

            if (bonusFeatureNode != null)
            {
                hillSpinResult.ProgressHill = 0;
                return changeProgressLevel(hillSpinResult);
            }

            if (bonusNode == null)
            {
                hillSpinResult.ProgressHill++;
                if(ResetHillInBonus)
                    hillSpinResult.ProgressHill = 0;
            }
            return changeProgressLevel(hillSpinResult);
        }

        protected BasePlaysonHillSpinResult changeProgressLevel(BasePlaysonHillSpinResult spinResult)
        {
            int progressHill    = spinResult.ProgressHill;
            int progressLevel   = 0;
            int sum             = 0;
            
            while(sum < progressHill && progressLevel < HillValues.Length - 1)
            {
                sum += HillValues[progressLevel];
                progressLevel++;
            }

            XmlDocument rootDoc = new XmlDocument();
            rootDoc.LoadXml(spinResult.ResultString);

            XmlNode gameNode = rootDoc.SelectSingleNode("/server/game");
            if (gameNode != null && gameNode.Attributes["progress_level"] != null)
                ((XmlElement)gameNode).SetAttribute("progress_level", progressLevel.ToString());

            spinResult.ResultString = rootDoc.InnerXml;
            return spinResult;
        }

        protected override BasePlaysonSlotSpinResult restoreResultInfo(string strGlobalUserID, BinaryReader reader)
        {
            BasePlaysonHillSpinResult result = new BasePlaysonHillSpinResult();
            result.SerializeFrom(reader);
            return result;
        }

        protected override void saveResultToHistory(BasePlaysonSlotSpinResult spinResult,int agentID, string strUserID, int currency, double balance, double betMoney, double winMoney, PlaysonActionTypes action, string strPlayClient)
        {
            if (!(spinResult.CurrentAction == PlaysonActionTypes.BONUS && spinResult.NextAction == PlaysonActionTypes.BONUS))
                base.saveResultToHistory(spinResult, agentID, strUserID, currency, balance, betMoney, winMoney, action, strPlayClient);
        }
    }
}
