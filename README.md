# Image Guesser

## The game

This project is a single or multi-player game on recognizing images.
In single player mode you will be playing against an oracle.
In two/multi player mode, two/multiple players are paired in the beginning of the game -- The Proposer and the Guesser/s.

There are some games which are designed to be fun but also achieve something useful. Such games are called [games with a purpose (GWAP)](https://en.wikipedia.org/wiki/Human-based_computation_game)

There are some examples of such games like [Peekaboom](https://www.cs.cmu.edu/~biglou/Peekaboom.pdf)

### Points system

The fewer segments needed to guess the better it is for both the players. The score for both players is the number of uncovered segments until a successful guess. This score is then maintained in a leaderboard showing top players.

## Tools

### Microsoft Web LibraryManager Cli

Update client-side libraries with command: '$ libman restore'

## Useful commands

### Launch specific launch profiles

$ dotnet run --launch-profile "nameOfProfile"
