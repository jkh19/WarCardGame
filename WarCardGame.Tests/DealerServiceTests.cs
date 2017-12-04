using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarCardGame.Enums;
using WarCardGame.Interfaces;
using WarCardGame.Models;
using WarCardGame.Services;

namespace WarCardGame.Tests
{
    [TestFixture]
    public class DealerServiceTests
    {
        private IDealerService _dealerService;

        [SetUp]
        public void Init()
        {
            _dealerService = new DealerService();
        }

        [Test]
        public void CreateDeck_Create_ShouldContain52Cards()
        {
            var deck = _dealerService.CreateDeck();

            Assert.AreEqual(52, deck.Count);
        }

        [Test]
        public void CreateDeck_Create_ShouldContainAllSuitRankCombinationsOnce()
        {
            var deck = _dealerService.CreateDeck();

            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    Assert.AreEqual(1, deck.Count(c => c.Suit == suit && c.Rank == rank));
                }
            }
        }
        
        [Test]
        public void DealCards_OnePlayer_ShouldGetDealtEntireDeck()
        {
            var deck = CreateSmallDeck();
            var player1 = new Player("Tester");
            var players = new List<Player>() { player1 };

            _dealerService.DealCards(deck, players);

            Assert.AreEqual(deck.Count, player1.Deck.Count);
        }

        [Test]
        public void DealCards_TwoPlayers_ShouldGetDealtDifferentCards()
        {
            var deck = new List<Card>() { new Card(Suit.Clubs, Rank.Ace), new Card(Suit.Clubs, Rank.Nine), new Card(Suit.Spades, Rank.Jack), new Card(Suit.Diamonds, Rank.Two) };
            var player1 = new Player("Tester1");
            var player2 = new Player("Tester2");
            var players = new List<Player>() { player1, player2 };

            _dealerService.DealCards(deck, players);
            
            CollectionAssert.AreEquivalent(deck, player1.Deck.Concat(player2.Deck));
        }

        [Test]
        public void DealCards_OddCardCountWithTwoPlayers_ShouldGetDealtEqualNumberOfCards()
        {
            var deck = new List<Card>() { new Card(Suit.Clubs, Rank.Ace), new Card(Suit.Clubs, Rank.Nine), new Card(Suit.Hearts, Rank.Ten) };
            var player1 = new Player("Tester1");
            var player2 = new Player("Tester2");
            var players = new List<Player>() { player1, player2 };

            _dealerService.DealCards(deck, players);

            Assert.AreEqual(1, player1.Deck.Count);
            Assert.AreEqual(1, player2.Deck.Count);
        }

        [Test]
        public void ShuffleDeck_Shuffle_ShouldKeepAllCardsInDeck()
        {
            var originalDeck = CreateSmallDeck();
            var shuffledDeck = new List<Card>(originalDeck);

            _dealerService.ShuffleDeck(shuffledDeck);

            foreach (var card in originalDeck)
            {
                Assert.AreEqual(originalDeck.Count(c => c.Suit == card.Suit && c.Rank == card.Rank),
                    shuffledDeck.Count(c => c.Suit == card.Suit && c.Rank == card.Rank));
            }
        }

        private List<Card> CreateSmallDeck()
        {
            return new List<Card>() {
                new Card(Suit.Clubs, Rank.Ace), new Card(Suit.Clubs, Rank.Nine), new Card(Suit.Hearts, Rank.Ten),
                new Card(Suit.Spades, Rank.Jack), new Card(Suit.Diamonds, Rank.Two), new Card(Suit.Spades, Rank.Ten) };
        }        
    }
}
