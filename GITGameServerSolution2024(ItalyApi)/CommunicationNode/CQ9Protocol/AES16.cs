using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CQ9Protocol
{
    public class AES16
    {
        protected string _ClientIvStr   = "";
        protected string _ServerIvStr   = "";
        protected string _KeyStr1       = "60A299E7243EDEA7";   //일반암호화키
        protected string _KeyStr2       = "a8Ds56a8wbf594fa";   //릴셋암호화키
        public AES16()
        {
            _ServerIvStr = generateIV();
        }
        public string doGetIVAndDecrypt(string encryptedText, int keytype)
        {
            _ClientIvStr    = encryptedText.Substring(0, 16);
            encryptedText   = encryptedText.Substring(16);
            if (keytype == 0)
                return doDecrypt(encryptedText, _KeyStr1, _ClientIvStr);
            else
                return doDecrypt(encryptedText, _KeyStr2, _ClientIvStr);
        }
        public string doEncryptAndAddIV(string plainText,int keytype)
        {
            if (string.IsNullOrEmpty(_ServerIvStr))
            {
                Console.WriteLine("Server IV value is empty");
                return string.Empty;
            }
            if(keytype == 0)
                return string.Format("{0}{1}", _ServerIvStr, doEncrypt(plainText, _KeyStr1, _ServerIvStr));
            else
                return string.Format("{0}{1}", _ServerIvStr, doEncrypt(plainText, _KeyStr2, _ServerIvStr));
        }
        public string doDecrypt(string textToDecrypt, string keystr, string ivStr)
        {
            try
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize             = 128; //AES128로 사용시 
                aes.FeedbackSize        = 128;
                aes.Mode                = CipherMode.CBC;
                aes.Padding             = PaddingMode.PKCS7;
                aes.Key                 = Encoding.UTF8.GetBytes(keystr);
                aes.IV                  = Encoding.UTF8.GetBytes(ivStr);

                var decrypt     = aes.CreateDecryptor();
                string Output   = null;
                using (var ms = new MemoryStream(Convert.FromBase64String(textToDecrypt)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(ms, decrypt, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            Output = srDecrypt.ReadToEnd();
                        }
                    }
                }
                return Output;
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception has been occured in decrypt: {0}", ex.Message);
                return string.Empty;
            }
        }
        public string doEncrypt(string textToEncrypt, string keystr, string ivStr)
        {
            try
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize             = 128; //AES128로 사용시 
                aes.FeedbackSize        = 128;
                aes.Mode                = CipherMode.CBC;
                aes.Padding             = PaddingMode.PKCS7;
                aes.Key                 = Encoding.UTF8.GetBytes(keystr);
                aes.IV                  = Encoding.UTF8.GetBytes(ivStr);

                var encrypt     = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] buf      = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Encoding.UTF8.GetBytes(textToEncrypt);
                        cs.Write(xXml, 0, xXml.Length);
                    }
                    buf = ms.ToArray();
                }
                string Output = Convert.ToBase64String(buf);
                return Output;
            }
            catch (Exception ex)
            {
                Console.WriteLine("exception has been occured in encrypt: {0}", ex.Message);
                return ex.Message;
            }
        }
        protected string generateIV()
        {
            string initStr      = "0123456789abcdef";
            string resultStr    = "";
            for (int i = 0; i < initStr.Length; i++)
                resultStr = string.Format("{0}{1}", resultStr, initStr[Pcg.Default.Next(16)]);
            return resultStr;
        }
    }
}
