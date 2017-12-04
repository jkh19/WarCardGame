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
    public class PlayerServiceTests
    {
        private IPlayerService _playerService;

        [SetUp]
        public void Init()
        {
            _playerService = new PlayerService();
        }

        [Test]
        public void CreatePlayers_TwoPlayers_ShouldReturnTwoPlayers()
        {
            var player1Name = "Tester1";
            var player2Name = "Tester2";

            var createdPlayers = _playerService.CreatePlayers(new List<string>() { player1Name, player2Name });

            Assert.AreEqual(2, createdPlayers.Count);
            Assert.AreEqual(1, createdPlayers.Count(p => p.Name == player1Name));
            Assert.AreEqual(1, createdPlayers.Count(p => p.Name == player2Name));
        }

        [Test]
        public void IsPlayerOutOfCards_PlayerWithEmptyDeck_ShouldReturnTrue()
        {
            var player = new Player("Tester") { Deck = new List<Card>() };

            var result = _playerService.IsPlayerOutOfCards(player);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsPlayerOutOfCards_PlayerWithCards_ShouldReturnFalse()
        {
            var player = new Player("Tester") { Deck = new List<Card>() { new Card(Suit.Clubs, Rank.Ace) } };

            var result = _playerService.IsPlayerOutOfCards(player);

            Assert.IsFalse(result);
        }

        [Test]
        public void RemoveTopCardForPlayer_PlayerWithSingleCard_ShouldReturnSingleCard()
        {
            var deck = new List<Card>() { new Card(Suit.Clubs, Rank.Ace) };            
            var player = new Player("Tester") { Deck = deck };
            var topCard = deck.First();

            var result = _playerService.RemoveTopCardForPlayer(player);

            Assert.AreEqual(topCard, result);
        }

        [Test]
        public void RemoveTopCardForPlayer_PlayerWithSingleCard_ShouldHaveEmptyDeckAfterReturning()
        {
            var deck = new List<Card>() { new Card(Suit.Clubs, Rank.Ace) };
            var player = new Player("Tester") { Deck = deck };
            var topCard = deck.First();

            var result = _playerService.RemoveTopCardForPlayer(player);

            Assert.IsEmpty(player.Deck);
        }

        [Test]
        public void RemoveTopCardForPlayer_PlayerWithMultipleCards_ShouldReturnFirstCardInList()
        {
            var deck = new List<Card>() { new Card(Suit.Clubs, Rank.Ace), new Card(Suit.Diamonds, Rank.Ten), new Card(Suit.Spades, Rank.King) };
            var player = new Player("Tester") { Deck = deck };
            var topCard = deck.First();

            var result = _playerService.RemoveTopCardForPlayer(player);

            Assert.AreEqual(topCard, result);
        }
    }
}
