using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarCardGame.Enums;
using WarCardGame.Interfaces;

namespace WarCardGame.Views
{
    public class ConsoleGameView
    {
        private const string COMPUTER_PLAYER_NAME = "CPU";
        private const string GAME_MODE_REQUEST_MESSAGE = "Enter Game Mode (\"a\" for auto mode, \"m\" for manual mode): ";
        private const string INVALID_ENTRY = "Invalid entry.  Please try again.";
        private const string PLAYER_NAME_REQUEST_MESSAGE = "Enter Player Name: ";
        private const string PRESS_SPACE_BAR_TO_CONTINUE = "Press space bar to continue...";
        private const string PRESS_Q_TO_QUIT = "Press \"q\" to quit...";
        private const string WELCOME_MESSAGE = "Welcome to War!";
        private const string WINNER_MESSAGE_FORMAT = "{0} won the game!";

        private IWarGameController _warGameController;

        public ConsoleGameView(IWarGameController warGameController)
        {
            _warGameController = warGameController;
        }

        public void PlayGame()
        {
            Console.WriteLine(WELCOME_MESSAGE);
            var playerName = GetPlayerName();
            var gameMode = GetGameMode();
            _warGameController.Initialize(new List<string>() { playerName, COMPUTER_PLAYER_NAME });
            
            while (!_warGameController.IsGameOver())
            { 
                Console.WriteLine(_warGameController.GetGameStandings());
                WaitForSpaceBarIfNeeded(gameMode);
                Console.WriteLine();
                var roundResult = _warGameController.StartNewRound();
                Console.WriteLine(roundResult);
                while(roundResult.IsWarRequired)
                {
                    WaitForSpaceBarIfNeeded(gameMode);
                    Console.WriteLine();
                    roundResult = _warGameController.ContinueRoundWithWar();
                    Console.WriteLine(roundResult);
                }
            }

            Console.WriteLine();
            Console.WriteLine(string.Format(WINNER_MESSAGE_FORMAT, _warGameController.GetGameWinnerName()));
            Console.WriteLine();
            Console.WriteLine(PRESS_Q_TO_QUIT);
            WaitForKeyPress(ConsoleKey.Q);
        }    

        private GameMode GetGameMode()
        {
            var mode = GameMode.Unknown;

            while(mode == GameMode.Unknown)
            {
                Console.Write(GAME_MODE_REQUEST_MESSAGE);
                var result = Console.ReadLine();
                switch(result)
                {
                    case "a":
                        mode = GameMode.Auto;
                        break;
                    case "m":
                        mode = GameMode.Manual;
                        break;
                    default:
                        Console.WriteLine(INVALID_ENTRY);
                        break;
                }
            }

            return mode;
        }

        private string GetPlayerName()
        {
            var name = string.Empty;

            while (string.IsNullOrEmpty(name))
            {
                Console.Write(PLAYER_NAME_REQUEST_MESSAGE);
                name = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine(INVALID_ENTRY);
                }
            }

            return name;
        }
        
        private void WaitForKeyPress(ConsoleKey key)
        {
            while (Console.ReadKey(true).Key != key) { }
        }

        private void WaitForSpaceBarIfNeeded(GameMode mode)
        {
            if(mode == GameMode.Manual)
            {
                Console.WriteLine(PRESS_SPACE_BAR_TO_CONTINUE);
                WaitForKeyPress(ConsoleKey.Spacebar);                
            }            
        }
    }
}
