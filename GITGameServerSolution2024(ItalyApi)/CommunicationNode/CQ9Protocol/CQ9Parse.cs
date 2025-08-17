using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ9Protocol
{
    public class CQ9Parse
    {
        protected string    _frame1         = "~m~";
        protected string    _frame2         = "~j~";
        public AES16        _Secure         = null;
        protected string    _packetString   = "";

        public CQ9Parse()
        {
            this._Secure = new AES16();
        }
        public CQ9RequestPacket parseMessage(string msgStr)
        {
            try
            {
                if (msgStr.Substring(0, 3) == _frame2)
                {
                    if (msgStr.Contains("\"req\":"))
                    {
                        CQ9RequestReqPacket reqPacket = JsonConvert.DeserializeObject<CQ9RequestReqPacket>(msgStr.Substring(3));
                        return reqPacket;
                    }
                    else if (msgStr.Contains("\"irq\":"))
                    {
                        CQ9RequestIrqPacket irqPacket = JsonConvert.DeserializeObject<CQ9RequestIrqPacket>(msgStr.Substring(3));
                        return irqPacket;
                    }
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        public List<string> removeFrame(string strMessage)
        {
            try
            {
                strMessage = strMessage.Replace(" ", "");
                _packetString += strMessage;
                List<string> resultArray = new List<string>();
                do
                {
                    if (_packetString.Length < _frame1.Length)
                        break;

                    if (_packetString.Substring(0, _frame1.Length) != _frame1)
                    {
                        _packetString = "";
                        break;
                    }

                    strMessage = _packetString.Substring(_frame1.Length);
                    if (strMessage.Length == 0)
                        break;

                    string lengthStr = "";
                    for (int i = 0; i < strMessage.Length; i++)
                    {
                        int number;
                        bool success = int.TryParse(strMessage[i] + "", out number);
                        if (success)
                            lengthStr += strMessage[i];
                        else
                            break;
                    }

                    if (lengthStr.Length == 0)
                    {
                        _packetString = "";
                        break;
                    }

                    if (strMessage.Length <= (lengthStr.Length + _frame1.Length))
                        break;

                    strMessage = strMessage.Substring(lengthStr.Length + _frame1.Length);
                    int msgLength = Convert.ToInt32(lengthStr);

                    if (strMessage.Length < msgLength)
                        break;

                    resultArray.Add(strMessage.Substring(0, msgLength));
                    _packetString = strMessage.Substring(msgLength);
                    if (_packetString.Length == 0)
                        break;
                }
                while (true);
                return resultArray;
            }
            catch(Exception)
            {
                return new List<string>();
            }
        }
    }
}
