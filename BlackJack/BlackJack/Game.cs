using BlackJack.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace BlackJack
{
    public static class Game
    {
        #region GameLoop

        /// <summary>
        /// The only thing to be run outside the game class
        /// Makes a new deck, fills it, and creates a new player and dealer
        /// </summary>
        public static void RunGame() //a game consists of up to multiple rounds
        {
            Deck.deck.Clear();
            Deck.NewDeck();
            //foreach (KeyValuePair<string, Card> card in Deck.deck)
            //{
            //    Console.WriteLine(card.Value.Point
            //        + ": " + card.Value.Name);
            //}

            Dealer dealer = new Dealer();

            Player player= new Player();

            Round(player, dealer); //the round starts // put 1 at end for debug

            //End game, could be own method

            Console.WriteLine("Game Over!");
            Console.WriteLine("Your final payout: " + player.Money);
            Console.WriteLine("Dealers final payout: " + dealer.Money);
            Console.WriteLine("-----------------");
            Console.WriteLine("press x to retry!");
            Console.WriteLine("press any other key to close!");

            ConsoleKey input = Console.ReadKey().Key;
            if (input == ConsoleKey.X)
            {
                RunGame();
            }
            else { return; }
        }
        /// <summary>
        /// A round to decide who wins money
        /// </summary>
        /// <param name="player">The player object</param>
        /// <param name="dealer">The dealer object</param>
        /// <param name="debug">Put in debug mode - put 1</param>
        private static void Round(Player player, Dealer dealer, int debug = 0) //a round consists of up to multiple turns
        {
            //card list must be empty from former round
            player.Cards.Clear();
            dealer.Cards.Clear();

            if (player.Money < 2 || dealer.Money < 2) //check if anybody broke, if broke, stop the rounds
            {
                return;
            }
            Console.Clear();
            Console.WriteLine("New round!");
            Console.WriteLine("-------------------------------------");
            //The player places their bet
            Console.WriteLine("Please place an even bet of minimum 2");
            Console.WriteLine("You have: " + player.Money);
            Console.WriteLine("Casino is willing to lose: " + dealer.Money + " Before stopping");
            Console.WriteLine("-------------------------------------");
            Console.Write("bet: ");
            string playerbet = Console.ReadLine().Trim();
            if(playerbet == string.Empty)
            {
                Console.WriteLine("Please input something before pressing enter.");
                Console.WriteLine("Press any key to go back");
                Console.ReadKey();
                Round(player, dealer);
                return;
            }
            int bet = Int32.Parse(new string(playerbet.Where(char.IsDigit).ToArray())); //Remove non-numeric and make into int
            if(bet < 2 || bet%2 != 0)
            {
                Console.WriteLine("The bet must be even and minimum 2");
                Console.WriteLine("press any key to go back");
                Console.ReadKey();
                Round(player, dealer);
                return;
            }
            if(bet > player.Money)
            {
                Console.WriteLine("Nice try");
                Console.WriteLine("You cannot bet more than you have");
                Console.ReadKey();
                Round(player, dealer);
                return;
            }

            player.Wage(bet);

            //give 2 cards to the dealer and the player
            if(debug == 0)
            {
                Deck.DealCard(dealer);
                Deck.DealCard(dealer);
                Deck.DealCard(player);
                Deck.DealCard(player);
            }
            if(debug == 1) //test for multiple aces ---------------------------------------------------------------------
            {
                Card card1 = new Card(1, 1);
                Card card2 = new Card(2, 1);
                Card card3 = new Card(3, 1);
                Card card4 = new Card(1, 1);
                Card card5 = new Card(1, 10);
                Card card6 = new Card(1, 10);
                player.Cards.Add(card1);
                //player.Cards.Add(card2);
                //player.Cards.Add(card3);
                //player.Cards.Add(card4);
                //player.Cards.Add(card5);
                player.Cards.Add(card6);
                Deck.DealCard(dealer);
                Deck.DealCard(dealer);
            }
            
            //if player has blackjack they win immediately, same for dealer
            bool playerJack = CheckForBlackjack(player.Cards);
            bool dealerjack = CheckForBlackjack(dealer.Cards);

            if(playerJack && dealerjack) //its a draw!!
            {
                
                Console.WriteLine("Its a draw!!");
                Console.WriteLine("Nobody wins, nobody loses!");
                Console.WriteLine("New round!");
                Console.ReadKey();
                DeclareWinner(player, dealer);
                Round(player, dealer);
                return;
            }
            else if (playerJack) //player wins with blackjack!!
            {
                
                DeclareWinner(player, dealer, true);
                Round(player, dealer);
                return;
            } 
            else if(dealerjack) //dealer wins with blackjack!! normal win...
            {
                
                DeclareWinner(player, dealer);
                Round(player, dealer);
                return;
            }


            //the turns now begin
            Turn(player, dealer); //---------the turn for player and dealer---------

            if(player.Cards.Count == 0)
            {
                DeclareWinner(player, dealer);
                Round(player, dealer);
                return;
            }
            if (CheckForBust(player.Cards)) //dealer wins
            {
                DeclareWinner(player, dealer);
                Round(player, dealer);
                return;
                
            }
            else if (CheckForBust(dealer.Cards)) //player wins
            {
                DeclareWinner(player, dealer);
                Round(player, dealer);
                return;
            }

            playerJack = CheckForBlackjack(player.Cards);
            dealerjack = CheckForBlackjack(dealer.Cards);

            if (playerJack) //player wins with blackjack!!
            {
                DeclareWinner(player, dealer, true);
                Round(player, dealer);
                return;
            }
            else if (dealerjack) //dealer wins with blackjack!! but its a normal win...
            {
                DeclareWinner(player, dealer);
                Round(player, dealer);
                return;
            }

            //neither bust nor blackjack - check who wins
            DeclareWinner(player, dealer);

            Round(player, dealer);
        } 

        /// <summary>
        /// The basis loop for going through the players and dealers turns. Calls itself if appropriate
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="dealer">The dealer</param>
        /// <param name="playerDone">bool for whether the player ís done - false by default</param>
        /// <param name="dealerDone">bool for whether the dealer ís done - false by default</param>
        private static void Turn(Player player, Dealer dealer, bool playerDone = false, bool dealerDone = false) 
        {
            //player choice
            int currentwage = player.Wager;
            if (playerDone == false) // Players turns
            {
                Console.Clear();
                PlayerUI(player, dealer);
                playerDone = PlayerTurn(player);
            }
            if(player.Cards.Count == 0) //did the player surrender?
            {
                return;
            }
            if(CheckForBust(player.Cards))
            {
                Console.WriteLine("You bust!");
                Console.WriteLine("Dealer wins!");
                Console.ReadKey();
                return;
                //dealer wins
            }

            if (dealerDone == false) //dealers turn
            {
                dealerDone = DealerTurn(dealer);
            }
            if (CheckForBust(dealer.Cards))
            {
                Console.WriteLine("Dealer busts!");
                Console.WriteLine("You win!");
                Console.ReadKey();
                return;
                //player wins
            }


            if (playerDone == false || dealerDone == false) //calls itself if someone is still allowed to act
            {
                Turn(player, dealer, playerDone, dealerDone);
                return;
            }
            else
            {
                return;
            }
        } 

        /// <summary>
        /// The players turn. takes any action that adds card to their deck. decided whether or not they get to draw again
        /// if they so choose to.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>Returns true if they are done this round, false if they get another chance</returns>
        private static bool PlayerTurn(Player player)
        {

            //check for blackjack, maybe make as other method and insert into both player and dealer
            if (CheckScore(player.Cards) == 21)
            {
                Console.WriteLine("You have 21 points");
                Console.WriteLine("");
                Console.WriteLine("You cannot take anymore turns this round.");
                Console.WriteLine("Press to confirm");
                Console.ReadKey();
                return true;
            }

            Console.WriteLine("");
            Console.WriteLine("What will you do?");
            if(player.Cards.Count == 2)
            {
                Console.WriteLine("1*Hit? 2*Stand? 3*Double? or 4*Surrender?");
            }
            else
            {
                Console.WriteLine("1*Hit? 2*Stand? or 3*Double?");
            }
            Console.WriteLine(" ");
            Console.WriteLine("-----------------------------------------");
            string PlayerChoice = Console.ReadLine();
            PlayerChoice = PlayerChoice.Trim().ToLower();
            if (PlayerChoice != "1" && PlayerChoice != "2" && PlayerChoice != "3" && PlayerChoice != "4" && PlayerChoice != "hit" && PlayerChoice != "stand" && PlayerChoice != "double" && PlayerChoice != "surrender")
            {
                Console.WriteLine("Invalid choice. Please input a number 1-4 or the action name.");
                return PlayerTurn(player);
            }
            if(player.Cards.Count > 2 && PlayerChoice == "surrender" || player.Cards.Count > 2 && PlayerChoice == "4")
            {
                Console.WriteLine("No more surrendering! Its time to fight!");
                return PlayerTurn(player);
            }
            Console.WriteLine("");

            if(PlayerChoice == "1" || PlayerChoice == "hit") //Hit
            {
                Console.Clear();
                Console.WriteLine("You take a card");
                Console.WriteLine(" -----------------");
                Deck.DealCard(player);
                Console.WriteLine("You got: " + player.Cards[player.Cards.Count-1].Name);
                Console.WriteLine("You have: ");
                WriteCards(player.Cards);
                Console.WriteLine("For a total score of: " + CheckScore(player.Cards));
                Console.WriteLine("---------------------------------");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                return false;
            }
            if (PlayerChoice == "2" || PlayerChoice == "stand") //Stand
            {
                Console.Clear();
                Console.WriteLine("You chose Stand");
                Console.WriteLine(" -----------------");

                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                return true;
            }
            if (PlayerChoice == "3" || PlayerChoice == "double") //Double
            {
                if (player.Money < player.Wager )
                {
                    Console.WriteLine("You do not have enough money left to double the wager!");
                    return PlayerTurn(player);
                }
                Console.Clear();
                Console.WriteLine("You chose Double");
                Console.WriteLine(" -----------------");

                player.Money -= player.Wager;
                player.Wager *= 2; //double wager
                
                
                Deck.DealCard(player); //get a card
                Console.WriteLine("You got: " + player.Cards[player.Cards.Count - 1].Name);
                Console.WriteLine("You have the cards: ");
                WriteCards(player.Cards);
                Console.WriteLine("For a total score of: " + CheckScore(player.Cards));
                Console.WriteLine("---------------------------------");
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                return true;
            }
            if (PlayerChoice == "4" || PlayerChoice == "surrender") //Surrender
            {
                Console.Clear();
                Console.WriteLine("You chose Surrender");
                Console.WriteLine("You will lose only half your initial bet");
                Console.WriteLine(" -----------------");
                
                player.Wager /= 2;  //half the wager for player here
                player.Money += player.Wager; //return the current half amount of initial bet
                Console.WriteLine("You gave up your cards in despair!");
                player.Cards.Clear();

                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                return true;
            }
            return false;
        }

        /// <summary>
        /// The dealers turn. The dealers AI.
        /// </summary>
        /// <param name="dealer">the dealer</param>
        /// <returns>true if they stand or has blackjack. False if hit.</returns>
        private static bool DealerTurn(Dealer dealer)
        {
            Console.WriteLine("Dealers turn");

            //check for blackjack
            if (CheckForBlackjack(dealer.Cards))
            {
                return true;
            }

            
            //if points are below 17 *hit otherwise *stand according best practices
            int dealerScore = CheckScore(dealer.Cards);

            //*hit. Dealer takes a card.
            if (dealerScore < 17)
            {
                Console.WriteLine("Dealer takes a card.");
                Console.WriteLine("--------------------");
                Deck.DealCard(dealer);
                Console.ReadKey();
                return false;
            }
            else //*Stand. Dealer takes no more cards this round
            {
                Console.WriteLine("Dealer stands.");
                Console.WriteLine("---------------------------");
                Console.ReadKey();
                return true;
            }

        }

        #endregion

        #region Check Functions

        

        /// <summary>
        /// Check whether someones hand is a blackjack. They have 21 points, 2 cards and one card is an ace.
        /// </summary>
        /// <param name="cards">The perons list of cards</param>
        /// <returns>true if they have blackjack</returns>
        private static bool CheckForBlackjack(List<Card> cards)
        {
            
            int score=0;
            bool HasAce = false;
            foreach (Card card in cards) //count score
            {
                score += card.Point;
                if (card.Point == 11)
                {
                    HasAce = true;
                }
            }
            if (score == 21 && HasAce == true && cards.Count == 2) //A Person has blackjack
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// checks whether a given hand for someone is a bust. (a score above 21)
        /// </summary>
        /// <param name="cards">The cards in hand for a given someone</param>
        /// <returns>True if bust. False if not.</returns>
        private static bool CheckForBust(List<Card> cards)
        {
            if(CheckScore(cards) > 21)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// checks and returns a persons points based on their hand of cards.
        /// </summary>
        /// <param name="cards"></param>
        /// <returns>int - a persons total points. minus 10 if above 21 and has ace</returns>
        private static int CheckScore(List<Card> cards)
        {
            int score = 0;
            int aces = 0;
            foreach (Card card in cards) //count score
            {
                score += card.Point;
                if(card.Point == 11)
                {
                    aces += 1;
                }
                
            }
            if(score > 21 && aces > 0) //account for aces
            {
                for (int i = 1; i <= aces; i++)
                {
                    if(score - (10*i) < 22)
                    {
                        return score - (10*i);
                    }
                }
                return score - 10 * aces; //return minimum otherwise
            }

            return score;
        }

        #endregion


        /// <summary>
        /// Dynamic method for comparing the score of all participants in blackjack. If one person bust, then the other is declared winner
        /// </summary>
        /// <param name="playerList">A list of all participants. Includes the dealer.</param>
        /// <returns>The winning 'person'</returns>
        private static Person CompareScore(List<Person> playerList)
        {
            //run each person through, check how close to 21 they are
            //foreach if closer than last, add person as winner until end of list,
            //if person has score > 21 ignore, if everyone above 21 draw

            Person winner = null;
            int Highscore = -1;
            foreach (Person person in playerList)
            {
                int personScore = 0;
                if (CheckForBlackjack(person.Cards)) //A Person has blackjack
                {
                    personScore = 21;
                }
                else
                {
                    personScore = CheckScore(person.Cards);
                }
                if (personScore > 21)
                {
                    personScore = 0;
                }
                else if (personScore == Highscore) //A draw if they have same amount of points
                {
                    return winner = null;
                }
                if (personScore <= 21 && personScore > Highscore)
                {
                    Highscore = personScore;
                    winner = person;
                }

            }
            return winner;
        }

        /// <summary>
        /// The player wins. They get their wager back plus equivelant from the dealer. *2 from the dealer if blackjack.
        /// </summary>
        /// <param name="player">The winning player</param>
        /// <param name="dealer">the losing dealer</param>
        /// <param name="blackJack">Did the player get blackjack?</param>
        //private static void PlayerWin(Player player, Dealer dealer, bool blackJack = false)
        //{
        //    if (blackJack) //If the player won with blackjack
        //    {
        //        dealer.Money -= player.Wager * 2;
        //        player.Money += player.Wager * 3;
        //        Console.WriteLine("You win with blackjack!");
        //        Console.WriteLine("You earn 1.5 times the bet!");
        //        Console.WriteLine("A total of: " + player.Wager * 3);
        //    }
        //    else //normal win
        //    {
        //        dealer.Money -= player.Wager * 1;
        //        player.Money += player.Wager * 2;
        //        Console.WriteLine("You win this round!");
        //        Console.WriteLine("A total of: " + player.Wager * 2);
        //    }
        //    player.Wager = 0;
        //    Console.ReadKey();
        //}


        ///// <summary>
        ///// The dealer has won. The player loses their wager, and it is added to the dealers total money.
        ///// </summary>
        ///// <param name="player">The losing player</param>
        ///// <param name="dealer">The winning dealer</param>
        //private static void DealerWin(Player player, Dealer dealer)
        //{
        //    dealer.Money += player.Wager;
        //    Console.WriteLine("Dealer wins the wager of" + player.Wager);
        //    player.Wager = 0;
        //    Console.ReadKey();
        //}

        ///// <summary>
        ///// No winner. Player gets their wager back.
        ///// </summary>
        ///// <param name="player">The player</param>
        //private static void NoWinner(Player player)
        //{
        //    player.Money += player.Wager;
        //    player.Wager = 0;
        //    Console.WriteLine("It's a draw! You get your wager back");
        //    Console.ReadKey();
        //}

        /// <summary>
        /// Gives general info for the player. Current cards, points. Dealers cards, etc.
        /// Meant to be shown before a decision, while the player has a wager
        /// </summary>
        /// <param name="player">The current player</param>
        /// <param name="dealer">The current dealer</param>
        private static void PlayerUI(Player player, Dealer dealer)
        {
            Console.WriteLine("Current earnings");
            Console.WriteLine(player.Money);
            Console.WriteLine("Current wager");
            Console.WriteLine(player.Wager);
            Console.WriteLine("--------------------------");
            Console.WriteLine("You have the cards: ");
            WriteCards(player.Cards);
            Console.Write("They are worth: ");
            Console.WriteLine(CheckScore(player.Cards) + " points:");

            Console.WriteLine("--------------------------");
            Console.WriteLine("Dealer earnings: ");
            Console.WriteLine(dealer.Money);
            Console.WriteLine("The dealers cards are: ");
            for (int i = 0; i < dealer.Cards.Count; i++)
            {
                if(i == 0) { Console.WriteLine(dealer.Cards[i].Name); } //first card is named, the rest are unknown
                else
                {
                    Console.WriteLine(" + A face down card");
                }
            }
            Console.Write("You can see the faceup card is worth: ");
            Console.WriteLine(dealer.Cards[0].Point);
            Console.WriteLine("--------------------------");
        }

        /// <summary>
        /// quick way of writing all of someones cards
        /// </summary>
        /// <param name="cards"></param>
        private static void WriteCards(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                Console.WriteLine(card.Name);
            }
        }

        /// <summary>
        /// Shows the cards and point total for dealer and player.
        /// Meant to be shown after a round has ended, after wager has been reset
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="dealer">The dealer</param>
        private static void EndSummary(Player player, Dealer dealer)
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine("Your cards: ");
            WriteCards(player.Cards);
            Console.Write("Your score: ");
            Console.WriteLine(CheckScore(player.Cards));
            CheckScore(player.Cards);
            Console.WriteLine("-------------------------------");
            Console.WriteLine("Dealers cards");
            WriteCards(dealer.Cards);
            Console.Write("Dealer score: ");
            Console.WriteLine(CheckScore(dealer.Cards));
            Console.WriteLine("-------------------------------");
        }

        /// <summary>
        /// Exchange the money between player and dealer. Write the conclusion to console.
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="dealer">The dealer</param>
        /// <param name="blackjack">If won with blackjack - put true, otherwise leave blank</param>
        private static void DeclareWinner(Player player, Dealer dealer, bool blackjack = false)
        {
            Console.Clear();
            List<Person> persons = new List<Person>();
            persons.Add(player);
            persons.Add(dealer);
            Person winner = CompareScore(persons);

            if(winner == null)
            {
                Console.WriteLine("It's a draw! You get your wager back");
                player.Money += player.Wager;
            }
            else if (winner == player && blackjack)
            {
                Console.WriteLine("You win with blackjack!");
                Console.WriteLine("You earn double the wager!");
                Console.WriteLine("A total of: " + player.Wager * 2 + " and your wager back");
                dealer.Money -= player.Wager * 2;
                player.Money += player.Wager * 3;
                
            }
            else if(winner == player)
            {
                Console.WriteLine("You win this round!");
                Console.WriteLine("You earn: " + player.Wager + " and your wager back");
                dealer.Money -= player.Wager ;
                player.Money += player.Wager * 2;
            }
            else if (winner == dealer)
            {
                Console.WriteLine("Dealer wins the players wager of " + player.Wager);
                dealer.Money += player.Wager;
            }
            if (dealer.Money < 0)
            {
                Console.WriteLine("The casino is now somehow in debt!");
            }

            player.Wager = 0; //reset the wager
            EndSummary(player, dealer);
            Console.ReadKey();
            return;
        }
    }
}
