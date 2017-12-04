# War Card Game

## Synopsis

This solution contains a console version of the classic card game of War written in C#.

## Setup

To play the game, there are a few options available:
- Clone or download the repository, build the solution, and run from Visual Studio.
- Use the prebuilt release binaries in Run-Options\1-PrebuiltBinaries.  From a command prompt, run WarCardGame.exe.
- Use the published application in Run-Options\2-PublishedFromVisualStudio.  Run setup.exe.

## Gameplay

After starting the game, the program will prompt for your name and game mode.  

In an Auto game, the game will automatically run to completion.  In a Manual game, the game will step through each card played, and you must hit the space bar to play each hand.

When a game starts, a deck of cards is distributed between you and a computer player.  At the start of a round, each player reveals the top card in their personal deck.  The player with the highest card (aces high) wins the round and collects both cards.
If the cards have the same value, then it is time for war!  

At the start of war, an additional card from each player’s deck is added to the pot for the round, and then each player reveals the top card in their personal deck.

If a player runs out of cards in the middle of war, the player is unable to continue the round and loses the game.


