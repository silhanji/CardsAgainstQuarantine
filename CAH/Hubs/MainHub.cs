using System;
using System.Threading.Tasks;
using CAH.GameLogic;
using Microsoft.AspNetCore.SignalR;

namespace CAH.Hubs
{
    public class MainHub : Hub
    {
        public async Task JoinGame(string playerName, string gameId)
        {
            if (string.IsNullOrWhiteSpace(playerName) || string.IsNullOrWhiteSpace(gameId))
                return;
            
            var gamesManager = await Task.Run(GamesManager.GetGamesManager);
            var game = await Task.Run(() => gamesManager.GetGame(gameId));
            if (game == null)
                await Clients.Caller.SendAsync("JoinGameFail");
            else
            {
                var player = new Player(playerName);
                game.AddPlayer(player);
                await Clients.Caller.SendAsync("JoinGame", player.Id,gameId);
            }
        }

        public async Task CreateGame(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
                return;
            
            var gamesManager = await Task.Run(GamesManager.GetGamesManager);
            var player = new Player(playerName);
            string gameId = await Task.Run(() => gamesManager.CreateNewGame(player));
            await Clients.Caller.SendAsync("JoinGame", player.Id, gameId);
        }
    }
}