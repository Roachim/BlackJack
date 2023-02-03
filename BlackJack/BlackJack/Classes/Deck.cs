using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack.Classes
{
    public static class Deck
    {
        //Dictionary instead of list so easier to remove cards when given to a player
        public static readonly Dictionary<string, Card> deck = new Dictionary<string, Card>();
        //public static readonly List<Card> deck = new List<Card>();

        //have method for adding a full deck of cards in random order (public)
        public static void NewDeck()
        {
            for(int i = 1; i <= 4; i++) //up to inkluding 4
            {
                for (int e = 1; e <= 13; e++) //up to inkluding 13
                {
                    Card card = new Card(i, e);
                    if (deck.ContainsKey(card.Name))
                    {
                        continue;
                    }
                    deck.Add(card.Name, card);
                }
            }
        }

        //have method for giving card to player / remove card from deck. (public)
        public static bool DealCard(Person person)
        {
            Random rnd = new Random();
            try
            {
                if (deck.Count == 0)
                {
                    Console.WriteLine("Deck is empty! No more cards this round");
                    return false;
                }
                Card rndCard = deck.ElementAt(rnd.Next(0, deck.Count())).Value; //get a random card 
                person.Cards.Add(rndCard); //add the card to a persons stack of cards
                deck.Remove(rndCard.Name); //remove card from deck
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
