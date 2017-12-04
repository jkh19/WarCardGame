using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarCardGame.Enums;

namespace WarCardGame.Models
{
    public class Card : IComparable<Card>
    {
        private const string CARD_STRING_FORMAT = "{0} of {1}";

        public Suit Suit { get; }
        public Rank Rank { get; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString()
        {
            return string.Format(CARD_STRING_FORMAT, Rank.ToString(), Suit.ToString());
        }

        public int CompareTo(Card other)
        {
            return Rank.CompareTo(other.Rank);
        }
    }
}
