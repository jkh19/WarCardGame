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
    public class WarGameController : IWarGameController
    {
        private const string NO_WINNER_NAME = "Nobody";
        private const string ERROR_TWO_PLAYER_GAME_REQUIRED = "Current implementation only supports the standard 2-player game.";
        private const string RESULT_WAR_REQUIRED = "time for war!";
        private const string RESULT_ROUND_WINNER_FORMAT = "{0} won this round!";
        private const string SUMMARY_CARD_COUNT_HEADER = "Current card counts: ";
        private const string SUMMARY_CARD_COUNT_PLAYER_DETAILS_FORMAT = "\n\t{0}: {1}";

        private IDealerService _dealerService;
        private IPlayerService _playerService;
        private IGameRoundService _gameRoundService;
        private List<Player> _players;
        private CurrentRound _currentRound;
        
        public WarGameController(IDealerService dealerService, IPlayerService playerService, IGameRoundService gameRoundService)
        {
            _dealerService = dealerService;
            _playerService = playerService;
            _gameRoundService = gameRoundService;
        }

        public string GetGameWinnerName()
        {
            if (_playerService.IsPlayerOutOfCards(_players[0]) && _playerService.IsPlayerOutOfCards(_players[1]))
            {
                return NO_WINNER_NAME;
            }
            else if (_playerService.IsPlayerOutOfCards(_players[0]))
            {
                return _players[1].Name;
            }
            else
            {
                return _players[0].Name;
            }
        }

        public void Initialize(List<string> playerNames)
        {
            if(!IsStartingTwoPlayerGame(playerNames))
            {
                throw new ArgumentException(ERROR_TWO_PLAYER_GAME_REQUIRED);
            }

            var deck = _dealerService.CreateDeck();
            _dealerService.ShuffleDeck(deck);
            _players = _playerService.CreatePlayers(playerNames);          
            _dealerService.DealCards(deck, _players);
        }

        public bool IsGameOver()
        {
            return _playerService.IsPlayerOutOfCards(_players[0]) || _playerService.IsPlayerOutOfCards(_players[1]);
        }

        public SingleCardPlayResult StartNewRound()
        {
            _currentRound = new CurrentRound(_players);
            return PlaySingleCardDraw();
        }

        public SingleCardPlayResult ContinueRoundWithWar()
        {
            var singleCardPlayResult = _gameRoundService.GetResultIfNotEnoughCardsToContinue(_currentRound);
            if (singleCardPlayResult == null)
            {
                //Add extra card to pot before revealing next card for war result
                _gameRoundService.AddCardFromAllPlayersToPot(_currentRound);
                singleCardPlayResult = PlaySingleCardDraw();
            }
            return singleCardPlayResult;
        }      

        private void CollectPotWinnings(Player player)
        {
            //Introduce some randomness by shuffling the pot deck before adding to the player's deck
            var winnings = _gameRoundService.GetRoundWinnings(_currentRound);
            _dealerService.ShuffleDeck(winnings);
            player.Deck.AddRange(winnings);
        }   

        private bool IsStartingTwoPlayerGame(List<string> playerNames)
        {
            return playerNames != null && playerNames.Count == 2;
        }

        private SingleCardPlayResult PlaySingleCardDraw()
        {
            var singleCardPlayResult = _gameRoundService.GetResultIfNotEnoughCardsToContinue(_currentRound);
            if(singleCardPlayResult == null)
            {
                _gameRoundService.AddCardFromAllPlayersToPot(_currentRound);
                var cardDrawResult = _gameRoundService.GetCardDrawResult(_currentRound);
                switch(cardDrawResult.ComparisonResult)
                {
                    case ComparisonResult.Tie:
                        singleCardPlayResult = new SingleCardPlayResult(cardDrawResult.FirstPlayerCard, cardDrawResult.SecondPlayerCard, true, RESULT_WAR_REQUIRED);
                        break;
                    case ComparisonResult.FirstCardWins:
                        singleCardPlayResult = new SingleCardPlayResult(cardDrawResult.FirstPlayerCard, cardDrawResult.SecondPlayerCard, false, string.Format(RESULT_ROUND_WINNER_FORMAT, _players[0].Name));
                        CollectPotWinnings(_players[0]);
                        break;
                    case ComparisonResult.SecondCardWins:
                        singleCardPlayResult = new SingleCardPlayResult(cardDrawResult.FirstPlayerCard, cardDrawResult.SecondPlayerCard, false, string.Format(RESULT_ROUND_WINNER_FORMAT, _players[1].Name));
                        CollectPotWinnings(_players[1]);
                        break;
                }              
            }

            return singleCardPlayResult;
        }

        public string GetGameStandings()
        {
            var summary = SUMMARY_CARD_COUNT_HEADER;
            foreach(var player in _players)
            {
                summary += string.Format(SUMMARY_CARD_COUNT_PLAYER_DETAILS_FORMAT, player.Name, player.Deck.Count);
            }
            return summary;
        }
    }
}
