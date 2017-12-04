using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarCardGame.Enums;

namespace WarCardGame.Models
{
    /// <summary>
    /// Contains the result of a single card draw as part of a full round
    /// </summary>
    public class SingleCardPlayResult
    {
        private const string RESULT_STRING_FORMAT = "{0} vs {1}...{2}";
        
        public Card FirstPlayerCard { get; }
        public Card SecondPlayerCard { get; }
        public bool IsWarRequired { get; }
        public string Message { get; }

        public SingleCardPlayResult(Card firstPlayerCard, Card secondPlayerCard, bool isWarRequired, string message)
        {
            FirstPlayerCard = firstPlayerCard;
            SecondPlayerCard = secondPlayerCard;
            IsWarRequired = isWarRequired;
            Message = message;
        }

        public override string ToString()
        {
            if (FirstPlayerCard != null && SecondPlayerCard != null)
            {
                return string.Format(RESULT_STRING_FORMAT, FirstPlayerCard, SecondPlayerCard, Message);
            }
            else
            {
                return Message;
            }
        }
    }
}
