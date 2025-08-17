using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CurrencyConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            string strKRWInitString    = File.ReadAllText("krw.txt");
            string strTargetInitString = File.ReadAllText("target.txt");

            var krwParams       = splitResponseToParams(strKRWInitString);
            var targetParams    = splitResponseToParams(strTargetInitString);

            krwParams["sc"]   = targetParams["sc"];
            krwParams["defc"] = targetParams["defc"];
            if (krwParams.ContainsKey("total_bet_min"))
                krwParams["total_bet_min"] = targetParams["total_bet_min"];

            if (krwParams.ContainsKey("total_bet_max"))
                krwParams["total_bet_max"] = targetParams["total_bet_max"];


            string strResult = convertKeyValuesToString(krwParams);
            File.WriteAllText("result.txt", strResult);
        }

        protected Dictionary<string, string> splitResponseToParams(string strResponse)
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
        protected string convertKeyValuesToString(Dictionary<string, string> keyValues)
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
