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
    /// <summary>
    /// Service to handle one round of the war card game (collecting cards
    /// from players, comparing cards, and maintaining winnings for the round)
    /// </summary>
    public class GameRoundService : IGameRoundService
    {
        private const string NO_CARDS_ALL_PLAYERS = "All players ran out of cards.";
        private const string NO_CARDS_SINGLE_PLAYER = "{0} ran out of cards.";

        private IPlayerService _playerService;
        
        public GameRoundService(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public void AddCardFromAllPlayersToPot(CurrentRound currentRound)
        {
            foreach(var player in currentRound.Players)
            {
                currentRound.CardPot.Add(_playerService.RemoveTopCardForPlayer(player));
            }
        }

        public CardComparisonResult GetCardDrawResult(CurrentRound currentRound)
        {
            var firstPlayerCard = currentRound.CardPot[currentRound.CardPot.Count - 2];
            var secondPlayerCard = currentRound.CardPot[currentRound.CardPot.Count - 1];
            var comparisonResult = CompareCards(firstPlayerCard, secondPlayerCard);

            return new CardComparisonResult(firstPlayerCard, secondPlayerCard, comparisonResult);
        }

        public SingleCardPlayResult GetResultIfNotEnoughCardsToContinue(CurrentRound currentRound)
        {
            SingleCardPlayResult result = null;

            if (_playerService.IsPlayerOutOfCards(currentRound.Players[0]) && _playerService.IsPlayerOutOfCards(currentRound.Players[1]))
            {
                result = new SingleCardPlayResult(null, null, false, NO_CARDS_ALL_PLAYERS);
            }
            else if (_playerService.IsPlayerOutOfCards(currentRound.Players[0]))
            {
                result = new SingleCardPlayResult(null, null, false, string.Format(NO_CARDS_SINGLE_PLAYER, currentRound.Players[0].Name));
            }
            else if (_playerService.IsPlayerOutOfCards(currentRound.Players[1]))
            {
                result = new SingleCardPlayResult(null, null, false, string.Format(NO_CARDS_SINGLE_PLAYER, currentRound.Players[1].Name));
            }

            return result;
        }

        public List<Card> GetRoundWinnings(CurrentRound currentRound)
        {
            return currentRound.CardPot;
        }

        private ComparisonResult CompareCards(Card card1, Card card2)
        {
            var compareToResult = card1.CompareTo(card2);
            if (compareToResult > 0)
            {
                return ComparisonResult.FirstCardWins;
            }
            else if(compareToResult < 0)
            {
                return ComparisonResult.SecondCardWins;
            }

            return ComparisonResult.Tie;
        }
    }
}
