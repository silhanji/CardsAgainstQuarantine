using System.Collections.Generic;

namespace CAH.GameLogic
{
    public class Game
    {
        public CardDeck<WhiteCard> WhiteCards { get; }
        public CardDeck<BlackCard> BlackCards { get; }
        public Dictionary<int, Player> Players { get; private set; }
        
        public Player Owner { get; private set; }

        public string Id { get; }
        
        public Game(string id, CardDeck<WhiteCard> whiteCards, CardDeck<BlackCard> blackCards, Player owner)
        {
            Id = id;
            WhiteCards = whiteCards;
            BlackCards = blackCards;
            Players = new Dictionary<int, Player>();
            Owner = owner;
            AddPlayer(owner);
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player.Id, player);
        }
    }

    

    
    
    
    
}