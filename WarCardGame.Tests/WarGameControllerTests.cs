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
    public class WarGameControllerTests
    {
        private Mock<IDealerService> _mockDealerService;
        private Mock<IPlayerService> _mockPlayerService;
        private Mock<IGameRoundService> _mockGameRoundService;
        private IWarGameController _warGameController;

        [SetUp]
        public void Init()
        {
            _mockDealerService = new Mock<IDealerService>(MockBehavior.Strict);
            _mockPlayerService = new Mock<IPlayerService>(MockBehavior.Strict);
            _mockGameRoundService = new Mock<IGameRoundService>(MockBehavior.Strict);
            _warGameController = new WarGameController(_mockDealerService.Object, _mockPlayerService.Object, _mockGameRoundService.Object);
        }

        [Test]
        public void ContinueRoundWithWar_NotEnoughCardsToPlay_ShouldReturnResultWithoutAddingToPot()
        {
            var players = InitializeTwoPlayerGame();
            var expectedResult = new SingleCardPlayResult(null, null, false, "Test");
            _mockGameRoundService.Setup(mgrs => mgrs.GetResultIfNotEnoughCardsToContinue(It.IsAny<CurrentRound>())).Returns(expectedResult);
            _mockGameRoundService.Setup(mgrs => mgrs.AddCardFromAllPlayersToPot(It.IsAny<CurrentRound>()));

            var result = _warGameController.ContinueRoundWithWar();

            Assert.AreEqual(expectedResult, result);
            _mockGameRoundService.Verify(mgrs => mgrs.AddCardFromAllPlayersToPot(It.IsAny<CurrentRound>()), Times.Never);
        }

        [Test]
        public void ContinueRoundWithWar_WithEnoughCardsToPlay_ShouldAddCardsToPotTwice()
        {
            var players = InitializeTwoPlayerGame();
            var comparisonResult = new CardComparisonResult(null, null, ComparisonResult.Tie);
            _mockGameRoundService.Setup(mgrs => mgrs.GetResultIfNotEnoughCardsToContinue(It.IsAny<CurrentRound>())).Returns((SingleCardPlayResult)null);
            _mockGameRoundService.Setup(mgrs => mgrs.AddCardFromAllPlayersToPot(It.IsAny<CurrentRound>()));
            _mockGameRoundService.Setup(mgrs => mgrs.GetCardDrawResult(It.IsAny<CurrentRound>())).Returns(comparisonResult);

            var result = _warGameController.ContinueRoundWithWar();

            _mockGameRoundService.Verify(mgrs => mgrs.AddCardFromAllPlayersToPot(It.IsAny<CurrentRound>()), Times.Exactly(2));
        }

        [Test]
        public void GetGameStandings_TwoPlayers_ShouldContainPlayerCardCounts()
        {
            var players = InitializeTwoPlayerGame();
            players[0].Deck = new List<Card>() { new Card(Suit.Clubs, Rank.Eight), new Card(Suit.Clubs, Rank.Ace) };
            players[1].Deck = new List<Card>() { new Card(Suit.Diamonds, Rank.Jack) };

            var result = _warGameController.GetGameStandings();

            Assert.IsTrue(result.Contains(players[0].Name + ": " + players[0].Deck.Count));
            Assert.IsTrue(result.Contains(players[1].Name + ": " + players[1].Deck.Count));

        }

        [Test]
        public void GetGameWinnerName_BothPlayersOutOfCards_ShouldReturnNoWinnerString()
        {
            InitializeTwoPlayerGame();
            _mockPlayerService.Setup(mps => mps.IsPlayerOutOfCards(It.IsAny<Player>())).Returns(true);

            var result = _warGameController.GetGameWinnerName();

            Assert.AreEqual("Nobody", result);
        }

        [Test]
        public void GetGameWinnerName_FirstPlayerOutOfCards_ShouldReturnSecondPlayerName()
        {
            var players = InitializeTwoPlayerGame();
            _mockPlayerService.Setup(mps => mps.IsPlayerOutOfCards(players[0])).Returns(true);
            _mockPlayerService.Setup(mps => mps.IsPlayerOutOfCards(players[1])).Returns(false);

            var result = _warGameController.GetGameWinnerName();

            Assert.AreEqual(players[1].Name, result);
        }

        [Test]
        public void GetGameWinnerName_SecondPlayerOutOfCards_ShouldReturnSecondPlayerName()
        {
            var players = InitializeTwoPlayerGame();
            _mockPlayerService.Setup(mps => mps.IsPlayerOutOfCards(players[0])).Returns(false);
            _mockPlayerService.Setup(mps => mps.IsPlayerOutOfCards(players[1])).Returns(true);

            var result = _warGameController.GetGameWinnerName();

            Assert.AreEqual(players[0].Name, result);
        }

        [Test]
        public void Initialize_WithOnePlayerName_ShouldThrowArgumentException()
        {
            var playerNames = new List<string>() { "Tester1" };
            
            Assert.Throws<ArgumentException>(() => _warGameController.Initialize(playerNames));
        }

        [Test]
        public void Initialize_WithTwoPlayerNames_ShouldCreateShuffledDeck()
        {
            var playerNames = new List<string>() { "Tester1", "Tester2" };
            _mockDealerService.Setup(mds => mds.CreateDeck()).Returns(new List<Card>());
            _mockDealerService.Setup(mds => mds.ShuffleDeck(It.IsAny<List<Card>>()));
            _mockDealerService.Setup(mds => mds.DealCards(It.IsAny<List<Card>>(), It.IsAny<List<Player>>()));
            _mockPlayerService.Setup(mps => mps.CreatePlayers(It.IsAny<List<string>>())).Returns(new List<Player>());

            _warGameController.Initialize(playerNames);

            _mockDealerService.Verify(mds => mds.CreateDeck(), Times.Once);
            _mockDealerService.Verify(mds => mds.ShuffleDeck(It.IsAny<List<Card>>()), Times.Once);
        }

        [Test]
        public void Initialize_WithTwoPlayerNames_ShouldCreatePlayers()
        {
            var playerNames = new List<string>() { "Tester1", "Tester2" };
            _mockDealerService.Setup(mds => mds.CreateDeck()).Returns(new List<Card>());
            _mockDealerService.Setup(mds => mds.ShuffleDeck(It.IsAny<List<Card>>()));
            _mockDealerService.Setup(mds => mds.DealCards(It.IsAny<List<Card>>(), It.IsAny<List<Player>>()));
            _mockPlayerService.Setup(mps => mps.CreatePlayers(It.IsAny<List<string>>())).Returns(new List<Player>());

            _warGameController.Initialize(playerNames);

            _mockPlayerService.Verify(mps => mps.CreatePlayers(playerNames), Times.Once);
        }

        [Test]
        public void Initialize_WithTwoPlayerNames_ShouldDealCardsToPlayers()
        {
            var playerNames = new List<string>() { "Tester1", "Tester2" };
            var players = new List<Player>();
            var cardDeck = new List<Card>();
            _mockDealerService.Setup(mds => mds.CreateDeck()).Returns(cardDeck);
            _mockDealerService.Setup(mds => mds.ShuffleDeck(It.IsAny<List<Card>>()));
            _mockDealerService.Setup(mds => mds.DealCards(It.IsAny<List<Card>>(), It.IsAny<List<Player>>()));
            _mockPlayerService.Setup(mps => mps.CreatePlayers(It.IsAny<List<string>>())).Returns(players);

            _warGameController.Initialize(playerNames);

            _mockDealerService.Verify(mds => mds.DealCards(cardDeck, players), Times.Once);
        }

        [Test]
        public void IsGameOver_BothPlayersOutOfCards_ShouldReturnTrue()
        {
            InitializeTwoPlayerGame();
            _mockPlayerService.Setup(mps => mps.IsPlayerOutOfCards(It.IsAny<Player>())).Returns(true);

            var result = _warGameController.IsGameOver();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsGameOver_FirstPlayerOutOfCards_ShouldReturnTrue()
        {
            InitializeTwoPlayerGame();
            _mockPlayerService.SetupSequence(mps => mps.IsPlayerOutOfCards(It.IsAny<Player>())).Returns(true).Returns(false);

            var result = _warGameController.IsGameOver();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsGameOver_SecondPlayerOutOfCards_ShouldReturnTrue()
        {
            InitializeTwoPlayerGame();
            _mockPlayerService.SetupSequence(mps => mps.IsPlayerOutOfCards(It.IsAny<Player>())).Returns(false).Returns(true);

            var result = _warGameController.IsGameOver();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsGameOver_BothPlayersHaveCards_ShouldReturnFalse()
        {
            InitializeTwoPlayerGame();
            _mockPlayerService.Setup(mps => mps.IsPlayerOutOfCards(It.IsAny<Player>())).Returns(false);

            var result = _warGameController.IsGameOver();

            Assert.IsFalse(result);
        }

        [Test]
        public void StartNewRound_NotEnoughCardsToPlay_ShouldReturnResultWithoutAddingToPot()
        {
            var players = InitializeTwoPlayerGame();
            var expectedResult = new SingleCardPlayResult(null, null, false, "Test");
            _mockGameRoundService.Setup(mgrs => mgrs.GetResultIfNotEnoughCardsToContinue(It.IsAny<CurrentRound>())).Returns(expectedResult);
            _mockGameRoundService.Setup(mgrs => mgrs.AddCardFromAllPlayersToPot(It.IsAny<CurrentRound>()));

            var result = _warGameController.StartNewRound();

            Assert.AreEqual(expectedResult, result);
            _mockGameRoundService.Verify(mgrs => mgrs.AddCardFromAllPlayersToPot(It.IsAny<CurrentRound>()), Times.Never);
        }

        [Test]
        public void StartNewRound_WithEnoughCardsToPlay_ShouldAddCardsToPot()
        {
            var players = InitializeTwoPlayerGame();
            var comparisonResult = new CardComparisonResult(null, null, ComparisonResult.Tie);
            _mockGameRoundService.Setup(mgrs => mgrs.GetResultIfNotEnoughCardsToContinue(It.IsAny<CurrentRound>())).Returns((SingleCardPlayResult)null);
            _mockGameRoundService.Setup(mgrs => mgrs.AddCardFromAllPlayersToPot(It.IsAny<CurrentRound>()));
            _mockGameRoundService.Setup(mgrs => mgrs.GetCardDrawResult(It.IsAny<CurrentRound>())).Returns(comparisonResult);

            var result = _warGameController.StartNewRound();
            
            _mockGameRoundService.Verify(mgrs => mgrs.AddCardFromAllPlayersToPot(It.IsAny<CurrentRound>()), Times.Once);
        }

        [Test]
        public void StartNewRound_CardsAreTied_ShouldRequireWarRound()
        {
            var players = InitializeTwoPlayerGame();
            var player1Card = new Card(Suit.Clubs, Rank.Ace);
            var player2Card = new Card(Suit.Clubs, Rank.Nine);
            var comparisonResult = new CardComparisonResult(player1Card, player2Card, ComparisonResult.Tie);
            _mockGameRoundService.Setup(mgrs => mgrs.GetResultIfNotEnoughCardsToContinue(It.IsAny<CurrentRound>())).Returns((SingleCardPlayResult)null);
            _mockGameRoundService.Setup(mgrs => mgrs.AddCardFromAllPlayersToPot(It.IsAny<CurrentRound>()));
            _mockGameRoundService.Setup(mgrs => mgrs.GetCardDrawResult(It.IsAny<CurrentRound>())).Returns(comparisonResult);

            var result = _warGameController.StartNewRound();

            _mockGameRoundService.Verify(mgrs => mgrs.GetCardDrawResult(It.IsAny<CurrentRound>()), Times.Once);
            Assert.IsTrue(result.IsWarRequired);
            Assert.AreEqual(player1Card, result.FirstPlayerCard);
            Assert.AreEqual(player2Card, result.SecondPlayerCard);
        }

        [Test]
        public void StartNewRound_FirstPlayerCardWins_ShouldNotRequireWarRound()
        {
            var players = InitializeTwoPlayerGame();
            var player1Card = new Card(Suit.Clubs, Rank.Ace);
            var player2Card = new Card(Suit.Clubs, Rank.Nine);
            players[0].Deck = new List<Card>();
            var comparisonResult = new CardComparisonResult(player1Card, player2Card, ComparisonResult.FirstCardWins);
            _mockGameRoundService.Setup(mgrs => mgrs.GetResultIfNotEnoughCardsToContinue(It.IsAny<CurrentRound>())).Returns((SingleCardPlayResult)null);
            _mockGameRoundService.Setup(mgrs => mgrs.AddCardFromAllPlayersToPot(It.IsAny<CurrentRound>()));
            _mockGameRoundService.Setup(mgrs => mgrs.GetCardDrawResult(It.IsAny<CurrentRound>())).Returns(comparisonResult);
            _mockGameRoundService.Setup(mgrs => mgrs.GetRoundWinnings(It.IsAny<CurrentRound>())).Returns(new List<Card>());

            var result = _warGameController.StartNewRound();

            _mockGameRoundService.Verify(mgrs => mgrs.GetCardDrawResult(It.IsAny<CurrentRound>()), Times.Once);
            Assert.IsFalse(result.IsWarRequired);
            Assert.AreEqual(player1Card, result.FirstPlayerCard);
            Assert.AreEqual(player2Card, result.SecondPlayerCard);
        }

        [Test]
        public void StartNewRound_FirstPlayerCardWins_ShouldAddWinningsToFirstPlayerDeck()
        {
            var players = InitializeTwoPlayerGame();
            var player1Card = new Card(Suit.Clubs, Rank.Ace);
            var player2Card = new Card(Suit.Clubs, Rank.Nine);
            players[0].Deck = new List<Card>();
            var comparisonResult = new CardComparisonResult(player1Card, player2Card, ComparisonResult.FirstCardWins);
            var roundWinnings = new List<Card>() { new Card(Suit.Diamonds, Rank.Jack), new Card(Suit.Hearts, Rank.Queen) };
            _mockGameRoundService.Setup(mgrs => mgrs.GetResultIfNotEnoughCardsToContinue(It.IsAny<CurrentRound>())).Returns((SingleCardPlayResult)null);
            _mockGameRoundService.Setup(mgrs => mgrs.AddCardFromAllPlayersToPot(It.IsAny<CurrentRound>()));
            _mockGameRoundService.Setup(mgrs => mgrs.GetCardDrawResult(It.IsAny<CurrentRound>())).Returns(comparisonResult);
            _mockGameRoundService.Setup(mgrs => mgrs.GetRoundWinnings(It.IsAny<CurrentRound>())).Returns(roundWinnings);
            
            var result = _warGameController.StartNewRound();

            _mockGameRoundService.Verify(mgrs => mgrs.GetRoundWinnings(It.IsAny<CurrentRound>()), Times.Once);
            foreach(var card in roundWinnings)
            {
                Assert.Contains(card, players[0].Deck);
            }
        }

        [Test]
        public void StartNewRound_SecondPlayerCardWins_ShouldNotRequireWarRound()
        {
            var players = InitializeTwoPlayerGame();
            var player1Card = new Card(Suit.Clubs, Rank.Five);
            var player2Card = new Card(Suit.Clubs, Rank.Nine);
            players[1].Deck = new List<Card>();
            var comparisonResult = new CardComparisonResult(player1Card, player2Card, ComparisonResult.SecondCardWins);
            _mockGameRoundService.Setup(mgrs => mgrs.GetResultIfNotEnoughCardsToContinue(It.IsAny<CurrentRound>())).Returns((SingleCardPlayResult)null);
            _mockGameRoundService.Setup(mgrs => mgrs.AddCardFromAllPlayersToPot(It.IsAny<CurrentRound>()));
            _mockGameRoundService.Setup(mgrs => mgrs.GetCardDrawResult(It.IsAny<CurrentRound>())).Returns(comparisonResult);
            _mockGameRoundService.Setup(mgrs => mgrs.GetRoundWinnings(It.IsAny<CurrentRound>())).Returns(new List<Card>());

            var result = _warGameController.StartNewRound();

            _mockGameRoundService.Verify(mgrs => mgrs.GetCardDrawResult(It.IsAny<CurrentRound>()), Times.Once);
            Assert.IsFalse(result.IsWarRequired);
            Assert.AreEqual(player1Card, result.FirstPlayerCard);
            Assert.AreEqual(player2Card, result.SecondPlayerCard);
        }

        [Test]
        public void StartNewRound_SecondPlayerCardWins_ShouldAddWinningsToSecondPlayerDeck()
        {
            var players = InitializeTwoPlayerGame();
            var player1Card = new Card(Suit.Clubs, Rank.Five);
            var player2Card = new Card(Suit.Clubs, Rank.Nine);
            players[1].Deck = new List<Card>();
            var comparisonResult = new CardComparisonResult(player1Card, player2Card, ComparisonResult.SecondCardWins);
            var roundWinnings = new List<Card>() { new Card(Suit.Diamonds, Rank.Jack), new Card(Suit.Hearts, Rank.Queen) };
            _mockGameRoundService.Setup(mgrs => mgrs.GetResultIfNotEnoughCardsToContinue(It.IsAny<CurrentRound>())).Returns((SingleCardPlayResult)null);
            _mockGameRoundService.Setup(mgrs => mgrs.AddCardFromAllPlayersToPot(It.IsAny<CurrentRound>()));
            _mockGameRoundService.Setup(mgrs => mgrs.GetCardDrawResult(It.IsAny<CurrentRound>())).Returns(comparisonResult);
            _mockGameRoundService.Setup(mgrs => mgrs.GetRoundWinnings(It.IsAny<CurrentRound>())).Returns(roundWinnings);

            var result = _warGameController.StartNewRound();

            _mockGameRoundService.Verify(mgrs => mgrs.GetRoundWinnings(It.IsAny<CurrentRound>()), Times.Once);
            foreach (var card in roundWinnings)
            {
                Assert.Contains(card, players[1].Deck);
            }
        }

        private List<Player> InitializeTwoPlayerGame()
        {
            var playerNames = new List<string>() { "Tester1", "Tester2" };
            var players = new List<Player>() { new Player(playerNames[0]), new Player(playerNames[1]) };
            var cardDeck = new List<Card>();
            _mockDealerService.Setup(mds => mds.CreateDeck()).Returns(cardDeck);
            _mockDealerService.Setup(mds => mds.ShuffleDeck(It.IsAny<List<Card>>()));
            _mockDealerService.Setup(mds => mds.DealCards(It.IsAny<List<Card>>(), It.IsAny<List<Player>>()));
            _mockPlayerService.Setup(mps => mps.CreatePlayers(It.IsAny<List<string>>())).Returns(players);

            _warGameController.Initialize(playerNames);

            return players;
        }
    }
}
