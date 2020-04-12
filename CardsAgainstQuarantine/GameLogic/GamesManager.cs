using System;
using System.Collections.Generic;
using System.Linq;
using CardsAgainstQuarantine.GameLogic;

namespace CardsAgainstQuarantine
{
    public class GamesManager
    {
        private static GamesManager _gamesManager;
        private CardDeck<WhiteCard> _whiteCards;
        private CardDeck<BlackCard> _blackCards;
        private Dictionary<string, Game> _games;

        private GamesManager()
        {
            _games = new Dictionary<string, Game>();
            _whiteCards = CardDeck<WhiteCard>.ParseFromFile("WhiteCards.txt", WhiteCard.ParseFromString);
            _blackCards = CardDeck<BlackCard>.ParseFromFile("BlackCards.txt", BlackCard.ParseFromString);
        }

        public string CreateNewGame(Player owner)
        {
            string gameId;
            
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            do
                gameId = new string(Enumerable.Repeat(chars, 5)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            while (_games.ContainsKey(gameId));
            
            _games.Add(gameId, new Game(gameId, _whiteCards, _blackCards, owner));
            return gameId;
        }

        public Game GetGame(string id)
        {
            bool success = _games.TryGetValue(id, out var game);
            if (!success)
                game = null;
            return game;
        }

        public void FinishGame(string id)
        {
            _games.Remove(id);
        }

        public static GamesManager GetGamesManager()
        {
            if(_gamesManager == null)
                _gamesManager = new GamesManager();
            return _gamesManager;
        }
        
    }
}