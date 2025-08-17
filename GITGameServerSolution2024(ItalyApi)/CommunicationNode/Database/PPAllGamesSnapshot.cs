using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CommNode.Database
{
    class PPAllGamesSnapshot
    {
        private static PPAllGamesSnapshot   _sInstance  = new PPAllGamesSnapshot();
        public static PPAllGamesSnapshot    Instance    => _sInstance;

        public PPAllGamesSnapshot()
        {
            this.LastUpdateTime     = new DateTime(1, 1, 1);
            this.AllGames           = new List<PPGame>();
            this.MiniLobbyGames     = "";
        }
        public string       MiniLobbyGames  { get; set; }
        public DateTime     LastUpdateTime  { get; set; }
        public List<PPGame> AllGames        { get; set; }

        public void generateMiniLobbyGamesSnapshot()
        {
            GameLobbyInfo info = new GameLobbyInfo();
            info.error              = 0;
            info.description        = "OK";
            info.gameLaunchURL      = "/pp/reOpenGame";
            info.gameIconsURL       = "/PPGame/lobby/icons";
            info.lobbyCategories    = new List<GameLobbyCategory>();

            GameLobbyCategory allCategory   = new GameLobbyCategory();
            allCategory.categoryName        = "All games";
            allCategory.categorySymbol      = "all";
            allCategory.lobbyGames          = new List<GameLobbyGameInfo>();

            foreach(PPGame ppGame in this.AllGames)
            {
                GameLobbyGameInfo gameInfo  = new GameLobbyGameInfo();
                gameInfo.name               = ppGame.Name;
                gameInfo.symbol             = ppGame.Symbol;
                gameInfo.hasDeveloed        = ppGame.HasDeveloped ? 1 : 0;

                allCategory.lobbyGames.Add(gameInfo);
            }
            info.lobbyCategories.Add(allCategory);

            GameLobbyCategory newCategory = new GameLobbyCategory();
            newCategory.categoryName    = "New games";
            newCategory.categorySymbol  = "new";
            newCategory.lobbyGames      = new List<GameLobbyGameInfo>();

            foreach (PPGame ppGame in AllGames.FindAll(x => x.IsNew).OrderBy(x => x.NewOrder))
            {
                GameLobbyGameInfo gameInfo  = new GameLobbyGameInfo();
                gameInfo.name               = ppGame.Name;
                gameInfo.symbol             = ppGame.Symbol;
                gameInfo.hasDeveloed        = ppGame.HasDeveloped ? 1 : 0;
                newCategory.lobbyGames.Add(gameInfo);
            }
            info.lobbyCategories.Add(newCategory);

            GameLobbyCategory hotCategory   = new GameLobbyCategory();
            hotCategory.categoryName        = "Hot games";
            hotCategory.categorySymbol      = "hot";
            hotCategory.lobbyGames          = new List<GameLobbyGameInfo>();

            foreach (PPGame ppGame in AllGames.FindAll(x => x.IsHot).OrderBy(x => x.HotOrder))
            {
                GameLobbyGameInfo gameInfo = new GameLobbyGameInfo();
                gameInfo.name           = ppGame.Name;
                gameInfo.symbol         = ppGame.Symbol;
                gameInfo.hasDeveloed    = ppGame.HasDeveloped ? 1 : 0;

                hotCategory.lobbyGames.Add(gameInfo);
            }
            info.lobbyCategories.Add(hotCategory);
            MiniLobbyGames = JsonConvert.SerializeObject(info);
        }
    }

    public class PPGame
    {
        public string Name              { get; set; }
        public string Symbol            { get; set; }
        public bool   HasDeveloped      { get; set; }        
        public bool   IsNew             { get; set; }
        public bool   IsHot             { get; set; }
        public int    NewOrder          { get; set; }
        public int    HotOrder          { get; set; }

        public PPGame(string strSymbol, string strName, bool hasDeveloped, bool isNew, bool isHot, int newOrder, int hotOrder)
        {
            Symbol         = strSymbol;
            Name           = strName;
            HasDeveloped   = hasDeveloped;
            IsNew          = isNew;
            IsHot          = isHot;
            NewOrder       = newOrder;
            HotOrder       = hotOrder;
        }
    }
}
