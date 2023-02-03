using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.Classes
{
    public interface Person
    {
        public List<Card> Cards { get; set; }

        public int Money { get; set; }
    }
}
