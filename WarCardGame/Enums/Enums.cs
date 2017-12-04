using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarCardGame.Enums
{
    public enum Suit { Clubs, Diamonds, Spades, Hearts};

    public enum Rank { Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace };

    public enum ComparisonResult { Tie, FirstCardWins, SecondCardWins}

    public enum GameMode { Unknown, Auto, Manual }
}
