using System;
using System.Collections.Generic;

namespace CAH.GameLogic
{
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