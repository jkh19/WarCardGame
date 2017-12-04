using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarCardGame.Models;

namespace WarCardGame.Interfaces
{
    public interface IWarGameController
    {
        SingleCardPlayResult ContinueRoundWithWar();
        string GetGameStandings();
        string GetGameWinnerName();
        void Initialize(List<string> playerNames);
        bool IsGameOver();
        SingleCardPlayResult StartNewRound();
    }
}
