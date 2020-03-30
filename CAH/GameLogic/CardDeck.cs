using System;
using System.Collections.Generic;
using System.IO;

namespace CAH.GameLogic
{
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
}