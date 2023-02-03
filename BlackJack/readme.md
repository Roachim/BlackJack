#jackblack
##Design
The program has been designed by OOP rules.
Objetcs such as a player, dealer, cards and decks are objects.
The deck is static, we only need one deck. How many cards it has is currently 52 - one of each card.
Dealer and Player are both using the Person interface, this lets them both use certain methods.
Game has all of the methods and game mechanics in there, with the exception of making a new deck, and giving out a card,
this is found in the static Deck object.

##game flow
Game runs once on default, having rounds in a loop till someone has too little money to continue.
A round consists of multiple turns, until someone cant draw or someone wins by default.
A turn consist of both player and dealer turns.

###summary
With the exception of 2, all methods can be found in "Game".

Player and Dealer are both using the "Person" interface so more generic methods can be made.

1 Game == Rounds * x. => Game ends when money of either Player or Dealer is insufficient.

1 Round == turns * y. => Round ends when neither Player nor Dealer wants to/can draw anymore cards.
A blackjack or bust will end the round immediately.

1 Turn == 1 playerTurn + 1 DealerTurn

###Missing
If deck runs out mid game - unnacounted for. Assumed someone will run out of money first

###ShowAndTell
- The classes
	* Person - player and dealer
- Game flow
	* Game
	* Runde
	* Turns
	* The checks

####Things to add
- Split hands
- money into chips => chips into money
- Multiple decks option
- Unity tests for passable funcitons

####Things to change
- Cleaner turn for declared winner calls
- Maybe a better name than "Money" for the currency

####Things to change ++
-Make in unity
