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
    public class BasePlaysonBonusSpinResult : BasePlaysonSlotSpinResult
    {
        public string   BonusTriggerString  { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            BonusTriggerString  = reader.ReadString();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(BonusTriggerString);
        }
    }

    class BasePlaysonBonusSlotGame : BasePlaysonSlotGame
    {
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

                if ((_dicUserResultInfos[strGlobalUserID] as BasePlaysonBonusSpinResult).BonusTriggerString != "")
                {
                    XmlDocument bonusTriggerResult = new XmlDocument();
                    bonusTriggerResult.LoadXml((_dicUserResultInfos[strGlobalUserID] as BasePlaysonBonusSpinResult).BonusTriggerString);
                    XmlNode triggerServerNode = bonusTriggerResult.SelectSingleNode("/server");

                    XmlElement spin_cmdNode = responseDoc.CreateElement("spin_cmd");
                    if (triggerServerNode.Attributes["status"] != null)
                        spin_cmdNode.SetAttribute("status", triggerServerNode.Attributes["status"].Value);

                    foreach (XmlNode xn in triggerServerNode.ChildNodes)
                    {
                        XmlNode copiedNode = responseDoc.ImportNode(xn, true);
                        spin_cmdNode.AppendChild(copiedNode);
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
        }
        
        protected override BasePlaysonSlotSpinResult calculateResult(string strGlobalUserID, BasePlaysonSlotBetInfo betInfo, string strSpinResponse, bool isFirst, PlaysonActionTypes action)
        {
            BasePlaysonSlotSpinResult spinResult = base.calculateResult(strGlobalUserID, betInfo, strSpinResponse, isFirst, action);

            BasePlaysonBonusSpinResult bonusSpinResult = new BasePlaysonBonusSpinResult();
            bonusSpinResult.TotalWin             = spinResult.TotalWin;
            bonusSpinResult.FreeTrigerWin        = spinResult.FreeTrigerWin;
            bonusSpinResult.CurrentAction        = spinResult.CurrentAction;
            bonusSpinResult.NextAction           = spinResult.NextAction;
            bonusSpinResult.ResultString         = spinResult.ResultString;
            bonusSpinResult.RoundID              = spinResult.RoundID;
            bonusSpinResult.TransactionID        = spinResult.TransactionID;
            bonusSpinResult.BonusTriggerString   = "";
            if (bonusSpinResult.CurrentAction == PlaysonActionTypes.BONUS)
            {
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    if (_dicUserResultInfos[strGlobalUserID].CurrentAction == PlaysonActionTypes.BONUS)
                        bonusSpinResult.BonusTriggerString = (_dicUserResultInfos[strGlobalUserID] as BasePlaysonBonusSpinResult).BonusTriggerString;
                    else
                        bonusSpinResult.BonusTriggerString = _dicUserResultInfos[strGlobalUserID].ResultString;
                }
            }

            return bonusSpinResult;
        }

        protected override BasePlaysonSlotSpinResult restoreResultInfo(string strGlobalUserID, BinaryReader reader)
        {
            BasePlaysonBonusSpinResult result = new BasePlaysonBonusSpinResult();
            result.SerializeFrom(reader);
            return result;
        }

        protected override void saveResultToHistory(BasePlaysonSlotSpinResult spinResult,int agentID ,string strUserID, int currency, double balance, double betMoney, double winMoney, PlaysonActionTypes action, string strPlayClient)
        {
            if (!(spinResult.CurrentAction == PlaysonActionTypes.BONUS && spinResult.NextAction == PlaysonActionTypes.BONUS))
                base.saveResultToHistory(spinResult, agentID, strUserID, currency, balance, betMoney, winMoney, action, strPlayClient);
        }
    }
}
