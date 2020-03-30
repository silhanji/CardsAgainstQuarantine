using System;
using System.Collections.Generic;
using System.IO;

namespace CAH
{
    public class Game
    {
        public CardDeck<WhiteCard> WhiteCards { get; }
        public CardDeck<BlackCard> BlackCards { get; }
        public Dictionary<int, Player> Players { get; private set; }

        public Game(CardDeck<WhiteCard> whiteCards, CardDeck<BlackCard> blackCards)
        {
            WhiteCards = whiteCards;
            BlackCards = blackCards;
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player.Id, player);
        }
    }

    public abstract class Card
    {
        public int Id { get; }
        public string Text { get;  }

        public Card(int id, string text)
        {
            Id = id;
            Text = text;
        }
    }

    public class WhiteCard : Card
    {
        public WhiteCard(int id, string text) : base(id, text)
        {
            
        }
        
        public static WhiteCard ParseFromString(string s, int id)
        {
            return new WhiteCard(id, s);
        }
    }

    public class BlackCard : Card
    {
        public int WhiteCardsNeeded { get; }
        
        public BlackCard(int id, string text, int whiteCardsNeeded) : base(id, text)
        {
            WhiteCardsNeeded = whiteCardsNeeded;
        }

        public new static BlackCard ParseFromString(string s, int id)
        {
            int whiteCardsNeeded = 0;
            bool wasUnderscore = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '_')
                {
                    if (!wasUnderscore)
                    {
                        whiteCardsNeeded++;
                    }
                    wasUnderscore = true;
                }
                else
                {
                    wasUnderscore = false;
                }
            }

            if (whiteCardsNeeded == 0) whiteCardsNeeded = 1; // At least one white card is always needed
            
            return new BlackCard(id, s, whiteCardsNeeded);
        }
    }

    public class CardDeck<T> where T : Card
    {
        private List<T> _cards;
        private int index;
        
        public CardDeck(IReadOnlyCollection<T> cards)
        {
            if(cards == null || cards.Count == 0)
                throw new ArgumentException("Deck must contain at least one card");
            _cards = new List<T>(cards);
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
                T tmp = _cards[index1];
                _cards[index1] = _cards[index2];
                _cards[index2] = tmp;
            }
        }
        
        public T DrawCard()
        {
            T tmp = _cards[index];
            index++;
            if (_cards.Count == index)
            {
                Shuffle();
                index = 0;
            }

            return tmp;
        }

        public static CardDeck<T> ParseFromFile(string filename, Func<string, int, T> cardParser)
        {
            StreamReader reader = new StreamReader(filename);
            
            List<T> parsedCards = new List<T>();
            string line;
            int id = 0;
            while ((line = reader.ReadLine()) != null)
            {
                parsedCards.Add(cardParser(line, id));
                id++;
            }

            return new CardDeck<T>(parsedCards);
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

        public void DrawCard(CardDeck<WhiteCard> cardDeck)
        {
            if(Hand.Count >= 10)
                throw new InvalidOperationException("Can't draw more than 10 cards");
            
            Hand.Add(cardDeck.DrawCard());
        }
    }
    
}