using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarCardGame.Models;

namespace WarCardGame.Interfaces
{
    public interface IDealerService
    {
        List<Card> CreateDeck();
        void DealCards(List<Card> deck, List<Player> players); 
        void ShuffleDeck(List<Card> deck);
    }
}
