using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.Classes
{
    public class Player : Person
    {
        public List<Card> Cards { get; set; }
        public int Money { get; set; }
        public int Wager { get; set; }
        
        public Player() 
        {
            Cards = new List<Card>();
            Money = 500;
            Wager = 0;
        }

        /// <summary>
        /// The player wages money. The money is lost until a winner is decided, Whereafter they may still be lost :)
        /// </summary>
        /// <param name="amount">How much to wage</param>
        /// <returns>True if player has enough. False if they don't</returns>
        public bool Wage(int amount)
        {
            if(amount > Money || amount < 1) { return false; }
            
            Money -= amount;
            Wager += amount;
            return true;
        }
       
    }
}
