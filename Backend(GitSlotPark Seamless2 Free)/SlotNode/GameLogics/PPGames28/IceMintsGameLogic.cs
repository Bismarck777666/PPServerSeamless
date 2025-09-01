using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol.Utils;

namespace SlotGamesNode.GameLogics
{
    class IceMintsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20popbottles";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 20; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
        }
        protected override int ROWS
        {
            get
            {
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=94797a55a594986&cfgs=1&ver=4&reel_set_size=6&def_sb=37395&def_sa=35886&rt=d&gameInfo={rtps:{regular:\"96.50\",purchase:\"96.55\"},props:{max_rnd_win:\"10000\",max_rnd_hr:\"4557885\",max_rnd_sim:\"1\"}}&wl_i=tbm~10000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&ntp=0.00&paytable=3~3:40,4:200,5:800;4~3:20,4:100,5:400;5~3:15,4:75,5:300;6~3:10,4:50,5:200;7~3:5,4:25,5:100;8~3:4,4:20,5:80;9~3:3,4:15,5:60;a~3:3,4:12,5:50&total_bet_max=8,000,000.00&reel_set0=989a8a57a5733346S6674857499a444864a666948835559884593968a3976967a3a5793a~49438494a56778b36b499a8777S9666a9997333a8aaa558684447555959bbba7579763a67a8976888~aS9a596667a8897ab8959933375558348677735865a8883774446a673469~64954a3866635b7a9S9a6a444755599a99585584388879a598784777898a3337576S845S69867a637a~364S873845557795947a867687397959365333a6a444895S9a96664a9S43a86&reel_set2=858aa87aS98a74797998579a4aa875559aa333785a9444a4599a7895a99a498789a7796668~a6a8889aa7789875a687737856777668969baa73755599S87aa68333aa97999988759aS99a3799a8985444a6aaa6893a9775a99857666a887899a~a87a987a9a8S68a67b6a633387a7a89995556a399797a779489984834777989444a847aaa939a666779386998aa8499496aS8aa779a~68897a94976aa8b75687877bbba733394888a9659aaSa757444aa73S8898a5a555986a99998a79a766667399a945a47776S7Sb489989aaa~3a3675S7998876a797a9a44486555769957a35aa666974Sa889899987a5864476333488a9a897a9a9&reel_set1=55a7879796aa4988854aS3444a4a76495333a669aaa48688555778766693988aa~389a3S6586847ab3aS6663979686555aa94aa7778a6657aaa748885444733368aa49949854787~888S8aba73a683a785477bbb9555aaa6a333957588777998a485444963897a65867597S68a9a666846ba95a3a7999~6949a9893aabaa6784867668883a6883Sa789735Sa88555a4448894a3339S947556784574a5a7696476663777~a3a9574689a895a9693Sa869333a44489897843565447aaa4a88a8666S637435877a67a487586595553476a976aSaaS&reel_set4=44886a568864a6679587aa48a4a7a66aa68333a854a67555a77739668886a66696a4a44488a647aa948a3aaa984854a54988a66846788968a8aa6S654Sa999~4b9998b8669baS7baa6a86686884689a96555b6aa8948b8aaa44abbb8a777bb4446333ab48a4957b7888b4a8a7465a666458668ba7b5bba4aSa687a66aa35a4ab6b858babb84b89664a93babS~8ab76638878a598564S868a4aa6b3bb9a4S8aa5b94b473335aa4aS898ab48bbb7a4448aa666886a955576aaa88b46a999766aa66a6a69657774aa4a8b888bb4646a4S~a8387a464ab96555aa65a8497776a679b8a644bbba9a86a48S86aaa8877444ba3339Saa8aS6aa8a746854676888564a9996a886664686b84954a88~88a59aa65995a8a97a6a844aa3334634799988784446aaa68a64a6679555766a8666S4a88a648S88466a6a44aS&reel_set3=59588S754a9aa33389998555778a9a898a9a89aaaa9779444a966697a4577~9a76579768689555883444a939333b9a96S9aaaa878978579aaS9a777a5a7a886668a7a73a9~6a48993a9a93798888479777aa8974448487a9S999b333787aa77948555a786aaaa9aa88S6989a7666693a~777889a7a49967a6669b7859a999Sa97a78aa7S59433388585679aaaa8345558444a77976S9a896a46a9a3~8583a847839aa976547a99679588889S6S8a88699a8666444794957969698a989aaa5a3337789aa8Saa7349555aS478a6a7a7a77&purInit=[{bet:1600,type:\"default\"}]&reel_set5=938637a8a485974333977944468889a5939a877555a597775a77659357759399aa999498759aaa773989a6668S5795~95888b55aab79b7893339b7Sa4448b87b374a588bbb99975babb555a6b9b59b99bb97b969a3a777S86667b38S7b77aaa98977b5974698~5a94583b9a9679759S3a79aaa7S849333599777b9S98444756858b7999779bbb7b555a5b36668b75a9737888~aaab365a9a67S9996a75b956668S578977943955578b8997a9S4977778bbbS9333a77483744439789a3a95a99795aS7788898b9585997757b~798aS9399568a889S9539977957537559333999744497a66677895974959a473a7755568998S773355a&total_bet_min=200.00";
            }
        }

        protected override double PurchaseFreeMultiple
        {
            get { return 80; }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }


        #endregion
        public IceMintsGameLogic()
        {
            _gameID = GAMEID.IceMints;
            GameName = "IceMints";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter,string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
	
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);
                
                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);

                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in IceMintsGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }

                if (!isNotIntergerMultipleBetPerLine(betInfo.BetPerLine, minChip))
                {
                    _logger.Error("{0} betInfo.BetPerLine is illegual: {1} != {2} * integer", strGlobalUserID, betInfo.BetPerLine, minChip);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1} != {2}", strGlobalUserID, betInfo.LineCount, this.ClientReqLineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in IceMintsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
