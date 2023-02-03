using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.Classes
{
    public class Dealer : Person
    {
        public List<Card> Cards { get; set; }
        private int _money;
        public int Money 
        {
            get { return _money; }
            set { if (Money < 0) { Money = 0; } else { _money = value; } }
        }
        public Dealer() 
        {
            Cards = new List<Card>();
            _money = 1000;
        }
        
    }
}
