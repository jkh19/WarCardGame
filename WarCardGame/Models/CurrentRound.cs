using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarCardGame.Models
{
    /// <summary>
    /// A "round" consists of the initial card draw and any subsequent wars
    /// </summary>
    public class CurrentRound
    {
        public List<Player> Players { get; }

        //When cards are put into play for a new round (and subsequent wars), they are added to the pot; winner of round takes all
        public List<Card> CardPot { get; }

        public CurrentRound(List<Player> players) : 
            this(players, new List<Card>())
        {
        }

        public CurrentRound(List<Player> players, List<Card> cardPot)
        {
            Players = players;
            CardPot = cardPot;
        }
    }
}
