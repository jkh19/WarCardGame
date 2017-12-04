using Moq;
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
    public class GameRoundTests
    {
        private Mock<IPlayerService> _mockPlayerService;
        private IGameRoundService _gameRoundService;

        [SetUp]
        public void Init()
        {
            _mockPlayerService = new Mock<IPlayerService>(MockBehavior.Strict);
            _gameRoundService = new GameRoundService(_mockPlayerService.Object);
        }

        [Test]
        public void AddCardFromAllPlayersToPot_TwoPlayers_ShouldRemoveTopCardFromBothPlayers()
        {
            var player1 = new Player("Tester1") { Deck = new List<Card>() { new Card(Suit.Clubs, Rank.Eight) } };
            var player2 = new Player("Tester2") { Deck = new List<Card>() { new Card(Suit.Diamonds, Rank.Five) } };
            _mockPlayerService.Setup(mps => mps.RemoveTopCardForPlayer(player1)).Returns(player1.Deck.First());
            _mockPlayerService.Setup(mps => mps.RemoveTopCardForPlayer(player2)).Returns(player2.Deck.First());
            var currentRound = new CurrentRound(new List<Player>() { player1, player2 });

            _gameRoundService.AddCardFromAllPlayersToPot(currentRound);

            _mockPlayerService.Verify(mps => mps.RemoveTopCardForPlayer(player1), Times.Once);
            _mockPlayerService.Verify(mps => mps.RemoveTopCardForPlayer(player2), Times.Once);
        }

        [Test]
        public void AddCardFromAllPlayersToPot_TwoPlayers_ShouldAddTopCardsToPot()
        {
            var player1TopCard = new Card(Suit.Clubs, Rank.Eight);
            var player2TopCard = new Card(Suit.Diamonds, Rank.Five);
            var player1 = new Player("Tester1") { Deck = new List<Card>() { player1TopCard } };
            var player2 = new Player("Tester2") { Deck = new List<Card>() { player2TopCard } };
            _mockPlayerService.Setup(mps => mps.RemoveTopCardForPlayer(player1)).Returns(player1.Deck.First());
            _mockPlayerService.Setup(mps => mps.RemoveTopCardForPlayer(player2)).Returns(player2.Deck.First());
            var currentRound = new CurrentRound(new List<Player>() { player1, player2 });

            _gameRoundService.AddCardFromAllPlayersToPot(currentRound);

            Assert.AreEqual(2, currentRound.CardPot.Count);
            Assert.Contains(player1TopCard, currentRound.CardPot);
            Assert.Contains(player2TopCard, currentRound.CardPot);
        }

        [Test]
        public void GetCardDrawResult_TwoPlayerCards_ShouldPopulatePlayerCardsInResult()
        {
            var player1Card = new Card(Suit.Clubs, Rank.Eight);
            var player2Card = new Card(Suit.Diamonds, Rank.King);
            var currentRound = new CurrentRound(new List<Player>() { new Player("1"), new Player("2") });
            currentRound.CardPot.Add(player1Card);
            currentRound.CardPot.Add(player2Card);

            var result = _gameRoundService.GetCardDrawResult(currentRound);

            Assert.AreEqual(player1Card, result.FirstPlayerCard);
            Assert.AreEqual(player2Card, result.SecondPlayerCard);
        }

        [Test]
        public void GetCardDrawResult_TwoNumericCardsWithHigherFirstCard_ShouldGetFirstCardWinsResult()
        {
            var player1Card = new Card(Suit.Clubs, Rank.Eight);
            var player2Card = new Card(Suit.Clubs, Rank.Five);
            var currentRound = new CurrentRound(new List<Player>() { new Player("1"), new Player("2") });
            currentRound.CardPot.Add(player1Card);
            currentRound.CardPot.Add(player2Card);

            var result = _gameRoundService.GetCardDrawResult(currentRound);

            Assert.AreEqual(ComparisonResult.FirstCardWins, result.ComparisonResult);
        }

        [Test]
        public void GetCardDrawResult_TwoFaceCardsWithHigherFirstCard_ShouldGetFirstCardWinsResult()
        {
            var player1Card = new Card(Suit.Clubs, Rank.King);
            var player2Card = new Card(Suit.Clubs, Rank.Jack);
            var currentRound = new CurrentRound(new List<Player>() { new Player("1"), new Player("2") });
            currentRound.CardPot.Add(player1Card);
            currentRound.CardPot.Add(player2Card);

            var result = _gameRoundService.GetCardDrawResult(currentRound);

            Assert.AreEqual(ComparisonResult.FirstCardWins, result.ComparisonResult);
        }

        [Test]
        public void GetCardDrawResult_AceAndFaceCardWithHigherFirstCard_ShouldGetFirstCardWinsResult()
        {
            var player1Card = new Card(Suit.Clubs, Rank.Ace);
            var player2Card = new Card(Suit.Clubs, Rank.King);
            var currentRound = new CurrentRound(new List<Player>() { new Player("1"), new Player("2") });
            currentRound.CardPot.Add(player1Card);
            currentRound.CardPot.Add(player2Card);

            var result = _gameRoundService.GetCardDrawResult(currentRound);

            Assert.AreEqual(ComparisonResult.FirstCardWins, result.ComparisonResult);
        }

        [Test]
        public void GetCardDrawResult_TwoNumericCardsDifferentSuitsWithHigherFirstCard_ShouldGetFirstCardWinsResult()
        {
            var player1Card = new Card(Suit.Clubs, Rank.Seven);
            var player2Card = new Card(Suit.Spades, Rank.Four);
            var currentRound = new CurrentRound(new List<Player>() { new Player("1"), new Player("2") });
            currentRound.CardPot.Add(player1Card);
            currentRound.CardPot.Add(player2Card);

            var result = _gameRoundService.GetCardDrawResult(currentRound);

            Assert.AreEqual(ComparisonResult.FirstCardWins, result.ComparisonResult);
        }

        [Test]
        public void GetCardDrawResult_TwoNumericCardsWithHigherSecondCard_ShouldGetSecondCardWinsResult()
        {
            var player1Card = new Card(Suit.Clubs, Rank.Two);
            var player2Card = new Card(Suit.Clubs, Rank.Three);
            var currentRound = new CurrentRound(new List<Player>() { new Player("1"), new Player("2") });
            currentRound.CardPot.Add(player1Card);
            currentRound.CardPot.Add(player2Card);

            var result = _gameRoundService.GetCardDrawResult(currentRound);

            Assert.AreEqual(ComparisonResult.SecondCardWins, result.ComparisonResult);
        }

        [Test]
        public void GetCardDrawResult_TwoFaceCardsWithHigherSecondCard_ShouldGetSecondCardWinsResult()
        {
            var player1Card = new Card(Suit.Clubs, Rank.Jack);
            var player2Card = new Card(Suit.Clubs, Rank.Queen);
            var currentRound = new CurrentRound(new List<Player>() { new Player("1"), new Player("2") });
            currentRound.CardPot.Add(player1Card);
            currentRound.CardPot.Add(player2Card);

            var result = _gameRoundService.GetCardDrawResult(currentRound);

            Assert.AreEqual(ComparisonResult.SecondCardWins, result.ComparisonResult);
        }

        [Test]
        public void GetCardDrawResult_FaceCardAndAceWithHigherSecondCard_ShouldGetSecondCardWinsResult()
        {
            var player1Card = new Card(Suit.Clubs, Rank.Queen);
            var player2Card = new Card(Suit.Clubs, Rank.Ace);
            var currentRound = new CurrentRound(new List<Player>() { new Player("1"), new Player("2") });
            currentRound.CardPot.Add(player1Card);
            currentRound.CardPot.Add(player2Card);

            var result = _gameRoundService.GetCardDrawResult(currentRound);

            Assert.AreEqual(ComparisonResult.SecondCardWins, result.ComparisonResult);
        }

        [Test]
        public void GetCardDrawResult_TwoNumericCardsDifferentSuitsWithHigherSecondCard_ShouldGetSecondCardWinsResult()
        {
            var player1Card = new Card(Suit.Clubs, Rank.Five);
            var player2Card = new Card(Suit.Spades, Rank.Nine);
            var currentRound = new CurrentRound(new List<Player>() { new Player("1"), new Player("2") });
            currentRound.CardPot.Add(player1Card);
            currentRound.CardPot.Add(player2Card);

            var result = _gameRoundService.GetCardDrawResult(currentRound);

            Assert.AreEqual(ComparisonResult.SecondCardWins, result.ComparisonResult);
        }

        [Test]
        public void GetCardDrawResult_SameNumericValue_ShouldGetTiedResult()
        {
            var player1Card = new Card(Suit.Clubs, Rank.Five);
            var player2Card = new Card(Suit.Spades, Rank.Five);
            var currentRound = new CurrentRound(new List<Player>() { new Player("1"), new Player("2") });
            currentRound.CardPot.Add(player1Card);
            currentRound.CardPot.Add(player2Card);

            var result = _gameRoundService.GetCardDrawResult(currentRound);

            Assert.AreEqual(ComparisonResult.Tie, result.ComparisonResult);
        }

        [Test]
        public void GetCardDrawResult_SameFaceCardValue_ShouldGetTiedResult()
        {
            var player1Card = new Card(Suit.Diamonds, Rank.Queen);
            var player2Card = new Card(Suit.Hearts, Rank.Queen);
            var currentRound = new CurrentRound(new List<Player>() { new Player("1"), new Player("2") });
            currentRound.CardPot.Add(player1Card);
            currentRound.CardPot.Add(player2Card);

            var result = _gameRoundService.GetCardDrawResult(currentRound);

            Assert.AreEqual(ComparisonResult.Tie, result.ComparisonResult);
        }

        [Test]
        public void GetCardDrawResult_TwoAces_ShouldGetTiedResult()
        {
            var player1Card = new Card(Suit.Spades, Rank.Ace);
            var player2Card = new Card(Suit.Hearts, Rank.Ace);
            var currentRound = new CurrentRound(new List<Player>() { new Player("1"), new Player("2") });
            currentRound.CardPot.Add(player1Card);
            currentRound.CardPot.Add(player2Card);

            var result = _gameRoundService.GetCardDrawResult(currentRound);

            Assert.AreEqual(ComparisonResult.Tie, result.ComparisonResult);
        }

        [Test]
        public void GetResultIfNotEnoughCardsToContinue_BothPlayersHaveCards_ShouldReturnNull()
        {            
            _mockPlayerService.Setup(mps => mps.IsPlayerOutOfCards(It.IsAny<Player>())).Returns(false);
            var currentRound = new CurrentRound(new List<Player>() { new Player("Tester1"), new Player("Tester2") });

            var result = _gameRoundService.GetResultIfNotEnoughCardsToContinue(currentRound);

            Assert.IsNull(result);
        }

        [Test]
        public void GetResultIfNotEnoughCardsToContinue_NeitherPlayersHaveCards_ShouldReturnResultWithNoCardsAllPlayersMessage()
        {
            _mockPlayerService.Setup(mps => mps.IsPlayerOutOfCards(It.IsAny<Player>())).Returns(true);
            var currentRound = new CurrentRound(new List<Player>() { new Player("Tester1"), new Player("Tester2") });

            var result = _gameRoundService.GetResultIfNotEnoughCardsToContinue(currentRound);
            
            Assert.AreEqual(result.Message, "All players ran out of cards.");
            Assert.IsNull(result.FirstPlayerCard);
            Assert.IsNull(result.SecondPlayerCard);
            Assert.IsFalse(result.IsWarRequired);            
        }

        [Test]
        public void GetResultIfNotEnoughCardsToContinue_FirstPlayersHasNoCards_ShouldReturnResultWithFirstPlayerNameInMessage()
        {
            var player1 = new Player("Tester1");
            var player2 = new Player("Tester2");
            _mockPlayerService.Setup(mps => mps.IsPlayerOutOfCards(player1)).Returns(true);
            _mockPlayerService.Setup(mps => mps.IsPlayerOutOfCards(player2)).Returns(false);
            var currentRound = new CurrentRound(new List<Player>() { player1, player2 });

            var result = _gameRoundService.GetResultIfNotEnoughCardsToContinue(currentRound);

            Assert.AreEqual(result.Message, "Tester1 ran out of cards.");
            Assert.IsNull(result.FirstPlayerCard);
            Assert.IsNull(result.SecondPlayerCard);
            Assert.IsFalse(result.IsWarRequired);
        }

        [Test]
        public void GetResultIfNotEnoughCardsToContinue_SecondPlayersHasNoCards_ShouldReturnResultWithSecondPlayerNameInMessage()
        {
            var player1 = new Player("Tester1");
            var player2 = new Player("Tester2");
            _mockPlayerService.Setup(mps => mps.IsPlayerOutOfCards(player1)).Returns(false);
            _mockPlayerService.Setup(mps => mps.IsPlayerOutOfCards(player2)).Returns(true);
            var currentRound = new CurrentRound(new List<Player>() { player1, player2 });

            var result = _gameRoundService.GetResultIfNotEnoughCardsToContinue(currentRound);

            Assert.AreEqual(result.Message, "Tester2 ran out of cards.");
            Assert.IsNull(result.FirstPlayerCard);
            Assert.IsNull(result.SecondPlayerCard);
            Assert.IsFalse(result.IsWarRequired);
        }

        [Test]
        public void GetRoundWinnings_WithCards_ShouldReturnCardPot()
        {
            var cards = new List<Card>() { new Card(Suit.Clubs, Rank.Eight), new Card(Suit.Diamonds, Rank.Jack) };
            var currentRound = new CurrentRound(new List<Player>(), cards);

            var result = _gameRoundService.GetRoundWinnings(currentRound);

            CollectionAssert.AreEqual(cards, result);
        }
    }
}
