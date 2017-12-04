using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarCardGame.Models;

namespace WarCardGame.Interfaces
{
    public interface IPlayerService
    {
        List<Player> CreatePlayers(List<string> playerNames);
        bool IsPlayerOutOfCards(Player player);
        Card RemoveTopCardForPlayer(Player player);
    }
}
