using System;
using System.Collections.Generic;

namespace CAH.GameLogic
{
    public class Game
    {
        public CardDeck<WhiteCard> WhiteCards { get; }
        public CardDeck<BlackCard> BlackCards { get; }
        public Dictionary<int, Player> Players { get; private set; }
        
        public bool Started { get; private set; }

        public BlackCard CurrentBlack;

        public Player CurrentCzar;

        public int PlayersPlayed => _cardsOnTheTable.Count;
        
        public List<KeyValuePair<int, Card[]>> LastOnTable { get; private set; }
        public List<bool> Revealed { get; private set; }
        
        private enum GamePhase
        {
            CzarTurn,
            PlayersTurn
        }

        private GamePhase _currentPhase;
        private int _currentCzarIndex;
        private List<int> _playerIds;
        private Dictionary<int, Card[]> _cardsOnTheTable;
        
        public Player Owner { get; private set; }

        public string Id { get; }
        
        public Game(string id, CardDeck<WhiteCard> whiteCards, CardDeck<BlackCard> blackCards, Player owner)
        {
            Id = id;
            WhiteCards = whiteCards;
            WhiteCards.Shuffle();
            BlackCards = blackCards;
            BlackCards.Shuffle();
            Players = new Dictionary<int, Player>();
            Owner = owner;
            AddPlayer(owner);
        }

        public void AddPlayer(Player player)
        {
            if(Started)
                throw new InvalidOperationException("Can not add players to already started game");
            Players.Add(player.Id, player);
        }

        public void Start()
        {
            _currentPhase = GamePhase.PlayersTurn;
            CurrentBlack = BlackCards.DrawCard();
            _cardsOnTheTable = new Dictionary<int, Card[]>();
            _playerIds = new List<int>();
            foreach(var player in Players)
                _playerIds.Add(player.Key);
            _currentCzarIndex = 0;
            CurrentCzar = Players[_playerIds[_currentCzarIndex]];
            foreach (var player in Players.Values)
            {
                for(int i = 0; i < 10; i++)
                    player.Hand.Add(WhiteCards.DrawCard());
            }
        }

        public void PlayCard(int playerId, int[] cardIds)
        {
            if(_currentPhase != GamePhase.PlayersTurn)
                throw new InvalidOperationException("Not allowed during Czar turn");
            
            bool success = Players.TryGetValue(playerId, out var player);
            if(!success)
                throw new ArgumentException("Player with provided ID is not present in the game");

            if(_cardsOnTheTable.ContainsKey(playerId))
                throw new ArgumentException("Player with provided ID already played this turn");
            
            List<int> cardIndices = new List<int>();
            foreach(int cardId in cardIds)
            {
                bool present = false;
                
                for(int i = 0; i < player.Hand.Count; i++)
                    if (player.Hand[i].Id == cardId)
                    {
                        cardIndices.Add(i);
                        present = true;
                        break;
                    }
                if(!present)
                    throw new ArgumentException($"Card with ID {cardId} is not present in players hand");
            }

            Card[] playedCards = new Card[cardIndices.Count];
            for (int i = 0; i < playedCards.Length; i++)
            {
                playedCards[i] = player.Hand[cardIndices[i]];
                player.Hand[cardIndices[i]] = WhiteCards.DrawCard();
            }
            
            _cardsOnTheTable.Add(playerId, playedCards);
        }

        public bool HaveAllPlayersPlayed()
        {
            if(_currentPhase != GamePhase.PlayersTurn)
                throw new InvalidOperationException("Not allowed during Czar turn");
            
            return _cardsOnTheTable.Count == Players.Count - 1;
        }

        public List<KeyValuePair<int, Card[]>> GetCardsOnTheTable()
        {
            if (_currentPhase != GamePhase.PlayersTurn)
                throw new InvalidOperationException("Not allowed during Czar turn");
            
            List<KeyValuePair<int, Card[]>> result = new List<KeyValuePair<int, Card[]>>();
            foreach(var entry in _cardsOnTheTable)
            {
                result.Add(entry);
            }
            
            // Shuffle the entries
            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                int index1 = random.Next(0, result.Count);
                int index2 = random.Next(0, result.Count);
                var tmp = result[index1];
                result[index1] = result[index2];
                result[index2] = tmp;
            }

            _currentPhase = GamePhase.CzarTurn;

            LastOnTable = result;
            Revealed = new List<bool>();
            for(int i = 0; i < result.Count; i++)
                Revealed.Add(false);
            
            return result;
        }

        public void AddPointToPlayer(int playerId)
        {
            if(_currentPhase != GamePhase.CzarTurn)
                throw new InvalidOperationException("Not allowed during players turn");
            
            bool success = Players.TryGetValue(playerId, out var player);
            if(!success)
                throw new ArgumentException("Player with provided ID is not present in the game");
            player.Score++;

            LastOnTable = null;
            Revealed = null;
        }

        public BlackCard DealBlack()
        {
            if(_currentPhase != GamePhase.CzarTurn)
                throw new InvalidOperationException("Not allowed during players turn");

            _cardsOnTheTable.Clear();
            _currentPhase = GamePhase.PlayersTurn;

            _currentCzarIndex++;
            if (_currentCzarIndex == _playerIds.Count)
                _currentCzarIndex = 0;
            CurrentCzar = Players[_playerIds[_currentCzarIndex]];
            
            CurrentBlack = BlackCards.DrawCard();
            return CurrentBlack;
        }

        public bool HasPlayerPlayed(int playerId)
        {
            return _cardsOnTheTable.ContainsKey(playerId);
        }
    }

    

    
    
    
    
}