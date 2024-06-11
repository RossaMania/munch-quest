using System;

namespace MunchQuest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new instance of the game
            Game game = new Game();

            // Start the game
            game.Start();
        }
    }

    public class Game
    {
        // Random number generator for food placement and selection
        private Random random = new Random();

        // Flag to determine if the game should exit
        private bool shouldExit = false;

        // Variables to store the height and width of the console window
        private int height, width;

        // Variables to store the player's position
        private int playerX, playerY;

        // Variables to store the food's position
        private int foodX, foodY;

        // Arrays to store the different states of the player and food
        private string[] states = { "('-')", "(^-^)", "(X_X)" };
        private string[] foods = { "@@@@@", "$$$$$", "#####" };

        // Current player state
        private string player = "('-')";

        // Index of the current food
        private int food = 0;

        // Start the game loop
        public void Start()
        {
            // Initialize the game state
            InitializeGame();

            // Main game loop
            while (!shouldExit)
            {
                // Handle player movement
                Move();

                // Check if the terminal has been resized
                if (TerminalResized())
                {
                    // Clear the console and display a message if the terminal was resized
                    Console.Clear();
                    Console.WriteLine("Uh oh! Console was resized! Exiting game...");
                    break;
                }
            }
        }

        // Initialize the game state
        public void InitializeGame()
        {
            // Hide the cursor in the console
            Console.CursorVisible = false;

            // Set the height and width based on the current console window size
            height = Console.WindowHeight - 1;
            width = Console.WindowWidth - 5;

            // Clear the console
            Console.Clear();

            // Display the initial food
            ShowFood();

            // Display the initial player position
            Console.SetCursorPosition(0, 0);
            Console.Write(player);
        }

        // Check if the terminal has been resized
        public bool TerminalResized()
        {
            // Return true if the current console window size does not match the initial size
            return height != Console.WindowHeight - 1 || width != Console.WindowWidth - 5;
        }

        // Display random food at a random location
        public void ShowFood()
        {
            // Select a random food index
            food = random.Next(0, foods.Length);

            // Select random positions for the food
            foodX = random.Next(0, width - player.Length);
            foodY = random.Next(0, height - 1);

            // Set the cursor position and display the food
            Console.SetCursorPosition(foodX, foodY);
            Console.Write(foods[food]);
        }

        // Change the player state to match the food consumed
        public void ChangePlayer()
        {
            // Update the player state based on the food consumed
            player = states[food];

            // Set the cursor position and display the new player state
            Console.SetCursorPosition(playerX, playerY);
            Console.Write(player);

            // Freeze the player if they are in the "sick" state
            if (IsSick())
            {
                FreezePlayer();
            }
        }

        // Temporarily stop the player's movement
        public void FreezePlayer()
        {
            // Pause the game for 1 second
            System.Threading.Thread.Sleep(1000);

            // Reset the player state to normal
            player = states[0];
        }

        // Handle player movement based on keyboard input
        public void Move()
        {
            // Store the last known position of the player
            int lastX = playerX;
            int lastY = playerY;

            // Read a key from the console without displaying it
            var key = Console.ReadKey(true).Key;

            // Update the player's position based on the key pressed
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    playerY--;
                    break;
                case ConsoleKey.DownArrow:
                    playerY++;
                    break;
                case ConsoleKey.LeftArrow:
                    playerX--;
                    break;
                case ConsoleKey.RightArrow:
                    playerX++;
                    break;
                case ConsoleKey.Escape:
                    // Set the flag to exit the game if the Escape key is pressed
                    shouldExit = true;
                    break;
                default:
                    // Clear the console and display a message if an unsupported key is pressed
                    Console.Clear();
                    Console.WriteLine("Unsupported key entered. Exiting game...");
                    shouldExit = true;
                    return;
            }

            // If the player is in the "fast" state, increase movement speed
            if (IsFast() && (key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow))
            {
                playerX += (key == ConsoleKey.LeftArrow) ? -3 : 3;
            }

            // Clear the previous player position
            Console.SetCursorPosition(lastX, lastY);
            for (int i = 0; i < player.Length; i++)
            {
                Console.Write(" ");
            }

            // Keep the player within the bounds of the console window
            playerX = Math.Clamp(playerX, 0, width);
            playerY = Math.Clamp(playerY, 0, height);

            // Set the cursor position and display the player at the new position
            Console.SetCursorPosition(playerX, playerY);
            Console.Write(player);

            // Check if the player has consumed the food
            if (FoodIsEaten())
            {
                // Change the player state and display new food
                ChangePlayer();
                ShowFood();
            }
        }

        // Check if the player has consumed the food
        public bool FoodIsEaten()
        {
            return playerX == foodX && playerY == foodY;
        }

        // Check if the player is in the "sick" state
        public bool IsSick()
        {
            return states[food] == states[2];
        }

        // Check if the player is in the "fast" state
        public bool IsFast()
        {
            return states[food] == states[1];
        }

        // Check if the player is in the "normal" state
        public bool IsNormal()
        {
            return states[food] == states[0];
        }
    }
}