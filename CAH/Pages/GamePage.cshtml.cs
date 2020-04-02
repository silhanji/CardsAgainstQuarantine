using CAH.GameLogic;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CAH.Pages
{
    public class GamePage : PageModel
    {
        public Game CurrentGame {get; private set; }
        
        public void OnGet()
        {
            var manager = GamesManager.GetGamesManager();
            CurrentGame = manager.GetGame(Request.Cookies["game-id"]);
        }
    }
}