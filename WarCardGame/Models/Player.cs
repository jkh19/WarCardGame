using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarCardGame.Models
{
    public class Player
    {
        public List<Card> Deck { get; set; }
        public string Name { get; }

        public Player(string name)
        {
            Name = name;
        }
    }
}
