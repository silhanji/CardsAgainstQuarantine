using System;
using System.Collections.Generic;

namespace CAH
{
    public class Game
    {
        public CardDeck WhiteCards { get; }
        public CardDeck BlackCards { get; }
        public Dictionary<int, Player> Players { get; private set; }

        public Game(CardDeck whiteCards, CardDeck blackCards)
        {
            WhiteCards = whiteCards;
            BlackCards = blackCards;
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player.Id, player);
        }
    }

    public class Card
    {
        public int Id { get; }
        public string Text { get;  }
        
        public Card(int id, string text)
        {
            Id = id;
            Text = text;
        }
        
    }

    public class CardDeck
    {
        private List<Card> _cards;
        private int index;
        
        public CardDeck(IReadOnlyCollection<Card> cards)
        {
            if(cards == null || cards.Count == 0)
                throw new ArgumentException("Deck must contain at least one card");
            _cards = new List<Card>(cards);
            index = 0;
        }

        public void Shuffle()
        {
            Random random = new Random();
            int swaps = 1000;
            for (int i = 0; i < swaps; i++)
            {
                int index1 = random.Next(0, _cards.Count);
                int index2 = random.Next(0, _cards.Count);
                Card tmp = _cards[index1];
                _cards[index1] = _cards[index2];
                _cards[index2] = tmp;
            }
        }
        
        public Card DrawCard()
        {
            Card tmp = _cards[index];
            index++;
            if (_cards.Count == index)
            {
                Shuffle();
                index = 0;
            }

            return tmp;
        }
    }
    
    public class Player
    {
        public int Id { get; }
        public string Name { get; }
        public int Score { get; set; }
        public List<Card> Hand { get; }

        public Player(string name)
        {
            Name = name;
            Id = name.GetHashCode(); //TODO: Change to something bullet proof
            Hand = new List<Card>();
        }

        public void DrawCard(CardDeck cardDeck)
        {
            if(Hand.Count >= 10)
                throw new InvalidOperationException("Can't draw more than 10 cards");
            
            Hand.Add(cardDeck.DrawCard());
        }
    }
}