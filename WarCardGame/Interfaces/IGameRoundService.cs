using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarCardGame.Enums;
using WarCardGame.Models;

namespace WarCardGame.Interfaces
{
    public interface IGameRoundService
    {
        void AddCardFromAllPlayersToPot(CurrentRound currentRound);
        CardComparisonResult GetCardDrawResult(CurrentRound currentRound);
        SingleCardPlayResult GetResultIfNotEnoughCardsToContinue(CurrentRound currentRound);
        List<Card> GetRoundWinnings(CurrentRound currentRound);      
    }
}
