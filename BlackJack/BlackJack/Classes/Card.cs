using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BlackJack.Classes
{
    public class Card
    {
        private readonly string _name;
        private readonly int _point;
        private readonly Type _type;
        private readonly Rank _rank;
        public int Point { get { return _point; } }
        public  string Name { get { return _name; } }
        enum Type
        {
            Heart = 1,
            Club = 2,
            Diamond = 3, 
            Spade = 4

        }
        enum Rank
        {
            Ace = 1, 
            Two = 2, 
            Three = 3, 
            Four = 4,
            Five = 5, 
            Six = 6, 
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Ten = 10, 
            Jack = 11,
            Queen = 12,
            King = 13
        }
        /// <summary>
        /// The class for all cards in a deck. Which card is made via the "type" and "rank";
        /// Name will be based on type and rank. Points = rank.
        /// </summary>
        /// <param name="type">card type. spade, club, etc.</param>
        /// <param name="rank">card rank. 4, 5, 6. etc.</param>
        public Card(int type, int rank)
        {
            _type = (Type)type;
            _rank = (Rank)rank;
            _name = _rank.ToString() + " of " +_type.ToString();
            if(rank == 1)
            {
                _point = 11;
            }
            else if(rank > 10)
            {
                _point = 10;
            }else { _point = rank; }
            
        }
        
    }
}
