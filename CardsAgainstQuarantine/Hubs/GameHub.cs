using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CardsAgainstQuarantine.Hubs
{
    public class GameHub : Hub
    {
        public async Task SetupGame(string gameId, string playerId)
        {
            var manager = await Task.Run(GamesManager.GetGamesManager);
            var game = manager.GetGame(gameId);
            if (game == null)
                return; //TODO: Maybe notify the user?

            bool success = Int32.TryParse(playerId, out int id);
            if (!success)
                return; //TODO: Maybe notify the user?
            success = game.Players.TryGetValue(id, out var player);
            if (!success)
                return; //TODO: Maybe notify the user?

            var handIds = new List<int>();
            var handTexts = new List<string>();
            foreach (var card in player.Hand)
            {
                handIds.Add(card.Id);
                handTexts.Add(card.Text);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Caller.SendAsync("ChangeBlack", game.CurrentBlack.Text, game.CurrentBlack.WhiteCardsNeeded);
            bool isCzar = game.CurrentCzar.Id == id;
            await Clients.Caller.SendAsync("SetHand", handIds, handTexts, !isCzar);

            List<string> names = new List<string>();
            List<int> scores = new List<int>();
            foreach(var registeredPlayer in game.Players.Values)
            {
                if(game.CurrentCzar == registeredPlayer)
                    names.Add("* " + registeredPlayer.Name);
                else
                    names.Add(registeredPlayer.Name);
                
                scores.Add(registeredPlayer.Score);
            }
            await Clients.Caller.SendAsync("SetPlayers", names, scores);

            if (game.PlayersPlayed > 0)
            {
                if(game.HasPlayerPlayed(id))
                    await Clients.Caller.SendAsync("AddingSuccessful");
                
                for(int i = 0; i < game.PlayersPlayed; i++)
                    await Clients.Caller.SendAsync("AddCardOnTable", game.CurrentBlack.WhiteCardsNeeded);

                if (game.LastOnTable != null)
                {
                    List<int> cardOnTableIds = new List<int>();
                    List<List<string>> cardOnTableTexts = new List<List<string>>();
                    foreach (var card in game.LastOnTable)
                    {
                        cardOnTableIds.Add(card.Key);
                        List<string> texts = new List<string>();
                        foreach (var text in card.Value)
                        {
                            texts.Add(text.Text);
                        }
                        cardOnTableTexts.Add(texts);
                    
                    }

                    await Clients.Caller.SendAsync("AddInfoToCardsOnTable", game.CurrentCzar.Id,
                        cardOnTableIds, cardOnTableTexts);

                    bool somethingNotRevealed = false;
                    for (int i = 0; i < game.Revealed.Count; i++)
                    {
                        if (game.Revealed[i])
                            await Clients.Caller.SendAsync("RevealCard", game.LastOnTable[i].Key);
                        else
                            somethingNotRevealed = true;
                    }
                    
                    if(!somethingNotRevealed)
                        await Clients.Caller.SendAsync("StartScoring", game.CurrentCzar.Id);
                }
            }
        }
        
        
        public async Task PlayWhiteCards(string gameId, string playerId, IList<int> cardIds)
        {
            var manager = await Task.Run(GamesManager.GetGamesManager);
            var game = manager.GetGame(gameId);
            if (game == null)
                return; //TODO: Maybe notify the user?

            bool success = Int32.TryParse(playerId, out int id);
            if (!success)
                return; //TODO: Maybe notify the user?

            int[] cards = cardIds.ToArray();

            try
            {
                game.PlayCard(id, cards);
            }
            catch (ArgumentException e)
            {
                return; //TODO: Maybe notify the user?
            }

            success = game.Players.TryGetValue(id, out var player);
            if (!success)
                return; //TODO: Maybe notify the user?
            var handIds = new List<int>();
            var handTexts = new List<string>();
            foreach (var card in player.Hand)
            {
                handIds.Add(card.Id);
                handTexts.Add(card.Text);
            }
            await Clients.Caller.SendAsync("SetHand", handIds, handTexts);
            await Clients.Caller.SendAsync("AddingSuccessful");
            await Clients.Group(gameId).SendAsync("AddCardOnTable", cardIds.Count);
            
            if (game.HaveAllPlayersPlayed())
            {
                var cardsOnTable = await Task.Run(game.GetCardsOnTheTable);
                List<int> cardOnTableIds = new List<int>();
                List<List<string>> cardOnTableTexts = new List<List<string>>();
                foreach (var card in cardsOnTable)
                {
                    cardOnTableIds.Add(card.Key);
                    List<string> texts = new List<string>();
                    foreach (var text in card.Value)
                    {
                        texts.Add(text.Text);
                    }
                    cardOnTableTexts.Add(texts);
                    
                }

                await Clients.Group(gameId).SendAsync("AddInfoToCardsOnTable", game.CurrentCzar.Id,
                    cardOnTableIds, cardOnTableTexts);
            }
        }

        public async Task AddPointsToPlayer(string gameId, string czarId, string playerId)
        {
            var manager = await Task.Run(GamesManager.GetGamesManager);
            var game = manager.GetGame(gameId);
            if (game == null)
                return; //TODO: Maybe notify the user?

            bool success = Int32.TryParse(czarId, out int cId);
            if (!success)
                return; //TODO: Maybe notify the user?

            success = Int32.TryParse(playerId, out int pId);
            if (!success)
                return; //TODO: Maybe notify the user?

            if (game.CurrentCzar.Id != cId)
                return; //TODO: Maybe notify the user?

            try
            {
                game.AddPointToPlayer(pId);
                game.DealBlack();
            }
            catch (ArgumentException e)
            {
                return; //TODO: Maybe notify the user?
            }
            
            await Clients.Group(gameId).SendAsync("ChangeBlack", game.CurrentBlack.Text, game.CurrentBlack.WhiteCardsNeeded);
            await Clients.Group(gameId).SendAsync("NewRound", game.CurrentCzar.Id);

            List<string> names = new List<string>();
            List<int> scores = new List<int>();
            foreach(var registeredPlayer in game.Players.Values)
            {
                if(game.CurrentCzar == registeredPlayer)
                    names.Add("* " + registeredPlayer.Name);
                else
                    names.Add(registeredPlayer.Name);
                
                scores.Add(registeredPlayer.Score);
            }
            await Clients.Group(gameId).SendAsync("SetPlayers", names, scores);
        }

        public async Task TurnCardOnTable(string gameId, string czarId, string cardOwnerId)
        {
            var manager = await Task.Run(GamesManager.GetGamesManager);
            var game = manager.GetGame(gameId);
            if (game == null)
                return; //TODO: Maybe notify the user?

            bool success = Int32.TryParse(czarId, out int cId);
            if (!success)
                return; //TODO: Maybe notify the user?

            success = Int32.TryParse(cardOwnerId, out int ownerId);
            if (!success)
                return; //TODO: Maybe notify the user?

            for (int i = 0; i < game.LastOnTable.Count; i++)
            {
                if (game.LastOnTable[i].Key == ownerId)
                    game.Revealed[i] = true;
            }
            
            if(game.CurrentCzar.Id == cId)
                await Clients.Group(gameId).SendAsync("RevealCard", cardOwnerId);
            
            foreach(bool visible in game.Revealed)
            {
                if (!visible)
                    return;
            }
            await Clients.Group(gameId).SendAsync("StartScoring", game.CurrentCzar.Id);
        }

        public async Task SelectCardOnTheTable(string gameId, string czarId, string cardOwnerId)
        {
            var manager = await Task.Run(GamesManager.GetGamesManager);
            var game = manager.GetGame(gameId);
            if (game == null)
                return; //TODO: Maybe notify the user?

            bool success = Int32.TryParse(czarId, out int cId);
            if (!success)
                return; //TODO: Maybe notify the user?

            success = Int32.TryParse(cardOwnerId, out int ownerId);
            if (!success)
                return; //TODO: Maybe notify the user?
            
            if(game.CurrentCzar.Id == cId)
                await Clients.Group(gameId).SendAsync("HighlightCard", cardOwnerId);
        }
    }
}