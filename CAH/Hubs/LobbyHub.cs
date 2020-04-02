using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CAH.Hubs
{
    public class LobbyHub : Hub
    {
        public async Task StartGame(string gameId, string playerId)
        {
            var manager = await Task.Run(GamesManager.GetGamesManager);
            var game = await Task.Run(() => manager.GetGame(gameId));
            if (game == null)
                return; //TODO: Some error notification

            bool success = Int32.TryParse(playerId, out int id);
            if (!success)
                return;
            
            if (game.Owner.Id != id)
                return; //TODO: Some error notification

            game.Start();
            
            await Clients.Group(gameId).SendAsync("StartGame");
        }

        public async Task PlayerJoined(string gameId, string playerId)
        {
            var manager = await Task.Run(GamesManager.GetGamesManager);
            var game = await Task.Run(() => manager.GetGame(gameId));
            if (game == null)
                return; //TODO: Some error notification

            bool success = Int32.TryParse(playerId, out int id);
            if (!success)
                return; //TODO: Some error notification
            success = game.Players.TryGetValue(id, out var player);
            if (!success)
                return; //TODO: Some error notification

            await Clients.Group(gameId).SendAsync("PlayerJoined", player.Name);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }
    }
}