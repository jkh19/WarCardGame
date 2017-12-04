using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarCardGame.Interfaces;
using WarCardGame.Models;

namespace WarCardGame.Services
{
    public class PlayerService : IPlayerService
    {
        public List<Player> CreatePlayers(List<string> playerNames)
        {
            var players = new List<Player>();
            foreach(var name in playerNames)
            {
                players.Add(new Player(name));
            }
            return players;
        }

        public bool IsPlayerOutOfCards(Player player)
        {
            return player.Deck == null || player.Deck.Count == 0;
        }

        public Card RemoveTopCardForPlayer(Player player)
        {
            var topCard = player.Deck.First();
            player.Deck.RemoveAt(0);
            return topCard;
        }
    }
}
