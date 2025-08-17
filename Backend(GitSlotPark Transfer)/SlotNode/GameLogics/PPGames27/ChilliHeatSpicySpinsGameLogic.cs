using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class ChilliHeatSpicySpinsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10chillihmr";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 10; }
        }
        protected override int ServerResLineCount
        {
            get { return 10; }
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
                return "def_s=eadehcjfifgdgbd&cfgs=1&ver=4&reel_set_size=8&def_sb=bjfgh&def_sa=ehiea&rt=d&gameInfo={rtps:{regular:\"96.58\",purchase:\"96.58\"},props:{max_rnd_win:\"10000\",max_rnd_hr:\"16181230\",max_rnd_sim:\"1\"}}&wl_i=tbm~10000&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&ntp=0.00&paytable=W~3:50,4:200,5:2000;a~2:5,3:50,4:200,5:2000;b~3:30,4:150,5:1000;c~3:20,4:100,5:500;d~3:20,4:100,5:500;e~3:10,4:50,5:200;f~3:5,4:25,5:100;g~3:5,4:25,5:100;h~3:5,4:25,5:100;i~3:5,4:25,5:100;j~3:5,4:25,5:100&total_bet_max=10,000,000.00&reel_set0=iafagfhhehddhfbacjhdigghbachfdbaaafiebedWjahdjighbacdigcicjhfbfhcgdddgeadegcgeaghfbeaefchdfeicbcfadedifb~hjbdgcaadWhcbdaacggcccjgaWfecbejcfgfbecdddhdehebhchfihiedgjeWdbi~aacjgffbcaabddcaccdagdfdejggfbWhWicccaejdbeehhgefieffcgdhhejidhbhiceaebccg~cjcejejihhbWbagdfWfifgebjiefaaaibcbcegccdhddhgfdhfgabcfgaighdddgcdgahaciagbjcddihehfedegifWbfh~cgegiajehgdhheefihehfdgeigdicagefidgfjhfjggehcfeeeggbfhijihagfhdbfghchhfebjhegdeicjedegWgfehicbfif&reel_set2=ihjhceghifgfhWffigdWageiWhhfjfchgdejfihjbjghgeif~dheifhgfiheedfegfeigjhcfgiihhfjgfhigbeWgdeWighgcjfeWaeejfhb~gdhefgedeaeegdacfcijefhehgfgbiWgijcihgbgffahfj~ifgheidhdgjdcgfghhegehjebfhjfecjffgeihgighicgjafef~hWbegchfijeigghhighgbdjfegggfWdghgchjifhfgfeaifhgijejegh&reel_set1=hfiffjbdihhiejhjWigfhijejfigfgifiighdficigbgihchjgghjecjcbhabjgdjefiehcf~eigjifgfjefjfhiWfhcjchghejcigifggeebiihfhijhggdjhiifdejhafeggcf~hcjjagbegfejdifffhigdjfjeaeiigghgggffhiihhjfehgejifd~gbejecgefjcdijihghegfhfgdifgffjbhjfhffahjdhfdjgiibcgibfhgfjcieaWc~gjifbgjhcdjdeiicfehcdghegijgjeejjhhaifgifiecgihhieWfdgghgfefhfjjfjbgbj&reel_set4=ijhdcijghSfbShgdgghieieiSaffgfjfiegfieifidSgebSeaShgjfdhijcbchjhjghejdj~jefhijhfehihjedehfefejbghjjdhggjWgejWdidfchifehfihjijgcfggifgafjgfhii~ggbfdhbgSehafidSjeiihhdggiijWSfjbSeegjhgehdegiihjhjfScchjggSegiWejiegfchfiSf~fijiWgfhfihidiefjgjgfiejdchejbihejfhieahfgjgg~ggjfjfijSgiihiiegidjdSjbSgcejehjjfcfSediefehfghafjSaghSabhSgidheieWcidgefegi&reel_set3=egebiibejhgjehbcjciejighehciihcbhcgecjjbgiggjigc~cdehahfcdaefafdhehhfdafhdcahhecdceadffcec~dibfbffafgdjddbidjfafgjijibigigibdgjdiijidajgbjga~jcjciafiehjcficbhdbgbhdagcdefeg~gfcejdijhcidgbjadhefgcdahjihfdceidghjfbebjfehcib&purInit=[{bet:1000,type:\"default\"}]&reel_set6=fbiSfSedifShijSgdahShjegjhS~jdjhgifgfigghdihjiejdfhgcifgjfcagehWdhehifheWijbjejjh~bfShjegSeciSjghgSgefSiShidhShjdfgSiShcSSgj~ehghiehijdiijfejgegjjfjidiehhfgifegdijfijhjfjgiefihghieheficjffafeiegbgghfjbW~dfeSgShdhciejShfSgShSijiSgS&reel_set5=hgcficgdhgfSadhchichdiSaiaiaSigaSfShidSgSdgfcddghcdfSdfSf~idjbigdbggieibegegdgbbeedjjdgjjiibjjdigejide~eSfcjeefSjaShSeahSaSefbbhabbchSchhjSjffSahejachbffbejajcefecebbjc~cjafbjfdihgehibgdefcjbdgdggjdhicjbadeaabccegiigejiejhcfh~hSibhScgbfgaejecahbSjegdSijefcSfhfdbjegShSecjSdfibcidiaSdgciShfbbijhajgSe&reel_set7=SgSiScfiaShccgddhgfihSiaSidcigagShShdSfaSdiSdccgScfSddSdg~jijejiijddbgjijbggiijdeieegebdgjdgeiggdegijbgbebbdidedjb~ShSjSfecSfebhScfeSajhSjSbfSeSefcaSjSjaSjSbe~gbgiecgjdbdggedahdcjfijgjcidgibdabcciehahjbediefehfehjichahjgicjfb~ihjSejchShfSbSdeSbbjSdfbiSiSidcaSebSgfdSfSacShSihSgS&total_bet_min=200.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100; }
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
        
        public ChilliHeatSpicySpinsGameLogic()
        {
            _gameID     = GAMEID.ChilliHeatSpicySpins;
            GameName    = "ChilliHeatSpicySpins";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{fs:{def_s:\"MMMMMMMMMMMMMMMMMMMMMMMMM\",mo_s:\"S\",mo_v:\"5,10,20,30,40,50,70,100,200,500,1000\",sh:\"5\",st:\"rect\",sw:\"5\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);

            if (dicParams.ContainsKey("pw"))
                dicParams["pw"] = convertWinByBet(dicParams["pw"], currentBet);

        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine  = (float)message.Pop();
                betInfo.LineCount   = (int)message.Pop();
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in ChilliHeatSpicySpinsGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
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
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ChilliHeatSpicySpinsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
