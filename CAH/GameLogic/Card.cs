namespace CAH.GameLogic
{
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

        public static BlackCard ParseFromString(string s, int id)
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
}