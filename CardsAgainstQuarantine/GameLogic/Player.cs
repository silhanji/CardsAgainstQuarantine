using System;
using System.Collections.Generic;

namespace CardsAgainstQuarantine.GameLogic
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
    }
}