using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarCardGame.Enums;

namespace WarCardGame.Models
{
    public class CardComparisonResult
    {
        public Card FirstPlayerCard { get; }
        public Card SecondPlayerCard { get; }
        public ComparisonResult ComparisonResult { get; }

        public CardComparisonResult(Card firstPlayerCard, Card secondPlayerCard, ComparisonResult comparisonResult)
        {
            FirstPlayerCard = firstPlayerCard;
            SecondPlayerCard = secondPlayerCard;
            ComparisonResult = comparisonResult;
        }
    }
}
