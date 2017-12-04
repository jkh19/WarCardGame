using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarCardGame.Enums;
using WarCardGame.Interfaces;
using WarCardGame.Models;

namespace WarCardGame.Services
{
    public class DealerService : IDealerService
    {
        private Random _random;

        public DealerService()
        {
            _random = new Random();
        }

        public List<Card> CreateDeck()
        {
            var deck = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    deck.Add(new Card(suit, rank));
                }
            }
            return deck;
        }
        
        public void DealCards(List<Card> deck, List<Player> players)
        {            
            foreach(var player in players)
            {
                player.Deck = new List<Card>();
            }

            var deckIndex = 0;
            var cardsPerPlayer = deck.Count / players.Count;            
            for (int i = 0; i < cardsPerPlayer; i++)
            {
                foreach(var player in players)
                {
                    player.Deck.Add(deck[deckIndex++]);
                }
            }
        }

        /// <summary>
        /// Shuffle deck based on Fisher-Yates algorithm
        /// </summary>
        /// <param name="deck"></param>
        /// <returns></returns>
        public void ShuffleDeck(List<Card> deck)
        {
            for (int i = deck.Count - 1; i > 0; i--)
            {
                var swapLocation = _random.Next(i + 1);
                var temp = deck[swapLocation];
                deck[swapLocation] = deck[i];
                deck[i] = temp;
            }
        }
    }
}
