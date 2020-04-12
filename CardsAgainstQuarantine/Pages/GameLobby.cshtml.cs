using System;
using CardsAgainstQuarantine.GameLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CardsAgainstQuarantine.Pages
{
    public class GameLobby : PageModel
    {
        public Game NewGame { get; private set; }
        
        public bool IsCreator { get; private set; }

        public void OnGet()
        {
            var manager = GamesManager.GetGamesManager();
            NewGame = manager.GetGame(Request.Cookies["game-id"]);

            var playerId = Request.Cookies["player-id"];
            Int32.TryParse(playerId, out var id);
            IsCreator = id == NewGame.Owner.Id ? true : false;
        }
    }
}